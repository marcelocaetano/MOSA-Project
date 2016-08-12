﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common;
using Mosa.Compiler.Linker;
using Mosa.Compiler.MosaTypeSystem;
using Mosa.Utility.BootImage;
using Mosa.Utility.DebugEngine;
using Mosa.Utility.Launcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Threading;

namespace Mosa.UnitTest.Engine
{
	public class UnitTestEngine : IBuilderEvent, IStarterEvent, IDisposable
	{
		public Options Options { get; private set; }
		public string TestAssemblyPath { get; set; }
		public string Platform { get; set; }
		public string TestSuiteFile { get; set; }
		public AppLocations AppLocations { get; set; }

		protected TypeSystem typeSystem;
		protected BaseLinker linker;
		protected string imagefile;
		protected Starter starter;
		protected Process process;
		protected DebugServerEngine debugServerEngine;

		private bool fatalError = false;
		private bool compiled = false;
		private bool processStarted = false;
		private int retries = 0;
		private bool restartVM = false;
		private bool imageSent = false;
		private volatile bool ready = false;

		private Stopwatch stopwatch = new Stopwatch();

		private const uint MaxRetries = 10;
		private const uint RetryDelay = 1; // 1- seconds

		private const int DefaultMaxSentQueue = 100;

		private Queue<DebugMessage> queue = new Queue<DebugMessage>();
		private HashSet<DebugMessage> sent = new HashSet<DebugMessage>();

		private int MaxSentQueue = DefaultMaxSentQueue;

		//private int SentQueueCountDown = 0;

		private Thread processThread;
		private volatile bool processThreadAbort = false;

		public UnitTestEngine()
		{
			Options = new Options()
			{
				EnableSSA = true,
				EnableIROptimizations = true,
				EnableVariablePromotion = true,
				EnableSparseConditionalConstantPropagation = true,
				Emulator = EmulatorType.Qemu,
				ImageFormat = ImageFormat.IMG,
				BootFormat = BootFormat.Multiboot_0_7,
				PlatformType = PlatformType.X86,
				LinkerFormatType = LinkerFormatType.Elf32,
				MemoryInMB = 128,
				DestinationDirectory = Path.Combine(Path.GetTempPath(), "MOSA-UnitTest"),
				FileSystem = FileSystem.FAT16,
				UseMultipleThreadCompiler = true,
				EnableInlinedMethods = true,
				InlinedIRMaximum = 8,
				BootLoader = BootLoader.Syslinux_3_72,
				VBEVideo = false,
				Width = 640,
				Height = 480,
				Depth = 32,
				BaseAddress = 0x00400000,
				EmitRelocations = false,
				EmitSymbols = false,
				Emitx86IRQMethods = true,
				DebugConnectionOption = DebugConnectionOption.TCPServer,
				DebugConnectionPort = 9999,
				ExitOnLaunch = true,
				GenerateASMFile = true,
				GenerateMapFile = true,
				BootLoaderImage = @"..\Tests\BootImage\Mosa.BootLoader.x86.img"
			};

			AppLocations = new AppLocations();

			AppLocations.FindApplications();

			Initialize();

			stopwatch.Start();

			processThread = new Thread(ProcessQueue);
			processThread.Name = "ProcesQueue";

			processThread.Start();
		}

		public void Initialize()
		{
			if (Platform == null)
				Platform = "x86";

			if (TestAssemblyPath == null)
				TestAssemblyPath = AppContext.BaseDirectory;

			if (TestSuiteFile == null)
				TestSuiteFile = "Mosa.UnitTests." + Platform + ".exe";
		}

		private void ProcessQueue()
		{
			try
			{
				while (!processThreadAbort)
				{
					DebugMessage message = null;

					lock (queue)
					{
						// check if queue has requests or too many have already been sent
						if (queue.Count <= 0 || sent.Count > MaxSentQueue)
						{
							Thread.Sleep(5);
							continue;
						}

						message = queue.Dequeue();
					}

					PrepareUnitTest();

					message.Other = message;
					message.CallBack = MessageCallBack;

					debugServerEngine.SendCommand(message);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		private void MessageCallBack(DebugMessage response)
		{
			if (response == null)
				return;

			lock (queue)
			{
				//Console.WriteLine(response.ToString());

				sent.Remove(response);
			}

			var message = response.Other as UnitTestRequest;

			if (message != null)
			{
				message.ParseResultData(response.ResponseData);
			}
		}

		private void QueueMessage(DebugMessage request)
		{
			lock (queue)
			{
				queue.Enqueue(request);
			}
		}

		private bool IsQueueEmpty
		{
			get
			{
				lock (queue)
				{
					return queue.Count == 0 && sent.Count == 0;
				}
			}
		}

		public T Run<T>(string ns, string type, string method, params object[] parameters)
		{
			CheckCompiled();

			var request = new UnitTestRequest(ns, type, method, parameters);

			request.Resolve(typeSystem, linker);

			var message = new DebugMessage(DebugCode.ExecuteUnitTest, request.Message);

			QueueMessage(message);

			while (!request.HasResult)
			{
				Thread.Sleep(5); // temporary
			}

			var result = request.Result;

			if (request.RuntimeMethod.Signature.ReturnType.IsVoid)
				return default(T);

			try
			{
				if (default(T) is ValueType)
					return (T)result;
				else
					return default(T);
			}
			catch (InvalidCastException e)
			{
				Debug.Assert(false, String.Format("Failed to convert result {0} of destination {1} destination type {2}.", result, result.GetType(), typeof(T).ToString()));
				throw e;
			}
		}

		private bool CheckCompiled()
		{
			lock (this)
			{
				if (compiled)
					return true;

				if (fatalError)
					return false;

				Compile();

				return compiled;
			}
		}

		private void Compile()
		{
			Options.SourceFile = Path.Combine(TestAssemblyPath, TestSuiteFile);

			var builder = new Builder(Options, AppLocations, this);

			builder.Compile();

			linker = builder.Linker;
			typeSystem = builder.TypeSystem;
			imagefile = Options.BootLoaderImage; // builder.ImageFile;

			fatalError = builder.HasCompileError;
			compiled = !fatalError;
		}

		public void LaunchVirtualMachine()
		{
			if (starter == null)
			{
				starter = new Starter(Options, AppLocations, imagefile, this);
			}

			process = starter.Launch();

			if (process == null)
				fatalError = true;
			else if (process.HasExited)
				fatalError = true;

			processStarted = !fatalError;
			ready = false;
			imageSent = false;
		}

		public void ConnectToDebugEngine()
		{
			if (debugServerEngine == null)
			{
				debugServerEngine = new DebugServerEngine();
				debugServerEngine.SetGlobalDispatch(GlobalDispatch);
			}

			while (!debugServerEngine.IsConnected)
			{
				if (retries > MaxRetries)
				{
					fatalError = true;
					return;
				}

				retries++;
				ready = false;
				imageSent = false;

				try
				{
					Connect();
				}
				catch
				{
					Thread.Sleep(100);
				}
			}
		}

		private void Connect()
		{
			if (!debugServerEngine.IsConnected)
			{
				if (Options.DebugConnectionOption == DebugConnectionOption.TCPServer)
				{
					var client = new TcpClient(Options.DebugConnectionAddress, Options.DebugConnectionPort);
					debugServerEngine.Stream = new DebugNetworkStream(client.Client, true);
				}
				else if (Options.DebugConnectionOption == DebugConnectionOption.Pipe)
				{
					var pipeStream = new NamedPipeClientStream(".", Options.DebugPipeName, PipeDirection.InOut);
					pipeStream.Connect();
					debugServerEngine.Stream = pipeStream;
				}
			}
		}

		private void RestartVirtualMachine()
		{
			if (process != null)
			{
				process.Kill();
				process.WaitForExit(5000); // wait for up to 5 seconds

				if (!process.HasExited)
					fatalError = true;
			}

			process = null;
			restartVM = false;
			processStarted = false;
		}

		private void WaitForReady()
		{
			if (ready)
				return;

			//todo --- add timeouts

			while (!ready)
			{
				Thread.Sleep(10);
			}
		}

		private volatile bool wait = false;

		public void SendMessageAndWait(DebugMessage message)
		{
			wait = true;

			message.CallBack = Acknowledge;

			debugServerEngine.SendCommand(message);

			while (wait)
			{
				Thread.Sleep(5);
			}
		}

		public void Acknowledge(DebugMessage message)
		{
			wait = false;
		}

		public void SendImage()
		{
			var bss = linker.LinkerSections[(int)SectionKind.BSS];

			var message = new DebugMessage(DebugCode.ClearMemory, new int[] { (int)bss.VirtualAddress, (int)bss.Size });

			SendMessageAndWait(message);

			foreach (var section in linker.LinkerSections)
			{
				if (section.SectionKind == SectionKind.BSS)
					continue;

				var stream = new MemoryStream((int)section.Size);

				// similar code in the Section.WriteTo method
				foreach (var symbol in section.Symbols)
				{
					stream.Seek(symbol.SectionOffset, SeekOrigin.Begin);
					if (symbol.IsDataAvailable)
					{
						symbol.Stream.Position = 0;
						symbol.Stream.WriteTo(stream);
					}
				}

				stream.WriteZeroBytes((int)(section.AlignedSize - stream.Position));
				stream.Position = 0;

				var array = stream.ToArray();
				uint at = 0;

				while (at < array.Length)
				{
					uint size = (uint)array.Length - at;

					if (size > 1024) size = 1024;

					var bytes = new byte[size + 8];

					Array.Copy(array, at, bytes, 8, size);

					uint address = (uint)(section.VirtualAddress + at);
					Console.WriteLine(section.SectionKind.ToString() + " : 0x" + address.ToString("X"));

					bytes[0] = (byte)(address & 0xFF);
					bytes[1] = (byte)((address >> 8) & 0xFF);
					bytes[2] = (byte)((address >> 16) & 0xFF);
					bytes[3] = (byte)((address >> 24) & 0xFF);

					bytes[4] = (byte)(size & 0xFF);
					bytes[5] = (byte)((size >> 8) & 0xFF);
					bytes[6] = (byte)((size >> 16) & 0xFF);
					bytes[7] = (byte)((size >> 24) & 0xFF);

					message = new DebugMessage(DebugCode.WriteMemory, bytes);

					SendMessageAndWait(message);

					at = at + size;
				}
			}
		}

		public void SendImage2()
		{
			var bss = linker.LinkerSections[(int)SectionKind.BSS];

			var message = new DebugMessage(DebugCode.ClearMemory, new int[] { (int)bss.VirtualAddress, (int)bss.Size });

			SendMessageAndWait(message);

			foreach (var symbol in linker.Symbols)
			{
				Console.WriteLine(symbol.Name);

				if (symbol.SectionKind == SectionKind.BSS)
					continue;

				if (symbol.Size == 0)
					continue;

				int address = (int)symbol.VirtualAddress;
				int size = (int)symbol.Size;

				symbol.Stream.Position = 0;

				int left = size;

				while (left > 0)
				{
					if (size > 2048 / 4)
						size = 2048 / 4;

					left = left - size;

					var bytes = new List<byte>(size + 8);

					bytes.Add((byte)(address & 0xFF));
					bytes.Add((byte)((address >> 8) & 0xFF));
					bytes.Add((byte)((address >> 16) & 0xFF));
					bytes.Add((byte)((address >> 24) & 0xFF));

					bytes.Add((byte)(size & 0xFF));
					bytes.Add((byte)((size >> 8) & 0xFF));
					bytes.Add((byte)((size >> 16) & 0xFF));
					bytes.Add((byte)((size >> 24) & 0xFF));

					address = address + size;

					while (size > 0)
					{
						var b = symbol.Stream.ReadByte();

						bytes.Add((byte)b);

						size--;
					}

					message = new DebugMessage(DebugCode.WriteMemory, bytes);

					SendMessageAndWait(message);
				}
			}

			imageSent = true;
			return;
		}

		public bool PrepareUnitTest()
		{
			lock (this)
			{
				if (fatalError)
					return false;

				if (restartVM)
				{
					RestartVirtualMachine();
				}

				if (fatalError)
					return false;

				if (!processStarted)
				{
					LaunchVirtualMachine();
				}

				if (fatalError)
					return false;

				ConnectToDebugEngine();

				if (fatalError)
					return false;

				if (!ready)
				{
					WaitForReady();
				}

				if (ready && !imageSent)
				{
					SendImage();
				}

				if (fatalError)
					return false;

				return true;
			}
		}

		private void GlobalDispatch(DebugMessage response)
		{
			if (response == null)
				return;

			if (response.Code == DebugCode.Ready)
			{
				ready = true;
			}

			//Console.WriteLine(response.ToString());
		}

		void IBuilderEvent.NewStatus(string status)
		{
			//Console.WriteLine(status);
		}

		void IBuilderEvent.UpdateProgress(int total, int at)
		{
		}

		void IStarterEvent.NewStatus(string status)
		{
			//Console.WriteLine(status);
		}

		void IDisposable.Dispose()
		{
			processThreadAbort = true;

			if (processThread != null)
			{
				if (processThread.IsAlive)
				{
					processThread.Abort();
				}
			}

			if (process == null)
				return;

			if (process.HasExited)
				return;

			process.Kill();

			process = null;
		}
	}
}

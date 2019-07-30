﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System;

namespace Mosa.Kernel.BareMetal
{
	public static class Platform
	{
		// These methods will be plugged and implemented elsewhere in the platform specific implementation

		public static uint GetPageShift()
		{
			return 0;
		}

		public static void EntryPoint()
		{
		}

		public static AddressRange GetBootReservedRegion()
		{
			return new AddressRange(0, 0);
		}

		public static void UpdateBootMemoryMap()
		{
		}

		public static AddressRange GetInitialGCMemoryPool()
		{
			return new AddressRange(0, 0);
		}

		public static void PageTableSetup()
		{ }

		public static void PageTableInitialize()
		{ }

		public static void PageTableEnable()
		{ }

		public static void PageTableMapVirtualAddressToPhysical(uint virtualAddress, uint physicalAddress, bool present = true)
		{ }

		public static IntPtr PageTableGetPhysicalAddressFromVirtual(IntPtr virtualAddress)
		{
			return IntPtr.Zero;
		}

		public static void ConsoleWrite(byte c)
		{ }
	}
}

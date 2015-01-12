/*
* (c) 2008 MOSA - The Managed Operating System Alliance
*
* Licensed under the terms of the New BSD License.
*
* Authors:
*  Phil Garcia (tgiphil) <phil@thinkedge.com>
*/

using System.Collections.Generic;

namespace Mosa.DeviceSystem
{
	/// <summary>
	///
	/// </summary>
	public class PCIControllerManager
	{
		/// <summary>
		///
		/// </summary>
		protected IDeviceManager deviceManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="PCIControllerManager"/> class.
		/// </summary>
		/// <param name="deviceManager">The device manager.</param>
		public PCIControllerManager(IDeviceManager deviceManager)
		{
			this.deviceManager = deviceManager;
		}

		/// <summary>
		/// Probes for a PCI device.
		/// </summary>
		/// <param name="pciController">The pci controller.</param>
		/// <param name="bus">The bus.</param>
		/// <param name="slot">The slot.</param>
		/// <param name="fun">The fun.</param>
		/// <returns></returns>
		protected bool ProbeDevice(IPCIController pciController, byte bus, byte slot, byte fun)
		{
			uint value = pciController.ReadConfig32(bus, slot, fun, 0);
			//HAL.DebugWrite(": " + value.ToString("x"));
			return value != 0xFFFFFFFF;
		}

		/// <summary>
		/// Creates the partition devices.
		/// </summary>
		public void CreatePCIDevices()
		{
			// Find PCI controller devices
			LinkedList<IDevice> devices = deviceManager.GetDevices(new FindDevice.IsPCIController(), new FindDevice.IsOnline());

			// For each controller
			foreach (IDevice device in devices)
			{
				IPCIController pciController = device as IPCIController;

				for (int bus = 0; bus < 255; bus++)
				{
					for (int slot = 0; slot < 16; slot++)
					{
						for (int fun = 0; fun < 7; fun++)
						{
							if (ProbeDevice(pciController, (byte)bus, (byte)slot, (byte)fun))
							{
								deviceManager.Add(new Mosa.DeviceSystem.PCI.PCIDevice(pciController, (byte)bus, (byte)slot, (byte)fun));
							}
						}
					}
				}
			}
		}
	}
}

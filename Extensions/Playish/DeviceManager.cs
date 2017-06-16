using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;


namespace Playish
{
	public class DeviceManager
	{
		// Singleton
		private static DeviceManager currentInstance = null;

		// Devices
		/// <summary>
		/// The connected devices to the console.
		/// </summary>
		public Dictionary<String, Device> devices = new Dictionary<String, Device> ();

		// Events
		public delegate void DevicesChangedHandler (DeviceEventArgs e);
		public event DevicesChangedHandler deviceAddedEvent;
		public event DevicesChangedHandler deviceRemovedEvent;
		public event DevicesChangedHandler deviceChangedEvent;


		// ---- MARK: Setup

		private DeviceManager()
		{}

		public static DeviceManager getInstance()
		{
			if (currentInstance == null)
			{
				currentInstance = new DeviceManager ();
			}
			return currentInstance;
		}


		// ---- MARK: Manage devices

		public void addDevice(Device newDevice)
		{
			if (!devices.ContainsKey (newDevice.getDeviceId ()))
			{
				devices.Add (newDevice.getDeviceId (), newDevice);
				onDeviceAdded (new DeviceEventArgs(newDevice.getDeviceId()));
			}
			else
			{
				devices [newDevice.getDeviceId ()] = newDevice;
			}
		}

		public void removeDevice(String deviceId)
		{
			if (devices.ContainsKey (deviceId))
			{
				devices.Remove (deviceId);
				onDeviceRemoved (new DeviceEventArgs(deviceId));
			}
		}

		public Device getDevice(String deviceId)
		{
			if (devices.ContainsKey (deviceId))
			{
				return devices [deviceId];
			}
			return null;
		}

		public void clearDevices()
		{
			devices.Clear ();
			onDeviceRemoved (new DeviceEventArgs(""));
		}


		// ---- MARK: Helper functions

		public void changeControllerForAll(Controller controller)
		{
			var keys = devices.Keys.ToArray ();

			for (int i = 0; i < keys.Length; i++)
			{
				var item = devices [keys [i]];
				item.setController (controller);
			}
		}


		// ---- MARK: Events

		private void onDeviceAdded (DeviceEventArgs e)
		{
			if (deviceAddedEvent != null) deviceAddedEvent (e);
			onDevicesChanged (e);
		}

		private void onDeviceRemoved (DeviceEventArgs e)
		{
			if (deviceRemovedEvent != null) deviceRemovedEvent (e);
			onDevicesChanged (e);
		}

		private void onDevicesChanged(DeviceEventArgs e)
		{
			e.deviceId = "";
			if (deviceChangedEvent != null) deviceChangedEvent (e);
		}


		// ---- MARK: Related definitions

		public class DeviceEventArgs : EventArgs
		{
			public String deviceId = "";

			public DeviceEventArgs(String deviceId)
			{
				this.deviceId = deviceId;
			}
		}
	}
}

using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using PlayishUnityHost;


namespace Playish
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class PlayishManager : MonoBehaviour
	{
		// General
		private static PlayishManager currentInstance = null;
		private const String VERSION = "host_0.1.1";
		private bool hasSentPluginLoaded = false;
		

		// Events
		public delegate void PlayishStateChangedHandler (EventArgs e);
		/// <summary>
		/// Occurs when a playish console pauses the game.
		/// </summary>
		public event PlayishStateChangedHandler playishPauseEvent;
		/// <summary>
		/// Occurs when a playish console sends remove event to the game. Make sure your game handles 
		/// resume on it's own regardless of this event.
		/// </summary>
		public event PlayishStateChangedHandler playishResumeEvent;


		// ---- MARK: Setup

		private void Awake()
		{
			if (currentInstance == null)
			{
				currentInstance = this;
			}
			else if (this.GetHashCode () != currentInstance.GetHashCode ())
			{
				writeWebConsoleLog ("InvalidPlayishManager");
				name = "InvalidPlayishManager";
				Destroy (transform.gameObject);
				return;
			}

			DeviceManager.getInstance ().clearDevices ();
		}

		private void Start()
		{
			if (currentInstance != null && this.GetHashCode () != currentInstance.GetHashCode ())
			{
				return;
			}

			if (Application.isPlaying)
			{
				DontDestroyOnLoad (transform.gameObject);
			}
		}

		public static PlayishManager getInstance()
		{
			if (currentInstance == null)
			{
				var playishManager = new GameObject ("PlayishManager");
				currentInstance = playishManager.AddComponent<PlayishManager> ();
			}
			return currentInstance;
		}


		// ---- MARK: Update

		private void Update()
		{
			#if UNITY_EDITOR
			/**
			 * Making sure that the manager has the correct name.
			 * Otherwise it wont be able to be called from the playish website.
			 */
			if (name != "PlayishManager")
			{
				name = "PlayishManager";
			}
			#endif

			if (!hasSentPluginLoaded && this.GetHashCode () == currentInstance.GetHashCode ())
			{
				hasSentPluginLoaded = true;
				setUnityPlayishPluginLoaded ();
			}
		}


		// ---- MARK: Ingoing communication

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onPause()
		{
			if (playishPauseEvent != null)
			{
				playishPauseEvent (new EventArgs());
			}
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onResume()
		{
			if (playishResumeEvent != null)
			{
				playishResumeEvent (new EventArgs());
			}
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onDeviceInput(string data)
		{
			parseAndSetInput (data);
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onDeviceSync(string data)
		{
			var deviceManager = DeviceManager.getInstance ();

			var deviceIds = data.Split (new char[]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
			var existingDeviceIds = deviceManager.devices.Keys.ToList ();

			for (int i = 0; i < deviceIds.Length; i++)
			{
				var deviceIdElement = deviceIds [i];
				existingDeviceIds.Remove (deviceIdElement);

				if (!deviceManager.devices.ContainsKey (deviceIdElement))
				{
					var device = new Device (deviceIdElement);
					deviceManager.addDevice (device);
				}
			}

			for (int i = 0; i < existingDeviceIds.Count; i++)
			{
				deviceManager.removeDevice (existingDeviceIds[i]);
			}
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onDeviceConnected(string data)
		{
			if (data.Length > 0)
			{
				if (!DeviceManager.getInstance ().devices.ContainsKey (data))
				{
					var device = new Device (data);
					DeviceManager.getInstance ().addDevice (device);
				}
			}
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onDeviceDisconnected(string data)
		{
			if (data.Length > 0)
			{
				DeviceManager.getInstance ().removeDevice (data);
			}
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onControllerChanged(string data)
		{
			var controllerTuple = parseControllerDefinition (data);
			if (controllerTuple.deviceId != "" && controllerTuple.controller != null)
			{
				var device = DeviceManager.getInstance ().getDevice (controllerTuple.deviceId);
				if (device != null)
				{
					device.setController (controllerTuple.controller);
				}
			}
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onControllerChangedForAll(string data)
		{
			Controller controller = JsonUtility.FromJson<Controller> (data);
			if (controller != null)
			{
				DeviceManager.getInstance ().changeControllerForAll (controller);
			}
		}


		// ---- MARK: Outgoing communication

		/// <summary>
		/// Sends to the console that the plugin was loaded and is ready to be used by the console. Should not be called directly.
		/// </summary>
		public void setUnityPlayishPluginLoaded()
		{
#if UNITY_WEBGL
			Application.ExternalCall("PlayishUnityPluginLoaded", VERSION);
#endif

#if UNITY_EDITOR
			ServerConnection.getInstance().PlayishUnityPluginLoaded(VERSION);
			#endif
		}

		/// <summary>
		/// Changes the controller for a specific device. Not implemented in this plugin-version, will become available in the future.
		/// </summary>
		public void changeController(String deviceId, String controllerName)
		{
#if UNITY_WEBGL
			Application.ExternalCall("PlayishUnityChangeController", deviceId, controllerName);
#endif

#if UNITY_EDITOR
			ServerConnection.getInstance().PlayishUnityChangeController(deviceId, controllerName);
			#endif
		}

		/// <summary>
		/// Changes the controller for all connected devices.
		/// </summary>
		public void changeController(String controllerName)
		{
#if UNITY_WEBGL
			Application.ExternalCall("PlayishUnityChangeControllerForAll", controllerName);
#endif

#if UNITY_EDITOR
			ServerConnection.getInstance().PlayishUnityChangeControllerForAll(controllerName);
			#endif
		}

		public void writeWebConsoleLog(String message)
		{
#if UNITY_WEBGL
			Application.ExternalCall("PlayishUnityConsoleLog", message);
#endif

#if UNITY_EDITOR
			ServerConnection.getInstance().PlayishUnityConsoleLog(message);
			#endif
		}


		// ---- MARK: Parseing

		public DeviceIdControllerTuple parseControllerDefinition(String JSONController)
		{
			int deviceIdEndIndex = JSONController.IndexOf (';');
			if (deviceIdEndIndex < 1 || JSONController.Length <= deviceIdEndIndex + 1)
			{
				return null;
			}
			String deviceId = JSONController.Substring (0, deviceIdEndIndex);
			Controller controller = JsonUtility.FromJson<Controller> (JSONController.Substring (deviceIdEndIndex + 1));

			return new DeviceIdControllerTuple (deviceId, controller);
		}

		public void parseAndSetInput(String input) // "deviceId;00000000casd"
		{
			int deviceIdEndIndex = input.IndexOf (';');
			if (deviceIdEndIndex < 1 || input.Length <= deviceIdEndIndex + 1)
			{
				return;
			}

			String deviceId = input.Substring (0, deviceIdEndIndex);

			var device = DeviceManager.getInstance ().getDevice (deviceId);
			if (device != null)
			{
				device.parseAndSetInput (input, deviceIdEndIndex);
			}
		}

		public String parseDeviceId(String data)
		{
			int deviceIdEndIndex = data.IndexOf (';');
			if (deviceIdEndIndex > 0)
			{
				return data.Substring (0, deviceIdEndIndex);
			}
			return null;
		}


		// ---- MARK: Related definitions

		public class DeviceIdControllerTuple
		{
			public String deviceId = "";
			public Controller controller = null;

			public DeviceIdControllerTuple(String deviceId, Controller controller)
			{
				this.deviceId = deviceId;
				this.controller = controller;
			}
		}
	}
}

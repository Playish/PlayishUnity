using UnityEngine;
using System.Collections;
using System;
using System.Linq;


namespace Playish
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class PlayishManager : MonoBehaviour
	{
		// General
		private static PlayishManager currentInstance = null;
		private const String VERSION = "0.1";

		// Events
		public delegate void PlayishStateChangedHandler (EventArgs e);
		/// <summary>
		/// Occurs when a playish console pauses the game.
		/// </summary>
		public event PlayishStateChangedHandler playishPauseEvent;
		/// <summary>
		/// Occurs when a playish console resumes the game.
		/// </summary>
		public event PlayishStateChangedHandler playishResumeEvent;

		// Keep track of playish state
		private bool consoleIsPaused = false;


		// ---- MARK: Setup

		private PlayishManager()
		{
			currentInstance = this;
		}

		private void Awake()
		{
			PlayerManager.getInstance ().clearPlayers ();
		}

		private void Start()
		{
			setUnityPlayishPluginLoaded ();
		}

		public static PlayishManager getInstance()
		{
			/*
			if (currentInstance == null)
			{
				currentInstance = new PlayishManager ();
			}
			*/
			return currentInstance;
		}


		// ---- MARK: Playish state

		/// <summary>
		/// Gets if the console is paused. "playishPauseEvent" and "playishResumeEvent" can also be 
		/// helpful to keep track of the console state.
		/// </summary>
		public bool isConsolePaused()
		{
			return consoleIsPaused;
		}


		// ---- MARK: Update

		#if UNITY_EDITOR
		private void Update()
		{
			/**
			 * Making sure that the manager has the correct name.
			 * Otherwise it wont be able to be called from the playish website.
			 */
			if (name != "PlayishManager")
			{
				name = "PlayishManager";
			}
		}
		#endif


		// ---- MARK: Ingoing communication

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onPause()
		{
			consoleIsPaused = true;
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
			consoleIsPaused = false;
			if (playishResumeEvent != null)
			{
				playishResumeEvent (new EventArgs());
			}
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onControllerInput(string data)
		{
			parseAndSetInput (data);
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onPlayerSync(string data)
		{
			var playerManager = PlayerManager.getInstance ();

			var deviceIds = data.Split (new char[]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
			var existingDeviceIds = playerManager.players.Keys.ToList ();

			for (int i = 0; i < deviceIds.Length; i++)
			{
				var playerDeviceId = deviceIds [i];
				existingDeviceIds.Remove (playerDeviceId);

				if (!playerManager.players.ContainsKey (playerDeviceId))
				{
					var player = new Player (playerDeviceId);
					playerManager.addPlayer (player);
				}
			}

			for (int i = 0; i < existingDeviceIds.Count; i++)
			{
				playerManager.removePlayer (existingDeviceIds[i]);
			}
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onPlayerConnected(string data)
		{
			if (data.Length > 0)
			{
				if (!PlayerManager.getInstance ().players.ContainsKey (data))
				{
					var player = new Player (data);
					PlayerManager.getInstance ().addPlayer (player);
				}
			}
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onPlayerDisconnected(string data)
		{
			if (data.Length > 0)
			{
				PlayerManager.getInstance ().removePlayer (data);
			}
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onControllerChanged(string data)
		{
			var controllerTuple = parseController (data);
			if (controllerTuple.deviceId != "" && controllerTuple.controller != null)
			{
				var player = PlayerManager.getInstance ().getPlayer (controllerTuple.deviceId);
				if (player != null)
				{
					player.setController (controllerTuple.controller);
				}
			}
		}

		/// <summary>
		/// Should not be called directly. Used by the browser to communicate to the game.
		/// </summary>
		public void onControllerChangedForAll(string data)
		{
			DynamicController controller = JsonUtility.FromJson<DynamicController> (data);
			if (controller != null)
			{
				PlayerManager.getInstance ().changeControllerForAll (controller);
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
		}

		/// <summary>
		/// Changes the controller for a specific device. Not implemented in this plugin-version, will become available in the future.
		/// </summary>
		public void changeController(String deviceId, String controllerName)
		{
			#if UNITY_WEBGL

			Application.ExternalCall("PlayishUnityChangeController", deviceId, controllerName);

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
		}

		public void writeWebConsoleLog(String message)
		{
			#if UNITY_WEBGL

			Application.ExternalCall("PlayishUnityConsoleLog", message);

			#endif
		}


		// ---- MARK: Parseing

		public DeviceIdControllerTuple parseController(String JSONController)
		{
			int deviceIdEndIndex = JSONController.IndexOf (';');
			if (deviceIdEndIndex < 1 || JSONController.Length <= deviceIdEndIndex + 1)
			{
				return null;
			}
			String deviceId = JSONController.Substring (0, deviceIdEndIndex);
			DynamicController controller = JsonUtility.FromJson<DynamicController> (JSONController.Substring (deviceIdEndIndex + 1));

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

			var player = PlayerManager.getInstance ().getPlayer (deviceId);
			if (player != null)
			{
				player.parseAndSetInput (input, deviceIdEndIndex);
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
			public DynamicController controller = null;

			public DeviceIdControllerTuple(String deviceId, DynamicController controller)
			{
				this.deviceId = deviceId;
				this.controller = controller;
			}
		}
	}
}

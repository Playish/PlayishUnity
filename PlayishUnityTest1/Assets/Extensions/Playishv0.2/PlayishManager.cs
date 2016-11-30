using UnityEngine;
using System.Collections;
using System;


namespace Playish
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class PlayishManager : MonoBehaviour
	{
		private static PlayishManager currentInstance = null;


		// ---- MARK: Setup

		private PlayishManager()
		{}

		private void Awake()
		{
			currentInstance = new PlayishManager ();
			PlayerManager.getInstance ().clearPlayers ();
		}

		private void Start()
		{
			setUnityReadyForInput ();
		}

		public static PlayishManager getInstance()
		{
			if (currentInstance == null)
			{
				currentInstance = new PlayishManager ();
			}
			return currentInstance;
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

		public void onControllerInput(string data)
		{
			parseAndSetInput (data);
		}

		public void onPlayerSync(string data)
		{
			var deviceIds = data.Split (new char[';'], StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < deviceIds.Length; i++)
			{
				if (!PlayerManager.getInstance ().players.ContainsKey (deviceIds[i]))
				{
					var player = new Player (data);
					PlayerManager.getInstance ().addPlayer (player);
				}
			}
		}

		public void onPlayerConnected(string data)
		{
			/*
			var controllerTuple = parseController (data);
			if (controllerTuple.deviceId != "" && controllerTuple.controller != null)
			{
				var player = new Player (controllerTuple.deviceId);
				player.setController (controllerTuple.controller);
				PlayerManager.getInstance ().addPlayer (player);
			}
			*/

			if (data.Length > 0)
			{
				if (!PlayerManager.getInstance ().players.ContainsKey (data))
				{
					var player = new Player (data);
					PlayerManager.getInstance ().addPlayer (player);
				}
			}
		}

		public void onPlayerDisconnected(string data)
		{
			if (data.Length > 0)
			{
				PlayerManager.getInstance ().removePlayer (data);
			}
		}

		public void onControllerChanged(string data)
		{
			var controllerTuple = parseController (data);
			if (controllerTuple.deviceId != "" && controllerTuple.controller != null)
			{
				/*
				var player = new Player (controllerTuple.deviceId);
				player.setController (controllerTuple.controller);
				PlayerManager.getInstance ().addPlayer (player);
				*/

				var player = PlayerManager.getInstance ().getPlayer (controllerTuple.deviceId);
				if (player != null)
				{
					player.setController (controllerTuple.controller);
				}
			}
		}

		public void onControllerChangedForAll(string data)
		{
			DynamicController controller = JsonUtility.FromJson<DynamicController> (data);
			if (controller != null)
			{
				PlayerManager.getInstance ().changeControllerForAll (controller);
			}
		}


		// ---- MARK: Outgoing communication

		public void setUnityReadyForInput()
		{
			#if UNITY_WEBGL

			Application.ExternalCall("PlayishUnityReadyForInput");

			#endif
		}

		public void changeController(String deviceId, String controllerName)
		{
			#if UNITY_WEBGL

			// Will be implemented in future updates!
			//Application.ExternalCall("PlayishUnityChangeController", deviceId, controllerName);

			#endif
		}

		public void changeController(String controllerName)
		{
			#if UNITY_WEBGL

			Application.ExternalCall("PlayishUnityChangeControllerForAll", controllerName);

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

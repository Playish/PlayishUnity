using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

using UnityEditor.SceneManagement;

namespace Playish
{
	internal class PlayishHostWindow : EditorWindow
	{
		// Login
		public string username = "";
		public string password = "";

		// Game
		public SceneAsset initialSceneAsset = null;
		public SceneAsset playishSetupSceneAsset = null;
		public string gameslug = "";

		// Console
		public string statusText = "Not connected";
		public int connectedDevices = 0;

		// Errors
		public bool showError = false;
		public string errorMessage = "";

		// Stored info
		private const string consoleHostUsernamePrefsKey = "consoleHostUsernamePrefsKey";
		private const string consoleHostSceneNamePrefsKey = "consoleHostSceneNamePrefsKey";
		private const string consoleHostGameslugPrefsKey = "consoleHostGameslugPrefsKey";
		private const string consoleHostDeviceIdPrefsKey = "consoleHostDeviceIdPrefsKey";
		public string consoleHostDeviceId = "";

		// Debugging
		private bool isDebugging = false;
		private string debugButtonText = "";
		private string lastSceneName = "";
		private string lastScenePath = "";


		// ---- MARK: MenuItem and window UI

		[MenuItem("Window/Playish Host")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(PlayishHostWindow), false, "Playish Host", true);
		}

		private void OnGUI()
		{
			if (debugButtonText == "")
			{
				updateDebugButtonText();
			}

			var enableInput = !Application.isPlaying;

			GUILayout.Label("Login", EditorStyles.boldLabel);

			GUI.enabled = enableInput;
			username = EditorGUILayout.TextField("Email:", username);
			password = EditorGUILayout.PasswordField("Password:", password);
			GUI.enabled = true;
			EditorGUILayout.Space();

			GUILayout.Label("Game", EditorStyles.boldLabel);
			GUI.enabled = enableInput;
			initialSceneAsset = (SceneAsset)EditorGUILayout.ObjectField("Initial scene:", initialSceneAsset, typeof(SceneAsset), true);
			playishSetupSceneAsset = (SceneAsset)EditorGUILayout.ObjectField("Playish setup scene:", playishSetupSceneAsset, typeof(SceneAsset), true);


			gameslug = EditorGUILayout.TextField("Game slug:", gameslug);
			GUI.enabled = true;

			if (!enableInput)
			{
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Provided input is used to log in and create a session. To change information turn off play mode.", MessageType.Info, true);
			}

			if (showError)
			{
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox(errorMessage, MessageType.Error, true);
			}
			EditorGUILayout.Space();

			GUILayout.Label("Console host", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Connection status:", statusText);
			EditorGUILayout.LabelField("Connected devices:", connectedDevices.ToString());

			// Button to start debugging
			EditorGUILayout.Space();
			GUI.enabled = (enableInput || (!enableInput && isDebugging));
			if (GUILayout.Button(debugButtonText))
			{
				if (!isDebugging)
				{
					startDebugging();
				}
				else
				{
					stopDebugging();
				}
			}
		}

		private void Awake()
		{
			var savedDeviceId = EditorPrefs.GetString(consoleHostDeviceIdPrefsKey, "");
			if (!EditorPrefs.HasKey(consoleHostDeviceIdPrefsKey) || savedDeviceId.Length == 0)
			{
				savedDeviceId = PlayishUnityHost.Tools.GenerateDeviceId();
				EditorPrefs.SetString(consoleHostDeviceIdPrefsKey, savedDeviceId);
			}
			consoleHostDeviceId = savedDeviceId;
			username = EditorPrefs.GetString(consoleHostUsernamePrefsKey, "");
			//initialSceneName = EditorPrefs.GetString(consoleHostSceneNamePrefsKey, "");
			gameslug = EditorPrefs.GetString(consoleHostGameslugPrefsKey, "");

			updateDebugButtonText();
		}

		private void OnDestroy()
		{
			EditorPrefs.SetString(consoleHostDeviceIdPrefsKey, consoleHostDeviceId);
			EditorPrefs.SetString(consoleHostUsernamePrefsKey, username);
			//EditorPrefs.SetString(consoleHostSceneNamePrefsKey, initialSceneName);
			EditorPrefs.SetString(consoleHostGameslugPrefsKey, gameslug);
		}


		// ---- MARK: Handle debugging

		private void updateDebugButtonText()
		{
			if (isDebugging)
			{
				debugButtonText = "Stop debugging";
			}
			else
			{
				debugButtonText = "Start debugging";
			}
		}

		private void startDebugging()
		{
			isDebugging = true;
			updateDebugButtonText();

			var currentScene = EditorSceneManager.GetActiveScene();
			lastScenePath = currentScene.path;
			lastSceneName = currentScene.name;

			if (lastSceneName == "playish_host_setup")
			{
				EditorApplication.isPlaying = true;
				return;
			}

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

			EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(playishSetupSceneAsset));
			EditorApplication.isPlaying = true;
		}

		private void stopDebugging()
		{
			EditorApplication.isPlaying = false;
		}

		public void onSessionDisposed()
		{
			isDebugging = false;
			updateDebugButtonText();

			if (lastSceneName != EditorSceneManager.GetActiveScene().name && lastScenePath != "")
			{
				EditorSceneManager.OpenScene(lastScenePath);
			}
		}
	}
}
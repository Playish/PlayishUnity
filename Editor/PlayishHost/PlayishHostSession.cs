using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlayishUnityHost;


namespace Playish
{
	internal class PlayishHostSession : PlayishUnityHost.IPrint
	{
		// References
		private PlayishHostWindow hostWindow = null;
		private ServerConnection serverConnection = null;
		private PlayishHostConnectClient connectionClient = null;

		// Settings from host window
		private String deviceId = "";
		private String username = "";
		private String password = "";
		private SceneAsset initialScene = null;
		private String gameslug = "";


		public PlayishHostSession()
		{
			hostWindow = EditorWindow.GetWindow<PlayishHostWindow>(false, "Playish Host", false);
			serverConnection = ServerConnection.getInstance();
			connectionClient = new PlayishHostConnectClient();

			deviceId = hostWindow.consoleHostDeviceId;
			username = hostWindow.username;
			password = hostWindow.password;
			initialScene = hostWindow.initialSceneAsset;
			gameslug = hostWindow.gameslug;

			if (deviceId.Length > 0 && username.Length > 0 && password.Length > 0 && gameslug.Length > 0 && initialScene != null)
			{
				var currentScene = EditorSceneManager.GetActiveScene();
				if (currentScene.name != "playish_host_setup")
				{
					hostWindow.errorMessage = "Start from the scene playish_host_setup instead";
					hostWindow.showError = true;
					hostWindow.Repaint();
					return;
				}

				serverConnection.connect(deviceId, username, password);
				serverConnection.setDebugPrinter(this);
				serverConnection.setGameSlug(gameslug);
				serverConnection.setUnityConnectionClient(connectionClient);
				serverConnection.serverConnectionStatusChangedEvent += serverConnectionStatusChangedEvent;
				serverConnection.serverConnectionDevicesChangedEvent += serverConnectionDevicesChangedEvent;
			}
			else
			{
				hostWindow.errorMessage = "Not enough information provided";
				hostWindow.showError = true;
				hostWindow.Repaint();
			}
		}

		public void dispose()
		{
			serverConnection.setUnityConnectionClient(null);
			serverConnection.disconnect();
			
			hostWindow.errorMessage = "";
			hostWindow.showError = false;
			
			hostWindow.statusText = "Not connected";
			hostWindow.connectedDevices = 0;
			hostWindow.Repaint();
			hostWindow.onSessionDisposed();
			
			hostWindow = null;
			serverConnection = null;
			connectionClient = null;
		}


		// ---- MARK: ServerConnection events

		private void serverConnectionStatusChangedEvent(ServerConnection.ServerConnectionStatusChangedArgs e)
		{
			Dispatcher.getInstance().Invoke(() =>
			{
				updateStatusRelatedTexts(e.status);

				if (e.status == ServerConnection.ServerConnectionStatus.CONNECTED)
				{
					EditorSceneManager.LoadScene(initialScene.name, UnityEngine.SceneManagement.LoadSceneMode.Single);
				}
			});
		}
		
		private void serverConnectionDevicesChangedEvent(EventArgs e)
		{
			Dispatcher.getInstance().Invoke(() =>
			{
				hostWindow.connectedDevices = serverConnection.getConnectedDevicesCount();
				hostWindow.Repaint();
			});
		}


		// ---- MARK: Update playish window

		internal void updateStatusRelatedTexts(ServerConnection.ServerConnectionStatus status)
		{
			var statusText = "";

			if (status == ServerConnection.ServerConnectionStatus.CONNECTED)
			{
				statusText = "Connected";
			}
			else if (status == ServerConnection.ServerConnectionStatus.CONNECTING)
			{
				statusText = "Connecting";
			}
			else if (status == ServerConnection.ServerConnectionStatus.LOGIN_FAILED)
			{
				statusText = "Login failed";
			}
			else if (status == ServerConnection.ServerConnectionStatus.LOST_CONNECTION)
			{
				statusText = "Lost connection";
			}
			else // status == ServerConnection.ServerConnectionStatus.WAITING_FOR_START
			{
				statusText = "Not connected";
			}

			hostWindow.statusText = statusText;
			hostWindow.Repaint();
		}


		// ---- MARK: IPrint

		void IPrint.print(string message)
		{
			Debug.Log(message);
		}
	}
}

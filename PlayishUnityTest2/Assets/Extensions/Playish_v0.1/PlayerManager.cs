using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;


namespace Playish
{
	public class PlayerManager
	{
		// Singleton
		private static PlayerManager currentInstance = null;

		// Players
		/// <summary>
		/// The connected players to the console.
		/// </summary>
		public Dictionary<String, Player> players = new Dictionary<String, Player> ();

		// Events
		public delegate void PlayerChangedHandler (PlayerEventArgs e);
		public event PlayerChangedHandler playerAddedEvent;
		public event PlayerChangedHandler playerRemovedEvent;
		public event PlayerChangedHandler playersChangedEvent;


		// ---- MARK: Setup

		private PlayerManager()
		{}

		public static PlayerManager getInstance()
		{
			if (currentInstance == null)
			{
				currentInstance = new PlayerManager ();
			}
			return currentInstance;
		}


		// ---- MARK: Manage players

		public void addPlayer(Player newPlayer)
		{
			if (!players.ContainsKey (newPlayer.getDeviceId ()))
			{
				PlayishManager.getInstance ().writeWebConsoleLog ("addPlayer with deviceId: " + newPlayer.getDeviceId());

				players.Add (newPlayer.getDeviceId (), newPlayer);
				onPlayerAdded (new PlayerEventArgs(newPlayer.getDeviceId()));
			}
			else
			{
				players [newPlayer.getDeviceId ()] = newPlayer;
			}
		}

		public void removePlayer(String deviceId)
		{
			if (players.ContainsKey (deviceId))
			{
				PlayishManager.getInstance ().writeWebConsoleLog ("removePlayer with deviceId: " + deviceId);

				players.Remove (deviceId);
				onPlayerRemoved (new PlayerEventArgs(deviceId));
			}
		}

		public Player getPlayer(String deviceId)
		{
			if (players.ContainsKey (deviceId))
			{
				return players [deviceId];
			}
			return null;
		}

		public void clearPlayers()
		{
			players.Clear ();
			onPlayerRemoved (new PlayerEventArgs(""));
		}


		// ---- MARK: Helper functions

		public void changeControllerForAll(DynamicController controller)
		{
			var keys = players.Keys.ToArray ();

			PlayishManager.getInstance ().writeWebConsoleLog ("changeControllerForAll with controller name: " + controller.name + ", users.count: " + keys.Count());

			for (int i = 0; i < keys.Length; i++)
			{
				var item = players [keys [i]];
				item.setController (controller);
			}
		}


		// ---- MARK: Events

		private void onPlayerAdded (PlayerEventArgs e)
		{
			if (playerAddedEvent != null) playerAddedEvent (e);
			onPlayersChanged (e);
		}

		private void onPlayerRemoved (PlayerEventArgs e)
		{
			if (playerRemovedEvent != null) playerRemovedEvent (e);
			onPlayersChanged (e);
		}

		private void onPlayersChanged(PlayerEventArgs e)
		{
			e.deviceId = "";
			if (playersChangedEvent != null) playersChangedEvent (e);
		}


		// ---- MARK: Related definitions

		public class PlayerEventArgs : EventArgs
		{
			public String deviceId = "";

			public PlayerEventArgs(String deviceId)
			{
				this.deviceId = deviceId;
			}
		}
	}
}

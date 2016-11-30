using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;


namespace Playish
{
	public class PlayerManager
	{
		private static PlayerManager currentInstance = null;

		public Dictionary<String, Player> players = new Dictionary<String, Player> ();


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
				players.Add (newPlayer.getDeviceId (), newPlayer);
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
				players.Remove (deviceId);
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

		public Player clearPlayers()
		{
			players.Clear ();
		}


		// ---- MARK: Helper functions

		public void changeControllerForAll(DynamicController controller)
		{
			var keys = players.Keys.ToArray ();

			for (int i = 0; i < keys.Length; i++)
			{
				var item = players [keys [i]];
				item.setController (controller);
			}
		}
	}
}

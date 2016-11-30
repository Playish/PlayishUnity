using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Playish;

public class CubeMover : MonoBehaviour
{
	public GameObject buttonback;
	public GameObject buttonselect;
	public GameObject buttonup;
	public GameObject buttondown;
	public GameObject buttonleft;
	public GameObject buttonright;

	private PlayerManager playerManager;
	private PlayishManager playishManager;

	private Rigidbody rigidBody;

	// Use this for initialization
	void Start ()
	{
		Application.runInBackground = true;

		playerManager = PlayerManager.getInstance ();
		playerManager.playerAddedEvent += onPlayerAdded;
		playerManager.playerRemovedEvent += onPlayerRemoved;
		playerManager.playersChangedEvent += onPlayersChanged;

		playishManager = PlayishManager.getInstance ();

		rigidBody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		var playerKeys = playerManager.players.Keys.ToArray ();
		for (int playerKeyIndex = 0; playerKeyIndex < playerKeys.Length; playerKeyIndex++)
		{
			var player = playerManager.getPlayer (playerKeys [playerKeyIndex]);
			if (player == null || !player.hasController())
				return;

			var newVelocity = new Vector3 (0, 0, 0);

			var rotationInput = new Quaternion (-player.getFloatInput ("rotationX"), -player.getFloatInput ("rotationZ"), -player.getFloatInput ("rotationY"), player.getFloatInput ("rotationW"));
			transform.rotation = rotationInput;

			if (player.getBoolInput ("buttonleft"))
			{
				newVelocity.x = -1;
				buttonleft.transform.localScale = new Vector3 (buttonleft.transform.localScale.x, 0.4f, buttonleft.transform.localScale.z);
			}
			else
			{
				buttonleft.transform.localScale = new Vector3 (buttonleft.transform.localScale.x, 1f, buttonleft.transform.localScale.z);
			}

			if (player.getBoolInput ("buttonright"))
			{
				newVelocity.x = 1;
				buttonright.transform.localScale = new Vector3 (buttonright.transform.localScale.x, 0.4f, buttonright.transform.localScale.z);
			}
			else
			{
				buttonright.transform.localScale = new Vector3 (buttonright.transform.localScale.x, 1f, buttonright.transform.localScale.z);
			}

			if (player.getBoolInput ("buttonup"))
			{
				newVelocity.y = 1;
				buttonup.transform.localScale = new Vector3 (buttonup.transform.localScale.x, 0.4f, buttonup.transform.localScale.z);
			}
			else
			{
				buttonup.transform.localScale = new Vector3 (buttonup.transform.localScale.x, 1f, buttonup.transform.localScale.z);
			}

			if (player.getBoolInput ("buttondown"))
			{
				newVelocity.y = -1;
				buttondown.transform.localScale = new Vector3 (buttondown.transform.localScale.x, 0.4f, buttondown.transform.localScale.z);
			}
			else
			{
				buttondown.transform.localScale = new Vector3 (buttondown.transform.localScale.x, 1f, buttondown.transform.localScale.z);
			}

			if (player.getBoolInput ("buttonback"))
			{
				buttonback.transform.localScale = new Vector3 (buttonback.transform.localScale.x, 0.4f, buttonback.transform.localScale.z);
			}
			else
			{
				buttonback.transform.localScale = new Vector3 (buttonback.transform.localScale.x, 1f, buttonback.transform.localScale.z);
			}

			if (player.getBoolInput ("buttonselect"))
			{
				buttonselect.transform.localScale = new Vector3 (buttonselect.transform.localScale.x, 0.4f, buttonselect.transform.localScale.z);
			}
			else
			{
				buttonselect.transform.localScale = new Vector3 (buttonselect.transform.localScale.x, 1f, buttonselect.transform.localScale.z);
			}

			var accx = player.getFloatInput ("accelerationX") * 5000f * Time.deltaTime;
			var accy = player.getFloatInput ("accelerationY") * 5000f * Time.deltaTime;
			var accz = player.getFloatInput ("accelerationZ") * 5000f * Time.deltaTime;

			rigidBody.AddRelativeForce (new Vector3(-accx, -accz, -accy));
			rigidBody.velocity = newVelocity;
		}
	}


	// ---- MARK: Player updates

	private void onPlayerAdded(PlayerManager.PlayerEventArgs e)
	{
		playishManager.writeWebConsoleLog ("onPlayerAdded deviceId: " + e.deviceId + ", players.count: " + playerManager.players.Count);
	}

	private void onPlayerRemoved(PlayerManager.PlayerEventArgs e)
	{
		playishManager.writeWebConsoleLog ("onPlayerRemoved deviceId: " + e.deviceId + ", players.count: " + playerManager.players.Count);
	}

	private void onPlayersChanged(PlayerManager.PlayerEventArgs e)
	{
		playishManager.writeWebConsoleLog ("onPlayersChanged deviceId: " + e.deviceId + ", players.count: " + playerManager.players.Count);
	}
}

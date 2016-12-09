using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Playish;
using UnityEngine.UI;


public class CubeSpawner : MonoBehaviour
{
	// Controller prefab
	public GameObject cubeControllerPrefab;

	// UI Canvas
	public GameObject canvas;
	public Font font;

	// Playish references
	private PlayerManager playerManager;
	private PlayishManager playishManager;

	// Controller cubes
	private Dictionary<string, GameObject> cubeControllers = new Dictionary<string, GameObject>();
	private Dictionary<string, PlayerInfo> playersInfo = new Dictionary<string, PlayerInfo>();
	private int lastCubeControllerNumber = 0;


	// Use this for initialization
	void Start ()
	{
		Application.runInBackground = true;

		playerManager = PlayerManager.getInstance ();
		playerManager.playerAddedEvent += onPlayerAdded;
		playerManager.playerRemovedEvent += onPlayerRemoved;
		playerManager.playersChangedEvent += onPlayersChanged;

		playishManager = PlayishManager.getInstance ();
	}


	// ---- MARK: Player updates

	private void onPlayerAdded(PlayerManager.PlayerEventArgs e)
	{
		playishManager.writeWebConsoleLog ("onPlayerAdded deviceId: " + e.deviceId + ", players.count: " + playerManager.players.Count);
		spawnCubeController (e.deviceId);
	}

	private void onPlayerRemoved(PlayerManager.PlayerEventArgs e)
	{
		playishManager.writeWebConsoleLog ("onPlayerRemoved deviceId: " + e.deviceId + ", players.count: " + playerManager.players.Count);
		removeCubeController (e.deviceId);
	}

	private void onPlayersChanged(PlayerManager.PlayerEventArgs e)
	{}


	// ---- MARK: Cube controller management

	private void spawnCubeController(string playerDeviceId)
	{
		PlayerInfo playerInfo = null;
		if (playersInfo.ContainsKey (playerDeviceId))
		{
			playerInfo = playersInfo [playerDeviceId];
			playerInfo.showConnected ();
		}
		else
		{
			lastCubeControllerNumber += 1;

			playerInfo = new PlayerInfo (lastCubeControllerNumber);
			playerInfo.spawnUIParts (ref canvas, ref font);
			playerInfo.showConnected ();
			playersInfo.Add (playerDeviceId, playerInfo);
		}

		var position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);

		var newCubeController = Instantiate<GameObject> (cubeControllerPrefab);
		newCubeController.transform.position = position;

		// Change color
		var meshRenderer = newCubeController.GetComponent<MeshRenderer>();
		if (meshRenderer != null)
		{
			var existingMaterial = meshRenderer.material;
			if (existingMaterial != null)
			{
				existingMaterial.color = playerInfo.playerColor;
			}
		}

		var cubeMoverScript = newCubeController.GetComponent<CubeMover> ();
		if (cubeMoverScript != null)
		{
			cubeMoverScript.playerDeviceId = playerDeviceId;
		}

		cubeControllers.Add (playerDeviceId, newCubeController);
	}

	private void removeCubeController(string playerDeviceId)
	{
		if (cubeControllers.ContainsKey (playerDeviceId))
		{
			var player = cubeControllers [playerDeviceId];
			Destroy (player);
			cubeControllers.Remove (playerDeviceId);
		}

		if (playersInfo.ContainsKey (playerDeviceId))
		{
			var playerInfo = playersInfo [playerDeviceId];
			playerInfo.showDisconnected ();
		}
	}


	// ---- MARK: Related definitions

	private class PlayerInfo
	{
		public int playerNumber = -1;
		public Color playerColor = Color.white;
		public GameObject uiColorDisplay = null;
		public GameObject uiText = null;

		private static Color[] playerColors = new Color[] {
			new Color(1f, 0f, 0f), new Color(0f, 1f, 0f),
			new Color(0f, 0f, 1f), new Color(1f, 1f, 1f),
			new Color(0f, 0f, 0f), new Color(1f, 0.5f, 0f),
			new Color(1f, 1f, 0f), new Color(0f, 1f, 1f),
			new Color(1f, 0f, 1f) };

		private const float colorSize = 35f;
		private const float paddingSize = 20f;
		private const float extraPaddingForText = 3f;
		

		public PlayerInfo(int playerNumber)
		{
			this.playerNumber = playerNumber;
			var colorIndex = Mathf.Abs(playerNumber - 1) % playerColors.Length;
			playerColor = playerColors[colorIndex];
		}

		public void spawnUIParts(ref GameObject canvas, ref Font font)
		{
			uiColorDisplay = new GameObject ("ColorForPlayer" + playerNumber);
			uiText = new GameObject ("TextForPlayer" + playerNumber);

			uiColorDisplay.transform.SetParent (canvas.transform);
			uiText.transform.SetParent (canvas.transform);

			var text = uiText.AddComponent<Text> ();
			text.color = Color.white;
			text.font = font;
			text.fontSize = 24;
			text.horizontalOverflow = HorizontalWrapMode.Overflow;
			text.verticalOverflow = VerticalWrapMode.Overflow;

			var rawImage = uiColorDisplay.AddComponent<RawImage> ();
			rawImage.color = playerColor;

			// Transforms
			float verticalBase = (float)(playerNumber - 1) * (paddingSize + colorSize);

			var colorDisplayTransform = uiColorDisplay.GetComponent<RectTransform> ();
			colorDisplayTransform.anchorMin = new Vector2 (0, 1);
			colorDisplayTransform.anchorMax = new Vector2 (0, 1);
			colorDisplayTransform.pivot = new Vector2 (0, 1);
			colorDisplayTransform.anchoredPosition = new Vector2 (paddingSize, -(verticalBase + paddingSize));
			colorDisplayTransform.sizeDelta = new Vector2 (colorSize, colorSize);

			var textTransform = uiText.GetComponent<RectTransform> ();
			textTransform.anchorMin = new Vector2 (0, 1);
			textTransform.anchorMax = new Vector2 (0, 1);
			textTransform.pivot = new Vector2 (0, 1);
			textTransform.anchoredPosition = new Vector2 (paddingSize * 2f + colorSize, -(verticalBase + paddingSize + extraPaddingForText));
		}

		public void showDisconnected()
		{
			if (uiText != null)
			{
				var textComp = uiText.GetComponent<Text> ();
				if (textComp != null)
				{
					textComp.text = "Player " + playerNumber + ", disconnected";
					textComp.color = Color.grey;
				}
			}
		}

		public void showConnected()
		{
			if (uiText != null)
			{
				var textComp = uiText.GetComponent<Text> ();
				if (textComp != null)
				{
					textComp.text = "Player " + playerNumber;
					textComp.color = Color.white;
				}
			}
		}
	}
}

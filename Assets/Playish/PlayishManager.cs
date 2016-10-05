using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Playish
{

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class PlayishManager : MonoBehaviour
{
	private static PlayishManager instance;

	private void Awake()
	{
		PlayishController.LoadTypes();
	}

	private void Update()
	{
		#if UNITY_EDITOR

		/**
		 * Making sure that the manager has the correct name.
		 * Otherwise it wont be able to be called from the playish website.
		 */
		if(name != "PlayishManager")
		{
			name = "PlayishManager";
		}

		#endif

		if(instance == null)
		{
			instance = this;
		}
	}

	public static PlayishManager GetInstance()
	{
		return instance;
	}

	#region Utility

	/**
	 * Process raw data and turn it into a command.
	 */
	private PlayishCommand CommandFromData(string data)
	{
		return new PlayishCommand(data);
	}

	#endregion

	#region Receive

	//----------------------------------------------------------------------
	// Receiving functions catching data from the webpage the game is run in.
	//----------------------------------------------------------------------

	/**
	 * Receive input from a player controller, process it and update the right controller.
	 */
	public void R_ControllerInput(string data)
	{
		PlayishController.UpdateController(CommandFromData(data));
	}

	#endregion
}

}
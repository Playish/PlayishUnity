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
			if(instance == null)
			{
				instance = this;
			}

			PlayishController.LoadTypes();
			//S_ResendControllers();
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

			PlayishController[] cons = PlayishController.GetAll();
			for(int i = 0; i < cons.Length; i++)
			{
				cons[i].TickInputs();
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

		public void R_ControllerConnected(string data)
		{
			PlayishController.AddController(CommandFromData(data));
		}

		public void R_ControllerDisconnected(string data)
		{
			PlayishController.RemoveController(CommandFromData(data));
		}

		#endregion

		#region Receive

		//------------------------------------------------------------------------------------
		// Sending functions sending data from the game to the webpage the game is running in.
		//------------------------------------------------------------------------------------

		/**
		 * Ask playish to resend the controllers calling R_ControllerInput once for each controller.
		 */
		public void S_ResendControllers()
		{
			#if UNITY_WEBGL

			Application.ExternalCall("ResendControllers");

			#endif
		}

		#endregion
	}
}
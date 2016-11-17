using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace Playish
{

[System.Serializable]
public class PlayishController
{
	/** The player this controller data is bound to. */
	public string id;

	public PlayishController(){}

	public PlayishController(PlayishCommand command)
	{
		id = command.playerId;

		UpdateInputData(command);
	}

	#region Input

	public Dictionary<string, IPlayishInput> inputs = new Dictionary<string, IPlayishInput>();

	/**
	 * Update the controller with the given player id. If the controller does not exist it will create it.
	 */
	public static void UpdateController(PlayishCommand command)
	{
		if(PlayerHaveController(command.playerId))
		{
			controllers[command.playerId].UpdateInputData(command);
		}
		else
		{
			controllers.Add(command.playerId, new PlayishController(command));
		}
	}

	/**
	 * Update all the inputs for the controller specified in the incomming command.
	 */
	private void UpdateInputData(PlayishCommand command)
	{
		for(int i = 0; i < command.Count(); i++)
		{
			string[] data = command.GetData(i);

			if(inputs.ContainsKey(data[0]))
			{
				if(inputs[data[0]].GetTypeName() == data[1])
				{
					inputs[data[0]] = inputTypes[data[1]].Create();
				}
			}
			else
			{
				inputs[data[0]] = inputTypes[data[1]].Create();
			}

			inputs[data[0]].UpdateInputs(data);
		}
	}

	public T GetInput<T>(string name)
	{
		return (T)inputs[name];
	}

	public IPlayishInput[] GetAllInputs()
	{
		IPlayishInput[] inputArray = new IPlayishInput[inputs.Count];
		inputs.Values.CopyTo(inputArray, 0);
		return inputArray;
	}

	#endregion

	#region Tick

	public static void TickAllControllers()
	{
		PlayishController[] cons = GetAll();
		for(int i = 0; i < cons.Length; i++)
		{
			cons[i].TickInputs();
		}
	}

	public void TickInputs()
	{
		IPlayishInput[] ins = GetAllInputs();
		for(int i = 0; i < inputs.Count; i++)
		{
			ins[i].Tick();
		}
	}

	#endregion

	#region Controller

	/** Internal dictionary keeping track of all the player controllers during runtime. */
	private static Dictionary<string, PlayishController> controllers = new Dictionary<string, PlayishController>();

	/** All the input types available in the assembly. */
	private static Dictionary<string, IPlayishInput> inputTypes = new Dictionary<string, IPlayishInput>();

	/** Amount of currently connected controllers. */
	public static int controllerCount {get; private set;}

	public static void AddController(PlayishCommand command)
	{
		if(!PlayerHaveController(command.playerId))
		{
			controllers.Add(command.playerId, new PlayishController());
			players.Add(players.Count, controllers[command.playerId]);
			controllers[command.playerId].TriggerControllerConnected(controllers[command.playerId]);
		}
	}

	public static void RemoveController(PlayishCommand command)
	{
		if(PlayerHaveController(command.playerId))
		{
			controllers[command.playerId].TriggerControllerDisconnected(controllers[command.playerId]);
			players.Remove(players.Count - 1);
			controllers.Remove(command.playerId);
		}
	}

	/**
	 * Get the controller with the given player id. Returns null if no controller with that id was found.
	 */
	public static PlayishController Get(string playerId)
	{
		if(controllers.ContainsKey(playerId))
		{
			return controllers[playerId];
		}

		return null;
	}

	/**
	 * Get all controllers.
	 */
	public static PlayishController[] GetAll()
	{
		PlayishController[] controllerArray = new PlayishController[controllers.Count];
		controllers.Values.CopyTo(controllerArray, 0);
		return controllerArray;
	}

	public static bool PlayerHaveController(string playerId)
	{
		return controllers.ContainsKey(playerId);
	}

	/**
	 * Load all type declarations in the assembly.
	 */
	public static void LoadTypes()
	{
		if(inputTypes != null && inputTypes.Count > 0){return;}

		IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where((Assembly assembly) => assembly.FullName.Contains("Assembly"));
		foreach(Assembly assembly in scriptAssemblies)
		{
			foreach(Type type in assembly.GetTypes ().Where (T => T.IsClass && !T.IsAbstract && T.GetInterfaces ().Contains (typeof (IPlayishInput))))
			{
				IPlayishInput typeInstance = assembly.CreateInstance(type.FullName) as IPlayishInput;
				if(typeInstance == null)
					throw new UnityException("Error with Type Declaration " + type.FullName);
				inputTypes.Add(typeInstance.GetTypeName(), typeInstance);
			}
		}
	}

	#endregion

	#region Players

	private static Dictionary<int, PlayishController> players = new Dictionary<int, PlayishController>();

	public static PlayishController GetByPlayerId(int id)
	{
		if(players.ContainsKey(id))
		{
			return players[id];
		}

		return null;
	}

	#endregion

	#region Events

	public delegate void ActionControllerConnected(PlayishController controller);

	public delegate void ActionControllerDisconnected(PlayishController controller);

	public event ActionControllerConnected EventControllerConnected;

	public event ActionControllerDisconnected EventControllerDisconnected;

	private void TriggerControllerConnected(PlayishController controller)
	{
		if(EventControllerConnected != null)
		{
			EventControllerConnected(controller);
		}
	}

	private void TriggerControllerDisconnected(PlayishController controller)
	{
		if(EventControllerDisconnected != null)
		{
			EventControllerDisconnected(controller);
		}
	}

	#endregion
}

}
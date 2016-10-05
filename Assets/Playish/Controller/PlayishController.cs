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
	/** Internal dictionary keeping track of all the player controllers during runtime. */
	private static Dictionary<string, PlayishController> controllers = new Dictionary<string, PlayishController>();

	/** All the input types available in the assembly. */
	private static Dictionary<string, IPlayishInput> inputTypes = new Dictionary<string, IPlayishInput>();

	public Dictionary<string, IPlayishInput> inputs = new Dictionary<string, IPlayishInput>();

	/** The player this controller data is bound to. */
	public string player;

	public PlayishController(PlayishCommand command)
	{
		player = command.playerId;

		UpdateInputs(command);
	}

	/**
	 * Update all the inputs for the controller specified in the incomming command.
	 */
	private void UpdateInputs(PlayishCommand command)
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

			inputs[data[0]].Update(data);
		}
	}

	public T GetInput<T>(string name)
	{
		return (T)inputs[name];
	}

	/**
	 * Update the controller with the given player id. If the controller does not exist it will create it.
	 */
	public static void UpdateController(PlayishCommand command)
	{
		if(command.comType != "input_update"){return;}

		if(!controllers.ContainsKey(command.playerId))
		{
			controllers.Add(command.playerId, new PlayishController(command));
		}
		else
		{
			controllers[command.playerId].UpdateInputs(command);
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
}

}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayishCommand
{
	public string playerId;

	private List<string[]> values = new List<string[]>();

	public PlayishCommand(string data)
	{
		string[] commandSplit = data.Split('#');

		playerId = commandSplit[0];

		if(commandSplit.Length <= 1){return;}

		string[] valueSplit;

		for(int i = 1; i < commandSplit.Length; i++)
		{
			valueSplit = commandSplit[i].Split(';');

			if(valueSplit.Length > 0)
			{
				values.Add(valueSplit);
			}
		}
	}

	/**
	 * Get the value with the given index in the data block with the given name as raw string.
	 */
	public string[] GetData(int index)
	{
		return values[index];
	}

	public List<string[]> GetAllData()
	{
		return values;
	}

	/**
	 * Number of data blocks in this command.
	 */
	public int Count()
	{
		return values.Count;
	}
}
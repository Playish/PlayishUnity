using UnityEngine;
using System.Collections;

namespace Playish
{

public class PlayishButton : IPlayishInput
{
	/** State of this button. */
	public bool state = false;

	public string name = "DEFAULT";

	public string GetTypeName()
	{
		return "button";
	}

	public string GetName()
	{
		return name;
	}

	public void Update(string[] values)
	{
		if(values[1] == "button")
		{
			name = values[0];
			state = bool.Parse(values[2]);
		}
	}

	public IPlayishInput Create()
	{
		return new PlayishButton();
	}

	public static PlayishButton Get(string playerId, string buttonName)
	{
		if(PlayishController.PlayerHaveController(playerId))
		{
			return PlayishController.Get(playerId).GetInput<PlayishButton>(buttonName);
		}

		return new PlayishButton();
	}
}

}
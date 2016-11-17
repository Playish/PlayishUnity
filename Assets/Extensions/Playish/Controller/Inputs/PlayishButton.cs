using UnityEngine;
using System.Collections;

namespace Playish
{

public class PlayishButton : IPlayishInput
{
	/** State of this button. */
	private int state = 0;
	/** State of the button last frame. */
	private int last = 0;

	public string name = "DEFAULT";

	public string GetTypeName()
	{
		return "button";
	}

	public string GetName()
	{
		return name;
	}

	public void Tick()
	{
		last = state;
	}

	public void UpdateInputs(string[] values)
	{
		if(values[1] == "button")
		{
			name = values[0];
			state = int.Parse(values[2]);
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

	#region Button

	public bool IsPressed()
	{
		return state > 0;
	}

	public bool IsReleased()
	{
		return state == 0;
	}

	public bool JustPressed()
	{
		return state > 0 && last == 0;
	}

	public bool JustReleased()
	{
		return state == 0 && last > 0;
	}

	#endregion
}

}
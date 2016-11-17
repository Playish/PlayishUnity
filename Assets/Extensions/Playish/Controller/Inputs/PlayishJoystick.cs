using UnityEngine;
using System.Collections;

namespace Playish
{

public class PlayishJoystick : IPlayishInput
{
	/** The current positional value of this joystick. */
	public Vector2 positon;
	/** Position of the joystick during the last frame. */
	private Vector2 lastPos;

	public string name = "DEFAULT";

	public string GetTypeName()
	{
		return "joystick";
	}

	public string GetName()
	{
		return name;
	}

	public void Tick()
	{
		
	}

	public void UpdateInputs(string[] values)
	{
		if(values[1] == "joystick")
		{
			name = values[0];
			lastPos = positon;
			positon = new Vector2((float)int.Parse(values[2]) / 100, (float)int.Parse(values[3]) / 100);
		}
	}

	public IPlayishInput Create()
	{
		return new PlayishJoystick();
	}

	public static PlayishJoystick Get(string playerId, string joystickName)
	{
		if(PlayishController.PlayerHaveController(playerId))
		{
			return PlayishController.Get(playerId).GetInput<PlayishJoystick>(joystickName);
		}

		return new PlayishJoystick();
	}
}

}
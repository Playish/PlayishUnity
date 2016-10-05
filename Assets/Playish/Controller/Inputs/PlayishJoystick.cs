using UnityEngine;
using System.Collections;

namespace Playish
{

public class PlayishJoystick : IPlayishInput
{
	/** The current positional value of this joystick. */
	public Vector2 positon;

	public string name = "Joystick";

	public string GetTypeName()
	{
		return "joystick";
	}

	public string GetName()
	{
		return name;
	}

	public void Update(string[] values)
	{
		if(values[1] == "joystick")
		{
			name = values[0];
			positon = new Vector2(int.Parse(values[2]) / 100, int.Parse(values[3]) / 100);
		}
	}

	public IPlayishInput Create()
	{
		return new PlayishJoystick();
	}

	public static PlayishJoystick Get(string playerId, string joystickName)
	{
		return PlayishController.Get(playerId).GetInput<PlayishJoystick>(joystickName);
	}
}

}
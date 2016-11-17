using UnityEngine;
using System.Collections;

namespace Playish.Debug
{

public class InputTester : MonoBehaviour
{
	private void Update()
	{
		Vector2 joystick;
		int a = 0;
		int b = 0;

		if(Input.GetKey(KeyCode.Z))
		{
			a = 1;
		}
		else
		{
			a = 0;
		}

		if(Input.GetKey(KeyCode.X))
		{
			b = 1;
		}
		else
		{
			b = 0;
		}

		joystick = new Vector2((int)(Input.GetAxis("Horizontal") * 100), (int)(Input.GetAxis("Vertical") * 100));
		UnityEngine.Debug.Log(joystick);

		PlayishManager.GetInstance().R_ControllerInput("test#joystick;joystick;" + joystick.x + ";" + joystick.y + "#buttona;button;" + a + "#buttonb;button;" + b);
	}
}

}
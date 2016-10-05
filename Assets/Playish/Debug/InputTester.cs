using UnityEngine;
using System.Collections;

namespace Playish.Debug
{

public class InputTester : MonoBehaviour
{
	private void Update()
	{
		if(Input.GetKey(KeyCode.A))
		{
			PlayishManager.GetInstance().R_ControllerInput("input_update#test#button;button;true");
		}
		else
		{
			PlayishManager.GetInstance().R_ControllerInput("input_update#test#button;button;false");
		}
	}
}

}
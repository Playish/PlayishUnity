using UnityEngine;

namespace Playish
{

[RequireComponent(typeof(PlatformerMotor2D))]
public class TestDriver : MonoBehaviour
{
	private PlatformerMotor2D motor;

	private Vector2 dPad;

	private void Start()
	{
		motor = GetComponent<PlatformerMotor2D>();
	}

	private void Update()
	{
		//PlayishController con = PlayishController.GetByPlayerId(0);

		//if(con == null){return;}

		if(Mathf.Abs(PlayishJoystick.Get("test", "joystick").positon.x) > PC2D.Globals.INPUT_THRESHOLD)
		{
			motor.normalizedXMovement = PlayishJoystick.Get("test", "joystick").positon.x;
		}
		else
		{
			motor.normalizedXMovement = 0;
		}

		// Jump?
		if(PlayishButton.Get("test", "buttona").JustPressed())
		{
			motor.Jump();
		}

		motor.jumpingHeld = PlayishButton.Get("test", "buttona").IsPressed();

		if(PlayishJoystick.Get("test", "joystick").positon.y < -PC2D.Globals.FAST_FALL_THRESHOLD)
		{
			motor.fallFast = true;
		}
		else
		{
			motor.fallFast = false;
		}

		if(PlayishButton.Get("test", "buttonb").IsPressed())
		{
			motor.Dash();
		}
	}
}
}
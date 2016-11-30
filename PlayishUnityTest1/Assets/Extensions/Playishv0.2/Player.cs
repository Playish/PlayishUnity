using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Playish
{
	public class Player
	{
		private String deviceId = "";
		private DynamicController controller = null;

		private Dictionary<String, int> intInputs = new Dictionary<String, int> ();
		private Dictionary<String, float> floatInputs = new Dictionary<String, float> ();


		public Player(String deviceId)
		{
			this.deviceId = deviceId;
		}


		// ---- MARK: Player

		public String getDeviceId()
		{
			return deviceId;
		}


		// ---- MARK: Controller

		public DynamicController getController()
		{
			return controller;
		}

		public void setController(DynamicController controller)
		{
			this.controller = controller;
		}


		// ---- MARK: Input handling

		public void clearInputs()
		{
			intInputs.Clear ();
			floatInputs.Clear ();
		}

		public void setInput(String name, int value)
		{
			if (intInputs.ContainsKey (name))
			{
				intInputs [name] = value;
			}
			else
			{
				intInputs.Add (name, value);
			}
		}

		public void setInput(String name, float value)
		{
			if (floatInputs.ContainsKey (name))
			{
				floatInputs [name] = value;
			}
			else
			{
				floatInputs.Add (name, value);
			}
		}

		public bool getBoolInput(String name)
		{
			if (intInputs.ContainsKey(name))
			{
				return intInputs[name] > 0;
			}
			return false;
		}

		public int getIntInput()
		{
			if (intInputs.ContainsKey(name))
			{
				return intInputs[name];
			}
			return 0;
		}

		public float getFloatInput()
		{
			if (floatInputs.ContainsKey(name))
			{
				return floatInputs[name];
			}
			return 0;
		}


		// ---- MARK: Parse and set input

		public void parseAndSetInput(String input, int deviceIdEndIndex) // "deviceId;00000000casd"
		{
			if (controller == null || input.Length <= deviceIdEndIndex + 1)
			{
				return;
			}

			int currentCharPos = deviceIdEndIndex + 1;

			// Areas
			foreach (var areaDef in controller.areas)
			{
				if (areaDef.type == "joystick")
				{
					if (currentCharPos + 1 >= input.Length)
					{
						return;
					}

					int valuex = Convert.ToInt32 (input[currentCharPos]) - 132;
					int valuey = Convert.ToInt32 (input[currentCharPos + 1]) - 132;
					currentCharPos += 2;

					setInput (areaDef.name + "X", valuex);
					setInput (areaDef.name + "Y", valuey);
				}
				else if (areaDef.type == "button")
				{
					if (currentCharPos >= input.Length)
					{
						return;
					}

					int value = (int)Char.GetNumericValue (input [currentCharPos]);
					currentCharPos += 1;

					setInput (areaDef.name, value);
				}
				else
				{
					// Skip input
					currentCharPos += 1;
				}
			}

			// Sensors
			if (controller.sendgravity == 1)
			{
				if (currentCharPos + 5 >= input.Length)
				{
					return;
				}

				int valuex1 = Convert.ToInt32 (input[currentCharPos]) - 132;
				int valuex2 = Convert.ToInt32 (input[currentCharPos + 1]) - 132;
				int valuey1 = Convert.ToInt32 (input[currentCharPos + 2]) - 132;
				int valuey2 = Convert.ToInt32 (input[currentCharPos + 3]) - 132;
				int valuez1 = Convert.ToInt32 (input[currentCharPos + 4]) - 132;
				int valuez2 = Convert.ToInt32 (input[currentCharPos + 5]) - 132;

				float gravx = getInputFromCompressedRange(new int[valuex1, valuex2], 2) - 1;
				float gravy = getInputFromCompressedRange(new int[valuey1, valuey2], 2) - 1;
				float gravz = getInputFromCompressedRange(new int[valuez1, valuez2], 2) - 1;

				currentCharPos += 6;

				setInput ("gravityX", gravx);
				setInput ("gravityY", gravy);
				setInput ("gravityZ", gravz);
			}

			if (controller.sendacceleration == 1)
			{
				if (currentCharPos + 5 >= input.Length)
				{
					return;
				}

				int valuex1 = Convert.ToInt32 (input[currentCharPos]) - 132;
				int valuex2 = Convert.ToInt32 (input[currentCharPos + 1]) - 132;
				int valuey1 = Convert.ToInt32 (input[currentCharPos + 2]) - 132;
				int valuey2 = Convert.ToInt32 (input[currentCharPos + 3]) - 132;
				int valuez1 = Convert.ToInt32 (input[currentCharPos + 4]) - 132;
				int valuez2 = Convert.ToInt32 (input[currentCharPos + 5]) - 132;

				float accelx = getInputFromCompressedRange(new int[valuex1, valuex2], 2) - 1;
				float accely = getInputFromCompressedRange(new int[valuey1, valuey2], 2) - 1;
				float accelz = getInputFromCompressedRange(new int[valuez1, valuez2], 2) - 1;

				currentCharPos += 6;

				setInput ("accelerationX", accelx);
				setInput ("accelerationY", accely);
				setInput ("accelerationZ", accelz);
			}
		}

		private float getInputFromCompressedRange(int[] inputs, double maxRange)
		{
			double numberBase = 199;
			int resultOffset = 99;

			double wholeNumberPrecision = Math.Pow(numberBase, (double)inputs.Length) - 1;

			int result = 0;
			int currentMod = 0;

			for (var inputIndex = 0; inputIndex < inputs.Length; inputIndex++)
			{
				int input = inputs[inputIndex];
				currentMod = (int)(Math.Pow(numberBase, (double)((inputs.Length - inputIndex) - 1)));
				int localValue = input + resultOffset;

				if (inputIndex != inputs.Length - 1)
				{
					result += localValue * currentMod;
				}
				else
				{
					result += localValue;
				}
			}
			return (float)((double)(result) / wholeNumberPrecision * maxRange);
		}
	}
}

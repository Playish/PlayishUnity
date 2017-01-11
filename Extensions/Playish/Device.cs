using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Playish
{
	public class Device
	{
		private String deviceId = "";
		private Controller controller = null;

		private Dictionary<String, int> intInputs = new Dictionary<String, int> ();
		private Dictionary<String, float> floatInputs = new Dictionary<String, float> ();


		public Device(String deviceId)
		{
			this.deviceId = deviceId;
		}


		// ---- MARK: Device

		/// <summary>
		/// Gets the deviceId which is used to identify a device.
		/// </summary>
		public String getDeviceId()
		{
			return deviceId;
		}


		// ---- MARK: Controller

		/// <summary>
		/// Returns true if device has a controller defined. Without the controller the input is useless.
		/// </summary>
		public bool hasController()
		{
			return controller != null;
		}

		/// <summary>
		/// Gets the controller defined for the device. The controller is used by the plugin to interpret the input from 
		/// connected devices.
		/// </summary>
		public Controller getController()
		{
			return controller;
		}

		/// <summary>
		/// Used by the plugin to change a devices controller. If the game want to change a controller for a connected device see
		/// PlayishManager.changeController(String deviceId (optional), String controllerName).
		/// </summary>
		public void setController(Controller controller)
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

		public int getIntInput(String name)
		{
			if (intInputs.ContainsKey(name))
			{
				return intInputs[name];
			}
			return 0;
		}

		public float getFloatInput(String name)
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
			if (controller.sendrotation == 1)
			{
				if (currentCharPos + 7 >= input.Length)
				{
					return;
				}

				int valuex1 = Convert.ToInt32 (input[currentCharPos]) - 132;
				int valuex2 = Convert.ToInt32 (input[currentCharPos + 1]) - 132;
				int valuey1 = Convert.ToInt32 (input[currentCharPos + 2]) - 132;
				int valuey2 = Convert.ToInt32 (input[currentCharPos + 3]) - 132;
				int valuez1 = Convert.ToInt32 (input[currentCharPos + 4]) - 132;
				int valuez2 = Convert.ToInt32 (input[currentCharPos + 5]) - 132;
				int valuew1 = Convert.ToInt32 (input[currentCharPos + 6]) - 132;
				int valuew2 = Convert.ToInt32 (input[currentCharPos + 7]) - 132;

				float gravx = getInputFromCompressedRange(new int[]{ valuex1, valuex2 }, 2) - 1;
				float gravy = getInputFromCompressedRange(new int[]{ valuey1, valuey2 }, 2) - 1;
				float gravz = getInputFromCompressedRange(new int[]{ valuez1, valuez2 }, 2) - 1;
				float gravw = getInputFromCompressedRange(new int[]{ valuew1, valuew2 }, 2) - 1;

				currentCharPos += 8;

				setInput ("rotationX", gravx);
				setInput ("rotationY", gravy);
				setInput ("rotationZ", gravz);
				setInput ("rotationW", gravw);
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

				float accelx = getInputFromCompressedRange(new int[]{ valuex1, valuex2 }, 20) - 10;
				float accely = getInputFromCompressedRange(new int[]{ valuey1, valuey2 }, 20) - 10;
				float accelz = getInputFromCompressedRange(new int[]{ valuez1, valuez2 }, 20) - 10;

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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Playish
{
	[Serializable]
	public class Controller
	{
		public String name = "";
		public int sendrotation = 0;
		public int sendacceleration = 0;
		public InputDefinition[] areas = new InputDefinition[0];


		public Controller()
		{}


		// ---- MARK: Related definitions

		[Serializable]
		public class InputDefinition
		{
			public String name = "";
			public String type = "";
		}
	}
}

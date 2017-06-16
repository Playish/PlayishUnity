using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Playish;


namespace Playish
{
	public class DispatcherUpdate : MonoBehaviour
	{
		void Update()
		{
			if (name != "MainQueueInvoker")
			{
				name = "MainQueueInvoker";
			}

			Dispatcher.getInstance().InvokePending();
		}
	}
}

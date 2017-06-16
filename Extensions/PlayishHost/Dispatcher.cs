using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Playish
{
	public class Dispatcher : IDispatcher
	{
		private static Dispatcher currentInstance = null;
		public List<Action> pending = new List<Action>();


		private Dispatcher()
		{}

		public static Dispatcher getInstance()
		{
			if (currentInstance == null)
			{
				currentInstance = new Dispatcher();
			}
			return currentInstance;
		}

		public void Invoke(Action function)
		{
			lock (pending)
			{
				pending.Add(function);
			}
		}

		public void InvokePending()
		{
			lock (pending)
			{
				for (int i = 0; i < pending.Count; i++)
				{
					pending[i].Invoke();
				}
				pending.Clear();
			}
		}
	}
}

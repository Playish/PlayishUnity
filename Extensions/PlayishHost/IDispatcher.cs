using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Playish
{
	public interface IDispatcher
	{
		void Invoke(Action function);
	}
}

using UnityEngine;
using System.Collections;

namespace Playish
{

public class TestDriver : MonoBehaviour
{
	public float speed;

	private PlayishButton button;

	private void Update()
	{
		button = PlayishButton.Get("test", "button");
		if(button != null)
		{
			if(button.state)
			{
				transform.Rotate(0, 0, speed * Time.deltaTime);
			}
		}
	}
}

}
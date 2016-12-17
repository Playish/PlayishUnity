using UnityEngine;
using System.Collections;

public class PersistOnSceneLoad : MonoBehaviour {

	// Use this for initialization
	private void Start ()
	{
		DontDestroyOnLoad (transform.gameObject);
	}
}

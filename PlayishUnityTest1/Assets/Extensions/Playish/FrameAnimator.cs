using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[Serializable]
public class FrameAnimator : MonoBehaviour
{
	[Serializable]
	public struct FrameArray
	{
		public string name;
		public int[] frames;

		public bool pingPong;
		public bool randomStartingFrame;
	}
	public FrameArray[] animations;

	public int selectedAnim = 0;

	public float interval = 0.25f;
	public bool playOnAwake = true;
	public bool isPlaying = false;

	private int currentFrame = 0;
	private int numFrames = 0;
	private float currentFrameTime = 0f;
	private int dir = 1;

	private void Awake()
	{
		for(int i=0;i<transform.childCount;i++)
		{
			if (transform.GetChild(i).name.ToLower().StartsWith("frame")) numFrames++;
		}

		Reset();
		SetCurrentAnimation("idle");

		if (playOnAwake)
		{
			Play();
		}
	}

	private void Update()
	{
		if ((numFrames<=0) || !isPlaying || (animations.Length == 0)) return;

		currentFrameTime += Time.deltaTime;
		if (currentFrameTime >= interval)
		{
			currentFrameTime = 0;

			currentFrame += dir;
			if (dir == 1 && currentFrame == GetAnimation(selectedAnim).frames.Length)
			{
				if (GetAnimation(selectedAnim).pingPong)
				{
					currentFrame--;
					dir = -1;
				}
				else currentFrame = 0;
			}
			else if (dir == -1 && currentFrame == 0)
			{
				dir = 1;
			}

			int thisFrame = 0;
			for(int i=0;i<transform.childCount;i++)
			{
				if (transform.GetChild(i).name.ToLower().StartsWith("frame"))
				{
					transform.GetChild(i).gameObject.SetActive(false);
					if (GetAnimation(selectedAnim).frames[currentFrame] == thisFrame)
					{
						transform.GetChild(i).gameObject.SetActive(true);
					}

					thisFrame++;
				}
			}
		}
	}

	public void Play()
	{
		isPlaying = true;
	}

	public void Pause()
	{
		isPlaying = false;
	}

	public void Reset()
	{
		isPlaying = false;
		currentFrame = 0;
		currentFrameTime = 0;
	}

	public FrameArray GetAnimation(string name)
	{
		foreach(FrameArray array in animations)
		{
			if(array.name == name)
			{
				return array;
			}
		}

		return animations[0];
	}

	public FrameArray GetAnimation(int id)
	{	
		return animations[id];
	}

	public void SetCurrentAnimation(string name)
	{
		for(int i = 0; i < animations.Length; i++)
		{
			if(GetAnimation(i).name == name)
			{
				if(i != selectedAnim)
				{
					currentFrame = 0;
					dir = 1;
					selectedAnim = i;
				}
				return;
			}
		}

		Debug.Log("[" + name + "] is not an existing animation.");
	}

	public void SetCurrentAnimation(int id)
	{
		if(id >= animations.Length){return;}

		for(int i = 0; i < animations.Length; i++)
		{
			if(i == id)
			{
				if(i != selectedAnim)
				{
					currentFrame = 0;
					dir = 1;
					selectedAnim = i;
				}
				return;
			}
		}

		Debug.Log("[" + name + "] is not an existing animation.");
	}
}
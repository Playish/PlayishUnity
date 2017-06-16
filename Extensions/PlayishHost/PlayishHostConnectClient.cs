using UnityEngine;
using System.Collections;
using PlayishUnityHost;
using System;
using Playish;

public class PlayishHostConnectClient : IGameConnectionClient
{
	public void onControllerChanged(string data)
	{
		Dispatcher.getInstance().Invoke(() =>
		{
			if (Application.isPlaying)
			{
				PlayishManager.getInstance().onControllerChanged(data);
			}
		});
	}

	public void onControllerChangedForAll(string data)
	{
		Dispatcher.getInstance().Invoke(() =>
		{
			if (Application.isPlaying)
			{
				PlayishManager.getInstance().onControllerChangedForAll(data);
			}
		});
	}

	public void onDeviceConnected(string data)
	{
		Dispatcher.getInstance().Invoke(() =>
		{
			if (Application.isPlaying)
			{
				PlayishManager.getInstance().onDeviceConnected(data);
			}
		});
	}

	public void onDeviceDisconnected(string data)
	{
		Dispatcher.getInstance().Invoke(() =>
		{
			if (Application.isPlaying)
			{
				PlayishManager.getInstance().onDeviceDisconnected(data);
			}
		});
	}

	public void onDeviceInput(string data)
	{
		Dispatcher.getInstance().Invoke(() =>
		{
			if (Application.isPlaying)
			{
				PlayishManager.getInstance().onDeviceInput(data);
			}
		});
	}

	public void onDeviceSync(string data)
	{
		Dispatcher.getInstance().Invoke(() =>
		{
			if (Application.isPlaying)
			{
				PlayishManager.getInstance().onDeviceSync(data);
			}
		});
	}

	public void onPause()
	{
		Dispatcher.getInstance().Invoke(() =>
		{
			if (Application.isPlaying)
			{
				PlayishManager.getInstance().onPause();
			}
		});
	}

	public void onResume()
	{
		Dispatcher.getInstance().Invoke(() =>
		{
			if (Application.isPlaying)
			{
				PlayishManager.getInstance().onResume();
			}
		});
	}
}

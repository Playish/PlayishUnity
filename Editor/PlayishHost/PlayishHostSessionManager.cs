using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Playish
{
	[InitializeOnLoad]
	public class PlayishHostSessionManager
	{
		static PlayishHostSession session = null;
		static bool wasPlaying = false;
		

		static PlayishHostSessionManager()
		{
			EditorApplication.playmodeStateChanged += onPlaymodeStateChanged;
		}

		static void onPlaymodeStateChanged()
		{
			if (EditorApplication.isPlaying && !EditorApplication.isPaused && !wasPlaying)
			{
				session = new PlayishHostSession();
			}
			else if (!EditorApplication.isPlaying && !EditorApplication.isPaused && session != null)
			{
				session.dispose();
				session = null;
			}

			wasPlaying = EditorApplication.isPlaying;
		}
	}
}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Playish
{

public interface IPlayishInput
{
	/** Type of the input. Button, D-Pad, Gyro, etc. */
	string GetTypeName();

	/** Name of the input. */
	string GetName();

	void Update(string[] values);

	IPlayishInput Create();
}

}
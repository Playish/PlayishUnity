using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Playish;

public class CubeMover : MonoBehaviour
{
	public GameObject buttonback;
	public GameObject buttonselect;
	public GameObject buttonup;
	public GameObject buttondown;
	public GameObject buttonleft;
	public GameObject buttonright;

	public String playerDeviceId = "";

	private DeviceManager deviceManager;
	private PlayishManager playishManager;

	private Rigidbody rigidBody;


	// Use this for initialization
	void Start ()
	{
		deviceManager = DeviceManager.getInstance ();
		playishManager = PlayishManager.getInstance ();

		rigidBody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		var device = deviceManager.getDevice (playerDeviceId);
		if (device == null || !device.hasController ())
		{
			rigidBody.velocity = Vector3.zero;
			return;
		}

		var newVelocity = new Vector3 (0, 0, 0);

		var rotationInput = new Quaternion (
			-device.getFloatInput ("rotationX"),
			-device.getFloatInput ("rotationZ"),
			-device.getFloatInput ("rotationY"),
			device.getFloatInput ("rotationW"));
		
		transform.rotation = rotationInput;

		if (device.getBoolInput ("buttonleft"))
		{
			newVelocity.x = -1;
			buttonleft.transform.localScale = new Vector3 (buttonleft.transform.localScale.x, 0.4f, buttonleft.transform.localScale.z);
		}
		else
		{
			buttonleft.transform.localScale = new Vector3 (buttonleft.transform.localScale.x, 1f, buttonleft.transform.localScale.z);
		}

		if (device.getBoolInput ("buttonright"))
		{
			newVelocity.x = 1;
			buttonright.transform.localScale = new Vector3 (buttonright.transform.localScale.x, 0.4f, buttonright.transform.localScale.z);
		}
		else
		{
			buttonright.transform.localScale = new Vector3 (buttonright.transform.localScale.x, 1f, buttonright.transform.localScale.z);
		}

		if (device.getBoolInput ("buttonup"))
		{
			newVelocity.y = 1;
			buttonup.transform.localScale = new Vector3 (buttonup.transform.localScale.x, 0.4f, buttonup.transform.localScale.z);
		}
		else
		{
			buttonup.transform.localScale = new Vector3 (buttonup.transform.localScale.x, 1f, buttonup.transform.localScale.z);
		}

		if (device.getBoolInput ("buttondown"))
		{
			newVelocity.y = -1;
			buttondown.transform.localScale = new Vector3 (buttondown.transform.localScale.x, 0.4f, buttondown.transform.localScale.z);
		}
		else
		{
			buttondown.transform.localScale = new Vector3 (buttondown.transform.localScale.x, 1f, buttondown.transform.localScale.z);
		}

		if (device.getBoolInput ("buttonback"))
		{
			buttonback.transform.localScale = new Vector3 (buttonback.transform.localScale.x, 0.4f, buttonback.transform.localScale.z);
		}
		else
		{
			buttonback.transform.localScale = new Vector3 (buttonback.transform.localScale.x, 1f, buttonback.transform.localScale.z);
		}

		if (device.getBoolInput ("buttonselect"))
		{
			buttonselect.transform.localScale = new Vector3 (buttonselect.transform.localScale.x, 0.4f, buttonselect.transform.localScale.z);
		}
		else
		{
			buttonselect.transform.localScale = new Vector3 (buttonselect.transform.localScale.x, 1f, buttonselect.transform.localScale.z);
		}

		var accx = -device.getFloatInput ("accelerationX");
		var accy = -device.getFloatInput ("accelerationZ");
		var accz = -device.getFloatInput ("accelerationY");

		/*
		newVelocity.x += accx * 1f;
		newVelocity.y += accy * 1f;
		newVelocity.z += accz * 1f;
		*/

		rigidBody.velocity = newVelocity;
	}
}

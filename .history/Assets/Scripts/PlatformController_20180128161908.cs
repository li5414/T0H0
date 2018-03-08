﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController {

	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 velocity = MovieTexture * Time.deltaTime;
		transform.Translate(velocity);
	}
}

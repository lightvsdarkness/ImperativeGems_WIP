﻿using System;
using UnityEngine;
using System.Collections;

namespace Tenkoku.Demo
{
    public class ui_devFPS : MonoBehaviour
	{

	public UnityEngine.UI.Text textObj_fps;
	public bool showFPS = true;

	private float updateInterval = 0.5f;
	private float accum = 0.0f; // FPS accumulated over the interval
	private float frames  = 0f; // Frames drawn over the interval
	private float timeleft; // Left time for current interval


	void Start () {
		InvokeRepeating("SetType",0.1f,0.5f);
	}

	void LateUpdate () {

		// CALCULATE FPS
		if (showFPS){
		    timeleft -= Time.deltaTime;
		    accum += Time.timeScale/Time.deltaTime;
		    ++frames;
		   

		    // Interval ended - update GUI text and start new interval
		    if( timeleft <= 0.0f )
		    {
		        // display two fractional digits (f2 format)
		        timeleft = updateInterval;
		        accum = 0.0f;
		        frames = 0f;
		    }
		} else {
			textObj_fps.text = "";
		}

	}


	void SetType(){
	   	if (textObj_fps != null &&  accum > 0f && frames > 0f){
			textObj_fps.text = "FPS: "+(accum/frames).ToString("f0");
		}
	}

}
}
using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System;
using UnityEngine.UI;

public class testSerial : MonoBehaviour {
	SerialPort test = new SerialPort("COM3", 115200 );
	int buff = 0;
	// Use this for initialization
	void Start () {
		test.ReadTimeout = 20;
	}
	
	// Update is called once per frame
	void Update () {
		if(test.IsOpen){
			try{
				buff =  int.Parse(test.ReadLine());
			//String testBuff = buff.Split(',');
				Debug.Log (buff);
				buff += 1000;
				test.WriteLine(buff.ToString());
				}catch(TimeoutException){
					Debug.Log ("time out");
				}
		}
	}
}

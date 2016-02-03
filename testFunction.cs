using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.IO.Ports;

public class testFunction : MonoBehaviour {
	public int[] sensorValue = {0,0,0,0,1,1,1,1};
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		InputSensorValue ();
	}

	void InputSensorValue(){	//それぞれ0~7を入力するとセンサーの値が切り替わる
		if(Input.GetKeyDown(KeyCode.Alpha0))
			ChangeSensorValue(ref sensorValue[0]);
		if(Input.GetKeyDown(KeyCode.Alpha1))
			ChangeSensorValue(ref sensorValue[1]);
		if(Input.GetKeyDown(KeyCode.Alpha2))
			ChangeSensorValue(ref sensorValue[2]);
		if(Input.GetKeyDown(KeyCode.Alpha3))
			ChangeSensorValue(ref sensorValue[3]);
		if(Input.GetKeyDown(KeyCode.Alpha4))
			ChangeSensorValue(ref sensorValue[4]);
		if(Input.GetKeyDown(KeyCode.Alpha5))
			ChangeSensorValue(ref sensorValue[5]);
		if(Input.GetKeyDown(KeyCode.Alpha6))
			ChangeSensorValue(ref sensorValue[6]);
		if(Input.GetKeyDown(KeyCode.Alpha7))
			ChangeSensorValue(ref sensorValue[7]);

		string values = string.Format ("{0},{1},{2},{3},{4},{5},{6},{7}",
		                               sensorValue[0],sensorValue[1],sensorValue[2],sensorValue[3],
		                               sensorValue[4],sensorValue[5],sensorValue[6],sensorValue[7]);
		Debug.Log (values);
		//print (values);

	}
	void ChangeSensorValue(ref int value){
		if (value == 0)
			value = 1;
		else
			value = 0;
	}

	public void testPressValue(ref int[] _values){
		_values [4] = 1;
		_values [5] = 1;
		_values [6] = 1;
		_values [7] = 1;
	}

	public int [] GetSensorValues(){
		return sensorValue;
	}

	public void testSerialWrite(SerialPort _stream){
		if (_stream.IsOpen) {
			if (Input.GetKeyDown (KeyCode.Q)) {
				_stream.Write ("3");
				Debug.Log ("3");
			} else if (Input.GetKeyUp (KeyCode.Q)) {
				_stream.Write ("4");
				Debug.Log ("4");
			}

			else if (Input.GetKeyDown (KeyCode.O)) {
				_stream.Write ("5");
				Debug.Log ("5");
			} else if (Input.GetKeyUp (KeyCode.O)) {
				_stream.Write ("6");
				Debug.Log ("6");
			}
		}
		else{
			Debug.Log("srial write");
		}
	}
}
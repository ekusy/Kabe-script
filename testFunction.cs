using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.IO.Ports;

public class testFunction : MonoBehaviour {
    const char DRILL_ROLL = '1';
    const char DRILL_STOP = '3';
    const char JACK_UP = '4';
    const char JACK_DOWN = '5';
    const char JACK_STOP = '6';
    const char WINCH_UP = '7';
    const char WINCH_DOWN = '8';
    const char WINCH_STOP = '9';
    const char ALL_STOP = '0';

    public int[] sensorValue = {1,1,1,1,1,1};
    bool changeFlg = false;
	// Use this for initialization
	void Start () {
        for(int i = 0; i < 6; i++)
        {
            sensorValue[i] = 1;
        }
    }
    // Update is called once per frame
    void Update () {
        getTestValue();
	}
    void getTestValue()
    {
        
        int value1 = (int)UnityEngine.Random.Range(0, 100);
        int value2 = (int)UnityEngine.Random.Range(0, 100);
        //Debug.Log(value1 + ":" + value2);
        
        while (value1 == value2)
        {
            value2 = (int)UnityEngine.Random.Range(0, 100);
        }
        

        if(changeFlg == false)
        {
            if(value1 < value2)
            {
                //Debug.Log("value1 < value2");
                int tmp = value1;
                value1 = value2;
                value2 = tmp;
            }
        }
        else
        {
            if (value1 > value2)
            {
                int tmp = value1;
                value1 = value2;
                value2 = tmp;
            }
        }
        sensorValue[0] = value1;
        sensorValue[1] = value2;
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeSensorValue(ref sensorValue[2]);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeSensorValue(ref sensorValue[3]);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeSensorValue(ref sensorValue[4]);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            ChangeSensorValue(ref sensorValue[5]);
        //Debug.Log("getTestValue");
        string values = string.Format("{0},{1},{2},{3},{4},{5}",
                               sensorValue[0], sensorValue[1], sensorValue[2], sensorValue[3],
                               sensorValue[4], sensorValue[5]);
        //Debug.Log(values);

        if (Input.GetKeyDown(KeyCode.C))
            changeFlg = !changeFlg;
        //Debug.Log("changeFlg=" + changeFlg);

    }
	void ChangeSensorValue(ref int value){
		if (value == 0)
			value = 1;
		else
			value = 0;
	}
	public void testPressValue(ref int[] _values){
        _values[2] = 1;
        _values[3] = 1;
        _values [4] = 1;
		_values [5] = 1;
		
	}
	public int [] GetSensorValues(){
		return sensorValue;
	}
	public void testSerialWrite(SerialPort _stream){
		//if (_stream.IsOpen) {
			if (Input.GetKey (KeyCode.J)) {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    writeArduino(JACK_STOP,_stream);
                    Debug.Log("JACK_STOP");
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    writeArduino(JACK_DOWN,_stream);
                    Debug.Log("JACK_DOWN");
                }
                else if (Input.GetKeyDown(KeyCode.U))
                {
                    writeArduino(JACK_UP,_stream);
                    Debug.Log("JACK_UP");
                }
			}
            else if (Input.GetKey(KeyCode.W))
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    writeArduino(WINCH_STOP,_stream);
                    Debug.Log("WINCH_STOP");
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    writeArduino(WINCH_DOWN,_stream);
                    Debug.Log("WINCH_DOWN");
                }
                else if (Input.GetKeyDown(KeyCode.U))
                {
                    writeArduino(WINCH_UP,_stream);
                    Debug.Log("WINCH_UP");
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    writeArduino(DRILL_STOP,_stream);
                    Debug.Log("DRILL_STOP");
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    writeArduino(DRILL_ROLL,_stream);
                    Debug.Log("DRILL_ROLL");
                }
                
            }
        //}
	}
    void writeArduino(char _data,SerialPort stream1)
    {
        if (stream1.IsOpen)
        {
            while (true)
            {
                try
                {
                    stream1.Write(_data.ToString());

                }
                catch (TimeoutException e)
                {
                    Debug.Log("time out Write：" + _data);
                }
                Debug.Log("OK test serial,send:" + _data.ToString());
                break;
            }
        }
        else
        {
            Debug.Log("not connected serial");
        }
    }
}

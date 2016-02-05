using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System;
using UnityEngine.UI;

public class ArduinoSerialIOScript : MonoBehaviour {
    //arduinoから受け取るセンサの値の数
    const int VALUE_NUM = 6;

    //各センサの値の添字
    const int MOVE_RIGHT_HAND = 0;
    const int MOVE_LEFT_HAND = 1;
    const int PRESS_RIGHT_HAND = 2;
    const int PRESS_LEFT_HAND = 3;
    const int PRESS_RIGHT_LEG = 4;
    const int PRESS_LEFT_LEG = 5;

    //arduinoに送信する指示と値
    const char DRILL_ROLL = '1';
    const char DRILL_STOP = '3';
    const char JACK_UP = '4';
    const char JACK_DOWN = '5';
    const char JACK_STOP = '6';
    const char WINCH_UP = '7';
    const char WINCH_DOWN = '8';
    const char WINCH_STOP = '9';
    const char ALL_STOP = '0';

    //各動作と機構の対応
    const char FALL_START = DRILL_ROLL;
    const char FALL_STOP = DRILL_STOP;
    const char HEAD_UP = JACK_UP;
    const char HEAD_DOWN = JACK_DOWN;
    const char HEAD_STOP = JACK_STOP;
    const char HAMMOK_DOWN = WINCH_DOWN;
    const char HAMMOK_UP = WINCH_UP;
    const char HAMMOK_STOP = WINCH_STOP;

    //プレイヤーの状態
    const int MODE_WALL = 1;
    const int MODE_CEILING = 2;
    const int MODE_DANGER = 3;
    const int MODE_FALL = 4;



    int[] sensor = {0,0,1,1,1,1};//
	int[] preSensor = {0,0,0,0,0,0};//
	//int speed = 0;
    float speed = 0.0f;
    Boolean fall = false;
	int mode = 1;
	float theta = 360.0F;



	public AudioClip audio1,audio2,audio3,audio4,audio5;
	AudioSource audioSource;
	/*
	1 垂直モード
	2　水平モード
	3　落下モード

	A0,A4 = 左手
	A1,A5 = 右手
	A2,A6 = 左足
	A3,A7 = 右足

	*/

	//---------追加--------------
	testFunction tF;	//テスト用関数　センサー値をキーボードで入力
	moveFunction mF;	//移動用関数
	canvusEnable cE;	//GAMEOVERオブジェクト取得用
	GameObject gameOver;	//ゲームオーバーの表示用

	const float SPEED = 0.1f;	//テスト用移動スピード
	float fallTimer = 0.0f;	//落下までの時間カウント
	int nowMode = 1;	//落下前の状態を保持
	float fallAngle = 0.5f;	//落下時の回転速度
	float serialTimer = 0.0f;
	float[] drillTimer = {-1.0f,-1.0f,-1.0f};	//ドリルの動作時間計測
	float[] drill = {3.0f,0.5f,0.5f};	//

	bool first_fall = false;
	bool startFlg = false;
	bool safeFlg = true;
	//-----------------------



	SerialPort stream1 = new SerialPort("COM4", 9600 );
	//SerialPort stream2 = new SerialPort("COM4", 115200 );


	void Start () {
		//センサー入力をキーボードで代用するためシリアル通信停止
		OpenConnection (stream1);
		//各種スクリプトを使用可能に
		tF = GetComponent<testFunction> ();
		mF = GetComponent<moveFunction> ();
		gameOver = GameObject.Find ("GAMEOVER");
		cE = gameOver.GetComponent<canvusEnable> ();

		audioSource = gameObject.GetComponent<AudioSource>();

		first_fall = true;
	}
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			startFlg = true;
		}
        //--------arduinoから値を受け取るときはreadSensorを、テスト用の値を使うときはtF.GetSensorValuesを使う
		//readSensor ();
		try {
			sensor = tF.GetSensorValues ();	//センサー値(ダミー)を受け取り
		} catch (NullReferenceException) {
			Debug.Log ("error sensor");
		}
		if (safeFlg) {
			tF.testPressValue (ref sensor);
		}

		tF.testSerialWrite (stream1);
		//Debug.Log("4&5:"+sensor[4]+","+sensor[5]);

		if (startFlg) {
			//getSpeed();	//移動判定を別のスクリプトへ
			try {
                //speed = mF.getSpeed (sensor, preSensor);	//移動判定
                speed = mF.getSpeed(sensor); //移動判定
            } catch (NullReferenceException) {
				Debug.Log ("error speed");
				speed = 0;
			}
		} else {
			tF.testSerialWrite(stream1);
		}
		//Debug.Log ("speed="+speed);
		switch (mode) {
			case MODE_WALL:
			case MODE_CEILING:
				fallTimer = 0.0f;
				move ();
				break;
			case MODE_DANGER:	//落ちそうな状態
			fallTimer+=Time.deltaTime;	//手を離してからの時間を取得
			break;
			case MODE_FALL:	//落下
				Fall();
				if(serialTimer > 0.45f && serialTimer != -1.0f){
					writeArduino(FALL_STOP);
					serialTimer = -1.0f;
				}
				else {
					serialTimer+=Time.deltaTime;	
				}
				break;
		default:
			break;
		}
		//本番は消す　テスト用
		if (Input.GetKey (KeyCode.F)) {
			//Fall();
			mode=MODE_FALL;
		}
		
		touch_sound ();

		if (drillTimer[0] != -1.0f) {
			drillTimer[0]+=Time.deltaTime;
			if(drillTimer[0] > drill[0]){
				if(mode == MODE_WALL){
					writeArduino(HEAD_STOP);
					drillTimer[0] = -1.0f;
				}
				else if(mode == MODE_CEILING){
					writeArduino(HEAD_STOP);
					drillTimer[0] = -1.0f;
				}
			}
			if(mode!=MODE_CEILING && mode != MODE_WALL){
				writeArduino(HEAD_STOP);
				drillTimer[0] = -1.0f;
			}
		}

		judge_fall ();
		//print (fallTimer);	//手を離してからの時間を表示

		
		for(int i = 0;i<VALUE_NUM;i++){
			preSensor[i] = sensor[i];
		}

	}
	void judge_fall(){
		if (sensor [PRESS_LEFT_HAND] == 1 && sensor [PRESS_RIGHT_HAND] == 1 && sensor[PRESS_LEFT_LEG] == 1 && sensor[PRESS_RIGHT_LEG] == 1 ) {
			//全部手足ついてる状態
			Debug.Log ("All");
			mode = nowMode;		
		} 
		else if(sensor [PRESS_LEFT_HAND] == 1 && sensor[PRESS_RIGHT_LEG] == 1 || sensor [PRESS_RIGHT_HAND] == 1 && sensor[PRESS_LEFT_LEG] == 1){
			//対角線手足セットパターン
			Debug.Log ("Diagnoal");
			mode = nowMode;
		}
		else if(sensor[PRESS_RIGHT_HAND] == 1 || sensor[PRESS_LEFT_HAND] == 1){
			//手テスト
			Debug.Log("ok");
			mode = nowMode;
		}
		else if(mode < 3){
			//落下しそう
			Debug.Log("Rerease");
			mode = 3;

			//Fall();
		}

		if (mode == 3 && fallTimer > 1.0f) {
			Debug.Log ("Fall");

			mode = 4;
			nowMode = 4;
			while(true){
				try{
					Debug.Log ("write_Start");
					writeArduino(FALL_START);

				}
				catch(TimeoutException){
					Debug.Log ("error write");
				}
			break;
			}
		}
	}
	void touch_sound(){
		int num = UnityEngine.Random.Range (0, 4);
		if (Input.GetKeyDown (KeyCode.A) == true) {
			switch(num){
			case 0:
				audioSource.PlayOneShot(audio1);
				break;
			case 1:
				audioSource.PlayOneShot(audio2);
				break;
			case 2:
				audioSource.PlayOneShot(audio3);
				break;
			case 3:
				audioSource.PlayOneShot(audio4);
				break;
			}


		}
	}
	void Fall(){
		Debug.Log ("fall start");
		if (!audioSource.isPlaying) {
			audioSource.clip = audio5;
			audioSource.Play ();
		}
		if(theta > 270){
			theta = theta - fallAngle;	//後ろに倒れる速度を変更できるように
		}
		transform.localRotation = Quaternion.Euler(theta, 270, 0);
		Rigidbody rb = GetComponent<Rigidbody> ();
		rb.useGravity = true;

//		if(mode != nowMode)
			mode = 4;

		Debug.Log ("fall end");

	}

	void OnCollisionEnter(Collision c){
		if (c.gameObject.tag == "Kabe1") {  //最初の壁から天井へ
			Destroy(c.gameObject);  //検出用のオブジェクトを消去
			transform.localRotation = Quaternion.Euler(270, 270, 0);    //プレイヤーの角度を変更
			mode = 2;   //モードを天井に
			safeFlg = false;    //落下するように

			nowMode = mode; //現在のモード保持変数にモードを代入
			drillTimer[0] = 0.0f;   //頭上下用のドリルのタイマー起動
			writeArduino(HEAD_DOWN);    //ドリルを動かして頭を下げる
		}
		else if (c.gameObject.tag == "Kabe2") { //天井から壁へ
			Destroy(c.gameObject);  //検出用オブジェクトを消去
			transform.localRotation = Quaternion.Euler(0, 270, 0);  //プレイヤーの角度を変更   
			mode = 1;   //モードを壁に

			//正転　下がる
			//逆転　上がる
			nowMode = mode; //現在のモード保持変数にモードを代入

			drillTimer[0] = 0.0f;   //頭上下用のドリルのタイマー起動
			writeArduino(HEAD_UP);  //ドリルを動かして頭を上げる

		}
		else if (c.gameObject.tag == "Kabe3") { //Kabe3の用途とは一体…
			Destroy(c.gameObject);  //

		}
		else if (c.gameObject.tag == "Yuka") {  //地面に接触したら
			audioSource.Stop(); //音（環境音？）を停止
			cE.enableCanvus();	//ゲームオーバーの表示
			fallAngle = 30.0f;	//後ろに倒れる速度を加速
		}
	}
	void readSensor(){
		string result1="";
        try
        {
            result1 = stream1.ReadLine();
            //Debug.Log(result1);
            string[] tmpSen = result1.Split(',');
            if (tmpSen.Length == 6)
            {
                try
                {
                    for (int i = 0; i < VALUE_NUM; i++)
                    {
                        sensor[i] = int.Parse(tmpSen[i]);
                        //Debug.Log ("sen"+i+" "+sensor[i]);
                    }
                    Debug.Log("getValue");
                }
                catch (FormatException)
                {
                }
                catch (IndexOutOfRangeException)
                {
                }
            }
        }
        catch (TimeoutException)
        {
            Debug.Log("time out read");
        }

	}
	void writeArduino(int data){
        if (stream1.IsOpen)
        {
            Debug.Log("writeArduino");
            stream1.Write(data.ToString());
            //stream1.Write ("1");
            Debug.Log("data=" + data + ",mode=" + mode);
            //stream1.Write ("2");
        }
        else
        {
            Debug.Log("not connected serial");
        }
	
	}
    void writeArduino(char _data)
    {
        if (stream1.IsOpen)
        {
            while (true)
            {
                try
                {
                    stream1.Write(_data.ToString());
                    Debug.Log("write：" + _data + " mode：" + mode);
                    
                }
                catch(TimeoutException e)
                {
                    Debug.Log("time out Write："+_data);
                }
                break;
            }
        }
        else
        {
            Debug.Log("not connected serial");
        }
    }
    void streamClear(){

		//stream2.WriteLine ("");
	}
    void move(){
		if(!fall){
			if( speed > 0){//Input.GetKey(KeyCode.Space )|| speed >0){
				switch(mode){
				case 1:
					transform.localPosition += new Vector3(0,speed,0);
					break;
				case 2:
					transform.localPosition += new Vector3(speed,0,0);
					break;
				}
			}
		}

	}
	void OnApplicationQuit(){
		stream1.Close();

		//stream2.Close ();

	}
    void OpenConnection(SerialPort _stream) {
		if (_stream != null) {
			if (_stream.IsOpen) {
				_stream.Close ();
				Debug.LogError ("Failed to open Serial Port, already open!");
			} else {
				_stream.Open ();
				_stream.ReadTimeout = 10;
				_stream.WriteTimeout = 10;
				Debug.Log ("Open Serial port1");      
			}
		}

	}

}
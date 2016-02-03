using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System;
using UnityEngine.UI;

public class ArduinoSerialIOScript : MonoBehaviour {
	int[] sensor = {0,0,0,0,1,1,0,0};//
	int[] preSensor = {0,0,0,0,0,0,0,0};//
	int speed = 0;
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
	float[] drill = {4.5f,4.5f,0.5f};	//
	
	bool first_fall = false;
	bool startFlg = false;
	bool safeFlg = true;
	bool moveFlg = true;
	//-----------------------
	
	
	
	SerialPort stream1 = new SerialPort("COM3", 115200 ); 
	//SerialPort stream2 = new SerialPort("COM4", 115200 );
	
	
	void Start () {
		//センサー入力をキーボードで代用するためシリアル通信停止
		//OpenConnection (stream1);
		//各種スクリプトを使用可能に
		tF = GetComponent<testFunction> ();
		mF = GetComponent<moveFunction> ();
		gameOver = GameObject.Find ("GAMEOVER");
		cE = gameOver.GetComponent<canvusEnable> ();
		audioSource = gameObject.GetComponent<AudioSource>();
		
		first_fall = true;
	}
	void Update () {
		if (Input.GetKeyDown (KeyCode.S)) {
			startFlg = true;
		}
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
				speed = mF.getSpeed (sensor, preSensor);	//移動判定
			} catch (NullReferenceException) {
				Debug.Log ("error speed");
				speed = 1;
			}
		} else {
			tF.testSerialWrite(stream1);
		}
		//Debug.Log ("speed="+speed);
		//本番は消す　テスト用
		if (Input.GetKeyDown(KeyCode.F)) {
			//Fall();
			mode=3;
			fallTimer = 2.0f;
		}
		switch (mode) {
		case 1:
		case 2:
			fallTimer = 0.0f;
			move ();
			break;
		case 3:	//落ちそうな状態
			fallTimer+=Time.deltaTime;	//手を離してからの時間を取得
			break;
		case 4:	//落下
			Fall();
			if(serialTimer > 0.45f && serialTimer != -1.0f){
				writeArduino(2);
				serialTimer = -1.0f;
			}
			else {
				serialTimer+=Time.deltaTime;	
			}
			break;
		default:
			break;
		}
		
		
		touch_sound ();
		if (drillTimer[0] != -1.0f) {
			drillTimer[0]+=Time.deltaTime;
			if(drillTimer[0] > drill[0]){
				if(mode == 1){
					writeArduino(4);
					drillTimer[0] = -1.0f;
				}
				else if(mode == 2){
					writeArduino(6);
					drillTimer[0] = -1.0f;
				}
			}
			if(mode>2){
				writeArduino(6);
				drillTimer[0] = -1.0f;
			}
		}
		
		judge_fall ();
		print (fallTimer);	//手を離してからの時間を表示
		
		
		for(int i = 0;i<8;i++){
			preSensor[i] = sensor[i];
		}
		
	}
	void judge_fall(){
		//A4 左手 A5 右手　A6左足 A7右足として　コーディングしている
		if (sensor [4] == 1 && sensor [5] == 1 && sensor[6] == 1 && sensor[7] == 1 ) {
			//全部手足ついてる状態
			Debug.Log ("All");
			mode = nowMode;		
		} 
		else if(sensor [4] == 1 && sensor[7] == 1 || sensor [5] == 1 && sensor[6] == 1){
			//対角線手足セットパターン
			Debug.Log ("Diagnoal");
			mode = nowMode;
		}
		if(sensor[4] == 1 || sensor[5] == 1){
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
					writeArduino(1);
					writeArduino(7);

					
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
		if (theta > 270) {
			theta = theta - fallAngle;	//後ろに倒れる速度を変更できるように
		} else {
			mode = 5;
		}
		transform.localRotation = Quaternion.Euler(theta, 270, 0);
		Rigidbody rb = GetComponent<Rigidbody> ();
		rb.useGravity = true;
		
		//		if(mode != nowMode)
		mode = 4;
		
		Debug.Log ("fall end");
		
	}
	
	void OnCollisionEnter(Collision c){
		if (c.gameObject.tag == "Kabe1") {
			Destroy (c.gameObject);
			transform.localRotation = Quaternion.Euler (270, 270, 0);
			mode = 2;
			safeFlg = false;
			
			nowMode = mode;
			Debug.Log ("write = 5");
			drillTimer [0] = 0.0f;
			writeArduino (5);
		} else if (c.gameObject.tag == "Kabe2") {
			Debug.Log ("colision Kabe2:1");
			writeArduino (6);
			Destroy (c.gameObject);
			transform.localRotation = Quaternion.Euler (0, 270, 0);
			mode = 1;
			Debug.Log ("colision Kabe2;2");
			//正転　下がる
			//逆転　上がる
			nowMode = mode;
			Debug.Log ("write = 3");
			drillTimer [0] = 0.0f;
			writeArduino (3);
			
		} else if (c.gameObject.tag == "Kabe3") {
			Destroy (c.gameObject);
			
		} else if (c.gameObject.tag == "Yuka") {
			audioSource.Stop ();
			cE.enableCanvus ();	//ゲームオーバーの表示
			fallAngle = 30.0f;	//後ろに倒れる速度を加速
			writeArduino(8);
		} else if (c.gameObject.tag == "Goal") {
			moveFlg = false;
		}
	}
	
	
	void readSensor(){
		string result1="";
		try{
			result1 = stream1.ReadLine();
			//Debug.Log(result1);
			string[] tmpSen = result1.Split(',');
			
			try{
				for (int i = 0; i<8; i++) {
					sensor[i] = int.Parse(tmpSen[i]);
					//Debug.Log ("sen"+i+" "+sensor[i]);
				}
				Debug.Log ("getValue");
			}
			catch(FormatException){
			}
			catch(IndexOutOfRangeException){
			}
		}
		catch(TimeoutException){
			Debug.Log ("time out");
		}
	}
	
	
	// 代入されたmodeの値に基づいてarduino DUOに首に関する信号を送る。
	void writeArduino(int data){
		if (stream1.IsOpen) {
			Debug.Log ("writeArduino");
			while (true) {
				try {
					stream1.Write (data.ToString ());
					Debug.Log ("data=" + data + ",mode=" + mode);
					break;
				} catch (TimeoutException) {
					Debug.Log ("write timeout");
				}//stream1.Write ("1");
			}
		
			Debug.Log ("data=" + data + ",mode=" + mode + " complete");
			//stream1.Write ("2");
		} else {
			Debug.Log ("not connected Arduino");
		}
		
	}
	
	void streamClear(){
		
		//stream2.WriteLine ("");
	}
	
	//-------------使用していない,moveFunction.csに移動---------
	void getSpeed(){
		//A0 左手 A1 右手　A2左足 A3右足として　コーディングしている
		if (sensor[1] != preSensor[1] && sensor[2] != preSensor[2]
		    ||sensor[0] != preSensor[0] && sensor[3] != preSensor[3]) {
			speed++;
		}
		else if(sensor[2] != preSensor[2] || sensor[3] != preSensor[3]){
			//speed+=10;
		}
		else if(speed != 0){
			speed--;
		}
	}
	//---------------------------------------------
	
	void move(){
		if(!fall){
			if( speed > 0 && moveFlg){//Input.GetKey(KeyCode.Space )|| speed >0){
				switch(mode){
				case 1:
					transform.localPosition += new Vector3(0,SPEED,0);	//移動速度を定数化,テスト用
					break;
				case 2:
					transform.localPosition += new Vector3(SPEED,0,0);
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
				_stream.WriteTimeout = 40;
				
				Debug.Log ("Open Serial port1");      
			}
		}
		
	}
	
}
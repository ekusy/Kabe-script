using UnityEngine;
using System.Collections;

public class moveFunction : MonoBehaviour {
	const int LEFT_HAND = 1;
	const int RIGHT_HAND = 0;
	private int[] moveCount = {0,0};
	private int speed = 0;
	motionScript mS;
	// Use this for initialization
	//移動の処理が複雑化するかもしれないので別のスクリプトにしていた
	void Start () {
		
		mS = GetComponent<motionScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int getSpeed(int[] _values,int[] _preValues){	//センサー値と前のフレームのセンサー値が引数
		Debug.Log ("getSpeed");
		//A0 右手 A1 左手　A2右足 A3左足として　コーディングしている
		if(_values[LEFT_HAND] != _preValues[LEFT_HAND] ){
			if(moveCount[LEFT_HAND] != 5){	//左手を何度も動かしていなければ
				if(moveCount[LEFT_HAND]==0)
					mS.setL();
				moveCount[LEFT_HAND]++;		//左手を動かした回数加算
				speed=10;				//スピード設定(今のところ速度は一定値)
				Debug.Log ("LEFT_HAND");
			}
			moveCount[RIGHT_HAND] = 0;		//右手を動かした回数初期化
		}
		else if(_values[RIGHT_HAND] != _preValues[RIGHT_HAND] ){	//右手も同じ処理
			if(moveCount[RIGHT_HAND] != 5){
				if(moveCount[RIGHT_HAND]==0)
					mS.setR();
				moveCount[RIGHT_HAND]++;
				speed=10;
			}
			moveCount[LEFT_HAND] = 0;
		}
		else if(speed != 0){
			speed--;
		}

		return speed;
	}
}


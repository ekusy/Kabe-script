using UnityEngine;
using System.Collections;

public class moveFunction : MonoBehaviour {
	const int LEFT_HAND = 1;
	const int RIGHT_HAND = 0;
    const int MOVE_LIMIT = 3000;
    const int GET_LIMIT = 10;
	private int[] moveCount = {0,0};
    private int[] getCount = { 0, 0 };
	private int speed = 0;
    
	// Use this for initialization
	//移動の処理が複雑化するかもしれないので別のスクリプトにしていた
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int getSpeed(int[] _values,int[] _preValues){	//センサー値と前のフレームのセンサー値が引数
		Debug.Log ("getSpeed");
		//A0 右手 A1 左手　A2右足 A3左足として　コーディングしている
		if(_values[LEFT_HAND] != _preValues[LEFT_HAND] ){
			if(moveCount[LEFT_HAND] != 5){	//左手を何度も動かしていなければ
				moveCount[LEFT_HAND]++;		//左手を動かした回数加算
				speed=10;				//スピード設定(今のところ速度は一定値)
				Debug.Log ("LEFT_HAND");
			}
			moveCount[RIGHT_HAND] = 0;		//右手を動かした回数初期化
		}
		else if(_values[RIGHT_HAND] != _preValues[RIGHT_HAND] ){	//右手も同じ処理
			if(moveCount[RIGHT_HAND] != 5){
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

    public float getSpeed(int[] _values)
    {
        int rightValue = _values[0];
        int leftValue = _values[1];
        float Speed = 0.0f;
        Debug.Log("getSpeed motor");
        //Debug.Log("moveRight:"+ moveCount[RIGHT_HAND]+" getRight:"+ getCount[RIGHT_HAND]);
        if (rightValue >= leftValue && moveCount[RIGHT_HAND] < MOVE_LIMIT && getCount[RIGHT_HAND] < GET_LIMIT)
        {
            Debug.Log("reset Left");
            if (moveCount[LEFT_HAND] > 0 && getCount[LEFT_HAND] > 0)
            {
                Debug.Log("left move =" + moveCount[LEFT_HAND] + "left count = "+ getCount[LEFT_HAND]);
            }
            moveCount[LEFT_HAND] = 0;
            getCount[LEFT_HAND] = 0;
            moveCount[RIGHT_HAND] += rightValue;
            getCount[RIGHT_HAND]++;
            Speed = rightValue;
        }
        else if ( leftValue > rightValue && moveCount[LEFT_HAND] < MOVE_LIMIT && getCount[LEFT_HAND] < GET_LIMIT)
        {
            Debug.Log("reset Right");
            if (moveCount[RIGHT_HAND] > 0 && getCount[RIGHT_HAND] > 0)
            {
                Debug.Log("right move =" + moveCount[RIGHT_HAND] + "right count = " + getCount[RIGHT_HAND]);
            }
            moveCount[RIGHT_HAND] = 0;
            getCount[RIGHT_HAND] = 0;
            moveCount[LEFT_HAND] += leftValue;
            getCount[LEFT_HAND]++;
            Speed = leftValue;
        }
        return Speed/500.0f;
    }
}


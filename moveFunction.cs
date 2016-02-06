using UnityEngine;
using System.Collections;

public class moveFunction : MonoBehaviour {
	const int LEFT_HAND = 1;    //配列の左手の添字
	const int RIGHT_HAND = 0;   //配列の右手の添字
    const int MOVE_LIMIT = 3000;    //片手だけの移動量限界（センサー値の累計）
    const int GET_LIMIT = 20;   //片手だけの移動回数限界（フレーム数）
	private int[] moveCount = {0,0};    //移動量
    private int[] getCount = { 0, 0 };  //移動回数
    private int zeroCount = 0;  //両方が０になっているフレーム数
    
	// Use this for initialization
	//移動の処理が複雑化するかもしれないので別のスクリプトにしていた
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float getSpeed(int[] _values)
    {
        int rightValue = _values[0];
        int leftValue = _values[1];
        if(rightValue < 10)
        {
            rightValue = 0;
        }
        if(leftValue < 10)
        {
            leftValue = 0;
        }
        float Speed = 0.0f;
        Debug.Log("getSpeed motor");
        //Debug.Log("moveRight:"+ moveCount[RIGHT_HAND]+" getRight:"+ getCount[RIGHT_HAND]);
        if (rightValue > leftValue && moveCount[RIGHT_HAND] < MOVE_LIMIT && getCount[RIGHT_HAND] < GET_LIMIT)
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
        else if(rightValue == leftValue  && rightValue == 0)
        {
            zeroCount++;
        }
        if (zeroCount > 10)
        {
            zeroCount = 0;
            moveCount[RIGHT_HAND] = 0;
            getCount[RIGHT_HAND] = 0;
            moveCount[LEFT_HAND] = 0;
            getCount[LEFT_HAND] = 0;
        }
        return Speed/1000.0f;
    }
}


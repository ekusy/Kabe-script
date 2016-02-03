using UnityEngine;
using System.Collections;

public class canvusEnable : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Canvas>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void enableCanvus(){	//文字の表示
		GetComponent<Canvas>().enabled = true;
	}

	public void disableCanvus(){	//文字を非表示
		GetComponent<Canvas>().enabled = false;
	}
}

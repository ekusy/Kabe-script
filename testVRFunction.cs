using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class testVRFunction : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Rキーで位置トラッキングをリセットする
		if (Input.GetKeyDown(KeyCode.R)) {
			InputTracking.Recenter();
			Debug.Log ("Reset");
		}
	}
}

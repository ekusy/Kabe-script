using UnityEngine;
using System.Collections;

public class motionScript : MonoBehaviour {
	public Animator motion;// = GetComponent<Animator>();
	public AnimatorStateInfo state;
	bool flg = false;
	int motionNum = 0;
	// Use this for initialization
	void Start () {
		motion.speed = 2.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
		state = motion.GetCurrentAnimatorStateInfo (0);
		//if(Input.GetKeyDown("space") ){
		if (motionNum == 1) {
			if (state.IsName ("Locomotion.Idle")) {
				Debug.Log ("space true");
				motion.SetBool ("space", true);
			} else if (state.IsName ("Locomotion.L1")) {
				if (flg == false) {
					Debug.Log ("space1 true");
					motion.SetBool ("space1", true);
				}
			
			} else if (state.IsName ("Locomotion.L2")) {
				Debug.Log ("space2 true");
				motion.SetBool ("space2", true);
				motionNum = 0;
			}
		}
		if(motionNum == 2){
			if(state.IsName("Locomotion.Idle2")){
				motion.SetBool("space3", true);
			}
			else if(state.IsName("Locomotion.R1")){
				motion.SetBool("space4", true);
			}
			else if(state.IsName("Locomotion.R2")){
				motion.SetBool("space", false);
				motion.SetBool("space1", false);
				motion.SetBool("space2", false);
				motion.SetBool("space3", false);
				motion.SetBool("space4", false);
				motionNum = 0;
			}
		}

	}

	public void setL(){
		motionNum = 1;
	}

	public void setR(){
		motionNum = 2;
	}
}

using UnityEngine;
using System.Collections;

public class motionFunction : MonoBehaviour {
    private Animator m_animator;
    private float frame = 0.0f;
    private float add = 0.0f;
    int a = 1;
    int count = 0;
    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("frame=" + frame + ",add=" + add);
        
        if (frame >= 76.0f)
        {
            frame = 60.0f;
        }
        
        setAnimationFrame(frame);
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("input plus");
            if (frame < 76)
                frame++;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("input minus");
            if (frame > 0)
                frame--;
        }
        //Debug.Log(frame);

    }

    public void setAnimationFrame(float _frame)
    {
        AnimatorClipInfo[] clipInfoList = m_animator.GetCurrentAnimatorClipInfo(0);
        AnimationClip clip = clipInfoList[0].clip;
        float time = _frame / (clip.length*clip.frameRate);
        Debug.Log("time="+time+",frame="+_frame);
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        int animationHash = stateInfo.GetHashCode();

        m_animator.Play(0, 0, time);
    }
    public void addFunc(float _add)
    {
        frame += 0.4f;// _add*24/60;
    }
}

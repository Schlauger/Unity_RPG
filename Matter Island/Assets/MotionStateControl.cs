using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionStateControl : MonoBehaviour {

    Animator animator;
    int isWalkingHash;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    void Update(){
        bool isWalking = animator.GetBool(isWalkingHash);
        bool forward = Input.GetKey("w");
        if ( !isWalking && forward){
            animator.SetBool(isWalkingHash, true);
        }
        if (isWalking && !forward){
            animator.SetBool(isWalkingHash, false);
        }
    }
}

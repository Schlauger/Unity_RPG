using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionStateControl : MonoBehaviour {

    Animator animator;
    float zAxis = 0.0f;
    float xAxis = 0.0f;
    float maxWalkSpeed = 0.5f;
    float maxRunSpeed = 3.0f;
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    int xHash;
    int zHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        xHash = Animator.StringToHash("xAxis");
        zHash = Animator.StringToHash("zAxis");
    }

    
    // Update is called once per frame
    void Update()
    {
        bool forward = Input.GetKey(KeyCode.W);
        bool backward = Input.GetKey(KeyCode.S);
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);
        bool runShift = Input.GetKey(KeyCode.LeftShift);
        float currentMaxSpeed = runShift ? maxRunSpeed : maxWalkSpeed;

        updateVelocity(forward, backward, left, right, runShift, currentMaxSpeed);
        setVelocity(forward, backward, left, right, runShift, currentMaxSpeed);

        animator.SetFloat(zHash, zAxis);
        animator.SetFloat(xHash, xAxis);
    }

    void updateVelocity(bool forward, bool backward, bool left, bool right, bool runShift, float currentMaxSpeed){
        // bool isRunning = animator.GetBool(isRunningHash);
        // Accelerate
        if (forward && zAxis < currentMaxSpeed){
            zAxis += Time.deltaTime * acceleration;
        }
        if (backward && zAxis > -currentMaxSpeed){
            zAxis -= Time.deltaTime * acceleration;
        }
        if (left && xAxis > -currentMaxSpeed){
            xAxis -= Time.deltaTime * acceleration;
        }
        if (right && xAxis < currentMaxSpeed){
            xAxis += Time.deltaTime * acceleration;
        }

        // decelerate
        if (!forward && zAxis > 0.0f){
            zAxis -= Time.deltaTime * deceleration;
        }
        if (!backward && zAxis < 0.0f){
            zAxis += Time.deltaTime * deceleration;
        }

        if (!left && xAxis < 0.0f){
            xAxis += Time.deltaTime * deceleration;
        }

        if (!right && xAxis > 0.0f){
            xAxis -= Time.deltaTime * deceleration;
        }

    }

    void setVelocity(bool forward, bool backward, bool left, bool right, bool runShift, float currentMaxSpeed) {
         // reset zAxis
        // if (!forward && zAxis < 0.0f){
        //     zAxis = 0.0f;
        // }
        // reset zAxis
         if (!forward && !backward && zAxis != 0.0f && (zAxis>-0.05f && zAxis<0.05f)){
            zAxis = 0.0f;
        }

        // reset xAxis
         if (!left && !right && xAxis != 0.0f && (xAxis>-0.05f && xAxis<0.05f)){
            xAxis = 0.0f;
        }
       // set zAxis
        if (forward && runShift && zAxis>currentMaxSpeed){
            zAxis = currentMaxSpeed;
        }else if (forward && zAxis > currentMaxSpeed) {
            zAxis -= Time.deltaTime * deceleration;

            if(zAxis>currentMaxSpeed && zAxis<(currentMaxSpeed+ 0.05f)){
                zAxis = currentMaxSpeed;
            }
        }else if (forward && zAxis < currentMaxSpeed && zAxis>(currentMaxSpeed-0.05f)) {
            zAxis = currentMaxSpeed;
        }
        // backward
        if (backward && runShift && zAxis < -currentMaxSpeed) {
            zAxis = -currentMaxSpeed;
        }else if (backward && zAxis < -currentMaxSpeed) {
            zAxis += Time.deltaTime * deceleration;

            if(zAxis<-currentMaxSpeed && zAxis<(-currentMaxSpeed - 0.05f)){
                zAxis = -currentMaxSpeed;
            }
        }else if (backward && zAxis > -currentMaxSpeed && zAxis<(-currentMaxSpeed+0.05f)) {
            zAxis = -currentMaxSpeed;
        }
        ///////////////////////////////////////////////////////////////
        // set xAxis left
        if(left && runShift && xAxis<-currentMaxSpeed){
            xAxis = -currentMaxSpeed;
        }else if (left && xAxis < -currentMaxSpeed) {
            xAxis += Time.deltaTime * deceleration;

            if(xAxis<-currentMaxSpeed && xAxis<(-currentMaxSpeed - 0.05f)){
                xAxis = -currentMaxSpeed;
            }
        }else if (left && xAxis > -currentMaxSpeed && xAxis<(-currentMaxSpeed+0.05f)) {
            xAxis = -currentMaxSpeed;
        }
        // set xAxis right
        if(right && runShift && xAxis> currentMaxSpeed){
            xAxis = currentMaxSpeed;
        }else if (right && xAxis > currentMaxSpeed){
            xAxis -= Time.deltaTime * deceleration;
            if(xAxis>currentMaxSpeed && xAxis< (currentMaxSpeed+0.05f)){
                xAxis = currentMaxSpeed;
            }else if(right&& xAxis <currentMaxSpeed && xAxis > (currentMaxSpeed-0.05f)){
                xAxis = currentMaxSpeed;
            }
        }


    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/* Attributions
 * 
 * https://www.youtube.com/watch?v=QREKO1sf8b8 
*/


public class Movement : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Vector2 leftStickValue; 
	public SteamVR_Action_Vector2 rightStickValue;  

    public float maxSpeed;

	public Quaternion orientation;

    private CharacterController charController = null;
    private Transform cameraRig = null;
    private Transform head = null;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }
    
    private void Start()
    {
        cameraRig = SteamVR_Render.Top().origin;
        head = SteamVR_Render.Top().head;
    }

    private void Update()
    {
		//print(leftStickValue.axis);
        HandleHead();
        HandleHeight();
        CalculateMovement();
    }

    private void HandleHead(){
        // Store oldPosition of the cameraRig, rotate the player controller object, then reset cameraRig (since cameraRig is a child of the player controller)
        Vector3 oldPosition = cameraRig.position;
        Quaternion oldRotation = cameraRig.rotation;

        transform.eulerAngles = new Vector3(head.rotation.eulerAngles.x, head.rotation.eulerAngles.y, head.rotation.eulerAngles.z);

        cameraRig.position = oldPosition;
        cameraRig.rotation = oldRotation;
    }

    private void CalculateMovement(){
        //Find movement orientation
        Vector3 orientationEuler = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        orientation = Quaternion.Euler(orientationEuler);
        Vector3 movement = Vector3.zero;

        //If moving laterally
		if((leftStickValue.axis.x > 0.1 || leftStickValue.axis.x < -0.1) || (leftStickValue.axis.y > 0.1 || leftStickValue.axis.y < -0.1)){
			var x = 0;
			var y = 0;
			if (leftStickValue.axis.x > 0.1){
				x = 1;
			}
			else if (leftStickValue.axis.x < -0.1){
				x = -1;
			}
			if (leftStickValue.axis.y > 0.1){
				y = 1;
			}
			else if (leftStickValue.axis.y < -0.1){
				y = -1;
			} 

            var newAxis = new Vector3(x, 0, y);
            movement += orientation * newAxis * maxSpeed * Time.deltaTime;
            charController.Move(movement);
        }


		//If moving vertically
		if(rightStickValue.axis.y > 0.1 || rightStickValue.axis.y < -0.1){
			var y = 0;
			if (rightStickValue.axis.y > 0.1){
				y = 1;
			}
			else if (rightStickValue.axis.y < -0.1){
				y = -1;
			} 

			var newAxis = new Vector3(0, y, 0);
			movement += orientation * newAxis * maxSpeed * Time.deltaTime;
			charController.Move(movement);
		}

    }

	//Changes center of character controller object based on head position
    private void HandleHeight(){
        float headHeight = Mathf.Clamp(head.localPosition.y, 1, 2);
        charController.height = headHeight;

        Vector3 newCenter = Vector3.zero;
        newCenter.y = charController.height / 2;
        newCenter.y += charController.skinWidth;

        newCenter.x = head.localPosition.x;
        newCenter.z = head.localPosition.z;
        newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;

        charController.center = newCenter;
    }
}

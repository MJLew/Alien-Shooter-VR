using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class shooting : MonoBehaviour
{
	public SteamVR_Input_Sources handType;
	public SteamVR_Behaviour_Pose controllerPose;
	public SteamVR_Action_Boolean shootingAction;

	private Transform bulletTransform;
	public GameObject bulletPrefab;
	private GameObject bullet;

	private Quaternion orientation;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		//Shoots while shoot button is pressed
		if (shootingAction.GetState(handType)){
			InvokeRepeating("spawnBullet", 0, 0.7f);
		}
		else {
			CancelInvoke("spawnBullet");
		}
	}

	//Spawn bullet at controller position and toward player orientation
	void spawnBullet(){
		Vector3 orientationEuler = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
		orientation = Quaternion.Euler(orientationEuler);

		bullet = Instantiate(bulletPrefab, transform.position, orientation);
	}
}

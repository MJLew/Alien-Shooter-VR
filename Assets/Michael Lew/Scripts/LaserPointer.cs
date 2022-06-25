using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LaserPointer : MonoBehaviour
{
	public SteamVR_Input_Sources handType;
	public SteamVR_Behaviour_Pose controllerPose;
	public SteamVR_Action_Boolean spawnLaser;

	public GameObject indicatorPrefab;
	private GameObject indicator;
	private Transform indicTransform;
    public LineRenderer laserLineRenderer;
	private Vector3 hitPoint;

	// Use this for initialization
	void Start () {
		indicator = Instantiate(indicatorPrefab, new Vector3(0,0,0), Quaternion.identity);
		indicator.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(spawnLaser.GetState(handType)){
			//Set first point
			laserLineRenderer.SetPosition(0, transform.position);
			// Cast Ray and see if it intersects.
			// If yes, do action
			// If no, set new collision point in distance
			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.forward, out hit, 100)){
				hitPoint = hit.point;
				if (indicator.transform.position != hitPoint){
					indicator.transform.position = hitPoint;
					if (!indicator.activeSelf){
						indicator.SetActive(true);
					}
				}
			}
			else {
				indicator.SetActive(false);
                hitPoint = transform.position + (20 * transform.forward);
			}
			ShowLaser(hitPoint);
		}
		// If not holding down trigger, make line invisible
		else {
			indicator.SetActive(false);
			laserLineRenderer.SetPosition(0,transform.position);
			laserLineRenderer.SetPosition(1,transform.position);
		}

	}

	private void ShowLaser(Vector3 hitPoint) {
        laserLineRenderer.SetPosition(1, hitPoint);
	}
		
}

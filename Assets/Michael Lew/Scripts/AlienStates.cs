using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Attributions
 * 
 * Animation States:
 * https://learn.unity.com/tutorial/controlling-animation#5c7f8528edbc2a002053b4e2
 * 
 * Checking if enemy is visible:
 * https://answers.unity.com/questions/720447/if-game-object-is-in-cameras-field-of-view.html
 * 
 * State Machines:
 * https://unity3d.college/2019/04/28/unity3d-ai-with-state-machine-drones-and-lasers/
*/

public class AlienStates : MonoBehaviour
{
	//Global Variables:

	//Player Related
	public GameObject player;
	public Camera playerCam;
	public float playerDistance;
	Vector3 playerPosition;

	//Ship Related
	Health health;
	Rigidbody rb;
	State currentState;

	public float sightDistance;
	public float attackDistance;
	public float rotateSpeed;
	public float moveSpeed;
	public int patrolRange;
	public int numWaypoints;

	List<Vector3> waypointList;
	Vector3 wayPoint;
	int currWaypoint = 0;

	//Other
	Transform bulletSpawnPoint;
	public GameObject bulletPrefab;
	private GameObject bullet;
	float time = 0f;
	float shotPeriod = .75f;
	float wanderPeriod = 10;

	bool playerClose;
	RNG rng;

	//Main Functions:

	// Start is called before the first frame update
	void Start()
	{
		//Find game global RNG to stop repeats with every retrieve
		rng = GameObject.Find("RNG").GetComponent<RNG>();
		//Get positions
		rb = GetComponent<Rigidbody>();
		bulletSpawnPoint = transform.GetChild(0);
		//Set states
		playerClose = false;
		currentState = State.patrol;
		health = GetComponent<Health>();

		//Set waypoints based on current position
		waypointList = new List<Vector3>();
		for (int i = 0; i < numWaypoints; i++){
			waypointList.Add(newWaypoint());
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		//If alien runs out of health, it is destroyed
		if (health.remaining < 1){
			health.kill(0f);
		}

		//Check player position, every frame since every state depends on this
		playerClose = playerCloseBy();

		//State Machine
		switch(currentState){
		case State.patrol: //Patrol State: Move between preset waypoints until player comes close
			{
				if (!playerClose){
					setWaypoint();
					rotateTowardPosition(wayPoint);
					moveForward(moveSpeed/2);
				}
				else{
					currentState = State.aware;
				}
				break;
			}
		case State.aware: //Aware State: If player is still close, track movements but don't chase. If player looks, then attack+chase
			{
				if(playerClose){
					rotateTowardPosition(playerPosition);

					if (playerLooking()){
						currentState = State.chase;
					}
				}
				else{
					currentState = State.patrol;
				}
				break;
			}
		case State.chase: //Chase State: If player is still close, continue chasing and shooting. If not, then go back to patrolling
			{
				if(playerDistance < attackDistance){
					shootBullet();
					rotateTowardPosition(playerPosition);
					moveForward(moveSpeed);
				}
				else{
					currentState = State.patrol;
				}
				break;
			}
		}
	}

	//Helper Methods and Variables:

	//Keep track of state
	public enum State{
		patrol, chase, attack, aware
	}

	//Get distance to player, to see if should be aggressive
	private bool playerCloseBy(){
		playerPosition = player.transform.position;
		playerDistance = Mathf.Sqrt(
			Mathf.Pow(playerPosition.x - transform.position.x, 2) +
			Mathf.Pow(playerPosition.y - transform.position.y, 2) +
			Mathf.Pow(playerPosition.z - transform.position.z, 2));

		return (playerDistance <= sightDistance);
	}

	//Check if player is looking at it
	private bool playerLooking(){
		Vector3 screenPoint = playerCam.WorldToViewportPoint(transform.position);
		bool isViewable = ((screenPoint.x > 0 && screenPoint.x < 1) && (screenPoint.y > 0 && screenPoint.y < 1)) && screenPoint.z > 0;

		return isViewable;
	}

	//Rotation towards player position
	private void rotateTowardPosition(Vector3 pos){
		rb.angularVelocity = new Vector3(0,0,0); //Reset angular velocity first or ship will go spinning out of control
		float rotateStep = rotateSpeed * Time.deltaTime;
		Vector3 lookAtPos = new Vector3(pos.x - transform.position.x, pos.y - transform.position.y, pos.z - transform.position.z);
		Quaternion lookAtQuat = Quaternion.LookRotation(lookAtPos);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAtQuat, rotateStep);
	}

	//Movement
	private void moveForward(float moveSpeed){
		rb.velocity = new Vector3(0, 0, 0); //Reset velocity first or ship will fly out of control
		rb.MovePosition(rb.position + transform.TransformDirection(Vector3.forward * moveSpeed * Time.deltaTime));
	}

	//Spawns a bullet and points it toward player position. Keeps track of time between shots to create reasonable interval
	private void shootBullet(){
		time += Time.deltaTime;
		if (time >= shotPeriod){
			time = time - shotPeriod;
			Vector3 orientationEuler = (player.transform.position - bulletSpawnPoint.position).normalized;
			Quaternion orientation = Quaternion.LookRotation(orientationEuler);

			bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, orientation);
		}
	}

	//Change current waypoint after reaching their target waypoint or after exceeding a certain amount of time (to prevent being stuck)
	private void setWaypoint(){
		time += Time.deltaTime;
		if (transform.position == waypointList[currWaypoint] || time >= wanderPeriod){
			time = time - wanderPeriod;
			if (currWaypoint < numWaypoints-1){
				currWaypoint++;
			}
			else{
				currWaypoint = 0;
			}
			wayPoint = waypointList[currWaypoint];
		}
	}

	//Create waypoints close to original position, to add them to list of patrol waypoints for this ship
	private Vector3 newWaypoint(){
		int x = RNG.RandomNumber((int)transform.position.x - patrolRange, (int)transform.position.x + patrolRange);
		int y = RNG.RandomNumber((int)transform.position.y - patrolRange, (int)transform.position.y + patrolRange);
		int z = RNG.RandomNumber((int)transform.position.z - patrolRange, (int)transform.position.z + patrolRange);

		return new Vector3(x, y, z);
	}
		
}

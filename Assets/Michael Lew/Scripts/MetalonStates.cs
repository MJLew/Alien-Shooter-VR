using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/* Attributions
 * 
 * Animation States:
 * https://learn.unity.com/tutorial/controlling-animation#5c7f8528edbc2a002053b4e2
 * 
 * State Machines:
 * https://unity3d.college/2019/04/28/unity3d-ai-with-state-machine-drones-and-lasers/
*/
public class MetalonStates : MonoBehaviour
{
	//Global Variables:

	//Player Related
	public GameObject player;
	Vector3 playerPosition;
	public float playerDistance;

	//Metalon Related
	Health health;
	Rigidbody rb;
	public float sightDistance;
	public float castDistance;
	public float rotateSpeed;
	public float moveSpeed;
	State currentState;
	Transform bulletSpawnPoint;

	//Other
	Vector3 wayPoint = new Vector3(0, 0, 0);
	RNG rng;
	public GameObject bulletPrefab;
	GameObject bullet;
	float time = 0f;
	float shotPeriod = 1f;
	float wanderPeriod = 10;
	bool playerClose;

	//Animator
	Animator anim;
	int idleHash = Animator.StringToHash("Idle");
	int walkHash = Animator.StringToHash("Walk Forward");
	int runHash = Animator.StringToHash("Run Forward");
	int castHash = Animator.StringToHash("Cast Spell");
	int dieHash = Animator.StringToHash("Die");

	//Main Functions:

    // Start is called before the first frame update
    void Start()
    {
		//Animator
		anim = GetComponent<Animator>();
		//Rigidbody for movement
		rb = GetComponent<Rigidbody>();
		//Get game object that acts as spawn point for bullets
		bulletSpawnPoint = transform.GetChild(18);
		//Get global RNG
		rng = GameObject.Find("RNG").GetComponent<RNG>();
		//This object's health component
		health = GetComponent<Health>();

		playerClose = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		//Get animation state info
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

		//If metalon runs out of health, it dies and is destroyed
		if (health.remaining < 1 && currentState != State.dying){
			currentState = State.dying;
			anim.SetTrigger(dieHash);
			health.kill(1f);
		}

		//Check player position, every frame since every state depends on this
		playerClose = playerCloseBy();

		//State Machine
		switch(currentState){
			case State.idle: //Idle State: If player is close, start chasing
			{
				if (playerClose){
					currentState = State.chase;
				}
				break;
			}
			case State.chase: //Chase State: If player is still close, continue chasing. If within range, start casting
			{
				if(playerClose){
					if (playerDistance < castDistance) {
						currentState = State.cast;
						anim.SetBool(runHash, false);
					}
					else {
						rotateTowardPosition(playerPosition);
						moveForward(moveSpeed);
						//Update animation
						anim.SetBool(runHash, true);
					}
				}
				else{
					currentState = State.wander;
					anim.SetBool(runHash, false);
				}
				break;
			}
			case State.cast: //Cast State: Shoot a bullet then go back to Idle state
			{
				if (playerDistance < castDistance){
					anim.SetTrigger(castHash);
					shootBullet();
				}
				else{
					currentState = State.idle;
				}
				break;
			}
			case State.wander: //Wander State: Set a random waypoint every so often until player gets close
			{
				if (!playerClose){
					setWaypoint();
					rotateTowardPosition(wayPoint);
					moveForward(moveSpeed/2);
					anim.SetBool(walkHash, true);

				}
				else{
					currentState = State.chase;
					anim.SetBool(walkHash,false);
				}
				break;
			}
		}
    }

	//Helper Methods and Variables:

	//Tracks state
	public enum State{
		idle, wander, chase, cast, dying
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
		
	//Rotation towards player position
	private void rotateTowardPosition(Vector3 pos){
		float rotateStep = rotateSpeed * Time.deltaTime;
		Vector3 lookAtPos = new Vector3(pos.x - transform.position.x, 0, pos.z - transform.position.z);
		Quaternion lookAtQuat = Quaternion.LookRotation(lookAtPos);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAtQuat, rotateStep);
	}

	//Movement
	private void moveForward(float moveSpeed){
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

	//Creates a random waypoint and sets current destination waypoint to the new one
	private void setWaypoint(){
		time += Time.deltaTime;
		if (time >= wanderPeriod){
			time = time - wanderPeriod;
			wanderPeriod = RNG.RandomNumber(0, 10);
			wayPoint.Set(RNG.RandomNumber(-50, 50), 0, RNG.RandomNumber(-50, 50));
		}
	}
}
	
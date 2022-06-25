using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
	public float speed;
	public int damage;

	GameObject collided;
	Health collidedHealth;

    // Update is called once per frame
	//Propel bullet
    void Update()
    {
		GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }

	//After hits, check if it collided with an enemy, player, or boss
	void OnCollisionEnter(Collision collision){
		//Destroy bullet
		Destroy(this.gameObject);

		collided = collision.gameObject;
		collidedHealth = collided.GetComponent<Health>();

		//If enemy, deplete health
		if (collided.CompareTag("Killable")){
			collidedHealth.remaining -= damage;
		}
		//If player, deplete health and update game state through health script
		else if (collided.CompareTag("Player")){
			collidedHealth.remaining -= damage;
			collidedHealth.playerHit();
		}
		//If boss, deplete health and update game state through health script
		else if (collided.CompareTag("Boss")){
			collidedHealth.remaining -= damage;
			collidedHealth.bossHit();
		}
	}
}

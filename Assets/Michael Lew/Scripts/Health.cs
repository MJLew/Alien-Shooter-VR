using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	GameState gameState;
	public int total;
	public int remaining;

	void Start() {
		//Find game state object
		gameState = GameObject.Find("GameState").GetComponent<GameState>();
		total = remaining;
	}

	//When an enemy is killed, invoke their death function after a set time (unique to death animation)
	public void kill(float delay){
		Invoke("enemyDeath", delay);
	}
	public void enemyDeath(){
		gameState.updateEnemiesRemaining();
		Destroy(this.gameObject);
	}

	//If player is hit, tell gameState to update health text. If no more health, tell game state to display lose text.
	public void playerHit(){
		if (remaining > 0){
			gameState.updatePlayerHealth();
		}
		else{
			gameState.displayLose();
		}
	}

	//If boss is hit, tell gameState to update health text. If no more health, tell game state to display win text.
	public void bossHit(){
		gameState.updateBossHealth();
		if(remaining <= 0){
			gameState.displayWin();
		}
	}
}

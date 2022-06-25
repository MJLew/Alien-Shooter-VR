using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
	//UI text items
	public Text countText;
	public Text healthText;
	public Text bossText;
	public Text bossHealthText;

	//Player Related
	public GameObject player;
	Health playerHealth;
	int enemiesRemaining;

	public GameObject boss;
	Health bossHealth;

	public State currState;

	public enum State{
		playing, won, lost
	}

    // Start is called before the first frame update
    void Start()
    {
		//Get/Set states
		currState = State.playing;
		playerHealth = player.GetComponent<Health>();
		bossHealth = boss.GetComponent<Health>();

		//Set default text
		enemiesRemaining = GameObject.FindGameObjectsWithTag("Killable").Length + 2;
		Invoke("updateEnemiesRemaining", 0.1f);
		Invoke("updatePlayerHealth", 0.1f);
    }

	//Updates text for "Enemies Remaining" UI field
	public void updateEnemiesRemaining(){
		enemiesRemaining--;
		countText.text = "Enemies Remaining: " + enemiesRemaining.ToString();

		if (enemiesRemaining == 1){
			playerHealth.remaining = playerHealth.total;
			updatePlayerHealth();
			spawnBoss();
		}
	}

	//Updates text for "Player Health" UI field
	public void updatePlayerHealth(){
		InvokeRepeating("toggleHealthTextColour", 0f, 0.1f);
		healthText.text = "Player Health\n" + playerHealth.remaining.ToString();
		Invoke("resetHealthTextColour", 1f);
	}

	//Toggles "Player Health" UI field colour
	public void toggleHealthTextColour(){
		if (healthText.color == Color.red){
			healthText.color = Color.white;
		}
		else {
			healthText.color = Color.red;
		}
	}
	public void resetHealthTextColour(){
		healthText.color = Color.white;
		CancelInvoke("toggleHealthTextColour");
	}

	//Updates text for "Boss Health" UI field
	public void updateBossHealth(){
		bossHealthText.text = "Boss Health\n" + bossHealth.remaining.ToString();
	}

	//Spawn the boss, flash warning text, call for update to "Boss Health" UI field
	public void spawnBoss(){
		bossText.text = "BOSS HAS APPEARED";
		Invoke("removeBossText", 5f);
		updateBossHealth();
		boss.SetActive(true);
	}
	public void removeBossText(){
		bossText.text = "";
	}
		
	//Display win text, if player hasn't lost already
	public void displayWin(){
		if (currState != State.lost){
			currState = State.won;
			bossHealthText.text = "Boss Health\n0";
			bossText.text = "YOU WIN!";
		}
	}

	//Display lose text (don't need to check if won, since there are no enemies left to kill you)
	public void displayLose(){
		currState = State.lost;
		healthText.text = "Health Remaining: 0";
		bossText.text = "YOU DIED!\nRestart to try again.";
	}


}

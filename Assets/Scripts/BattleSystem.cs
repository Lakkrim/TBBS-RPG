using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOSE }


public class BattleSystem : MonoBehaviour
{
	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	Unit playerUnit;
	Unit enemyUnit;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

	public Text dialogueText;
	public GameObject combatButtons;


	void Start()
	{
		state = BattleState.START;
		StartCoroutine(SetupBattle());
	}

	IEnumerator SetupBattle()
    {
		
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();
		
		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		dialogueText.text = "FIGHT IT OUT!";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
    }

	IEnumerator PlayerAttack()
    {
		bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = "Attack";

		yield return new WaitForSeconds(2f);

        if (isDead)
        {
			state = BattleState.WON;
			EndBattle();
        }
        else
        {
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
        }
    }

	IEnumerator EnemyTurn()
    {

		if (enemyUnit.currentHP >= enemyUnit.maxHP * .7f) 
		{
			dialogueText.text = enemyUnit.unitName + " attacks!";

			bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
			playerHUD.SetHP(playerUnit.currentHP);

			yield return new WaitForSeconds(1f);

			if (isDead)
			{
				state = BattleState.LOSE;
				EndBattle();
			}
			else
			{
				state = BattleState.PLAYERTURN;
				PlayerTurn();
			}
		}
		else 
		{
			int number = Random.Range(0, 10);
			if (number >= 2)
			{
				dialogueText.text = enemyUnit.unitName + " attacks!";

				bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
				playerHUD.SetHP(playerUnit.currentHP);

				yield return new WaitForSeconds(1f);
				
				if (isDead)
				{
					state = BattleState.LOSE;
					EndBattle();
				}
				else
				{
					state = BattleState.PLAYERTURN;
					PlayerTurn();
				}
			}
			else
			{
				dialogueText.text = enemyUnit.unitName + " heals!";

				enemyUnit.Heal(5);
				enemyHUD.SetHP(enemyUnit.currentHP);

				yield return new WaitForSeconds(1f);

				state = BattleState.PLAYERTURN;
				PlayerTurn();
			}
		}
		
		
    }


	void EndBattle()
    {
		if (state == BattleState.WON)
		{
			dialogueText.text = "VICTORIOUS!";
		}
		else if (state == BattleState.LOSE)
        {
			dialogueText.text = "GAME OVER...";
        }
    }

	void PlayerTurn()
    {
		
		dialogueText.text = "Choose an action...";
		combatButtons.SetActive(true);

	}

	IEnumerator PlayerHeal()
    {
		playerUnit.Heal(5);
		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "HEAL";

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
	}

	public void OnAttackButton()
    {
		if(state != BattleState.PLAYERTURN)
			return;
		
		StartCoroutine(PlayerAttack());
		combatButtons.SetActive(false);
	}

	public void OnHealButton()
    {
		if(state != BattleState.PLAYERTURN)
        {
			return;
        }

		StartCoroutine(PlayerHeal());
		combatButtons.SetActive(false);
    }

}

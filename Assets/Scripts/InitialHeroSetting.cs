using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialHeroSetting : MonoBehaviour
{
    [SerializeField] private WheelManager playerWheelManager;
    [SerializeField] private WheelManager enemyWheelManager;
    [SerializeField] private HeroSelectionRotator heroSelection;

    [SerializeField] private HeroActions playerSquareActions;
    [SerializeField] private HeroActions playerDiamondActions;
    [SerializeField] private HeroActions enemySquareActions;
    [SerializeField] private HeroActions enemyDiamondActions;

    [SerializeField] private EvaluationManager playerEvaluationManager;
    [SerializeField] private EvaluationManager enemyEvaluationManager;

    [SerializeField] private TurnManager turnManager;

    [SerializeField] private CoverManager coverManager;

    [SerializeField] private GameObject[] readyButtons;
    [SerializeField] private GameObject[] playerSelectionButtons;
    [SerializeField] private GameObject[] enemySelectionButtons;

    private bool playerReady = false;
    private bool enemyReady = false;

    public void SetReady(string user)
    {
        if (user == "Player")
        {
            playerReady = true;
            DeactivateSelection(playerSelectionButtons);
        }
        if (user == "Enemy")
        {
            enemyReady = true;
            DeactivateSelection(enemySelectionButtons);
        }

        if (playerReady && enemyReady)
        {
            foreach (var button in readyButtons)
            {
                button.SetActive(false);
            }

            playerWheelManager.ActivateSpinButton();
            enemyWheelManager.ActivateSpinButton();

            InitializeEverything();
        }
    }

    private void DeactivateSelection(GameObject[] bttns)
    {
        foreach (var bttn in bttns)
        {
            bttn.SetActive(false);
        }
    }

    private void InitializeEverything()
    {
        Hero playerSquareHero = heroSelection.GetPlayerSquareHero();
        Hero playerDiamondHero = heroSelection.GetPlayerDiamondHero();
        Hero enemySquareHero = heroSelection.GetEnemySquareHero();
        Hero enemyDiamondHero = heroSelection.GetEnemyDiamondHero();

        //Alles für den Spieler Square Helden setzen
        playerSquareActions.SetPlayerHeroes(playerSquareHero, playerDiamondHero);
        playerSquareActions.SetEnemyHeroes(enemySquareHero, enemyDiamondHero);
        playerSquareActions.SetSquareSideMain();
        playerSquareActions.SetPlayerSideMain();

        //Alles für den Spieler Diamond Helden setzen
        playerDiamondActions.SetPlayerHeroes(playerDiamondHero, playerSquareHero);
        playerDiamondActions.SetEnemyHeroes(enemySquareHero, enemyDiamondHero);
        playerDiamondActions.SetDiamondSideMain();
        playerDiamondActions.SetPlayerSideMain();

        //Alles für den Gegner Square Helden setzen
        enemySquareActions.SetPlayerHeroes(enemySquareHero, enemyDiamondHero);
        enemySquareActions.SetEnemyHeroes(playerSquareHero, playerDiamondHero);
        enemySquareActions.SetSquareSideMain();
        enemySquareActions.SetEnemySideMain();

        //Alles für den Gegner Diamond Helden setzen
        enemyDiamondActions.SetPlayerHeroes(enemyDiamondHero, enemySquareHero);
        enemyDiamondActions.SetEnemyHeroes(playerSquareHero, playerDiamondHero);
        enemyDiamondActions.SetDiamondSideMain();
        enemyDiamondActions.SetEnemySideMain();

        //Alle Heroes mit HeroActions befüllen
        playerSquareHero.SetHeroActions(playerSquareActions);
        playerDiamondHero.SetHeroActions(playerDiamondActions);
        enemySquareHero.SetHeroActions(enemySquareActions);
        enemyDiamondHero.SetHeroActions(enemyDiamondActions);

        //EvaluationManager befüllen
        playerEvaluationManager.SetHeroes(playerSquareHero, playerDiamondHero);
        enemyEvaluationManager.SetHeroes(enemySquareHero, enemyDiamondHero);

        //Turnmanager befüllen
        turnManager.SetHeroes(playerSquareHero, playerDiamondHero, enemySquareHero, enemyDiamondHero);

        //Cover hochstellen
        StartCoroutine(coverManager.SetCoverUp());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private Hero[] playerHeroes;   //Helden des Spielers
    [SerializeField] private Hero[] enemyHeroes;    //Helden des Gegners

    [SerializeField] private WheelManager playerWheelManager;   //WheelManager des Spielers
    [SerializeField] private WheelManager enemyWheelManager;    //WheelManager des Gegners

    [SerializeField] private CrownManager playerCrownManager;   //Crownmanager des Spielers
    [SerializeField] private CrownManager enemyCrownManager;    //CrownManager des Gegners

    [SerializeField] private BulwarkMover playerBulwarkMover;   //BulwarkManager des Spielers
    [SerializeField] private BulwarkMover enemyBulwarkMover;    //BulwarkManager des Gegners

    private int currentTurnStep = 1;

    public void BeginTurn()
    {
        //Resette den aktuellen Schritt und starte den Zugzyklus
        currentTurnStep = 1;
        StartCoroutine(ProcessTurnStep());
    }

    private IEnumerator ProcessTurnStep()
    {
        //TODO
        yield return null;
    }

    private void EndTurn()
    {
        Debug.Log("Runde beendet. Nächste Runde wird vorbereitet!");
        playerWheelManager.ResetRound();
        enemyWheelManager.ResetRound();
    }


}

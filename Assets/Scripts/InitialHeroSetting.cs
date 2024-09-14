using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialHeroSetting : MonoBehaviour
{
    [SerializeField] private WheelManager playerWheelManager;
    [SerializeField] private WheelManager enemyWheelManager;

    private bool playerIsSet = false;
    private bool enemyIsSet = false;

    public void SetAllAssignments(WheelManager manager)
    {
        if (manager == playerWheelManager)
        {
            playerIsSet = true;
        }
        if (manager == enemyWheelManager)
        {
            enemyIsSet = true;
        }

        if (playerIsSet && enemyIsSet)
        {
            playerWheelManager.SetHeroAssignments();
            enemyWheelManager.SetHeroAssignments();
        }
    }
}

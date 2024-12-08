using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownManager : MonoBehaviour
{
    [SerializeField] private EnemyScriptDebug enemyScript;  //Debug only

    [SerializeField] private int maxHP = 10;      //Maximaler HP-Wert, mit dem der Spieler beginnt
    private int currentHP;

    //Referenzen zu den  R�dern f�r die Anzeige
    [SerializeField] Transform oneWheel;
    [SerializeField] Transform tenWheel;

    private void Start()
    {
        SetStartStats();

        TurnManager.Instance.OnInitializeCrownHP += TurnManager_OnInitializeCrownHP;
    }

    private void TurnManager_OnInitializeCrownHP(object sender, EventArgs e)
    {
        if (enemyScript != null)
        {
            TurnManager.Instance.ChangeCrownHP(enemyScript.playerId, maxHP);
        }
        else
        {
            TurnManager.Instance.ChangeCrownHP(PlayerScript.Instance.playerId, maxHP);
        }
    }

    //Startwerte Setzen
    private void SetStartStats()
    {
        currentHP = maxHP;
        UpdateHPDisplay();
        //Debug.Log("Startstats eingegeben f�r " + this + "!");
    }

    //Methode zum Abziehen von HP
    public void DecreaseHP(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0)
        {
            currentHP = 0;      //Sicherstellen, dass HP nicht negativ wird
        }

        if (enemyScript != null)
        {
            TurnManager.Instance.ChangeCrownHP(enemyScript.playerId, currentHP);
        }
        else
        {
            TurnManager.Instance.ChangeCrownHP(PlayerScript.Instance.playerId, currentHP);
        }

        UpdateHPDisplay();
    }

    //Methode zum Hinzuf�gen von HP
    public void IncreaseHP(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;      //Sicherstellen, dass HP nicht �ber maxHP steigt
        }

        if (enemyScript != null)
        {
            TurnManager.Instance.ChangeCrownHP(enemyScript.playerId, currentHP);
        }
        else
        {
            TurnManager.Instance.ChangeCrownHP(PlayerScript.Instance.playerId, currentHP);
        }

        UpdateHPDisplay();
    }

    //Methode zur Aktualisierung der HP-Anzeige
    private void UpdateHPDisplay()
    {
        int ones = currentHP % 10;      //Einerstelle
        int tens = currentHP / 10;      //Zehnerstelle

        //R�der entsprechend der aktuellen HP rotieren lassen
        float targetRotationOnes = ones * -36f;
        float targetRotationTens = tens * -36f;
        StartCoroutine(WheelRotator(targetRotationOnes, targetRotationTens));

        if (enemyScript != null)
        {
            PlayerScript.Instance.GetHPScripts().SetCurrentHP(enemyScript.playerId, currentHP);
        }
        else
        {
            PlayerScript.Instance.GetHPScripts().SetCurrentHP(PlayerScript.Instance.playerId, currentHP);
        }
    }

    private IEnumerator WheelRotator(float rotationOnes, float rotationTens)
    {
        //Dauer der Animation
        float duration = 0.3f;
        float timeElapsed = 0f;

        //Die aktuelle Rotation in Quaternion speichern
        Quaternion initialRotationOnes = oneWheel.rotation;
        Quaternion initialRotationTens = tenWheel.rotation;

        //Zielrotation in Quaternion berechnen, um eine sanfte Imterpolation durchzuf�hren
        Quaternion targetRotationOnes = Quaternion.Euler(rotationOnes, oneWheel.eulerAngles.y, oneWheel.eulerAngles.z);
        Quaternion targetRotationTens = Quaternion.Euler(rotationTens, tenWheel.eulerAngles.y, tenWheel.eulerAngles.z);

        //F�hre die Interpolation �ber die Zeit durch
        while (timeElapsed < duration)
        {
            //Linear interpolieren zwischen der Startrotation und der Zielrotation
            oneWheel.rotation = Quaternion.Lerp(initialRotationOnes, targetRotationOnes, timeElapsed / duration);
            tenWheel.rotation = Quaternion.Lerp(initialRotationTens, targetRotationTens, timeElapsed / duration);

            //Erh�he die verstrichene Zeit
            timeElapsed += Time.deltaTime;
            //Warte bis zum n�chsten Frame
            yield return null;
        }

        oneWheel.rotation = targetRotationOnes;
        tenWheel.rotation = targetRotationTens;
    }
}

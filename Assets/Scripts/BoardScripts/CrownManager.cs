using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownManager : MonoBehaviour
{
    [SerializeField] private EnemyScript enemyScript;  //Offline only

    [SerializeField] private int maxHP = 10;      //Maximaler HP-Wert, mit dem der Spieler beginnt
    private int currentHP;

    //Referenzen zu den  Rädern für die Anzeige
    [SerializeField] Transform oneWheel;
    [SerializeField] Transform tenWheel;

    //Events
    public event Action<float, float> OnSetNewHPStatus;

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
        //Debug.Log("Startstats eingegeben für " + this + "!");
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

    //Methode zum Hinzufügen von HP
    public void IncreaseHP(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;      //Sicherstellen, dass HP nicht über maxHP steigt
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

        //Räder entsprechend der aktuellen HP rotieren lassen
        float targetRotationOnes = ones * -36f;
        float targetRotationTens = tens * -36f;
        StartCoroutine(WheelRotator(targetRotationOnes, targetRotationTens));
        OnSetNewHPStatus?.Invoke(targetRotationOnes, targetRotationTens);

        //if (enemyScript != null)
        //{
        //    enemyScript.GetHPScriptsSelf().SetCurrentHP(enemyScript.playerId, currentHP);
        //    enemyScript.GetHPScriptsEnemy().SetCurrentHP(enemyScript.playerId, currentHP);
        //}
        //else
        //{
        //    PlayerScript.Instance.GetHPScriptsSelf().SetCurrentHP(PlayerScript.Instance.playerId, currentHP);
        //    PlayerScript.Instance.GetHPScriptsEnemy().SetCurrentHP(PlayerScript.Instance.playerId, currentHP);
        //}
    }

    private IEnumerator WheelRotator(float rotationOnes, float rotationTens)
    {
        //Dauer der Animation
        float duration = 0.3f;
        float timeElapsed = 0f;

        //Die aktuelle Rotation in Quaternion speichern
        Vector3 initialRotationOnes = Vector3.zero;
        Vector3 initialRotationTens = Vector3.zero;

        initialRotationOnes.x = oneWheel.localEulerAngles.x;
        initialRotationTens.x = tenWheel.localEulerAngles.x;

        //Zielrotation in Quaternion berechnen, um eine sanfte Imterpolation durchzuführen
        Vector3 targetRotationOnes = Vector3.zero;
        Vector3 targetRotationTens = Vector3.zero;

        targetRotationOnes.x = rotationOnes;
        targetRotationTens.x = rotationTens;

        //Führe die Interpolation über die Zeit durch
        while (timeElapsed < duration)
        {
            //Linear interpolieren zwischen der Startrotation und der Zielrotation
            oneWheel.localEulerAngles = Vector3.Lerp(initialRotationOnes, targetRotationOnes, timeElapsed / duration);
            tenWheel.localEulerAngles = Vector3.Lerp(initialRotationTens, targetRotationTens, timeElapsed / duration);


            //Erhöhe die verstrichene Zeit
            timeElapsed += Time.deltaTime;
            //Warte bis zum nächsten Frame
            yield return null;
        }

        oneWheel.localEulerAngles = targetRotationOnes;
        tenWheel.localEulerAngles = targetRotationTens;
    }

    //private IEnumerator WheelRotator(float rotationOnes, float rotationTens)
    //{
    //    //Dauer der Animation
    //    float duration = 0.3f;
    //    float timeElapsed = 0f;

    //    //Die aktuelle Rotation in Quaternion speichern
    //    Quaternion initialRotationOnes = oneWheel.rotation;
    //    Quaternion initialRotationTens = tenWheel.rotation;

    //    //Zielrotation in Quaternion berechnen, um eine sanfte Imterpolation durchzuführen
    //    Quaternion targetRotationOnes = Quaternion.Euler(rotationOnes, oneWheel.eulerAngles.y, oneWheel.eulerAngles.z);
    //    Quaternion targetRotationTens = Quaternion.Euler(rotationTens, tenWheel.eulerAngles.y, tenWheel.eulerAngles.z);

    //    //Führe die Interpolation über die Zeit durch
    //    while (timeElapsed < duration)
    //    {
    //        //Linear interpolieren zwischen der Startrotation und der Zielrotation
    //        oneWheel.rotation = Quaternion.Lerp(initialRotationOnes, targetRotationOnes, timeElapsed / duration);
    //        tenWheel.rotation = Quaternion.Lerp(initialRotationTens, targetRotationTens, timeElapsed / duration);

    //        //Erhöhe die verstrichene Zeit
    //        timeElapsed += Time.deltaTime;
    //        //Warte bis zum nächsten Frame
    //        yield return null;
    //    }

    //    oneWheel.rotation = targetRotationOnes;
    //    tenWheel.rotation = targetRotationTens;
    //}
}

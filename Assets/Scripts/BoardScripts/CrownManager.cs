using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CrownManager : NetworkBehaviour
{
    [SerializeField] private EnemyScript enemyScript;  //Offline only

    [SerializeField] private int maxHP = 10;      //Maximaler HP-Wert, mit dem der Spieler beginnt
    //CrownHP als Networkvariable, damit nur der Server diese ändern kann
    private NetworkVariable<int> currentHP = new NetworkVariable<int>(
        10,     //Startwert
        NetworkVariableReadPermission.Everyone,     //Clients dürfen den Wert lesen
        NetworkVariableWritePermission.Server       //Nur der Server darf den Wert schreiben
        );

    //Referenzen zu den  Rädern für die Anzeige
    [SerializeField] private Transform oneWheel;
    [SerializeField] private Transform tenWheel;

    //Events
    public event Action<float, float> OnSetNewHPStatus;

    private void Start()
    {
        if (IsServer) { SetStartStatsRpc(); }

        InitialHeroSetting.Instance.OnInitializeCrownHP += TurnManager_OnInitializeCrownHP;
    }

    private void TurnManager_OnInitializeCrownHP(object sender, EventArgs e)
    {
        if (IsLocalPlayer) { ChangeCrownHPOnTurnManagerRpc(); }
    }

    [Rpc(SendTo.Server)]
    private void ChangeCrownHPOnTurnManagerRpc(RpcParams rpcParams = default)
    {
        TurnManager.Instance.ChangeCrownHP(rpcParams.Receive.SenderClientId, currentHP.Value);
    }

    //Startwerte Setzen
    [Rpc(SendTo.Server)]
    private void SetStartStatsRpc()
    {
        Debug.Log($"CrownHP werden auf {maxHP} gesetzt.");
        currentHP.Value = maxHP;
        //Debug.Log("Startstats eingegeben für " + this + "!");
    }

    //Methode zum Abziehen von HP
    [Rpc(SendTo.Server)]
    public void DecreaseHPRpc(int amount)
    {
        Debug.Log($"CrownHP werden um {amount} verringert.");
        currentHP.Value -= amount;
        if (currentHP.Value < 0)
        {
            currentHP.Value = 0;      //Sicherstellen, dass HP nicht negativ wird
        }

        ChangeCrownHPOnTurnManagerRpc();

        UpdateHPDisplay();
    }

    //Methode zum Hinzufügen von HP
    [Rpc(SendTo.Server)]
    public void IncreaseHPRpc(int amount)
    {
        Debug.Log($"CrownHP werden um {amount} erhöht.");
        currentHP.Value += amount;
        if (currentHP.Value > maxHP)
        {
            currentHP.Value = maxHP;      //Sicherstellen, dass HP nicht über maxHP steigt
        }

        ChangeCrownHPOnTurnManagerRpc();

        UpdateHPDisplay();
    }

    //Methode zur Aktualisierung der HP-Anzeige
    private void UpdateHPDisplay()
    {
        UpdateHPDisplayRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateHPDisplayRpc()
    {
        int ones = currentHP.Value % 10;      //Einerstelle
        int tens = currentHP.Value / 10;      //Zehnerstelle

        //Räder entsprechend der aktuellen HP rotieren lassen
        float targetRotationOnes = ones * -36f;
        float targetRotationTens = tens * -36f;
        StartCoroutine(WheelRotator(targetRotationOnes, targetRotationTens));
        OnSetNewHPStatus?.Invoke(targetRotationOnes, targetRotationTens);
    }

    //Für das erste Anzeigenupdate
    public void UpdateHPDisplayGlobal()
    {
        UpdateHPDisplayRpc();
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
}

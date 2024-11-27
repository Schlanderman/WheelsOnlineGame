using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownManager : MonoBehaviour
{
    [SerializeField] private int maxHP = 10;      //Maximaler HP-Wert, mit dem der Spieler beginnt
    private int currentHP;

    //Referenzen zu den  Rädern für die Anzeige
    [SerializeField] Transform oneWheel;
    [SerializeField] Transform tenWheel;

    private void Start()
    {
        SetStartStats();
    }

    //Startwerte Setzen
    private void SetStartStats()
    {
        currentHP = maxHP;
        UpdateHPDisplay();
        Debug.Log("Startstats eingegeben für " + this + "!");
    }

    //Methode zum Abziehen von HP
    public void DecreaseHP(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0)
        {
            currentHP = 0;      //Sicherstellen, dass HP nicht negativ wird
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
    }

    private IEnumerator WheelRotator(float rotationOnes, float rotationTens)
    {
        //Dauer der Animation
        float duration = 0.3f;
        float timeElapsed = 0f;

        //Die aktuelle Rotation in Quaternion speichern
        Quaternion initialRotationOnes = oneWheel.rotation;
        Quaternion initialRotationTens = tenWheel.rotation;

        //Zielrotation in Quaternion berechnen, um eine sanfte Imterpolation durchzuführen
        Quaternion targetRotationOnes = Quaternion.Euler(rotationOnes, oneWheel.eulerAngles.y, oneWheel.eulerAngles.z);
        Quaternion targetRotationTens = Quaternion.Euler(rotationTens, tenWheel.eulerAngles.y, tenWheel.eulerAngles.z);

        //Führe die Interpolation über die Zeit durch
        while (timeElapsed < duration)
        {
            //Linear interpolieren zwischen der Startrotation und der Zielrotation
            oneWheel.rotation = Quaternion.Lerp(initialRotationOnes, targetRotationOnes, timeElapsed / duration);
            tenWheel.rotation = Quaternion.Lerp(initialRotationTens, targetRotationTens, timeElapsed / duration);

            //Erhöhe die verstrichene Zeit
            timeElapsed += Time.deltaTime;
            //Warte bis zum nächsten Frame
            yield return null;
        }

        oneWheel.rotation = targetRotationOnes;
        tenWheel.rotation = targetRotationTens;
    }

    //Methode zum auslesen der HP
    public int GetCurrentHP()
    {
        return currentHP;
    }
}

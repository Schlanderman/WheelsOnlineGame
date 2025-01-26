using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPLightManager : MonoBehaviour
{
    [SerializeField] private GameObject[] xpLamps;      //Array von Lampen
    [SerializeField] private Material lampOffMaterial;  //Material für die ausgeschaltete Lampe
    [SerializeField] private Material lampOnMaterial;   //Material für die eingeschaltete Lampe

    //Events
    public event Action<int> OnActivateLampUpdate;

    //Methode zum Aktualisieren der Lampen basierend auf dem XP-Wert des Helden
    public void UpdateXPLamps(int currentXP)
    {
        OnActivateLampUpdate?.Invoke(currentXP);

        //Iteriere über alle Lampen
        for (int i = 0; i < xpLamps.Length; i++)
        {
            //Wenn die XP des Helden größer sind, als der Index der Lampe, dann wird diese eingeschaltet
            if (i < currentXP)
            {
                SetLampMaterial(xpLamps[i], lampOnMaterial);    //Lampe einschalten
            }
            else
            {
                SetLampMaterial(xpLamps[i], lampOffMaterial);   //Lampe ausschalten
            }
        }
    }

    //Methode zum setzen des Materials der Lampe
    private void SetLampMaterial(GameObject lamp, Material material)
    {
        Renderer lampRenderer = lamp.GetComponent<Renderer>();
        if (lampRenderer != null)
        {
            lampRenderer.material = material;
        }
    }
}

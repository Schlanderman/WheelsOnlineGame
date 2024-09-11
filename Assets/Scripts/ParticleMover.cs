using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMover : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart dollyCart;    //Verweise auf das Dolly Cart

    [SerializeField] private GameObject star;       //Star Objekt
    [SerializeField] private GameObject square;     //Square Objekt
    [SerializeField] private GameObject diamond;    //Diamond Objekt
    [SerializeField] private GameObject hammer;     //Hammer Objekt

    [SerializeField] private float duration = 1f; //Zeit, um den gesamten Pfad zu durchlaufen (Standard 0.8 Sekunden)
    private float timeElapsed = 0f;     //Zeit, die seit dem Start vergangen ist

    private bool active = false;

    private void Start()
    {
        star.SetActive(false);
        square.SetActive(false);
        diamond.SetActive(false);
        hammer.SetActive(false);
    }

    private void Update()
    {
        if (active)
        {
            //Berechne den Fortschritt (zwischen 0 und 1)
            timeElapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(timeElapsed / duration);

            //setze die Position auf dem Track basierend auf dem Fortschritt
            dollyCart.m_Position = progress * dollyCart.m_Path.PathLength;

            //Wenn der Fortschritt 100% erreicht hat, stoppe das Update
            if (progress >= 1f)
            {
                active = false;
                timeElapsed = 0f;

                star.SetActive(false);
                square.SetActive(false);
                diamond.SetActive(false);
                hammer.SetActive(false);
            }
        }
    }

    public void ActivateMovment(string symbol)
    {
        if (symbol == "Star")
        {
            star.SetActive(true);
        }
        else if (symbol == "Square")
        {
            square.SetActive(true);
        }
        else if (symbol == "Diamond")
        {
            diamond.SetActive(true);
        }
        else if (symbol == "Hammer")
        {
            hammer.SetActive(true);
        }
        else
        {
            Debug.LogError(symbol + " ist nicht vorhanden und kann nicht aktiviert werden!");
            return;
        }
        
        active = true;
    }
}

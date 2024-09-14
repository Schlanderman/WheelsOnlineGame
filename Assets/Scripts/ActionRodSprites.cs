using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRodSprites : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject book;
    [SerializeField] private GameObject bomb;
    [SerializeField] private GameObject dagger;
    [SerializeField] private GameObject fireball;
    [SerializeField] private GameObject hammer;
    [SerializeField] private GameObject sword;

    private void Start()
    {
        //Alles erstmal deaktivieren
        DeactivateAll();
    }

    //Methoden, um die Sprites wieder sichtbar zu machen
    public void ActivateArrow()
    {
        arrow.SetActive(true);
    }

    public void ActivateBomb()
    {
        bomb.SetActive(true);
    }

    public void ActivateBook()
    {
        book.SetActive(true);
    }

    public void ActivateDagger()
    {
        dagger.SetActive(true);
    }

    public void ActivateFireball()
    {
        fireball.SetActive(true);
    }

    public void ActivateHammer()
    {
        hammer.SetActive(true);
    }

    public void ActivateSword()
    {
        sword.SetActive(true);
    }

    //Alles auschalten
    public void DeactivateAll()
    {
        arrow.SetActive(false);
        bomb.SetActive(false);
        book.SetActive(false);
        dagger.SetActive(false);
        fireball.SetActive(false);
        hammer.SetActive(false);
        sword.SetActive(false);
    }
}

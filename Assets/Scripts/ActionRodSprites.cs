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

    //private SpriteRenderer arrowSprite;
    //private SpriteRenderer bookSprite;
    //private SpriteRenderer daggerSprite;
    //private SpriteRenderer fireballSprite;
    //private SpriteRenderer hammerSprite;
    //private SpriteRenderer swordSprite;

    private void Start()
    {
        //Sprites von Gameobjects holen
        //arrowSprite = arrow.GetComponent<SpriteRenderer>();
        //bookSprite = book.GetComponent<SpriteRenderer>();
        //daggerSprite = dagger.GetComponent<SpriteRenderer>();
        //fireballSprite = fireball.GetComponent<SpriteRenderer>();
        //hammerSprite = hammer.GetComponent<SpriteRenderer>();
        //swordSprite = sword.GetComponent<SpriteRenderer>();

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

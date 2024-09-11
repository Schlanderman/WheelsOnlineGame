using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnimationManager : MonoBehaviour
{
    private Hero squareHero;
    private Hero diamondHero;

    private Animator squareAnimator;
    private Animator diamondAnimator;

    //Methode um die Helden und Animatoren zuzuweisen
    public void SetAnimators(Hero square, Hero diamond)
    {
        //Heldenzuweiseung
        squareHero = square;
        diamondHero = diamond;

        //Animatorzuweisung
        squareAnimator = square.GetComponent<Animator>();
        diamondAnimator = diamond.GetComponent<Animator>();
    }

    public void TriggerHeroAction(string stand)
    {
        if (stand == "Square")
        {
            squareAnimator.SetTrigger("ActivateAction");
        }
        else if (stand == "Diamond")
        {
            diamondAnimator.SetTrigger("ActivateAction");
        }
        else
        {
            Debug.LogError(stand + " ist keine valide Heldenseite!");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnimationManager : MonoBehaviour
{
    private Animator squareAnimator;
    private Animator diamondAnimator;

    //Methode um die Helden und Animatoren zuzuweisen
    public void SetAnimators(GameObject square, GameObject diamond)
    {
        //Animatorzuweisung
        squareAnimator = square.GetComponentInChildren<Animator>();
        diamondAnimator = diamond.GetComponentInChildren<Animator>();
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

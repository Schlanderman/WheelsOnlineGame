using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator Animator;

    public void TriggerHeroAction()
    {
        Animator.SetTrigger("ActivateAction");
    }
}

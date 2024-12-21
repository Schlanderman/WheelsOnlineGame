using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HeroAnimationManager : NetworkBehaviour
{
    [SerializeField] private Animator Animator;

    public void TriggerHeroAction()
    {
        Animator.SetTrigger("ActivateAction");
    }
}

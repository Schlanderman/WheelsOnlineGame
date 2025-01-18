using System;
using System.Collections;
using UnityEngine;

public class ActionRodAnimManager : MonoBehaviour
{
    //Action Rods
    [SerializeField] private GameObject[] actionRod;

    //Sprite Skripte
    private ActionRodSprites[] rodSprite = new ActionRodSprites[4];

    //Animators
    private Animator[] rodAnimator = new Animator[4];

    //Events
    public event Action<int, string, string> OnActivateActionRodAnimation;

    //Alles zuweisen
    private void Start()
    {
        for (int i = 0; i < actionRod.Length; i++)
        {
            //Debug.Log("An Posotion " + i + " ist das Element: " + actionRod[i] + ", was das Element " + actionRod[i].GetComponentInChildren<ActionRodSprites>() + " enthält und nun in " + rodSprite[i] + " einfügt.");
            rodSprite[i] = actionRod[i].GetComponentInChildren<ActionRodSprites>();
            rodAnimator[i] = actionRod[i].GetComponent<Animator>();
        }
    }

    //Animation ausführen
    public float ActivateRodAnimation(int rodNumber, string sprite, string animation)
    {
        switch (sprite)
        {
            case "Arrow":
                rodSprite[rodNumber].ActivateArrow();
                break;

            case "Bomb":
                rodSprite[rodNumber].ActivateBomb();
                break;

            case "Book":
                rodSprite[rodNumber].ActivateBook();
                break;

            case "Dagger":
                rodSprite[rodNumber].ActivateDagger();
                break;

            case "Fireball":
                rodSprite[rodNumber].ActivateFireball();
                break;

            case "Hammer":
                rodSprite[rodNumber].ActivateHammer();
                break;

            case "Sword":
                rodSprite[rodNumber].ActivateSword();
                break;

            default:
                Debug.LogError(sprite + " ist kein Valides Argunent für die Spriteauswahl!");
                break;
        }

        OnActivateActionRodAnimation?.Invoke(rodNumber, sprite, animation);
        return AnimationMaker(rodNumber, animation);
    }

    private float AnimationMaker(int rodNumber, string animation)
    {
        //Animation triggern
        rodAnimator[rodNumber].SetTrigger(animation);

        //Länge der Animation ermitteln
        float animationLength = rodAnimator[rodNumber].GetCurrentAnimatorStateInfo(0).length;

        //Coroutine für die Deaktivierung starten
        StartCoroutine(HandleAnimationCompletion(rodNumber, animationLength));

        //Animationsdauer zurückgeben
        return animationLength;
    }

    private IEnumerator HandleAnimationCompletion(int rodNumber, float animationLength)
    {
        //Warten, bis die Animation fertig ist
        yield return new WaitForSeconds(animationLength + 0.5f);

        //Deaktiviere alle Sprites
        rodSprite[rodNumber].DeactivateAll();
    }

    public void TestActivateRodAnimation(int rodNumber, string sprite, string animation)
    {
        switch (sprite)
        {
            case "Arrow":
                rodSprite[rodNumber].ActivateArrow();
                break;

            case "Bomb":
                rodSprite[rodNumber].ActivateBomb();
                break;

            case "Book":
                rodSprite[rodNumber].ActivateBook();
                break;

            case "Dagger":
                rodSprite[rodNumber].ActivateDagger();
                break;

            case "Fireball":
                rodSprite[rodNumber].ActivateFireball();
                break;

            case "Hammer":
                rodSprite[rodNumber].ActivateHammer();
                break;

            case "Sword":
                rodSprite[rodNumber].ActivateSword();
                break;

            default:
                Debug.LogError(sprite + " ist kein Valides Argunent für die Spriteauswahl!");
                break;
        }

        //StartCoroutine(AnimationMaker(rodNumber, animation));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRodAnimManager : MonoBehaviour
{
    //Action Rods
    [SerializeField] private GameObject[] actionRod;

    //Sprite Skripte
    private ActionRodSprites[] rodSprite = new ActionRodSprites[4];

    //Animators
    private Animator[] rodAnimator = new Animator[4];

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

    //Animation ausführen (ohne Zeitrückgabe)
    public IEnumerator ActivateRodAnimation(int rodNumber, string sprite, string animation)
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

        yield return StartCoroutine(AnimationMaker(rodNumber, animation));
    }

    //Animation ausführen (mit Zeitrückgabe)
    public float ActivateRodAnimationTime(int rodNumber, string sprite, string animation)
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

        StartCoroutine(AnimationMaker(rodNumber, animation));
        return rodAnimator[rodNumber].GetCurrentAnimatorStateInfo(0).length;
    }

    private IEnumerator AnimationMaker(int rodNumber, string animation)
    {
        //Animation triggern
        rodAnimator[rodNumber].SetTrigger(animation);

        //Warte, bis die Animation fertig ist
        yield return new WaitForSeconds(rodAnimator[rodNumber].GetCurrentAnimatorStateInfo(0).length);

        rodSprite[rodNumber].DeactivateAll();
    }
}

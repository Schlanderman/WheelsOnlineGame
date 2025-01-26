using System;
using System.Collections;
using System.Linq;
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

        OnActivateActionRodAnimation?.Invoke(rodNumber, sprite, animation);
        yield return AnimationMaker(rodNumber, animation);
    }

    private IEnumerator AnimationMaker(int rodNumber, string animation)
    {
        //Animation triggern
        rodAnimator[rodNumber].SetTrigger(animation);

        //Länge der Animation ermitteln
        float animationLength = rodAnimator[rodNumber].GetCurrentAnimatorStateInfo(0).length;

        //Coroutine für die Deaktivierung starten
        yield return StartCoroutine(HandleAnimationCompletion(rodNumber, animationLength));
    }

    private IEnumerator HandleAnimationCompletion(int rodNumber, float animationLength)
    {
        //Warten, bis die Animation fertig ist
        yield return new WaitForSeconds(animationLength + 0.5f);

        //Deaktiviere alle Sprites
        rodSprite[rodNumber].DeactivateAll();
    }

    public float GetAnimationLength(int rodNumber, string animation)
    {
        //Potentiell noch extra 0.5 Sekunden hinzufügen, falls sich Animationen überlagern
        return rodAnimator[rodNumber].runtimeAnimatorController.animationClips.First(clip => clip.name == animation).length + 0.8f;
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

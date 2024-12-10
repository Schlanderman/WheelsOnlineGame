using System.Collections;
using UnityEngine;

public class CopyActionRods : ManagerCopiesHandler<ActionRodAnimManager>
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
            rodSprite[i] = actionRod[i].GetComponentInChildren<ActionRodSprites>();
            rodAnimator[i] = actionRod[i].GetComponent<Animator>();
        }
    }

    protected override void SetEvents()
    {
        originalManager.OnActivateActionRodAnimation += ActionRodAnimManager_OnActivateActionRodAnimation;
    }

    private void ActionRodAnimManager_OnActivateActionRodAnimation(int rodNumber, string sprite, string animation)
    {
        string changedAnimation = ChangeAnimationString(animation);

        StartCoroutine(ActivateRodAnimation(rodNumber, sprite, changedAnimation));
    }

    //Animation ausführen (ohne Zeitrückgabe)
    private IEnumerator ActivateRodAnimation(int rodNumber, string sprite, string animation)
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

    private IEnumerator AnimationMaker(int rodNumber, string animation)
    {
        //Animation triggern
        rodAnimator[rodNumber].SetTrigger(animation);

        //Warte, bis die Animation fertig ist
        yield return new WaitForSeconds(rodAnimator[rodNumber].GetCurrentAnimatorStateInfo(0).length);

        yield return new WaitForSeconds(0.5f);
        rodSprite[rodNumber].DeactivateAll();
    }

    private string ChangeAnimationString(string animation)
    {
        if (animation == "AttackRightBulwark") { return "AttackLeftBulwark"; }
        if (animation == "AttackLeftBulwark") { return "AttackRightBulwark"; }
        if (animation == "AttackRightCrown") { return "AttackLeftCrown"; }
        if (animation == "AttackLeftCrown") { return "AttackRightCrown"; }
        if (animation == "FireballRightHigh") { return "FireballLeftHigh"; }
        if (animation == "FireballLeftHigh") { return "FireballRightHigh"; }
        if (animation == "ArrowRightBulwark") { return "ArrowLeftBulwark"; }
        if (animation == "ArrowLeftBulwark") { return "ArrowRightBulwark"; }
        if (animation == "ArrowRightCrown") { return "ArrowLeftCrown"; }
        if (animation == "ArrowLeftCrown") { return "ArrowRightCrown"; }
        
        return animation;
    }
}

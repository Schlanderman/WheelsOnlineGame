using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EvaluationManager : NetworkBehaviour
{
    [SerializeField] private BulwarkMover bulwarkMover;

    [SerializeField] private ParticleManager particleManager;

    [SerializeField] private WheelManager wheelManager;

    private Hero squareHero;    // Held, der Energie von Square-Symbolen erh�lt
    private Hero diamondHero;   // Held, der Energie von Diamond-Symbolen erh�lt

    private void Start()
    {
        TurnManager.Instance.OnApplyXPandLevelUps += TurnManager_OnApplyXPandLevelUps;
        TurnManager.Instance.OnApplyHammerPanels += TurnManager_OnApplyHammerPanels;
        TurnManager.Instance.OnApplyEnergyPanels += TurnManager_OnApplyEnergyPanels;
    }

    private void TurnManager_OnApplyXPandLevelUps(object sender, EventArgs e)
    {
        EvaluateXPGained(wheelManager.GetWheels());
    }

    private void TurnManager_OnApplyHammerPanels(object sender, EventArgs e)
    {
        EvaluateHammerCount(wheelManager.GetWheels());
    }

    private void TurnManager_OnApplyEnergyPanels(object sender, EventArgs e)
    {
        EvaluateEnergyCount(wheelManager.GetWheels());
    }

    public void EvaluateXPGained(WheelSpin[] wheels)
    {
        int xpSquareCount = 0;
        int xpDiamondCount = 0;
        int wheelNumber = 0;
        bool[] wheelsActivateAnimationSquare = new bool[5] { false, false, false, false, false };
        bool[] wheelsActivateAnimationDiamond = new bool[5] { false, false, false, false, false };

        foreach (var wheel in wheels)
        {
            Symbol topSymbol = wheel.GetCurrentSymbol();

            if (!Enum.IsDefined(typeof(Symbol), topSymbol))
            {
                Debug.LogWarning("Das Symbol " + topSymbol + " von " + wheel + " ist nicht in diesem Kontext vorhanden!");
                break;
            }

            switch (topSymbol)
            {
                case Symbol.SquarePlus:
                case Symbol.SquareSquarePlus:
                    xpSquareCount++;

                    //Partikelanimation vorbereiten
                    //particleManager.ActivateParticleMove(wheelNumber, "SquareStar");
                    wheelsActivateAnimationSquare[wheelNumber] = true;
                    break;

                case Symbol.DiamondPlus:
                case Symbol.DiamondDiamondPlus:
                    xpDiamondCount++;

                    //Partikelanimation vorbereiten
                    //particleManager.ActivateParticleMove(wheelNumber, "DiamondStar");
                    wheelsActivateAnimationDiamond[wheelNumber] = true;
                    break;

                default:
                    break;
            }

            wheelNumber++;
        }

        //Berechne die XP f�r die Square- und Diamant-Helden
        UpdateHeroEnergyRpc(squareHero.gameObject.GetComponent<NetworkObject>(), 0, xpSquareCount, wheelsActivateAnimationSquare);
        UpdateHeroEnergyRpc(diamondHero.gameObject.GetComponent<NetworkObject>(), 0, xpDiamondCount, wheelsActivateAnimationDiamond);
    }

    public void EvaluateHammerCount(WheelSpin[] wheels)
    {
        int hammerCount = 0;
        int wheelNumber = 0;
        bool[] wheelsActivateAnimation = new bool[5] { false, false, false, false, false };

        foreach (var wheel in wheels)
        {
            Symbol topSymbol = wheel.GetCurrentSymbol();

            if (!Enum.IsDefined(typeof(Symbol), topSymbol))
            {
                Debug.LogWarning("Das Symbol " + topSymbol + " von " + wheel + " ist nicht in diesem Kontext vorhanden!");
                break;
            }

            switch (topSymbol)
            {
                case Symbol.Hammer:
                    hammerCount++;

                    //Partikelanimation vorbereiten
                    //particleManager.ActivateParticleMove(wheelNumber, "Hammer");
                    wheelsActivateAnimation[wheelNumber] = true;
                    break;

                case Symbol.HammerHammer:
                    hammerCount += 2;

                    //Partikelanimation vorbereiten
                    //particleManager.ActivateParticleMove(wheelNumber, "Hammer");
                    wheelsActivateAnimation[wheelNumber] = true;
                    break;

                case Symbol.HammerHammerHammer:
                    hammerCount += 3;

                    //Partikelanimation vorbereiten
                    //particleManager.ActivateParticleMove(wheelNumber, "Hammer");
                    wheelsActivateAnimation[wheelNumber] = true;
                    break;

                default:
                    break;
            }

            wheelNumber++;
        }

        //Bulwark-Level erh�hen, wenn gen�gend H�mmer gerollt wurden
        UpdateBulwarkRpc(hammerCount, wheelsActivateAnimation);
    }

    public void EvaluateEnergyCount(WheelSpin[] wheels)
    {
        //Z�hler f�r die Symbole
        int squareCount = 0;
        int diamondCount = 0;
        int wheelNumber = 0;
        bool[] wheelsActivateAnimationSquare = new bool[5] { false, false, false, false, false };
        bool[] wheelsActivateAnimationDiamond = new bool[5] { false, false, false, false, false };

        foreach (var wheel in wheels)
        {
            Symbol topSymbol = wheel.GetCurrentSymbol();

            if (!Enum.IsDefined(typeof(Symbol), topSymbol))
            {
                Debug.LogWarning("Das Symbol " + topSymbol + " von " + wheel + " ist nicht in diesem Kontext vorhanden!");
                break;
            }

            switch (topSymbol)
            {
                case Symbol.Square:
                case Symbol.SquarePlus:
                    squareCount++;

                    //Partikelanimation vorbereiten
                    wheelsActivateAnimationSquare[wheelNumber] = true;
                    break;

                case Symbol.SquareSquare:
                case Symbol.SquareSquarePlus:
                    squareCount += 2;

                    //Partikelanimation vorbereiten
                    wheelsActivateAnimationSquare[wheelNumber] = true;
                    break;

                case Symbol.Diamond:
                case Symbol.DiamondPlus:
                    diamondCount++;

                    //Partikelanimation vorbereiten
                    wheelsActivateAnimationDiamond[wheelNumber] = true;
                    break;

                case Symbol.DiamondDiamond:
                case Symbol.DiamondDiamondPlus:
                    diamondCount += 2;

                    //Partikelanimation vorbereiten
                    wheelsActivateAnimationDiamond[wheelNumber] = true;
                    break;

                default:
                    break;
            }

            wheelNumber++;
        }

        //Berechne die Energie f�r die Square- und Diamant-Helden
        UpdateHeroEnergyRpc(squareHero.gameObject.GetComponent<NetworkObject>(), squareCount, 0, wheelsActivateAnimationSquare);
        UpdateHeroEnergyRpc(diamondHero.gameObject.GetComponent<NetworkObject>(), diamondCount, 0, wheelsActivateAnimationDiamond);
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateHeroEnergyRpc(NetworkObjectReference heroNetworkObjectReference, int symbolCount, int gainedXP, bool[] whatWheels)
    {
        if (!heroNetworkObjectReference.TryGet(out NetworkObject heroNetworkObject))
        {
            Debug.LogWarning($"{heroNetworkObjectReference} enth�lt kein NetworkObject!");
            return;
        }

        Hero hero = heroNetworkObject.GetComponent<Hero>();

        StartCoroutine(UpdateHeroEnergy(hero, symbolCount, gainedXP, whatWheels));
    }

    private IEnumerator UpdateHeroEnergy(Hero hero, int symbolCount, int gainedXP, bool[] whatWheels)
    {
        int energyGained = 0;
        if (symbolCount >= 3)
        {
            //Energieberechnung: Ziehe 2 von der Anzahl der Symbole ab
            energyGained = symbolCount - 2;
        }

        string toWhatHeroXP = "SquareStar";
        string toWhatHeroSymbol = "Square";
        if (hero == diamondHero)
        {
            toWhatHeroXP = "DiamondStar";
            toWhatHeroSymbol = "Diamond";
        }

        //XP Animation
        if (gainedXP >= 1)
        {
            for (int i = 0; i < whatWheels.Length; i++)
            {
                if (whatWheels[i])
                {
                    particleManager.ActivateParticleMove(i, toWhatHeroXP);
                }
            }
        }

        //Symbol Animation
        if (symbolCount >= 1)
        {
            if (energyGained >= 1)
            {
                for (int i = 0; i < whatWheels.Length; i++)
                {
                    if (whatWheels[i])
                    {
                        particleManager.ActivateParticleMove(i, toWhatHeroSymbol);
                    }
                }
            }
        }

        //Nur XP an den Helden geben, wenn es der Server ausf�hrt
        if (!IsServer) { yield break; }

        yield return new WaitForSeconds(0.8f);

        if (gainedXP >= 1)
        {
            hero.AddXP(gainedXP);
        }

        if (energyGained >= 1)
        {
            hero.AddEnergy(energyGained);
        }

        Debug.Log(hero + " hat " + gainedXP + " XP, und " + energyGained + " Energie erhalten");
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateBulwarkRpc(int hammerCount, bool[] whatWheels)
    {
        StartCoroutine(UpdateBulwark(hammerCount, whatWheels));
    }

    private IEnumerator UpdateBulwark(int hammerCount, bool[] whatWheels)
    {
        int bulwarkIncrease = 0;
        if (hammerCount >= 3)
        {
            //Bulwark wird um die Anzahl der H�mmer - 2 erh�ht
            bulwarkIncrease = hammerCount - 2;
            //Debug.Log("Das Bulwark ist " + (hammerCount - 2) + " Stufen gr��er geworden.");
        }

        if (bulwarkIncrease >= 1)
        {
            for (int i = 0; i < whatWheels.Length; i++)
            {
                if (whatWheels[i])
                {
                    particleManager.ActivateParticleMove(i, "Hammer");
                }
            }
        }

        //Nur den Bulwark erh�hen, wenn es der Server ausf�hrt
        if (!IsServer) { yield break; }

        yield return new WaitForSeconds(0.8f);

        bulwarkMover.increaseBulwark(bulwarkIncrease);
    }

    public void SetHeroes(Hero square, Hero diamond)
    {
        squareHero = square;
        diamondHero = diamond;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] ParticleMover[] squareParticles;
    [SerializeField] ParticleMover[] diamondParticles;
    [SerializeField] ParticleMover[] hammerParticles;

    //Events
    public event Action<int, string> OnActivateParticleMove;

    public void ActivateParticleMove(int wheel, string particle)
    {
        //Schauen, welches Rad angesprochen wird
        for (int i = 0; i < 5; i++)
        {
            if (i == wheel)
            {
                //Schauen, welches Partikel gesender werden soll.
                switch (particle)
                {
                    case "Square":
                        squareParticles[i].ActivateMovment("Square");
                        break;

                    case "Diamond":
                        diamondParticles[i].ActivateMovment("Diamond");
                        break;

                    case "Hammer":
                        hammerParticles[i].ActivateMovment("Hammer");
                        break;

                    case "SquareStar":
                        squareParticles[i].ActivateMovment("Star");
                        break;

                    case "DiamondStar":
                        diamondParticles[i].ActivateMovment("Star");
                        break;

                    default:
                        Debug.LogError(particle + " ist kein Valides Argument!");
                        break;
                }
            }
        }

        OnActivateParticleMove?.Invoke(wheel, particle);
    }
}

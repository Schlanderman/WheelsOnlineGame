using UnityEngine;

public class CopyXPParticles : ManagerCopiesHandler<ParticleManager>
{
    [SerializeField] ParticleMover[] squareParticles;
    [SerializeField] ParticleMover[] diamondParticles;
    [SerializeField] ParticleMover[] hammerParticles;

    private void ActivateParticleMove(int wheel, string particle)
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
    }

    protected override void SetEvents()
    {
        originalManager.OnActivateParticleMove += OriginalParticles_OnActivateParticleMove;
    }

    private void OriginalParticles_OnActivateParticleMove(int wheel, string particle)
    {
        ActivateParticleMove(wheel, particle);
    }
}

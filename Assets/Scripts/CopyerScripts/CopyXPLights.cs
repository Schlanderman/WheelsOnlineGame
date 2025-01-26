using UnityEngine;

public class CopyXPLights : ManagerCopiesHandler<XPLightManager>
{
    [SerializeField] private GameObject[] xpLamps;      //Array von Lampen
    [SerializeField] private Material lampOffMaterial;  //Material für die ausgeschaltete Lampe
    [SerializeField] private Material lampOnMaterial;   //Material für die eingeschaltete Lampe

    protected override void SetEvents()
    {
        originalManager.OnActivateLampUpdate += XPLightManager_OnActivateLampUpdate;
    }

    private void XPLightManager_OnActivateLampUpdate(int currentXP)
    {
        UpdateXPLamps(currentXP);
    }



    //Methode zum Aktualisieren der Lampen basierend auf dem XP-Wert des Helden
    private void UpdateXPLamps(int currentXP)
    {
        //Iteriere über alle Lampen
        for (int i = 0; i < xpLamps.Length; i++)
        {
            //Wenn die XP des Helden größer sind, als der Index der Lampe, dann wird diese eingeschaltet
            if (i < currentXP)
            {
                SetLampMaterial(xpLamps[i], lampOnMaterial);    //Lampe einschalten
            }
            else
            {
                SetLampMaterial(xpLamps[i], lampOffMaterial);   //Lampe ausschalten
            }
        }
    }

    //Methode zum setzen des Materials der Lampe
    private void SetLampMaterial(GameObject lamp, Material material)
    {
        Renderer lampRenderer = lamp.GetComponent<Renderer>();
        if (lampRenderer != null)
        {
            lampRenderer.material = material;
        }
    }
}

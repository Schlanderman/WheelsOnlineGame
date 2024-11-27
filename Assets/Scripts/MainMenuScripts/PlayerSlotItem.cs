using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSlotItem : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public RawImage playerIcon;

    public string playerId {  get; private set; }

    public void SetPlayerData(string name, string id)
    {
        playerName.text = name;
        //playerIcon.texture = icon;
        playerId = id;
    }
}

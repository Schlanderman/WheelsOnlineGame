using UnityEngine;
using TMPro;

public class LobbyListItem : MonoBehaviour
{
    public TextMeshProUGUI lobbyNameText;
    public TextMeshProUGUI playerCountText;

    public void SetLobbyDetails(string name, int currentPlayers, int maxPlayers)
    {
        lobbyNameText.text = name;
        playerCountText.text = $"{currentPlayers}/{maxPlayers}";
    }
}

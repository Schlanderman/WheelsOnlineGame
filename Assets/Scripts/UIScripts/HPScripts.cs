using UnityEngine;
using TMPro;

public class HPScripts : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerHP;
    [SerializeField] private TextMeshProUGUI enemyHP;

    public void SetCurrentHP(ulong playerId, int currentHP)
    {
        if (playerId == PlayerScript.Instance.playerId)
        {
            playerHP.text = currentHP.ToString();
        }
        else
        {
            enemyHP.text = currentHP.ToString();
        }
    }
}

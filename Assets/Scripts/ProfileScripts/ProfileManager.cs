using UnityEngine;
using TMPro;

public class ProfileManager : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_Text displayName;

    private void Start()
    {
        nameInputField.gameObject.SetActive(false);

        //Lade den gespeicherten Namen (falls vorhanden)
        //if (PlayerPrefs.HasKey("ProfileName"))
        //{
        //    string savedName = PlayerPrefs.GetString("ProfileName");
        //    displayName.text = savedName;
        //    nameInputField.text = savedName;
        //}

        displayName.text = ProfileData.Instance.ProfileName;
        nameInputField.text = ProfileData.Instance.ProfileName;
    }

    public void ChangeName()
    {
        nameInputField.gameObject.SetActive(true);
        // Aktiviert das InputField und bringt es in den Bearbeitungsmodus
        nameInputField.ActivateInputField();
    }

    public void AcceptNewName()
    {
        nameInputField.gameObject.SetActive(false);
        string newName = nameInputField.text;
        displayName.text = newName;
        ProfileData.Instance.SetProfileName(newName);
    }

    private void SaveProfileName()
    {
        string newName = nameInputField.text;
        displayName.text = newName;
        PlayerPrefs.SetString("ProfileName", newName);  //Speichern für die nächste Sitzung
        PlayerPrefs.Save();
    }
}

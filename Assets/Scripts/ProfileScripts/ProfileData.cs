using UnityEngine;
using System.IO;

public class ProfileData : MonoBehaviour
{
     //Singleton Instanz
     public static ProfileData Instance { get; private set; }

    //Profildaten
    public string ProfileName { get; private set; }
    public Texture2D ProfileImage { get; private set; }
    public bool DontShowStartScreenAgain { get; private set; }

    private const string ProfileNameKey = "ProfileName";
    private const string ProfileImagePathKey = "ProfileImagePath";
    private const string ProfileDontShowStartAgainKey = "DontShowStartAgain";

    private void Awake()
    {
        //Singleton-Initialisierung
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadProfileData();
    }

    //Setzt den Profilnamen und speichert ihn
    public void SetProfileName(string name)
    {
        ProfileName = name;
        PlayerPrefs.SetString(ProfileNameKey, name);
        PlayerPrefs.Save();
    }

    //Setzt das Profilbild und speichert es
    public void SetProfileImage(Texture2D image)
    {
        ProfileImage = image;

        //Speichern des Bildes auf dem Dateisystem
        string imagePath = Application.persistentDataPath + "/profileImage.png";
        File.WriteAllBytes(imagePath, image.EncodeToPNG());
        PlayerPrefs.SetString(ProfileImagePathKey, imagePath);
        PlayerPrefs.Save();
    }

    //Setzt, ob die Startnachricht erneut gezeigt werden soll
    public void SetShowStartScreenAgain(bool valueToSave)
    {
        DontShowStartScreenAgain = valueToSave;

        //Speichern, ob der Startscreen angezeigt werden soll
        PlayerPrefs.SetInt(ProfileDontShowStartAgainKey, valueToSave ? 1 : 0);
        PlayerPrefs.Save();
    }

    //Lädt die Profildaten aus den PlayerPrefs
    private void LoadProfileData()
    {
        //Profilname Laden
        ProfileName = PlayerPrefs.GetString(ProfileNameKey, "Player");

        //ShowStartScreenAgain Laden
        DontShowStartScreenAgain = PlayerPrefs.GetInt(ProfileDontShowStartAgainKey, 0) == 1;    //Standardwert ist 0 (false)

        //Profilbild laden
        string imagePath = PlayerPrefs.GetString(ProfileImagePathKey, string.Empty);
        if (File.Exists(imagePath))
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            ProfileImage = new Texture2D(2, 2);
            ProfileImage.LoadImage(imageData);
        }
        else
        {
            ProfileImage = null;
        }
    }

    //ProfileData zurücksetzen
    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Alle PlayerPrefs wurden zurückgesetzt!");
    }
}

using SFB;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfilePictureManager : MonoBehaviour
{
    public RawImage profileImage;

    private string profilePicturePath => Path.Combine(Application.persistentDataPath, "ProfilePicture.png");

    private void Start()
    {
        //LoadProfilePicture();
        profileImage.texture = ProfileData.Instance.ProfileImage;
    }

    public void UploadProfilePicture()
    {
        //Öffne den Datei-Browser
        var paths = StandaloneFileBrowser.OpenFilePanel("Wähle ein Profilbild", "", "png", false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            StartCoroutine(LoadImage(paths[0]));
        }
    }

    //Speichert die Textur auf der Festplatte
    private void SaveProfilePicture(Texture2D texture)
    {
        //Konvertiere die Textur in ein PNG-Byte-Array
        byte[] imageData = texture.EncodeToPNG();

        //Speichere das Bild im Datei-System
        File.WriteAllBytes(profilePicturePath, imageData);

        //Speichere den Pfad in PlayerPrefs
        PlayerPrefs.SetString("ProfilePicturePath", profilePicturePath);
        PlayerPrefs.Save();
    }

    //Lädt die Textur von der Festplatte
    private void LoadProfilePicture()
    {
        if (PlayerPrefs.HasKey("ProfilePicturePath"))
        {
            string savedPath = PlayerPrefs.GetString("ProfilePicturePath");

            if (File.Exists(savedPath))
            {
                byte[] imageData = File.ReadAllBytes(savedPath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageData);
                profileImage.texture = texture;

                Debug.Log("Profilbild erfolgreich geladen.");
            }
            else
            {
                Debug.LogWarning("Gespeichertes Profilbild wurde nicht gefunden.");
            }
        }
    }

    private void ApplyCircleMask(RawImage profileImage)
    {
        //Sorgt dafür, dass das Profilbild quadratisch wird
        RectTransform rectTransform = profileImage.GetComponent<RectTransform>();
        float minSize = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height);
        rectTransform.sizeDelta = new Vector2(minSize, minSize);
    }

    //private IEnumerator LoadImage(string path)
    //{
    //    var www = new WWW("file:///" + path);
    //    yield return www;
    //    Texture2D texture = www.texture;
    //    profileImage.texture = texture;

    //    //Größe normalisieren
    //    ApplyCircleMask(profileImage);

    //    //Speichere die Textur lokal und registriere den Pfad
    //    //SaveProfilePicture(texture);
    //    ProfileData.Instance.SetProfileImage(texture);
    //}

    private IEnumerator LoadImage(string path)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(path))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                profileImage.texture = texture;

                //Größe normalisieren
                ApplyCircleMask(profileImage);

                //Speichere die Textur lokal und registriere den Pfad
                //SaveProfilePicture(texture);
                ProfileData.Instance.SetProfileImage(texture);
            }
        }
    }
}

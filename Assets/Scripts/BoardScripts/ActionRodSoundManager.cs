using UnityEngine;

public class ActionRodSoundManager : MonoBehaviour
{
    public void PlaySound(SoundClipRef soundName)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySoundClip(soundName, SoundSourceRef.SFXSource, 0.3f);
        }
        else
        {
            Debug.LogWarning("AudioManager ist nicht vorhanden");
        }
    }
}

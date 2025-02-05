using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundSourceRef
{
    UISource,
    SFXSource
}

public enum SoundClipRef
{
    Button,
    Click,
    Slap,
    ChainRussle,
    Thud,
    LeverPull,
    MechanicalClick,
    MechanicalClickMulti,
    BrickSlide,
    RotatingWheelLoop,
    Chime,
    ChimeLow,
    WandPing,
    GlassSwipe,
    MetalImpact,
    MetalRattleSlow,
    MetalSlideClunk,
    BrickBuildUp,
    MetalSlideNoClunk,
    LevelUp,
    Win,
    Lose,
    LoopingRotatingWheelShort,
    Tie
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public SoundClipRef name;   //Name des Sounds
        public AudioClip clip;      //Der AudioClip
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource soundUIObject;
    [SerializeField] private AudioSource soundFXObject;

    [Header("Audio Mixer")]
    [SerializeField] private UnityEngine.Audio.AudioMixer audioMixer;     //Verknüpft den Unity Audio Mixer

    [Header("SoundClip-Zuweisung")]
    [SerializeField] private List<Sound> sounds = new List<Sound>();    //Liste mit allen Sounds
    private Dictionary<SoundClipRef, AudioClip> soundDictionary;

    private Dictionary<SoundClipRef, Coroutine> loopingSouds = new Dictionary<SoundClipRef, Coroutine>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //Sounds in Dictionary laden für schnelleren Zugriff
        soundDictionary = new Dictionary<SoundClipRef, AudioClip>();
        foreach (var sound in sounds)
        {
            soundDictionary[sound.name] = sound.clip;
        }
    }

    public void PlaySoundClip(SoundClipRef clipRef, SoundSourceRef soundSourceRef, float volume)
    {
        //Spawn gameObject
        AudioSource audioSource = null;

        if (soundSourceRef == SoundSourceRef.UISource)
        {
            audioSource = Instantiate(soundUIObject, Vector3.zero, Quaternion.identity);
        }
        else
        {
            audioSource = Instantiate(soundFXObject, Vector3.zero, Quaternion.identity);
        }

        //Assign AudioClip
        if (soundDictionary.TryGetValue(clipRef, out AudioClip clip))
        audioSource.clip = clip;

        //Assign Volume
        audioSource.volume = volume;

        //Play Sound
        audioSource.Play();

        //Get length of AudioClip
        float clipLength = audioSource.clip.length;

        //Destroy AudioClip, after it's done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    private float PlaySoundClipWithTimeReturn(SoundClipRef clipRef, SoundSourceRef soundSourceRef, float volume)
    {
        //Spawn gameObject
        AudioSource audioSource = null;

        if (soundSourceRef == SoundSourceRef.UISource)
        {
            audioSource = Instantiate(soundUIObject, Vector3.zero, Quaternion.identity);
        }
        else
        {
            audioSource = Instantiate(soundFXObject, Vector3.zero, Quaternion.identity);
        }

        //Assign AudioClip
        if (soundDictionary.TryGetValue(clipRef, out AudioClip clip))
            audioSource.clip = clip;

        //Assign Volume
        audioSource.volume = volume;

        //Play Sound
        audioSource.Play();

        //Get length of AudioClip
        float clipLength = audioSource.clip.length;

        //Destroy AudioClip, after it's done playing
        Destroy(audioSource.gameObject, clipLength);

        return clipLength;
    }





    public void SetVolume(string category, float volume)
    {
        audioMixer.SetFloat(category, Mathf.Log10(volume) * 20);    // Für dB-Skalierung
    }











    //public void PlayRandomSoundClip(AudioClip[] clips)
    //{
    //    //Assign random Index
    //    int rand = Random.Range(0, clips.Length);

    //    //Spawn gameObject
    //    AudioSource audioSource = Instantiate(soundFXObject, Vector3.zero, Quaternion.identity);

    //    //Assign AudioClip
    //    audioSource.clip = clips[rand];

    //    //Assign Volume
    //    audioSource.volume = 1f;

    //    //Play Sound
    //    audioSource.Play();

    //    //Get length of AudioClip
    //    float clipLength = audioSource.clip.length;

    //    //Destroy AudioClip, after it's done playing
    //    Destroy(audioSource, clipLength);
    //}
}

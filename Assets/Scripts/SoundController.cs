using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    [Header("Mixers")]
    [SerializeField] AudioMixer mixer;

    private string _masterId = "MasterVolume";
    private string _musicId = "MusicVolume";
    private string _sfxId = "SFXVolume";

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private float LinearToDecibel(float value)
    {
        if (value <= 0f)
            return -80f;

        return Mathf.Log10(value) * 20f;
    }

    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat(_masterId, LinearToDecibel(volume));
    }

    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat(_musicId, LinearToDecibel(volume));
    }

    public void ToggleSFX(bool on)
    {
        if (on)
            mixer.SetFloat(_sfxId, LinearToDecibel(1));
        else
            mixer.SetFloat(_sfxId, LinearToDecibel(0));
    }   
}

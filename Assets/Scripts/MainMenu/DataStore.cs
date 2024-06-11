using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DataStore : MonoBehaviour
{
    public static DataStore Instance;
    [SerializeField] DefaultOptions options;

    [Header("Mixers")]
    [SerializeField] AudioMixer sfxMixer;
    [SerializeField] AudioMixer musicMixer;
    public float DefaultFOV { get => options.fov; }
    public float DefaultSFX { get => options.sfx; }
    public float DefaultMusic { get => options.music; }
    public float DefaultSensitivity { get => options.sensitivity; }

    private float sfx = -10;
    public float SFX { get => !MuteSFX ? (sfx > -1 ? sfx : DefaultSFX) : 0; set => sfx = value; }
    private float music = -10;
    public float Music { get => !MuteMusic ? (music > -1 ? music : DefaultMusic) : 0; set => music = value; }
    private float sensitivity = -10;
    public float Sensitivity { get => sensitivity > -1 ? sensitivity : DefaultSensitivity; set => sensitivity = value; }
    private float fov = -10;
    public float FOV { get => fov > -1 ? fov : DefaultFOV; set => fov = value; }

    private bool muteSFX = false;
    public bool MuteSFX { get => muteSFX; set => muteSFX = value; }
    private bool muteMusic = false;
    public bool MuteMusic { get => muteMusic; set => muteMusic = value; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Music < 1)
        {
            musicMixer.SetFloat("musicVolume", -80);
        }
        else
        {
            musicMixer.SetFloat("musicVolume", (Music / 2) - 30);
        }

        if (SFX < 1)
        {
            sfxMixer.SetFloat("sfxVolume", -80);
        }
        else
        {
            sfxMixer.SetFloat("sfxVolume", (SFX / 2) - 30);
        }

        if (MuteSFX)
        {
            sfxMixer.SetFloat("sfxPitch", 0.01f);
        }
        else
        {
            sfxMixer.SetFloat("sfxPitch", Time.timeScale);
        }

        if (MuteMusic)
        {
            musicMixer.SetFloat("musicPitch", 0.01f);
        }
        else
        {
            musicMixer.SetFloat("musicPitch", Time.timeScale);
        }
    }
}

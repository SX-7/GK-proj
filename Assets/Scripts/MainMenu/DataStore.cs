using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStore : MonoBehaviour
{
    public static DataStore Instance;
    [SerializeField] DefaultOptions options;
    public float DefaultFOV { get => options.fov; }
    public float DefaultSFX { get => options.sfx; }
    public float DefaultMusic { get => options.music; }
    public float DefaultSensitivity { get => options.sensitivity; }

    private float sfx = -10;
    public float SFX { get => sfx > -1 ? sfx : DefaultSFX; set => sfx = value; }
    private float music = -10;
    public float Music { get => music > -1 ? music : DefaultMusic; set => music = value; }
    private float sensitivity = -10;
    public float Sensitivity { get => sensitivity > -1 ? sensitivity : DefaultSensitivity; set => sensitivity = value; }
    private float fov = -10;
    public float FOV { get => fov > -1 ? fov : DefaultFOV; set => fov = value; }

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
}

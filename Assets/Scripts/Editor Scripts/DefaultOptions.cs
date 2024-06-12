using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DefaultOptions", order = 1)]
public class DefaultOptions : ScriptableObject
{
    public int fov;
    public float sfx;
    public float music;
    public float sensitivity;
}
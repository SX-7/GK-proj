using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderWriter : MonoBehaviour
{
    Material mapTextMaterial;
    Renderer _renderer;
    [SerializeField] PlayerController playerController;
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        mapTextMaterial = _renderer.material;

        float[] stringArray = new float[] { 0f };
        mapTextMaterial.SetFloatArray("_String_Chars", stringArray);
        mapTextMaterial.SetInt("_StringCharacterCount", stringArray.Length);
    }

    private void Update()
    {
        var vel = playerController.GetComponent<Rigidbody>().velocity.magnitude;
        float[] stringArray = new float[] { (vel*5 % 100) };
        mapTextMaterial.SetFloatArray("_String_Chars", stringArray);
        mapTextMaterial.SetInt("_StringCharacterCount", stringArray.Length);
        mapTextMaterial.SetColor(
            "_Color",
            new Color(
                (2 + Mathf.Sin(vel / 10)) / 4,
                (0.5f),
                (2 + Mathf.Sin(vel / 20)) / 4
            )
        );
    }
}

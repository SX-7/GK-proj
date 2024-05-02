using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderWriter : MonoBehaviour
{
    Material mapTextMaterial;
    private float timer = 0f;
    Renderer _renderer;
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
        timer += Time.deltaTime;
        float[] stringArray = new float[] { (timer % 100) };
        mapTextMaterial.SetFloatArray("_String_Chars", stringArray);
        mapTextMaterial.SetInt("_StringCharacterCount", stringArray.Length);
        mapTextMaterial.SetColor(
            "_Color",
            new Color(
                (2+Mathf.Sin(timer))/4,
                (2+Mathf.Sin(timer+1))/4,
                (2+Mathf.Sin(timer+2))/4
            )
        );
    }
}

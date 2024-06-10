using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandShaderWriter : MonoBehaviour
{
    Material mapTextMaterial;
    Renderer _renderer;
    [SerializeField] PlayerController playerController;
    private float timer = 0f;
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        mapTextMaterial = _renderer.material;

        float[] stringArray = new float[] { 0f };
        mapTextMaterial.SetFloatArray("_String_Chars", stringArray);
        mapTextMaterial.SetInt("_StringCharacterCount", stringArray.Length);
    }

    private void OnEnable()
    {
        PlayerController.OnReceiveDamage += OnReceiveDamage;
    }
    private void OnDisable()
    {
        PlayerController.OnReceiveDamage -= OnReceiveDamage;
    }

    private void OnReceiveDamage(DamageInfo damage)
    {
        timer = playerController.IFrames;
    }

    private void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        if (timer < 0f)
        {
            timer = 0f;
        }
        var vel = playerController.GetComponent<Rigidbody>().velocity.magnitude;
        float[] stringArray = new float[] { (int)(timer * 100) };
        mapTextMaterial.SetFloatArray("_String_Chars", stringArray);
        mapTextMaterial.SetInt("_StringCharacterCount", stringArray.Length);
        mapTextMaterial.SetColor(
            "_Color",
            new Color(
                (2 + Mathf.Sin(vel / 10)) / 4,
                (timer > 0f ? 1f : 0.5f),
                (2 + Mathf.Sin(vel / 20)) / 4
            )
        );
    }
}

using UnityEngine;

public class RightHandShaderWriter : MonoBehaviour
{
    Material fancyMaterial;
    Renderer _renderer;
    [SerializeField] PlayerController playerController;
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        fancyMaterial = _renderer.material;
        fancyMaterial.SetFloat("_GradientEffectTimePoint", 0);
        fancyMaterial.SetFloat("_MaxGradientTimePointOffset", playerController.DashDuration);
    }

    void OnEnable()
    {
        PlayerController.OnDash += OnDash;
    }

    void OnDisable()
    {
        PlayerController.OnDash -= OnDash;
    }

    void OnDash(int count)
    {
        fancyMaterial.SetFloat("_GradientEffectTimePoint", Time.timeSinceLevelLoad + playerController.DashDuration);
    }
}

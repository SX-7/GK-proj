using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostprocessingManager : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] PostProcessVolume DamagePpv;
    //unity has for some reasons problems with multiple components, thus i'm splitting it onto a hand
    [SerializeField] PostProcessVolume DashAndSlomoPpv;
    private Vignette vig;
    [SerializeField] float onDamageRampupTime = 0.1f;
    [SerializeField] float onDamageRampdownTime = 0.9f;
    private ChromaticAberration chr;
    private Grain gra;
    private LensDistortion ldi;
    private ColorGrading col;
    void Start()
    {
        if (controller == null) { throw new UnassignedReferenceException("No player controller assigned to " + name); }
        if (DamagePpv == null) { throw new UnassignedReferenceException("No post process volume assigned to " + name); }
        if (DashAndSlomoPpv == null) { throw new UnassignedReferenceException("No post process volume assigned to " + name); }
        if (!DamagePpv.profile.TryGetSettings(out vig)) { throw new MissingComponentException("Failed to find " + vig.GetType()); };
        if (!DamagePpv.profile.TryGetSettings(out gra)) { throw new MissingComponentException("Failed to find " + gra.GetType()); }
        if (!DamagePpv.profile.TryGetSettings(out col)) { throw new MissingComponentException("Failed to find " + col.GetType()); }
        if (!DashAndSlomoPpv.profile.TryGetSettings(out chr)) { throw new MissingComponentException("Failed to find " + chr.GetType()); };
        if (!DashAndSlomoPpv.profile.TryGetSettings(out ldi)) { throw new MissingComponentException("Failed to find " + ldi.GetType()); };

        vig.enabled.value = false;
        chr.enabled.value = false;
        gra.enabled.value = false;
        ldi.enabled.value = false;

    }

    private void OnEnable()
    {
        PlayerController.OnReceiveDamage += OnReceiveDamage;
        PlayerController.OnDash += OnDash;
        PlayerController.OnSlowMotion += OnSlowMo;
        PlayerController.OnPause += OnPause;
    }
    private void OnDisable()
    {
        PlayerController.OnReceiveDamage -= OnReceiveDamage;
        PlayerController.OnDash -= OnDash;
        PlayerController.OnSlowMotion -= OnSlowMo;
        PlayerController.OnPause -= OnPause;
    }
    private void OnReceiveDamage(DamageInfo damage)
    {
        vig.enabled.value = true;
        gra.enabled.value = true;
        StartCoroutine(OnDamageEffect());
    }

    private void OnDash(int current_dashes)
    {
        ldi.enabled.value = true;
        StartCoroutine(OnDashEffect());
    }

    private void OnSlowMo(bool state)
    {
        chr.enabled.value = true;
        if (state)
        {
            chr.intensity.value = 0;
        }
        else
        {
            chr.intensity.value = 1;
        }
        StartCoroutine(SloMoEffect(state));
    }

    private void OnPause(bool pause)
    {
        col.mixerBlueOutBlueIn.overrideState = pause;
        col.mixerRedOutRedIn.overrideState = pause;
        col.mixerGreenOutGreenIn.overrideState = pause;
        col.saturation.overrideState = pause;
    }

    private IEnumerator SloMoEffect(bool state)
    {
        var start = 0.5f;
        var end = 0f;
        if (state)
        {
            end = start;
            start = 0f;
        }
        float timer = 0f;
        while (timer < 0.1f)
        {
            timer += Time.unscaledDeltaTime;
            chr.intensity.value = Mathf.Lerp(start, end, timer / 0.1f);
            yield return null;
        }
    }

    private IEnumerator OnDamageEffect()
    {
        float timer = 0f;
        while (timer < onDamageRampupTime)
        {
            timer += Time.deltaTime;
            vig.intensity.value = Mathf.Lerp(0.1f, 0.25f, timer / onDamageRampupTime);
            gra.intensity.value = Mathf.Lerp(0, 1, timer / onDamageRampupTime);
            yield return null;
        }
        while (timer < onDamageRampdownTime + onDamageRampupTime)
        {
            timer += Time.deltaTime;
            vig.intensity.value = Mathf.Lerp(0f, 0.25f, Mathf.Sqrt(1 - (timer - onDamageRampupTime) / onDamageRampdownTime));
            gra.intensity.value = Mathf.Lerp(0, 1, Mathf.Sqrt(1 - (timer - onDamageRampupTime) / onDamageRampdownTime));
            yield return null;
        }
        vig.intensity.value = 0;
    }

    private IEnumerator OnDashEffect()
    {
        float timer = 0f;
        while (timer < 0.05f)
        {
            timer += Time.deltaTime;
            ldi.intensity.value = Mathf.Lerp(0, -20, timer / 0.05f);
            yield return null;
        }
        while (timer < 0.1f)
        {
            timer += Time.deltaTime;
            ldi.intensity.value = Mathf.Lerp(-20, 0, (timer - 0.05f) / 0.05f);
            yield return null;
        }
        ldi.intensity.value = 0;
    }
}

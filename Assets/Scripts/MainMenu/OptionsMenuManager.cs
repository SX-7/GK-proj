using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    [Header("Vanity info")]
    [SerializeField] Camera cam;
    private List<RectTransform> menuElements;
    [SerializeField] float fadeSpeed = 10f;
    [Header("Slider info")]
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider fovSlider;
    [SerializeField] Slider sensitivitySlider;

    // Start is called before the first frame update
    void Start()
    {
        menuElements = GetComponentsInChildren<RectTransform>().Where((x) => x.GetComponent<OptionsMenuManager>() == null).ToList();
        musicSlider.value = DataStore.Instance.Music;
        fovSlider.value = DataStore.Instance.FOV;
        sensitivitySlider.value = DataStore.Instance.Sensitivity;
        sfxSlider.value = DataStore.Instance.SFX;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FadeInto(bool state)
    {
        if (state)
        {
            FadeIn();
        }
        else
        {
            FadeOut();

        }
    }

    void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInCR());
    }

    IEnumerator FadeInCR()
    {
        var timed = 0f;
        while (timed < 1f)
        {
            foreach (var item in menuElements)
            {
                item.localScale = Vector3.Lerp(item.localScale, new Vector3(1, 1, 1), Time.deltaTime * fadeSpeed);
            }
            timed += Time.deltaTime;
            yield return null;
        }
        foreach (var item in menuElements)
        {
            item.localScale = new Vector3(1, 1, 1);
        }
    }

    void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCR());
    }

    IEnumerator FadeOutCR()
    {
        var timed = 0f;
        while (timed < 1f)
        {
            foreach (var item in menuElements)
            {
                item.localScale = Vector3.Lerp(item.localScale, new Vector3(0, 0, 0), Time.deltaTime * fadeSpeed);
            }
            timed += Time.deltaTime;
            yield return null;
        }
        foreach (var item in menuElements)
        {
            item.localScale = new Vector3(0, 0, 0);
        }
    }

    private void ResetMusic()
    {
        musicSlider.value = DataStore.Instance.DefaultMusic;
    }

    private void ResetSFX()
    {
        sfxSlider.value = DataStore.Instance.DefaultSFX;
    }

    private void ResetFOV()
    {
        fovSlider.value = DataStore.Instance.DefaultFOV;
    }

    private void ResetSensitivity()
    {
        sensitivitySlider.value = DataStore.Instance.DefaultSensitivity;
    }

    private void ReadMusic()
    {
        DataStore.Instance.Music = musicSlider.value;
    }

    private void ReadSFX()
    {
        DataStore.Instance.SFX = sfxSlider.value;
    }

    private void ReadFOV()
    {
        DataStore.Instance.FOV = fovSlider.value;
    }

    private void ReadSensitivity()
    {
        DataStore.Instance.Sensitivity = sensitivitySlider.value;
    }
}

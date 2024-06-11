using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelectManager : MonoBehaviour
{
    private List<RectTransform> menuElements;
    private List<Transform> menuButtons;
    [SerializeField] Scrollbar scrl;
    [SerializeField] GameObject buttonPrefab;
    [Header("Enable if you're trying to access the old functionality")]
    [SerializeField] bool doScenesList;
    [Tooltip("Only put scenes here, otherwise it might behave unexpectedly")][SerializeField] string[] scenesList;
    [SerializeField] float fadeSpeed = 10f;
    private float scrollRange = 0;
    private float curScrollFac = 0f;
    // Start is called before the first frame update
    void Start()
    {
        var offset = 0;
        foreach (var scene in scenesList)
        {
            Instantiate(buttonPrefab, transform).GetComponent<SceneButton>().Init(scene, offset);
            offset -= 40;
        }
        scrollRange = -offset;
        scrollRange -= 40;
        if (scrollRange < 1)
        {
            scrollRange = 1;
        }
        menuElements = GetComponentsInChildren<RectTransform>().Where((x) => x.GetComponent<SceneSelectManager>() == null).ToList();
        menuButtons = GetComponentsInChildren<Transform>().Where((x) => x.GetComponent<SceneButton>() != null).ToList();
    }

    void Scroll()
    {
        foreach (var button in menuButtons)
        {
            button.GetComponent<RectTransform>().localPosition += new Vector3(0, (scrl.value - curScrollFac) * scrollRange, 0);
        }
        curScrollFac = scrl.value;
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
}

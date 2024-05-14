using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OptionsMenuManager : MonoBehaviour
{
    private List<RectTransform> menuElements;
    // Start is called before the first frame update
    void Start()
    {
        menuElements = GetComponentsInChildren<RectTransform>().Where((x) => x.GetComponent<OptionsMenuManager>() == null).ToList();
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
                item.localScale = Vector3.Lerp(item.localScale, new Vector3(1, 1, 1), 0.1f);
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
                item.localScale = Vector3.Lerp(item.localScale, new Vector3(0, 0, 0), 0.1f);
            }
            timed += Time.deltaTime;
            yield return null;
        }
        foreach (var item in menuElements)
        {
            item.localScale = new Vector3(0, 0, 0);
        }

    }

    private void Cheese()
    {
        Debug.Log("Cheese option");
    }
}

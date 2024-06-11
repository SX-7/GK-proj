using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] string scene;
    private string _scene;

    private void Awake()
    {
        if (scene != null) { _scene = scene; }
    }
    public void Init(string scene, float offset)
    {
        _scene = scene;
        GetComponentInChildren<TMP_Text>().text = Regex.Match(scene, @"[^/]*(?=\.[^/]*)").Value;
        transform.localPosition += new Vector3(0, offset, 0);
    }

    public void OnPointerClick(PointerEventData eventData) {
        StartCoroutine(LoadSceneCR(_scene));
    }
    IEnumerator LoadSceneCR(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene));
    }
}

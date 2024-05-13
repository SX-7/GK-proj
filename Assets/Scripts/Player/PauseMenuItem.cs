using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuItem : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] PauseMenuManager pauseMenuManager;
    public void Awake()
    {
        pauseMenuManager = FindAnyObjectByType<PauseMenuManager>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        pauseMenuManager.SendMessage("ItemSelected",transform.position);
    }
    
}

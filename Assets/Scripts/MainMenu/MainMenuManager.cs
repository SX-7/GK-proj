using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject optionsMenu;
    private bool isOptionsActive = false;
    [SerializeField] GameObject stageSelectMenu;
    private bool isStageSelectActive = false;
    private List<Button> mainMenuButtons;
    // Start is called before the first frame update
    void Start()
    {
        mainMenuButtons = GetComponentsInChildren<Button>().Where((x)=> x.GetComponent<MainMenuManager>() == null).ToList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetButtonState(bool state)
    {
        foreach (var button in mainMenuButtons)
        {
            button.interactable = state;
        }
    }

    private void SelectScene()
    {
        stageSelectMenu.SendMessage("FadeInto", !isStageSelectActive);
        isStageSelectActive = !isStageSelectActive;
        SetButtonState(!isStageSelectActive);
    }

    private void Options()
    {
        optionsMenu.SendMessage("FadeInto", !isOptionsActive);
        isOptionsActive = !isOptionsActive;
        SetButtonState(!isOptionsActive);
    }

    private void Exit()
    {
        Debug.Log("Exits the game");
        Application.Quit();
    }
}

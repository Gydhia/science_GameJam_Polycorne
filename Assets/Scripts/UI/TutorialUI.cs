using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public GameObject DialogPanel;
    public GameObject Image;

    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI MainText;

    public Button NextBtn;
    public Button CloseBtn;

    public List<string> Dialogs;
    public string Title;

    private int _dialogIndex = 0;

    private void Start()
    {
        if(Dialogs.Count > 0)
            ShowPopup();
    }

    public void ClosePopup()
    {
        DialogPanel.SetActive(false);
        Image.SetActive(false);
    }

    public void ShowPopup()
    {
        DialogPanel.SetActive(true);
        Image.SetActive(true);

        ShowNextDialog();
    }

    public void SetupPopup(string title, string text)
    {
        TitleText.text = title;
        MainText.text = text;
    }

    public void ShowNextDialog()
    {
        if(_dialogIndex > Dialogs.Count - 1) {
            ClosePopup();
        } else {
            SetupPopup(Title, Dialogs[_dialogIndex]);
            _dialogIndex++;
        }
    }

    public void RefreshTutorial()
    {
        _dialogIndex = 0;
        TitleText.text = Title;
        MainText.text = Dialogs[0];

        ShowPopup();
    }
}

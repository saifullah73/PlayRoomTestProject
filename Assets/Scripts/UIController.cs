using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public TMP_Text tmpText;
    public Button resetButton;
    public void ShowText(string text){
        tmpText.text = text;
    }

    public void Show(bool show){
        this.gameObject.SetActive(show);
    }

    public void ShowButton(bool show){
        this.resetButton.gameObject.SetActive(show);
    }
}

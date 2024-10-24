using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LimitMode : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _questButton;
    [SerializeField] private Button _backButton;

    [Header("UI Object")]
    [SerializeField] private GameObject _uiOption;
    [SerializeField] private GameObject _uiQuest;
    [SerializeField] private GameObject _uiTitle;

    void Start()
    {
        Init();

        _optionButton.onClick.AddListener(OnClickOptionButton);
        _questButton.onClick.AddListener(OnClickQuestButton);
        _backButton.onClick.AddListener(OnClickBackButton);
    }

    private void Init()
    {
        _uiQuest.SetActive(false);
    }

    #region Button Events
    public void OnClickOptionButton()
    {
        _uiOption.SetActive(true);
    }

    public void OnClickQuestButton()
    {
        if (_uiQuest.gameObject.activeSelf)
            _uiQuest.SetActive(false);
        else
            _uiQuest.SetActive(true);
    }

    public void OnClickBackButton()
    {
        this.gameObject.SetActive(false);

        _uiTitle.SetActive(true);
    }
    #endregion
}

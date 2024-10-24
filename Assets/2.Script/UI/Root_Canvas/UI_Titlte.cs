using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Titlte : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _nomalModeButton;
    [SerializeField] private Button _limitModeButton;

    [Header("UI GameObejct")]
    [SerializeField] private GameObject _nomalModeObj;
    [SerializeField] private GameObject _limitModeObj;

    private void Start()
    {
        _nomalModeButton.onClick.AddListener(OnClickNomalModeButton);
        _limitModeButton.onClick.AddListener(OnClickLimitModeButton);
    }

    #region Button Events
    public void OnClickNomalModeButton()
    {
        this.gameObject.SetActive(false); // UI_Title 비활성화
        _nomalModeObj.gameObject.SetActive(true);
    }

    public void OnClickLimitModeButton()
    {
        this.gameObject.SetActive(false); // UI_Title 비활성화
        _limitModeObj.gameObject.SetActive(true);

    }
    #endregion
}

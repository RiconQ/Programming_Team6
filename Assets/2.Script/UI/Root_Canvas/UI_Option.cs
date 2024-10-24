using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Option : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _closeButton;

    [Header("Sliders")]
    [SerializeField] private Slider _bgmSound;
    [SerializeField] private Slider _effectSound;

    // Start is called before the first frame update
    void Start()
    {
        _closeButton.onClick.AddListener(OnClickCloseButton);
    }

    #region
    public void OnClickCloseButton()
    {
        this.gameObject.SetActive(false);
    }
    #endregion
}

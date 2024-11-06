using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIButton _gameStopButton;
    [SerializeField] private UIButton _gameRestartButton;
    [SerializeField] private UIButton _leftMoveButton;
    [SerializeField] private UIButton _rightMoveButton;
    [SerializeField] private UIButton _dropButton;

    [SerializeField] private UIToggle _uiSwitchToggle;

    [SerializeField] private UILabel _highScoreLabel;
    [SerializeField] private UILabel _nowScoreLabel;

    [SerializeField] private UISprite _cloudSprite;

    private void Start()
    {
        GameManager.Instance.OnScoreChanged += OnUpdateUIScoreText;
        GameManager.Instance.OnScoreMaxChanged += OnUpdateUIScoreMaxScoreText;

        //_gameStopButton.onClick.Add(new EventDelegate(() => OnClickGameStopButton()));
        //_gameRestartButton.onClick.Add(new EventDelegate(() => OnClickGameStopButton()));
        //_dropButton.onClick.Add(new EventDelegate(() => OnClickDropButton()));
        //_uiSwitchToggle.onChange.Add(new EventDelegate(() => OnToggleChangedUiSwitch(_uiSwitchToggle.value)));
    }


    #region Button OnClick Event Method
    private void OnClickGameStopButton()
    {
        Time.timeScale = 0f;

        // ���� Popup UI ��� 
    }

    private void OnClickGameRestartButton()
    {
        // �̴� ���� �÷��� ������ �ʱ�ȭ
    }

    private void OnClickDropButton()
    {
        // ���� ��� ���
        GameManager.Instance.TouchDown();
        GameManager.Instance.TouchUp();
    }
    #endregion

    #region Toggle OnChanged Event Method
    private void OnToggleChangedUiSwitch(bool isActive)
    {
        if(isActive)
        {
            // ������ ��� (����Ű, ���� ��ư UI ������ ��ġ) - ��ġ�� ����
        }
        else
        {
            // �޼� ��� (����Ű, ���� ��ư UI ������ ��ġ) - ��ġ�� ����
        }
    }
    #endregion

    #region Update UI Method
    public void OnUpdateUIScoreText(int score)
    {
        _nowScoreLabel.text = score.ToString();
    }

    public void OnUpdateUIScoreMaxScoreText(int maxScore)
    {
        _highScoreLabel.text = maxScore.ToString();
    }
    #endregion
}

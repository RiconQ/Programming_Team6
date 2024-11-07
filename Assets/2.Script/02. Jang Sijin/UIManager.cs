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


    //====================================================================

    [SerializeField] private GameObject _switchButtons;
    [SerializeField] private GameObject _toggleButton;

    // private UI_Switch uI_Switch;

    private void Start()
    {
        GameManager.Instance.OnScoreChanged += OnUpdateUIScoreText;
        GameManager.Instance.OnScoreMaxChanged += OnUpdateUIScoreMaxScoreText;

        //_gameStopButton.onClick.Add(new EventDelegate(() => OnClickGameStopButton()));
        //_gameRestartButton.onClick.Add(new EventDelegate(() => OnClickGameStopButton()));
        //_dropButton.onClick.Add(new EventDelegate(() => OnClickDropButton()));
        _uiSwitchToggle.onChange.Add(new EventDelegate(() => OnToggleChangedUiSwitch(_uiSwitchToggle.value)));

        //  uI_Switch = FindObjectOfType<UI_Switch>();
    }


    #region Button OnClick Event Method
    private void OnClickGameStopButton()
    {
        Time.timeScale = 0f;

        // 정지 Popup UI 출력 
    }

    private void OnClickGameRestartButton()
    {
        // 미니 게임 플레이 데이터 초기화
    }

    private void OnClickDropButton()
    {
        // 과일 드롭 기능
        GameManager.Instance.TouchDown();
        GameManager.Instance.TouchUp();
    }
    #endregion

    #region Toggle OnChanged Event Method
    private void OnToggleChangedUiSwitch(bool isActive)
    {
        Vector3 currentPosition_s = _switchButtons.transform.position;
        Vector3 currentPosition_t = _toggleButton.transform.position;
        if (isActive)
        {
            // 왼손 모드 (방향키, 낙하 버튼 UI 좌측에 배치) - 위치만 변경
            currentPosition_s.x *= -1;
            _switchButtons.transform.position = currentPosition_s;

            currentPosition_t.x *= -1;
            _toggleButton.transform.position = currentPosition_t;

        }
        else
        {
            currentPosition_s.x *= -1;
            _switchButtons.transform.position = currentPosition_s;

            currentPosition_t.x *= -1;
            _toggleButton.transform.position = currentPosition_t;
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

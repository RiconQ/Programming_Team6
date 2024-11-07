using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("----------Button")]
    [SerializeField] private UIButton _gameStopButton;
    [SerializeField] private UIButton _gameRestartButton;
    [SerializeField] private UIButton _dropButton;
    [SerializeField] private UIButton _leftMoveButton;
    [SerializeField] private UIButton _rightMoveButton;

    [Header("----------SubButton")]
    [SerializeField] private UIButton _pauseResumeButton;
    [SerializeField] private UIButton _resetCancelButton;
    [SerializeField] private UIButton _resetNoButton;
    [SerializeField] private UIButton _resetYesButton;

    [Header("----------Panel")]
    [SerializeField] private UIPanel _retryPanel;
    [SerializeField] private UIPanel _pausePanel;

    [Header("----------Toggle")]
    [SerializeField] private UIToggle _uiSwitchToggle;

    [Header("----------Label")]
    [SerializeField] private UILabel _highScoreLabel;
    [SerializeField] private UILabel _nowScoreLabel;

    [Header("----------ETC")]
    [SerializeField] private UISprite _cloudSprite;
    [SerializeField] private GameObject _switchButtons;
    // private UI_Switch uI_Switch;



    private void Start()
    {
        GameManager.Instance.OnScoreChanged += OnUpdateUIScoreText;
        GameManager.Instance.OnScoreMaxChanged += OnUpdateUIScoreMaxScoreText;
        GameManager.Instance.OnWaitBallLvChanged += OnUpdateUIWaitBallSprite;

        // 버튼에 OnClick 이벤트 할당
        _gameStopButton.onClick.Add(new EventDelegate(() => OnClickGameStopButton()));
        _gameRestartButton.onClick.Add(new EventDelegate(() => OnClickGameRestartButton()));
        _dropButton.onClick.Add(new EventDelegate(() => OnClickDropButton()));
        // _leftMoveButton.onClick.Add(new EventDelegate(() => OnClickLeftMoveButton()));
        // _rightMoveButton.onClick.Add(new EventDelegate(() => OnClickRightMoveButton()));

        // 버튼 눌림 상태 추적을 위한 이벤트 리스너 등록
        UIEventListener.Get(_leftMoveButton.gameObject).onPress = OnPressLeftMoveButton;
        UIEventListener.Get(_rightMoveButton.gameObject).onPress = OnPressRightMoveButton;

        _pauseResumeButton.onClick.Add(new EventDelegate(() => OnClickPauseResumeButton()));
        _resetCancelButton.onClick.Add(new EventDelegate(() => OnClickResetCancelButton()));
        _resetNoButton.onClick.Add(new EventDelegate(() => OnClickResetCancelButton()));
        _resetYesButton.onClick.Add(new EventDelegate(() => OnClickResetAcceptButton()));

        // 토글
        _uiSwitchToggle.onChange.Add(new EventDelegate(() => OnToggleChangedUiSwitch(_uiSwitchToggle.value)));
        // uI_Switch = FindObjectOfType<UI_Switch>();

        // atlas 세팅 - 땜빵 조치
        _cloudSprite.atlas = GameManager.Instance.lastBall.fruitData.atlas;

        // 스코어 초기화
        _nowScoreLabel.text = GameManager.Instance.score.ToString();
        _highScoreLabel.text = GameManager.Instance.maxScore.ToString();
    }


    #region Button OnClick Event Method
    // 버튼 - 일시정지 눌렀을 때
    private void OnClickGameStopButton()
    {
        Time.timeScale = 0;
        _pausePanel.gameObject.SetActive(true);
    }

    // 버튼 - 재시작 눌렀을 때
    private void OnClickGameRestartButton()
    {
        // 일단 일시정지 효과도 부여
        Time.timeScale = 0;
        _retryPanel.gameObject.SetActive(true);
    }

    // 버튼 - 드랍
    private void OnClickDropButton()
    {
        GameManager.Instance.DropTheBall();
    }


    private float holdTime = 0.1f;         // 최초 클릭 후 반복 시작 시간 (초)
    private float repeatInterval = 0.1f;   // 반복 호출 간격 (초)
    private float lastPressedTime = 0f;    // 버튼이 눌린 시간 기록

    // 왼쪽 버튼 눌림 상태 추적 (버튼을 누르고 있으면 true)
    private bool isLeftRepeating = false;      // 반복 실행 여부 추적
    private bool isLeftButtonPressed = false;  // 버튼 눌린 상태 추적

    // 오른쪽 버튼 눌림 상태 추적 (버튼을 누르고 있으면 true)
    private bool isRightRepeating = false;      // 반복 실행 여부 추적
    private bool isRightButtonPressed = false;  // 버튼 눌린 상태 추적

    private void OnPressLeftMoveButton(GameObject go, bool isPressed)
    {
        if (isPressed)
        {
            if (!isLeftButtonPressed)
            {
                // 버튼이 처음 눌렸을 때
                isLeftButtonPressed = true;
                lastPressedTime = Time.time;  // 현재 시간 기록
                isLeftRepeating = false;          // 반복을 시작하지 않음 (FixedUpdate에서 제어)
            }
        }
        else
        {
            // 버튼에서 손을 뗐을 때
            isLeftButtonPressed = false;
            isLeftRepeating = false;  // 버튼을 떼면 반복을 멈춤
        }
    }
    private void OnPressRightMoveButton(GameObject go, bool isPressed)
    {
        if (isPressed)
        {
            if (!isRightButtonPressed)
            {
                // 버튼이 처음 눌렸을 때
                isRightButtonPressed = true;
                lastPressedTime = Time.time;  // 현재 시간 기록
                isRightRepeating = false;          // 반복을 시작하지 않음 (FixedUpdate에서 제어)
            }
        }
        else
        {
            // 버튼에서 손을 뗐을 때
            isRightButtonPressed = false;
            isRightRepeating = false;  // 버튼을 떼면 반복을 멈춤
        }
    }

    void FixedUpdate()
    {
        // 버튼이 눌리고, 최초 클릭 후 일정 시간이 지난 후 반복을 시작
        if (isLeftButtonPressed)
        {
            if (!isLeftRepeating && (Time.time - lastPressedTime) >= holdTime)
            {
                // holdTime이 지난 후부터 반복 시작
                isLeftRepeating = true;
            }

            if (isLeftRepeating)
            {
                // 버튼을 누르고 있을 때 반복 실행
                GameManager.Instance.MoveTheBall(-1);
                lastPressedTime = Time.time;  // 반복 간격을 맞추기 위해 시간 갱신
            }
        }
        else if (isRightButtonPressed)
        {
            if (!isRightRepeating && (Time.time - lastPressedTime) >= holdTime)
            {
                // holdTime이 지난 후부터 반복 시작
                isRightRepeating = true;
            }

            if (isRightRepeating)
            {
                // 버튼을 누르고 있을 때 반복 실행
                GameManager.Instance.MoveTheBall(1);
                lastPressedTime = Time.time;  // 반복 간격을 맞추기 위해 시간 갱신
            }
        }
    }


    #endregion



    #region SubButton OnClick Event Method
    // 세부 버튼 - 일시정지 해제
    private void OnClickPauseResumeButton()
    {
        Time.timeScale = 1;
        _pausePanel.gameObject.SetActive(false);
    }

    // 세부 버튼 - 재시작 취소
    private void OnClickResetCancelButton()
    {
        Time.timeScale = 1;
        _retryPanel.gameObject.SetActive(false);
    }

    // 세부 버튼 - 재시작 확인
    private void OnClickResetAcceptButton()
    {
        Time.timeScale = 1;
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion



    #region Toggle OnChanged Event Method
    private void OnToggleChangedUiSwitch(bool isActive)
    {

        Vector3 currentPosition_s = _switchButtons.transform.position;
        Vector3 currentPosition_t = _uiSwitchToggle.transform.position;
        currentPosition_s.x *= -1;
        _switchButtons.transform.position = currentPosition_s;

        currentPosition_t.x *= -1;
        _uiSwitchToggle.transform.position = currentPosition_t;
        if (isActive)
        {
            // 왼손모드

        }
        else
        {
            //오른손모드
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

    public void OnUpdateUIWaitBallSprite(int ballLv)
    {
        _cloudSprite.spriteName = GameManager.Instance.lastBall.fruitData.fruits[ballLv].attribute.imgName;
    }
    #endregion
}

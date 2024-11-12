using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("----------Button")]
    [SerializeField] private UIButton _gameStopButton;
    [SerializeField] private UIButton _gameRestartButton;
    [SerializeField] private UIButton _dropButton;
    [SerializeField] private UIButton _leftMoveButton;
    [SerializeField] private UIButton _rightMoveButton;
    [SerializeField] private UIButton _itemNeedleButton;

    [Header("----------SubButton")]
    [SerializeField] private UIButton _pauseResumeButton;
    [SerializeField] private UIButton _resetCancelButton;
    [SerializeField] private UIButton _resetNoButton;
    [SerializeField] private UIButton _resetYesButton;
    [SerializeField] private UIButton _gameoverResetButton;

    [Header("----------Panel")]
    [SerializeField] private UIPanel _retryPanel;
    [SerializeField] private UIPanel _pausePanel;
    [SerializeField] private UIPanel _gameoverPanel;

    [Header("----------Toggle")]
    [SerializeField] private UIToggle _uiSwitchToggle;

    [Header("----------Label")]
    [SerializeField] private UILabel _highScoreLabel;
    [SerializeField] private UILabel _nowScoreLabel;

    [Header("----------ETC")]
    [SerializeField] private UISprite _cloudSprite;
    public GameObject ItemEnvironmentBox;
    [SerializeField] private GameObject _activezone;
    [SerializeField] private GameObject _backGround;
    [SerializeField] private GameObject _switchButtons;


    // 게임 매니져 싱글톤 적용
    public static UIManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            // 싱글톤 용 인스턴스 생성
            Instance = this;
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        GameManager.Instance.OnScoreChanged += OnUpdateUIScoreText;
        GameManager.Instance.OnScoreMaxChanged += OnUpdateUIScoreMaxScoreText;
        GameManager.Instance.OnWaitBallLvChanged += OnUpdateUIWaitBallSprite;

        // 버튼에 OnClick 이벤트 할당
        _gameStopButton.onClick.Add(new EventDelegate(() => OnClickGameStopButton()));
        _gameRestartButton.onClick.Add(new EventDelegate(() => OnClickGameRestartButton()));
        _dropButton.onClick.Add(new EventDelegate(() => OnClickDropButton()));

        _itemNeedleButton.onClick.Add(new EventDelegate(() => OnClickItemButton()));

        // _leftMoveButton.onClick.Add(new EventDelegate(() => OnClickLeftMoveButton()));
        // _rightMoveButton.onClick.Add(new EventDelegate(() => OnClickRightMoveButton()));

        // 버튼 눌림 상태 추적을 위한 이벤트 리스너 등록
        UIEventListener.Get(_leftMoveButton.gameObject).onPress = OnPressLeftMoveButton;
        UIEventListener.Get(_rightMoveButton.gameObject).onPress = OnPressRightMoveButton;

        UIEventListener.Get(_activezone).onClick += OnClickInside;
        UIEventListener.Get(_backGround).onClick += OnClickOutside;

        _pauseResumeButton.onClick.Add(new EventDelegate(() => OnClickPauseResumeButton()));
        _resetCancelButton.onClick.Add(new EventDelegate(() => OnClickResetCancelButton()));
        _resetNoButton.onClick.Add(new EventDelegate(() => OnClickResetCancelButton()));
        _resetYesButton.onClick.Add(new EventDelegate(() => OnClickResetAcceptButton())); ;
        _gameoverResetButton.onClick.Add(new EventDelegate(() => OnClickGameoverResetButton())); ;

        // 토글
        _uiSwitchToggle.onChange.Add(new EventDelegate(() => OnToggleChangedUiSwitch(_uiSwitchToggle.value)));

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

    // 버튼 - 아이템
    private bool isActiveItemMode = false;

    private void OnClickItemButton()
    {
        ItemEnvironmentBox.SetActive(true);
        NeedleManager.instance.ReadyUseNeedle(()=>
        {
            ItemEnvironmentBox.SetActive(false);
        });
        isActiveItemMode = true;

    }


    private void OnClickInside(GameObject go)
    {
        Debug.Log("사각형 영역 내부에서 클릭되었습니다.");

    }

    private void OnClickOutside(GameObject go)
    {
        Debug.Log("사각형 영역 외부에서 클릭되었습니다.");
        // 사각형 밖에서의 클릭 시 처리 로직
        ItemEnvironmentBox.SetActive(false);
    
    }



    private bool isLeftButtonPressed = false;  // 버튼 눌린 상태 추적
    private bool isRightButtonPressed = false;

    private void OnPressLeftMoveButton(GameObject go, bool isPressed)
    {
        isLeftButtonPressed = isPressed;
    }
    private void OnPressRightMoveButton(GameObject go, bool isPressed)
    {
        isRightButtonPressed = isPressed;
    }

    void FixedUpdate()
    {
        // 버튼이 눌려있으면 지속적으로 구슬을 이동
        // 혹여나 2개 같이 누르고 있으면... 결과적으로 멈춰있도록
        if (isLeftButtonPressed)
        {
            GameManager.Instance.MoveTheBall(-1);
        }
        if (isRightButtonPressed)
        {
            GameManager.Instance.MoveTheBall(1);
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

    private void OnClickGameoverResetButton()
    {
        Time.timeScale = 1;
        DOTween.KillAll();
        _gameoverPanel.gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion



    #region Toggle OnChanged Event Method
    private void OnToggleChangedUiSwitch(bool isActive)
    {
        Vector3 currentPosition_s = _switchButtons.transform.position;
        Vector3 currentPosition_t = _uiSwitchToggle.transform.position;
        Vector3 currentPosition_i = _itemNeedleButton.transform.position;
        currentPosition_s.x *= -1;
        _switchButtons.transform.position = currentPosition_s;

        currentPosition_t.x *= -1;
        _uiSwitchToggle.transform.position = currentPosition_t;

        currentPosition_i.x *= -1;
        _itemNeedleButton.transform.position = currentPosition_i;
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

    public void OnUpdateUIItemCount(int count)
    {
        UILabel needleCount = _itemNeedleButton.GetComponentInChildren<UILabel>();
        // UI Count 업뎃하삼 
    }
    #endregion
}

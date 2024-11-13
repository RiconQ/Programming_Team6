using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public int score { get; private set; }
    public int maxScore { get; private set; }
    private bool isGameOver;

    [Header("Setting")]
    public int SpawnSpecies; // [조절 대상] 스폰되는 가짓 수. (1 = 최소 레벨 구슬만 등장)


    [Header("Object")]
    public Sprite[] BallSprites; // JSON 적용 이후 미사용 - 구슬 스프라이트
    public Ball lastBall; // 다음에 나올 공
    private int waitBallLv; // 예고된 공의 레벨
    private float leftBorder;
    private float rightBorder;

    [Header("Pooling")]
    public GameObject BallPrefab;
    public Transform BallGroup;
    public List<Ball> BallPool;
    public GameObject effectPrefab;
    public Transform effectGroup;
    [Range(1, 30)] public int poolSize;
    public int poolCursor;

    [Header("UI")]
    public Text scoreText;
    public Text maxScoreText;
    public GameObject GameOverUI;

    [Header("About Item")]
    [SerializeField] int userLevel;
    [SerializeField] int itemProbability;

    // [UI Changed Event]
    public event Action<int> OnScoreChanged;
    public event Action<int> OnScoreMaxChanged;
    public event Action<int> OnWaitBallLvChanged;

    // 게임 매니져 싱글톤 적용
    public static GameManager Instance;

    public ItemReward itemReward;

    public RewardTable rewardTable;
    public RewardInfo rewardInfo;





    private void Awake()
    {
        if (Instance == null)
        {
            // 싱글톤 용 인스턴스 생성
            Instance = this;

            // 프레임 조절
            Application.targetFrameRate = 60;

            // 스코어 관련
            if (!PlayerPrefs.HasKey("MaxScore")) PlayerPrefs.SetInt("MaxScore", 0);
            maxScore = PlayerPrefs.GetInt("MaxScore");
            // maxScoreText.text = maxScore.ToString();

            // 오브젝트 풀링 관련
            BallPool = new List<Ball>();
            for (int i = 0; i < poolSize; i++) MakeBall();
        }
        else Destroy(gameObject);
    }



    private void Start()
    {
        SoundManager.instance.PlayBGM();
        waitBallLv = UnityEngine.Random.Range(0, SpawnSpecies);
        NextBall();

        // 유저 레벨 판별
        rewardTable = SelectUserLevel();
    }

    // [오브젝트 풀링] 게임 시작 or 모든 풀링 사용 중 일때, 새로운 오브젝트 생성
    private Ball MakeBall()
    {
        GameObject instant = Instantiate(BallPrefab, BallGroup);
        instant.name = "Ball " + BallPool.Count;
        Ball instantBall = instant.GetComponent<Ball>();

        GameObject instantEffectOBJ = Instantiate(effectPrefab, effectGroup);
        ParticleSystem instantEffect = instantEffectOBJ.GetComponent<ParticleSystem>();
        instantBall.effect = instantEffect;

        BallPool.Add(instantBall);

        NeedleManager.instance.AddButton(instantBall.GetComponentInChildren<UIButton>());

        return instantBall;
    }

    // [오브젝트 풀링] 풀에서 오브젝트를 꺼내오는 메소드
    private Ball GetBall()
    {
        for (int i = 0; i < BallPool.Count; i++)
        {
            poolCursor = (poolCursor + 1) % BallPool.Count;
            // 커서가 비활성화된 걸 가리키고 있다면?
            if (!BallPool[poolCursor].gameObject.activeSelf)
            {
                // 그거를 풀에서 가져오자
                return BallPool[poolCursor];
            }
        }
        // 다 쓰고 있다면 하나 추가
        return MakeBall();
    }

    // 다음 구슬을 가져옴.
    private void NextBall()
    {
        if (isGameOver) return;
        Ball newBall = GetBall();
        lastBall = newBall;

        lastBall.level = waitBallLv;
        leftBorder = -2.75f + lastBall.transform.localScale.x;
        rightBorder = 2.75f - lastBall.transform.localScale.x;
        // 눈사람 방지 위해 x를 아주 미세하게 랜덤
        lastBall.transform.position += Vector3.right * UnityEngine.Random.Range(-0.01f, 0.01f);
        //lastBall.gameObject.GetComponent<SpriteRenderer>().sprite = BallSprites[lastBall.level];
        lastBall.gameObject.SetActive(true);
        waitBallLv = UnityEngine.Random.Range(0, SpawnSpecies);
        OnWaitBallLvChanged?.Invoke(waitBallLv);

        SoundManager.instance.PlaySFX("Next");
        StartCoroutine(NextBall_co());
    }

    // 기존 구슬이 드랍될 때까지 + 0.5초 대기 후 다음 구슬
    private IEnumerator NextBall_co()
    {
        yield return new WaitUntil(() => lastBall == null);
        yield return new WaitForSeconds(0.5f);
        NextBall();
    }

    // [레거시] 터치가 시작될 때 
    public void TouchDown()
    {
        if (lastBall == null) return;
        lastBall.Drag();
    }

    // [레거시] 터치를 땔 때
    public void TouchUp()
    {
        if (lastBall == null) return;
        lastBall.Drop();
        lastBall = null;
    }

    // 좌,우 버튼 누르면
    public void MoveTheBall(int direction)
    {
        if (lastBall == null) return;
        lastBall.transform.position += Vector3.right * direction * 0.1f;
        if (lastBall.transform.position.x > rightBorder)
        {
            lastBall.transform.position = new Vector3(rightBorder, lastBall.transform.position.y, 0);
        }
        else if (lastBall.transform.position.x < leftBorder)
        {
            lastBall.transform.position = new Vector3(leftBorder, lastBall.transform.position.y, 0);
        }
    }

    // 드랍 버튼 누르면
    public void DropTheBall()
    {
        if (lastBall == null) return;
        lastBall.Drop();
        lastBall = null;
    }

    // 점수 추가 메소드
    public void Addscore(int add)
    {
        if (isGameOver) return;
        score += add;
        //scoreText.text = score.ToString();
        OnScoreChanged?.Invoke(score);

        if (score > maxScore)
        {
            maxScore = score;
            //maxScoreText.text = maxScore.ToString();
            OnScoreMaxChanged?.Invoke(maxScore);
        }
    }

    // 게임 오버 메소드
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        StartCoroutine(GameOver_co());
    }

    private IEnumerator GameOver_co()
    {
        // 모든 공을 불러와 리스트에 저장
        Ball[] balls = FindObjectsOfType<Ball>();

        // 모든 공의 물리효과 제거 후
        foreach (Ball b in balls)
        {
            b.rigid.simulated = false;
        }

        // 0.05초 간격으로 사라짐
        foreach (Ball b in balls)
        {
            b.Hide(Vector3.up * 999);
            yield return new WaitForSeconds(0.05f);
        }

        // 최대 점수 기록
        maxScore = Mathf.Max(score, maxScore);
        PlayerPrefs.SetInt("MaxScore", maxScore);

        // BGM 정지 및 게임 오버 사운드
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySFX("GameOver");

        GameOverUI.SetActive(true);
    }


    //유저 레벨 확인 후, 그 값에 해당하는 테이블 값 가져오기
    public RewardTable SelectUserLevel()
    {
        for (int i = 0; i < itemReward.rewardDataTable.Count; i++)
        {
            RewardTable reward = itemReward.rewardDataTable[i];
            if (userLevel >= reward.Min_Lv && userLevel <= reward.Max_Lv)
            {
                return reward;
            }
        }
        return null;
    }

    public bool IsDropItem()
    {
        float itemPercent = UnityEngine.Random.Range(0f, 1f);
        return (itemPercent < 0.5f);
    }

    //리워드 인포에서 하나를 선택해 아이템 생성하기
    public RewardInfo ChooseItem()
    {
        //RewardInfo에서 랜덤값 하나 가져오기 -> 추후 확률 조정 필요
        int id = UnityEngine.Random.Range(0, itemReward.rewardInfos.Count);
        RewardInfo selectedReward = itemReward.rewardInfos[id];


        //RewardInfo에 있는 ItemKind를 가져오기    
        ItemInfo correspondingItem = itemReward.itemInfos.Find(item => item.ID == selectedReward.Kind);
        Debug.Log($"{correspondingItem.Item_Name}이 {selectedReward.Amount}개 생성");
        

        //
        return selectedReward;
    }

}



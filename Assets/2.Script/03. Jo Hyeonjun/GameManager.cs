using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public int score { get; private set; }
    public int maxScore { get; private set; }
    private bool isGameOver;
    public bool debugMode = false;

    [Header("----------Setting")]
    public int SpawnSpecies; // [조절 대상] 스폰되는 가짓 수. (1 = 최소 레벨 구슬만 등장)


    [Header("----------Object")]
    public Sprite[] BallSprites; // JSON 적용 이후 미사용 - 구슬 스프라이트
    public Ball lastBall; // 다음에 나올 공
    public float spawnDelaytime = 0.3f;
    private int waitBallLv; // 예고된 공의 레벨
    private float borderLeft, borderRight; // 드랍할 공의 좌,우 범위
    private float recentX = 0; // 가장 최근에 드랍한 X 좌표


    [Header("----------Pooling")]
    public FruitDataImporter fruitData;
    public GameObject BallPrefab;
    public Transform BallGroup;
    public List<Ball> BallPool;
    public GameObject effectPrefab;
    public Transform effectGroup;
    [Range(1, 30)] public int poolSize;
    public int poolCursor;

    [Header("----------UI")]
    public Text scoreText;
    public Text maxScoreText;
    public GameObject GameOverUI;

    [Header("----------Wall")]
    public Transform leftWall;
    public Transform rightWall;

    [Header("----------About Item")]
    [SerializeField] public int userLevel;


    // [UI Changed Event]
    public event Action<int> OnScoreChanged;
    public event Action<int> OnScoreMaxChanged;
    public event Action<int> OnWaitBallLvChanged;

    // 게임 매니져 싱글톤 적용
    public static GameManager Instance;

    public ItemReward itemReward;

    public RewardTable rewardTable;
    public ItemInfo itemInfo;





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
        waitBallLv = GetSpawnLevel();
        NextBall();

        // 스폰 구슬의 레벨 최댓값의 scale + 0.015f
        float borderFix = GetMaxSpawnLevelScale() + 0.015f;
        borderLeft = leftWall.position.x + leftWall.lossyScale.x / 2 + borderFix;
        borderRight = rightWall.position.x - rightWall.lossyScale.x / 2 - borderFix;

        //itemReward.Check();
        //Debug.Log("Check End");

        // 유저 레벨 판별


      
        //  Debug.Log($"리스트카운트: {rewardTable.reward.Count}");
        //     Debug.Log(itemReward.rewardDataTable.)
        //  Debug.Log(itemReward.rewardDataTable[0].reward.Count);


    }

    int frameCnt = 0;
    private void Update()
    {
        // 5 프레임마다 궤적 선 갱신
        frameCnt++;
        if (frameCnt >= 5)
        {
            frameCnt = 0;
            if (lastBall != null)
            {
                if (!lastBall.rigid.simulated) lastBall.DrawLine(true);
            }
        }
        if (debugMode) DropTheBall();
    }

    // [오브젝트 풀링] 게임 시작 or 모든 풀링 사용 중 일때, 새로운 오브젝트 생성
    private Ball MakeBall()
    {
        GameObject instant = Instantiate(BallPrefab, BallGroup);
        instant.name = "Ball " + BallPool.Count;
        Ball instantBall = instant.GetComponent<Ball>();

        //    GameObject instantEffectOBJ = Instantiate(effectPrefab, effectGroup);
        //    ParticleSystem instantEffect = instantEffectOBJ.GetComponent<ParticleSystem>();
        //    instantBall.effect = instantEffect;

        GameObject instantEffectOBJ = Instantiate(effectPrefab, effectGroup);
        NGUIAnimator ani=instantEffectOBJ.GetComponent<NGUIAnimator>();
        instantBall.animator = ani;

        BallPool.Add(instantBall);

        //NeedleManager.instance.AddButton(instantBall.GetComponentInChildren<UIButton>());

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
        MoveBallInBorder(recentX);

        //lastBall.gameObject.GetComponent<SpriteRenderer>().sprite = BallSprites[lastBall.level];
        lastBall.gameObject.SetActive(true);
        // 드랍하기 전의 공이므로 공의 rigid Off
        lastBall.rigid.simulated = false;
        lastBall.DrawLine(true);
        waitBallLv = GetSpawnLevel();
        OnWaitBallLvChanged?.Invoke(waitBallLv);

    //    SoundManager.instance.PlaySFX("Next");
        StartCoroutine(NextBall_co());
    }

    // 기존 구슬이 드랍되어 무언가 닿을 때까지 + 일정 시간 대기 후 다음 구슬
    private IEnumerator NextBall_co()
    {
        yield return new WaitUntil(() => lastBall == null);
        yield return new WaitForSeconds(spawnDelaytime);
        NextBall();
    }

    // 합성되어 새로운 구슬이 생성
    public void AppearNextLevel(Vector3 targetPos, int lv)
    {
        Ball newBall = GetBall();
        newBall.level = lv;
        // 좌표 설정
        newBall.transform.position = targetPos;
        // 합성된 구슬은 이미 드랍 상태
        newBall.isDropped = true;
        // 오브젝트 활성화
        newBall.gameObject.SetActive(true);
    }


    /*
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
    */
    // 좌,우 버튼 누르면
    public void MoveTheBall(int direction)
    {
        // 없거나 떨어지는 중이면 조작 불가능
        if (lastBall == null) return;
        if (lastBall.rigid.simulated) return;
        lastBall.DrawLine(false);
        lastBall.transform.position += Vector3.right * direction * 0.1f;
        MoveBallInBorder(lastBall.transform.position.x);
        lastBall.DrawLine(true);
    }

    // 드랍할 공의 범위 아웃 체크
    public void MoveBallInBorder(float posX)
    {
        posX = Mathf.Clamp(posX, borderLeft, borderRight) + UnityEngine.Random.Range(-0.01f, 0.01f);
        lastBall.transform.position = new Vector3(posX, lastBall.transform.position.y, 0);
    }

    // 드랍 버튼 누르면
    public void DropTheBall()
    {
        // 드랍할 공이 비어있거나 완전히 등장하지 않았다면 미적용
        if (lastBall == null) return;
        if (lastBall.scaleFrames > 0) return;
        recentX = lastBall.transform.position.x;
        lastBall.Drop();
        // 공이 무언가에 닿고 나서야 null이 되어 다음 드랍 가능하도록 변경
        // lastBall = null;
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
            b.Hide();
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

    // Excel에서 불러온 스폰 확률을 기반하여 다음 구슬의 레벨 설정
    private int GetSpawnLevel()
    {
        float randomfloat = UnityEngine.Random.Range(0.0f, 1.0f);
        for (int lv = 1; lv <= 11; lv++)
        {
            randomfloat -= GetSpawnProb(lv);
            if (randomfloat < 0) return lv;
        }
        Debug.Log("확률의 합이 1 미만인것 같습니다.");
        return 0;
    }

    // 구슬의 스폰 확률 가져오기
    private float GetSpawnProb(int lv)
    {
        return fruitData.fruits[lv - 1].attribute.spawnProb;
    }

    // 스폰 구슬의 최대 레벨의 크기 구하기 (드랍 x 범위 설정 용)
    private float GetMaxSpawnLevelScale()
    {
        for (int i = fruitData.fruits.Length - 1; i > 0; i--)
        {
            if (GetSpawnProb(i) > 0) return fruitData.fruits[i].attribute.scaleX;
        }
        return fruitData.fruits[0].attribute.scaleX;
    }

}







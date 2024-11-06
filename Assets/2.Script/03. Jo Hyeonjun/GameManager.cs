using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Core")]
    private int score;
    private int maxScore;
    private bool isGameOver;

    [Header("Setting")]
    public int SpawnSpecies; // [조절 대상] 스폰되는 가짓 수. (1 = 최소 레벨 구슬만 등장)


    [Header("Object")]
    public Sprite[] BallSprites; // [JSON 예정] 구슬 스프라이트
    public Ball lastBall; // 다음에 나올 공

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

    // [UI Changed Event]
    public event Action<int> OnScoreChanged;
    public event Action<int> OnScoreMaxChanged;

    // 게임 매니져 싱글톤 적용
    public static GameManager Instance;
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
            maxScoreText.text = maxScore.ToString();

            // 오브젝트 풀링 관련
            BallPool = new List<Ball>();
            for (int i = 0; i < poolSize; i++) MakeBall();
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        SoundManager.instance.PlayBGM();
        NextBall();
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

        lastBall.level = UnityEngine.Random.Range(0, SpawnSpecies);
        lastBall.gameObject.GetComponent<SpriteRenderer>().sprite = BallSprites[lastBall.level];
        lastBall.gameObject.SetActive(true);

        SoundManager.instance.PlaySFX("Next");
        StartCoroutine(NextBall_co());
    }

    // 기존 구슬이 드랍될 때까지 + 0.4초 대기 후 다음 구슬
    private IEnumerator NextBall_co()
    {
        yield return new WaitUntil(() => lastBall == null);
        yield return new WaitForSeconds(0.4f);
        NextBall();
    }

    // 터치가 시작될 때 
    public void TouchDown()
    {
        if (lastBall == null) return;
        lastBall.Drag();
    }

    // 터치를 땔 때
    public void TouchUp()
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
    }

    // [디버그 용] 리셋 버튼 - 지금은 누르면 확인 없이 바로 리셋
    public void PressButton(int n)
    {
        if (n == 1) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}



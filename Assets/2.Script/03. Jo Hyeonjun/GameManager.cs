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
    public int SpawnSpecies; // [���� ���] �����Ǵ� ���� ��. (1 = �ּ� ���� ������ ����)


    [Header("Object")]
    public Sprite[] BallSprites; // [JSON ����] ���� ��������Ʈ
    public Ball lastBall; // ������ ���� ��

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

    // ���� �Ŵ��� �̱��� ����
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            // �̱��� �� �ν��Ͻ� ����
            Instance = this;

            // ������ ����
            Application.targetFrameRate = 60;

            // ���ھ� ����
            if (!PlayerPrefs.HasKey("MaxScore")) PlayerPrefs.SetInt("MaxScore", 0);
            maxScore = PlayerPrefs.GetInt("MaxScore");
            maxScoreText.text = maxScore.ToString();

            // ������Ʈ Ǯ�� ����
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

    // [������Ʈ Ǯ��] ���� ���� or ��� Ǯ�� ��� �� �϶�, ���ο� ������Ʈ ����
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

    // [������Ʈ Ǯ��] Ǯ���� ������Ʈ�� �������� �޼ҵ�
    private Ball GetBall()
    {
        for (int i = 0; i < BallPool.Count; i++)
        {
            poolCursor = (poolCursor + 1) % BallPool.Count;
            // Ŀ���� ��Ȱ��ȭ�� �� ����Ű�� �ִٸ�?
            if (!BallPool[poolCursor].gameObject.activeSelf)
            {
                // �װŸ� Ǯ���� ��������
                return BallPool[poolCursor];
            }
        }
        // �� ���� �ִٸ� �ϳ� �߰�
        return MakeBall();
    }

    // ���� ������ ������.
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

    // ���� ������ ����� ������ + 0.4�� ��� �� ���� ����
    private IEnumerator NextBall_co()
    {
        yield return new WaitUntil(() => lastBall == null);
        yield return new WaitForSeconds(0.4f);
        NextBall();
    }

    // ��ġ�� ���۵� �� 
    public void TouchDown()
    {
        if (lastBall == null) return;
        lastBall.Drag();
    }

    // ��ġ�� �� ��
    public void TouchUp()
    {
        if (lastBall == null) return;
        lastBall.Drop();
        lastBall = null;
    }

    // ���� �߰� �޼ҵ�
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

    // ���� ���� �޼ҵ�
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        StartCoroutine(GameOver_co());
    }

    private IEnumerator GameOver_co()
    {
        // ��� ���� �ҷ��� ����Ʈ�� ����
        Ball[] balls = FindObjectsOfType<Ball>();

        // ��� ���� ����ȿ�� ���� ��
        foreach (Ball b in balls)
        {
            b.rigid.simulated = false;
        }

        // 0.05�� �������� �����
        foreach (Ball b in balls)
        {
            b.Hide(Vector3.up * 999);
            yield return new WaitForSeconds(0.05f);
        }

        // �ִ� ���� ���
        maxScore = Mathf.Max(score, maxScore);
        PlayerPrefs.SetInt("MaxScore", maxScore);

        // BGM ���� �� ���� ���� ����
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySFX("GameOver");
    }

    // [����� ��] ���� ��ư - ������ ������ Ȯ�� ���� �ٷ� ����
    public void PressButton(int n)
    {
        if (n == 1) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}



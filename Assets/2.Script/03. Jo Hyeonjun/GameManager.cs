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
    public int SpawnSpecies; // [���� ���] �����Ǵ� ���� ��. (1 = �ּ� ���� ������ ����)


    [Header("Object")]
    public Sprite[] BallSprites; // JSON ���� ���� �̻�� - ���� ��������Ʈ
    public Ball lastBall; // ������ ���� ��
    private int waitBallLv; // ����� ���� ����
    private float BorderX; // ����� ���� ��,�� ����
    private float recentX = 0; // ���� �ֱٿ� ����� X ��ǥ


    [Header("Pooling")]
    public FruitDataImporter fruitData;
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

    // ���� �Ŵ��� �̱��� ����
    public static GameManager Instance;

    public ItemReward itemReward;

    public RewardTable rewardTable;
    public RewardInfo rewardInfo;





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
            // maxScoreText.text = maxScore.ToString();

            // ������Ʈ Ǯ�� ����
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

        // ���� ���� �Ǻ�
        rewardTable = SelectUserLevel();
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

        //NeedleManager.instance.AddButton(instantBall.GetComponentInChildren<UIButton>());

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

        lastBall.level = waitBallLv;
        BorderX = 2.74f - lastBall.transform.localScale.x;
        MoveBallInBorder(recentX);

        //lastBall.gameObject.GetComponent<SpriteRenderer>().sprite = BallSprites[lastBall.level];
        lastBall.gameObject.SetActive(true);
        waitBallLv = GetSpawnLevel();
        OnWaitBallLvChanged?.Invoke(waitBallLv);

        SoundManager.instance.PlaySFX("Next");
        StartCoroutine(NextBall_co());
    }

    // �ռ��Ǿ� ���ο� ������ ����
    public void AppearNextLevel(Vector3 targetPos, int lv)
    {
        Ball newBall = GetBall();
        newBall.level = lv;
        // ��ǥ ����
        newBall.transform.position = targetPos;
        // ������Ʈ Ȱ��ȭ
        newBall.gameObject.SetActive(true);
        // ���� ���� ��� Ȱ��ȭ
        newBall.rigid.simulated = true;
        newBall.circle_col.enabled = true;
    }


    // ���� ������ ����� ������ + 0.5�� ��� �� ���� ����
    private IEnumerator NextBall_co()
    {
        yield return new WaitUntil(() => lastBall == null);
        yield return new WaitForSeconds(0.5f);
        NextBall();
    }
    /*
    // [���Ž�] ��ġ�� ���۵� �� 
    public void TouchDown()
    {
        if (lastBall == null) return;
        lastBall.Drag();
    }

    // [���Ž�] ��ġ�� �� ��
    public void TouchUp()
    {
        if (lastBall == null) return;
        lastBall.Drop();
        lastBall = null;
    }
    */
    // ��,�� ��ư ������
    public void MoveTheBall(int direction)
    {
        if (lastBall == null) return;
        lastBall.transform.position += Vector3.right * direction * 0.1f;
        MoveBallInBorder(lastBall.transform.position.x);
    }

    // ����� ���� ���� �ƿ� üũ
    public void MoveBallInBorder(float posX)
    {
        posX = Mathf.Clamp(posX, -BorderX, BorderX) + UnityEngine.Random.Range(-0.01f, 0.01f);
        lastBall.transform.position = new Vector3(posX, lastBall.transform.position.y, 0);
    }

    // ��� ��ư ������
    public void DropTheBall()
    {
        if (lastBall == null) return;
        recentX = lastBall.transform.position.x;
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

        GameOverUI.SetActive(true);
    }

    // Excel���� �ҷ��� ���� Ȯ���� ����Ͽ� ���� ������ ���� ����
    private int GetSpawnLevel()
    {
        float randomfloat = UnityEngine.Random.Range(0.0f, 1.0f);
        for (int lv = 0; lv < 10; lv++)
        {
            randomfloat -= fruitData.fruits[lv].attribute.spawnProb;
            if (randomfloat < 0) return lv;
        }
        Debug.Log("Ȯ���� ���� 1 �̸��ΰ� �����ϴ�.");
        return 0;
    }

    //���� ���� Ȯ�� ��, �� ���� �ش��ϴ� ���̺� �� ��������
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

    //������ �������� �ϳ��� ������ ������ �����ϱ�
    public RewardInfo ChooseItem()
    {
        //RewardInfo���� ������ �ϳ� �������� -> ���� Ȯ�� ���� �ʿ�
        int id = UnityEngine.Random.Range(0, itemReward.rewardInfos.Count);
        RewardInfo selectedReward = itemReward.rewardInfos[id];


        //RewardInfo�� �ִ� ItemKind�� ��������    
        ItemInfo correspondingItem = itemReward.itemInfos.Find(item => item.ID == selectedReward.Kind);
        Debug.Log($"{correspondingItem.Item_Name}�� {selectedReward.Amount}�� ����");
        

        //
        return selectedReward;
    }

}



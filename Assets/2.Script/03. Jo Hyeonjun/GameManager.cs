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
    public bool debugMode = false;

    [Header("----------Setting")]
    public int SpawnSpecies; // [���� ���] �����Ǵ� ���� ��. (1 = �ּ� ���� ������ ����)


    [Header("----------Object")]
    public Sprite[] BallSprites; // JSON ���� ���� �̻�� - ���� ��������Ʈ
    public Ball lastBall; // ������ ���� ��
    public float spawnDelaytime = 0.3f;
    private int waitBallLv; // ����� ���� ����
    private float borderLeft, borderRight; // ����� ���� ��,�� ����
    private float recentX = 0; // ���� �ֱٿ� ����� X ��ǥ


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
    [SerializeField] int userLevel;
    [SerializeField] int itemProbability;
    [SerializeField] GameObject itemOnlyData;
    [SerializeField] GameObject itemSprite;
    [SerializeField] List<GameObject> inventoryList;

    // [UI Changed Event]
    public event Action<int> OnScoreChanged;
    public event Action<int> OnScoreMaxChanged;
    public event Action<int> OnWaitBallLvChanged;

    // ���� �Ŵ��� �̱��� ����
    public static GameManager Instance;

    public ItemReward itemReward;

    public RewardTable rewardTable;
    public ItemInfo itemInfo;





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
        print(fruitData.fruits[0].attribute.spawnProb);
        SoundManager.instance.PlayBGM();
        waitBallLv = GetSpawnLevel();
        NextBall();

        // ���� ������ ���� �ִ��� scale + 0.015f
        float borderFix = GetMaxSpawnLevelScale() + 0.015f;
        borderLeft = leftWall.position.x + leftWall.lossyScale.x / 2 + borderFix;
        borderRight = rightWall.position.x - rightWall.lossyScale.x / 2 - borderFix;

        //itemReward.Check();
        //Debug.Log("Check End");

        // ���� ���� �Ǻ�
        rewardTable = SelectUserLevel();


        Debug.Log($"���� ����: {rewardTable.userLevel}");
        //  Debug.Log($"����Ʈī��Ʈ: {rewardTable.reward.Count}");
        //     Debug.Log(itemReward.rewardDataTable.)
        //  Debug.Log(itemReward.rewardDataTable[0].reward.Count);


    }

    int frameCnt = 0;
    private void Update()
    {
        // 5 �����Ӹ��� ���� �� ����
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
        MoveBallInBorder(recentX);

        //lastBall.gameObject.GetComponent<SpriteRenderer>().sprite = BallSprites[lastBall.level];
        lastBall.gameObject.SetActive(true);
        // ����ϱ� ���� ���̹Ƿ� ���� rigid Off
        lastBall.rigid.simulated = false;
        lastBall.DrawLine(true);
        waitBallLv = GetSpawnLevel();
        OnWaitBallLvChanged?.Invoke(waitBallLv);

        SoundManager.instance.PlaySFX("Next");
        StartCoroutine(NextBall_co());
    }

    // ���� ������ ����Ǿ� ���� ���� ������ + ���� �ð� ��� �� ���� ����
    private IEnumerator NextBall_co()
    {
        yield return new WaitUntil(() => lastBall == null);
        yield return new WaitForSeconds(spawnDelaytime);
        NextBall();
    }

    // �ռ��Ǿ� ���ο� ������ ����
    public void AppearNextLevel(Vector3 targetPos, int lv)
    {
        Ball newBall = GetBall();
        newBall.level = lv;
        // ��ǥ ����
        newBall.transform.position = targetPos;
        // �ռ��� ������ �̹� ��� ����
        newBall.isDropped = true;
        // ������Ʈ Ȱ��ȭ
        newBall.gameObject.SetActive(true);
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
        // ���ų� �������� ���̸� ���� �Ұ���
        if (lastBall == null) return;
        if (lastBall.rigid.simulated) return;
        lastBall.DrawLine(false);
        lastBall.transform.position += Vector3.right * direction * 0.1f;
        MoveBallInBorder(lastBall.transform.position.x);
        lastBall.DrawLine(true);
    }

    // ����� ���� ���� �ƿ� üũ
    public void MoveBallInBorder(float posX)
    {
        posX = Mathf.Clamp(posX, borderLeft, borderRight) + UnityEngine.Random.Range(-0.01f, 0.01f);
        lastBall.transform.position = new Vector3(posX, lastBall.transform.position.y, 0);
    }

    // ��� ��ư ������
    public void DropTheBall()
    {
        if (lastBall == null) return;
        recentX = lastBall.transform.position.x;
        lastBall.Drop();
        // ���� ���𰡿� ��� ������ null�� �Ǿ� ���� ��� �����ϵ��� ����
        // lastBall = null;
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
            b.Hide();
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
        for (int lv = 1; lv <= 11; lv++)
        {
            randomfloat -= GetSpawnProb(lv);
            if (randomfloat < 0) return lv;
        }
        Debug.Log("Ȯ���� ���� 1 �̸��ΰ� �����ϴ�.");
        return 0;
    }

    //���� ���� Ȯ�� ��, �� ���� �ش��ϴ� ���̺� �� ��������
    public RewardTable SelectUserLevel()
    {
        var tempLevelTable = new RewardTable();
        for (int i = 0; i < itemReward.rewardDataTable.Count; i++)
        {
            if (userLevel == itemReward.rewardDataTable[i].userLevel)
            {
                tempLevelTable.userLevel = itemReward.rewardDataTable[i].userLevel;
                //Debug.Log(tempLevelTable.userLevel);
                //Debug.Log(itemReward.rewardDataTable[i].reward.Count);
                //if (itemReward.rewardDataTable[i].reward is null)
                //{
                //    Debug.Log("test 2 is null");
                //}

                tempLevelTable.reward = itemReward.rewardDataTable[i].reward;
                return tempLevelTable;
                //if (tempLevelTable.reward is null)
                //{
                //    Debug.Log("Select User Level Null");
                //}
            }

        }
        return null;

        //RewardTable userLevelTable = itemReward.rewardDataTable[i];
        //if (userLevel == userLevelTable.userLevel)
        //{
        //    Debug.Log(userLevelTable.userLevel);
        //    if(userLevelTable.reward is null)
        //    {
        //        Debug.Log("Select User Level Null");
        //    }
        //    if (itemReward.rewardDataTable[i] is null)
        //    {
        //        Debug.Log("test 2 is null");
        //    }
        ////    Debug.Log(userLevelTable.reward[2]);

    }



    // ������ ���� Ȯ�� ��������
    private float GetSpawnProb(int lv)
    {
        return fruitData.fruits[lv].attribute.spawnProb;
    }

    // ���� ������ �ִ� ������ ũ�� ���ϱ� (��� x ���� ���� ��)
    private float GetMaxSpawnLevelScale()
    {
        for (int i = fruitData.fruits.Length - 1; i > 0; i--)
        {
            if (GetSpawnProb(i) > 0) return fruitData.fruits[i].attribute.scaleX;
        }
        return fruitData.fruits[1].attribute.scaleX;
    }

    // ������ ���� ������ ���̺��� ������ �������� ���� RewardInfo ��������
    public RewardInfo FindItemRewardInfo(int ballLv)
    {
        //ballLevel�� 1����, List�� 0����
        int ranNum = UnityEngine.Random.Range(0, rewardTable.reward[ballLv - 1].rewardInfos.Count);
        Debug.Log("������: " + ballLv);
        Debug.Log(ranNum + 1 + "��° ����Ʈ ����Ұ���");

        // ���� ������ �ش�Ǵ� ����Ʈ ��, randNum��° RewardInfo�� ���
        RewardInfo selectReward = rewardTable.reward[ballLv - 1].rewardInfos[ranNum];

        Debug.Log("Kind: " + selectReward.kind +
    "  Value: " + selectReward.value + "  Amount: " + selectReward.amount);
        return selectReward;
    }

   
    //RewardInfo�� Kind, Value ���� ���� ItemInfo ã��
public ItemInfo FindItemInfo(RewardInfo rewardInfo)
    {
        ItemInfo itemInfo = itemReward.itemInfos.Find(
       item => item.Item_Kind == rewardInfo.kind && item.Item_Value == rewardInfo.value);
        return itemInfo;
    }



    // �κ��丮 �ý��� Ȯ���� ���� �ӽ� ��ũ��Ʈ
    public void MakeItem(GameObject ball, RewardInfo rewardInfo, ItemInfo itemInfo)
    {
        for (int i = 0; i < rewardInfo.amount; i++)
        {
            GameObject newItem = Instantiate(itemOnlyData); //������ ������ ���� �� ������Ʈ ����
            newItem.name = itemInfo.Item_Name;
            Item_AtlasSetting itemData = newItem.GetComponent<Item_AtlasSetting>();
            itemData.Initialize(itemInfo);  // �� ������Ʈ�� ������ ���� ����


            Debug.Log("Name: " + itemInfo.Item_Name + "�� ������ " + (i + 1) + "��°");
            newItem.transform.SetParent(ball.transform, false); // ������ Transform �Ʒ��� ��ġ
            newItem.transform.localPosition = Vector3.zero; // �ڽ��� ��ġ�� �θ� �߽����� �ʱ�ȭ

        }
    }
    // ������ RewardInfo�� ItemInf
    public void AddItemToInventory(RewardInfo rewardInfo, ItemInfo itemInfo)
    {

 inventoryList.Add()
 

        //     GameObject newitemSprite = Instantiate(itemSprite);
        //     Item_AtlasSetting itemAtlas=newitemSprite.GetComponent<>
    }
}






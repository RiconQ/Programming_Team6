using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance = null;

    [Header("----------SpriteBox_MoveSettings")]
    [SerializeField] private Transform targetTransform; // ��ǥ ��ġ (Transform)
    private Vector3 targetPos; // ��ǥ ��ġ (Vector3)
    [SerializeField] private Transform originTransform; // ���� ��ġ (Transform)
    private Vector3 originPos; // ���� ��ġ (Vector3)

    [SerializeField] private float moveSpeed = 5f; // �̵� �ӵ�
    [SerializeField] private float delayBeforeHide = 1f; // HideBox ȣ�� �� ��� �ð�

    [Header("----------About Item")]
    [SerializeField] public List<GameObject> itemPool;
    [SerializeField] public Transform itemGroup;
    [SerializeField] private int itemPoolCount;
    [SerializeField] GameObject itemOnlyData;
    public ItemReward itemReward;
    [HideInInspector] public RewardTable rewardTable;
    [HideInInspector] public ItemInfo itemInfo;

    [Header("----------Item Duration")]
    [SerializeField] GameObject itemToGo;
    [SerializeField] GameObject sprite_Box;
    [SerializeField] Transform controlPoint;
    [SerializeField] Transform targetBox;

    [Header("----------Inventory")]
    [SerializeField] List<GameObject> inventoryList;



    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // ��ǥ ��ġ�� ���� ��ġ ����
        targetPos = targetTransform.position;
        originPos = originTransform.position;
        rewardTable = SelectUserLevel();
    }

    public void StartDelayBeforeHideBox_co()
    {
        StartCoroutine(DelayBeforeHideBox());

    }
    private IEnumerator DelayBeforeHideBox()
    {
        // ������ �ð���ŭ ���
        yield return new WaitForSeconds(delayBeforeHide);

        // HideBox ȣ��
        StartMoveBox(originPos);
    }

    public void StartMoveBox(Vector3 _targetPos)
    {
        StartCoroutine(MoveBox_co(_targetPos));
    }


    private IEnumerator MoveBox_co(Vector3 _targetPos)
    {
        while (Vector3.Distance(sprite_Box.transform.position, _targetPos) > 0.01f)
        {
            sprite_Box.transform.position = Vector3.MoveTowards(sprite_Box.transform.position, _targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        sprite_Box.transform.position = _targetPos;
    }


    // ������ item Object�� ball�� Setting
    public void SetItemInBall(GameObject ball, List<GameObject> itemList)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].transform.SetParent(ball.transform, false);
            itemList[i].transform.localPosition = Vector2.zero;
        }
    }


    // ȹ���� �������� �κ��丮 ����Ʈ�� �߰�
    public void AddItemsToInventory(List<GameObject> items)
    {
        foreach (var item in items)
        {
            // �������� �κ��丮�� �߰�
            inventoryList.Add(item);

            Vector3 target = targetBox.transform.position;
            StartMoveBox(target);
            var particle = item.GetComponentInChildren<ParticleSystem>(true);
            particle.gameObject.SetActive(true); 
            // �������� ȭ�鿡�� ����
            Extension.MoveCurve(item, itemToGo, controlPoint, 1.0f);
            Debug.Log($"������: {item.name}�� �κ��丮�� �̵�");
        }
    }



    //���� ���� Ȯ�� ��, �� ���� �ش��ϴ� ���̺� �� ��������
    public RewardTable SelectUserLevel()
    {
        var tempLevelTable = new RewardTable();
        Debug.Log(GameManager.Instance.userLevel);
        for (int i = 0; i < itemReward.rewardDataTable.Count; i++)
        {
            if (GameManager.Instance.userLevel == itemReward.rewardDataTable[i].userLevel)
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

    // Reward
    public RewardInfo FindItemRewardInfo(int ballLv)
    {
        //ballLevel�� 1����, List�� 0����
        int ranNum = UnityEngine.Random.Range(0, rewardTable.reward[ballLv - 1].rewardInfos.Count);
        if (rewardTable == null || rewardTable.reward == null || rewardTable.reward.Count < ballLv)
        {
            Debug.LogError($"rewardTable.reward�� ��ȿ���� �ʽ��ϴ�. ballLv: {ballLv}, reward.Count: {rewardTable.reward?.Count}");
            return null;
        }
        //   Debug.Log("������: " + ballLv);
        //    Debug.Log(ranNum + 1 + "��° ����Ʈ ����Ұ���");

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

    public List<GameObject> MakeItemStart()
    {
        for (int i = 0; i < itemPoolCount; i++)
        {
            GameObject newItem = null;
            newItem = Instantiate(itemOnlyData);
            itemPool.Add(newItem);
        }
        return itemPool;
    }

    // �κ��丮 Ȯ���� ���� �ӽ� ��ũ��Ʈ
    public List<GameObject> MakeItem(GameObject ball, RewardInfo rewardInfo, ItemInfo itemInfo)
    {
        // amount��ŭ ������ ���� �ӽ� ����Ʈ ����
        List<GameObject> itemList = new List<GameObject>();

        for (int i = 0; i < rewardInfo.amount; i++)
        {
            GameObject newItem = null;

            // Ǯ���� ��Ȱ��ȭ�� ������Ʈ �˻�
            foreach (GameObject item in itemPool)
            {
                if (!item.activeInHierarchy && item.name == itemInfo.Item_Name) // ��Ȱ��ȭ + �̸� ��
                {
                    newItem = item; // ���� ������Ʈ ����
                    newItem.SetActive(true);
                    break;
                }
            }

            // Ǯ�� ��� ������ ������Ʈ�� ������ ���� ����
            if (newItem == null)
            {
                newItem = Instantiate(itemOnlyData); // �� ������Ʈ ����
                newItem.name = itemInfo.Item_Name;  // �̸� ����
                itemPool.Add(newItem); // Ǯ�� �߰�
                Debug.Log($"���ο� ������Ʈ ����: {newItem.name}");
            }

            // �ʱ�ȭ (���� �Ǵ� ���� ������ ��� ���)
            Item_Data data = newItem.GetComponent<Item_Data>();
            if (data != null)
            {
                data.Initialize(itemInfo); // ������ �ʱ�ȭ
            }

            // ����Ʈ�� �߰�
            itemList.Add(newItem);
        }

        return itemList;
    }
}


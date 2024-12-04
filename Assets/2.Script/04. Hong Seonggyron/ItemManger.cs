using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance = null;

    [Header("----------SpriteBox_MoveSettings")]
    [SerializeField] private Transform targetTransform; // 목표 위치 (Transform)
    private Vector3 targetPos; // 목표 위치 (Vector3)
    [SerializeField] private Transform originTransform; // 원래 위치 (Transform)
    private Vector3 originPos; // 원래 위치 (Vector3)

    [SerializeField] private float moveSpeed = 5f; // 이동 속도
    [SerializeField] private float delayBeforeHide = 1f; // HideBox 호출 전 대기 시간

    [Header("----------About Item")]
    [SerializeField] public List<GameObject> itemPool;
    [SerializeField] public Transform itemGroup;
    [SerializeField] private int itemPoolCount;
    [SerializeField] GameObject itemOnlyData;
    public ItemReward itemReward;
    [HideInInspector] public RewardTable rewardTable;
    [HideInInspector] public ItemInfo itemInfo;
    [HideInInspector] public int itemCount=0;  // 생성된 아이템 갯수 -> 박스 미리 들어감 방지

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
        // 목표 위치와 원래 위치 설정
        targetPos = targetTransform.position;
        originPos = originTransform.position;
        rewardTable = SelectUserLevel();
    }

    public void StartDelayBeforeHideBox()
    {
        StartCoroutine(DelayBeforeHideBox_co());

    }
    private IEnumerator DelayBeforeHideBox_co()
    {
        // 초기 대기 시간
        float currentDelay = delayBeforeHide;

        while (true)
        {
            if (itemCount <= 0)
            {
                // 설정된 대기 시간만큼 대기
                yield return new WaitForSeconds(currentDelay);

                // 대기 중 itemCount가 증가하면 대기를 연장
                if (itemCount > 0)
                {
                    Debug.Log("itemCount 증가");
                    currentDelay += 0.3f; 
                }
                else
                {
                    // itemCount가 여전히 0 이하이면 박스를 숨김
                    Debug.Log("박스 숨기기 실행");
                    StartMoveBox(originPos);
                    yield break; // 코루틴 종료
                }
            }
            else
            {
                // itemCount가 0보다 크면 루프를 벗어나지 않고 계속 확인
                Debug.Log("아이템 카운트가 여전히 남아 있음");
                yield return null;
            }
        }
    }


    public void StartMoveBox(Vector3 _target)
    {
        StartCoroutine(MoveBox_co(_target));
    }


    private IEnumerator MoveBox_co(Vector3 _target)
    {
        while (Vector3.Distance(sprite_Box.transform.position, _target) > 0.01f)
        {
            sprite_Box.transform.position = Vector3.MoveTowards(sprite_Box.transform.position, _target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        sprite_Box.transform.position = _target;
    }


    // 생성된 item Object를 ball에 Setting
    public void SetItemInBall(GameObject ball, List<GameObject> itemList)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].transform.SetParent(ball.transform, false);
            itemList[i].transform.localPosition = Vector2.zero;
        }
    }


    // 획득한 아이템을 인벤토리 리스트에 추가
    public void AddItemsToInventory(List<GameObject> items)
    {
        foreach (var item in items)
        {
            // 아이템을 인벤토리에 추가
            inventoryList.Add(item);

            StartMoveBox_a();
                        var particle = item.GetComponentInChildren<ParticleSystem>(true);
            particle.gameObject.SetActive(true);
            particle.gameObject.transform.localPosition = Vector3.zero;
            // 아이템을 화면에서 제거
            itemCount += 1;
            Extension.MoveCurve(item, itemToGo, controlPoint, 1.0f);
            Debug.Log($"아이템: {item.name}이 인벤토리로 이동");
        }
    }

    public void StartMoveBox_a()
    {
        StartMoveBox(targetPos);
    }



    //유저 레벨 확인 후, 그 값에 해당하는 테이블 값 가져오기
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
        //ballLevel은 1부터, List는 0부터
        int ranNum = UnityEngine.Random.Range(0, rewardTable.reward[ballLv - 1].rewardInfos.Count);
        if (rewardTable == null || rewardTable.reward == null || rewardTable.reward.Count < ballLv)
        {
            Debug.LogError($"rewardTable.reward가 유효하지 않습니다. ballLv: {ballLv}, reward.Count: {rewardTable.reward?.Count}");
            return null;
        }
        //   Debug.Log("볼레벨: " + ballLv);
        //    Debug.Log(ranNum + 1 + "번째 리스트 사용할거임");

        // 과일 레벨에 해당되는 리스트 중, randNum번째 RewardInfo를 사용
        RewardInfo selectReward = rewardTable.reward[ballLv - 1].rewardInfos[ranNum];

        Debug.Log("Kind: " + selectReward.kind +
    "  Value: " + selectReward.value + "  Amount: " + selectReward.amount);
        return selectReward;
    }


    //RewardInfo의 Kind, Value 값을 통해 ItemInfo 찾기
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

    // 인벤토리 확인을 위한 임시 스크립트
    public List<GameObject> MakeItem(GameObject ball, RewardInfo rewardInfo, ItemInfo itemInfo)
    {
        // amount만큼 보상을 담을 임시 리스트 생성
        List<GameObject> itemList = new List<GameObject>();

        for (int i = 0; i < rewardInfo.amount; i++)
        {
            GameObject newItem = null;

            // 풀에서 비활성화된 오브젝트 검색
            foreach (GameObject item in itemPool)
            {
                if (!item.activeInHierarchy && item.name == itemInfo.Item_Name) // 비활성화 + 이름 비교
                {
                    newItem = item; // 기존 오브젝트 재사용
                    newItem.SetActive(true);
                    break;
                }
            }

            // 풀에 사용 가능한 오브젝트가 없으면 새로 생성
            if (newItem == null)
            {
                newItem = Instantiate(itemOnlyData); // 새 오브젝트 생성
                newItem.name = itemInfo.Item_Name;  // 이름 설정
                itemPool.Add(newItem); // 풀에 추가
                Debug.Log($"새로운 오브젝트 생성: {newItem.name}");
            }

            // 초기화 (재사용 또는 새로 생성된 경우 모두)
            Item_Data data = newItem.GetComponent<Item_Data>();
            if (data != null)
            {
                data.Initialize(itemInfo); // 데이터 초기화
            }

            // 리스트에 추가
            itemList.Add(newItem);
        }

        return itemList;
    }
}


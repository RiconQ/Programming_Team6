using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
// using DG.Tweening;

public class Ball : MonoBehaviour
{
    // ������ ����
    [Header("State")]
    public int level;
    // private bool isDrag; // [Legacy] �巡�� ���ΰ�?
    public bool isDropped; // 드랍된 상태인가?
    private bool isMerge; // �������� ���ΰ�?
    public bool isBouns; // ������ ���Ե� �����ΰ�?
    public float scaleFrames; // scale 조절 자체 tween용. 0보다 크면, 크기 변동 중임을 의미

    // ���� ������Ʈ
    [Header("Component")]
   // public ParticleSystem effect;
    public NGUIAnimator animator;
    public Rigidbody2D rigid;
    public CircleCollider2D circle_col;
    [SerializeField] private UITexture texture;

    // [JSON ����] ���� ���� ����
    [Header("Info")]
    // public int maxLevel;
    public FruitDataImporter fruitData;

    [Range(0, 100)]
    public int lv3ItemChance; // ���� 3(4��°) - ������ ���Ե� ������ Ȯ��

    // ���� ���� ����
    [Header("About Time")]
    public float warningTime = 0.50f;
    public float failTime = 0.75f;
    public float hideTime = 0.30f;
    public float spawnAppearTime = 0.50f; // 스폰된 구슬이 커지는 시간
    public float mixAppearTime = 0.20f; // 합성되어 생긴 구슬이 커지는 시간
    public float physicOnTime = 0.40f; // 합성되어 생겨난 구슬의 물리가 켜지는 딜레이 시간
    public float physicOffTime = 0.10f; // 재료 구슬의 물리가 꺼지는 딜레이 시간

    // 도움 선 관련
    [Header("Support Line")]
    private LineRenderer lineRenderer;  // LineRenderer 컴포넌트

    [Header("Item Info")]
    private bool hasItem; // 아이템이 생성되었는지 여부
    private RewardInfo rewardInfo; // 생성된 보상리스트의 정보
    private ItemInfo itemInfo;  // 생성된 아이템 리스트의 정보


    private List<GameObject> generatedItems = new List<GameObject>(); // 생성된 아이템 목록
                           
    // Collider 조절 관련
    private int[] col_offsetY = { -2, -6, 0, -1, 0, -2, 0, -6, -4, -7, -2 };
    private int[] col_radius = { 46, 42, 50, 48, 49, 48, 47, 45, 47, 43, 47 };

    private void Awake()
    {
        // ���� ������Ʈ �ҷ�����
        rigid = GetComponent<Rigidbody2D>();
        circle_col = GetComponent<CircleCollider2D>();
        texture = GetComponent<UITexture>();

        lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        var ballScale = GetBallScale(level);
        transform.localRotation = Quaternion.Euler(0, 0, 30);

        // 레벨에 따른 Collider 조정
        int offsetX = (level == 8) ? 3 : 0;
        circle_col.offset = new Vector2(offsetX, col_offsetY[level - 1]);
        circle_col.radius = col_radius[level - 1];

        // 레벨 10(멜론) 에서 자연스러운 Collider를 위해 부득이 scaleX 감소
        if (level == 10)
        {
            transform.localScale = new Vector3(
                transform.localScale.x - 0.2f,
                transform.localScale.y,
                transform.localScale.z);
        }

        // 합성되어 생긴 구슬
        if (isDropped)
        {
            // 크기는 정해진 시간동안 커짐, 물리는 정해진 시간 후 적용
            StartCoroutine(AnimateScale(Vector3.zero, ballScale, mixAppearTime, "Mix"));
        }

        // 스폰된 구슬
        else
        {
            // 크기는 정해진 시간동안 커짐 (물리는 드랍된 이후 적용)
            StartCoroutine(AnimateScale(Vector3.zero, ballScale, spawnAppearTime, "Spawn"));
        }

        // 아이템 생성 여부
        float rr = UnityEngine.Random.Range(0.0f, 1.0f);
        // 합성으로 만든 구슬(isDropped)이 아니면 아이템 생성 안되게
        if (rr < fruitData.fruits[level - 1].attribute.itemProb && isDropped)
        {
            isBouns = true;
          //  sprite.color = Color.green;

            hasItem = true;

            rewardInfo = ItemManager.instance.FindItemRewardInfo(level);
            itemInfo = ItemManager.instance.FindItemInfo(rewardInfo);


            generatedItems = ItemManager.instance.MakeItem(this.gameObject, rewardInfo, itemInfo);

            ItemManager.instance.SetItemInBall(this.gameObject, generatedItems);
        }
        else hasItem = false;

        // this.GetComponent<CircleCollider2D>().radius = 47;
        rigid.mass = GetBallMass(level);
        // sprite.atlas = fruitData.atlas; [장시진 - 주석]

        texture.mainTexture = GetBallTexture(level);


        //   Debug.Log($"Last Ball - Level : {level}, Sprite : {sprite.spriteName}");
        // ��� ���� ���� ����
        // �ð� ����� �ε��� ���⸸ �ϵ� �ڵ��Ͽ����ϴ�... (���� �ذ� ����)
        // BorderLeft = -2.75f + transform.localScale.x;
        // BorderRight = -BorderLeft;
    }
    /*
     
    // Scale Tween 용 Method 
    private void AnimateScale(Vector3 startScale, Vector3 endScale, float duration, string type)
    {
        // TweenScale 컴포넌트 추가
        TweenScale tween = GetComponent<TweenScale>();
        tween.from = startScale; // 현재 크기
        tween.to = endScale; // 최종 크기
        tween.duration = duration; // 애니메이션 지속 시간

        
        // 트윈 종료 시 실행할 함수 설정
        EventDelegate.Set(tween.onFinished, () =>
        {
            if (type == "Mix") PhysicOn();
            else if (type == "Hide") gameObject.SetActive(false);
        });

        // 트윈 실행
        tween.PlayForward();
    }
    */

    // 등장,소멸 시 크기 조절 효과 (기존의 DOTween 대체, NGUI Tween는 버그가 많아서 사용불가)
    private IEnumerator AnimateScale(Vector3 startScale, Vector3 endScale, float duration, string type)
    {
        // 지속시간 0이면, 즉시 크기 조정
        if (duration == 0)
        {
            transform.localScale = endScale;
        }
        else
        {
            // 지속시간을 프레임으로 변경
            scaleFrames = Application.targetFrameRate * duration;
            float maxFrames = scaleFrames;

            // 남은 지속시간(프레임)에 따른 Scale 조정
            // 결과적으로 남은 프레임이 0이 되면 endScale에 도달
            while (scaleFrames > 0)
            {
                scaleFrames--;
                transform.localScale = Vector3.Lerp(endScale, startScale, scaleFrames / maxFrames);
                yield return null;
            }
        }
        // 크기 조절 종료 시, 타입에 따라 후속 동작 실행(Callback 대체)
        if (type == "Mix") StartCoroutine(PhysicON_co());
        if (type == "Hide") gameObject.SetActive(false);
    }
    

    // 생겨난 구슬의 물리 On 지연 시간
    IEnumerator PhysicON_co()
    {
        yield return new WaitForSeconds(physicOnTime);
        PhysicChange(true);
    }

    private void OnDisable()
    {
        // ���� ��Ȱ��ȭ �� ��
        // �Ӽ� �ʱ�ȭ
        // isDrag = false;
        isDropped = false;
        level = 1;
        isMerge = false;
        isBouns = false;
        // transform �ʱ�ȭ
        transform.localPosition = Vector3.zero;
        // transform.localScale = Vector3.zero;
        // ���� �ʱ�ȭ
        // PhysicChange(false);

    }



    public void Drag()
    {
        // isDrag = true;
    }
    public void Drop()
    {
        // isDrag = false;
        // isDropped = true;
        DrawLine(false);
        PhysicChange(true);
    }

    // ������ �ٸ� �ݶ��̴��� �������� ��
    private bool isAttach;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 만약 드랍한 구슬이었다면
        if (!isDropped)
        {
            isDropped = true;
            GameManager.Instance.lastBall = null;
        }

        if (collision.gameObject.tag == "Ball")
        {
            Ball other = collision.gameObject.GetComponent<Ball>();

            // �ռ� ���� ���� �� (�ϴ� �ִ� ������ ������� ����)
            if (level == other.level && !isMerge && !other.isMerge)
            {
                Vector3 myPos = transform.position;
                Vector3 otherPos = other.transform.position;
                if (isBouns) GetItem(this);
                if (other.isBouns) GetItem(other);

                // �� ��ü�� �� �Ʒ��� �ְų�, ���� ���̸� �����ʿ� ���� ��
                if (myPos.y < otherPos.y || (myPos.y == otherPos.y && myPos.x > otherPos.x))
                {
                    Vector3 targetPos = (otherPos + myPos) / 2;
                    // 점수가 재료 구슬의 레벨에 따라 증가
                    GameManager.Instance.Addscore(GetBallScore(level));
                    // 접촉한 2개의 구슬은 모두 사라짐
                    other.Hide();
                    Hide();
                    // 중점에 새로운 구슬 생성됨 (최대 레벨 2개 합성할 땐 안 생성)
                    if (level < 11)
                    {
                        GameManager.Instance.AppearNextLevel(targetPos, level + 1);
                        StartCoroutine(AttachSFX_co());
                        EffectPlay(targetPos);
                    }
                }
            }
        }
    }
    // 예상 궤적 그리는 메소드. 생성 또는 이동 직후 그려짐
    public void DrawLine(bool isDraw)
    {
        lineRenderer.positionCount = isDraw ? 2 : 0;
        if (isDraw)
        {
            // 시작점, 도착점 초기 지정
            Vector3 startPoint = transform.position + Vector3.down * transform.localScale.y;
            Vector3 endPoint = startPoint + Vector3.down * 100;

            // 아래 방향으로 Ray 발사
            RaycastHit2D hit = Physics2D.Raycast(startPoint, Vector2.down, 100f, ~LayerMask.GetMask("Ignore Raycast"));

            // Ray가 충돌하면 도착점 재지정
            if (hit.collider != null)
            {
                endPoint = hit.point;
            }
            else { print("!"); }
            // 궤적 그리기
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);
        }
    }


    // 아이템을 획득했을 때 메소드
    private void GetItem(Ball b)
    {
        if (GameManager.Instance.isGameOver) return;
        // 아이템 속성 없애기
        b.isBouns = false;
        // b.sprite.color = Color.white;
        // 아이템 획득 효과
        GameManager.Instance.TicketUsed();
    }

    public void Hide()
    {
        isMerge = true; // �ռ� ���� ���·�
        // 합성되는 즉시 물리 제거
        PhysicChange(false);

        if (hasItem && generatedItems.Count > 0)
        {
            GetItem();
        }
        // Disappear Effect
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(AnimateScale(transform.localScale, Vector3.zero, hideTime, "Hide"));
        }
    }

    private void GetItem()
    {
        foreach (var item in generatedItems)
        {
            // 부모 객체로부터 아이템 분리
            item.transform.SetParent(null);

            // 인벤토리에 추가
            ItemManager.instance.AddItemsToInventory(generatedItems);
        }

        // 생성된 아이템 리스트 초기화
        generatedItems.Clear();
        hasItem = false;
    }

    IEnumerator PhysicOff_co()
    {
        yield return new WaitForSeconds(physicOffTime);
        PhysicChange(false);
    }
    

    #region Legacy(Hide_co, LevelUp, LevelUp_co)
    /*
    private IEnumerator Hide_co(Vector3 targetPos)
    {
        // �ռ��Ǵ� 2���� �������� �̵���Ű��
        int frameCount = 0;
        while (frameCount < 18)
        {
            frameCount++;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.4f);
            yield return null;
        }
        isMerge = false;
        // ���� ����, �ӽ÷� ������ŭ
        gameObject.SetActive(false);
    }

    private void LevelUp(Vector3 targetPos)
    {
        isMerge = true; // �ռ� ���� ���·�
        rigid.velocity = Vector2.zero; // �̵��ӵ��� 0����
        rigid.angularVelocity = 0; // ȸ���ӵ��� 0����
        rigid.simu1lated = false;
        circle_col.enabled = false; // ���� ���� off

        StartCoroutine(LevelUp_co(targetPos));
    }

    private IEnumerator LevelUp_co(Vector3 targetPos)
    {
        // �ռ��Ǵ� 2���� �������� �̵���Ű��
        int frameCount = 0;
        while (frameCount < 5)
        {
            frameCount++;
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.4f);
            yield return null;
        }
        SoundManager.instance.PlaySFX("LvUp1");
        // ���� ��: Scale, Sprite�� �׿� �°� ����
        level++;

        var ballScale = GetBallScale(level);
        transform.localScale = Vector2.one * ballScale;
        sprite.spriteName = GetBallSprite(level);
        //   Debug.Log($"Level Up - Level : {level}, Sprite : {sprite.spriteName}");
        //gameObject.GetComponent<UISprite>().spriteName = GetBallSprite(level);
        //gameObject.GetComponent<SpriteRenderer>().sprite = GameManager.instance.BallSprites[level];

        EffectPlay();

        // 테스트를 위해 아이템 기능 임시 부활
        float rr = UnityEngine.Random.Range(0.0f, 1.0f);
        if (rr < fruitData.fruits[level].attribute.itemProb)
        {
            isBouns = true;
            sprite.color = Color.green;
        }
        // ������ ���� Ȯ�� ��� �� ����

        if (GameManager.Instance.IsDropItem())
        {
            //아이템 종류 정하고, 떨구는 로직
            var itemInfo = GameManager.Instance.ChooseItem(); // 아이템 종류 

        }

        // �ٽ� ���� ����ǰ�
        rigid.simul1ated = true;
        circle_col.enabled = true;
        isMerge = false;
    }
    */
    #endregion
    IEnumerator AttachSFX_co()
    {
        if (isAttach) yield break;
        isAttach = true;
        SoundManager.instance.PlaySFX("Next");
        yield return new WaitForSeconds(0.1f);
        isAttach = false;
    }

    // ������ ���� ���ο� �ӹ��� ���� �� (���� ���� ���� �޼ҵ�)
    private float deadTime;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            deadTime += Time.deltaTime;
            if (deadTime > warningTime) texture.color = new Color(0.8f, 0.2f, 0.2f);
            if (deadTime > failTime)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    // ������ ���� ������ ����� ��
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            // 탈락 존에 머물러 있다가 벗어난 과일을 빨간색에서 원래대로
            deadTime = 0;
            texture.color = Color.white;
        }
    }


    private void EffectPlay(Vector3 pos)
    {
        // effect.transform.position = transform.position;
        // effect.transform.localScale = transform.localScale;
        // effect.Play();

        animator.transform.position = pos;
        animator.transform.localScale = transform.localScale;
        animator.PlayAnimation();
    }


    // 물리 On/Off 메소드
    private void PhysicChange(bool isOn)
    {
        if (!isOn)
        {
            rigid.velocity = Vector2.zero;
            rigid.angularVelocity = 0;
        }
        rigid.simulated = isOn;
        circle_col.enabled = isOn;
    }

    // Data Table 에서 정보 가져오는 메소드

    private Vector3 GetBallScale(int level)
    {
        var ballScale = new Vector3
            (
                fruitData.fruits[level - 1].attribute.scaleX,
                fruitData.fruits[level - 1].attribute.scaleY,
                1
            );

        return ballScale;
    }


    private float GetBallMass(int level) => fruitData.fruits[level - 1].attribute.mass;
    private string GetBallSprite(int level) => fruitData.fruits[level - 1].attribute.imgName;

    public Texture2D GetBallTexture(int level) => fruitData.fruits[level - 1].attribute.texture;

    private int GetBallScore(int level) => fruitData.fruits[level - 1].attribute.score;

}

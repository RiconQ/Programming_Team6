using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    // ������ ����
    [Header("State")]
    public int level; 
    // private bool isDrag; // [Legacy] �巡�� ���ΰ�?
    public bool isDropped; // 드랍된 상태인가?
    private bool isMerge; // �������� ���ΰ�?
    private bool isBouns; // ������ ���Ե� �����ΰ�?

    // ���� ������Ʈ
    [Header("Component")]
    public ParticleSystem effect;
    public Rigidbody2D rigid;
    public CircleCollider2D circle_col;
    [SerializeField]private UISprite sprite;

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
    public float spawnAppearTime = 0.50f;
    public float physicOnTime = 0.40f; // 합성되어 생겨난 구슬의 물리가 켜지는 딜레이 시간
    public float physicOffTime = 0.10f; // 재료 구슬의 물리가 꺼지는 딜레이 시간

    // 도움 선 관련
    [Header("Support Line")]
    private LineRenderer lineRenderer;  // LineRenderer 컴포넌트


    private void Awake()
    {
        // ���� ������Ʈ �ҷ�����
        rigid = GetComponent<Rigidbody2D>();
        circle_col = GetComponent<CircleCollider2D>();
        sprite = GetComponent<UISprite>();

        lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        var ballScale = GetBallScale(level);

        // 합성되어 생긴 구슬
        if(isDropped)
        {
            // 크기는 즉시 적용, 물리는 정해진 시간 후 적용
            transform.DOScale(ballScale, 0).OnComplete(() => StartCoroutine(PhysicON_co()));
        }

        // 스폰된 구슬
        else
        {
            // 크기는 정해진 시간동안 커짐 (물리는 드랍된 이후 적용)
            transform.DOScale(ballScale, spawnAppearTime).SetEase(Ease.OutBack);
        }

        // 아이템 생성 여부
        float rr = UnityEngine.Random.Range(0.0f, 1.0f);
        if (rr < fruitData.fruits[level].attribute.itemProb)
        {
            isBouns = true;
            sprite.color = Color.green;
        }

        this.GetComponent<CircleCollider2D>().radius = 47;
        rigid.mass = GetBallMass(level);
        sprite.atlas = fruitData.atlas;

        sprite.spriteName = GetBallSprite(level);


        //   Debug.Log($"Last Ball - Level : {level}, Sprite : {sprite.spriteName}");
        // ��� ���� ���� ����
        // �ð� ����� �ε��� ���⸸ �ϵ� �ڵ��Ͽ����ϴ�... (���� �ذ� ����)
        // BorderLeft = -2.75f + transform.localScale.x;
        // BorderRight = -BorderLeft;
    }

    // 생겨난 구슬의 물리 On 지연 시간
    IEnumerator PhysicON_co()
    {
        float waitTime = isDropped? physicOnTime : 0;
        yield return new WaitForSeconds(waitTime);
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
        transform.localScale = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        // ���� �ʱ�ȭ
        // PhysicChange(false);
    }

    private void Update()
    {
        // [Legacy] 터치식 드랍
        /*
        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // ��� ���� ���� ����
            mousePos.x = Mathf.Clamp(mousePos.x, BorderLeft, BorderRight);
            mousePos.y = 4;
            mousePos.z = 0;
            transform.position = mousePos;
        }
        */

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
        StartCoroutine(AttachSFX_co());
  
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
                    if(level < 11)
                    {
                        GameManager.Instance.AppearNextLevel(targetPos, level + 1);
                    }
                }
            }
        }
    }
    // 예상 궤적 그리는 메소드. 생성 또는 이동 직후 그려짐
    public void DrawLine(bool isDraw)
    {
        lineRenderer.positionCount = isDraw? 2 : 0;
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
        // 아이템 속성 없애기
        b.isBouns = false;
        b.sprite.color = Color.white;
        // 아이템 획득 효과
        Debug.Log("Get Bouns!");
        GameManager.Instance.Addscore(10);
    }

    public void Hide()
    {
        isMerge = true; // �ռ� ���� ���·�
        // Disappear Effect
        transform.DOScale(0, hideTime).OnComplete(() => gameObject.SetActive(false));
        if(gameObject.activeInHierarchy) StartCoroutine(PhysicOff_co());
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
        SoundManager.instance.PlaySFX("Attach");
        yield return new WaitForSeconds(0.2f);
        isAttach = false;
    }

    // ������ ���� ���ο� �ӹ��� ���� �� (���� ���� ���� �޼ҵ�)
    private float deadTime;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Finish"))
        {
            deadTime += Time.deltaTime;
            if (deadTime > warningTime) sprite.color = new Color(0.8f, 0.2f, 0.2f);
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
            deadTime = 0;
            sprite.color = Color.white;
        }
    }


    private void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
    }


    private bool isGetItem()
    {
        float getItem = Random.Range(0, 1);
        return (getItem < 0.5);
    }

    // 물리 On/Off 메소드
    private void PhysicChange(bool isOn)
    {
        if(!isOn)
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
                fruitData.fruits[level].attribute.scaleX,
                fruitData.fruits[level].attribute.scaleY,
                1
            );

        return ballScale;
    }
    private float GetBallMass(int level) => fruitData.fruits[level].attribute.mass;
    private string GetBallSprite(int level) => fruitData.fruits[level].attribute.imgName;
    private int GetBallScore(int level) => fruitData.fruits[level].attribute.score;

}

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
    // private bool isDropped; // 드랍된 상태인가?
    private bool isMerge; // �������� ���ΰ�?
    private bool isBouns; // ������ ���Ե� �����ΰ�?

    // ���� ������Ʈ
    [Header("Component")]
    public ParticleSystem effect;
    public Rigidbody2D rigid;
    CircleCollider2D circle_col;
    [SerializeField]private UISprite sprite;

    // [JSON ����] ���� ���� ����
    [Header("Info")]
    public int maxLevel;
    public FruitDataImporter fruitData;

    [Range(0, 100)] 
    public int lv3ItemChance; // ���� 3(4��°) - ������ ���Ե� ������ Ȯ��

    // ���� ���� ����
    [Header("About Game Over")]
    public float warningTime = 0.50f;
    public float failTime = 0.75f;

    // ��� ���� X ����
    [Header("Drop Border")]
    private float BorderLeft;
    private float BorderRight;

    // 도움 선 관련
    [Header("Support Line")]
    private Vector3 startPoint;  // 선을 시작할 위치
    private LineRenderer lineRenderer;  // LineRenderer 컴포넌트


    private void Awake()
    {
        // ���� ������Ʈ �ҷ�����
        rigid = GetComponent<Rigidbody2D>();
        circle_col = GetComponent<CircleCollider2D>();
        sprite = GetComponent<UISprite>();

        startPoint = transform.position + Vector3.down * transform.localScale.y;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    private void OnEnable()
    {
        // ������ ������ �� ȿ��. DOTween Ȱ��
        var ballScale = GetBallScale(level);
        transform.DOScale(ballScale, 0.5f).SetEase(Ease.OutBack);
        this.GetComponent<CircleCollider2D>().radius = 47;
        rigid.mass = GetBallMass(level);
        sprite.atlas = fruitData.atlas;

        sprite.spriteName = GetBallSprite(level);
     //   Debug.Log($"Last Ball - Level : {level}, Sprite : {sprite.spriteName}");

        // ��� ���� ���� ����
        // �ð� ����� �ε��� ���⸸ �ϵ� �ڵ��Ͽ����ϴ�... (���� �ذ� ����)
        BorderLeft = -2.75f + transform.localScale.x;
        BorderRight = 2.75f - transform.localScale.x;
    }

    private void OnDisable()
    {
        // ���� ��Ȱ��ȭ �� ��
        // �Ӽ� �ʱ�ȭ
        // isDrag = false;
        // isDropped = false;
        level = 0;
        isMerge = false;
        isBouns = false;
        // transform �ʱ�ȭ
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        // ���� �ʱ�ȭ
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circle_col.enabled = true;
    }

    private void Update()
    {
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
        /*
        if (!isDropped)
        {
            // 아래 방향으로 Raycast 발사
            Ray ray = new Ray(startPoint, Vector3.down);
            RaycastHit hit;
            /*
            // Ray가 충돌하면
            if (Physics.Raycast(ray, out hit, 100f, ~0))
            {
                // 충돌 지점까지 선을 그립니다.
                lineRenderer.SetPosition(0, startPoint);  // 시작점
                lineRenderer.SetPosition(1, hit.point);  // 충돌 지점
            }

            lineRenderer.SetPosition(0, startPoint);  // 시작점
            lineRenderer.SetPosition(1, startPoint + Vector3.down * 10);  // 충돌 지점
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
        rigid.simulated = true;

    }

    // ������ �ٸ� �ݶ��̴��� �������� ��
    private bool isAttach;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(AttachSFX_co());
        if (collision.gameObject.tag == "Ball")
        {
            Ball other = collision.gameObject.GetComponent<Ball>();

            // �ռ� ���� ���� �� (�ϴ� �ִ� ������ ������� ����)
            if (level == other.level && !isMerge && !other.isMerge)
            {
                float myX = transform.position.x;
                float myY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;
                // �ռ��Ǵ� 2���� ���� �߿� �������� �ִ� ���
                // �켱 ����� ��� �� 10�� ������ ó��
                if (isBouns)
                {
                    isBouns = false;
                    sprite.color = Color.white;
                    Debug.Log("Get Bouns!");
                    GameManager.Instance.Addscore(10);
                }
                if (other.isBouns)
                {
                    other.isBouns = false;
                    other.sprite.color = Color.white;
                    Debug.Log("Get Bouns!");
                    GameManager.Instance.Addscore(10);
                }

                // �� ��ü�� �� �Ʒ��� �ְų�, ���� ���̸� �����ʿ� ���� ��
                if (myY < otherY || (myY == otherY && myX > otherX))
                {
                    Vector3 targetPos = new Vector3((myX + otherX) / 2, (myY + otherY) / 2, 0);
                    // 점수가 재료 구슬의 레벨에 따라 증가
                    GameManager.Instance.Addscore(fruitData.fruits[level].attribute.score);
                    other.Hide(targetPos);
                    if (level < maxLevel)
                    {
                        LevelUp(targetPos);
                    }
                    // 최대 레벨 2개가 합쳐지는 경우, 둘 다 사라지게 됨
                    else
                    {
                        Hide(targetPos);
                    }
                }
            }
        }
    }
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
        if(collision.tag == "Finish")
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

    public void Hide(Vector3 targetPos)
    {
        isMerge = true; // �ռ� ���� ���·�
        rigid.simulated = false;
        circle_col.enabled = false; // ���� ���� off
        StartCoroutine(Hide_co(targetPos));
    }

    private IEnumerator Hide_co(Vector3 targetPos)
    {
        // �ռ��Ǵ� 2���� �������� �̵���Ű��
        int frameCount = 0;
        while(frameCount < 10)
        {
            frameCount++;
            // ���� ���� ���ΰ��
            if (targetPos.y < 900)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.4f);
            }
            // ���� ������ ���� ������� ���
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.4f);
            }
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
        rigid.simulated = false;
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
        rigid.simulated = true;
        circle_col.enabled = true; 
        isMerge = false;
    }

    private void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
    }

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

    private bool isGetItem()
    {
        float getItem = Random.Range(0, 1);
        return (getItem < 0.5);
    }

    private float GetBallMass(int level) => fruitData.fruits[level].attribute.mass;
    private string GetBallSprite(int level) => fruitData.fruits[level].attribute.imgName;

}

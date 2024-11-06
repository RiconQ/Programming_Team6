using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    // 구슬의 상태
    [Header("State")]
    public int level; 
    private bool isDrag; // 드래그 중인가?
    private bool isMerge; // 합쳐지는 중인가?
    private bool isBouns; // 아이템 포함된 구슬인가?

    // 구슬 컴포넌트
    [Header("Component")]
    public ParticleSystem effect;
    public Rigidbody2D rigid;
    CircleCollider2D circle_col;
    SpriteRenderer sp_render;

    // [JSON 예정] 구슬 관련 정보
    [Header("Info")]
    public FruitDataImporter fruitData;

    [Range(0, 100)] 
    public int lv3ItemChance; // 레벨 3(4번째) - 아이템 포함된 구슬일 확률

    // 게임 오버 관련
    [Header("About Game Over")]
    public float warningTime = 0.50f;
    public float failTime = 0.75f;

    // 드랍 범위 X 제한
    [Header("Drop Border")]
    private float BorderLeft;
    private float BorderRight;

    private void Awake()
    {
        // 구슬 컴포넌트 불러오기
        isDrag = false;
        rigid = GetComponent<Rigidbody2D>();
        circle_col = GetComponent<CircleCollider2D>();
        sp_render = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // 구슬이 등장할 때 효과. DOTween 활용
        var ballScale = GetBallScale(level);
        transform.DOScale(ballScale, 0.5f).SetEase(Ease.OutBack);
        rigid.mass = GetBallMass(level);

        // 드랍 범위 제한 설정
        // 시간 관계상 부득이 여기만 하드 코딩하였습니다... (오류 해결 실패)
        BorderLeft = -2.75f + transform.localScale.x;
        BorderRight = 2.75f - transform.localScale.x;
    }

    private void OnDisable()
    {
        // 구슬 비활성화 될 때
        // 속성 초기화
        level = 0;
        isDrag = false;
        isMerge = false;
        isBouns = false;
        // transform 초기화
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        // 물리 초기화
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circle_col.enabled = true;
    }

    private void Update()
    {
        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // 드랍 가능 범위 제한
            mousePos.x = Mathf.Clamp(mousePos.x, BorderLeft, BorderRight);
            mousePos.y = 4;
            mousePos.z = 0;
            transform.position = mousePos;
        }
    }

    public void Drag()
    {
        isDrag = true;
    }
    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;
    }

    // 구슬이 다른 콜라이더에 접촉했을 때
    private bool isAttach;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(AttachSFX_co());
        if (collision.gameObject.tag == "Ball")
        {
            Ball other = collision.gameObject.GetComponent<Ball>();

            // 합성 조건 만족 시 (일단 최대 레벨은 사라지지 않음)
            if (level == other.level && !isMerge && !other.isMerge && level < 10)
            {
                float myX = transform.position.x;
                float myY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;
                // 합성되는 2개의 구슬 중에 아이템이 있는 경우
                // 우선 디버그 출력 및 10점 증가로 처리
                if (isBouns)
                {
                    isBouns = false;
                    sp_render.color = Color.white;
                    Debug.Log("Get Bouns!");
                    GameManager.instance.Addscore(10);
                }
                if (other.isBouns)
                {
                    other.isBouns = false;
                    other.sp_render.color = Color.white;
                    Debug.Log("Get Bouns!");
                    GameManager.instance.Addscore(10);
                }

                // 이 객체가 더 아래에 있거나, 같은 높이면 오른쪽에 있을 때
                if (myY < otherY || (myY == otherY && myX > otherX))
                {
                    Vector3 targetPos = new Vector3((myX + otherX) / 2, (myY + otherY) / 2, 0);
                    other.Hide(targetPos);
                    LevelUp(targetPos);
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

    // 구슬이 실패 라인에 머물러 있을 때 (게임 오버 판정 메소드)
    private float deadTime;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Finish")
        {
            deadTime += Time.deltaTime;
            if (deadTime > warningTime) sp_render.color = new Color(0.8f, 0.2f, 0.2f);
            if (deadTime > failTime)
            {
                GameManager.instance.GameOver();
            }
        }
    }
    
    // 구슬이 실패 라인을 벗어났을 때
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime = 0;
            sp_render.color = Color.white;
        }
    }

    public void Hide(Vector3 targetPos)
    {
        isMerge = true; // 합성 중인 상태로
        rigid.simulated = false;
        circle_col.enabled = false; // 물리 적용 off
        StartCoroutine(Hide_co(targetPos));
    }

    private IEnumerator Hide_co(Vector3 targetPos)
    {
        // 합성되는 2개의 중점으로 이동시키기
        int frameCount = 0;
        while(frameCount < 10)
        {
            frameCount++;
            // 게임 진행 중인경우
            if (targetPos.y < 900)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.4f);
            }
            // 게임 오버로 인해 사라지는 경우
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.4f);
            }
            yield return null;
        }
        isMerge = false;
        // 점수 증가, 임시로 레벨만큼
        GameManager.instance.Addscore(level);
        gameObject.SetActive(false);
    }

    private void LevelUp(Vector3 targetPos)
    {
        isMerge = true; // 합성 중인 상태로
        rigid.velocity = Vector2.zero; // 이동속도를 0으로
        rigid.angularVelocity = 0; // 회전속도를 0으로
        rigid.simulated = false;
        circle_col.enabled = false; // 물리 적용 off

        StartCoroutine(LevelUp_co(targetPos));
    }

    private IEnumerator LevelUp_co(Vector3 targetPos)
    {
        // 합성되는 2개의 중점으로 이동시키기
        int frameCount = 0;
        while (frameCount < 5)
        {
            frameCount++;
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.4f);
            yield return null;
        }
        SoundManager.instance.PlaySFX("LvUp1");
        // 레벨 업: Scale, Sprite도 그에 맞게 변경
        level++;
        var ballScale = GetBallScale(level);
        transform.localScale = Vector2.one * ballScale;
        gameObject.GetComponent<SpriteRenderer>().sprite = GameManager.instance.BallSprites[level];
        EffectPlay();
        // 아이템 등장 확률 계산 및 적용
        if(level == 3)
        {
            if (Random.Range(0, 100) < lv3ItemChance)
            {
                isBouns = true;
                sp_render.color = Color.green;
            }
        }

        // 다시 물리 적용되게
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

    private float GetBallMass(int level) => fruitData.fruits[level].attribute.mass;
    //private Sprite GetBallSprite(int level) => fruitData.fruits[level].attribute.atlas.GetSprite(fruitData.fruits[level].attribute.imgName);
}

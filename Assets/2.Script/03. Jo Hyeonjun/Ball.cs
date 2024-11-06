using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    // ������ ����
    [Header("State")]
    public int level; 
    private bool isDrag; // �巡�� ���ΰ�?
    private bool isMerge; // �������� ���ΰ�?
    private bool isBouns; // ������ ���Ե� �����ΰ�?

    // ���� ������Ʈ
    [Header("Component")]
    public ParticleSystem effect;
    public Rigidbody2D rigid;
    CircleCollider2D circle_col;
    SpriteRenderer sp_render;

    // [JSON ����] ���� ���� ����
    [Header("Info")]
    public float[] ballScale = { 0.30f, 0.36f, 0.44f, 0.56f, 0.68f, 
                                  0.80f, 0.96f, 1.10f, 1.20f, 1.36f, 1.60f};
    public float[] ballMass =  { 1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 
                                  3.5f, 4.0f, 4.5f, 5.0f, 5.5f, 6.0f};
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

    private void Awake()
    {
        // ���� ������Ʈ �ҷ�����
        isDrag = false;
        rigid = GetComponent<Rigidbody2D>();
        circle_col = GetComponent<CircleCollider2D>();
        sp_render = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // ������ ������ �� ȿ��. DOTween Ȱ��
        transform.DOScale(ballScale[level], 0.5f).SetEase(Ease.OutBack);
        // ��� ���� ���� ����
        // �ð� ����� �ε��� ���⸸ �ϵ� �ڵ��Ͽ����ϴ�... (���� �ذ� ����)
        BorderLeft = -2.75f + transform.localScale.x;
        BorderRight = 2.75f - transform.localScale.x;
    }

    private void OnDisable()
    {
        // ���� ��Ȱ��ȭ �� ��
        // �Ӽ� �ʱ�ȭ
        level = 0;
        isDrag = false;
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
        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // ��� ���� ���� ����
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

    // ������ �ٸ� �ݶ��̴��� �������� ��
    private bool isAttach;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(AttachSFX_co());
        if (collision.gameObject.tag == "Ball")
        {
            Ball other = collision.gameObject.GetComponent<Ball>();

            // �ռ� ���� ���� �� (�ϴ� �ִ� ������ ������� ����)
            if (level == other.level && !isMerge && !other.isMerge && level < 10)
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
                    sp_render.color = Color.white;
                    Debug.Log("Get Bouns!");
                    GameManager.Instance.Addscore(10);
                }
                if (other.isBouns)
                {
                    other.isBouns = false;
                    other.sp_render.color = Color.white;
                    Debug.Log("Get Bouns!");
                    GameManager.Instance.Addscore(10);
                }

                // �� ��ü�� �� �Ʒ��� �ְų�, ���� ���̸� �����ʿ� ���� ��
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

    // ������ ���� ���ο� �ӹ��� ���� �� (���� ���� ���� �޼ҵ�)
    private float deadTime;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Finish")
        {
            deadTime += Time.deltaTime;
            if (deadTime > warningTime) sp_render.color = new Color(0.8f, 0.2f, 0.2f);
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
            sp_render.color = Color.white;
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
        GameManager.Instance.Addscore(level);
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
        transform.localScale = Vector2.one * ballScale[level];
        gameObject.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.BallSprites[level];
        EffectPlay();
        // ������ ���� Ȯ�� ��� �� ����
        if(level == 3)
        {
            if (Random.Range(0, 100) < lv3ItemChance)
            {
                isBouns = true;
                sp_render.color = Color.green;
            }
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
}

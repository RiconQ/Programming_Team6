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
    [SerializeField]private UISprite sprite;

    // [JSON ����] ���� ���� ����
    [Header("Info")]
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

    private void Awake()
    {
        // ���� ������Ʈ �ҷ�����
        isDrag = false;
        rigid = GetComponent<Rigidbody2D>();
        circle_col = GetComponent<CircleCollider2D>();
        sprite = GetComponent<UISprite>();
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
        Debug.Log($"Last Ball - Level : {level}, Sprite : {sprite.spriteName}");

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

        var ballScale = GetBallScale(level);
        transform.localScale = Vector2.one * ballScale;
        sprite.spriteName = GetBallSprite(level);
        Debug.Log($"Level Up - Level : {level}, Sprite : {sprite.spriteName}");
        //gameObject.GetComponent<UISprite>().spriteName = GetBallSprite(level);
        //gameObject.GetComponent<SpriteRenderer>().sprite = GameManager.instance.BallSprites[level];
        
        EffectPlay();
        // ������ ���� Ȯ�� ��� �� ����
        if(level == 3)
        {
            if (Random.Range(0, 100) < lv3ItemChance)
            {
                isBouns = true;
                sprite.color = Color.green;
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
}

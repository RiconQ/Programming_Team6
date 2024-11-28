using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxControl : MonoBehaviour
{
    [SerializeField] private Transform targetTransform; // ��ǥ ��ġ (Transform)
    private Vector3 targetPos; // ��ǥ ��ġ (Vector3)
    [SerializeField] private Transform originTransform; // ���� ��ġ (Transform)
    private Vector3 originPos; // ���� ��ġ (Vector3)

    [SerializeField] private float moveSpeed = 5f; // �̵� �ӵ�
    [SerializeField] private float delayBeforeHide = 1f; // HideBox ȣ�� �� ��� �ð�

    public static BoxControl instance = null;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            // �浹�� ������Ʈ ��Ȱ��ȭ
            collision.gameObject.SetActive(false);

            // �ߺ� �߰� ����
            if (!GameManager.Instance.itemPool.Contains(collision.gameObject))
            {
                GameManager.Instance.itemPool.Add(collision.gameObject);
            }

            // 1�� ��� �� HideBox ȣ��
            StartCoroutine(DelayBeforeHideBox());

            Debug.Log($"������Ʈ ��ȯ: {collision.gameObject.name}");
        }
    }

    private void Start()
    {
        // ��ǥ ��ġ�� ���� ��ġ ����
        targetPos = targetTransform.position;
        originPos = originTransform.position;
    }

    private IEnumerator DelayBeforeHideBox()
    {
        // ������ �ð���ŭ ���
        yield return new WaitForSeconds(delayBeforeHide);

        // HideBox ȣ��
        StartMoveBox(originPos);
    }

    public void StartMoveBox(Vector3 targetPos)
    {
        StartCoroutine(MoveBox_co(targetPos));
    }


    private IEnumerator MoveBox_co(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
    }



}


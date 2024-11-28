using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxControl : MonoBehaviour
{
    [SerializeField] private Transform targetTransform; // 목표 위치 (Transform)
    private Vector3 targetPos; // 목표 위치 (Vector3)
    [SerializeField] private Transform originTransform; // 원래 위치 (Transform)
    private Vector3 originPos; // 원래 위치 (Vector3)

    [SerializeField] private float moveSpeed = 5f; // 이동 속도
    [SerializeField] private float delayBeforeHide = 1f; // HideBox 호출 전 대기 시간

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
            // 충돌한 오브젝트 비활성화
            collision.gameObject.SetActive(false);

            // 중복 추가 방지
            if (!GameManager.Instance.itemPool.Contains(collision.gameObject))
            {
                GameManager.Instance.itemPool.Add(collision.gameObject);
            }

            // 1초 대기 후 HideBox 호출
            StartCoroutine(DelayBeforeHideBox());

            Debug.Log($"오브젝트 반환: {collision.gameObject.name}");
        }
    }

    private void Start()
    {
        // 목표 위치와 원래 위치 설정
        targetPos = targetTransform.position;
        originPos = originTransform.position;
    }

    private IEnumerator DelayBeforeHideBox()
    {
        // 설정된 시간만큼 대기
        yield return new WaitForSeconds(delayBeforeHide);

        // HideBox 호출
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


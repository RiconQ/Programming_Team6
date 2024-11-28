using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxControl : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            collision.gameObject.SetActive(false);

            // 중복 추가 방지
            if (!GameManager.Instance.itemPool.Contains(collision.gameObject))
            {
                GameManager.Instance.itemPool.Add(collision.gameObject);
            }

            Debug.Log($"오브젝트 반환: {collision.gameObject.name}");
        }
    }
}

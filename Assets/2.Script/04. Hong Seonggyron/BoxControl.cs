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

            // �ߺ� �߰� ����
            if (!GameManager.Instance.itemPool.Contains(collision.gameObject))
            {
                GameManager.Instance.itemPool.Add(collision.gameObject);
            }

            Debug.Log($"������Ʈ ��ȯ: {collision.gameObject.name}");
        }
    }
}

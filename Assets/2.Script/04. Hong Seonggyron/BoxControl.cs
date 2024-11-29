using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxControl : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            // �浹�� ������Ʈ ��Ȱ��ȭ
            collision.gameObject.SetActive(false);

            // �ߺ� �߰� ����
            if (!ItemManager.instance.itemPool.Contains(collision.gameObject))
            {
                ItemManager.instance.itemPool.Add(collision.gameObject);
            }

            // 1�� ��� �� HideBox ȣ��
            ItemManager.instance.StartDelayBeforeHideBox_co();

            Debug.Log($"������Ʈ ��ȯ: {collision.gameObject.name}");
        }
    }
}

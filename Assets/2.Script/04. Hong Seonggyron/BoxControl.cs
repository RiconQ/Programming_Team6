using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxControl : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            // 충돌한 오브젝트 비활성화
            collision.gameObject.SetActive(false);

            // 중복 추가 방지
            if (!ItemManager.instance.itemPool.Contains(collision.gameObject))
            {
                ItemManager.instance.itemPool.Add(collision.gameObject);
            }

            // 1초 대기 후 HideBox 호출
            ItemManager.instance.StartDelayBeforeHideBox_co();

            Debug.Log($"오브젝트 반환: {collision.gameObject.name}");
        }
    }
}

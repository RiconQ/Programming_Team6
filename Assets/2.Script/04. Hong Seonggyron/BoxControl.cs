using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxControl : MonoBehaviour
{
    public GameObject effect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            collision.transform.localScale = Vector3.one;
            // 충돌한 오브젝트 비활성화
            var particle=collision.GetComponentInChildren<ParticleSystem>();
            particle.gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
            particle.Play();

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

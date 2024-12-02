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
            // �浹�� ������Ʈ ��Ȱ��ȭ
            var particle=collision.GetComponentInChildren<ParticleSystem>();
            particle.gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
            particle.Play();

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

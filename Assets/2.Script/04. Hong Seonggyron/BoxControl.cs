using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BoxControl : MonoBehaviour
{
    public GameObject effect;
    private ParticleSystem effect_Particle;

    private void Start()
    {
        effect_Particle = effect.GetComponent<ParticleSystem>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            collision.transform.localScale = Vector3.one;
            // ������ ��Ȱ��ȭ
            collision.gameObject.SetActive(false);
            ItemManager.instance.itemCount -= 1;

            //������ ��ƼŬ ����
            var itemParticle = collision.gameObject.GetComponentInChildren<ParticleSystem>();
            itemParticle.gameObject.SetActive(false);
            //â�� ��ƼŬ Ȱ��ȭ
            effect.SetActive(true);

            // �ߺ� �߰� ����
            if (!ItemManager.instance.itemPool.Contains(collision.gameObject))
            {
                ItemManager.instance.itemPool.Add(collision.gameObject);
            }


            if (ItemManager.instance.itemCount <= 0)
            {
                ItemManager.instance.StartDelayBeforeHideBox();
            }
            StartCoroutine(test());

            Debug.Log($"������Ʈ ��ȯ: {collision.gameObject.name}");
        }
    }



    public IEnumerator test()
    {

        yield return new WaitForSeconds(0.5f);
        // ȿ�� ��Ȱ��ȭ
        effect.SetActive(false);
    }
}

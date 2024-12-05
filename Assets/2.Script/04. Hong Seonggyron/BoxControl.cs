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
            // 아이템 비활성화
            collision.gameObject.SetActive(false);
            ItemManager.instance.itemCount -= 1;

            //아이템 파티클 해제
            var itemParticle = collision.gameObject.GetComponentInChildren<ParticleSystem>();
            itemParticle.gameObject.SetActive(false);
            //창고 파티클 활성화
            effect.SetActive(true);

            // 중복 추가 방지
            if (!ItemManager.instance.itemPool.Contains(collision.gameObject))
            {
                ItemManager.instance.itemPool.Add(collision.gameObject);
            }


            if (ItemManager.instance.itemCount <= 0)
            {
                ItemManager.instance.StartDelayBeforeHideBox();
            }
            StartCoroutine(test());

            Debug.Log($"오브젝트 반환: {collision.gameObject.name}");
        }
    }



    public IEnumerator test()
    {

        yield return new WaitForSeconds(0.5f);
        // 효과 비활성화
        effect.SetActive(false);
    }
}

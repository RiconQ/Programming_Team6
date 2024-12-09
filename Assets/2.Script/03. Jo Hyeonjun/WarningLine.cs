using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLine : MonoBehaviour
{
    public float warningTime = 0.75f; // ��� ������ �� �䱸�Ǵ� �ð�
    private float stayTime; // ��� ���ο� �ӹ��� �ð�
    private int ballCount; // ���ο� ������ ���� ��(0�� �Ǹ� �ð� �ʱ�ȭ)
    public GameObject failLine; // �����Ÿ��� �� ���� ��
    private UISprite uiSprite;

    private void Start()
    {
        uiSprite = failLine.GetComponent<UISprite>();
        stayTime = 0;
        ballCount = 0;
    }

    private void Update()
    {
        if (ballCount > 0) stayTime += Time.deltaTime;
        if (stayTime < warningTime) return;
        float alpha = (1 + Mathf.Cos(Mathf.PI * (stayTime - 0.75f) * 5)) * 0.5f;
        uiSprite.color = new Color(1, 1, 1, alpha);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball")) ballCount++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball")) ballCount--;
        if (ballCount == 0)
        {
            stayTime = 0;
            uiSprite.color = new Color(1, 1, 1, 1);
        }
    }
}

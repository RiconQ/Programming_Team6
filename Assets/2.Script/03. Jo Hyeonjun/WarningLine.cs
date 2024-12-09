using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLine : MonoBehaviour
{
    public float warningTime = 0.75f; // 경고가 켜지는 데 요구되는 시간
    private float stayTime; // 경고 라인에 머무른 시간
    private int ballCount; // 라인에 감지된 공의 수(0이 되면 시간 초기화)
    public GameObject failLine; // 깜빡거리게 될 실패 선
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NGUIAnimator : MonoBehaviour
{
    public UISprite sprite;            // �ִϸ��̼� ��� UISprite
    public string baseSpriteName;     // ��������Ʈ �̸��� �⺻ �� (��: "frame_")
    public int frameCount;            // ��Ʋ�󽺿� ��ϵǾ��ִ� sprite ����
    public float frameDelay = 0.1f;   // ������ �� �ð� ����
    public bool loop = true;          // �ִϸ��̼� �ݺ� ����

    private int currentFrame = 0;     // ���� ������ �ε���
    private bool isAnimating = false; // �ִϸ��̼� ����



    private void Start()
    {
        sprite.gameObject.SetActive(false);
    }
    public void PlayAnimation()
    {
        sprite.gameObject.SetActive(true);
        if (!isAnimating)
        {
            StartCoroutine(AnimateSprite());
        }
    }

    public void StopAnimation()
    {
        isAnimating = false;
        StopCoroutine(AnimateSprite());
    }

    private IEnumerator AnimateSprite()
    {
        isAnimating = true;
        currentFrame = 0;
        while (isAnimating)
        {
            // ��������Ʈ �̸� ���� (��: "frame_0", "frame_1", ...)
            sprite.spriteName = $"{baseSpriteName}{currentFrame}";
         //   sprite.MakePixelPerfect(); // ��������Ʈ ũ�� ���� (�ʿ� ��)

            // ���� ���������� �̵�
            currentFrame++;
            if (currentFrame >= frameCount)
            {
                if (loop)
                {
                    currentFrame = 0; // �ݺ� �ִϸ��̼�
                }
                else
                {
                    isAnimating = false; // �ִϸ��̼� ����
                    sprite.gameObject.SetActive(false);
                    break;
                }
            }

            yield return new WaitForSeconds(frameDelay); // ������ �� ���
        }
    }
}


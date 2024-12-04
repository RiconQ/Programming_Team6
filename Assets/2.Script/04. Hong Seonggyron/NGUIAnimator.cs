using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NGUIAnimator : MonoBehaviour
{
    public UISprite sprite;            // 애니메이션 대상 UISprite
    public string baseSpriteName;     // 스프라이트 이름의 기본 값 (예: "frame_")
    public int frameCount;            // 아틀라스에 등록되어있는 sprite 갯수
    public float frameDelay = 0.1f;   // 프레임 간 시간 간격
    public bool loop = true;          // 애니메이션 반복 여부

    private int currentFrame = 0;     // 현재 프레임 인덱스
    private bool isAnimating = false; // 애니메이션 상태



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
            // 스프라이트 이름 설정 (예: "frame_0", "frame_1", ...)
            sprite.spriteName = $"{baseSpriteName}{currentFrame}";
         //   sprite.MakePixelPerfect(); // 스프라이트 크기 조정 (필요 시)

            // 다음 프레임으로 이동
            currentFrame++;
            if (currentFrame >= frameCount)
            {
                if (loop)
                {
                    currentFrame = 0; // 반복 애니메이션
                }
                else
                {
                    isAnimating = false; // 애니메이션 종료
                    sprite.gameObject.SetActive(false);
                    break;
                }
            }

            yield return new WaitForSeconds(frameDelay); // 프레임 간 대기
        }
    }
}


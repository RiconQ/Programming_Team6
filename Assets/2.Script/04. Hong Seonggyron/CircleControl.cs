using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleControl : MonoBehaviour
{
    public bool isDrag;
    //인스펙터에서 잠깐 조정하려고
    private Rigidbody2D rigid;
    private SpringJoint2D springJoint;
    private Collider2D col;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        springJoint = GetComponent<SpringJoint2D>();
        col = GetComponent<Collider2D>();
        springJoint.autoConfigureConnectedAnchor = false;
    }

    void Update()
    {
        if (isDrag)
        {
            // 마우스 위치를 월드 좌표로 변환
            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePoint.z = 0;
           mousePoint.y = 2.5f; // Y축 고정

            if(mousePoint.x<-3.14f)
            {
                mousePoint.x = -3.14f;
            }
           else if (mousePoint.x > 3.14f)
            {
                mousePoint.x = 3.14f;
            }

            // 스프링 연결 지점 설정 - 사용자
            // 키링의 끝부분을 마우스 포인트로 지정
            springJoint.connectedAnchor = mousePoint;
            // 마우스 포인트를 따라가는 속도 느리게
            springJoint.connectedAnchor
                = Vector3.Lerp(springJoint.connectedAnchor, mousePoint, 0.05f);
            // 키링 줄 늘어남 방지 
            springJoint.distance = 1.6f;
            // 콜라이더 비활성화
            col.enabled = false;


        }

    }

    public void Drag()
    {
        isDrag = true;
      //  springJoint.enabled = true; // 드래그 시 스프링 활성화
    }

    public void Drop()
    {
        isDrag = false;
        col.enabled = true;
        springJoint.enabled = false;

    }

}

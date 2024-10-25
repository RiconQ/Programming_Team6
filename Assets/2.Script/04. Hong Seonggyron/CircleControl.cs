using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleControl : MonoBehaviour
{
    public bool isDrag;
    private Rigidbody2D rigid;
    private SpringJoint2D springJoint;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        springJoint = GetComponent<SpringJoint2D>();
        springJoint.autoConfigureConnectedAnchor = false;
        //springJoint.frequency = 0; // 감쇠가 없도록 설정
      //  springJoint.dampingRatio = 0; // 스프링 감쇠 비율 제거
    }

    void Update()
    {
        if (isDrag)
        {
            // 마우스 위치를 월드 좌표로 변환
            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePoint.z = 0;
            mousePoint.y = 5f; // Y축 고정

            if(mousePoint.x<-3.14f)
            {
                mousePoint.x = -3.14f;
            }
           else if (mousePoint.x > 3.14f)
            {
                mousePoint.x = 3.14f;
            }

            // 스프링 연결 지점 설정
            springJoint.connectedAnchor = mousePoint;
            springJoint.distance = 1.2f;
        }

    }

    public void Drag()
    {
        isDrag = true;
        springJoint.enabled = true; // 드래그 시 스프링 활성화
    }

    public void Drop()
    {
        isDrag = false;
        springJoint.enabled = false;

    }

}

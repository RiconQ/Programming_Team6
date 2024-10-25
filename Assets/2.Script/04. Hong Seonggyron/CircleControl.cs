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
        //springJoint.frequency = 0; // ���谡 ������ ����
      //  springJoint.dampingRatio = 0; // ������ ���� ���� ����
    }

    void Update()
    {
        if (isDrag)
        {
            // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePoint.z = 0;
            mousePoint.y = 5f; // Y�� ����

            if(mousePoint.x<-3.14f)
            {
                mousePoint.x = -3.14f;
            }
           else if (mousePoint.x > 3.14f)
            {
                mousePoint.x = 3.14f;
            }

            // ������ ���� ���� ����
            springJoint.connectedAnchor = mousePoint;
            springJoint.distance = 1.2f;
        }

    }

    public void Drag()
    {
        isDrag = true;
        springJoint.enabled = true; // �巡�� �� ������ Ȱ��ȭ
    }

    public void Drop()
    {
        isDrag = false;
        springJoint.enabled = false;

    }

}

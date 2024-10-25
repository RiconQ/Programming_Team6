using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleControl : MonoBehaviour
{
    public bool isDrag;
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
            // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePoint.z = 0;
           mousePoint.y = 2.5f; // Y�� ����

            if(mousePoint.x<-3.14f)
            {
                mousePoint.x = -3.14f;
            }
           else if (mousePoint.x > 3.14f)
            {
                mousePoint.x = 3.14f;
            }
            //------------���ڿ �κ�--------------

            // Ű���� ���� �κ��� ���콺 ����Ʈ�� ����
            springJoint.connectedAnchor = mousePoint;
            // ���콺 ����Ʈ�� ���󰡴� �ӵ� ������
            springJoint.connectedAnchor
                = Vector3.Lerp(springJoint.connectedAnchor, mousePoint, 0.05f);
            // Ű�� �� �þ ���� 
            springJoint.distance = 1.6f;
            // �ݶ��̴� ��Ȱ��ȭ
            col.enabled = false;
        }

    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        col.enabled = true;
        springJoint.enabled = false;
    }

}

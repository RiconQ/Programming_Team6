using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KimGhHun_Proto
{
    public class KeyRing : MonoBehaviour
    {
        //public GameObject hand;
        public Fruit connectedFruit;

        private DistanceJoint2D joint;

        private bool isDrag;

        //추가
        public Transform rotateObj;

        private void Awake()
        {
            joint = GetComponent<DistanceJoint2D>();
        }

        public void SetConnect()
        {
            joint.connectedBody = connectedFruit.GetComponent<Rigidbody2D>();
        }
        private void FixedUpdate()
        {
            if (isDrag)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float leftBorader = -4f;
                float rightBorader = 4f;
                mousePos.y = 4f;
                mousePos.z = 0f;
                mousePos.x = Mathf.Clamp(mousePos.x, leftBorader, rightBorader);
                // transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);


                //추가------------
                float targetAngle = Mathf.Lerp(-60f, 60f, (mousePos.x - leftBorader) / (rightBorader - leftBorader));

                // 현재 각도에서 목표 각도로 천천히 회전
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                rotateObj.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.2f); // 0.1f 값을 줄이면 더 느리게 회전

            }
        }

        public void Drag()
        {
            isDrag = true;
        }

        public void Drop()
        {
            isDrag = false;
            joint.connectedBody = null;
            connectedFruit.GetComponent<Collider2D>().enabled = true;
            connectedFruit.rb.AddForce(connectedFruit.rb.velocity * connectedFruit.rb.mass * 100);

        }
    }
}

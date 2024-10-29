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
                transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);
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

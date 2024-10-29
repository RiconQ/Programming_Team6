using UnityEngine;

namespace KimGhHun_Proto
{
    public class KeyRing : MonoBehaviour
    {
        public Fruit connectedFruit;
        private DistanceJoint2D joint;

        private bool isDrag;
        private Vector2 accumulatedDirection;
        private Vector3 previousPosition;
        private float accumulatedDistance;

        [Header("Settings")]
        public float maxAngle = 60f;            // �¿� ȸ�� ���� ����
        public float rotationSmoothness = 0.2f; // ȸ�� �ӵ�
        public float forceMultiplier = 50f;     // Drop�� �� ������ ���� ���� - �� ���� ������ ����
        public Transform rotateObj;

        private void Awake()
        {
            joint = GetComponent<DistanceJoint2D>();
        }

        public void SetConnect()
        {
            joint.connectedBody = connectedFruit.GetComponent<Rigidbody2D>();
            accumulatedDirection = Vector2.zero;
            accumulatedDistance = 0f;
            previousPosition = rotateObj.position; // ȸ�� �������� ��ġ �ʱ�ȭ
        }

        private void FixedUpdate()
        {
            if (isDrag)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float leftBorder = -4f;
                float rightBorder = 4f;
                mousePos.x = Mathf.Clamp(mousePos.x, leftBorder, rightBorder);

                // KeyRing�� ��ġ�� �����ϰ�, ������ ���콺�� ���� ����
                float targetAngle = Mathf.Lerp(-maxAngle, maxAngle, (mousePos.x - leftBorder) / (rightBorder - leftBorder));
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                rotateObj.rotation = Quaternion.Lerp(rotateObj.rotation, targetRotation, rotationSmoothness);

                // ȸ�� �̵��� ���� ���� ���� �� �Ÿ� ���
                Vector2 moveDirection = (rotateObj.position - previousPosition).normalized;
                accumulatedDirection += moveDirection;
                accumulatedDistance += Vector3.Distance(rotateObj.position, previousPosition);
                previousPosition = rotateObj.position;
            }
        }

        public void Drag()
        {
            isDrag = true;
            previousPosition = rotateObj.position; // �巡�� ���� �� ��ġ �ʱ�ȭ
        }

        public void Drop()
        {
            isDrag = false;
            joint.connectedBody = null;

            if (connectedFruit != null)
            {
                Rigidbody2D fruitRb = connectedFruit.GetComponent<Rigidbody2D>();

                // ������ ���� ���Ϳ� �Ÿ� ������� �� ���
                Vector2 launchDirection = accumulatedDirection.normalized;
                float launchPower = accumulatedDistance * forceMultiplier;

                // Debug�� ���ӷ� ũ�� Ȯ��
                Debug.Log($"Launch Direction: {launchDirection}, Launch Power: {launchPower}");

                // ����� �Ÿ� ����� �����ο� ������ ȿ�� ����
                fruitRb.AddForce(launchDirection * launchPower, ForceMode2D.Impulse);

                connectedFruit.GetComponent<Collider2D>().enabled = true;
            }
        }
    }
}

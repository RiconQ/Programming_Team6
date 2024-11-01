using UnityEngine;

namespace KimGhHun_Proto
{
    public class KeyRing : MonoBehaviour
    {
        public Fruit connectedFruit;
        private DistanceJoint2D joint;

        private LineRenderer lineRenderer;
        private float radius = 3.5f;

        private bool isDrag;
        private Vector2 accumulatedDirection;
        private Vector3 previousPosition;

        [Header("Settings")]
        public float maxAngle = 60f;             // �¿� ȸ�� ���� ����
        public float rotationSmoothness = 0.2f;  // ȸ�� �ӵ�
        public float forceMultiplier = 50f;      // Drop�� �� ������ ���� ����
        public Transform rotateObj;

        private void Awake()
        {
            joint = GetComponent<DistanceJoint2D>();
            lineRenderer = GetComponent<LineRenderer>();
        }
        private void Start()
        {
            DrawOrbit();
        }

        public void SetConnect()
        {
            joint.connectedBody = connectedFruit.GetComponent<Rigidbody2D>();
            accumulatedDirection = Vector2.zero;
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

                // ������ ����� �̵� �Ÿ� ���
                Vector2 moveDirection = (rotateObj.position - previousPosition).normalized;
                if (moveDirection != Vector2.zero)
                {
                    accumulatedDirection = moveDirection; // ���� �ֱ� ���� ������Ʈ
                    Debug.Log("accumulatedDirection ����");
                }
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

                // ���� �ֱ� ����(accumulatedDirection)�� ������� ū �� ����
                Vector2 launchDirection = accumulatedDirection.normalized;
                float launchPower = forceMultiplier; // ������ ���� �ֱ� ���� ���� forceMultiplier ���

            //    Debug.Log($"Launch Direction: {launchDirection}, Launch Power: {launchPower}");

                // ����� ���� ���� �����Ͽ� �Ѿ�ó�� �߻�
                fruitRb.AddForce(launchDirection * launchPower, ForceMode2D.Impulse);

                connectedFruit.GetComponent<Collider2D>().enabled = true;
            }
        }

        // U���� �̵����� ���� �ð�ȭ 
        private void DrawOrbit()
        {
            float centerX = rotateObj.position.x;
            float centerY = rotateObj.position.y;
            float angleStep = (maxAngle * 2) / (lineRenderer.positionCount - 1); // ���� ���� ����

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                float angle = -maxAngle + (angleStep * i); // -60������ 60������
                float radian = angle * Mathf.Deg2Rad; // ������ �������� ��ȯ

                // x, y ��ǥ ����Ͽ� �˵� �� ��ġ ����
                float x = centerX + Mathf.Sin(radian) * radius;
                float y = centerY - Mathf.Cos(radian) * radius; // y�� �̵� (�Ʒ���)

                lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            }
        }
    }
}

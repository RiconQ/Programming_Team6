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
        public float maxAngle = 60f;            // 좌우 회전 각도 제한
        public float rotationSmoothness = 0.2f; // 회전 속도
        public float forceMultiplier = 50f;     // Drop할 때 적용할 힘의 배율 - 더 높은 값으로 조정
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
            previousPosition = rotateObj.position; // 회전 기준으로 위치 초기화
        }

        private void FixedUpdate()
        {
            if (isDrag)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float leftBorder = -4f;
                float rightBorder = 4f;
                mousePos.x = Mathf.Clamp(mousePos.x, leftBorder, rightBorder);

                // KeyRing의 위치는 고정하고, 각도만 마우스에 따라 변경
                float targetAngle = Mathf.Lerp(-maxAngle, maxAngle, (mousePos.x - leftBorder) / (rightBorder - leftBorder));
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                rotateObj.rotation = Quaternion.Lerp(rotateObj.rotation, targetRotation, rotationSmoothness);

                // 회전 이동을 통한 누적 방향 및 거리 계산
                Vector2 moveDirection = (rotateObj.position - previousPosition).normalized;
                accumulatedDirection += moveDirection;
                accumulatedDistance += Vector3.Distance(rotateObj.position, previousPosition);
                previousPosition = rotateObj.position;
            }
        }

        public void Drag()
        {
            isDrag = true;
            previousPosition = rotateObj.position; // 드래그 시작 시 위치 초기화
        }

        public void Drop()
        {
            isDrag = false;
            joint.connectedBody = null;

            if (connectedFruit != null)
            {
                Rigidbody2D fruitRb = connectedFruit.GetComponent<Rigidbody2D>();

                // 누적된 방향 벡터와 거리 기반으로 힘 계산
                Vector2 launchDirection = accumulatedDirection.normalized;
                float launchPower = accumulatedDistance * forceMultiplier;

                // Debug로 가속력 크기 확인
                Debug.Log($"Launch Direction: {launchDirection}, Launch Power: {launchPower}");

                // 방향과 거리 기반의 자유로운 던지기 효과 적용
                fruitRb.AddForce(launchDirection * launchPower, ForceMode2D.Impulse);

                connectedFruit.GetComponent<Collider2D>().enabled = true;
            }
        }
    }
}

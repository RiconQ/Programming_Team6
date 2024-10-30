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

        [Header("Settings")]
        public float maxAngle = 60f;             // 좌우 회전 각도 제한
        public float rotationSmoothness = 0.2f;  // 회전 속도
        public float forceMultiplier = 50f;      // Drop할 때 적용할 힘의 배율
        public Transform rotateObj;

        private void Awake()
        {
            joint = GetComponent<DistanceJoint2D>();
        }

        public void SetConnect()
        {
            joint.connectedBody = connectedFruit.GetComponent<Rigidbody2D>();
            accumulatedDirection = Vector2.zero;
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

                // 누적된 방향과 이동 거리 계산
                Vector2 moveDirection = (rotateObj.position - previousPosition).normalized;
                if (moveDirection != Vector2.zero)
                {
                    accumulatedDirection = moveDirection; // 가장 최근 방향 업데이트
                    Debug.Log("accumulatedDirection 갱신");
                }
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

                // 가장 최근 방향(accumulatedDirection)을 기반으로 큰 힘 적용
                Vector2 launchDirection = accumulatedDirection.normalized;
                float launchPower = forceMultiplier; // 고정된 힘을 주기 위해 직접 forceMultiplier 사용

                Debug.Log($"Launch Direction: {launchDirection}, Launch Power: {launchPower}");

                // 방향과 강한 힘을 적용하여 총알처럼 발사
                fruitRb.AddForce(launchDirection * launchPower, ForceMode2D.Impulse);

                connectedFruit.GetComponent<Collider2D>().enabled = true;
            }
        }
    }
}

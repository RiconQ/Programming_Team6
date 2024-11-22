using UnityEngine;

public static class Extension
{    
    // Ȯ�� �޼���: � �̵�
    public static void MoveCurve(this GameObject startObject, GameObject endObject, Transform controlPoint, float duration)
    {
        startObject.GetComponent<MonoBehaviour>().StartCoroutine(MoveAlongCurve(startObject, endObject, controlPoint, duration));
    }

    // � �̵� �ڷ�ƾ
    private static System.Collections.IEnumerator MoveAlongCurve(GameObject startObject, GameObject endObject, Transform controlPoint, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = startObject.transform.position;
        Vector3 endPos = endObject.transform.position;
        Vector3 controlPos = controlPoint.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Bezier Curve ���
            Vector3 newPosition = CalculateBezierPosition(t, startPos, controlPos, endPos);

            // ��ġ �̵�
            startObject.transform.position = newPosition;

            yield return null;
        }

        // ������ ��ġ ����
        startObject.transform.position = endPos;
    }

    // Bezier Curve ��� �Լ�
    private static Vector3 CalculateBezierPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
    }
}

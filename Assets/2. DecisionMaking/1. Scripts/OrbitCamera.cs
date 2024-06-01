using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target; // �����, ������ ������� ����� ������ ������
    public float distance = 10.0f; // ���������� �� ������ �� �����
    public float speed = 10.0f; // �������� �������� ������

    private float angle = 0.0f; // ������� ���� ������

    void Update()
    {
        if(target != null)
        {
            // ��������� ���� �� ������ ������� � ��������
            angle += speed * Time.deltaTime;

            // ��������� ��������� ������ � ��������� XZ
            float x = target.position.x + distance * Mathf.Cos(angle);
            float z = target.position.z + distance * Mathf.Sin(angle);

            // ��������� ������� ������
            transform.position = new Vector3(x, transform.position.y, z);

            // ������������ ������ ���, ����� ��� �������� �� ����
            transform.LookAt(target);
        }
    }
}

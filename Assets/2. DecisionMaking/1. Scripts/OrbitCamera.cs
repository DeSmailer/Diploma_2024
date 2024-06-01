using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target; // Точка, вокруг которой будет летать камера
    public float distance = 10.0f; // Расстояние от камеры до точки
    public float speed = 10.0f; // Скорость вращения камеры

    private float angle = 0.0f; // Текущий угол камеры

    void Update()
    {
        if(target != null)
        {
            // Обновляем угол на основе времени и скорости
            angle += speed * Time.deltaTime;

            // Вычисляем положение камеры в плоскости XZ
            float x = target.position.x + distance * Mathf.Cos(angle);
            float z = target.position.z + distance * Mathf.Sin(angle);

            // Обновляем позицию камеры
            transform.position = new Vector3(x, transform.position.y, z);

            // Поворачиваем камеру так, чтобы она смотрела на цель
            transform.LookAt(target);
        }
    }
}

using UnityEngine;

namespace DecisionMaking.Utils {
  public static class RandomVector {
    public static Vector3 GetRandomVector3(float minX, float maxX, float minY, float maxy, float minZ, float maxZ) {
      float x = Random.Range(minX, maxX);
      float y = Random.Range(minY, maxy);
      float z = Random.Range(minZ, maxZ);
      return new Vector3(x, y, z);
    }

    /// <summary>
    /// Получить случайную точку в торе
    /// </summary>
    /// <param name="center"></param>
    /// <param name="minorRadius">Радиус малого круга</param>
    /// <param name="majorRadius">Радиус большого круга</param>
    /// <returns></returns>
    public static Vector3 GetRandomPointInTorus(Vector3 center, float minorRadius, float majorRadius) {
      // Случайный угол для большого радиуса (θ)
      float angle1 = Random.Range(0f, Mathf.PI * 2f);

      // Случайный угол для малого радиуса (φ)
      float angle2 = Random.Range(0f, Mathf.PI * 2f);

      // Позиция на окружности большого радиуса
      float x1 = majorRadius * Mathf.Cos(angle1);
      float z1 = majorRadius * Mathf.Sin(angle1);

      // Смещение вдоль малой окружности
      float x2 = minorRadius * Mathf.Cos(angle2);
      float y2 = minorRadius * Mathf.Sin(angle2);

      // Координаты точки в торе относительно центра
      float x = center.x + x1 + x2 * Mathf.Cos(angle1);
      float y = center.y + y2;
      float z = center.z + z1 + x2 * Mathf.Sin(angle1);

      return new Vector3(x, y, z);
    }


    /// <summary>
    /// Получить случайную точку в кольце
    /// </summary>
    /// <param name="center"></param>
    /// <param name="innerRadius">Радиус малого круга</param>
    /// <param name="outerRadius">Радиус большого круга</param>
    /// <returns></returns>
    public static Vector3 GetRandomPointInRing(Vector3 center, float innerRadius, float outerRadius) {
      // Случайный угол
      float angle = Random.Range(0f, Mathf.PI * 2);

      // Случайное расстояние от центра между малым и большим радиусом
      float radius = Mathf.Sqrt(Random.Range(innerRadius * innerRadius, outerRadius * outerRadius));

      // Координаты точки в кольце
      float x = center.x + radius * Mathf.Cos(angle);
      float z = center.z + radius * Mathf.Sin(angle);

      return new Vector3(x, center.y, z);
    }
  }
}

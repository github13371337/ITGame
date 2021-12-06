using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void RotateToTarget(this Transform transform, Vector3 vector, float rotationSpeed, bool direction = false, bool lockX = true, bool lockZ = true)
    {
        Vector3 forward;
        if (!direction) forward = transform.DirectionToTarget(vector, !lockX);
        else forward = vector;
        Quaternion lookAt = Quaternion.LookRotation(forward, transform.up);
        Vector3 angles = Quaternion.Lerp(transform.rotation, lookAt, rotationSpeed).eulerAngles;
        if (lockX) angles.x = 0;
        if (lockZ) angles.z = 0;
        transform.eulerAngles = angles;
    }
    public static float AngleToTarget(this Transform transform, Vector3 vector, bool includeY, bool direction = false)
    {
        Vector3 mnp2;
        if (direction) mnp2 = vector;
        else mnp2 = transform.DirectionToTarget(vector, includeY);
        float m1 = transform.forward.x; float m2 = mnp2.x;
        float n1 = transform.forward.y; float n2 = mnp2.y;
        float p1 = transform.forward.z; float p2 = mnp2.z;

        float cos = (m1 * m2 + n1 * n2 + p1 * p2) / (Mathf.Sqrt(m1 * m1 + n1 * n1 + p1 * p1) * Mathf.Sqrt(m2 * m2 + n2 * n2 + p2 * p2));

        return Mathf.Acos(cos) * Mathf.Rad2Deg;
    }
    public static Vector3 DirectionToTarget(this Transform transform, Vector3 position, bool includeY)
    {
        return new Vector3
        (
            position.x - transform.position.x,
           (position.y - transform.position.y) * (includeY ? 1f : 0f),
            position.z - transform.position.z
        ).normalized;
    }

    public static T[] Copy<T>(this T[] array)
    {
        if (array != null)
        {
            T[] ret = new T[array.Length];
            for (int i = 0; i < array.Length; i++) ret.SetValue((T)array.GetValue(i), i);
            return ret;
        }
        return null;
    }
    public static bool Contains<T>(this T[] array, T item) => Array.FindIndex(array, (t) => t.Equals(item)) >= 0;
    public static T Find<T>(this T[] array, Predicate<T> match) => Array.Find(array, match);
    public static int FindIndex<T>(this T[] array, Predicate<T> match) => Array.FindIndex(array, match);
}

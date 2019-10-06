using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper{

    public static float AngleBetween(Vector2 A, Vector2 B)
    {
        A -= B;
        float angle = Mathf.Atan2(A.y, A.x);
        return angle * Mathf.Rad2Deg;
    }

    public static Vector2 RadianToVector2(float radian) {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 RadianToVector2(float radian, float length)
    {
        return RadianToVector2(radian) * length;
    }
    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
    public static Vector2 DegreeToVector2(float degree, float length)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad) * length;
    }

    public static Vector3 DirectionToRotation(Vector3 direction)
    {
        Vector3 rotation = Vector3.zero;
        rotation.x = getAngle(direction.z, direction.y);
        rotation.y = -getAngle(direction.x, direction.z);
        rotation.z = -getAngle(direction.x, direction.y);
        return rotation;
    }

    public static float getRadAngle(float x, float y) {
        return Mathf.Atan2(y, x);
    }

    public static float getRadAngle(Vector2 p_vector2) {
        return getRadAngle(p_vector2.x, p_vector2.y);
    }

    public static float getAngle(Vector2 p_vector2)
    {
        return getAngle(p_vector2.x, p_vector2.y);
    }

    public static float getAngle(float x, float y)
    {
        return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }

    public static Vector2 RotateVectorBy(Vector2 vec,float angle) {
        float vangle = getRadAngle(vec);
        return RadianToVector2(vangle + angle * Mathf.Deg2Rad) * vec.magnitude;
    }

    public static Vector2 RotateVectorBy2(Vector2 vec, float angle) {
        float vangle = getAngle(vec);
        return DegreeToVector2(vangle + angle);
    }

    public static float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x)); 
    }

    public static float FakeDSigmoid(float y)
    {
        //return Sigmoid(x) * (1 - Sigmoid(x));
        return y * (1 - y);
    }
}

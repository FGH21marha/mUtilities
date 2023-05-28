// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class mMath
{
    public static Vector3 Divide(this Vector3 value, Vector3 divider)
    {
        return new Vector3(value.x / divider.x, value.y / divider.y, value.z / divider.z);
    }
    public static Vector3 Round(this Vector3 value)
    {
        return new Vector3(Mathf.Round(value.x), Mathf.Round(value.y), Mathf.Round(value.z));
    }

    public static Vector2 SetMagnitude(this Vector2 value, float magnitude)
    {
        return value.normalized * magnitude;
    }
    public static Vector3 SetMagnitude(this Vector3 value, float magnitude)
    {
        return value.normalized * magnitude;
    }

    public static Vector3 Make3D(this Vector2 value)
    {
        return new Vector3(value.x, 0f, value.y);
    }
    public static Vector2 AsVector2(this Vector3 value)
    {
        return new Vector2(value.x, value.y);
    }




    public static float Remap(this float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return Lerp(newMin, newMax, InverseLerp(oldMin, oldMax, value));
    }
    public static float Remap01(this float value, float oldMin, float oldMax)
    {
        return InverseLerp(oldMin, oldMax, value);
    }
    public static float RemapClamped(this float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return Lerp(newMin, newMax, InverseLerp(oldMin, oldMax, value)).Clamp(newMin, newMax);
    }
    public static float Remap01Clamped(this float value, float oldMin, float oldMax)
    {
        return InverseLerp(oldMin, oldMax, value).Clamp01();
    }



    public static float Clamp(this float value, float min, float max)
    {
        if (value <= min)
            return min;

        if (value >= max)
            return max;

        return value;
    }
    public static float Clamp01(this float value)
    {
        if (value <= 0f)
            return 0f;

        if (value >= 1f)
            return 1f;

        return value;
    }



    public static float Lerp(float a, float b, float t)
    {
        return (1 - t) * a + b * t;
    }
    public static float InverseLerp(float a, float b, float t)
    {
        return (t - a) / (b - a);
}
    public static float LerpClamped(float a, float b, float t)
    {
        return Lerp(a,b,t).Clamp(a,b);
    }
    public static float InverseLerpClamped(float a, float b, float t)
    {
        return InverseLerp(a, b, t).Clamp(a, b);
    }




    public static float AngleFromVector(this Vector2 value)
    {
        return Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg;
    }
    public static Vector2 Vector2FromAngle(this float angle)
    {
        return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
    }

    public static Vector2 Lerp(this Vector2 value, Vector2 to, float t)
    {
        return (1 - t) * value + to * t;
    }
    public static Vector3 Lerp(this Vector3 value, Vector3 to, float t)
    {
        return (1 - t) * value + to * t;
    }

    public static Vector2 ReplaceX(this Vector2 value, float x)
    {
        return new Vector2(x, value.y);
    }
    public static Vector2 ReplaceY(this Vector2 value, float y)
    {
        return new Vector2(value.x, y);
    }

    public static Vector3 ReplaceX(this Vector3 value, float x)
    {
        return new Vector3(x, value.y, value.z);
    }
    public static Vector3 ReplaceY(this Vector3 value, float y)
    {
        return new Vector3(value.x, y, value.z);
    }
    public static Vector3 ReplaceZ(this Vector3 value, float z)
    {
        return new Vector3(value.x, value.y, z);
    }

    public static Vector2 RemoveX(this Vector2 value)
    {
        return new Vector2(0, value.y);
    }
    public static Vector2 RemoveY(this Vector2 value)
    {
        return new Vector2(value.x, 0);
    }

    public static Vector3 RemoveX(this Vector3 value)
    {
        return new Vector3(0, value.y, value.z);
    }
    public static Vector3 RemoveY(this Vector3 value)
    {
        return new Vector3(value.x, 0, value.z);
    }
    public static Vector3 RemoveZ(this Vector3 value)
    {
        return new Vector3(value.x, value.y, 0);
    }




    public static Quaternion Delta(this Quaternion a, Quaternion b)
    {
        return b * Quaternion.Inverse(a);
    }
    public static Quaternion Add(this Quaternion a, Quaternion b)
    {
        return b * a;
    }




    public static bool IsPrime(this float value)
    {
        int val = (int)Mathf.Round(value);

        if (val < 2) return false;

        for (int i = 2; i <= val.Sqrt(); i++)
            if (val % i == 0)
                return false;

        return true;
    }
    public static bool IsPrime(this int value)
    {
        if (value < 2) return false;

        for (int i = 2; i <= value.Sqrt(); i++)
            if (value % i == 0)
                return false;

        return true;
    }

    public static int Sqrt(this float value)
    {
        int val = (int)Mathf.Round(value);

        if (0 == val) return 0;

        int n = (val / 2) + 1;
        int n1 = (n + (val / n)) / 2;

        while (n1 < n)
        {
            n = n1;
            n1 = (n + (val / n)) / 2;
        }

        return n;
    }
    public static int Sqrt(this int value)
    {
        if (0 == value) return 0;

        int n = (value / 2) + 1;
        int n1 = (n + (value / n)) / 2;

        while (n1 < n)
        {
            n = n1;
            n1 = (n + (value / n)) / 2;
        }

        return n;
    }




    public static bool CheckLayer(Collision collision, LayerMask mask)
    {
        return mask == (mask | (1 << collision.gameObject.layer));
    }
    public static bool CheckDirection(Vector3 direction, Collision collision, out Vector3 impulse, float accuracy = 0.7f)
    {
        List<Vector3> velocities = new List<Vector3>();

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (Vector3.Dot(collision.contacts[i].normal, direction) > accuracy)
                velocities.Add(collision.relativeVelocity);
        }

        if (velocities.Count > 0)
        {
            velocities.Sort((x1, x2) => x1.magnitude.CompareTo(x2.magnitude));
            impulse = velocities.First();
            return true;
        }

        impulse = Vector3.zero;
        return false;
    }
    public static bool CheckDirection(Vector3 direction, Collision collision, float accuracy = 0.7f)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (Vector3.Dot(collision.contacts[i].normal, direction) > accuracy)
                return true;
        }

        return false;
    }
}

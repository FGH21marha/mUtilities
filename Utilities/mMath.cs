// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

using UnityEngine;

public static class mMath
{
    public static Vector3 Mult(this Vector3 value, Vector3 multiplier)
    {
        return new Vector3(value.x * multiplier.x, value.y * multiplier.y, value.z * multiplier.z);
    }
    public static Vector3 Div(this Vector3 value, Vector3 divider)
    {
        return new Vector3(value.x / divider.x, value.y / divider.y, value.z / divider.z);
    }

    public static Vector2 SetMagnitude(this Vector2 value, float magnitude)
    {
        return value.normalized * magnitude;
    }
    public static Vector2 LimitMagnitude(this Vector2 value, float limit)
    {
        if (value.magnitude >= limit) return value.normalized * limit;
        else return value;
    }
    public static Vector3 SetMagnitude(this Vector3 value, float magnitude)
    {
        return value.normalized * magnitude;
    }
    public static Vector3 LimitMagnitude(this Vector3 value, float limit)
    {
        if (value.magnitude >= limit) return value.normalized * limit;
        else return value;
    }

    public static float Remap(this float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return Lerp(InverseLerp(value, oldMin, oldMax), newMin, newMax);
    }
    public static float Lerp(float value, float a, float b)
    {
        return (1 - value) * a + b * value;
    }
    public static float InverseLerp(float value, float a, float b)
    {
        return (value - a) / (b - a);
    }

    public static float AngleFromVector(this Vector2 value)
    {
        return Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg;
    }
    public static Vector2 Vector2FromAngle (this float angle)
    {
        return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
    }

    public static Vector2 Lerp(this Vector2 value, Vector2 a, Vector2 b, float t)
    {
        return (1 - t) * a + b * value;
    }
    public static Vector3 Lerp(this Vector3 value, Vector3 a, Vector3 b, float t)
    {
        return (1 - t) * a + b.Mult(value);
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

    public static bool IsPrime(this float value)
    {
        int val = (int)Mathf.Round(value);

        if (val < 2) return false;

        for (int i = 2; i <= val.Sqrt(); i++)
            if(val % i == 0)
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
}

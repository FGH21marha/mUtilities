// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

public static class mArray
{
    public static T[] Add<T>(this T[] array, T value)
    {
        T[] newArray = new T[array.Length + 1];

        for (int i = 0; i < newArray.Length; i++)
        {
            if (i < array.Length)
                newArray[i] = array[i];
            else
                newArray[i] = value;
        }

        return newArray;
    }
    public static T[] Add<T>(this T[] array, T value, out T[] newArray)
    {
        newArray = new T[array.Length + 1];

        for (int i = 0; i < newArray.Length; i++)
        {
            if (i < array.Length)
                newArray[i] = array[i];
            else
                newArray[i] = value;
        }

        return newArray;
    }
    public static T[] Add<T>(ref T[] array, T value) => array = array.Add(value);

    public static T[] Remove<T>(this T[] array, int index)
    {
        T[] newArray = new T[array.Length - 1];

        int x = 0;

        for (int i = 0; i < newArray.Length; i++)
        {
            if (i == index)
                x = 1;

            newArray[i] = array[i + x];
        }

        return newArray;
    }
    public static T[] Remove<T>(this T[] array, int index, out T[] newArray)
    {
        newArray = new T[array.Length - 1];

        int x = 0;

        for (int i = 0; i < newArray.Length; i++)
        {
            if (i == index)
                x = 1;

            newArray[i] = array[i + x];
        }


        return newArray;
    }
    public static T[] Remove<T>(ref T[] array, int index) => array = array.Remove(index);

    public static T[] Swap<T>(this T[] array, int a, int b)
    {
        var temp = array[a];
        array[a] = array[b];
        array[b] = temp;

        return array;
    }
    public static void Swap<T>(ref T[] array, int a, int b)
    {
        var temp = array[a];
        array[a] = array[b];
        array[b] = temp;
    }

    public static T[] Insert<T>(this T[] array, int index, T value)
    {
        T[] newArray = new T[array.Length + 1];

        int x = 0;

        for (int i = 0; i < newArray.Length; i++)
        {
            if (i == index)
                x = 1;

            newArray[i] = array[i - x];
            newArray[index] = value;
        }

        return newArray;
    }
    public static T[] Insert<T>(this T[] array, int index, T value, out T[] newArray)
    {
        newArray = new T[array.Length + 1];

        int x = 0;

        for (int i = 0; i < newArray.Length; i++)
        {
            if (i == index)
                x = 1;

            newArray[i] = array[i - x];
            newArray[index] = value;
        }

        return newArray;
    }
    public static T[] Insert<T>(ref T[] array, int index, T value) => array = array.Insert(index, value);
}

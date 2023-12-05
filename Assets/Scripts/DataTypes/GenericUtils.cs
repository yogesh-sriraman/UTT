public static class GenericUtils
{
    public static T[] GetNewArrayCopy<T>(T[] src, int size_of_T)
    {
        if (!(size_of_T > 0))
        {
            return null;
        }
        T[] dst = new T[src.Length];
        int len = src.Length * size_of_T;
        System.Buffer.BlockCopy(src, 0, dst, 0, len);
        return dst;
    }

    public static void PopulateDefaultArray<T>(this T[] arr, int size)
        where T : new()
    {

        for (int i = 0; i < size; i++)
        {
            arr[i] = new T();
        }
    }
}
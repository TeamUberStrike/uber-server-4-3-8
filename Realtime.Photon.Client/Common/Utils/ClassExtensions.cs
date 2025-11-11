using System.Collections.Generic;

public static class QueueExtention
{
    public static void Enqueue<T>(this Queue<T> q, T t, int maxCount)
    {
        while (q.Count > maxCount - 1)
        {
            q.Dequeue();
        }

        q.Enqueue(t);
    }
}
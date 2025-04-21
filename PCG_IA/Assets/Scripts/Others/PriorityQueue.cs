using System.Collections.Generic;

// Al chile no sé, se lo pedí a chat porque no sé mucho de listas
public class PriorityQueue<T>
{
    private List<(T item, float priority)> elements = new();

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
        elements.Sort((a, b) => a.priority.CompareTo(b.priority)); // Orden ascendente
    }

    public T Dequeue()
    {
        if (elements.Count == 0) return default;
        var item = elements[0].item;
        elements.RemoveAt(0);
        return item;
    }

    public T Peek()
    {
        return elements.Count > 0 ? elements[0].item : default;
    }

    public float PeekPriority()
    {
        return elements.Count > 0 ? elements[0].priority : float.MaxValue;
    }

    public int Count => elements.Count;
}

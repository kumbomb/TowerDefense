using System;
using System.Collections.Generic;

/// <summary>
/// 효율적인 Priority Queue 구현 (Min-Heap)
/// </summary>
/// <typeparam name="T"></typeparam>
public class PriorityQueue<T>
{
    private List<(T item, float priority)> elements = new List<(T, float)>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
        HeapifyUp(elements.Count - 1);
    }

    public T Dequeue()
    {
        if (elements.Count == 0)
            throw new InvalidOperationException("Priority Queue is empty.");

        T bestItem = elements[0].item;
        elements[0] = elements[elements.Count - 1];
        elements.RemoveAt(elements.Count - 1);
        HeapifyDown(0);
        return bestItem;
    }

    public bool Contains(T item)
    {
        foreach (var element in elements)
        {
            if (EqualityComparer<T>.Default.Equals(element.item, item))
                return true;
        }
        return false;
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (elements[index].priority < elements[parent].priority)
            {
                Swap(index, parent);
                index = parent;
            }
            else
            {
                break;
            }
        }
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = elements.Count - 1;
        while (true)
        {
            int leftChild = 2 * index + 1;
            int rightChild = 2 * index + 2;
            int smallest = index;

            if (leftChild <= lastIndex && elements[leftChild].priority < elements[smallest].priority)
            {
                smallest = leftChild;
            }

            if (rightChild <= lastIndex && elements[rightChild].priority < elements[smallest].priority)
            {
                smallest = rightChild;
            }

            if (smallest != index)
            {
                Swap(index, smallest);
                index = smallest;
            }
            else
            {
                break;
            }
        }
    }

    private void Swap(int i, int j)
    {
        var temp = elements[i];
        elements[i] = elements[j];
        elements[j] = temp;
    }
}

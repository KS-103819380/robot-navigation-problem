namespace Robot_Navigation_Problem
{
    //This class is used to store the nodes in the priority queue, the PriorityQueue class provided in the .NET framework exhibits strange behavior, and is unsuitable to be used in this use case
    //More detailed about this issue can be found in the report
    public class PriorityQueue<T>
    {
        private List<(T, int)> elements = new List<(T, int)>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(T item, int priority)
        {
            elements.Add((item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Item2 < elements[bestIndex].Item2)
                {
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }

        public IEnumerable<T> UnorderedItems
        {
            get
            {
                foreach (var element in elements)
                {
                    yield return element.Item1;
                }
            }
        }
    }
}

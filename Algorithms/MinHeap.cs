// Proje Ekibi:
// KAAN ORHAN 
// NUMAN KARABUGA 
// AHMET DEMIRBILEK 
// BUSRA DERELI


namespace PCBBaglantiAgiOptimizasyonu
{
    public class MinHeap
    {
        private Edge[] heapArray;
        private int size;
        private int capacity;

        public MinHeap(int capacity = 1000)
        {
            this.capacity = capacity;
            this.heapArray = new Edge[this.capacity];
            this.size = 0;
        }

        private void Resize()
        {
            capacity *= 2;
            Edge[] newArray = new Edge[capacity];
            for (int i = 0; i < size; i++)
                newArray[i] = heapArray[i];
            heapArray = newArray;
        }

        private int GetLeftChildIndex(int index) { return index * 2 + 1; }
        private int GetRightChildIndex(int index) { return index * 2 + 2; }
        private int GetParentIndex(int index) { return (index - 1) / 2; }

        public void Insert(Edge newEdge)
        {
            if (size == capacity) Resize();
            heapArray[size] = newEdge;
            HeapifyUp(size);
            size++;
        }

        private void HeapifyUp(int currentIndex)
        {
            int parentIndex = GetParentIndex(currentIndex);
            while (currentIndex > 0 && heapArray[currentIndex].Weight < heapArray[parentIndex].Weight)
            {
                Edge temp = heapArray[currentIndex];
                heapArray[currentIndex] = heapArray[parentIndex];
                heapArray[parentIndex] = temp;
                currentIndex = parentIndex;
                parentIndex = GetParentIndex(currentIndex);
            }
        }

        public Edge ExtractMin()
        {
            if (size == 0) return null;

            Edge minEdge = heapArray[0];
            heapArray[0] = heapArray[size - 1];
            heapArray[size - 1] = null;
            size--;
            HeapifyDown(0);

            return minEdge;
        }

        private void HeapifyDown(int currentIndex)
        {
            while (true)
            {
                int leftChildIndex = GetLeftChildIndex(currentIndex);
                int rightChildIndex = GetRightChildIndex(currentIndex);
                int smallestIndex = currentIndex;

                if (leftChildIndex < size && heapArray[leftChildIndex].Weight < heapArray[smallestIndex].Weight)
                {
                    smallestIndex = leftChildIndex;
                }
                if (rightChildIndex < size && heapArray[rightChildIndex].Weight < heapArray[smallestIndex].Weight)
                {
                    smallestIndex = rightChildIndex;
                }
                if (smallestIndex == currentIndex)
                {
                    break;
                }

                Edge temp = heapArray[currentIndex];
                heapArray[currentIndex] = heapArray[smallestIndex];
                heapArray[smallestIndex] = temp;
                currentIndex = smallestIndex;
            }
        }
    }
}

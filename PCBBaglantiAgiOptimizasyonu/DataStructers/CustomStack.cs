namespace PCBBaglantiAgiOptimizasyonu
{
    public class CustomStack
    {
        // Stack in sadece kendi icinde kullanacagi dugum yapisi
        private class StackNode
        {
            public Node Data;       // Dugumun icindeki veri
            public StackNode Next;  // Altindaki dugum

            public StackNode(Node data)
            {
                Data = data;
                Next = null;
            }
        }

        private StackNode top; // Stackin tepesi (son giren, ilk cikan - LIFO)

        // Stack e eleman ekle (Tepeden gir)
        public void Push(Node data)
        {
            StackNode newNode = new StackNode(data);
            newNode.Next = top;
            top = newNode;
        }

        // Stack ten eleman cikar (Tepeden al - LIFO)
        public Node Pop()
        {
            if (top == null) return null;

            Node temp = top.Data;
            top = top.Next;
            return temp;
        }

        // Tepeye bak ama cikarma (Sadece oku)
        public Node Peek()
        {
            if (top == null) return null;
            return top.Data;
        }

        // Stack bos mu kontrolu (DFS dongusu icin gerekli)
        public bool IsEmpty()
        {
            return top == null;
        }
    }
}

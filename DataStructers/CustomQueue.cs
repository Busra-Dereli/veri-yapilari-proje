// Proje Ekibi:
// Busra Dereli

namespace PCBBaglantiAgiOptimizasyonu
{
    public class CustomQueue 
    {
        // Kuyrugun sadece kendi icinde kullanacagi vagon yapisi
        private class QueueNode 
        {
            public Node Data;       // Vagonun icindeki yuk (Anakart bileseni)
            public QueueNode Next;  // Arkadaki vagon

            public QueueNode(Node data) 
            {
                Data = data;
                Next = null;
            }
        }

        private QueueNode front; // Kuyrugun basi (Islemi bitip cikacak olan)
        private QueueNode rear;  // Kuyrugun sonu (Yeni gelenin eklenecegi yer)

        // Kuyruga yeni eleman ekleme (Arkadan siraya girme)
        public void Enqueue(Node data) 
        {
            QueueNode newNode = new QueueNode(data);
            
            if (rear == null) // Eger kuyruk tamamen bossa
            {
                front = rear = newNode;
                return;
            }
            
            // Son elemanin arkasina yenisini bagla ve yeni son eleman o olsun
            rear.Next = newNode;
            rear = newNode;
        }

        // Kuyruktan eleman cikarma (Onden sirasi geleni alma)
        public Node Dequeue() 
        {
            if (front == null) return null; // Kuyruk bossa
            
            Node temp = front.Data; // Cikacak elemanin verisini yedekle
            front = front.Next;     // Kuyrugun basini bir arkadakine kaydir
            
            if (front == null) rear = null; // Son eleman da ciktiysa kuyrugu tamamen sifirla
            
            return temp;
        }

        // Kuyruk bos mu kontrolu (BFS in dongusunu bitirmek icin gerekli)
        public bool IsEmpty() 
        {
            return front == null;
        }
    }
}

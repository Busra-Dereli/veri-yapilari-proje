namespace PCBBaglantiAgiOptimizasyonu
{
    public class CustomQueue 
    {
        // Kuyruğun sadece kendi içinde kullanacağı vagon yapısı
        private class QueueNode 
        {
            public Node Data;       // Vagonun içindeki yük (Anakart bileşeni)
            public QueueNode Next;  // Arkadaki vagon

            public QueueNode(Node data) 
            {
                Data = data;
                Next = null;
            }
        }

        private QueueNode front; // Kuyruğun başı (İşlemi bitip çıkacak olan)
        private QueueNode rear;  // Kuyruğun sonu (Yeni gelenin ekleneceği yer)

        // Kuyruğa yeni eleman ekleme (Arkadan sıraya girme)
        public void Enqueue(Node data) 
        {
            QueueNode newNode = new QueueNode(data);
            
            if (rear == null) // Eğer kuyruk tamamen boşsa
            {
                front = rear = newNode;
                return;
            }
            
            // Son elemanın arkasına yenisini bağla ve yeni son eleman o olsun
            rear.Next = newNode;
            rear = newNode;
        }

        // Kuyruktan eleman çıkarma (Önden sırası geleni alma)
        public Node Dequeue() 
        {
            if (front == null) return null; // Kuyruk boşsa
            
            Node temp = front.Data; // Çıkacak elemanın verisini yedekle
            front = front.Next;     // Kuyruğun başını bir arkadakine kaydır
            
            if (front == null) rear = null; // Son eleman da çıktıysa kuyruğu tamamen sıfırla
            
            return temp;
        }

        // Kuyruk boş mu kontrolü (BFS'nin döngüsünü bitirmek için gerekli)
        public bool IsEmpty() 
        {
            return front == null;
        }
    }
}
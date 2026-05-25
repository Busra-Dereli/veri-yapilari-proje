namespace PCBBaglantiAgiOptimizasyonu
{
    public class Graph 
    {
        // Anakart üzerindeki tüm bileşenleri tutacağımız temel dizi
        public Node[] Components; 
        public int ComponentCount; // Şu an anakartta kaç parça var?

        // Constructor: Anakartın kapasitesini baştan belirliyoruz
        public Graph(int capacity = 100) 
        {
            Components = new Node[capacity];
            ComponentCount = 0;
        }

        // Anakart üzerine yeni bir çip/bileşen lehimleme (ekleme) işlemi
        public void AddNode(Node newNode) 
        {
            if (ComponentCount < Components.Length) 
            {
                Components[ComponentCount] = newNode;
                ComponentCount++;
            }
            else 
            {
                Console.WriteLine("Hata: Anakart kapasitesi dolu!");
            }
        }

        // İsmini bildiğimiz bir bileşeni anakartta bulmak için arama metodu
        public Node FindNode(string id) 
        {
            for (int i = 0; i < ComponentCount; i++) 
            {
                if (Components[i].Id == id) 
                {
                    return Components[i];
                }
            }
            return null; // Bileşen bulunamazsa
        }
    }
}
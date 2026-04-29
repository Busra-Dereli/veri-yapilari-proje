# Veri Yapıları Projesi

Faz 1: Temel Veri Yapıları ve Graf Altyapısı:

Projenin çekirdek mimarisini oluşturacak olan temel veri yapıları üzerinde çalışmalara başlanmıştır. Standart kütüphaneler kullanılmadan, sistemin bellekte verimli çalışmasını sağlayacak sınıfların (class) iskeletleri araştırılıp oluşturulmaktadır.

Mevcut Durum ve Alınan Kararlar:

Graf Modellemesi: PCB üzerindeki bileşenlerin birbirine bağlanması sürecini simüle etmek için "Komşuluk Listesi" yaklaşımı tercih edilmiştir. Bu yapı, bellek ve zaman karmaşıklığı optimizasyonu göz önünde bulundurularak Graph sınıfı altında sıfırdan geliştirilmektedir.

Dolaşım Yapıları: Graf üzerindeki erişilebilirlik kontrollerini yapabilmek adına, bağlı liste (linked list) mantığına dayanan özel CustomQueue (Kuyruk) ve CustomStack (Yığıt) sınıflarının tasarımlarına başlanmıştır.

Algoritmik Altyapı: Başlangıç düğüm bağlantılarının doğrulanması için DFS (Depth-First Search) ve BFS (Breadth-First Search) algoritmalarının pseudo-kodları çıkarılmış, sisteme entegrasyonu için ön hazırlıklar tamamlanmıştır.

Sonraki Adımlar: Node ve Edge sınıflarının kodlanması ve bu yapıların GitHub üzerinden kendi dalımdan (branch) ana dala (master) aktarılması (Pull Request).

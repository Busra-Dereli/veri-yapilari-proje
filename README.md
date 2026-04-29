# Veri Yapıları Projesi

# PCB Bağlantı Ağı Optimizasyon Simülasyonu

Bu proje, bir baskı devre kartı (PCB) üzerindeki bileşenlerin (direnç, kapasitör, entegre vb.) en kısa ve en verimli şekilde birbirine bağlanmasını sağlayan bir simülasyon sistemidir. Proje, mikroservis mimarisi kullanılarak asenkron ve ölçeklenebilir bir yapıda tasarlanmıştır.

## 🚀 Proje Durumu: Ara Rapor Aşaması
Projemiz, ekip üyelerinin ortak teknik tartışmalarını yürüttüğü, görev dağılımının netleştiği ve aktif versiyon kontrolü (Git) süreçlerinin başlatıldığı seviyeye ulaşmıştır.

## 📈 Git Akış Şeması (Workflow)
Proje süreci, ara rapor dökümanındaki rehbere tam uyumlu şekilde GitHub üzerinden yönetilmektedir:
* **Issue Tracking:** Her teknik zorluk ve özellik (feature) bir "Issue" olarak açılır ve ekip içinde tartışılır.
* **Branching:** Her üye kendi görevini feature/ ön ekiyle açtığı bağımsız dallarda geliştirir.
* **Pull Requests:** Kodlar ana branch'e birleştirilmeden önce Review sürecinden geçer ve dokümante edilir.

## 📝 Aktif Tartışmalar ve Güncel Kararlar
* **[ISSUE #1]** Veri yapısı olarak bellek verimliliği için "Adjacency List" (Komşuluk Listesi) kullanımı kararlaştırıldı.
* **[ISSUE #2]** Minumum yol-maliyet hesaplama için "Prim Algoritması"nı seçtik bundan dolayı da Min-Heap veri yapısı kullanılmasına karar verilmişir.
* **[ISSUE #3]** Dinamik güncelleme için 20-100 düğüm aralığında performans test senaryoları oluşturulması planlanıyor.

## 🧠 Algoritma ve Optimizasyon Altyapısı
Projenin kalbini oluşturan bağlantı maliyeti optimizasyonu, hazır kütüphanelerden tamamen bağımsız, özelleştirilmiş algoritmik yapılarla inşa edilmektedir. Bu kapsamda:
* **Özelleştirilmiş Veri Yapıları:** Arama ve minimum maliyetli kenar seçimi operasyonlarını logaritmik sürede gerçekleştirebilmek için **Min-Heap (Priority Queue)** yapısı sıfırdan tasarlanıp kodlanmaktadır.
* **MST İnşası ve Analiz:** Ağ üzerindeki düğümleri döngüye yer vermeden (cycle-free) en düşük maliyetle bağlamak için **Prim Algoritması** kullanılmaktadır. Geliştirilen bu çekirdek yapının Zaman (Time) ve Uzay (Space) karmaşıklıkları matematiksel olarak analiz edilip projenin ilerleyen fazlarında raporlanacaktır.


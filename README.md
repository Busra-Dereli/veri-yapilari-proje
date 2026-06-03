# 🚀 PCB Bağlantı Ağı Optimizasyonu

> Bu proje, anakart (PCB) üzerindeki bileşenler arasında **döngüsüz ve minimum maliyetli bağlantı ağı** oluşturmayı amaçlayan bir graf optimizasyon simülasyonudur. Prim Algoritması ve sıfırdan yazılmış veri yapıları kullanılarak geliştirilmiştir.

---

## 👥 Ekip Bilgileri

| İsim | Öğrenci No |
|------|------------|
| Kaan Orhan | 032490039 |
| Numan Karabuga | 032490044 |
| Ahmet Demirbilek | 032490045 |
| Busra Dereli | 032490047 |

> **Proje Durumu:** ✅ Tam fonksiyonel şekilde tamamlanmıştır.

---

## ⚙️ Sistem Mimarisi ve Teknolojik Altyapı

### 🔄 Katmanlı Mimari ve Eşzamanlılık (Thread-Safe / Async)

- **Modüler Tasarım:** Veri yapıları, algoritmalar ve arayüz katmanları birbirinden bağımsız modüller halinde geliştirilmiştir.
- **UI Thread Güvencesi:** Prim Algoritması, kullanıcı arayüzünü dondurmamak için `async/await` (`Task.Run`) mekanizmasıyla ana thread'den bağımsız çalıştırılmaktadır.

### 🐳 Docker ile Kurulum

Tüm sistem, tek bir komutla ortam bağımlılığı olmaksızın ayağa kaldırılabilir:

```bash
docker-compose up --build
```

`Dockerfile` ve `docker-compose.yml` dosyaları tüm bağımlılıkları otomatik olarak çözümler.

### 🌿 Git Workflow & Versiyon Kontrolü

Geliştirme süreci branch ve Pull Request (PR) mekanizmalarıyla yürütülmüştür. `master/main` dalına doğrudan müdahale edilmemiştir.

---

## 🏗️ Çekirdek Veri Yapıları

> **Not:** Tüm temel veri yapıları `System.Collections.Generic` gibi hazır kütüphaneler kullanılmadan, **işaretçilerle (pointer) sıfırdan** implemente edilmiştir.

### 📐 Sınıf (UML) Yapısı

| Sınıf | Açıklama |
|-------|----------|
| `Graph` | Tüm sistemi kapsar; anakart üzerindeki `Node` nesnelerini bir dizi (array) içinde tutar. |
| `Node` | Direnç, kapasitör veya entegre gibi fiziksel bileşenleri temsil eder. Kenarları bağlı liste mantığıyla saklar. |
| `Edge` | İki `Node` arasındaki elektriksel bağlantıyı ve ağırlığını (maliyet/mesafe) tutar. |
| `CustomStack` & `CustomQueue` | İşaretçilerle yazılmış dinamik veri yapılarıdır. |
| `MinHeap` & `PrimAlgorithm` | Projenin çekirdek optimizasyon motorudur; Minimum Spanning Tree hesaplar. |

---

## 📊 Algoritma Karmaşıklığı Analizi

### ⏳ Zaman Karmaşıklığı (Time Complexity)

| Metot / İşlem | Zaman Karmaşıklığı (Big-O) | Algoritmik Açıklama |
|---------------|---------------------------|----------------------|
| `Graph.AddNode` | O(1) | Diziye doğrudan indeks ile atama yapılır. |
| `Graph.FindNode` | O(V) | Benzersiz ID için doğrusal arama yapılır. |
| `Node.AddEdge` | O(E) | Bağlı listenin sonuna ulaşmak için liste taranır. |
| `CustomQueue` & `CustomStack` | O(1) | İşaretçilerle doğrudan ekleme/çıkarma yapılır. |
| `Min-Heap` (Insert/ExtractMin) | O(log V) | Heapify işlemleri ağaç yüksekliği kadar sürer. |
| **Prim Algoritması** | **O(E log V)** | Her kenar değerlendirilir, Min-Heap işlemine tabi tutulur. |

### 💾 Uzay Karmaşıklığı (Space Complexity)

| Yapı | Karmaşıklık |
|------|-------------|
| Komşuluk Listesi (Adjacency List) | O(V + E) |
| Min-Heap Kapasitesi | O(E) |
| Ziyaret Takip Dizisi (`IsVisited`) | O(V) |
| **Toplam** | **O(V + E)** |

### 🛠️ Mühendislik ve Güvenlik

- **Kısa Devre Engelleme:** Prim algoritması, PCB üzerinde döngü içermeyen en kısa bağlantıyı (MST) garanti eder.
- **Elektriksel Güvenlik:** `IsVisited` bayrağı sayesinde graf üzerinde döngü (cycle) oluşumu engellenerek **elektriksel kısa devre riskleri tamamen ortadan kaldırılmıştır**.

---

## 🖥️ Dinamik Kullanıcı Arayüzü ve Görselleştirme

- **Mouse ile Dinamik Etkileşim:** Simülasyon ekranına tıklandığında o koordinatta anında yeni bir `Node` oluşturulur.
- **Anlık İstatistik Paneli:** Yeni düğüm eklendiğinde sol paneldeki "Toplam Düğüm Sayısı" ve "Toplam Kenar Sayısı" asenkron olarak güncellenir.
- **Algoritmik Animasyon:** Prim algoritması tetiklendiğinde seçilen MST kenarları ayırt edici renklerle animasyonlu olarak çizilir.
- **Renk ve State Temizliği:** BFS/DFS sonrası Prim'e geçildiğinde eski dolaşım renkleri temizlenerek state karmaşası önlenir.
- **Sentetik Test Verisi:** Sistem, **20 ile 100 düğüm** arasında farklı ağırlıklı rastgele topolojiler üretilerek test edilmiştir.

---

## 🤖 GenAI Prompt Dökümü

Projede kullanılan yapay zeka destekli geliştirme sürecine ait promptlar:

1. **Mouse Olayları:** Simülasyon ekranında mouse tıklamasıyla node oluşturma, komşulara rastgele bağlama ve istatistik panelini güncelleme mekanizması.
2. **MinHeap Geliştirme:** C# ile sıfırdan `MinHeap` tasarımı; `Insert` ve `HeapifyUp` metotlarındaki yazım hataları ve indeks güncelleme eksiklikleri giderimi.
3. **Veri Yapısı Seçimi:** Komşuluk Listesi vs. Komşuluk Matrisi karşılaştırması; standart kütüphane kullanmadan bağlı liste mantığıyla sıfırdan implementasyon.
4. **Renk/State Temizliği:** BFS/DFS sonrası Prim'e geçildiğinde ekranda kalan eski dolaşım renklerinin temizlenmesi.
5. **Proje Eksik Analizi:** Değerlendirme kriterleri üzerinden projede eksik kalan noktaların tespiti.
6. **Docker Konfigürasyonu:** `Dockerfile` ve `docker-compose.yaml` dosyalarının hazırlanması.
7. **Sentetik Test Verisi:** 20–100 arası farklı ağırlıklarda düğüm oluşturma mekanizmasının eklenmesi.
8. **Thread Safety:** Prim temizleyicilerinin UI thread'inden `Task.Run` ile ayrı thread'e taşınması.

---

## 📺 Proje Demo Videosu

Sistemin Docker üzerinde ayağa kalkışını, dinamik düğüm ekleme/silme işlemlerini ve core kod analizini içeren demo videoya aşağıdan ulaşabilirsiniz:

🔗 **[Proje Demo Videosu](https://github.com/Busra-Dereli/veri-yapilari-proje)**

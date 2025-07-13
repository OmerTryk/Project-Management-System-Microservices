# 🛠️ Proje Yönetim Sistemi - Mikroservis Tabanlı Uygulama

Bu proje, görev yönetimini daha etkili hâle getirmek amacıyla geliştirilmiş, **ASP.NET 9.0** ile yazılmış bir **mikroservis mimarisine sahip** proje yönetim sistemidir.

## 🎯 Projenin Amacı

Proje, benzer görevlere sahip kullanıcıların performanslarını **görev tamamlama sürelerine göre puanlandırarak** karşılaştırmalı bir analiz sağlar. Böylece ekip içi verimlilik ve üretkenlik artırılabilir.

---

## ⚙️ Kullanılan Teknolojiler

- **Backend**
  - ASP.NET 9.0
  - Entity Framework Core
  - FluentValidation
  - MassTransit (RabbitMQ için)
  - JWT Authentication
  - Swagger (API dokümantasyonu)
  - SQL Server (veri tabanı)

- **Frontend**:
  - ASP.NET Razor Pages
---

## 🧩 Mikroservisler

Aşağıdaki servisler ayrı ayrı projelendirilmiş ve konteynerleştirilmiştir:

- **User Service** → Kullanıcı yönetimi ve kimlik doğrulama  
- **Project Service** → Proje oluşturma
- **Score Service** → Görev süresi ve başarıya göre puan hesaplama  
- **Notification Service** → E-posta veya uygulama içi bildirimler  
- **Task Services** → Görev oluşturma, atama

---

## 📊 Puanlama Sistemi Nasıl Çalışır?

1. Görev, kullanıcıya atandığında süre sayacı başlar.
2. Kullanıcı görevi tamamladığında süre hesaplanır.
3. Belirlenen eşik değerlerine göre puan verilir:
   - ⏱️ Kısa sürede tamamlanan görev → Daha yüksek puan
   - ⌛ Süresi aşan görev → Düşük puan
4. Kullanıcının toplam başarı skoru profiline yansıtılır.

<img width="1365" height="631" alt="Ekran görüntüsü 2025-07-13 182828" src="https://github.com/user-attachments/assets/939e715f-87b5-4501-9ff8-9ee771f6346e" />
<img width="1365" height="630" alt="Ekran görüntüsü 2025-07-13 183645" src="https://github.com/user-attachments/assets/ae39d817-f157-4e45-af55-7ebd187cb5be" />
<img width="1345" height="626" alt="Ekran görüntüsü 2025-07-13 184823" src="https://github.com/user-attachments/assets/335d8e4e-d00e-4ced-b8c3-1fffa6872a87" />
<img width="1365" height="634" alt="Ekran görüntüsü 2025-07-13 183221" src="https://github.com/user-attachments/assets/0cdad6ae-54c8-4b52-8208-45ecf43faf14" />
<img width="1365" height="631" alt="Ekran görüntüsü 2025-07-13 183207" src="https://github.com/user-attachments/assets/4f6b9334-ae62-4179-890a-2d46a10e8495" />
<img width="1352" height="627" alt="Ekran görüntüsü 2025-07-13 182136" src="https://github.com/user-attachments/assets/b5d99a7f-e920-4de0-af02-a507e2f11692" />
<img width="1365" height="623" alt="Ekran görüntüsü 2025-07-13 184739" src="https://github.com/user-attachments/assets/50af10cc-5d50-4082-9a76-e91edbf99b6f" />


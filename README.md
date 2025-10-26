<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Ürün Yönetim Sistemi / Product Management System</title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
            color: #333;
        }
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
            background-color: white;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }
        header {
            text-align: center;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 40px 20px;
            border-radius: 10px 10px 0 0;
        }
        header h1 {
            margin: 0;
            font-size: 2.5em;
        }
        .download-section {
            text-align: center;
            padding: 20px;
            background-color: #e8f5e8;
            border-radius: 5px;
            margin: 20px 0;
        }
        .download-btn {
            display: inline-block;
            background-color: #4CAF50;
            color: white;
            padding: 12px 24px;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
            transition: background-color 0.3s;
        }
        .download-btn:hover {
            background-color: #45a049;
        }
        .features {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            margin: 30px 0;
        }
        .feature-box {
            background-color: #f9f9f9;
            padding: 20px;
            border-radius: 8px;
            border-left: 4px solid #667eea;
        }
        .screenshots {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin: 40px 0;
        }
        .screenshot-item {
            text-align: center;
            background-color: #fff;
            padding: 15px;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            transition: transform 0.3s;
        }
        .screenshot-item:hover {
            transform: translateY(-5px);
        }
        .screenshot-item img {
            max-width: 100%;
            height: auto;
            border-radius: 5px;
        }
        .screenshot-item h3 {
            margin: 10px 0 5px;
            color: #667eea;
        }
        footer {
            text-align: center;
            padding: 20px;
            background-color: #333;
            color: white;
            margin-top: 40px;
            border-radius: 0 0 10px 10px;
        }
        @media (max-width: 768px) {
            .features {
                grid-template-columns: 1fr;
            }
            .screenshots {
                grid-template-columns: 1fr;
            }
        }
    </style>
</head>
<body>
    <div class="container">
        <header>
            <h1>Ürün Yönetim Sistemi / Product Management System</h1>
            <p><strong>Ürün Yönetim Sistemi</strong>, C# ve Access tabanlı kapsamlı bir işletme yönetim yazılımıdır. Sistem, stok yönetimi, satış takibi, finansal analiz ve raporlama süreçlerini tek bir platformda birleştirerek, işletmelere verimli ve güvenilir bir yönetim deneyimi sunar.</p>
            <p><strong>Product Management System</strong> is a comprehensive business management software developed in C# with an Access database. The system integrates inventory management, sales tracking, financial analysis, and reporting processes into a single platform, providing businesses with an efficient and reliable management experience.</p>
        </header>

        <div class="download-section">
            <h2>📦 Kurulum dosyası (.exe) / Installation File (.exe)</h2>
            <a href="https://drive.google.com/file/d/1cAoHV6GR8eTbx1QWRXVKFuZVp0RTCMYH/view?usp=drive_link" class="download-btn" target="_blank">İndir / Download</a>
        </div>

        <section class="features">
            <div class="feature-box">
                <h2>🔧 Temel Özellikler / Core Features</h2>
                <ul>
                    <li>fiş basımı (termal yazıcı desteği) / receipt printing (with thermal printer support)</li>
                    <li>stok kontrolü / inventory control</li>
                    <li>barkod oluşturma / barcode generation</li>
                    <li>Excel entegrasyonu / Excel integration</li>
                    <li>PDF raporlama / PDF reporting</li>
                    <li>finansal analiz / financial analysis</li>
                </ul>
            </div>
            <div class="feature-box">
                <h2>🚀 Gelişmiş Özellikler / Advanced Features</h2>
                <ul>
                    <li>kullanıcı yönetimi / user management</li>
                    <li>TC Merkez Bankası’na göre güncel döviz kurları takibi / real-time exchange rate tracking based on the Central Bank of Turkey</li>
                    <li>iade işlemleri / return handling</li>
                    <li>veri yedekleme / data backup</li>
                </ul>
            </div>
        </section>

        <section class="screenshots">
            <h2 style="grid-column: span 2; text-align: center; color: #667eea;">🖼️ Ekran Görüntüleri / Screenshots</h2>
            
            <div class="screenshot-item">
                <h3>Ana Ekran / Main Screen</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Ana_Sayfa.png" alt="Ana Sayfa">
            </div>
            
            <div class="screenshot-item">
                <h3>Ayarlar / Settings</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Ayarlar.png" alt="Ayarlar">
            </div>
            
            <div class="screenshot-item">
                <h3>Barkod Yazdırma / Barcode Printing</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Barkod_Yazdır.png" alt="Barkod Yazdır">
            </div>
            
            <div class="screenshot-item">
                <h3>Fiyat Görüntüleme / Price View</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Fiyat_Gör.png" alt="Fiyat Gör">
            </div>
            
            <div class="screenshot-item">
                <h3>Fiyat Teklifi / Price Offer</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Fiyat_Teklifi.png" alt="Fiyat Teklifi">
            </div>
            
            <div class="screenshot-item">
                <h3>Kasa / Cash Register</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Kasa.png" alt="Kasa">
            </div>
            
            <div class="screenshot-item">
                <h3>Kullanıcı Girişi / User Login</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Kullanıcı_Girişi.png" alt="Kullanıcı Girişi">
            </div>
            
            <div class="screenshot-item">
                <h3>Kullanıcılar / Users</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Kullanıcılar.png" alt="Kullanıcılar">
            </div>
            
            <div class="screenshot-item">
                <h3>Müşteri Borç Detayı / Customer Debt Detail</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Müşteri_Borç_Detayı.png" alt="Müşteri Borç Detayı">
            </div>
            
            <div class="screenshot-item">
                <h3>Müşteri Borç Listesi / Customer Debt List</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Müşteri_Borç_Listesi.png" alt="Müşteri Borç Listesi">
            </div>
            
            <div class="screenshot-item">
                <h3>Müşteriden İade Al / Return From Customer</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Müşteriden_İade_al.png" alt="Müşteriden İade Al">
            </div>
            
            <div class="screenshot-item">
                <h3>Müşteriler / Customers</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Müşteriler.png" alt="Müşteriler">
            </div>
            
            <div class="screenshot-item">
                <h3>Raporlar / Reports</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Raporlar.png" alt="Raporlar">
            </div>
            
            <div class="screenshot-item">
                <h3>Satış İşlemleri / Sales Operations</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Satış_İşlemleri.png" alt="Satış İşlemleri">
            </div>
            
            <div class="screenshot-item">
                <h3>Toplu Ürün Sil / Bulk Delete Products</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Toplu_Ürün_Sil.png" alt="Toplu Ürün Sil">
            </div>
            
            <div class="screenshot-item">
                <h3>Toptancı Borç Listesi / Supplier Debt List</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Toptancı_Borç_Listesi.png" alt="Toptancı Borç Listesi">
            </div>
            
            <div class="screenshot-item">
                <h3>Toptancı Hesap Detayı / Supplier Account Detail</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Toptancı_Hesap_Detayı.png" alt="Toptancı Hesap Detayı">
            </div>
            
            <div class="screenshot-item">
                <h3>Toptancılar / Suppliers</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Toptancılar.png" alt="Toptancılar">
            </div>
            
            <div class="screenshot-item">
                <h3>Toptancıya Ürün İade Et / Return Product To Supplier</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Toptancıya_Ürün_İade_Et.png" alt="Toptancıya Ürün İade Et">
            </div>
            
            <div class="screenshot-item">
                <h3>Ürün Detayı / Product Detail</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Ürün_Detayı.png" alt="Ürün Detayı">
            </div>
            
            <div class="screenshot-item">
                <h3>Ürün İade Al / Receive Product Return</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Ürün_İade_Al.png" alt="Ürün İade Al">
            </div>
            
            <div class="screenshot-item">
                <h3>Ürün İade Et / Return Product</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Ürün_İade_Et.png" alt="Ürün İade Et">
            </div>
            
            <div class="screenshot-item">
                <h3>Ürün İşlemleri / Product Operations</h3>
                <img src="Ürün_Yönetim_Sistemi_IMG/Ürün_İşlemleri.png" alt="Ürün İşlemleri">
            </div>
        </section>

        <footer>
            <p>&copy; 2025 Ürün Yönetim Sistemi. Tüm hakları saklıdır.</p>
        </footer>
    </div>
</body>
</html>

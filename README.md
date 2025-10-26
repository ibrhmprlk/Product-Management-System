<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Ürün Yönetim Sistemi</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
            margin: 0;
            padding: 0;
            background: #f5f5f5;
        }
        header {
            background: #4CAF50;
            color: white;
            padding: 1rem;
            text-align: center;
        }
        main {
            max-width: 1200px;
            margin: auto;
            padding: 2rem;
        }
        h1, h2, h3 {
            color: #333;
        }
        a.button {
            display: inline-block;
            padding: 0.5rem 1rem;
            background: #4CAF50;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            margin-bottom: 1rem;
        }
        section {
            margin-bottom: 2rem;
        }
        .screenshots {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 1rem;
        }
        .screenshots figure {
            background: white;
            padding: 0.5rem;
            border-radius: 5px;
            text-align: center;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }
        .screenshots img {
            max-width: 100%;
            border-radius: 5px;
        }
        footer {
            text-align: center;
            padding: 1rem;
            background: #333;
            color: white;
        }
    </style>
</head>
<body>
    <header>
        <h1>Ürün Yönetim Sistemi / Product Management System</h1>
    </header>
    <main>
        <section>
            <p>
                <strong>Ürün Yönetim Sistemi</strong>, C# ve Access tabanlı kapsamlı bir işletme yönetim yazılımıdır. 
                Sistem, stok yönetimi, satış takibi, finansal analiz ve raporlama süreçlerini tek bir platformda birleştirir.
            </p>
            <p>
                <strong>Product Management System</strong> is a comprehensive business management software developed in C# with an Access database.
            </p>
            <a class="button" href="https://drive.google.com/file/d/1cAoHV6GR8eTbx1QWRXVKFuZVp0RTCMYH/view?usp=drive_link" target="_blank">
                İndir / Download
            </a>
        </section>

        <section>
            <h2>Ekran Görüntüleri / Screenshots</h2>
            <div class="screenshots">
                <figure>
                    <img src="Ürün_Yönetim_Sistemi_IMG/Ana_Sayfa.png" alt="Ana Sayfa">
                    <figcaption>Ana Ekran / Main Screen</figcaption>
                </figure>
                <figure>
                    <img src="Ürün_Yönetim_Sistemi_IMG/Ayarlar.png" alt="Ayarlar">
                    <figcaption>Ayarlar / Settings</figcaption>
                </figure>
                <figure>
                    <img src="Ürün_Yönetim_Sistemi_IMG/Barkod_Yazdır.png" alt="Barkod Yazdır">
                    <figcaption>Barkod Yazdırma / Barcode Printing</figcaption>
                </figure>
                <figure>
                    <img src="Ürün_Yönetim_Sistemi_IMG/Fiyat_Gör.png" alt="Fiyat Gör">
                    <figcaption>Fiyat Görüntüleme / Price View</figcaption>
                </figure>
                <figure>
                    <img src="Ürün_Yönetim_Sistemi_IMG/Fiyat_Teklifi.png" alt="Fiyat Teklifi">
                    <figcaption>Fiyat Teklifi / Price Offer</figcaption>
                </figure>
                <figure>
                    <img src="Ürün_Yönetim_Sistemi_IMG/Kasa.png" alt="Kasa">
                    <figcaption>Kasa / Cash Register</figcaption>
                </figure>
                <!-- Dilersen diğer ekran görüntülerini aynı şekilde ekleyebilirsin -->
            </div>
        </section>
    </main>
    <footer>
        &copy; 2025 İbrahim PARLAK
    </footer>
</body>
</html>

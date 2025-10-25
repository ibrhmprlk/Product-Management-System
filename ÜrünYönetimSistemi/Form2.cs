using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;      // Veritabanı için
using System.IO;             // File.Exists için
using System.Drawing.Drawing2D;  // Yüksek kaliteli çizim için (InterpolationMode vb.)

namespace ÜrünYönetimSistemi
{
    public partial class Form2 : Form
    {
        public Form1 frm1;

        // Arka plan resmini saklamak için
        private Image backgroundImage;

        // Mevcut dili saklamak için
        public string CurrentCulture { get; set; } = "tr-TR";

        // Veritabanı bağlantı string'i (Form1'den aynı)
        private readonly string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Application.StartupPath}\\ÜrünYönetimSistemi.accdb";

        // Çeviri sözlüğü
        private Dictionary<string, Dictionary<string, string>> translations =
            new Dictionary<string, Dictionary<string, string>>
            {
                ["tr-TR"] = new Dictionary<string, string>
            {
                {"Form2", "Ana Sayfa"},
                {"Btn1", "Ürün İşlemleri"},
                {"Btn2", "Satış İşlemi"},
                {"Btn3", "Fiyat Gör"},
                {"Btn4", "Fiyat Teklifi"},
                {"Btn5", "Müşteriler"},
                {"Btn6", "Barkod Yazdır"},
                {"Btn7", "Raporlar"},
                {"Btn8", "Kasa"},
                {"Btn9", "Toptancılar"},
                {"Btn10", "Kullanıcılar"},
                {"Btn11", "Uygulamayı Kapat"}
            },
                ["en-US"] = new Dictionary<string, string>
            {
                {"Form2", "Home Page"},
                {"Btn1", "Product Entry"},
                {"Btn2", "Sales"},
                {"Btn3", "View Price"},
                {"Btn4", "Price Quote"},
                {"Btn5", "Customers"},
                {"Btn6", "Print Barcode"},
                {"Btn7", "Reports"},
                {"Btn8", "Cash"},
                {"Btn9", "Wholesalers"},
                {"Btn10", "Users"},
                {"Btn11", "Exit Application"}
            },
                ["de-DE"] = new Dictionary<string, string>
            {
                {"Form2", "Startseite"},
                {"Btn1", "Produkt Eingabe"},
                {"Btn2", "Verkauf"},
                {"Btn3", "Preis Anzeigen"},
                {"Btn4", "Preisangebot"},
                {"Btn5", "Kunden"},
                {"Btn6", "Barcode Drucken"},
                {"Btn7", "Berichte"},
                {"Btn8", "Kasse"},
                {"Btn9", "Großhändler"},
                {"Btn10", "Benutzer"},
                {"Btn11", "Anwendung Beenden"}
            }
            };

        public Form2()
        {
            InitializeComponent();
            this.FormClosing += Form2_FormClosing; // Bu satır zaten var (temizlik için)

            // FormClosed olayını bağlayın (UYGULAMAYI KAPATMAK İÇİN)
            this.FormClosed += Form2_FormClosed; // **Bu satırı Form2_Load içine ekleyin!**

            // Resize event'ini bağla (boyut değişince yeniden çiz)
            this.Resize += Form2_Resize;
            ArkaPlanGuncelle();  // Form açılırken arka planı yükle
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (frm1 != null)
            {
                CurrentCulture = frm1.SelectedLanguage switch
                {
                    "Türkçe" => "tr-TR",
                    "Almanca" => "de-DE",
                    "İngilizce" => "en-US",
                    _ => "tr-TR"
                };
            }
            ChangeLanguage(CurrentCulture);

            // Önce tüm butonları devre dışı bırak (varsayılan enabled'ı override et)
            DisableAllButtons();

            // İzinleri yükle (yetki bazlı buton enabled/disabled)
            LoadPermissions();

            // Paint event'ini bağla (yüksek kaliteli arka plan için)
            this.Paint += Form2_Paint;

            // FormClosing event'ini bağla (resmi temizle)
            this.FormClosing += Form2_FormClosing;

            // Resize event'ini bağla (boyut değişince yeniden çiz)
            this.Resize += Form2_Resize;
        }

        // İzinleri veritabanından yükle ve butonları enabled yap
        private void LoadPermissions()
        {
            if (frm1 == null || string.IsNullOrEmpty(frm1.yetki))
            {
                MessageBox.Show($"Yetki bilgisi yok: {frm1?.yetki ?? "null"}");  // Debug: Yetkiyi göster
                DisableAllButtons();
                return;
            }

            try
            {
                using (OleDbConnection baglan = new OleDbConnection(connectionString))
                {
                    OleDbCommand kmt = new OleDbCommand("SELECT * FROM Personel WHERE Yetki=@yetki", baglan);
                    kmt.Parameters.AddWithValue("@yetki", frm1.yetki);
                    baglan.Open();
                    OleDbDataReader okuyucu = kmt.ExecuteReader();
                    if (okuyucu.Read())
                    {
                        // Debug: Bir izin değerini göster (örneğin Ürün Girişi)


                        // Sütun adlarını tablodan gelen Alan Adı'na göre ayarla (exact match)
                        button1.Enabled = GetPermission(okuyucu, "UrunGirisi");  // Ürün Girişi
                        button2.Enabled = GetPermission(okuyucu, "SatisIslemi"); // Satış İşlemi
                        button3.Enabled = GetPermission(okuyucu, "FiyatGor");    // Fiyat Gör
                        button4.Enabled = GetPermission(okuyucu, "FiyatTeklifi"); // Fiyat Teklifi
                        button5.Enabled = GetPermission(okuyucu, "Musteriler");  // Müşteriler
                        button6.Enabled = GetPermission(okuyucu, "BarkodYazdir"); // Barkod Yazdır
                        button7.Enabled = GetPermission(okuyucu, "Raporlar"); // Barkod Yazdır
                        button8.Enabled = GetPermission(okuyucu, "Kasa");         // Kasa
                        button9.Enabled = GetPermission(okuyucu, "Toptancilar");   // Toptancılar
                        button10.Enabled = GetPermission(okuyucu, "Kullanicilar");  // Kullanıcılar (Unlancilar sütunu)
                        button13.Enabled = GetPermission(okuyucu, "UrunIade");    // Ürün İade Al (Uru nlade -> UrunIade varsayımı)
                        button14.Enabled = GetPermission(okuyucu, "UrunDetayi");
                        button12.Enabled = GetPermission(okuyucu, "UrunAlis");     // Ürün Detayı (Urunlis)
                        button15.Enabled = GetPermission(okuyucu, "Ayarlar");     // Ayarlar

                        // Çıkış butonu her zaman enabled

                    }
                    else
                    {
                        MessageBox.Show($"Yetki bulunamadı: {frm1.yetki}");  // Debug
                        DisableAllButtons();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İzinler yüklenirken hata: {ex.Message}\nStack: {ex.StackTrace}");  // Debug detaylı
                DisableAllButtons();  // Hata durumunda güvenli mod
            }
        }

        // Yardımcı metod: İzin değerini güvenli oku (Yes/No boolean, sütun yoksa false)
        private bool GetPermission(OleDbDataReader reader, string columnName)
        {
            try
            {
                object value = reader[columnName];
                if (value == DBNull.Value) return false;
                return Convert.ToBoolean(value);  // Access Yes/No -> true/false
            }
            catch
            {
                // Sütun yoksa veya dönüştürme hatası
                return false;
            }
        }

        // Yardımcı metod: Tüm butonları devre dışı bırak (güvenlik)
        private void DisableAllButtons()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            button10.Enabled = false;

            button12.Enabled = false;
            button13.Enabled = false;
            button14.Enabled = false;
            button15.Enabled = false;
        }

        public void ChangeLanguage(string cultureName)
        {
            if (!translations.ContainsKey(cultureName))
                cultureName = "tr-TR";

            var t = translations[cultureName];
            this.Text = t["Form2"];
            button1.Text = t["Btn1"];
            button2.Text = t["Btn2"];
            button3.Text = t["Btn3"];
            button4.Text = t["Btn4"];
            button5.Text = t["Btn5"];
            button6.Text = t["Btn6"];
            button7.Text = t["Btn7"];
            button8.Text = t["Btn8"];
            button9.Text = t["Btn9"];
            button10.Text = t["Btn10"];


            CurrentCulture = cultureName;
        }

        // EXIF orientation'ı düzelten yardımcı metod
        private Image FixOrientation(Image img)
        {
            if (img == null) return null;

            // EXIF Orientation tag'i var mı kontrol et (ID: 274 = 0x0112)
            if (Array.IndexOf(img.PropertyIdList, 274) > -1)
            {
                var orientationItem = img.GetPropertyItem(274);
                int orientation = orientationItem.Value[0];

                switch (orientation)
                {
                    case 2:
                        img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        img.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        img.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        img.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }

                // Tag'i sil ki tekrar uygulanmasın (isteğe bağlı, ama iyi pratik)
                img.RemovePropertyItem(274);
            }

            return img;
        }

        // --- Butonlar ---
        private void button1_Click(object sender, EventArgs e)
        {
            // Ürün Girişi formunu aç
            Ürün_Girişi urunGirisiFormu = new Ürün_Girişi();
            urunGirisiFormu.Show();

            // Açık olan Toptanci veya Toplu_Ürün_Sil formlarını kapat
            // Kapatma işlemi sırasında form koleksiyonu değişebileceğinden, ToList() ile bir kopya üzerinde döngü yapıyoruz.
            List<Form> acikFormlar = Application.OpenForms.Cast<Form>().ToList();

            foreach (Form frm in acikFormlar)
            {
                // Toptancı formu ise kapat
                if (frm is Toptanci)
                {
                    frm.Close();
                }
                // Toplu Ürün Sil formu ise kapat (Form adının Toplu_Ürün_Sil olduğunu varsayıyoruz)
                else if (frm is Toplu_Ürün_Sil)
                {
                    frm.Close();
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Toptanci toptanciForm = Application.OpenForms.OfType<Toptanci>().FirstOrDefault();
            if (toptanciForm == null)
            {
                toptanciForm = new Toptanci();
                toptanciForm.Show();
            }
            else toptanciForm.BringToFront();

            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i] is Ürün_Girişi)
                {
                    Application.OpenForms[i].Close();
                    break;
                }
            }
        }

        private async void button11_Click(object sender, EventArgs e) // Metot async yapıldı
        {
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Fiyat_Gör frm = new Fiyat_Gör();
            frm.CurrentCulture = this.CurrentCulture;
            frm.Show();

            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i] is Ürün_Girişi)
                {
                    Application.OpenForms[i].Close();
                    break;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Barkod_Yazdır frmBarkod = new Barkod_Yazdır();
            frmBarkod.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Satış_İşlemleri frm = new Satış_İşlemleri();
            frm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // YENİ EKLEME: MusteriEkle formunun açık olup olmadığını kontrol et ve kapat.
            foreach (Form openForm in Application.OpenForms)
            {
                // Açık formun MusteriEkle tipi olup olmadığını kontrol et
                if (openForm is MusteriEkle)
                {
                    // Eğer MusteriEkle formu açıksa, kapat
                    openForm.Close();
                    // Formu bulup kapattığımız için döngüyü sonlandırabiliriz
                    break;
                }
            }

            // MEVCUT KOD: Müşteriler formunu aç.
            Müşteriler musteriForm = new Müşteriler();
            musteriForm.Show();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            FiyatTeklifi fiyatForm = new FiyatTeklifi();
            fiyatForm.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Kullanicilar kullaniciForm = new Kullanicilar();
            kullaniciForm.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Kasa kasaForm = new Kasa();
            kasaForm.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Raporlar raporForm = new Raporlar();
            raporForm.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            UrunIadeAl urunIadeAlForm = new UrunIadeAl();
            urunIadeAlForm.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            UrunIadeEt urunIadeEtForm = new UrunIadeEt();
            urunIadeEtForm.Show();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            UrunDetayı urunDetayForm = new UrunDetayı();
            urunDetayForm.Show();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Ayarlar ayarForm = new Ayarlar(this);
            ayarForm.Show();
        }

        // --- Arka Planı Güncelle (Yüksek kaliteli versiyon) ---
        public void ArkaPlanGuncelle()
        {
            string resimYolu = null;

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))  // Tam yol kullan
                {
                    conn.Open();
                    string query = "SELECT ArkaPlanResmi FROM IsletmeAdi WHERE 1=1";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            resimYolu = result.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(resimYolu) && File.Exists(resimYolu))
                {
                    if (backgroundImage != null)
                        backgroundImage.Dispose();  // Eski resmi temizle

                    Image rawImage = Image.FromFile(resimYolu);
                    backgroundImage = FixOrientation(rawImage);  // Orientation'ı düzelt
                }
                else
                {
                    if (backgroundImage != null)
                    {
                        backgroundImage.Dispose();
                        backgroundImage = null;
                    }
                }

                this.Invalidate();  // Form'u yeniden çizdir (Paint event tetiklenir)
            }
            catch
            {
                if (backgroundImage != null)
                {
                    backgroundImage.Dispose();
                    backgroundImage = null;
                }
                this.Invalidate();
            }
        }

        // Form resize olunca yeniden çiz (tam kaplama için)
        private void Form2_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        // Yüksek kaliteli arka plan çizimi (Tam kaplama: crop ile aspect ratio korunarak)
        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            if (backgroundImage != null)
            {
                Graphics g = e.Graphics;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;  // Yüksek kaliteli ölçekleme
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;

                Rectangle destRect = this.ClientRectangle;

                // Tam kaplama için scale hesapla (aspect ratio koru, en büyük ölçeği al - crop etkisi)
                float scaleX = (float)destRect.Width / backgroundImage.Width;
                float scaleY = (float)destRect.Height / backgroundImage.Height;
                float scale = Math.Max(scaleX, scaleY);  // Max için tam kaplama (crop)

                int newWidth = (int)(backgroundImage.Width * scale);
                int newHeight = (int)(backgroundImage.Height * scale);

                // Ortala (crop için offset hesapla)
                int x = (destRect.Width - newWidth) / 2;
                int y = (destRect.Height - newHeight) / 2;
                Rectangle newRect = new Rectangle(x, y, newWidth, newHeight);

                // Resmi yeni rect'e çiz (tam kaplar, kenarlar crop olur)
                g.DrawImage(backgroundImage, newRect);
            }
        }
        // Formun kapatma düğmesine basıldığında tetiklenir
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 🔹 Veriler aktarılıyor ekranını göster
            using (frmverilerAktarılıyor aktarForm = new frmverilerAktarılıyor())
            {
                aktarForm.ShowDialog();
            }

            // 🔹 frmverilerAktarılıyor kapanınca yedekleme başlar

            string kaynakDosya = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ÜrünYönetimSistemi.accdb");
            string yedekKlasoru = "C:\\UygulamaYedekleri\\Access_Yedekler";
            string zamanDamgasi = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string hedefDosyaAdi = $"Veritabani_yedek_{zamanDamgasi}.accdb";
            string hedefDosya = Path.Combine(yedekKlasoru, hedefDosyaAdi);

            try
            {
                if (!Directory.Exists(yedekKlasoru))
                    Directory.CreateDirectory(yedekKlasoru);

                if (!File.Exists(kaynakDosya))
                {
                    MessageBox.Show($"UYARI: Kaynak veritabanı bulunamadı, yedekleme yapılamadı. Uygulama kapatılıyor.",
                                    "Yedekleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    File.Copy(kaynakDosya, hedefDosya, true);
                    // Burada mesaj vermiyoruz, kapanış sürecini hızlandırmak için.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Uygulama kapatılırken yedekleme başarısız oldu:\n" + ex.Message,
                                "Kapatma/Yedekleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 🔹 Uygulamayı kapat
            Application.Exit();
        }

        // 🔹 Form kapanırken arka plan resmini temizleme (performans için)
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundImage != null)
            {
                backgroundImage.Dispose();
                backgroundImage = null;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // İsteğe bağlı özel çizim
        }

        private void button16_Click(object sender, EventArgs e)
        {
            // Parametresiz constructor kullanıldı.
            DövizKurları dövizForm = new DövizKurları();
            dövizForm.Show();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            // Hata mesajınızdaki linki kullanıyoruz:
            string youtubeLink = "https://www.youtube.com/playlist?list=PLJGbvGEHAv5L9uyb1qMSVuEXKhzaq3qo5";

            try
            {
                // Platform bağımsız ve güvenli link açma yöntemi (Windows için shell'i kullanır)
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = youtubeLink,
                    UseShellExecute = true // Bu satır, sistemin varsayılan tarayıcıyı kullanmasını sağlar.
                });
            }
            catch (Exception ex)
            {
                // Hata oluşursa kullanıcıya bilgi verin
                MessageBox.Show("Link açılırken bir sorun oluştu. Lütfen linki kontrol edin.\nHata Detayı: " + ex.Message,
                                "Hata",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}
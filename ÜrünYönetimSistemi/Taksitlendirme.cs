using DocumentFormat.OpenXml.Office.Word;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.collection;
using iTextSharp.text.pdf.draw;
using RawPrint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Font = System.Drawing.Font; // iTextSharp ile çakışmayı önler
using ÜrünYönetimSistemi;

namespace ÜrünYönetimSistemi
{
    public partial class Taksitlendirme : Form
    {
        public string MusteriAdi { get; set; }
        public string GsmTelefon { get; set; }
        public decimal ToplamTutar { get; set; }
        public bool FaturaKesilecekMi { get; set; }

        private Satış_İşlemleri anaForm;

        public DataTable SepetVerisi { get; set; }
        public Taksitlendirme(Satış_İşlemleri parentForm)
        {

            InitializeComponent();
            // Gelen referansı, form içindeki değişkene ata
            this.anaForm = parentForm;

            textBox3.KeyPress += (s, e) =>
            {
                if (char.IsControl(e.KeyChar))
                {
                    e.Handled = false;
                    return;
                }
                if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                    return;
                }
                if (e.KeyChar == ',' || e.KeyChar == '.')
                {
                    if (string.IsNullOrEmpty(textBox3.Text) || textBox3.Text.Contains(",") || textBox3.Text.Contains("."))
                    {
                        e.Handled = true;
                        return;
                    }
                    e.Handled = false;
                    return;
                }
                e.Handled = true;
            };
            textBox4.KeyPress += (s, e) =>
            {
                if (char.IsControl(e.KeyChar))
                {
                    e.Handled = false;
                    return;
                }
                if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                    return;
                }
                if (e.KeyChar == ',' || e.KeyChar == '.')
                {
                    if (string.IsNullOrEmpty(textBox4.Text) || textBox4.Text.Contains(",") || textBox4.Text.Contains("."))
                    {
                        e.Handled = true;
                        return;
                    }
                    e.Handled = false;
                    return;
                }
                e.Handled = true;
            };
            textBox5.KeyPress += (s, e) =>
            {
                if (char.IsControl(e.KeyChar))
                {
                    e.Handled = false;
                    return;
                }
                if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                    return;
                }
                if (e.KeyChar == ',' || e.KeyChar == '.')
                {
                    if (string.IsNullOrEmpty(textBox5.Text) || textBox5.Text.Contains(",") || textBox5.Text.Contains("."))
                    {
                        e.Handled = true;
                        return;
                    }
                    e.Handled = false;
                    return;
                }
                e.Handled = true;
            };
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox6.ReadOnly = true;
        }

        private void HesaplamalariGuncelle()
        {
            // Öncelikle textbox'lardan verileri güvenli bir şekilde alalım.
            decimal toplamBorc = 0;
            decimal pesinat = 0;
            int taksitSayisi = 0;
            decimal aylikFaizOrani = 0;
            decimal kalanAnapara = 0;
            decimal aylikOdemeTutari = 0;

            // TryParse kullanarak hatalı girişleri yönetiyoruz (virgül destekli).
            if (!decimal.TryParse(textBox1.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out toplamBorc)) return;
            if (!decimal.TryParse(textBox3.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out pesinat)) return;
            if (!int.TryParse(textBox4.Text, out taksitSayisi)) taksitSayisi = 1;
            if (!decimal.TryParse(textBox5.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out aylikFaizOrani)) aylikFaizOrani = 0;

            // --- PEŞİNAT VE KALAN ANAPARA KONTROLÜ ---
            // Peşinatın eksiye düşmesini engelle.
            if (pesinat < 0)
            {
                MessageBox.Show("Peşinat tutarı eksi değer olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Text = "0,00"; // Hatalı değeri sıfırla (virgülle)
                pesinat = 0;
            }

            // Peşinatın toplam borçtan fazla olmasını engelle
            if (pesinat > toplamBorc)
            {
                MessageBox.Show("Peşinat tutarı, toplam borçtan fazla olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Text = toplamBorc.ToString("N2"); // Değeri toplam borca eşitle
                pesinat = toplamBorc;
            }

            // Kalan anaparayı hesapla
            kalanAnapara = toplamBorc - pesinat;

            // Kalan anaparayı ekrana yazdır
            textBox2.Text = kalanAnapara.ToString("N2");

            // Faizli toplam tutarı hesapla
            decimal faizliKalanAnapara = kalanAnapara + (kalanAnapara * aylikFaizOrani / 100);

            // --- AYLIK ÖDEME TUTARI KONTROLÜ ---
            // Aylık taksit tutarını hesapla
            if (taksitSayisi > 0)
            {
                aylikOdemeTutari = faizliKalanAnapara / taksitSayisi;
            }
            else
            {
                // Taksit sayısı 0 ise uyarı ver
                MessageBox.Show("Taksit sayısı sıfırdan büyük bir değer olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Text = "1"; // Değeri 1'e sıfırla
                taksitSayisi = 1;
                aylikOdemeTutari = faizliKalanAnapara; // Tek taksitte ödenecek tutar
            }

            // Aylık ödeme tutarını ekrana yazdır
            textBox6.Text = aylikOdemeTutari.ToString("N2");
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Taksitlendirme_Load(object sender, EventArgs e)
        {
            textBox2.Text = ToplamTutar.ToString("N2");

            // İhtiyacın olursa Müşteri Adını da aynı şekilde textBox1'e atayabilirsin.
            textBox1.Text = ToplamTutar.ToString("N2");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Sepette ürün olup olmadığını kontrol et
            if (SepetVerisi == null || SepetVerisi.Rows.Count == 0)
            {
                MessageBox.Show("Sepette ürün bulunmamaktadır. Lütfen satış yapmak için ürün ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ YENİ EKLENEN KOD BAŞLANGICI
            decimal aylikFaizOrani = 0;
            decimal aylikOdemeTutari = 0;
            int taksitSayisi = 0;

            // Taksit sayısı kontrolü
            if (!int.TryParse(textBox4.Text, out taksitSayisi) || taksitSayisi < 1)
            {
                MessageBox.Show("Taksit sayısı geçerli bir sayı olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Aylık Faiz Oranı için format kontrolü
            string faizStr = textBox5.Text.Trim();
            if (faizStr.Contains(",") && faizStr.Split(',')[1].Length > 2)
            {
                MessageBox.Show($"Lütfen 'Aylık Faiz Oranı' için virgülden sonra en fazla iki hane girin. (Örn: 2,50)", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(faizStr.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out aylikFaizOrani))
            {
                MessageBox.Show("Aylık Faiz Oranı geçerli bir sayısal değer olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Aylık Ödeme Tutarı için format kontrolü
            string odemeStr = textBox6.Text.Trim();
            if (odemeStr.Contains(",") && odemeStr.Split(',')[1].Length > 2)
            {
                MessageBox.Show($"Lütfen 'Aylık Ödeme Tutarı' için virgülden sonra en fazla iki hane girin. (Örn: 1500,50)", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(odemeStr.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out aylikOdemeTutari))
            {
                MessageBox.Show("Aylık Ödeme Tutarı geçerli bir sayısal değer olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // ✅ YENİ EKLENEN KOD BİTİŞİ

            // Stoğu kontrol et
            foreach (DataRow row in SepetVerisi.Rows)
            {
                decimal satilanMiktar = TryParseDecimal(row["Miktar"]);
                decimal kalanStok = TryParseDecimal(row["Stok_Miktari"]);
                if (satilanMiktar > (kalanStok + 0.0001m))
                {
                    DialogResult dr = MessageBox.Show(
                        $"'{row["Ürün_Adi"]}' adlı ürünün satılan miktarı ({satilanMiktar}) mevcut stoktan ({kalanStok}) fazla.\n" +
                        "Stok 0 olarak ayarlanacak ve işlem devam edecek. Onaylıyor musunuz?",
                        "Stok Yetersiz",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                    if (dr != DialogResult.Yes) return;
                }
            }

            // Satış onayı
            DialogResult result = MessageBox.Show(
                "Satış işlemini tamamlamak istediğinize emin misiniz?",
                "Satış Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result != DialogResult.Yes) return;

            string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
            using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
            {
                baglan.Open();
                OleDbTransaction transaction = baglan.BeginTransaction();
                try
                {
                    string satisSorgu = "INSERT INTO UrunSatis (Barkod_No, Urun_Adi, UrunGrubu, KalanStok, OlcuBirimi, Satis_Fiyati, Alis_Fiyati, [2SatisFiyati], AsgariStok, SatilanMiktar, ToplamTutar, Tarih, Saat, SatisTuru) VALUES (@BarkodNo, @UrunAdi, @UrunGrubu, @KalanStok, @OlcuBirimi, @SatisFiyati, @AlisFiyati, @IkinciSatisFiyati, @AsgariStok, @SatilanMiktar, @ToplamTutar, @Tarih, @Saat, @SatisTuru)";
                    string stokGuncellemeSorgu = "UPDATE ÜrünGirişi SET Stok_Miktari = @YeniStokMiktari WHERE Barkod_No = @BarkodNo";

                    // Gerekli verileri al (virgül destekli parse)
                    // ✅ BU KISIM YUKARI TAŞINDI
                    // int taksitSayisi = int.TryParse(textBox4.Text, out int ts) ? ts : 1;
                    // decimal aylikFaizOrani = TryParseDecimal(textBox5.Text);
                    // decimal aylikOdemeTutari = TryParseDecimal(textBox6.Text);

                    DateTime ilkTaksitTarihi = dateTimePicker1.Value.Date; // Sadece tarih (saat 00:00)

                    foreach (DataRow row in SepetVerisi.Rows)
                    {
                        decimal satilanMiktar = TryParseDecimal(row["Miktar"]);
                        decimal mevcutStok = TryParseDecimal(row["Stok_Miktari"]);
                        decimal yeniStokMiktari = mevcutStok - satilanMiktar;
                        if (yeniStokMiktari < 0m)
                        {
                            yeniStokMiktari = 0m; // Eksiye düşmeyi engelle
                            MessageBox.Show($"'{row["Ürün_Adi"]}' ürününün stoğu 0 olarak ayarlandı çünkü satılan miktar ({satilanMiktar}) mevcut stoktan ({mevcutStok}) fazlaydı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        // UrunSatis tablosuna ekleme
                        using (OleDbCommand satisCmd = new OleDbCommand(satisSorgu, baglan, transaction))
                        {
                            satisCmd.Parameters.AddWithValue("@BarkodNo", row["Barkod_No"]?.ToString() ?? (object)DBNull.Value);
                            satisCmd.Parameters.AddWithValue("@UrunAdi", row["Ürün_Adi"]?.ToString() ?? (object)DBNull.Value);
                            satisCmd.Parameters.AddWithValue("@UrunGrubu", row["Ürün_Grubu"]?.ToString() ?? (object)DBNull.Value);
                            satisCmd.Parameters.AddWithValue("@KalanStok", yeniStokMiktari);
                            satisCmd.Parameters.AddWithValue("@OlcuBirimi", row["OlcuBirimi"]?.ToString() ?? (object)DBNull.Value);
                            satisCmd.Parameters.AddWithValue("@SatisFiyati", TryParseDecimal(row["Satis_Fiyati"]));
                            satisCmd.Parameters.AddWithValue("@AlisFiyati", TryParseDecimal(row["Alis_Fiyati"]));
                            satisCmd.Parameters.AddWithValue("@IkinciSatisFiyati", TryParseDecimal(row["2SatisFiyati"]));
                            satisCmd.Parameters.AddWithValue("@AsgariStok", TryParseDecimal(row["AsgariStok"]));
                            satisCmd.Parameters.AddWithValue("@SatilanMiktar", satilanMiktar);
                            satisCmd.Parameters.AddWithValue("@ToplamTutar", TryParseDecimal(row["ToplamTutar"]));
                            satisCmd.Parameters.AddWithValue("@Tarih", DateTime.Now.ToString("MM/dd/yyyy")); // Tarih formatını ayarla
                            satisCmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToString("HH:mm:ss")); // Saat formatını ayarla
                            satisCmd.Parameters.AddWithValue("@SatisTuru", "Taksitli Satış");
                            satisCmd.ExecuteNonQuery();
                        }

                        // Stok güncelleme
                        using (OleDbCommand guncellemeCmd = new OleDbCommand(stokGuncellemeSorgu, baglan, transaction))
                        {
                            guncellemeCmd.Parameters.AddWithValue("@YeniStokMiktari", yeniStokMiktari);
                            guncellemeCmd.Parameters.AddWithValue("@BarkodNo", row["Barkod_No"]?.ToString() ?? (object)DBNull.Value);
                            guncellemeCmd.ExecuteNonQuery();
                        }

                        // MusteriSatis tablosuna veri ekleme
                        if (!string.IsNullOrWhiteSpace(MusteriAdi) || !string.IsNullOrWhiteSpace(GsmTelefon))
                        {
                            string musteriSatisSorgu = "INSERT INTO MusteriSatis (MusteriAdi, GsmTelefon, Barkod_No, Urun_Adi, Stok_Miktari, AsgariStok, OlcuBirimi, Satis_Fiyati, SatilanMiktar, ToplamTutar, SatisTuru, Tarih, Saat, IlkTaksitTarihi, TaksitSayisi, AylikFaizOrani, AylikOdemeTutari) " +
                                                        "VALUES (@MusteriAdi, @GsmTelefon, @BarkodNo, @UrunAdi, @StokMiktari, @AsgariStok, @OlcuBirimi, @SatisFiyati, @SatilanMiktar, @ToplamTutar, @SatisTuru, @Tarih, @Saat, @IlkTaksitTarihi, @TaksitSayisi, @AylikFaizOrani, @AylikOdemeTutari)";

                            using (OleDbCommand musteriSatisCmd = new OleDbCommand(musteriSatisSorgu, baglan, transaction))
                            {
                                musteriSatisCmd.Parameters.AddWithValue("@MusteriAdi", MusteriAdi ?? (object)DBNull.Value);
                                musteriSatisCmd.Parameters.AddWithValue("@GsmTelefon", GsmTelefon ?? (object)DBNull.Value);
                                musteriSatisCmd.Parameters.AddWithValue("@BarkodNo", row["Barkod_No"]?.ToString() ?? (object)DBNull.Value);
                                musteriSatisCmd.Parameters.AddWithValue("@UrunAdi", row["Ürün_Adi"]?.ToString() ?? (object)DBNull.Value);
                                musteriSatisCmd.Parameters.AddWithValue("@StokMiktari", yeniStokMiktari);
                                musteriSatisCmd.Parameters.AddWithValue("@AsgariStok", TryParseDecimal(row["AsgariStok"]));
                                musteriSatisCmd.Parameters.AddWithValue("@OlcuBirimi", row["OlcuBirimi"]?.ToString() ?? (object)DBNull.Value);
                                musteriSatisCmd.Parameters.AddWithValue("@SatisFiyati", TryParseDecimal(row["Satis_Fiyati"]));
                                musteriSatisCmd.Parameters.AddWithValue("@SatilanMiktar", satilanMiktar);
                                musteriSatisCmd.Parameters.AddWithValue("@ToplamTutar", TryParseDecimal(row["ToplamTutar"]));
                                musteriSatisCmd.Parameters.AddWithValue("@SatisTuru", "Taksitli Satış");
                                musteriSatisCmd.Parameters.AddWithValue("@Tarih", DateTime.Now.ToString("MM/dd/yyyy")); // Tarih formatını ayarla
                                musteriSatisCmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToString("HH:mm:ss")); // Saat formatını ayarla
                                musteriSatisCmd.Parameters.AddWithValue("@IlkTaksitTarihi", ilkTaksitTarihi.ToString("MM/dd/yyyy")); // Sadece tarih
                                musteriSatisCmd.Parameters.AddWithValue("@TaksitSayisi", taksitSayisi);
                                musteriSatisCmd.Parameters.AddWithValue("@AylikFaizOrani", aylikFaizOrani);
                                musteriSatisCmd.Parameters.AddWithValue("@AylikOdemeTutari", aylikOdemeTutari);
                                musteriSatisCmd.ExecuteNonQuery();
                            }

                            // Müşteriler tablosundaki Taksit borcunu güncelleme
                            string musteriGuncellemeSorgu = "UPDATE Musteriler SET Taksit = @YeniTaksit WHERE MusteriAdi = @MusteriAdi AND GsmTelefon = @GsmTelefon";
                            using (OleDbCommand musteriGuncellemeCmd = new OleDbCommand(musteriGuncellemeSorgu, baglan, transaction))
                            {
                                // Önce mevcut taksit tutarını al
                                decimal mevcutTaksit = 0;
                                string sorguGetir = "SELECT Taksit FROM Musteriler WHERE MusteriAdi = @MusteriAdi AND GsmTelefon = @GsmTelefon";
                                using (OleDbCommand getCmd = new OleDbCommand(sorguGetir, baglan, transaction))
                                {
                                    getCmd.Parameters.AddWithValue("@MusteriAdi", MusteriAdi ?? (object)DBNull.Value);
                                    getCmd.Parameters.AddWithValue("@GsmTelefon", GsmTelefon ?? (object)DBNull.Value);
                                    object sonuc = getCmd.ExecuteScalar();
                                    if (sonuc != null && sonuc != DBNull.Value)
                                    {
                                        decimal.TryParse(sonuc.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out mevcutTaksit);
                                    }
                                }

                                // Yeni toplam borcu hesapla
                                decimal toplamTutar = TryParseDecimal(row["ToplamTutar"]);
                                decimal yeniToplamTaksit = mevcutTaksit + toplamTutar;

                                // Sonra yeni toplamı geri yaz
                                musteriGuncellemeCmd.Parameters.AddWithValue("@YeniTaksit", yeniToplamTaksit.ToString("N2", new CultureInfo("tr-TR"))); // TR formatında kaydet
                                musteriGuncellemeCmd.Parameters.AddWithValue("@MusteriAdi", MusteriAdi ?? (object)DBNull.Value);
                                musteriGuncellemeCmd.Parameters.AddWithValue("@GsmTelefon", GsmTelefon ?? (object)DBNull.Value);
                                musteriGuncellemeCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    MessageBox.Show("Taksitli satış başarıyla tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Fiş basma kontrolü (checkbox2'ye bağlı)
                    if (FaturaKesilecekMi)
                    {
                        // İşletme bilgilerini al
                        string isletmeAdiFis = "", isletmeAdresiFis = "", isletmeYeriFis = "", gsmTelefonFis = "";
                        using (OleDbCommand cmdFis = new OleDbCommand("SELECT IsletmeAdi, IsletmeAdresi, IsletmeYeri, GsmTelefon FROM IsletmeAdi", baglan))
                        using (OleDbDataReader readerFis = cmdFis.ExecuteReader())
                        {
                            if (readerFis.Read())
                            {
                                isletmeAdiFis = readerFis["IsletmeAdi"].ToString();
                                isletmeAdresiFis = readerFis["IsletmeAdresi"].ToString();
                                isletmeYeriFis = readerFis["IsletmeYeri"].ToString();
                                gsmTelefonFis = "Tlf - " + readerFis["GsmTelefon"].ToString();
                            }
                        }

                        float pageWidth = 315;
                        float padding = 5;

                        // Kağıt yüksekliği tahmini
                        float paperHeight = 0;
                        paperHeight += 20; // Üst boşluk
                        paperHeight += 15 * 5; // İşletme bilgileri ve aralarındaki boşluklar
                        paperHeight += 10; // Çizgi
                        paperHeight += 25; // Tarih ve saat
                        paperHeight += 20; // Sütun başlıkları
                        paperHeight += 5; // Sütun çizgisi

                        using (var bmp = new Bitmap(1, 1))
                        using (Graphics g = Graphics.FromImage(bmp))
                        using (System.Drawing.Font trFont = new System.Drawing.Font("Arial", 8))
                        using (System.Drawing.Font trFontSemiBold = new System.Drawing.Font("Arial", 8, FontStyle.Regular))
                        {
                            float urunBilgiWidth = pageWidth * 0.55f - padding; // Ürün genişliğini artırarak miktar sütununu sola çek
                            foreach (DataRow row in SepetVerisi.Rows)
                            {
                                string barkodNo = row["Barkod_No"]?.ToString() ?? "";
                                string urunAdi = row["Ürün_Adi"]?.ToString() ?? "";
                                string barkodUrunAdi = $"{barkodNo} / {urunAdi}";
                                SizeF barkodUrunAdiSize = g.MeasureString(barkodUrunAdi, trFontSemiBold, (int)urunBilgiWidth);
                                paperHeight += barkodUrunAdiSize.Height + padding;
                            }
                        }

                        paperHeight += 10; // Çizgi
                        paperHeight += 20; // Genel toplam
                        paperHeight += 5;  // Bilgilendirme yazısı için boşluk

                        // Yazıcı fişi
                        PrintDocument pd = new PrintDocument();
                        pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)pageWidth, (int)Math.Ceiling(paperHeight));
                        pd.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

                        pd.PrintPage += (snd, ev) =>
                        {
                            float yPos = 20;
                            float _pageWidth = ev.PageSettings.PaperSize.Width - 10;
                            StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                            StringFormat leftFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                            StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };

                            using (System.Drawing.Font trFont = new System.Drawing.Font("Arial", 8))
                            using (System.Drawing.Font trFontBold = new System.Drawing.Font("Arial", 8, FontStyle.Bold))
                            using (System.Drawing.Font trFontSemiBold = new System.Drawing.Font("Arial", 8, FontStyle.Regular))
                            using (System.Drawing.Font trFontItalic = new System.Drawing.Font("Arial", 7, FontStyle.Italic))
                            {
                                // İşletme bilgileri
                                ev.Graphics.DrawString(isletmeAdiFis, trFontBold, Brushes.Black, new RectangleF(padding, yPos, _pageWidth - 2 * padding, 20), centerFormat);
                                yPos += 15;
                                ev.Graphics.DrawString(isletmeAdresiFis, trFont, Brushes.Black, new RectangleF(padding, yPos, _pageWidth - 2 * padding, 20), centerFormat);
                                yPos += 15;
                                ev.Graphics.DrawString(isletmeYeriFis, trFont, Brushes.Black, new RectangleF(padding, yPos, _pageWidth - 2 * padding, 20), centerFormat);
                                yPos += 15;
                                ev.Graphics.DrawString(gsmTelefonFis, trFont, Brushes.Black, new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                                yPos += 15;

                                // Çizgi
                                ev.Graphics.DrawLine(Pens.Black, padding, yPos, _pageWidth - padding, yPos);
                                yPos += 10;

                                // Tarih ve saat (saat daha da sola çekildi)
                                string dateText = $"Tarih: {DateTime.Now.ToShortDateString()}";
                                string timeText = $"Saat: {DateTime.Now.ToLongTimeString()}";
                                float halfWidth = (_pageWidth - 2 * padding) / 2;
                                ev.Graphics.DrawString(dateText, trFont, Brushes.Black,
                                    new RectangleF(padding, yPos, halfWidth - 5, 15), leftFormat);
                                ev.Graphics.DrawString(timeText, trFont, Brushes.Black,
                                    new RectangleF(padding + halfWidth - 15, yPos, halfWidth, 15), rightFormat);
                                yPos += 25;

                                // Sütun başlıkları
                                ev.Graphics.DrawString("Barkod No / Ürün Adı", trFontBold, Brushes.Black,
                                    new RectangleF(padding, yPos, _pageWidth * 0.45f - padding, 15), leftFormat);
                                ev.Graphics.DrawString("Miktar Fiyatı", trFontBold, Brushes.Black,
                                    new RectangleF(padding + _pageWidth * 0.45f - 5, yPos, _pageWidth * 0.25f, 15), centerFormat);
                                ev.Graphics.DrawString("Toplam", trFontBold, Brushes.Black,
                                    new RectangleF(padding + _pageWidth * 0.7f, yPos, _pageWidth * 0.3f - padding, 15), leftFormat);
                                yPos += 20;

                                // Sütun çizgisi
                                ev.Graphics.DrawLine(Pens.Black, padding, yPos, _pageWidth - padding, yPos);
                                yPos += 5;

                                // Ürünler
                                foreach (DataRow row in SepetVerisi.Rows)
                                {
                                    string barkodNo = row["Barkod_No"]?.ToString() ?? "";
                                    string urunAdi = row["Ürün_Adi"]?.ToString() ?? "";
                                    string miktar = $"{TryParseDecimal(row["Miktar"]):N0}";
                                    string satisFiyati = $"{TryParseDecimal(row["Satis_Fiyati"]):N2}";
                                    string toplamTutar = $"{TryParseDecimal(row["ToplamTutar"]):N2}";

                                    float urunBilgiWidth = _pageWidth * 0.45f - padding;
                                    float miktarFiyatWidth = _pageWidth * 0.25f;
                                    float toplamWidth = _pageWidth * 0.3f - padding;

                                    string barkodUrunAdi = $"{barkodNo} / {urunAdi}";
                                    string details = $"({miktar} x {satisFiyati})";

                                    SizeF barkodUrunAdiSize = ev.Graphics.MeasureString(barkodUrunAdi, trFontSemiBold, (int)urunBilgiWidth);
                                    float totalBlockHeight = barkodUrunAdiSize.Height + 5;
                                    if (totalBlockHeight < 20) totalBlockHeight = 20;

                                    ev.Graphics.DrawString(barkodUrunAdi, trFontSemiBold, Brushes.Black,
                                        new RectangleF(padding, yPos, urunBilgiWidth, totalBlockHeight), leftFormat);
                                    ev.Graphics.DrawString(details, trFont, Brushes.Black,
                                        new RectangleF(padding + urunBilgiWidth - 5, yPos, miktarFiyatWidth, totalBlockHeight), centerFormat);
                                    ev.Graphics.DrawString(toplamTutar, trFont, Brushes.Black,
                                        new RectangleF(padding + urunBilgiWidth + miktarFiyatWidth, yPos, toplamWidth, totalBlockHeight), leftFormat);

                                    yPos += totalBlockHeight;
                                }

                                // Çizgi
                                ev.Graphics.DrawLine(Pens.Black, padding, yPos, _pageWidth - padding, yPos);
                                yPos += 10;

                                // Genel toplam
                                ev.Graphics.DrawString($"GENEL TOPLAM: {ToplamTutar:N2} TL", trFontBold, Brushes.Black,
                                    new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                                yPos += 20;

                                // Bilgilendirme yazısı
                                ev.Graphics.DrawString("BİLGİ AMAÇLIDIR MALİ DEĞERİ YOKTUR", trFontItalic, Brushes.Black,
                                    new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                                yPos += 5;
                            }
                        };
                        pd.Print();
                    }

                    // Ana formu yenileme
                    if (anaForm != null)
                    {
                        anaForm.StokListesiniYenile();
                        // Bu metot ile satış formu üzerindeki DataGridView2'yi temizleyeceğiz.
                        anaForm.DataGridView2Temizle();
                    }

                    // SepetVerisi'nin bir kopyasını oluştur
                    DataTable tempData = SepetVerisi.Copy();
                    // Eğer Data1 diye bir veri kaynağı varsa, onu kullanmak istersen söyle, ona göre aktarırım

                    this.Close(); // Formu kapat
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Satış işlemi sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private decimal TryParseDecimal(object value)
        {
            decimal result = 0m;
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                string text = value.ToString();
                // normalize decimal separator
                text = text.Replace(",", ".");
                if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
            }
            return 0m;
        }


        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            HesaplamalariGuncelle();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

            HesaplamalariGuncelle();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            HesaplamalariGuncelle();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            dateTimePicker1.Value = DateTime.Today;
            this.Close();

        }
    }
}

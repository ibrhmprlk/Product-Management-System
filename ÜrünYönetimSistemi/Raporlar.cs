using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.collection;
using iTextSharp.text.pdf.draw;
namespace ÜrünYönetimSistemi
{
    public partial class Raporlar : Form
    {
        private DateTime eskiBaslangicTarihi;
        private DateTime eskiBitisTarihi;
        private int currentPrintRow = 0;
        private bool isPrinting = false;
        public Raporlar()
        {
            InitializeComponent();

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.ReadOnly = true;
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            this.dateTimePicker1.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker1.CustomFormat = "dd MMMM yyyy dddd";

            this.dateTimePicker2.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker2.CustomFormat = "dd MMMM yyyy dddd";

            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            checkBox1.Click += CheckBox_Click;
            checkBox2.Click += CheckBox_Click;
            checkBox3.Click += CheckBox_Click;
            checkBox4.Click += CheckBox_Click;
            checkBox5.Click += CheckBox_Click;
            checkBox6.Click += CheckBox_Click;
            checkBox7.Click += CheckBox_Click;
            checkBox8.Click += CheckBox_Click;

            // Uygulama ilk açıldığında "Tümü" seçili olsun
            checkBox1.Checked = true;
            checkBox8.Checked = true;
        }

        private void Raporlar_Load(object sender, EventArgs e)
        {
            VerileriDatagrideYukle();
            DatagridDoldur();

            // Başlangıçta tümü seçili olsun
            checkBox1.Checked = true;
            checkBox8.Checked = true;

            eskiBaslangicTarihi = dateTimePicker1.Value;
            eskiBitisTarihi = dateTimePicker2.Value;
            TumAlanlariSaltOkunurYap();
            TariheGoreFiltrele();
            ToplamlariGuncelle();
        }

        // --- Veritabanı ve DataGridView İşlemleri ---
        private void VerileriDatagrideYukle(string musteriGsm = null)
        {
            try
            {
                using (OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    string sorgu;

                    if (!string.IsNullOrEmpty(musteriGsm))
                    {
                        sorgu = @"
                            SELECT
                                FORMAT(Tarih, 'dd.MM.yyyy') & ' ' & FORMAT(Saat, 'hh:nn:ss') AS [Tarih/Saat],
                                Barkod_No AS [Barkod No],
                                Urun_Adi AS [Ürün Adı],
                                FORMAT(ToplamTutar, 'Standard') AS Tutarı,
                                'Müşteri Satışı' AS [İşlem Türü],
                                'Gelir' AS Türü
                            FROM
                                MusteriSatis
                            WHERE GsmTelefon = @GsmTelefon
                            UNION ALL
                            SELECT
                                FORMAT(Tarih, 'dd.MM.yyyy') & ' ' & FORMAT(Saat, 'hh:nn:ss') AS [Tarih/Saat],
                                Barkod_No AS [Barkod No],
                                Ürün_Adi AS [Ürün Adı],
                                FORMAT(ToplamTutar, 'Standard') AS Tutarı,
                                'Müşteri İadesi' AS [İşlem Türü],
                                'Gider' AS Türü
                            FROM
                                MusteriIade
                            WHERE GsmTelefon = @GsmTelefon;
                        ";
                    }
                    else
                    {
                        sorgu = @"
                            SELECT
                                FORMAT(Tarih, 'dd.MM.yyyy') & ' ' & FORMAT(Saat, 'hh:nn:ss') AS [Tarih/Saat],
                                Barkod_No AS [Barkod No],
                                Urun_Adi AS [Ürün Adı],
                                FORMAT(ToplamTutar, 'Standard') AS Tutarı,
                                'Ürün Satışı - ' & SatisTuru AS [Gelir Gider Sebebi],
                                'Gelir' AS Türü
                            FROM
                                UrunSatis
                            UNION ALL
                            SELECT
                                FORMAT(Tarih, 'dd.MM.yyyy') & ' ' & FORMAT(Saat, 'hh:nn:ss') AS [Tarih/Saat],
                                Barkod_No AS [Barkod No],
                                Ürün_Adi AS [Ürün Adı],
                                FORMAT(CCur(Replace(Alis_Fiyati, ',', '.')) * CCur(Replace(Miktar, ',', '.')), 'Standard') AS Tutarı,
                                'Ürün Alışı - ' & IslemTuru AS [Gelir Gider Sebebi],
                                'Gider' AS Türü
                            FROM
                                ÜrünGirişi
                            UNION ALL
                            SELECT
                                FORMAT(Tarih, 'dd.MM.yyyy') & ' ' & FORMAT(Saat, 'hh:nn:ss') AS [Tarih/Saat],
                                Barkod_No AS [Barkod No],
                                Ürün_Adi AS [Ürün Adı],
                                FORMAT(ToplamTutar, 'Standard') AS Tutarı,
                                'Müşteri İadesi' AS [Gelir Gider Sebebi],
                                'Gider' AS Türü
                            FROM
                                MusteriIade
                            UNION ALL
                            SELECT
                                FORMAT(Tarih, 'dd.MM.yyyy') & ' ' & FORMAT(Saat, 'hh:nn:ss') AS [Tarih/Saat],
                                Barkod_No AS [Barkod No],
                                Ürün_Adi AS [Ürün Adı],
                                FORMAT(ToplamTutar, 'Standard') AS Tutarı,
                                'Toptancı İadesi' AS [Gelir Gider Sebebi],
                                'Gelir' AS Türü
                            FROM
                                UrunIade;
                        ";
                    }

                    OleDbDataAdapter da = new OleDbDataAdapter(sorgu, baglanti);

                    if (!string.IsNullOrEmpty(musteriGsm))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@GsmTelefon", musteriGsm);
                    }

                    DataTable dt = new DataTable();
                    baglanti.Open();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantısı veya veri çekme sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DatagridDoldur()
        {
            try
            {
                using (OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    string sorgu = @"
                        SELECT
                            FORMAT(t1.[Tarih/Saat], 'dd.MM.yyyy') & ' ' & FORMAT(t1.[Tarih/Saat], 'hh:nn:ss') AS [Tarih/Saat],
                            'Toptancı' AS [Kişi Türü],
                            t2.ToptanciAdi AS [Ad Soyad],
                            FORMAT(t1.EklenenTutar, 'Standard') AS Tutarı,
                            'Toptancıya Borç - ' & t1.Aciklama AS [Gelir Gider Sebebi],
                            'Gider' AS Türü
                        FROM
                            BorcEkleme AS t1
                        INNER JOIN
                            Toptancilar AS t2 ON t1.GsmTelefon = t2.GsmTelefon
                        UNION ALL
                        SELECT
                            FORMAT(t1.[Tarih/Saat], 'dd.MM.yyyy') & ' ' & FORMAT(t1.[Tarih/Saat], 'hh:nn:ss') AS [Tarih/Saat],
                            'Toptancı' AS [Kişi Türü],
                            t2.ToptanciAdi AS [Ad Soyad],
                            FORMAT(t1.OdenenTutar, 'Standard') AS Tutarı,
                            'Toptancı Ödemesi - ' & t1.OdemeSekli AS [Gelir Gider Sebebi],
                            'Gelir' AS Türü
                        FROM
                            BorcOdeme AS t1
                        INNER JOIN
                            Toptancilar AS t2 ON t1.GsmTelefon = t2.GsmTelefon
                        UNION ALL
                        SELECT
                            FORMAT(t1.[Tarih/Saat], 'dd.MM.yyyy') & ' ' & FORMAT(t1.[Tarih/Saat], 'hh:nn:ss') AS [Tarih/Saat],
                            'Müşteri' AS [Kişi Türü],
                            t2.MusteriAdi AS [Ad Soyad],
                            FORMAT(t1.EklenenTutar, 'Standard') AS Tutarı,
                            'Müşteriden Alacak - ' & t1.Aciklama AS [Gelir Gider Sebebi],
                            'Gider' AS Türü
                        FROM
                            VeresiyeEkle AS t1
                        INNER JOIN
                            Musteriler AS t2 ON t1.GsmTelefon = t2.GsmTelefon
                        UNION ALL
                        SELECT
                            FORMAT(t1.[Tarih/Saat], 'dd.MM.yyyy') & ' ' & FORMAT(t1.[Tarih/Saat], 'hh:nn:ss') AS [Tarih/Saat],
                            'Müşteri' AS [Kişi Türü],
                            t2.MusteriAdi AS [Ad Soyad],
                            FORMAT(t1.OdenenTutar, 'Standard') AS Tutarı,
                            'Müşteri Tahsilatı - ' & t1.OdemeSekli AS [Gelir Gider Sebebi],
                            'Gelir' AS Türü
                        FROM
                            Tahsilat AS t1
                        INNER JOIN
                            Musteriler AS t2 ON t1.GsmTelefon = t2.GsmTelefon;
                    ";

                    OleDbDataAdapter da = new OleDbDataAdapter(sorgu, baglanti);
                    DataTable dt = new DataTable();

                    baglanti.Open();
                    da.Fill(dt);

                    dataGridView2.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantısı veya veri çekme sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Filtreleme ve Tarih Değişiklikleri ---
        private void TariheGoreFiltrele()
        {
            DateTime baslangicTarihi = dateTimePicker1.Value.Date;
            DateTime bitisTarihi = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);

            string tarihFiltresi = string.Format(
                "[Tarih/Saat] >= #{0}# AND [Tarih/Saat] <= #{1}#",
                baslangicTarihi.ToString("MM/dd/yyyy HH:mm:ss"),
                bitisTarihi.ToString("MM/dd/yyyy HH:mm:ss"));

            // DataGridView1 için filtre
            string dgv1Filtresi = tarihFiltresi;
            if (!checkBox1.Checked)
            {
                string ekFiltre = "";
                if (checkBox2.Checked) ekFiltre = "[Gelir Gider Sebebi] LIKE 'Ürün Satışı*'";
                else if (checkBox3.Checked) ekFiltre = "[Gelir Gider Sebebi] LIKE 'Ürün Alışı*'";
                else if (checkBox4.Checked) ekFiltre = "[Gelir Gider Sebebi] = 'Müşteri İadesi'";
                else if (checkBox5.Checked) ekFiltre = "[Gelir Gider Sebebi] = 'Toptancı İadesi'";
                if (!string.IsNullOrEmpty(ekFiltre)) dgv1Filtresi += " AND (" + ekFiltre + ")";
            }

            // DataGridView2 için filtre
            string dgv2Filtresi = tarihFiltresi;
            if (!checkBox8.Checked)
            {
                string ekFiltre = "";
                if (checkBox7.Checked) ekFiltre = "[Kişi Türü] = 'Toptancı'";
                else if (checkBox6.Checked) ekFiltre = "[Kişi Türü] = 'Müşteri'";
                if (!string.IsNullOrEmpty(ekFiltre)) dgv2Filtresi += " AND (" + ekFiltre + ")";
            }

            // DataGridView1 (Ürün Hareketleri)
            DataTable urunHareketleriTablosu = dataGridView1.DataSource as DataTable;
            if (urunHareketleriTablosu != null)
            {
                urunHareketleriTablosu.DefaultView.RowFilter = dgv1Filtresi;
            }

            // DataGridView2 (Borç/Alacak)
            DataTable borcAlacakTablosu = dataGridView2.DataSource as DataTable;
            if (borcAlacakTablosu != null)
            {
                borcAlacakTablosu.DefaultView.RowFilter = dgv2Filtresi;
            }

            // Filtreleme sonrası toplamları güncelle
            ToplamlariGuncelle();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            TariheGoreFiltrele();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            TariheGoreFiltrele();
        }

        // --- Toplam Hesaplama ve TextBox Güncelleme ---
        private void ToplamlariGuncelle()
        {
            // Genel Toplamlar
            decimal toplamGelir = 0;
            decimal toplamGider = 0;
            decimal toplamKdv = 0;

            // Satış Türlerine Göre Detaylı Toplamlar
            decimal veresiyeSatis = 0;
            decimal nakitSatis = 0;
            decimal havaleSatis = 0;
            decimal krediSatis = 0;
            decimal nakitKrediSatis = 0;
            decimal taksitliSatis = 0;
            decimal taksitliPesinati = 0;
            decimal satisKar = 0;

            // Diğer Toplamlar
            decimal urunGirisiKrediKarti = 0;
            decimal urunGirisiHavale = 0;
            decimal urunGirisiToptanciBorc = 0;
            decimal urunGirisiNakit = 0;
            decimal iadeEdilenNakitOdendi = 0;
            decimal iadeEdilenBorctanDusuldu = 0;
            decimal iadeAlinanNakitOdendi = 0;
            decimal iadeAlinanKrediKarti = 0;
            decimal iadeAlinanBorctanDusuldu = 0;
            decimal musteriOdemesiNakit = 0;
            decimal musteriOdemesiKrediKarti = 0;
            decimal musteriOdemesiHavale = 0;
            decimal toptanciyaOdemeNakit = 0;
            decimal toptanciyaOdemeKrediKarti = 0;
            decimal toptanciyaOdemeHavale = 0;

            decimal iadeEdilenToplami = 0;
            decimal musteriOdemeleri = 0;
            decimal digerGiderler = 0;
            decimal urunGirisiToplami = 0;
            decimal iadeAlinanToplami = 0;
            decimal toptanciyaOdeme = 0;

            System.Globalization.CultureInfo trCulture = new System.Globalization.CultureInfo("tr-TR");

            // dataGridView1 (Ürün Hareketleri) için döngü
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Visible)
                {
                    string tutarStr = row.Cells["Tutarı"]?.Value?.ToString() ?? "0";
                    tutarStr = tutarStr.Replace(".", "").Replace(',', '.');

                    if (decimal.TryParse(tutarStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal tutar))
                    {
                        string gelirGiderSebebi = row.Cells["Gelir Gider Sebebi"]?.Value?.ToString() ?? string.Empty;
                        string turu = row.Cells["Türü"]?.Value?.ToString() ?? string.Empty;

                        if (turu == "Gelir")
                        {
                            toplamGelir += tutar;
                        }
                        else if (turu == "Gider")
                        {
                            toplamGider += tutar;
                        }

                        // Detaylı ayrım
                        if (gelirGiderSebebi.Contains("Ürün Satışı"))
                        {
                            if (gelirGiderSebebi.Contains("Veresiye")) veresiyeSatis += tutar;
                            else if (gelirGiderSebebi.Contains("Nakit")) nakitSatis += tutar;
                            else if (gelirGiderSebebi.Contains("Havale")) havaleSatis += tutar;
                            else if (gelirGiderSebebi.Contains("Kredi Kartı")) krediSatis += tutar;
                            else if (gelirGiderSebebi.Contains("Nakit + Kredi Kartı")) nakitKrediSatis += tutar;
                            else if (gelirGiderSebebi.Contains("Taksitli")) taksitliSatis += tutar;
                        }
                        else if (gelirGiderSebebi.Contains("Ürün Alışı"))
                        {
                            urunGirisiToplami += tutar;
                            if (gelirGiderSebebi.Contains("Kredi Kartı")) urunGirisiKrediKarti += tutar;
                            else if (gelirGiderSebebi.Contains("Havale")) urunGirisiHavale += tutar;
                            else if (gelirGiderSebebi.Contains("Toptancı Borç")) urunGirisiToptanciBorc += tutar;
                            else if (gelirGiderSebebi.Contains("Nakit")) urunGirisiNakit += tutar;
                        }
                        else if (gelirGiderSebebi == "Müşteri İadesi")
                        {
                            iadeEdilenToplami += tutar;
                        }
                        else if (gelirGiderSebebi == "Toptancı İadesi")
                        {
                            iadeAlinanToplami += tutar;
                        }
                    }

                    // Kar ve KDV bilgisi
                    if (dataGridView1.Columns.Contains("Kar"))
                    {
                        if (row.Cells["Kar"]?.Value != null && decimal.TryParse(row.Cells["Kar"].Value.ToString(), out decimal kar))
                        {
                            satisKar += kar;
                        }
                    }
                    if (dataGridView1.Columns.Contains("KDV Tutarı"))
                    {
                        if (row.Cells["KDV Tutarı"]?.Value != null && decimal.TryParse(row.Cells["KDV Tutarı"].Value.ToString(), out decimal kdv))
                        {
                            toplamKdv += kdv;
                        }
                    }
                }
            }

            // dataGridView2 (Borç/Alacak) için döngü
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Visible)
                {
                    string tutarStr = row.Cells["Tutarı"]?.Value?.ToString() ?? "0";
                    tutarStr = tutarStr.Replace(".", "").Replace(',', '.');

                    if (decimal.TryParse(tutarStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal tutar))
                    {
                        string gelirGiderSebebi = row.Cells["Gelir Gider Sebebi"]?.Value?.ToString() ?? string.Empty;
                        string kisiTuru = row.Cells["Kişi Türü"]?.Value?.ToString() ?? string.Empty;
                        string turu = row.Cells["Türü"]?.Value?.ToString() ?? string.Empty;

                        if (turu == "Gelir")
                        {
                            toplamGelir += tutar;
                        }
                        else if (turu == "Gider")
                        {
                            toplamGider += tutar;
                        }

                        // Detaylı toplamlar
                        if (gelirGiderSebebi.Contains("Müşteri Tahsilatı"))
                        {
                            musteriOdemeleri += tutar;
                            if (gelirGiderSebebi.Contains("Nakit")) musteriOdemesiNakit += tutar;
                            else if (gelirGiderSebebi.Contains("Kredi Kartı")) musteriOdemesiKrediKarti += tutar;
                            else if (gelirGiderSebebi.Contains("Havale")) musteriOdemesiHavale += tutar;
                        }
                        else if (gelirGiderSebebi.Contains("Toptancı Ödemesi"))
                        {
                            toptanciyaOdeme += tutar;
                            if (gelirGiderSebebi.Contains("Nakit")) toptanciyaOdemeNakit += tutar;
                            else if (gelirGiderSebebi.Contains("Kredi Kartı")) toptanciyaOdemeKrediKarti += tutar;
                            else if (gelirGiderSebebi.Contains("Havale")) toptanciyaOdemeHavale += tutar;
                        }
                        else if (gelirGiderSebebi.Contains("Müşteriden Alacak"))
                        {
                            digerGiderler += tutar;
                        }
                        else if (gelirGiderSebebi.Contains("Toptancıya Borç"))
                        {
                            digerGiderler += tutar;
                        }
                    }
                }
            }

            // Toplamları ilgili TextBox'lara atama
            textBox1.Text = (nakitSatis + havaleSatis + krediSatis + nakitKrediSatis + taksitliSatis).ToString("N2", trCulture);
            textBox2.Text = iadeEdilenToplami.ToString("N2", trCulture);
            textBox3.Text = musteriOdemeleri.ToString("N2", trCulture);
            textBox4.Text = digerGiderler.ToString("N2", trCulture);
            textBox5.Text = urunGirisiToplami.ToString("N2", trCulture);
            textBox6.Text = iadeAlinanToplami.ToString("N2", trCulture);
            textBox7.Text = toptanciyaOdeme.ToString("N2", trCulture);

            // Kar/Zarar durumunu gösteren kutu
            decimal karZarar = toplamGelir - toplamGider;
            decimal textBox8Degeri = Math.Max(0, karZarar); // Negatifse 0 yap
            if (checkBox1.Checked) textBox8.Text = textBox8Degeri.ToString("N2", trCulture);
            else if (checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked && !checkBox5.Checked) textBox8.Text = Math.Max(0, toplamGelir).ToString("N2", trCulture);
            else if (checkBox3.Checked && !checkBox2.Checked && !checkBox4.Checked && !checkBox5.Checked) textBox8.Text = Math.Max(0, toplamGider).ToString("N2", trCulture);
            else textBox8.Text = textBox8Degeri.ToString("N2", trCulture);

            // Detaylı satış ve kar kutuları
            textBox22.Text = veresiyeSatis.ToString("N2", trCulture);
            textBox24.Text = nakitSatis.ToString("N2", trCulture);
            textBox23.Text = havaleSatis.ToString("N2", trCulture);
            textBox21.Text = krediSatis.ToString("N2", trCulture);
            textBox20.Text = nakitKrediSatis.ToString("N2", trCulture);
            textBox19.Text = taksitliSatis.ToString("N2", trCulture);
            textBox18.Text = taksitliPesinati.ToString("N2", trCulture);
            textBox17.Text = satisKar.ToString("N2", trCulture);


            // Yeni eklenen diğer textboxlar
            textBox30.Text = urunGirisiKrediKarti.ToString("N2", trCulture);
            textBox32.Text = urunGirisiHavale.ToString("N2", trCulture);
            textBox31.Text = urunGirisiToptanciBorc.ToString("N2", trCulture);
            textBox28.Text = iadeEdilenBorctanDusuldu.ToString("N2", trCulture);
            textBox27.Text = iadeAlinanNakitOdendi.ToString("N2", trCulture);
            textBox25.Text = iadeAlinanKrediKarti.ToString("N2", trCulture);
            textBox26.Text = iadeAlinanBorctanDusuldu.ToString("N2", trCulture);
            textBox29.Text = iadeEdilenBorctanDusuldu.ToString("N2", trCulture);
            textBox15.Text = musteriOdemesiNakit.ToString("N2", trCulture);
            textBox14.Text = musteriOdemesiKrediKarti.ToString("N2", trCulture);
            textBox13.Text = musteriOdemesiHavale.ToString("N2", trCulture);
            textBox12.Text = toptanciyaOdemeNakit.ToString("N2", trCulture);
            textBox11.Text = toptanciyaOdemeKrediKarti.ToString("N2", trCulture);
            textBox10.Text = toptanciyaOdemeHavale.ToString("N2", trCulture);
            textBox9.Text = urunGirisiNakit.ToString("N2", trCulture);
        }
        private void TumAlanlariSaltOkunurYap()
        {
            // Genel Toplam ve Ana Özet Kutuları
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox6.ReadOnly = true;
            textBox7.ReadOnly = true;
            textBox8.ReadOnly = true;

            // Detaylı Satış ve Kar Kutuları
            textBox22.ReadOnly = true; // Veresiye Satis
            textBox24.ReadOnly = true; // Nakit Satis
            textBox23.ReadOnly = true; // Havale Satis
            textBox21.ReadOnly = true; // Kredi Satis
            textBox20.ReadOnly = true; // Nakit Kredi Satis
            textBox19.ReadOnly = true; // Taksitli Satis
            textBox18.ReadOnly = true; // Taksitli Pesinati
            textBox17.ReadOnly = true; // Satis Kar

            // Yeni Eklenen Gider ve Ödeme Detayları
            textBox30.ReadOnly = true; // Urun Girisi Kredi Karti
            textBox32.ReadOnly = true; // Urun Girisi Havale
            textBox31.ReadOnly = true; // Urun Girisi Toptanci Borc
            textBox28.ReadOnly = true; // Iade Edilen Borctan Dusuldu
            textBox27.ReadOnly = true; // Iade Alinan Nakit Odendi
            textBox25.ReadOnly = true; // Iade Alinan Kredi Karti
            textBox26.ReadOnly = true; // Iade Alinan Borctan Dusuldu
            textBox29.ReadOnly = true; // Iade Edilen Borctan Dusuldu (Tekrar kullanılmış)
            textBox15.ReadOnly = true; // Musteri Odemesi Nakit
            textBox14.ReadOnly = true; // Musteri Odemesi Kredi Karti
            textBox13.ReadOnly = true; // Musteri Odemesi Havale
            textBox12.ReadOnly = true; // Toptanciya Odeme Nakit
            textBox11.ReadOnly = true; // Toptanciya Odeme Kredi Karti
            textBox10.ReadOnly = true; // Toptanciya Odeme Havale
            textBox9.ReadOnly = true;  // Urun Girisi Nakit
        }
        // --- CheckBox İşlemleri ---
        private void CheckBox_Click(object sender, EventArgs e)
        {
            CheckBox clickedBox = sender as CheckBox;

            if (clickedBox == null) return;

            // Tıklanan kutunun durumunu kontrol et. Eğer seçimi kaldırılıyorsa, tekrar seçili hale getir.
            if (clickedBox.Checked == false)
            {
                clickedBox.Checked = true;
                return; // İşlemi burada sonlandır
            }

            // DataGridView1 için checkbox işlemleri
            if (clickedBox == checkBox1 || clickedBox == checkBox2 || clickedBox == checkBox3 ||
                clickedBox == checkBox4 || clickedBox == checkBox5)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                clickedBox.Checked = true;
            }
            // DataGridView2 için checkbox işlemleri
            else if (clickedBox == checkBox6 || clickedBox == checkBox7 || clickedBox == checkBox8)
            {
                checkBox6.Checked = false;
                checkBox7.Checked = false;
                checkBox8.Checked = false;
                clickedBox.Checked = true;
            }

            // Filtreleme işlemini çağır
            TariheGoreFiltrele();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Hangi DataGridView'ın kullanılacağını kontrol ediyoruz (Bu senaryoda dataGridView1)
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Tabloda aktarılacak veri bulunmamaktadır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Dosyası|*.xlsx";

                // Eğer DataGridView müşteri Gsm'ye göre filtrelenmişse (musteriGsm != null ise)
                // Başlıkta bunu belirtebiliriz. Varsayılan olarak Genel İşlem başlığını kullanıyoruz.
                string dosyaAdi = "Rapor_" + ".xlsx";

                // Eğer bu formun üstünde Müşteri adı/GSM'si görünüyor ve bu detayı yansıtıyorsa
                // Örneğin: Gsm bilgisi textBox1'de ise (Bu bir tahmindir, formunuza göre ayarlayın)
                // if (!string.IsNullOrEmpty(textBox1.Text)) 
                // {
                //     dosyaAdi = "MusteriDetayRaporu_" + textBox1.Text + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
                //     raporBaslik = "MÜŞTERİ HESAP DETAY RAPORU: " + textBox1.Text;
                // }

                sfd.FileName = dosyaAdi;

                if (sfd.ShowDialog() != DialogResult.OK) return;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("İşlem Detayı");
                    int currentRow = 1;

                    // 1. Ana Başlık
                    worksheet.Range(currentRow, 1, currentRow, dataGridView1.Columns.Count).Merge();

                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 16;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    currentRow += 2; // Başlık sonrası 2 satır boşluk

                    int headerRow = currentRow;

                    // 2. Kolon Başlıkları (Headers)
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        worksheet.Cell(currentRow, i + 1).Value = dataGridView1.Columns[i].HeaderText;
                        worksheet.Cell(currentRow, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                    currentRow++;

                    // 3. Satır Verileri (Data Rows)
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (dataGridView1.Rows[i].IsNewRow) continue;

                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            worksheet.Cell(currentRow + i, j + 1).Value =
                                dataGridView1.Rows[i].Cells[j].Value?.ToString() ?? "";
                        }
                    }

                    // --- Biçimlendirme Uygulama ---

                    // Kenarlıklar
                    var tableRange = worksheet.Range(headerRow, 1, currentRow + dataGridView1.Rows.Count - 1, dataGridView1.Columns.Count);
                    tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // Sütun genişlikleri (Otomatik genişlik ayarı)
                    worksheet.Columns().AdjustToContents();
                    for (int i = 1; i <= dataGridView1.Columns.Count; i++)
                    {
                        // Minimum genişliği koru
                        if (worksheet.Column(i).Width < 18)
                            worksheet.Column(i).Width = 18;
                    }

                    // Satır yüksekliği
                    worksheet.Rows().Height = 22.22;

                    // Para Birimi Sütunu Sağa Hizalama (4. Sütun: Tutarı)
                    // SQL sorgunuzdaki 4. sütun [Tutarı] olduğu için burayı sağa hizalıyoruz.
                    worksheet.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    // Grid çizgilerini gizle
                    worksheet.ShowGridLines = false;

                    // Dosyayı kaydet
                    workbook.SaveAs(sfd.FileName);

                    MessageBox.Show("Veriler Excel dosyasına başarıyla aktarıldı.", "Başarılı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel'e aktarılırken hata oluştu: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Verilerin dataGridView2'de olduğunu varsayıyoruz.
            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Aktarılacak cari hesap hareketi bulunmamaktadır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Dosyası|*.xlsx";

                // Dinamik dosya adı oluşturma
                string dosyaAdi = "CariHesapHareketRaporu_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                string raporBaslik = "TOPTANCI VE MÜŞTERİ CARİ HESAP HAREKET RAPORU";

                sfd.FileName = dosyaAdi;

                if (sfd.ShowDialog() != DialogResult.OK) return;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Cari Hesap Hareketleri");
                    int currentRow = 1;

                    // 1. Ana Başlık
                    worksheet.Range(currentRow, 1, currentRow, dataGridView2.Columns.Count).Merge();
                    worksheet.Cell(currentRow, 1).Value = raporBaslik;
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 16;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    currentRow += 2; // Başlık sonrası 2 satır boşluk

                    int headerRow = currentRow;

                    // 2. Kolon Başlıkları (Headers)
                    for (int i = 0; i < dataGridView2.Columns.Count; i++)
                    {
                        worksheet.Cell(currentRow, i + 1).Value = dataGridView2.Columns[i].HeaderText;
                        worksheet.Cell(currentRow, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.DarkGray; // Başlık için koyu gri
                        worksheet.Cell(currentRow, i + 1).Style.Font.FontColor = XLColor.White;
                    }
                    currentRow++;

                    // 3. Satır Verileri (Data Rows)
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        if (dataGridView2.Rows[i].IsNewRow) continue;

                        for (int j = 0; j < dataGridView2.Columns.Count; j++)
                        {
                            worksheet.Cell(currentRow + i, j + 1).Value =
                                dataGridView2.Rows[i].Cells[j].Value?.ToString() ?? "";
                        }
                    }

                    // --- Biçimlendirme Uygulama ---

                    // Kenarlıklar
                    var tableRange = worksheet.Range(headerRow, 1, currentRow + dataGridView2.Rows.Count - 1, dataGridView2.Columns.Count);
                    tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // Sütun genişlikleri (Otomatik genişlik + minimum genişlik ayarı)
                    worksheet.Columns().AdjustToContents();
                    for (int i = 1; i <= dataGridView2.Columns.Count; i++)
                    {
                        // Minimum genişliği 15, Gelir Gider Sebebi sütununu daha geniş yap
                        if (i == 5) // Gelir Gider Sebebi sütunu
                            worksheet.Column(i).Width = 35;
                        else if (worksheet.Column(i).Width < 15)
                            worksheet.Column(i).Width = 15;
                    }

                    // Satır yüksekliği
                    worksheet.Rows().Height = 22.22;

                    // Para Birimi Sütunu Sağa Hizalama (4. Sütun: Tutarı)
                    worksheet.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    // Tarih/Saat Sütununu Sola Hizalama (1. Sütun)
                    worksheet.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    // Grid çizgilerini gizle
                    worksheet.ShowGridLines = false;

                    // Dosyayı kaydet
                    workbook.SaveAs(sfd.FileName);

                    MessageBox.Show("Cari hesap hareketleri Excel dosyasına başarıyla aktarıldı.", "Başarılı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel'e aktarılırken hata oluştu: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("PDF'e aktarılacak veri bulunmamaktadır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // .NET Core/.NET 5+ için encoding sağlayıcısını kaydet (windows-1252 hatasını önler)
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF Dosyası|*.pdf";
                // Dosya adını bir zaman damgasıyla dinamik yapıyoruz.
                sfd.FileName = "GenelRaporu" + ".pdf";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                // Türkçeyi destekleyen fontu (genellikle Arial ya da bir kod sayfasını) tanımlama
                string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                BaseFont bf = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                // Fontlar
                iTextSharp.text.Font fontBaslik = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                iTextSharp.text.Font fontSutunBaslik = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.WHITE);
                iTextSharp.text.Font fontVeri = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

                using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    // Belge oluşturma (yatay A4 yap, sıkışmayı önle)
                    iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 20, 20);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // 1. Ana Başlık
                    Paragraph baslik = new Paragraph("GENEL İŞLEMLER RAPORU", fontBaslik);
                    baslik.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    baslik.SpacingAfter = 20f;
                    document.Add(baslik);

                    // 2. Tablo Oluşturma
                    int sutunSayisi = dataGridView1.Columns.Count;
                    PdfPTable pdfTable = new PdfPTable(sutunSayisi);
                    pdfTable.WidthPercentage = 100;
                    pdfTable.DefaultCell.Padding = 5;
                    pdfTable.DefaultCell.BorderWidth = 1;
                    pdfTable.DefaultCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE; // Hücre iç hizalama

                    // Sütun genişliklerini ayarla (daha dengeli, sıkışmayı önle - toplam 10 birim)
                    float[] genislikler = new float[sutunSayisi];
                    genislikler[0] = 1.5f; // Tarih/Saat
                    genislikler[1] = 1.0f; // Barkod No
                    genislikler[2] = 2.5f; // Ürün Adı (en geniş, uzun metinler için)
                    genislikler[3] = 1.2f; // Tutarı
                    genislikler[4] = 2.0f; // Gelir Gider Sebebi / İşlem Türü (uzun açıklamalar için)
                    genislikler[5] = 0.8f; // Türü (en dar)
                    pdfTable.SetWidths(genislikler);

                    // 3. Sütun Başlıkları (Headers)
                    for (int i = 0; i < sutunSayisi; i++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dataGridView1.Columns[i].HeaderText, fontSutunBaslik));
                        cell.BackgroundColor = new iTextSharp.text.BaseColor(64, 64, 64); // Koyu gri arka plan
                        cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        cell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        cell.Padding = 8f; // Padding artır
                        pdfTable.AddCell(cell);
                    }

                    // 4. Veri Satırları (Data Rows)
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (dataGridView1.Rows[i].IsNewRow) continue;

                        for (int j = 0; j < sutunSayisi; j++)
                        {
                            string hucreDegeri = dataGridView1.Rows[i].Cells[j].Value?.ToString() ?? "";
                            PdfPCell cell = new PdfPCell(new Phrase(hucreDegeri, fontVeri));
                            cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                            cell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;

                            // "Tutarı" sütununu (3. sütun, j=3) sağa hizala
                            if (j == 3)
                            {
                                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                            }

                            // Uzun metinlerde wrap et (otomatik)
                            cell.NoWrap = false;

                            pdfTable.AddCell(cell);
                        }
                    }

                    // Tabloyu belgeye ekle
                    document.Add(pdfTable);

                    // Belgeyi kapat
                    document.Close();
                    writer.Close();
                }

                MessageBox.Show("Veriler PDF dosyasına başarıyla aktarıldı.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("PDF'e aktarılırken hata oluştu: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {


            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("PDF'e aktarılacak veri bulunmamaktadır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // .NET Core/.NET 5+ için encoding sağlayıcısını kaydet (windows-1252 hatasını önler)
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF Dosyası|*.pdf";
                // Dosya adını bir zaman damgasıyla dinamik yapıyoruz.
                sfd.FileName = "BorçTahsilatRaporu_" + ".pdf";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                // Türkçeyi destekleyen fontu (genellikle Arial ya da bir kod sayfasını) tanımlama
                string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                BaseFont bf = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                // Fontlar
                iTextSharp.text.Font fontBaslik = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                iTextSharp.text.Font fontSutunBaslik = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.WHITE);
                iTextSharp.text.Font fontVeri = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

                using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    // Belge oluşturma (yatay A4, sütun sayısı için)
                    iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 20, 20);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // 1. Ana Başlık
                    Paragraph baslik = new Paragraph("BORÇ/TAHSİLAT RAPORU", fontBaslik);
                    baslik.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    baslik.SpacingAfter = 20f;
                    document.Add(baslik);

                    // 2. Tablo Oluşturma
                    int sutunSayisi = dataGridView2.Columns.Count;
                    PdfPTable pdfTable = new PdfPTable(sutunSayisi);
                    pdfTable.WidthPercentage = 100;
                    pdfTable.DefaultCell.Padding = 5;
                    pdfTable.DefaultCell.BorderWidth = 1;
                    pdfTable.DefaultCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;

                    // Sütun genişliklerini ayarla (dengeli, sıkışmayı önle)
                    float[] genislikler = new float[sutunSayisi];
                    genislikler[0] = 1.5f; // Tarih/Saat
                    genislikler[1] = 1.0f; // Kişi Türü
                    genislikler[2] = 2.0f; // Ad Soyad
                    genislikler[3] = 1.2f; // Tutarı
                    genislikler[4] = 2.5f; // Gelir Gider Sebebi (en geniş)
                    genislikler[5] = 0.8f; // Türü
                    pdfTable.SetWidths(genislikler);

                    // 3. Sütun Başlıkları (Headers)
                    for (int i = 0; i < sutunSayisi; i++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dataGridView2.Columns[i].HeaderText, fontSutunBaslik));
                        cell.BackgroundColor = new iTextSharp.text.BaseColor(64, 64, 64); // Koyu gri arka plan
                        cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        cell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                        cell.Padding = 8f;
                        pdfTable.AddCell(cell);
                    }

                    // 4. Veri Satırları (Data Rows)
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        if (dataGridView2.Rows[i].IsNewRow) continue;

                        for (int j = 0; j < sutunSayisi; j++)
                        {
                            string hucreDegeri = dataGridView2.Rows[i].Cells[j].Value?.ToString() ?? "";
                            PdfPCell cell = new PdfPCell(new Phrase(hucreDegeri, fontVeri));
                            cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                            cell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;

                            // "Tutarı" sütununu (3. sütun, j=3) sağa hizala
                            if (j == 3)
                            {
                                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                            }

                            // Uzun metinlerde wrap et
                            cell.NoWrap = false;

                            pdfTable.AddCell(cell);
                        }
                    }

                    // Tabloyu belgeye ekle (otomatik sayfa kırar)
                    document.Add(pdfTable);

                    // Belgeyi kapat
                    document.Close();
                    writer.Close();
                }

                MessageBox.Show("Veriler PDF dosyasına başarıyla aktarıldı.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("PDF'e aktarılırken hata oluştu: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = eskiBaslangicTarihi;
            dateTimePicker2.Value = eskiBitisTarihi;

            // Filtreyi de tekrar uygula
            TariheGoreFiltrele();
        }
    }
}
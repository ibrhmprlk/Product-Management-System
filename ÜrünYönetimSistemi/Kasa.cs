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
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml;
using Excel = Microsoft.Office.Interop.Excel;


namespace ÜrünYönetimSistemi
{
    public partial class Kasa : Form
    {
        private DateTime eskiBaslangicTarihi;
        private DateTime eskiBitisTarihi;
        public Kasa()
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

            // CheckBox olaylarını atama
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
        }

        private void Kasa_Load(object sender, EventArgs e)
        {
            VerileriDatagrideYukle();
            DatagridDoldur();

            // Başlangıçta tümü seçili olsun
            checkBox1.Checked = true;

            eskiBaslangicTarihi = dateTimePicker1.Value;
            eskiBitisTarihi = dateTimePicker2.Value;

            TariheGoreFiltrele();
            ToplamlariGuncelle();
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox currentCheckBox = sender as CheckBox;

            // Bir CheckBox seçiliyse diğerlerini devredışı bırak
            if (currentCheckBox.Checked)
            {
                if (currentCheckBox != checkBox1)
                    checkBox1.Checked = false;
                if (currentCheckBox != checkBox2)
                    checkBox2.Checked = false;
                if (currentCheckBox != checkBox3)
                    checkBox3.Checked = false;
            }
            // Eğer hiçbir şey seçili değilse (seçim kaldırılırsa), Tümü'nü seçili yap
            else if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked)
            {
                checkBox1.Checked = true;
            }

            // Filtrelemeyi tetikle
            TariheGoreFiltrele();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            TariheGoreFiltrele();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            TariheGoreFiltrele();
        }

        private void TariheGoreFiltrele()
        {
            DateTime baslangicTarihi = dateTimePicker1.Value.Date;
            DateTime bitisTarihi = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);

            string tarihFiltresi = string.Format(
                "[Tarih/Saat] >= #{0}# AND [Tarih/Saat] <= #{1}#",
                baslangicTarihi.ToString("MM/dd/yyyy HH:mm:ss"),
                bitisTarihi.ToString("MM/dd/yyyy HH:mm:ss"));

            string gelirGiderFiltresi = "";

            if (checkBox2.Checked) // Sadece Gelir
            {
                gelirGiderFiltresi = " AND Türü = 'Gelir'";
            }
            else if (checkBox3.Checked) // Sadece Gider
            {
                gelirGiderFiltresi = " AND Türü = 'Gider'";
            }
            // 'Tümü' seçiliyse ekstra filtreye gerek yok

            string sonFiltreMetni = tarihFiltresi + gelirGiderFiltresi;

            // DataGridView1 (Ürün Hareketleri)
            DataTable urunHareketleriTablosu = dataGridView1.DataSource as DataTable;
            if (urunHareketleriTablosu != null && urunHareketleriTablosu.Columns.Contains("Tarih/Saat"))
            {
                urunHareketleriTablosu.DefaultView.RowFilter = sonFiltreMetni;
            }

            // DataGridView2 (Borç/Alacak)
            DataTable borcAlacakTablosu = dataGridView2.DataSource as DataTable;
            if (borcAlacakTablosu != null && borcAlacakTablosu.Columns.Contains("Tarih/Saat"))
            {
                borcAlacakTablosu.DefaultView.RowFilter = sonFiltreMetni;
            }

            // Filtreleme sonrası toplamları güncelle
            ToplamlariGuncelle();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = eskiBaslangicTarihi;
            dateTimePicker2.Value = eskiBitisTarihi;

            TariheGoreFiltrele();
        }

        private void ToplamlariGuncelle()
        {
            decimal toplamGelir = 0;
            decimal toplamGider = 0;

            System.Globalization.CultureInfo trCulture = new System.Globalization.CultureInfo("tr-TR");

            // dataGridView1'deki (Ürün Hareketleri) görünür satırları işler
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Yalnızca görünür olan satırları dikkate al
                if (row.Visible && row.Cells["Türü"].Value != null && row.Cells["Tutarı"].Value != null)
                {
                    string tutarStr = row.Cells["Tutarı"].Value.ToString();
                    tutarStr = tutarStr.Replace(".", "").Replace(",", ".");

                    if (decimal.TryParse(tutarStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal tutar))
                    {
                        if (row.Cells["Türü"].Value.ToString().Equals("Gelir", StringComparison.OrdinalIgnoreCase))
                        {
                            toplamGelir += Math.Abs(tutar); // Negatif gelirleri pozitife çevir
                        }
                        else if (row.Cells["Türü"].Value.ToString().Equals("Gider", StringComparison.OrdinalIgnoreCase))
                        {
                            toplamGider += Math.Abs(tutar); // Negatif giderleri pozitife çevir
                        }
                    }
                }
            }

            // dataGridView2'deki (Borç/Alacak) görünür satırları işler
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                // Yalnızca görünür olan satırları dikkate al
                if (row.Visible && row.Cells["Türü"].Value != null && row.Cells["Tutarı"].Value != null)
                {
                    string tutarStr = row.Cells["Tutarı"].Value.ToString();
                    tutarStr = tutarStr.Replace(".", "").Replace(",", ".");

                    if (decimal.TryParse(tutarStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal tutar))
                    {
                        if (row.Cells["Türü"].Value.ToString().Equals("Gelir", StringComparison.OrdinalIgnoreCase))
                        {
                            toplamGelir += Math.Abs(tutar); // Negatif gelirleri pozitife çevir
                        }
                        else if (row.Cells["Türü"].Value.ToString().Equals("Gider", StringComparison.OrdinalIgnoreCase))
                        {
                            toplamGider += Math.Abs(tutar); // Negatif giderleri pozitife çevir
                        }
                    }
                }
            }

            textBox1.Text = toplamGelir.ToString("N2", trCulture);
            textBox2.Text = toplamGider.ToString("N2", trCulture);

            // Toplam değerini duruma göre hesapla
            if (checkBox2.Checked) // Sadece Gelir seçiliyse
            {
                textBox3.Text = toplamGelir.ToString("N2", trCulture);
            }
            else if (checkBox3.Checked) // Sadece Gider seçiliyse
            {
                textBox3.Text = toplamGider.ToString("N2", trCulture);
            }
            else // Tümü seçiliyse
            {
                // Giderden geliri çıkar ve mutlak değer al, böylece negatif olmaz
                textBox3.Text = Math.Abs(toplamGider - toplamGelir).ToString("N2", trCulture);
            }
        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            GelirGiderGrafik gelirgiderForm = new GelirGiderGrafik();

            // Formu göster
            gelirgiderForm.Show();
        }


        private void VerileriDatagrideYukle(string musteriGsm = null)
        {
            try
            {
                using (OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    string sorgu;

                    // Eğer bir müşteri seçiliyse, sorguyu bu duruma göre özelleştir.
                    if (!string.IsNullOrEmpty(musteriGsm))
                    {
                        sorgu = @"
                    SELECT
                        FORMAT(Tarih, 'dd.MM.yyyy') & ' ' & FORMAT(Saat, 'hh:nn:ss') AS [Tarih/Saat],
                        Barkod_No AS [Barkod No],
                        Urun_Adi AS [Ürün Adı],
                        FORMAT(ToplamTutar, 'Standard') AS Tutarı,
                        'Müşteri Satışı - ' & SatisTuru AS [Gelir Gider Sebebi],
                        'Gelir' AS Türü
                    FROM
                        MusteriSatis
                    WHERE GsmTelefon = @GsmTelefon
                    UNION ALL
                    SELECT
                        FORMAT(t1.Tarih, 'dd.MM.yyyy') & ' ' & FORMAT(t1.Saat, 'hh:nn:ss') AS [Tarih/Saat],
                        t1.Barkod_No AS [Barkod No],
                        t1.Ürün_Adi AS [Ürün Adı],
                        FORMAT(t1.ToplamTutar, 'Standard') AS Tutarı,
                        'Müşteri İadesi - ' & t2.OdemeSekli AS [Gelir Gider Sebebi],
                        'Gelir' AS Türü
                    FROM
                        MusteriIade AS t1
                    INNER JOIN
                        Tahsilat AS t2 ON t1.GsmTelefon = t2.GsmTelefon
                    WHERE t1.GsmTelefon = @GsmTelefon;
                ";
                    }
                    else
                    {
                        // Müşteri seçili değilse, tüm verileri (tekrar etmeden) listele.
                        // UrunSatis tablosundan MusteriSatis'te olan kayıtları çıkar.
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
                    WHERE NOT EXISTS (
                        SELECT 1 FROM MusteriSatis
                        WHERE MusteriSatis.Barkod_No = UrunSatis.Barkod_No AND MusteriSatis.Tarih = UrunSatis.Tarih AND MusteriSatis.Saat = UrunSatis.Saat
                    )
                    UNION ALL
                    SELECT
                        FORMAT(Tarih, 'dd.MM.yyyy') & ' ' & FORMAT(Saat, 'hh:nn:ss') AS [Tarih/Saat],
                        Barkod_No AS [Barkod No],
                        Urun_Adi AS [Ürün Adı],
                        FORMAT(ToplamTutar, 'Standard') AS Tutarı,
                        'Müşteri Satışı - ' & SatisTuru AS [Gelir Gider Sebebi],
                        'Gelir' AS Türü
                    FROM
                        MusteriSatis
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
                        FORMAT(t1.Tarih, 'dd.MM.yyyy') & ' ' & FORMAT(t1.Saat, 'hh:nn:ss') AS [Tarih/Saat],
                        t1.Barkod_No AS [Barkod No],
                        t1.Ürün_Adi AS [Ürün Adı],
                        FORMAT(t1.ToplamTutar, 'Standard') AS Tutarı,
                        'Müşteri İadesi - ' & t2.OdemeSekli AS [Gelir Gider Sebebi],
                        'Gelir' AS Türü
                    FROM
                        MusteriIade AS t1
                    INNER JOIN
                        Tahsilat AS t2 ON t1.GsmTelefon = t2.GsmTelefon
                    UNION ALL
                    SELECT
                        FORMAT(t1.Tarih, 'dd.MM.yyyy') & ' ' & FORMAT(t1.Saat, 'hh:nn:ss') AS [Tarih/Saat],
                        t1.Barkod_No AS [Barkod No],
                        t1.Ürün_Adi AS [Ürün Adı],
                        FORMAT(t1.ToplamTutar, 'Standard') AS Tutarı,
                        'Toptancı İadesi - ' & t2.OdemeSekli AS [Gelir Gider Sebebi],
                        'Gider' AS Türü
                    FROM
                        UrunIade AS t1
                    INNER JOIN
                        BorcOdeme AS t2 ON t1.GsmTelefon = t2.GsmTelefon;
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
        private void button2_Click(object sender, EventArgs e)
        {
            // DataGridView'da veri olup olmadığını kontrol et
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Aktarılacak veri bulunmamaktadır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Kullanıcıya dosyayı kaydetme iletişim kutusu göster
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Dosyası|*.xlsx";
                sfd.Title = "Gelir Gider Raporu Kaydet";
                sfd.FileName = "GelirGiderRaporu_";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                // DataGridView verilerini DataTable'a dönüştür
                DataTable dt = new DataTable();
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    dt.Columns.Add(column.HeaderText);
                }
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    dt.Rows.Add(row.Cells.Cast<DataGridViewCell>().Select(c => c.Value).ToArray());
                }

                // ClosedXML ile Excel dosyasını oluştur ve kaydet
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Gelir Gider Raporu");
                    worksheet.Cell(1, 1).InsertTable(dt);

                    // Tabloyu daha şık hale getirmek için ayarlar yap
                    var table = worksheet.Table(0);
                    table.Theme = XLTableTheme.TableStyleMedium9;
                    worksheet.Columns().AdjustToContents();

                    workbook.SaveAs(sfd.FileName);
                }

                MessageBox.Show("Veriler başarıyla Excel'e aktarıldı.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri Excel'e aktarılırken bir hata oluştu: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    t1.Aciklama AS [Gelir Gider Sebebi],
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
                    t1.OdemeSekli AS [Gelir Gider Sebebi],
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
                    t1.Aciklama AS [Gelir Gider Sebebi],
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
                    t1.OdemeSekli AS [Gelir Gider Sebebi],
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
        private void button4_Click(object sender, EventArgs e)
        {
            // DataGridView'da veri olup olmadığını kontrol et
            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Aktarılacak veri bulunmamaktadır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Kullanıcıya dosyayı kaydetme iletişim kutusu göster
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Dosyası|*.xlsx";
                sfd.Title = "Borç-Alacak Raporu Kaydet";
                sfd.FileName = "GelirGiderRaporu1_";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                // DataGridView verilerini DataTable'a dönüştür
                DataTable dt = new DataTable();
                foreach (DataGridViewColumn column in dataGridView2.Columns)
                {
                    dt.Columns.Add(column.HeaderText);
                }
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    // Hücre değerlerini kontrol ederek aktarma
                    dt.Rows.Add(row.Cells.Cast<DataGridViewCell>().Select(c => c.Value).ToArray());
                }

                // ClosedXML ile Excel dosyasını oluştur ve kaydet
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Borç Alacak Raporu");
                    worksheet.Cell(1, 1).InsertTable(dt);

                    // Tabloyu daha şık hale getirmek için ayarlar yap
                    var table = worksheet.Table(0);
                    table.Theme = XLTableTheme.TableStyleMedium9;
                    worksheet.Columns().AdjustToContents();

                    workbook.SaveAs(sfd.FileName);
                }

                MessageBox.Show("Veriler başarıyla Excel'e aktarıldı.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri Excel'e aktarılırken bir hata oluştu: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      
    }
}
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.collection;
using iTextSharp.text.pdf.draw;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static ÜrünYönetimSistemi.UrunDetayı;
using Font = System.Drawing.Font; // iTextSharp ile çakışmayı önler

namespace ÜrünYönetimSistemi
{
    public partial class UrunDetayı : Form
    {
        private string selectedBarkodNo = null;
        private DateTime eskiBaslangicTarihi;
        private DateTime eskiBitisTarihi;
        private int currentPrintRow = 0;
        private bool isPrinting = false;
        public UrunDetayı()
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

            eskiBaslangicTarihi = dateTimePicker1.Value;
            eskiBitisTarihi = dateTimePicker2.Value;

            // Checkbox olaylarını burada tanımlıyoruz
            checkBox1.Click += CheckBox_Click;
            checkBox2.Click += CheckBox_Click;
            checkBox3.Click += CheckBox_Click;
            checkBox4.Click += CheckBox_Click;
            checkBox5.Click += CheckBox_Click;
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            textBox5.ReadOnly = true;

            textBox6.ReadOnly = true;
            textBox7.ReadOnly = true;
            textBox8.ReadOnly = true;

            // Uygulama ilk açıldığında "Tümü" seçili olsun
            checkBox1.Checked = true;

            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            // VerileriDatagrideYukle(); // Bu satırı kaldırdık, form açılışında DataGridView2 boş kalsın
        }
        private void TariheGoreFiltrele()
        {
            DateTime baslangicTarihi = dateTimePicker1.Value.Date;
            DateTime bitisTarihi = dateTimePicker2.Value.Date.Date.AddDays(1).AddSeconds(-1);

            // Tarih filtresini oluşturuyoruz.
            string tarihFiltresi = string.Format(
                "[Tarih] >= #{0}# AND [Tarih] <= #{1}#",
                baslangicTarihi.ToString("MM/dd/yyyy HH:mm:ss"),
                bitisTarihi.ToString("MM/dd/yyyy HH:mm:ss"));

            string gelirGiderFiltresi = "";

            if (checkBox2.Checked) // Sadece Satışlar
                gelirGiderFiltresi = " AND ([Gelir Gider Sebebi] LIKE 'Ürün Satışı%' OR [Gelir Gider Sebebi] LIKE 'Müşteri Satışı%')";
            else if (checkBox3.Checked) // Sadece Alışlar
                gelirGiderFiltresi = " AND [Gelir Gider Sebebi] LIKE 'Ürün Alışı%'";
            else if (checkBox4.Checked) // Sadece İade Alınanlar
                gelirGiderFiltresi = " AND [Gelir Gider Sebebi] = 'Müşteri İadesi'";
            else if (checkBox5.Checked) // Sadece İade Edilenler
                gelirGiderFiltresi = " AND [Gelir Gider Sebebi] = 'Toptancı İadesi'";

            string sonFiltreMetni = tarihFiltresi + gelirGiderFiltresi;

            DataTable borcAlacakTablosu = dataGridView2.DataSource as DataTable;
            if (borcAlacakTablosu != null && borcAlacakTablosu.Columns.Contains("Tarih"))
            {
                borcAlacakTablosu.DefaultView.RowFilter = sonFiltreMetni;
            }

            // Filtre sonrası toplamları güncelle
            VerileriHesapla();
            VerileriHesaplaSatis();
            VerileriHesaplaIade();
            VerileriHesaplaIadeAlinan();
        }

        private void CheckBox_Click(object sender, EventArgs e)
        {
            CheckBox clickedBox = sender as CheckBox;

            if (clickedBox != null)
            {
                // Tıklanan checkbox'ın işaretli olup olmadığını kontrol et
                if (clickedBox.Checked)
                {
                    // Eğer işaretliyse, diğer tüm checkbox'ların işaretini kaldır
                    if (checkBox1.Checked)
                    {
                        if (clickedBox != checkBox1)
                            checkBox1.Checked = false;
                    }
                    if (checkBox2.Checked)
                    {
                        if (clickedBox != checkBox2)
                            checkBox2.Checked = false;
                    }
                    if (checkBox3.Checked)
                    {
                        if (clickedBox != checkBox3)
                            checkBox3.Checked = false;
                    }
                    if (checkBox4.Checked)
                    {
                        if (clickedBox != checkBox4)
                            checkBox4.Checked = false;
                    }
                    if (checkBox5.Checked)
                    {
                        if (clickedBox != checkBox5)
                            checkBox5.Checked = false;
                    }
                }
            }

            // Hiçbir checkbox seçili değilse, "Tümü" (checkBox1) seçili olsun.
            // Bu, kullanıcının bir checkbox'ı seçip sonra tekrar tıklayarak işaretini kaldırması durumunda,
            // bir seçeneğin her zaman aktif olmasını sağlar.
            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked && !checkBox5.Checked)
            {
                checkBox1.Checked = true;
            }

            // Checkbox durumu değiştiğinde filtrelemeyi tekrar çalıştır
            TariheGoreFiltrele();
        }
        private void UrunDetayı_Load(object sender, EventArgs e)
        {
            Listele();
            // Form açılışında DataGridView1'in otomatik seçimini temizle, böylece DataGridView2 boş kalsın
            dataGridView1.ClearSelection();
            // DataGridView2'yi başlangıçta boş bir tablo ile başlat (gerekirse, ama şu anki sorgu null filtrede tüm veriyi getiriyor)
            // Eğer tamamen boş istiyorsan, VerileriDatagrideYukle() çağrısını SelectionChanged'e bırak
        }

        private void VerileriHesapla()
        {
            decimal toplamTutar = 0;
            int toplamAdet = 0;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells["Gelir Gider Sebebi"].Value != null && row.Cells["Gelir Gider Sebebi"].Value.ToString().Contains("Ürün Alışı"))
                {
                    if (row.Cells["Tutarı"].Value != null && row.Cells["Miktar"].Value != null)
                    {
                        string tutarStr = row.Cells["Tutarı"].Value.ToString();
                        string miktarStr = row.Cells["Miktar"].Value.ToString();
                        tutarStr = tutarStr.Replace(".", "").Replace(",", ".");
                        decimal tutar;
                        int miktar;
                        if (decimal.TryParse(tutarStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tutar) &&
                            int.TryParse(miktarStr, out miktar))
                        {
                            toplamTutar += tutar;
                            toplamAdet += miktar;
                        }
                    }
                }
            }
            textBox1.Text = toplamTutar.ToString("N2");
            textBox2.Text = toplamAdet.ToString();
        }

        private void VerileriHesaplaSatis()
        {
            decimal toplamSatisTutar = 0;
            int toplamSatisAdet = 0;
            var trCulture = new System.Globalization.CultureInfo("tr-TR");
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells["Gelir Gider Sebebi"].Value != null &&
                   (row.Cells["Gelir Gider Sebebi"].Value.ToString().Contains("Ürün Satışı") ||
                    row.Cells["Gelir Gider Sebebi"].Value.ToString().Contains("Müşteri Satışı")))
                {
                    string tutarStr = (row.Cells["Tutarı"].Value ?? "0").ToString().Trim();
                    string miktarStr = (row.Cells["Miktar"].Value ?? "0").ToString().Trim();
                    if (string.IsNullOrWhiteSpace(tutarStr) || tutarStr == "---")
                        tutarStr = "0";
                    if (string.IsNullOrWhiteSpace(miktarStr) || miktarStr == "---")
                        miktarStr = "0";
                    decimal tutar = 0;
                    int miktar = 0;
                    if (!decimal.TryParse(tutarStr, System.Globalization.NumberStyles.Any, trCulture, out tutar))
                    {
                        tutarStr = tutarStr.Replace(".", "").Replace(",", ".");
                        decimal.TryParse(tutarStr, System.Globalization.NumberStyles.Any,
                                         System.Globalization.CultureInfo.InvariantCulture, out tutar);
                    }
                    int.TryParse(miktarStr, out miktar);
                    toplamSatisTutar += tutar;
                    toplamSatisAdet += miktar;
                }
            }
            textBox3.Text = toplamSatisTutar.ToString("N2", trCulture);
            textBox4.Text = toplamSatisAdet.ToString();
        }

        private void VerileriHesaplaIadeAlinan()
        {
            decimal toplamIadeAlinanTutar = 0;
            int toplamIadeAlinanAdet = 0;
            var trCulture = new System.Globalization.CultureInfo("tr-TR");
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells["Gelir Gider Sebebi"].Value != null &&
                    row.Cells["Gelir Gider Sebebi"].Value.ToString().Contains("Müşteri İadesi"))
                {
                    string tutarStr = (row.Cells["Tutarı"].Value ?? "0").ToString().Trim();
                    string miktarStr = (row.Cells["Miktar"].Value ?? "0").ToString().Trim();
                    if (string.IsNullOrWhiteSpace(tutarStr) || tutarStr == "---") tutarStr = "0";
                    if (string.IsNullOrWhiteSpace(miktarStr) || miktarStr == "---") miktarStr = "0";
                    decimal tutar = 0;
                    int miktar = 0;
                    if (!decimal.TryParse(tutarStr, System.Globalization.NumberStyles.Any, trCulture, out tutar))
                    {
                        tutarStr = tutarStr.Replace(".", "").Replace(",", ".");
                        decimal.TryParse(tutarStr, System.Globalization.NumberStyles.Any,
                                         System.Globalization.CultureInfo.InvariantCulture, out tutar);
                    }
                    int.TryParse(miktarStr, out miktar);
                    toplamIadeAlinanTutar += tutar;
                    toplamIadeAlinanAdet += miktar;
                }
            }
            textBox7.Text = toplamIadeAlinanTutar.ToString("N2", trCulture);
            textBox8.Text = toplamIadeAlinanAdet.ToString();
        }

        private void VerileriHesaplaIade()
        {
            decimal toplamIadeTutar = 0;
            int toplamIadeAdet = 0;
            var trCulture = new System.Globalization.CultureInfo("tr-TR");
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells["Gelir Gider Sebebi"].Value != null &&
                    row.Cells["Gelir Gider Sebebi"].Value.ToString().Contains("Toptancı İadesi"))
                {
                    string tutarStr = (row.Cells["Tutarı"].Value ?? "0").ToString().Trim();
                    string miktarStr = (row.Cells["Miktar"].Value ?? "0").ToString().Trim();
                    if (string.IsNullOrWhiteSpace(tutarStr) || tutarStr == "---") tutarStr = "0";
                    if (string.IsNullOrWhiteSpace(miktarStr) || miktarStr == "---") miktarStr = "0";
                    decimal tutar = 0;
                    int miktar = 0;
                    if (!decimal.TryParse(tutarStr, System.Globalization.NumberStyles.Any, trCulture, out tutar))
                    {
                        tutarStr = tutarStr.Replace(".", "").Replace(",", ".");
                        decimal.TryParse(tutarStr, System.Globalization.NumberStyles.Any,
                                         System.Globalization.CultureInfo.InvariantCulture, out tutar);
                    }
                    int.TryParse(miktarStr, out miktar);
                    toplamIadeTutar += tutar;
                    toplamIadeAdet += miktar;
                }
            }
            textBox5.Text = toplamIadeTutar.ToString("N2", trCulture);
            textBox6.Text = toplamIadeAdet.ToString();
        }

        private void Listele()
        {
            string baglantiStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                 Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
            using (OleDbConnection baglan = new OleDbConnection(baglantiStr))
            {
                try
                {
                    DataTable dt = new DataTable();
                    OleDbDataAdapter da = new OleDbDataAdapter(
                        "SELECT Barkod_No, Ürün_Adi, OlcuBirimi, Satis_Fiyati, Alis_Fiyati, Stok_Miktari FROM [ÜrünGirişi]",
                        baglan);
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns["Barkod_No"].HeaderText = "Barkod No";
                    dataGridView1.Columns["Ürün_Adi"].HeaderText = "Ürün Adı";
                    dataGridView1.Columns["OlcuBirimi"].HeaderText = "Ölçü Birimi";
                    dataGridView1.Columns["Satis_Fiyati"].HeaderText = "Satış Fiyatı";
                    dataGridView1.Columns["Alis_Fiyati"].HeaderText = "Alış Fiyatı";
                    dataGridView1.Columns["Stok_Miktari"].HeaderText = "Stok Miktarı";
                    dataGridView1.Columns["Satis_Fiyati"].DefaultCellStyle.Format = "N2";
                    dataGridView1.Columns["Alis_Fiyati"].DefaultCellStyle.Format = "N2";
                    dataGridView1.Columns["Stok_Miktari"].DefaultCellStyle.Format = "N2";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veriler listelenirken hata oluştu:\n" + ex.Message);
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)

        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                selectedBarkodNo = dataGridView1.SelectedRows[0].Cells["Barkod_No"].Value.ToString();
                VerileriDatagrideYukle(selectedBarkodNo);
                TariheGoreFiltrele();
                VerileriHesapla();
                VerileriHesaplaSatis();
                VerileriHesaplaIade();
                VerileriHesaplaIadeAlinan();
            }
            else
            {
                // Eğer hiçbir satır seçili değilse, DataGridView2'yi temizle (opsiyonel, boş tablo yükle)
                DataTable emptyDt = new DataTable();
                // Boş tabloya sütunlar ekle (mevcut sütun isimlerini kopyala, ama basitçe bırak)
                dataGridView2.DataSource = emptyDt;
                // Toplamları sıfırla
                textBox1.Text = "0,00";
                textBox2.Text = "0";
                textBox3.Text = "0,00";
                textBox4.Text = "0";
                textBox5.Text = "0,00";
                textBox6.Text = "0";
                textBox7.Text = "0,00";
                textBox8.Text = "0";
            }
        }

        private void VerileriDatagrideYukle(string barkodNo = null)
        {
            try
            {
                using (OleDbConnection baglanti = new OleDbConnection(
                    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    string filtre = string.IsNullOrWhiteSpace(barkodNo) ? "True" : "T1.[Barkod_No] = '" + barkodNo + "'";
                    string sorgu = $@"
SELECT
    T1.[Tarih] AS [Tarih],
    T1.[Barkod_No] AS [Barkod No],
    T1.[Urun_Adi] AS [Ürün Adı],
    FORMAT(T1.[ToplamTutar],'Standard') AS [Tutarı],
    'Ürün Satışı - ' & T1.[SatisTuru] AS [Gelir Gider Sebebi],
    'Gelir' AS [Türü],
    T1.[Satis_Fiyati] AS [Satış Fiyatı],
    G.[Alis_Fiyati] AS [Alış Fiyatı],
    T1.[SatilanMiktar] AS [Miktar],
    '---' AS [Cari Hesap Adı]
FROM [UrunSatis] AS T1
LEFT JOIN [ÜrünGirişi] AS G ON T1.[Barkod_No] = G.[Barkod_No]
WHERE ({filtre})
UNION ALL
SELECT
    T1.[Tarih] AS [Tarih],
    T1.[Barkod_No],
    T1.[Urun_Adi],
    FORMAT(T1.[ToplamTutar],'Standard'),
    'Müşteri Satışı - ' & T1.[SatisTuru],
    'Gelir',
    T3.[Satis_Fiyati],
    T4.[Alis_Fiyati],
    T3.[SatilanMiktar],
    T2.[MusteriAdi]
FROM (([MusteriSatis] AS T1
LEFT JOIN [Musteriler] AS T2 ON T1.[GsmTelefon]=T2.[GsmTelefon])
LEFT JOIN [UrunSatis] AS T3 ON T1.[Barkod_No] = T3.[Barkod_No] AND T1.[Tarih] = T3.[Tarih])
LEFT JOIN [ÜrünGirişi] AS T4 ON T1.[Barkod_No] = T4.[Barkod_No]
WHERE ({filtre})
UNION ALL
SELECT
    T1.[Tarih] AS [Tarih],
    T1.[Barkod_No],
    T1.[Ürün_Adi],
    FORMAT(CCur(Replace(T1.[Alis_Fiyati],',','.')) * CCur(Replace(T1.[Miktar],',','.')),'Standard'),
    'Ürün Alışı - ' & T1.[IslemTuru],
    'Gider',
    T1.[Satis_Fiyati],
    T1.[Alis_Fiyati],
    T1.[Miktar],
    IIF(T2.[ToptanciAdi] IS NULL OR T2.[ToptanciAdi] = '', '---', T2.[ToptanciAdi]) AS [Cari Hesap Adı]
FROM [ÜrünGirişi] AS T1
LEFT JOIN [Toptancilar] AS T2 ON T1.[GsmTelefon]=T2.[GsmTelefon]
WHERE ({filtre})
UNION ALL
SELECT
    T1.[Tarih] AS [Tarih],
    T1.[Barkod_No],
    T1.[Ürün_Adi],
    FORMAT(T1.[ToplamTutar],'Standard'),
    'Müşteri İadesi',
    'Gelir',
    T2.[Satis_Fiyati],
    '---' AS [Alış Fiyatı],
    T1.[IadeAlinanMiktar],
    T3.[MusteriAdi]
FROM ([MusteriIade] AS T1
LEFT JOIN [MusteriSatis] AS T2 ON T1.[Barkod_No] = T2.[Barkod_No])
LEFT JOIN [Musteriler] AS T3 ON T1.[GsmTelefon] = T3.[GsmTelefon]
WHERE ({filtre})
UNION ALL
SELECT
    T1.[Tarih] AS [Tarih],
    T1.[Barkod_No],
    T1.[Ürün_Adi],
    FORMAT(T1.[ToplamTutar],'Standard'),
    'Toptancı İadesi',
    'Gider',
    '---' AS [Satış Fiyatı],
    T2.[Alis_Fiyati],
    T1.[IadeEdilenMiktar],
    T4.[ToptanciAdi]
FROM ((([UrunIade] AS T1
LEFT JOIN [ÜrünGirişi] AS T2 ON T1.[Barkod_No] = T2.[Barkod_No])
LEFT JOIN [UrunSatis] AS T3 ON T1.[Barkod_No] = T3.[Barkod_No])
LEFT JOIN [Toptancilar] AS T4 ON T1.[GsmTelefon] = T4.[GsmTelefon])
WHERE ({filtre})
";
                    OleDbDataAdapter da = new OleDbDataAdapter(sorgu, baglanti);
                    DataTable dt = new DataTable();
                    baglanti.Open();
                    da.Fill(dt);
                    if (dt.Columns.Contains("Tarih"))
                    {
                        dt.Columns["Tarih"].DataType = typeof(DateTime);
                    }
                    dataGridView2.DataSource = dt;
                    TariheGoreFiltrele();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri çekme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = eskiBaslangicTarihi;
            dateTimePicker2.Value = eskiBitisTarihi;
            TariheGoreFiltrele();
        }



        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            string aramaMetni = textBox9.Text.Trim();

            // DataGridView'in veri kaynağı olan DataTable'ı al
            DataTable dt = dataGridView1.DataSource as DataTable;

            if (dt == null) return;

            DataView dv = dt.DefaultView;

            // Tek tırnak karakterlerini kaçır
            string filtreMetni = aramaMetni.Replace("'", "''");

            // Arama metni boşsa filtreyi temizle, doluysa filtrele
            if (string.IsNullOrEmpty(aramaMetni))
            {
                dv.RowFilter = string.Empty;
            }
            else
            {
                dv.RowFilter =
                    $"CONVERT([Barkod_No], System.String) LIKE '%{filtreMetni}%' " +
                    $"OR [Ürün_Adi] LIKE '%{filtreMetni}%'";
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Lütfen bu formda hangi DataGridView'in kullanılacağını kontrol edin. 
            // Önceki talebinize göre bu metotta 'dataGridView2' kullanılmıştır.
            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Tabloda aktarılacak veri yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Dosyası|*.xlsx";
                // Dosya adı, tablonun içeriğine uygun olarak güncellendi.
                sfd.FileName = "UrunDetayi_" + ".xlsx";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Genel İşlem Dökümü");
                    int currentRow = 1;

                    // Not: İşletme adı çekme kısmı bu tabloda gerekli değilse atlanabilir. 
                    // Ancak verdiğiniz örnekteki yapıyı korumak için buraya bir başlık ekliyorum.
                    worksheet.Range(currentRow, 1, currentRow, dataGridView2.Columns.Count).Merge();

                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 16;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    currentRow += 2; // Başlık sonrası 2 satır boşluk

                    int headerRow = currentRow;

                    // Kolon Başlıkları (Headers)
                    for (int i = 0; i < dataGridView2.Columns.Count; i++)
                    {
                        worksheet.Cell(currentRow, i + 1).Value = dataGridView2.Columns[i].HeaderText;
                        worksheet.Cell(currentRow, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9"); // Açık Gri tonu
                    }
                    currentRow++;

                    // Satır Verileri (Data Rows)
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        // Yeni boş satırı atla (eğer AllowUserToAddRows aktif ise)
                        if (dataGridView2.Rows[i].IsNewRow) continue;

                        for (int j = 0; j < dataGridView2.Columns.Count; j++)
                        {
                            // Hücre değeri boşsa null yerine boş string kullan (Excel'e daha temiz yazar)
                            worksheet.Cell(currentRow + i, j + 1).Value =
                                dataGridView2.Rows[i].Cells[j].Value?.ToString() ?? "";
                        }
                    }

                    // --- Biçimlendirme Uygulama ---

                    // Kenarlıklar
                    var tableRange = worksheet.Range(headerRow, 1, currentRow + dataGridView2.Rows.Count - 1, dataGridView2.Columns.Count);
                    tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // Sütun genişlikleri (Otomatik genişlik + manuel düzeltme)
                    worksheet.Columns().AdjustToContents();
                    for (int i = 1; i <= dataGridView2.Columns.Count; i++)
                    {
                        // Sütunların en az 15 birim olmasını sağla
                        if (worksheet.Column(i).Width < 15)
                            worksheet.Column(i).Width = 15;
                    }

                    // Satır yüksekliği
                    worksheet.Rows().Height = 22.22;
                    worksheet.Rows(headerRow, headerRow).Height = 25; // Başlık satırını biraz daha yüksek yap

                    // Tarih ve Para Birimi Format/Hizalama
                    // İlk sütun (Tarih) ve Dördüncü sütun (Tutarı) için hizalama ve format.
                    // Önceki sorgunuzdaki 'FORMAT(T1.[ToplamTutar],'Standard')' ifadesi para birimi olduğunu gösteriyor.

                    // Sütun 1: Tarih - Sola Hizala
                    worksheet.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    // Sütun 4: Tutarı - Sağa Hizala
                    worksheet.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    // Opsiyonel: Grid çizgilerini gizle
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Yazdırılacak veri bulunmamaktadır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // .NET Core/.NET 5+ için encoding sağlayıcısını kaydet (gerekirse, ama printing için değil)
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                // PrintDocument oluştur
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += PrintPageHandler;
                currentPrintRow = 0;
                isPrinting = true;

                // Otomatik yazdırma (diyalog olmadan varsayılan yazıcıya)
                pd.Print();

                isPrinting = false;
                MessageBox.Show("Veriler başarıyla yazdırıldı.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                isPrinting = false;
                MessageBox.Show("Yazdırma hatası oluştu: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintPageHandler(object sender, PrintPageEventArgs e)
        {
            if (!isPrinting) return;

            Graphics g = e.Graphics;
            g.PageUnit = GraphicsUnit.Pixel; // Pixel birimi kullan

            // Sayfa kenar boşlukları (mm cinsinden yaklaşık pixel)
            float margin = 25 * 3.78f; // 25mm ≈ 94.5 pixel (96 dpi varsayımı)
            float pageWidth = e.PageBounds.Width - 2 * margin;
            float pageHeight = e.PageBounds.Height - 2 * margin;
            float yPosition = margin + 50; // Başlık sonrası başlangıç Y

            Font fontBaslik = new Font("Arial", 16, FontStyle.Bold);
            Font fontSutunBaslik = new Font("Arial", 10, FontStyle.Bold);
            Font fontVeri = new Font("Arial", 9);

            Brush brushBlack = Brushes.Black;
            Brush brushWhite = Brushes.White;
            Brush brushHeaderBg = new SolidBrush(Color.FromArgb(64, 64, 64)); // Koyu gri

            StringFormat leftAlign = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
            StringFormat centerAlign = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
            StringFormat rightAlign = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near };

            float xPosition = margin;
            int sutunSayisi = dataGridView2.Columns.Count;

            // Sütun genişlikleri (relative)
            float[] relativeWidths = new float[sutunSayisi];
            float totalRelative = 0;
            for (int i = 0; i < sutunSayisi; i++)
            {
                if (i == 0 || i == 4 || i == 9) // Tarih, Gelir Gider Sebebi, Cari Hesap
                    relativeWidths[i] = 1.5f;
                else
                    relativeWidths[i] = 1f;
                totalRelative += relativeWidths[i];
            }

            float[] columnWidths = new float[sutunSayisi];
            for (int i = 0; i < sutunSayisi; i++)
            {
                columnWidths[i] = (pageWidth * relativeWidths[i]) / totalRelative;
            }

            // 1. Ana Başlık (her sayfada)
            string baslikText = "ÜRÜN DETAYI RAPORU";
            SizeF baslikSize = g.MeasureString(baslikText, fontBaslik);
            float baslikX = margin + (pageWidth - baslikSize.Width) / 2;
            g.DrawString(baslikText, fontBaslik, brushBlack, baslikX, margin, centerAlign);
            yPosition += 40; // Başlık sonrası boşluk

            // Sütun başlıkları (sadece ilk sayfada)
            if (currentPrintRow == 0)
            {
                for (int col = 0; col < sutunSayisi; col++)
                {
                    // Arka plan dikdörtgen
                    g.FillRectangle(brushHeaderBg, xPosition, yPosition, columnWidths[col], 25);
                    // Text
                    string headerText = dataGridView2.Columns[col].HeaderText;
                    g.DrawString(headerText, fontSutunBaslik, brushWhite, xPosition, yPosition, centerAlign);
                    xPosition += columnWidths[col];
                }
                yPosition += 30; // Header sonrası
                xPosition = margin;
            }

            // Veri satırları
            int startRow = currentPrintRow;
            bool morePages = false;
            int nextRow = dataGridView2.Rows.Count; // Varsayılan olarak son

            for (int row = startRow; row < dataGridView2.Rows.Count; row++)
            {
                if (dataGridView2.Rows[row].IsNewRow) continue;

                // Satır yüksekliği hesapla (yaklaşık 20 pixel)
                float rowHeight = 20;
                if (yPosition + rowHeight > pageHeight - margin)
                {
                    morePages = true;
                    nextRow = row; // Break anındaki row'u kaydet (sonraki sayfa için)
                    break;
                }

                xPosition = margin;
                for (int col = 0; col < sutunSayisi; col++)
                {
                    string cellValue = dataGridView2.Rows[row].Cells[col].Value?.ToString() ?? "";
                    StringFormat sf = leftAlign;
                    if (col == 3) // Tutarı sütunu
                        sf = rightAlign;

                    g.DrawString(cellValue, fontVeri, brushBlack, xPosition, yPosition, sf);
                    xPosition += columnWidths[col];
                }
                yPosition += rowHeight;
            }

            currentPrintRow = nextRow;

            e.HasMorePages = morePages;

            // Temizlik
            brushHeaderBg.Dispose();
            fontBaslik.Dispose();
            fontSutunBaslik.Dispose();
            fontVeri.Dispose();
            leftAlign.Dispose();
            centerAlign.Dispose();
            rightAlign.Dispose();
        }
    }
}
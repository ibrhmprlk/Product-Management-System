using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static ÜrünYönetimSistemi.Ürün_Girişi;
using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;

namespace ÜrünYönetimSistemi
{
    public partial class MusteriBorcDetayi : Form
    {
        private DateTime eskiBaslangicTarihi;

        private DateTime eskiBitisTarihi;
        public string MusteriAdi { get; set; }
        public string GsmTelefon { get; set; }



        private string secilenGsmTelefon;
        public MusteriBorcDetayi()
        {
            InitializeComponent();


            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox6.ReadOnly = true;

            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            dataGridView1.ReadOnly = true; // Bu satırı ekleyin
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.ReadOnly = true; // Bu satırı ekleyin
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView3.ReadOnly = true; // Bu satırı ekleyin
            dataGridView3.AllowUserToDeleteRows = false;
            dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // DateTimePicker1 için
            this.dateTimePicker1.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker1.CustomFormat = "dd MMMM yyyy dddd"; // Gün, ay adı, yıl ve gün adı

            // DateTimePicker2 için
            this.dateTimePicker2.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker2.CustomFormat = "dd MMMM yyyy dddd"; // Gün, ay adı, yıl ve gün adı

            // İstersen buradan direkt borç hareketlerini listeleyebilirsin
        }
        private void TariheGoreFiltrele()
        {
            DateTime baslangicTarihi = dateTimePicker1.Value.Date;
            DateTime bitisTarihi = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);

            // DataGridView1 (Borç Detayları)
            DataTable borcTablosu = dataGridView1.DataSource as DataTable;
            if (borcTablosu != null && borcTablosu.Columns.Contains("Tarih/Saat"))
            {
                borcTablosu.DefaultView.RowFilter = string.Format(
                    "[Tarih/Saat] >= #{0}# AND [Tarih/Saat] <= #{1}#",
                    baslangicTarihi.ToString("MM/dd/yyyy HH:mm:ss"),
                    bitisTarihi.ToString("MM/dd/yyyy HH:mm:ss"));
            }

            // DataGridView2 (Ürün Satışları)
            DataTable urunTablosu = dataGridView2.DataSource as DataTable;
            if (urunTablosu != null && urunTablosu.Columns.Contains("Tarih"))
            {
                urunTablosu.DefaultView.RowFilter = string.Format(
                    "Tarih >= #{0}# AND Tarih <= #{1}#",
                    baslangicTarihi.ToString("MM/dd/yyyy HH:mm:ss"),
                    bitisTarihi.ToString("MM/dd/yyyy HH:mm:ss"));
            }

            // DataGridView3 (Ürün İadeleri)
            DataTable iadeTablosu = dataGridView3.DataSource as DataTable;
            if (iadeTablosu != null && iadeTablosu.Columns.Contains("Tarih"))
            {
                iadeTablosu.DefaultView.RowFilter = string.Format(
                    "Tarih >= #{0}# AND Tarih <= #{1}#",
                    baslangicTarihi.ToString("MM/dd/yyyy HH:mm:ss"),
                    bitisTarihi.ToString("MM/dd/yyyy HH:mm:ss"));
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            TariheGoreFiltrele();
        }
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            TariheGoreFiltrele();
        }
        private void MusteriBorcDetayi_Load(object sender, EventArgs e)
        {
            eskiBaslangicTarihi = dateTimePicker1.Value;
            eskiBitisTarihi = dateTimePicker2.Value;

            dataGridView1.Width = 740;
            dataGridView1.Height = 262;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            textBox1.Text = MusteriAdi;
            textBox2.Text = GsmTelefon;

            if (string.IsNullOrWhiteSpace(GsmTelefon))
                return;

            string baglantiDizesi = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
            {
                try
                {
                    baglan.Open();

                    using (OleDbCommand cmd = new OleDbCommand("SELECT DevredenBorc FROM Musteriler WHERE GsmTelefon = ?", baglan))
                    {
                        cmd.Parameters.AddWithValue("@GsmTelefon", GsmTelefon);
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            decimal toplamBorc = 0;
                            if (decimal.TryParse(result.ToString().Replace(".", ","), out toplamBorc))
                            {
                                textBox3.Text = toplamBorc.ToString("N2");
                                textBox5.Text = toplamBorc.ToString("N2");
                            }
                            else
                            {
                                textBox3.Text = result.ToString();
                                textBox5.Text = result.ToString();
                                MessageBox.Show("Borç değeri dönüştürülemedi: " + result.ToString(), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            textBox3.Text = "0.00";
                            textBox5.Text = "0.00";
                            MessageBox.Show("Bu GsmTelefon için müşteri kaydı bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    // VeresiyeEkle ve Tahsilat tablolarını birleştirme
                    DataTable tablo = new DataTable();
                    string query = @"
            SELECT [Tarih/Saat] AS [Tarih/Saat], Aciklama AS [İşlem Türü], EklenenTutar AS [İşlem Tutarı], EskiBorc AS [Önceki Bakiye], ToplamBorc AS [Kalan Borç]
            FROM VeresiyeEkle
            WHERE GsmTelefon = @GsmTelefon1
            UNION ALL
            SELECT [Tarih/Saat] AS [Tarih/Saat], Aciklama AS [İşlem Türü], -OdenenTutar AS [İşlem Tutarı], EskiBorc AS [Önceki Bakiye],
                    (EskiBorc - OdenenTutar) AS [Kalan Borç]
            FROM Tahsilat
            WHERE GsmTelefon = @GsmTelefon2
            ORDER BY [Tarih/Saat] ASC";

                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, baglan);
                    adapter.SelectCommand.Parameters.AddWithValue("@GsmTelefon1", GsmTelefon);
                    adapter.SelectCommand.Parameters.AddWithValue("@GsmTelefon2", GsmTelefon);
                    adapter.Fill(tablo);

                    dataGridView1.DataSource = tablo;

                    if (dataGridView1.Columns.Count >= 5)
                    {
                        // Sütun başlıkları ve biçimlendirmeleri
                        dataGridView1.Columns["Tarih/Saat"].HeaderText = "Tarih / Saat";
                        dataGridView1.Columns["İşlem Türü"].HeaderText = "İşlem Türü";
                        dataGridView1.Columns["İşlem Tutarı"].HeaderText = "İşlem Tutarı";
                        dataGridView1.Columns["Önceki Bakiye"].HeaderText = "Önceki Bakiye";
                        dataGridView1.Columns["Kalan Borç"].HeaderText = "Kalan Borç";

                        dataGridView1.Columns["Tarih/Saat"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                        dataGridView1.Columns["İşlem Tutarı"].DefaultCellStyle.Format = "C2";
                        dataGridView1.Columns["Önceki Bakiye"].DefaultCellStyle.Format = "C2";
                        dataGridView1.Columns["Kalan Borç"].DefaultCellStyle.Format = "C2";
                        dataGridView1.Columns["İşlem Tutarı"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dataGridView1.Columns["Önceki Bakiye"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dataGridView1.Columns["Kalan Borç"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Bu kısımlar, form yüklendiğinde diğer tabloları da dolduracaktır.
            UrunSatisDataGridDoldur();
            UrunIadeAlDataGridDoldur();

            // İlk yüklemede filtreyi uygula
            TariheGoreFiltrele();
        }



        private void ToplamUrunFiyatiniHesapla()
        {
            decimal toplamTutar = 0;

            // dataGridView2'deki her satırı döngüye al
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                // Satırın boş olup olmadığını kontrol et
                if (row.IsNewRow) continue;

                // 'Toplam Tutar' hücresindeki değeri al
                if (row.Cells["ToplamTutar"].Value != null)
                {
                    decimal tutar;
                    // Değeri ondalık sayıya dönüştürmeye çalış
                    if (decimal.TryParse(row.Cells["ToplamTutar"].Value.ToString(), out tutar))
                    {
                        toplamTutar += tutar;
                    }
                }
            }

            // Hesaplanan toplamı textBox5'e yaz
            // ToString("N2") ile para birimi formatında (örneğin, 1.234,56) göster
            textBox5.Text = toplamTutar.ToString("N2");
        }
        private void UrunSatisDataGridDoldur()
        {
            // Müşterinin GsmTelefon numarası boşsa işlemi durdur
            if (string.IsNullOrWhiteSpace(GsmTelefon))
            {
                return;
            }

            string baglantiDizesi = "Provider=Microsoft.ACE.OleDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
            {
                try
                {
                    baglan.Open();
                    DataTable satisTablosu = new DataTable();

                    // SQL sorgusu, gerekli sütunları çekiyor.
                    string satisSorgusu = $@"
            SELECT Tarih, Saat, SatisTuru,  Urun_Adi, Satis_Fiyati, SatilanMiktar, ToplamTutar
            FROM MusteriSatis
            WHERE GsmTelefon = @GsmTelefon
            ORDER BY Tarih ASC, Saat ASC";

                    using (OleDbDataAdapter satisAdapter = new OleDbDataAdapter(satisSorgusu, baglan))
                    {
                        satisAdapter.SelectCommand.Parameters.AddWithValue("@GsmTelefon", GsmTelefon);
                        satisAdapter.Fill(satisTablosu);
                    }

                    // Tarih ve Saat'i birleştirip yeni bir sütun oluştur
                    satisTablosu.Columns.Add("TarihSaat", typeof(string));
                    foreach (DataRow row in satisTablosu.Rows)
                    {
                        if (row["Tarih"] != DBNull.Value && row["Saat"] != DBNull.Value)
                        {
                            DateTime tarih = (DateTime)row["Tarih"];
                            DateTime saat = (DateTime)row["Saat"];
                            row["TarihSaat"] = tarih.ToString("dd.MM.yyyy") + " " + saat.ToString("HH:mm:ss");
                        }
                    }

                    // DataTable'ı DataGridView'e bağla
                    dataGridView2.DataSource = satisTablosu;

                    if (dataGridView2.Columns.Count >= 8)
                    {
                        // Sütun başlıklarını ve sıralamalarını ayarla
                        dataGridView2.Columns["TarihSaat"].HeaderText = "Tarih / Saat";
                        dataGridView2.Columns["SatisTuru"].HeaderText = "Ödeme Şekli";

                        dataGridView2.Columns["Urun_Adi"].HeaderText = "Ürün Adı";
                        dataGridView2.Columns["Satis_Fiyati"].HeaderText = "Birim Fiyatı";
                        dataGridView2.Columns["SatilanMiktar"].HeaderText = "Satılan Miktar";
                        dataGridView2.Columns["ToplamTutar"].HeaderText = "Toplam Tutar";

                        // Gereksiz sütunları gizle
                        dataGridView2.Columns["Tarih"].Visible = false;
                        dataGridView2.Columns["Saat"].Visible = false;

                        // Sütunların görünen sırasını ayarla
                        dataGridView2.Columns["TarihSaat"].DisplayIndex = 0;
                        dataGridView2.Columns["SatisTuru"].DisplayIndex = 1;

                        dataGridView2.Columns["Urun_Adi"].DisplayIndex = 3;
                        dataGridView2.Columns["Satis_Fiyati"].DisplayIndex = 4;
                        dataGridView2.Columns["SatilanMiktar"].DisplayIndex = 5;
                        dataGridView2.Columns["ToplamTutar"].DisplayIndex = 6;

                        // Hücre biçimlendirmeleri ve hizalama
                        dataGridView2.Columns["Satis_Fiyati"].DefaultCellStyle.Format = "C2";
                        dataGridView2.Columns["ToplamTutar"].DefaultCellStyle.Format = "C2";
                        dataGridView2.Columns["Satis_Fiyati"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dataGridView2.Columns["ToplamTutar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        // Sütun genişlikleri
                        dataGridView2.Columns["TarihSaat"].Width = 140; // Tarih ve saat için genişletildi
                        dataGridView2.Columns["SatisTuru"].Width = 110;

                        dataGridView2.Columns["Urun_Adi"].Width = 150;
                        dataGridView2.Columns["Satis_Fiyati"].Width = 120;
                        dataGridView2.Columns["SatilanMiktar"].Width = 120;
                        dataGridView2.Columns["ToplamTutar"].Width = 120;

                        // Toplam hesaplamayı çağır
                        ToplamUrunFiyatiniHesapla();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Müşteri satış verileri yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void UrunIadeAlDataGridDoldur()
        {
            // Seçilen GsmTelefon boşsa çık
            if (string.IsNullOrWhiteSpace(this.GsmTelefon))
            {
                return;
            }

            string baglantiDizesi = "Provider=Microsoft.ACE.OleDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
            {
                try
                {
                    baglan.Open();
                    DataTable iadeTablosu = new DataTable();

                    // SQL sorgusu
                    string iadeSorgusu = @"
            SELECT Tarih, Saat, ID, Barkod_No, Ürün_Adi, Aciklama, Stok_Miktari, IadeAlinanMiktar, ToplamTutar
            FROM MusteriIade
            WHERE GsmTelefon = @GsmTelefon
            ORDER BY ID ASC";

                    OleDbDataAdapter iadeAdapter = new OleDbDataAdapter(iadeSorgusu, baglan);
                    iadeAdapter.SelectCommand.Parameters.AddWithValue("@GsmTelefon", this.GsmTelefon);
                    iadeAdapter.Fill(iadeTablosu);

                    // Tarih ve Saat'i birleştirip yeni bir sütun oluştur
                    iadeTablosu.Columns.Add("TarihSaat", typeof(string));
                    foreach (DataRow row in iadeTablosu.Rows)
                    {
                        if (row["Tarih"] != DBNull.Value && row["Saat"] != DBNull.Value)
                        {
                            DateTime tarih = (DateTime)row["Tarih"];
                            DateTime saat = (DateTime)row["Saat"];
                            row["TarihSaat"] = tarih.ToString("dd.MM.yyyy") + " " + saat.ToString("HH:mm:ss");
                        }
                    }

                    // Toplam tutarı doğru şekilde hesapla
                    decimal toplamTutar = 0;
                    foreach (DataRow row in iadeTablosu.Rows)
                    {
                        if (row["ToplamTutar"] != DBNull.Value)
                        {
                            // Virgül veya nokta ayracı sorununu çözmek için CultureInfo ile parse et
                            if (decimal.TryParse(row["ToplamTutar"].ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tutar))
                            {
                                toplamTutar += tutar;
                            }
                        }
                    }

                    // Toplamı textBox6'ya yaz (2 ondalık ve noktalı)
                    textBox6.Text = Math.Round(toplamTutar, 2).ToString("N2");

                    // DataTable'ı dataGridView3'e bağla
                    dataGridView3.DataSource = iadeTablosu;

                    // Sütun başlıkları ve sıralar
                    // Burada çakışma olmaması için isimlendirmeler doğru şekilde yapıldı
                    dataGridView3.Columns["TarihSaat"].HeaderText = "Tarih / Saat";
                    dataGridView3.Columns["TarihSaat"].DisplayIndex = 0;

                    dataGridView3.Columns["Barkod_No"].HeaderText = "Barkod No";
                    dataGridView3.Columns["Barkod_No"].DisplayIndex = 1;

                    dataGridView3.Columns["Ürün_Adi"].HeaderText = "Ürün Adı";
                    dataGridView3.Columns["Ürün_Adi"].DisplayIndex = 2;

                    dataGridView3.Columns["Aciklama"].HeaderText = "İşlem Türü";
                    dataGridView3.Columns["Aciklama"].DisplayIndex = 3;

                    dataGridView3.Columns["Stok_Miktari"].HeaderText = "Mevcut Stok";
                    dataGridView3.Columns["Stok_Miktari"].DisplayIndex = 4;

                    dataGridView3.Columns["IadeAlinanMiktar"].HeaderText = "İade Alınan Miktar";
                    dataGridView3.Columns["IadeAlinanMiktar"].DisplayIndex = 5;

                    dataGridView3.Columns["ToplamTutar"].HeaderText = "Toplam Tutar";
                    dataGridView3.Columns["ToplamTutar"].DisplayIndex = 6;

                    // ID, Tarih, Saat gizle
                    dataGridView3.Columns["ID"].Visible = false;
                    dataGridView3.Columns["Tarih"].Visible = false;
                    dataGridView3.Columns["Saat"].Visible = false;

                    // Hücre biçimlendirmeleri
                    dataGridView3.Columns["ToplamTutar"].DefaultCellStyle.Format = "C2";
                    dataGridView3.Columns["ToplamTutar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    // Sütun genişlikleri (UrunAlis mantığına benzer)
                    dataGridView3.Columns["TarihSaat"].Width = 120;
                    dataGridView3.Columns["Barkod_No"].Width = 115;
                    dataGridView3.Columns["Ürün_Adi"].Width = 155;
                    dataGridView3.Columns["Aciklama"].Width = 190;
                    dataGridView3.Columns["Stok_Miktari"].Width = 110;
                    dataGridView3.Columns["IadeAlinanMiktar"].Width = 130;
                    dataGridView3.Columns["ToplamTutar"].Width = 110;

                    // DataGrid ayarları
                    dataGridView3.ReadOnly = true;
                    dataGridView3.AllowUserToDeleteRows = false;
                    dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ürün iade verileri yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {
            DataTable urunTablosu = dataGridView2.DataSource as DataTable;

            if (urunTablosu != null && urunTablosu.Columns.Contains("Ürün_Adi"))
            {
                // TextBox4 boşsa veya içeriği silinirse tüm ürünleri göster
                if (string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    urunTablosu.DefaultView.RowFilter = "";
                }
                else
                {
                    // Ürün adında büyük/küçük harf duyarsız arama
                    string filter = $"Ürün_Adi LIKE '%{textBox4.Text.Trim()}%'";
                    urunTablosu.DefaultView.RowFilter = filter;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Tabloda veri yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Dosyası|*.xlsx";
                string fileName = "MüşteriBorcDetayi_" + textBox1.Text + ".xlsx";
                sfd.FileName = fileName;

                if (sfd.ShowDialog() != DialogResult.OK) return;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Müşteri Borç Detayı");
                    int currentRow = 1;

                    // Başlık: IsletmeAdi tablosundan çekilecek
                    string isletmeAdi = "";
                    string baglantiDizesi = "Provider=Microsoft.ACE.OleDB.12.0;Data Source=" +
                                            Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

                    using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
                    {
                        baglan.Open();
                        string query = "SELECT TOP 1 IsletmeAdi FROM IsletmeAdi";
                        using (OleDbCommand cmd = new OleDbCommand(query, baglan))
                        {
                            var result = cmd.ExecuteScalar();
                            if (result != null)
                                isletmeAdi = result.ToString();
                        }
                    }

                    worksheet.Range(currentRow, 1, currentRow, dataGridView1.Columns.Count).Merge();
                    worksheet.Cell(currentRow, 1).Value = isletmeAdi; // Başlık
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 16;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    currentRow += 2;

                    // Üst bilgiler
                    worksheet.Cell(currentRow, 1).Value = "Müşteri Adı:";
                    worksheet.Cell(currentRow, 2).Value = textBox1.Text;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "GSM Telefon:";
                    worksheet.Cell(currentRow, 2).Value = textBox2.Text;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "Toplam Borç:";
                    worksheet.Cell(currentRow, 2).Value = textBox3.Text + " TL";
                    currentRow += 2; // boş satır

                    int headerRow = currentRow;

                    // Kolon başlıkları
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        worksheet.Cell(currentRow, i + 1).Value = dataGridView1.Columns[i].HeaderText;
                        worksheet.Cell(currentRow, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                    currentRow++;

                    // Satır verileri
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            worksheet.Cell(currentRow + i, j + 1).Value =
                                dataGridView1.Rows[i].Cells[j].Value?.ToString() ?? "";
                        }
                    }

                    // Kenarlık
                    var tableRange = worksheet.Range(headerRow, 1, headerRow + dataGridView1.Rows.Count, dataGridView1.Columns.Count);
                    tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // Sütun genişlikleri (artırılmış)
                    for (int i = 1; i <= dataGridView1.Columns.Count; i++)
                    {
                        worksheet.Column(i).Width = 25; // manuel genişlik
                    }

                    // Satır yüksekliği
                    worksheet.Rows().Height = 22.22;

                    // Sayısal kolonları sağa hizala (3. ve 4. kolon)
                    worksheet.Column(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    // Grid çizgilerini gizle
                    worksheet.ShowGridLines = false;

                    // Dosyayı kaydet
                    workbook.SaveAs(sfd.FileName);
                }

                MessageBox.Show("Veriler Excel dosyasına aktarıldı.", "Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel aktarılırken hata oluştu: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Tabloda veri yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string baglantiDizesi = "Provider=Microsoft.ACE.OleDB.12.0;Data Source=" +
                                        Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
                string isletmeAdi = "";

                // IsletmeAdi tablosundan işletme adını çek
                using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
                {
                    baglan.Open();
                    using (OleDbCommand cmd = new OleDbCommand("SELECT TOP 1 IsletmeAdi FROM IsletmeAdi", baglan))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            isletmeAdi = result.ToString();
                    }
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Dosyası|*.xlsx";
                string fileName = "UrunSatisDetayi_" + textBox1.Text + ".xlsx";
                sfd.FileName = fileName;

                if (sfd.ShowDialog() != DialogResult.OK) return;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Ürün Satış Detayı");
                    int currentRow = 1;

                    // Başlık: IsletmeAdi tablosundan
                    worksheet.Range(currentRow, 1, currentRow, dataGridView2.Columns.Count).Merge();
                    worksheet.Cell(currentRow, 1).Value = isletmeAdi;
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 16;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    currentRow += 2;

                    // Üst bilgiler
                    worksheet.Cell(currentRow, 1).Value = "Müşteri Adı:";
                    worksheet.Cell(currentRow, 2).Value = textBox1.Text;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "GSM No:";
                    worksheet.Cell(currentRow, 2).Value = textBox2.Text;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = " Satılan Ürünlerin Toplam Fiyatı:";
                    worksheet.Cell(currentRow, 2).Value = textBox5.Text + " TL";
                    currentRow += 2;

                    int headerRow = currentRow;

                    // Kolon başlıkları
                    for (int i = 0; i < dataGridView2.Columns.Count; i++)
                    {
                        worksheet.Cell(currentRow, i + 1).Value = dataGridView2.Columns[i].HeaderText;
                        worksheet.Cell(currentRow, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                    currentRow++;

                    // Satır verileri (Tarih sütunu sadece gün-ay-yıl)
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView2.Columns.Count; j++)
                        {
                            var cellValue = dataGridView2.Rows[i].Cells[j].Value;

                            if (dataGridView2.Columns[j].Name == "Tarih" && cellValue != null)
                            {
                                if (DateTime.TryParse(cellValue.ToString(), out DateTime tarih))
                                {
                                    worksheet.Cell(currentRow + i, j + 1).Value = tarih.ToString("dd.MM.yyyy");
                                }
                                else
                                {
                                    worksheet.Cell(currentRow + i, j + 1).Value = "";
                                }
                            }
                            else
                            {
                                worksheet.Cell(currentRow + i, j + 1).Value = cellValue?.ToString() ?? "";
                            }
                        }
                    }

                    // Kenarlık
                    var tableRange = worksheet.Range(headerRow, 1, headerRow + dataGridView2.Rows.Count, dataGridView2.Columns.Count);
                    tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // Sütun genişlikleri
                    for (int i = 1; i <= dataGridView2.Columns.Count; i++)
                    {
                        worksheet.Column(i).Width = 25;
                    }

                    // Satır yüksekliği
                    worksheet.Rows().Height = 22.22;

                    // Sayısal kolonları sağa hizala
                    for (int i = 0; i < dataGridView2.Columns.Count; i++)
                    {
                        string colName = dataGridView2.Columns[i].HeaderText;
                        if (colName == "Birim Fiyatı" || colName == "Miktar" || colName == "Toplam Tutar")
                        {
                            worksheet.Column(i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            worksheet.Column(i + 1).Style.NumberFormat.Format = "#,##0.00";
                        }
                    }

                    // Grid çizgilerini gizle
                    worksheet.ShowGridLines = false;

                    workbook.SaveAs(sfd.FileName);
                }

                MessageBox.Show("Veriler Excel dosyasına aktarıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel aktarılırken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = eskiBaslangicTarihi;
            dateTimePicker2.Value = eskiBitisTarihi;

            // Filtreyi de tekrar uygula
            TariheGoreFiltrele();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView3.Rows.Count == 0)
            {
                MessageBox.Show("Tabloda veri yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string baglantiDizesi = "Provider=Microsoft.ACE.OleDB.12.0;Data Source=" +
                                        Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
                string isletmeAdi = "";

                // IsletmeAdi tablosundan işletme adını çek
                using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
                {
                    baglan.Open();
                    using (OleDbCommand cmd = new OleDbCommand("SELECT TOP 1 IsletmeAdi FROM IsletmeAdi", baglan))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            isletmeAdi = result.ToString();
                    }
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Dosyası|*.xlsx";
                string fileName = "UrunIadeDetayi_" + textBox1.Text + ".xlsx";
                sfd.FileName = fileName;

                if (sfd.ShowDialog() != DialogResult.OK) return;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Ürün İade Detayı");
                    int currentRow = 1;

                    // Başlık: IsletmeAdi tablosundan (tek satırda ortalanmış büyük başlık)
                    worksheet.Range(currentRow, 1, currentRow, 7).Merge();
                    worksheet.Cell(currentRow, 1).Value = isletmeAdi;
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 16;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    currentRow += 2;

                    worksheet.Range(currentRow, 1, currentRow, 7).Merge();
                    worksheet.Cell(currentRow, 1).Value = $"Müşteri Adı : {textBox1.Text}";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    currentRow++;

                    worksheet.Range(currentRow, 1, currentRow, 7).Merge();
                    worksheet.Cell(currentRow, 1).Value = $"GSM No : {textBox2.Text}";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    currentRow++;

                    // İade Edilen Ürünlerin Toplam Fiyatı: (tek satırda, tüm sütunlara yayıldı, "Fiyatı : 2500 TL" formatı)
                    string toplamFiyatText = string.IsNullOrWhiteSpace(textBox6.Text) ? "0" : textBox6.Text;
                    worksheet.Range(currentRow, 1, currentRow, 7).Merge();
                    worksheet.Cell(currentRow, 1).Value = $"İade Edilen Ürünlerin Toplam Fiyatı : {toplamFiyatText} TL";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    currentRow += 2;

                    // İhraç edilecek sütun sırası ve başlıkları (kesin sıra)
                    var desiredHeaders = new List<string> {
                "Tarih/Saat",
                "Barkod No",
                "Ürün Adı",
                "İşlem Türü",
                "Mevcut Stok",
                "İade Edilen Miktar",
                "Toplam Tutar"
            };

                    // Helper: kolon eşleştirme için tokenler (olası ad varyasyonlarını kontrol eder)
                    Func<string, string> norm = s => (s ?? "").ToLower().Replace(" ", "").Replace("_", "").Replace("-", "");
                    Func<DataGridViewColumn, bool> IsIdColumn = c =>
                    {
                        var n = norm(c.Name);
                        var h = norm(c.HeaderText);
                        return n == "id" || h == "id" || n == "userid" || h == "userid";
                    };

                    int idxTarihSaat = -1, idxTarih = -1, idxSaat = -1;
                    int colCount = dataGridView3.Columns.Count;

                    // Olası eşleşme tokenleri
                    var tokens = new Dictionary<string, string[]>
            {
                { "tarih", new[]{ "tarihsaat","tarihsaat","tarih", "saat", "tarih_saat", "tarih/saat" } },
                { "barkod", new[]{ "barkod", "barcode" } },
                { "urun", new[]{ "urunadi", "urunadı", "ürünadi", "ürünadı", "urun", "ürün", "urunadi" } },
                { "islem", new[]{ "islem", "işlem", "islemturu", "islemtür" } },
                { "mevcutstok", new[]{ "mevcutstok", "mevcut", "stok", "stokmiktar", "stok_miktar" } },
                { "iademiktar", new[]{ "iademiktar", "iade", "iadeedilen", "iade_edilen", "iade_miktar", "iadetutar" } },
                { "toplamtutar", new[]{ "toplamtutar", "toplam", "tutar", "tutarı" } }
            };

                    // Belirli kolonu bulma fonksiyonu
                    Func<string[], int> FindColumnIndex = (keys) =>
                    {
                        for (int i = 0; i < colCount; i++)
                        {
                            var c = dataGridView3.Columns[i];
                            var name = norm(c.Name);
                            var head = norm(c.HeaderText);
                            foreach (var k in keys)
                            {
                                var kk = k.Replace(" ", "").Replace("_", "").Replace("-", "").ToLower();
                                if (name.Contains(kk) || head.Contains(kk))
                                    return i;
                            }
                        }
                        return -1;
                    };

                    // Tarih/Saat için önce birleşik kolonu ararız, yoksa ayrı Tarih ve Saat kolonlarını buluruz
                    idxTarihSaat = FindColumnIndex(tokens["tarih"]);
                    if (idxTarihSaat == -1)
                    {
                        // ayrı ayrı arama: tarih ve saat için ayrı tokenlerle kontrol et
                        idxTarih = -1; idxSaat = -1;
                        for (int i = 0; i < colCount; i++)
                        {
                            var c = dataGridView3.Columns[i];
                            var name = norm(c.Name);
                            var head = norm(c.HeaderText);
                            if ((name.Contains("tarih") || head.Contains("tarih")) && idxTarih == -1) idxTarih = i;
                            if ((name.Contains("saat") || head.Contains("saat")) && idxSaat == -1) idxSaat = i;
                        }
                    }

                    // Diğer sütun indekslerini sırayla al
                    int idxBarkod = FindColumnIndex(tokens["barkod"]);
                    int idxUrun = FindColumnIndex(tokens["urun"]);
                    int idxIslem = FindColumnIndex(tokens["islem"]);
                    int idxMevcut = FindColumnIndex(tokens["mevcutstok"]);
                    int idxIadeMiktar = FindColumnIndex(tokens["iademiktar"]);
                    int idxToplam = FindColumnIndex(tokens["toplamtutar"]);

                    // Oluşan export listesi (sırayla). Eğer bir sütun bulunamazsa boş bırakılır (hata vermeden)
                    var exportIndices = new List<int?>
            {
                // Tarih/Saat handled specially during yazma
                idxTarihSaat != -1 ? (int?)idxTarihSaat : (idxTarih != -1 || idxSaat != -1 ? (int?)-2 : (int?)null),
                idxBarkod >= 0 ? (int?)idxBarkod : null,
                idxUrun >= 0 ? (int?)idxUrun : null,
                idxIslem >= 0 ? (int?)idxIslem : null,
                idxMevcut >= 0 ? (int?)idxMevcut : null,
                idxIadeMiktar >= 0 ? (int?)idxIadeMiktar : null,
                idxToplam >= 0 ? (int?)idxToplam : null
            };

                    // Başlıkları yaz (kesin başlık metinleri istediğin gibi)
                    int headerRow = currentRow;
                    for (int k = 0; k < desiredHeaders.Count; k++)
                    {
                        worksheet.Cell(currentRow, k + 1).Value = desiredHeaders[k];
                        worksheet.Cell(currentRow, k + 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, k + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                    currentRow++;

                    // Satır verileri
                    for (int r = 0; r < dataGridView3.Rows.Count; r++)
                    {
                        // Eğer son boş ekleme satırı varsa atla
                        if (dataGridView3.Rows[r].IsNewRow) continue;

                        for (int c = 0; c < exportIndices.Count; c++)
                        {
                            var map = exportIndices[c];
                            string cellText = "";

                            if (map == null)
                            {
                                cellText = "";
                            }
                            else if (map == -2)
                            {
                                // ayrı Tarih + Saat birleşimi
                                string tval = idxTarih >= 0 ? (dataGridView3.Rows[r].Cells[idxTarih].Value?.ToString() ?? "") : "";
                                string sval = idxSaat >= 0 ? (dataGridView3.Rows[r].Cells[idxSaat].Value?.ToString() ?? "") : "";
                                cellText = (tval + " " + sval).Trim();
                            }
                            else
                            {
                                var actualIndex = map.Value;
                                var v = dataGridView3.Rows[r].Cells[actualIndex].Value;
                                cellText = v?.ToString() ?? "";
                            }

                            // Tarih/Saat sütunu için ekstra biçimlendirme: tarih objesi ise ToString kullan
                            if (c == 0) // Tarih/Saat sütunu
                            {
                                // Eğer hücre DateTime ise standart format ver
                                DateTime dt;
                                if (DateTime.TryParse(cellText, out dt))
                                    worksheet.Cell(currentRow, c + 1).Value = dt.ToString("g"); // kısa tarih+saat
                                else
                                    worksheet.Cell(currentRow, c + 1).Value = cellText;
                            }
                            else
                            {
                                // Normal hücre yazımı
                                worksheet.Cell(currentRow, c + 1).Value = cellText;
                            }
                        }

                        currentRow++;
                    }

                    // Kenarlık (tablo alanı)
                    int dataRowsCount = currentRow - headerRow - 1; // headerRow içerir
                    if (dataRowsCount < 0) dataRowsCount = 0;
                    var tableRange = worksheet.Range(headerRow, 1, headerRow + dataRowsCount, desiredHeaders.Count);
                    tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // Sütun genişlikleri: tasarımı bozmadan makul bir ayar, kullanıcının dediği gibi satır uzunluklarını dokunmayacak şekilde
                    for (int i = 1; i <= desiredHeaders.Count; i++)
                    {
                        worksheet.Column(i).Width = 25;
                    }

                    // Satır yüksekliği (bizim önceki ayar)
                    worksheet.Rows().Height = 22.22;

                    // Toplam Tutar sütununu sağa hizala ve sayı formatı uygula (hangi sütun olduğunu bul)
                    int toplamKolunIndexi = desiredHeaders.FindIndex(h => h == "Toplam Tutar");
                    if (toplamKolunIndexi >= 0)
                    {
                        var col = worksheet.Column(toplamKolunIndexi + 1);
                        col.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        col.Style.NumberFormat.Format = "#,##0.00";
                    }

                    // Grid çizgilerini gizle (önceki tasarımla aynı)
                    worksheet.ShowGridLines = false;

                    workbook.SaveAs(sfd.FileName);
                }

                MessageBox.Show("Veriler Excel dosyasına aktarıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel aktarılırken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            DataTable iadeTablosu = dataGridView3.DataSource as DataTable;

            if (iadeTablosu != null)
            {
                string searchText = textBox7.Text.Trim().Replace("'", "''");

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    // Boşsa tüm satırları göster
                    iadeTablosu.DefaultView.RowFilter = "";
                }
                else
                {
                    // Barkod_No veya Ürün_Adi içinde arama yap
                    iadeTablosu.DefaultView.RowFilter =
                        $"Convert(Barkod_No, 'System.String') LIKE '%{searchText}%' OR [Ürün_Adi] LIKE '%{searchText}%'";
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // TextBox'ların boş olup olmadığını kontrol et
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Lütfen müşteri adı ve telefon bilgilerini girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // textBox1 ve textBox2'den müşteri adını ve telefon numarasını al
            string musteriAdi = textBox1.Text;
            string gsmTelefon = textBox2.Text;

            // Yeni bir TaksitOdeme formu oluştur
            TaksitOdeme taksitForm = new TaksitOdeme();

            // Formdaki ilgili özelliklere verileri aktar
            taksitForm.MusteriAdi = musteriAdi;
            taksitForm.GsmTelefon = gsmTelefon;

            // Yeni formu göster ve mevcut formu gizle
            taksitForm.Show();

        }

        private void button7_Click(object sender, EventArgs e)
        { // 1. Açık Müşteriler formunu bul.
            Müşteriler musteriForm = Application.OpenForms.OfType<Müşteriler>().FirstOrDefault();

            if (musteriForm == null)
            {
                // Form açık değilse, yeni bir tane oluştur ve göster.
                musteriForm = new Müşteriler();
                musteriForm.Show();
            }
            else
            {
                // Form açıksa, ön plana getir. (Bu aynı zamanda gizliyse gösterir).
                musteriForm.Show();      // Form gizliyse görünür hale getirir.
                musteriForm.BringToFront(); // Formu diğer pencerelerin önüne getirir.
            }

            // 2. Ürün_Girişi formlarını kapat. (Mevcut mantık)
            foreach (Form openForm in Application.OpenForms.OfType<Ürün_Girişi>().ToList())
            {
                openForm.Close();
            }

            // 3. Mevcut formu kapat.
            this.Close();
        }

     

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
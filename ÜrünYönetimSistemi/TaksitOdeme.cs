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
using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;
using DocumentFormat.OpenXml.Drawing.Charts;
namespace ÜrünYönetimSistemi
{
    public partial class TaksitOdeme : Form
    {
        public string MusteriAdi { get; set; }
        public string GsmTelefon { get; set; }

        public TaksitOdeme()
        {
            InitializeComponent();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            // DataGridView'in CellFormatting olayını ekle
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
        }

        private void TaksitOdeme_Load(object sender, EventArgs e)
        {
            try
            {
                string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

                using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
                {
                    baglan.Open();

                    // ID, ToplamTutar, IlkTaksitTarihi, OdemeTarih, OdenenTutar ve Durum sütunlarını çek
                    string satisSorgu = "SELECT ID, ToplamTutar, IlkTaksitTarihi, OdemeTarih, OdenenTutar, Durum FROM MusteriSatis WHERE GsmTelefon = @GsmTelefon AND SatisTuru = 'Taksitli Satış'";

                    using (OleDbDataAdapter da = new OleDbDataAdapter(satisSorgu, baglan))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@GsmTelefon", GsmTelefon);
                        System.Data.DataTable dt = new System.Data.DataTable();
                        da.Fill(dt);

                        // KalanTutar sütununu dinamik olarak hesapla ve ekle
                        dt.Columns.Add("KalanTutar", typeof(decimal));
                        foreach (DataRow row in dt.Rows)
                        {
                            decimal toplamTutar = TryParseDecimal(row["ToplamTutar"]?.ToString());
                            decimal odenenTutar = TryParseDecimal(row["OdenenTutar"]?.ToString());
                            row["KalanTutar"] = toplamTutar - odenenTutar;

                            // Durum boş ise otomatik olarak "Ödenecek" ata
                            if (string.IsNullOrWhiteSpace(row["Durum"]?.ToString()))
                            {
                                row["Durum"] = "Ödenecek";
                            }
                        }

                        // DataGridView'e veriyi yükle
                        dataGridView1.DataSource = dt;
                        dataGridView1.Invalidate();

                        // Sütun başlıklarını ve sıralamasını ayarla
                        dataGridView1.Columns["ID"].HeaderText = "Sıra No";
                        dataGridView1.Columns["IlkTaksitTarihi"].HeaderText = "İlk Taksit Tarihi";
                        dataGridView1.Columns["OdemeTarih"].HeaderText = "Ödeme Tarihi";
                        dataGridView1.Columns["ToplamTutar"].HeaderText = "Toplam Tutar";
                        dataGridView1.Columns["OdenenTutar"].HeaderText = "Ödenen Tutar";
                        dataGridView1.Columns["Durum"].HeaderText = "Durum";
                        dataGridView1.Columns["KalanTutar"].HeaderText = "Kalan Tutar";

                        // Sütun sıralamasını ayarla
                        dataGridView1.Columns["ID"].DisplayIndex = 0;
                        dataGridView1.Columns["IlkTaksitTarihi"].DisplayIndex = 1;
                        dataGridView1.Columns["OdemeTarih"].DisplayIndex = 2;
                        dataGridView1.Columns["ToplamTutar"].DisplayIndex = 3;
                        dataGridView1.Columns["OdenenTutar"].DisplayIndex = 4;
                        dataGridView1.Columns["Durum"].DisplayIndex = 5;
                        dataGridView1.Columns["KalanTutar"].DisplayIndex = 6;

                        // Renkleri manuel olarak uygula (ilk yüklemede CellFormatting tetiklenmeyebilir)
                        UygulaRenkleri();

                        // Form açılışında otomatik seçimi temizle, böylece SelectionChanged tetiklenmesin
                        dataGridView1.ClearSelection();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri çekme sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UygulaRenkleri()
        {
            if (dataGridView1.Rows.Count == 0) return;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue; // Yeni satırı atla

                string durum = row.Cells["Durum"].Value?.ToString();

                // Eğer durum "Ödenecek" ise, tüm satırı kırmızı yap.
                if (durum != null && durum.Equals("Ödenecek", StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.SelectionBackColor = Color.DarkRed;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
                // Eğer durum "Ödendi" ise, tüm satırı yeşil yap.
                else if (durum != null && durum.Equals("Ödendi", StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = Color.Green;
                    row.DefaultCellStyle.SelectionBackColor = Color.DarkGreen;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
                else
                {
                    // Diğer tüm durumlar için satırın varsayılan rengini kullan.
                    row.DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                    row.DefaultCellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.SelectionBackColor;
                    row.DefaultCellStyle.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
                }
            }

            dataGridView1.Refresh(); // Yenilemeyi zorla
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Sadece Durum sütununun içeriğiyle ilgileniyoruz, ancak satırın tamamını renklendiriyoruz.
            if (e.ColumnIndex == dataGridView1.Columns["Durum"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string durum = row.Cells["Durum"].Value?.ToString();

                // Eğer durum "Ödenecek" ise, tüm satırı kırmızı yap.
                if (durum != null && durum.Equals("Ödenecek", StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.SelectionBackColor = Color.DarkRed;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
                // Eğer durum "Ödendi" ise, tüm satırı yeşil yap.
                else if (durum != null && durum.Equals("Ödendi", StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = Color.Green;
                    row.DefaultCellStyle.SelectionBackColor = Color.DarkGreen;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
                else
                {
                    // Diğer tüm durumlar için satırın varsayılan rengini kullan.
                    row.DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                    row.DefaultCellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.SelectionBackColor;
                    row.DefaultCellStyle.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
                }
            }
        }

        private decimal TryParseDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            input = input.Replace(',', '.');
            decimal result = 0;
            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            string satisIDStr = selectedRow.Cells["ID"].Value?.ToString();
            int satisID = 0;
            if (!int.TryParse(satisIDStr, out satisID))
            {
                MessageBox.Show("Seçilen satırda geçerli bir Satış ID bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string odenenTutarStr = selectedRow.Cells["OdenenTutar"].Value?.ToString();
            decimal odenenTutar = TryParseDecimal(odenenTutarStr);

            string toplamTutarStr = selectedRow.Cells["ToplamTutar"].Value?.ToString();
            decimal toplamTutar = TryParseDecimal(toplamTutarStr);

            decimal kalanTutar = toplamTutar - odenenTutar;

            string odemeTutariInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Lütfen Ödeme Tutarını Giriniz:",
                "Taksit Ödemesi",
                "0"
            );

            if (string.IsNullOrWhiteSpace(odemeTutariInput))
                return;

            decimal odemeTutari = TryParseDecimal(odemeTutariInput);
            if (odemeTutari <= 0)
            {
                MessageBox.Show("Geçerli bir ödeme tutarı girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (odemeTutari > kalanTutar)
            {
                MessageBox.Show("Ödeme tutarı kalan borçtan fazla olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

                using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
                {
                    baglan.Open();

                    decimal yeniOdenenTutar = odenenTutar + odemeTutari;
                    string durum = (yeniOdenenTutar >= toplamTutar) ? "Ödendi" : "Ödenecek";

                    string satisGuncelleSorgu = "UPDATE MusteriSatis SET OdemeTarih = @OdemeTarih, OdenenTutar = @OdenenTutar, Durum = @Durum WHERE ID = @SatisID";
                    using (OleDbCommand satisGuncelleCmd = new OleDbCommand(satisGuncelleSorgu, baglan))
                    {
                        satisGuncelleCmd.Parameters.AddWithValue("@OdemeTarih", DateTime.Now.ToShortDateString());
                        satisGuncelleCmd.Parameters.AddWithValue("@OdenenTutar", yeniOdenenTutar.ToString());
                        satisGuncelleCmd.Parameters.AddWithValue("@Durum", durum);
                        satisGuncelleCmd.Parameters.AddWithValue("@SatisID", satisID);
                        satisGuncelleCmd.ExecuteNonQuery();
                    }

                    decimal mevcutTaksit = 0m;
                    string taksitSorgu = "SELECT Taksit FROM Musteriler WHERE GsmTelefon = @GsmTelefon";
                    using (OleDbCommand taksitCmd = new OleDbCommand(taksitSorgu, baglan))
                    {
                        taksitCmd.Parameters.AddWithValue("@GsmTelefon", GsmTelefon);
                        object result = taksitCmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            decimal.TryParse(result.ToString(), out mevcutTaksit);
                        }
                    }

                    // Yeni eklenen kontrol: Taksit değerinin eksiye düşmesini engelle
                    decimal yeniTaksit = mevcutTaksit - odemeTutari;
                    if (yeniTaksit < 0)
                    {
                        yeniTaksit = 0;
                    }

                    string guncelleSorgu = "UPDATE Musteriler SET Taksit = @mevcutTaksit WHERE GsmTelefon = @GsmTelefon";
                    using (OleDbCommand guncelleCmd = new OleDbCommand(guncelleSorgu, baglan))
                    {
                        guncelleCmd.Parameters.AddWithValue("@mevcutTaksit", yeniTaksit.ToString());
                        guncelleCmd.Parameters.AddWithValue("@GsmTelefon", GsmTelefon);
                        guncelleCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show($"Taksit ödemesi başarılı. {odemeTutari} TL borç düşüldü.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                TaksitOdeme_Load(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ödeme işlemi sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Tabloda veri yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // İşletme adını veritabanından çekme
            string isletmeAdi = "";
            string baglantiDizesi = "Provider=Microsoft.ACE.OleDB.12.0;Data Source=" +
                                    Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            try
            {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşletme adı alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Toplam taksit borcunu veritabanından çekme
            decimal toplamTaksitBorcu = 0;
            try
            {
                using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
                {
                    baglan.Open();
                    string taksitSorgu = "SELECT Taksit FROM Musteriler WHERE GsmTelefon = @GsmTelefon";
                    using (OleDbCommand taksitCmd = new OleDbCommand(taksitSorgu, baglan))
                    {
                        taksitCmd.Parameters.AddWithValue("@GsmTelefon", GsmTelefon);
                        object result = taksitCmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            decimal.TryParse(result.ToString(), out toplamTaksitBorcu);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri taksit borcu bilgisi alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Dosyası (*.xlsx)|*.xlsx";
            saveFileDialog.Title = "Excel Dosyasını Kaydet";
            saveFileDialog.FileName = $"{MusteriAdi}_Taksit_Detaylari_{DateTime.Now.ToString("yyyyMMdd")}.xlsx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                bool isSaved = false;
                while (!isSaved)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Müşteri Bilgileri");
                            int currentRow = 1;

                            // İşletme Adı Başlığı
                            worksheet.Range("A1:G1").Merge().Value = isletmeAdi;
                            worksheet.Range("A1:G1").Style.Font.Bold = true;
                            worksheet.Range("A1:G1").Style.Font.FontSize = 16;
                            worksheet.Range("A1:G1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.FromHtml("#4F81BD");
                            worksheet.Range("A1:G1").Style.Font.FontColor = XLColor.White;
                            currentRow += 2;

                            // Müşteri Bilgileri
                            worksheet.Cell(currentRow, 1).Value = "Müşteri Adı:";
                            worksheet.Cell(currentRow, 2).Value = MusteriAdi;
                            worksheet.Cell(currentRow + 1, 1).Value = "Telefon No:";
                            worksheet.Cell(currentRow + 1, 2).Value = GsmTelefon;
                            worksheet.Cell(currentRow + 2, 1).Value = "Toplam Taksit Borcu:";
                            worksheet.Cell(currentRow + 2, 2).Value = toplamTaksitBorcu.ToString("N2") + " TL";
                            worksheet.Range(currentRow, 1, currentRow + 2, 1).Style.Font.Bold = true;
                            currentRow += 4;

                            // DataGridView verilerini Excel'e aktar
                            int headerRow = currentRow;
                            for (int i = 0; i < dataGridView1.Columns.Count; i++)
                            {
                                worksheet.Cell(currentRow, i + 1).Value = dataGridView1.Columns[i].HeaderText;
                                worksheet.Cell(currentRow, i + 1).Style.Font.Bold = true;
                                worksheet.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");
                            }
                            worksheet.Range(currentRow, 1, currentRow, dataGridView1.Columns.Count).Style.Border.SetBottomBorder(XLBorderStyleValues.Thick);
                            worksheet.Range(currentRow, 1, currentRow, dataGridView1.Columns.Count).Style.Border.SetTopBorder(XLBorderStyleValues.Thick);
                            currentRow++;

                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                if (dataGridView1.Rows[i].IsNewRow) continue;
                                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                                {
                                    worksheet.Cell(currentRow + i, j + 1).Value = dataGridView1.Rows[i].Cells[j].Value?.ToString() ?? "";
                                }
                            }

                            // Tablo verisi için kenarlık
                            var dataRange = worksheet.Range(headerRow, 1, currentRow + dataGridView1.Rows.Count - 1, dataGridView1.Columns.Count);
                            dataRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
                            dataRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

                            // Sütun genişliklerini içeriğe göre ayarla
                            worksheet.Columns().AdjustToContents();

                            // Sayısal kolonları sağa hizala
                            var odenenTutarColumn = worksheet.Column("E");
                            if (odenenTutarColumn != null) odenenTutarColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            var kalanTutarColumn = worksheet.Column("G");
                            if (kalanTutarColumn != null) kalanTutarColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            // Arka plan çizgilerini (grid lines) kaldır
                            worksheet.ShowGridLines = false;

                            // Dosyayı kaydetme
                            workbook.SaveAs(saveFileDialog.FileName);
                            isSaved = true; // Kaydetme başarılı oldu, döngüden çık
                        }

                        MessageBox.Show("Excel dosyası başarıyla oluşturuldu.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (IOException ex)
                    {
                        // Dosya kullanımda ise özel hata mesajı ver
                        if (ex.Message.Contains("because it is being used by another process"))
                        {
                            DialogResult result = MessageBox.Show("Oluşturmaya çalıştığınız Excel dosyası açık. Lütfen dosyayı kapatıp tekrar deneyin.", "Dosya Kullanımda", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);

                            if (result == DialogResult.Cancel)
                            {
                                isSaved = true; // Kullanıcı iptal etti, döngüden çık
                            }
                        }
                        else
                        {
                            // Diğer hatalar için genel mesaj
                            MessageBox.Show("Dosya kaydedilirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            isSaved = true; // Diğer hata türlerinde döngüden çık
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Dosya kaydedilirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isSaved = true; // Diğer hata türlerinde döngüden çık
                    }
                }
            }
        }


    }
}
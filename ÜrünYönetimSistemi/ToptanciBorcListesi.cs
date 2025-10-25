using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ÜrünYönetimSistemi
{
    public partial class ToptanciBorcListesi : Form
    {
        private DataTable tablo;

        public ToptanciBorcListesi()
        {
            InitializeComponent();
            textBox1.ReadOnly = true;

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.SortCompare += dataGridView1_SortCompare; // SortCompare olayını ekle
        }

        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Name == "ToplamBorc") // Güncellendi: Toplam Borç -> ToplamBorc
            {
                string value1 = e.CellValue1?.ToString().Trim() ?? "0";
                string value2 = e.CellValue2?.ToString().Trim() ?? "0";
                value1 = value1.Replace(",", ".");
                value2 = value2.Replace(",", ".");
                decimal.TryParse(value1, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal dec1);
                decimal.TryParse(value2, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal dec2);
                e.SortResult = dec1.CompareTo(dec2);
                e.Handled = true;
            }
        }

        private void ToptanciBorcListesi_Load(object sender, EventArgs e)
        {
            // Form yüklendiğinde otomatik olarak Toptancı Adına göre sıralama yapsın.
            // Bu, en baştaki varsayılan ayardır.
            checkBox1.Checked = true;

            dataGridView1.Width = 1057;
            dataGridView1.Height = 354;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Access veritabanı bağlantı dizesi
            string baglantiDizesi = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                     Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
            {
                try
                {
                    baglan.Open();
                    tablo = new DataTable();

                    string query = @"
                        SELECT
                            ToptanciAdi,
                            GsmTelefon,
                            Adres,
                            ToplamBorc
                        FROM Toptancilar"; // Güncellendi: Gerçek sütun adlarını kullanıyoruz

                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, baglan);
                    adapter.Fill(tablo);

                    dataGridView1.DataSource = tablo;

                    if (dataGridView1.Columns.Count >= 4)
                    {
                        // Sütun başlıklarını düzenle
                        dataGridView1.Columns["ToptanciAdi"].HeaderText = "Toptancı Adı"; // Veritabanı adını header olarak ayarla
                        dataGridView1.Columns["GsmTelefon"].HeaderText = "GSM Telefon";
                        dataGridView1.Columns["Adres"].HeaderText = "Adres";
                        dataGridView1.Columns["ToplamBorc"].HeaderText = "Toplam Borç"; // Veritabanı adını header olarak ayarla

                        // Borç sütunu için hizalama ve metin formatı
                        dataGridView1.Columns["ToplamBorc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; // Header'a göre güncellendi
                    }

                    dataGridView1.RowTemplate.Height = 40;

                    GuncelleToplamBorc();

                    // Varsayılan sıralama: ToptanciAdi ascending (checkBox1 tetikleniyor ama garanti olsun)
                    if (dataGridView1.Columns.Contains("ToptanciAdi")) // Güncellendi: Toptancı Adı -> ToptanciAdi
                    {
                        try
                        {
                            dataGridView1.Sort(dataGridView1.Columns["ToptanciAdi"], ListSortDirection.Ascending); // Güncellendi
                            dataGridView1.DataSource = tablo.DefaultView; // Yenile
                        }
                        catch (Exception ex)
                        {
                            // Uyarı kaldırıldı
                        }
                    }
                    else
                    {
                        // Uyarı kaldırıldı
                    }
                }
                catch (Exception ex)
                {
                    // Uyarı kaldırıldı
                }
            }
        }

        private void GuncelleToplamBorc()
        {
            decimal toplamBorc = 0;

            // DataGridView'deki tüm satırlara bak
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["ToplamBorc"].Value != null && row.Cells["ToplamBorc"].Value != DBNull.Value)
                {
                    string borcStr = row.Cells["ToplamBorc"].Value.ToString().Trim();
                    borcStr = borcStr.Replace(",", ".");
                    if (decimal.TryParse(borcStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal borc))
                    {
                        toplamBorc += borc;
                    }
                }
            }

            // Toplam borcu kısa metin formatında yaz
            textBox1.Text = toplamBorc.ToString("N2", CultureInfo.InvariantCulture);
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (tablo == null) return;

            string filtre = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(filtre))
            {
                tablo.DefaultView.RowFilter = "";
            }
            else
            {
                tablo.DefaultView.RowFilter = $"ToptanciAdi LIKE '%{filtre}%' OR GsmTelefon LIKE '%{filtre}%'";
            }

            GuncelleToplamBorc();

            dataGridView1.DataSource = tablo.DefaultView;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;

                if (dataGridView1.Columns.Contains("ToptanciAdi"))
                {
                    try
                    {
                        dataGridView1.Sort(dataGridView1.Columns["ToptanciAdi"], ListSortDirection.Ascending);
                        dataGridView1.DataSource = tablo.DefaultView;
                    }
                    catch (Exception ex)
                    {
                        // Uyarı kaldırıldı
                    }
                }
                else
                {
                    // Uyarı kaldırıldı
                }
            }
            else if (!checkBox2.Checked)
            {
                checkBox1.Checked = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
                checkBox3.Visible = false;

                if (dataGridView1.Columns.Contains("ToplamBorc"))
                {
                    try
                    {
                        dataGridView1.Sort(dataGridView1.Columns["ToplamBorc"], ListSortDirection.Descending);
                        dataGridView1.DataSource = tablo.DefaultView;
                    }
                    catch (Exception ex)
                    {
                        // Uyarı kaldırıldı
                    }
                }
                else
                {
                    // Uyarı kaldırıldı
                }
            }
            else
            {
                if (!checkBox1.Checked)
                {
                    checkBox2.Checked = true;
                }

                checkBox3.Visible = true;
            }
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (tablo == null) return;

            if (checkBox3.Checked)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox2.Visible = false;

                if (tablo.Columns.Contains("ToplamBorc_Decimal"))
                    tablo.DefaultView.RowFilter = "ToplamBorc_Decimal = 0";
                else if (tablo.Columns.Contains("ToplamBorc"))
                    tablo.DefaultView.RowFilter = "CONVERT(ToplamBorc, 'System.Decimal') = 0";

                dataGridView1.DataSource = tablo.DefaultView;
                GuncelleToplamBorc(); // sadece sıfır borçlular
            }
            else
            {
                checkBox2.Visible = true;

                // Filtreyi kaldır
                tablo.DefaultView.RowFilter = "";

                // DataSource’u tekrar ata
                dataGridView1.DataSource = tablo.DefaultView;

                // 🔥 Burada toplamı tekrar güncelle
                GuncelleToplamBorc();

                // İstersen sıralama için checkBox1 açık olsun
                checkBox1.Checked = true;
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                return;
            }

            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Dosyası|*.xlsx";
                sfd.FileName = "ToptanciBorcListesi.xlsx";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Toptancı Borç Listesi");
                    int currentRow = 1;

                    using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                                                        Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                    {
                        baglan.Open();
                        string sorgu = "SELECT TOP 1 IsletmeAdi FROM IsletmeAdi";
                        using (OleDbCommand cmd = new OleDbCommand(sorgu, baglan))
                        {
                            object sonuc = cmd.ExecuteScalar();
                            if (sonuc != null)
                            {
                                worksheet.Range(currentRow, 1, currentRow, dataGridView1.Columns.Count).Merge();
                                worksheet.Cell(currentRow, 1).Value = sonuc.ToString();
                                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                                worksheet.Cell(currentRow, 1).Style.Font.FontSize = 16;
                                worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                worksheet.Cell(currentRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                currentRow += 2;
                            }
                        }
                    }

                    worksheet.Cell(currentRow, 1).Value = "Toplam Borç:";
                    worksheet.Cell(currentRow, 2).Value = textBox1.Text + " TL"; // TL eklendi
                    currentRow += 2;

                    int headerRow = currentRow;

                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        worksheet.Cell(currentRow, i + 1).Value = dataGridView1.Columns[i].HeaderText;
                        worksheet.Cell(currentRow, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                    currentRow++;

                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            var cellValue = dataGridView1.Rows[i].Cells[j].Value;
                            worksheet.Cell(currentRow + i, j + 1).Value = cellValue?.ToString() ?? "";
                        }
                    }

                    var tableRange = worksheet.Range(headerRow, 1, headerRow + dataGridView1.Rows.Count, dataGridView1.Columns.Count);
                    tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    for (int i = 1; i <= dataGridView1.Columns.Count; i++)
                    {
                        worksheet.Column(i).Width = 25;
                    }

                    worksheet.Rows().Height = 22.22;

                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        string colName = dataGridView1.Columns[i].HeaderText;
                        if (colName == "Toplam Borç")
                        {
                            worksheet.Column(i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            worksheet.Column(i + 1).Style.NumberFormat.Format = "#,##0.00 ";
                        }
                    }

                    worksheet.ShowGridLines = false;

                    workbook.SaveAs(sfd.FileName);
                }

                // Excel oluşturulduğunda uyarıyı geri ekledim
                MessageBox.Show("Veriler Excel dosyasına aktarıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Uyarı kaldırıldı
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Toptanci toptanciForm = Application.OpenForms.OfType<Toptanci>().FirstOrDefault();
            if (toptanciForm == null)
            {
                toptanciForm = new Toptanci();
                toptanciForm.Show();
            }
            else
            {
                toptanciForm.BringToFront();
            }

            foreach (Form openForm in Application.OpenForms.OfType<Ürün_Girişi>().ToList())
            {
                openForm.Close();
            }

            this.Close();
        }

      


     
    }
}
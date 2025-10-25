using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office.Word;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.collection;
using iTextSharp.text.pdf.draw;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using RawPrint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ÜrünYönetimSistemi;
using Font = System.Drawing.Font;

// iTextSharp ile çakışmayı önler

namespace ÜrünYönetimSistemi
{
    public partial class FiyatTeklifi : Form
    {
        public Form1 frm1;
        public Form2 frm2;
        private Dictionary<string, decimal> orijinalFiyatlar = new Dictionary<string, decimal>();
        private DataTable dt; // dataGridView1 DataSource
        private ContextMenuStrip contextMenuStrip2; // Sağ tık menüsü
        public FiyatTeklifi()
        {
            InitializeComponent();
            textBox6.Visible = false;
            label9.Visible = false;
            textBox2.Visible = false;

            // dataGridView2 için manuel olarak sütunları ekledim
            dataGridView2.Columns.Add("Barkod_No", "Barkod No");
            dataGridView2.Columns.Add("Ürün_Adi", "Ürün Adı");
            dataGridView2.Columns.Add("Ürün_Grubu", "Ürün Grubu");
            dataGridView2.Columns.Add("Stok_Miktari", "Kalan Stok");
            dataGridView2.Columns.Add("OlcuBirimi", "Ölçü Birimi");
            dataGridView2.Columns.Add("Satis_Fiyati", "Satış Fiyatı");
            dataGridView2.Columns.Add("Alis_Fiyati", "Alış Fiyatı");
            dataGridView2.Columns.Add("2SatisFiyati", "İndirimli Fiyat");
            dataGridView2.Columns.Add("AsgariStok", "Asgari Stok");
            dataGridView2.Columns.Add("Miktar", "Miktar");
            dataGridView2.Columns.Add("ToplamTutar", "Toplam Tutar");

            // <<< İSTENEN DÜZENLENEBİLİRLİK AYARI BAŞLANGIÇ >>>

            // DataGridView'deki tüm sütunları döngüye al
            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                // Varsayılan olarak tüm sütunları salt okunur yap (ReadOnly = true)
                column.ReadOnly = true;
            }

            // Sadece "Satis_Fiyati" ve "Miktar" sütunlarını düzenlenebilir yap (ReadOnly = false)
            if (dataGridView2.Columns.Contains("Satis_Fiyati"))
            {
                dataGridView2.Columns["Satis_Fiyati"].ReadOnly = false;
            }

            if (dataGridView2.Columns.Contains("Miktar"))
            {
                dataGridView2.Columns["Miktar"].ReadOnly = false;
            }

            // Kullanıcının klavye girişi veya F2 ile düzenleme yapmasına izin ver
            dataGridView2.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            // <<< DÜZENLENEBİLİRLİK AYARI BİTİŞ >>>

            dataGridView2.CellValueChanged += dataGridView2_CellValueChanged;
            dataGridView2.EditingControlShowing += dataGridView2_EditingControlShowing;
            dataGridView2.CellEndEdit += dataGridView2_CellEndEdit;
            dataGridView2.CellMouseDown += dataGridView2_CellMouseDown;
            textBox2.ReadOnly = true;

            // Sağ tık menüsü
            contextMenuStrip2 = new ContextMenuStrip();
            ToolStripMenuItem removeItem = new ToolStripMenuItem("Ürünü Kaldır");
            removeItem.Click += new EventHandler(removeItem_Click);
            contextMenuStrip2.Items.Add(removeItem);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            Listele();
        }
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece rakamların (0-9) girilmesine izin ver
            // ve Backspace tuşuna basılmasına izin ver.
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Diğer tüm karakterleri engelle
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Satır indeksinin geçerliliğini kontrol et
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;

            DataGridViewRow clickedRow = dataGridView1.Rows[e.RowIndex];
            DataGridViewRow newRow = new DataGridViewRow();
            newRow.CreateCells(dataGridView2);

            // Verileri DataGridView1'den DataGridView2'ye aktar
            newRow.Cells[0].Value = clickedRow.Cells["Barkod_No"].Value;
            newRow.Cells[1].Value = clickedRow.Cells["Ürün_Adi"].Value;
            newRow.Cells[2].Value = clickedRow.Cells["Ürün_Grubu"].Value;
            newRow.Cells[3].Value = clickedRow.Cells["Stok_Miktari"].Value;
            newRow.Cells[4].Value = clickedRow.Cells["OlcuBirimi"].Value;
            newRow.Cells[5].Value = clickedRow.Cells["Satis_Fiyati"].Value; // Orijinal Satis_Fiyati
            newRow.Cells[6].Value = clickedRow.Cells["Alis_Fiyati"].Value;
            newRow.Cells[7].Value = clickedRow.Cells["2SatisFiyati"].Value;
            newRow.Cells[8].Value = clickedRow.Cells["AsgariStok"].Value;

            // Miktar ve Toplam Tutar Hesaplaması
            decimal miktar = TryParseDecimal(textBox6.Text);
            newRow.Cells[9].Value = miktar;
            decimal satisFiyati = TryParseDecimal(clickedRow.Cells["Satis_Fiyati"].Value);
            // Toplam tutarı hesapla ve biçimlendir
            newRow.Cells[10].Value = (satisFiyati * miktar).ToString("0.00", CultureInfo.InvariantCulture);

            // Orijinal satış fiyatını Tag'e kaydet
            newRow.Tag = satisFiyati.ToString(CultureInfo.InvariantCulture); // Orijinal fiyatı Tag'e kaydet

            // Yeni satırı DataGridView2'ye ekle
            dataGridView2.Rows.Add(newRow);

            // Kontrol kutusu ayarlamaları
            textBox6.Text = "1";
            textBox6.Visible = true;
            label6.Visible = true;
            label9.Visible = true;
            textBox2.Visible = true;

            // --- KALDIRILAN KISIM ---
            // string barkodNo = clickedRow.Cells["Barkod_No"].Value.ToString();
            // DataRow[] rowsToRemove = dt.Select(string.Format("[Barkod_No] = '{0}'", barkodNo));
            // if (rowsToRemove.Length > 0)
            //    dt.Rows.Remove(rowsToRemove[0]); 
            // Bu satırlar kaldırıldığı için, ürün dt tablosundan ve dolayısıyla dataGridView1'den kaybolmayacaktır.

            // Görünürlük ve Hesaplama Metotlarını çağır
            UpdateButton2Visibility();
            HesaplaParaUstuVeKar();
        }

        private void HesaplaParaUstuVeKar()
        {
            if (dataGridView2.Rows.Count == 0)
            {
                textBox2.Text = "0.00";
                textBox6.Visible = false;
                label6.Visible = false;
                return;
            }
            decimal genelToplam = 0m;
            decimal toplamKar = 0m;
            foreach (DataGridViewRow r in dataGridView2.Rows)
            {
                if (r.IsNewRow) continue;
                decimal satis = TryParseDecimal(r.Cells["Satis_Fiyati"].Value);
                decimal alis = TryParseDecimal(r.Cells["Alis_Fiyati"].Value);
                decimal miktar = TryParseDecimal(r.Cells["Miktar"].Value);
                decimal toplamTutar = Math.Round(satis * miktar, 2);
                decimal kar = Math.Round((satis - alis) * miktar, 2);
                r.Cells["ToplamTutar"].Value = toplamTutar.ToString("0.00", CultureInfo.InvariantCulture);
                genelToplam += toplamTutar;
                toplamKar += kar;
            }

            textBox2.Text = genelToplam.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private void Listele()
        {
            string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
            {
                string sorgu = "SELECT Barkod_No, Ürün_Adi, Ürün_Grubu, Stok_Miktari, OlcuBirimi, Satis_Fiyati, Alis_Fiyati, [2SatisFiyati], AsgariStok FROM ÜrünGirişi";
                OleDbDataAdapter da = new OleDbDataAdapter(sorgu, baglan);
                dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt.DefaultView;
                // Başlık ayarları (varsa)
                if (dataGridView1.Columns.Contains("Barkod_No")) dataGridView1.Columns["Barkod_No"].HeaderText = "Barkod No";
                if (dataGridView1.Columns.Contains("Ürün_Adi")) dataGridView1.Columns["Ürün_Adi"].HeaderText = "Ürün Adı";
                if (dataGridView1.Columns.Contains("Ürün_Grubu")) dataGridView1.Columns["Ürün_Grubu"].HeaderText = "Ürün Grubu";
                if (dataGridView1.Columns.Contains("Stok_Miktari")) dataGridView1.Columns["Stok_Miktari"].HeaderText = "Mevcut Stok";
                if (dataGridView1.Columns.Contains("OlcuBirimi")) dataGridView1.Columns["OlcuBirimi"].HeaderText = "Ölçü Birimi";
                if (dataGridView1.Columns.Contains("Satis_Fiyati")) dataGridView1.Columns["Satis_Fiyati"].HeaderText = "Satış Fiyatı";
                if (dataGridView1.Columns.Contains("Alis_Fiyati")) dataGridView1.Columns["Alis_Fiyati"].HeaderText = "Alış Fiyatı";
                if (dataGridView1.Columns.Contains("2SatisFiyati")) dataGridView1.Columns["2SatisFiyati"].HeaderText = "İndirimli Fiyat";
                if (dataGridView1.Columns.Contains("AsgariStok")) dataGridView1.Columns["AsgariStok"].HeaderText = "Asgari Stok";
            }
        }

        private void FiyatTeklifi_Load(object sender, EventArgs e)
        {
            int istenenSatirSayisi = 7;

            // MaxDropDownItems ayarınızı zaten 5 yaptınız, ancak bu ayar görmezden gelindiği için
            // DropDownHeight özelliğini kod ile zorlayacağız.
            comboBox1.MaxDropDownItems = istenenSatirSayisi;

            // Yüksekliği hesaplayın: (İstenen Satır Sayısı * Her Satırın Yüksekliği) + Kenarlık Boşluğu
            comboBox1.DropDownHeight = (istenenSatirSayisi * comboBox1.ItemHeight) + 2;
            textBox1.KeyDown += textBox1_KeyDown;
            // Form yüklenirken textBox6 ve label9'un gizli olduğundan emin ol
            textBox6.Visible = false;
            label6.Visible = false;
            textBox2.Visible = false;
            UpdateButton2Visibility();
            UrunGrubuDoldur();
        }

        private void UrunGrubuDoldur()
        {
            string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
            try
            {
                using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
                {
                    baglan.Open();
                    string sorgu = "SELECT DISTINCT Ürün_Grubu FROM ÜrünGirişi";
                    using (OleDbCommand cmd = new OleDbCommand(sorgu, baglan))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            comboBox1.Items.Clear();
                            comboBox1.Items.Add("Tümü");
                            while (reader.Read())
                            {
                                if (reader["Ürün_Grubu"] != DBNull.Value)
                                    comboBox1.Items.Add(reader["Ürün_Grubu"].ToString());
                            }
                        }
                    }
                }
                comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün grubu listesi yüklenirken hata: " + ex.Message);
            }
        }

        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox tb)
            {
                tb.KeyPress -= new KeyPressEventHandler(dataGridView_KeyPress);
                tb.KeyPress += new KeyPressEventHandler(dataGridView_KeyPress);
            }
        }

        private void dataGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            var cell = dataGridView2.CurrentCell;

            if (cell != null && (cell.ColumnIndex == dataGridView2.Columns["Satis_Fiyati"].Index || cell.ColumnIndex == dataGridView2.Columns["Miktar"].Index))
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

                if (cell.ColumnIndex == dataGridView2.Columns["Satis_Fiyati"].Index)
                {
                    if (e.KeyChar == ',' || e.KeyChar == '.')
                    {
                        if (((TextBox)sender).Text.Contains(','))
                        {
                            e.Handled = true;
                            return;
                        }
                        e.Handled = false;
                        return;
                    }
                }

                if (cell.ColumnIndex == dataGridView2.Columns["Miktar"].Index && (e.KeyChar == ',' || e.KeyChar == '.'))
                {
                    e.Handled = true;
                    return;
                }

                e.Handled = true;
            }
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (cell.ColumnIndex == dataGridView2.Columns["Miktar"].Index)
            {
                if (!int.TryParse(cell.Value?.ToString(), out int miktar))
                {
                    cell.Value = 0;
                }
                else if (miktar < 0)
                {
                    cell.Value = 0;
                }
            }
            else if (cell.ColumnIndex == dataGridView2.Columns["Satis_Fiyati"].Index)
            {
                if (!decimal.TryParse(cell.Value?.ToString(), out decimal satisFiyati))
                {
                    cell.Value = 0.00;
                }
                else if (satisFiyati < 0)
                {
                    cell.Value = 0.00;
                }
            }
        }

        private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dataGridView2.Rows[e.RowIndex].Selected = true;
                contextMenuStrip2.Show(Cursor.Position);
            }
        }

        private void removeItem_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView2.SelectedRows[0];
                DialogResult result = MessageBox.Show(
                    "Seçili ürünü sepetten kaldırmak istediğinize emin misiniz?",
                    "Onay",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

               

                    dataGridView2.Rows.Remove(row);
                    HesaplaParaUstuVeKar();
                    UpdateButton2Visibility();
                }
            }
        


        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string girilenBarkod = textBox1.Text.Trim();
                if (string.IsNullOrEmpty(girilenBarkod)) return;
                DataRow[] bulunanSatirlar = dt.Select(string.Format("[Barkod_No] = '{0}'", girilenBarkod));
                if (bulunanSatirlar.Length > 0)
                {
                    DataRow satir = bulunanSatirlar[0];
                    int index = dataGridView2.Rows.Add();
                    dataGridView2.Rows[index].Cells["Barkod_No"].Value = satir["Barkod_No"];
                    dataGridView2.Rows[index].Cells["Ürün_Adi"].Value = satir["Ürün_Adi"];
                    dataGridView2.Rows[index].Cells["Ürün_Grubu"].Value = satir["Ürün_Grubu"];
                    dataGridView2.Rows[index].Cells["Stok_Miktari"].Value = satir["Stok_Miktari"];
                    dataGridView2.Rows[index].Cells["Satis_Fiyati"].Value = satir["Satis_Fiyati"];
                    dataGridView2.Rows[index].Cells["Alis_Fiyati"].Value = satir["Alis_Fiyati"];
                    dataGridView2.Rows[index].Cells["2SatisFiyati"].Value = satir["2SatisFiyati"];
                    dataGridView2.Rows[index].Cells["AsgariStok"].Value = satir["AsgariStok"];
                    dataGridView2.Rows[index].Cells["Miktar"].Value = TryParseDecimal(textBox6.Text);
                    decimal satisFiyati = TryParseDecimal(satir["Satis_Fiyati"]);
                    dataGridView2.Rows[index].Cells["ToplamTutar"].Value = (satisFiyati * TryParseDecimal(textBox6.Text)).ToString("0.00", CultureInfo.InvariantCulture);

                    // Orijinal satış fiyatını Tag'e kaydet
                    dataGridView2.Rows[index].Tag = satisFiyati.ToString(CultureInfo.InvariantCulture); // Orijinal fiyatı Tag'e kaydet

                    textBox6.Text = "1";
                    textBox6.Visible = true;
                    label6.Visible = true;
                    textBox2.Visible = true;
                    dt.Rows.Remove(satir);
                    UpdateButton2Visibility();
                    HesaplaParaUstuVeKar();
                    textBox1.Clear();
                }
                else
                {
                    MessageBox.Show("Barkod bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var cell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string cellValue = cell.Value?.ToString() ?? "";

            if (dataGridView2.Columns[e.ColumnIndex].Name == "Satis_Fiyati")
            {
                // Geçerli format: rakamlar, opsiyonel 1-2 ondalık basamak, nokta veya virgül
                if (!System.Text.RegularExpressions.Regex.IsMatch(cellValue, @"^\d+([.,]\d{1,2})?$"))
                {
                    // Girilen değeri temizle: nokta/virgül kaldır, sadece rakam ve iki ondalık bırak
                    string cleanedValue = cellValue.Replace(".", "").Replace(",", ".");
                    if (!decimal.TryParse(cleanedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal correctValue))
                    {
                        correctValue = 0m;
                    }

                    MessageBox.Show($"Geçersiz değer: '{cellValue}'. Lütfen {correctValue:0.##} veya {correctValue:0.00} gibi bir değer girin.",
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    cell.Value = correctValue.ToString("0.00", CultureInfo.InvariantCulture);
                }

                // Hücreyi geçerli ise Tag olarak sakla
                cell.Tag = cell.Value;
            }

            // Hesaplama
            decimal miktar = TryParseDecimal(dataGridView2.Rows[e.RowIndex].Cells["Miktar"].Value);
            decimal satisFiyati = TryParseDecimal(dataGridView2.Rows[e.RowIndex].Cells["Satis_Fiyati"].Value);
            decimal toplamTutar = Math.Round(satisFiyati * miktar, 2);
            dataGridView2.Rows[e.RowIndex].Cells["ToplamTutar"].Value = toplamTutar.ToString("0.00", CultureInfo.InvariantCulture);

            HesaplaParaUstuVeKar();
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

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            decimal miktar = TryParseDecimal(textBox6.Text);
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (!row.IsNewRow)
                {
                    row.Cells["Miktar"].Value = miktar.ToString(CultureInfo.InvariantCulture);
                }
            }
            HesaplaParaUstuVeKar(); // Miktar değiştiğinde toplamı güncelle
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0) return;
            DialogResult result = MessageBox.Show(
                "Seçili ürünü sepetten kaldırmak istediğinize emin misiniz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            dataGridView2.Rows.Clear();
            textBox6.Visible = false; // Satış iptal edildiğinde gizle
            label6.Visible = false;   // Satış iptal edildiğinde gizle
            textBox6.Text = "";
            textBox2.Visible = false;
            IskontoIptalVeGeriYukle();
            UpdateButton2Visibility();
            HesaplaParaUstuVeKar();
        
    
        }


        private void UpdateButton2Visibility()
        {
            bool isVisible = dataGridView2.Rows.Count > 0;
            button2.Visible = isVisible;
            button1.Visible = isVisible;
            button4.Visible = isVisible;
            button3.Visible = isVisible;
            label9.Visible = isVisible;
            label5.Visible = isVisible;
            textBox6.Visible = isVisible; // Sadece sepet doluysa görünür
            textBox2.Visible = isVisible; // Sadece sepet doluysa görünür
            label6.Visible = isVisible;   // Sadece sepet doluysa görünür

        }


        private void IskontoIptalVeGeriYukle()
        {


            // Veritabanı bağlantı dizesi
            string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            // Seçili satırları işle
            foreach (DataGridViewRow row in dataGridView2.SelectedRows)
            {
                if (row.IsNewRow) continue; // Yeni satırı atla

                // Yeni satır oluştur
                DataRow newRow = dt.NewRow();

                // Barkod numarasını al
                string barkodNo = row.Cells["Barkod_No"].Value?.ToString();
                decimal orijinalSatisFiyati = 0m;

                // Veritabanından orijinal fiyatı al
                if (!string.IsNullOrEmpty(barkodNo))
                {
                    try
                    {
                        using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
                        {
                            baglan.Open();
                            string sorgu = "SELECT Satis_Fiyati FROM ÜrünGirişi WHERE Barkod_No = @BarkodNo";
                            using (OleDbCommand cmd = new OleDbCommand(sorgu, baglan))
                            {
                                cmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                                object result = cmd.ExecuteScalar();
                                if (result != null && decimal.TryParse(result.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out orijinalSatisFiyati))
                                {
                                    newRow["Satis_Fiyati"] = orijinalSatisFiyati; // Veritabanından alınan orijinal fiyat (50 TL)
                                }
                                else
                                {
                                    MessageBox.Show($"Ürün '{row.Cells["Ürün_Adi"].Value}' için orijinal fiyat veritabanında bulunamadı!",
                                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    continue; // Bu satırı atla
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Veritabanı hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue; // Bu satırı atla
                    }
                }
                else
                {
                    MessageBox.Show($"Ürün '{row.Cells["Ürün_Adi"].Value}' için barkod numarası bulunamadı!",
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue; // Bu satırı atla
                }

                // Diğer sütunları kopyala
                newRow["Barkod_No"] = row.Cells["Barkod_No"].Value;
                newRow["Ürün_Adi"] = row.Cells["Ürün_Adi"].Value;
                newRow["Ürün_Grubu"] = row.Cells["Ürün_Grubu"].Value;
                newRow["Stok_Miktari"] = row.Cells["Stok_Miktari"].Value;
                newRow["OlcuBirimi"] = row.Cells["OlcuBirimi"].Value;
                newRow["Alis_Fiyati"] = row.Cells["Alis_Fiyati"].Value;
                newRow["2SatisFiyati"] = row.Cells["2SatisFiyati"].Value;
                newRow["AsgariStok"] = row.Cells["AsgariStok"].Value;

                // DataTable'a satırı ekle
                dt.Rows.Add(newRow);

                // dataGridView2'deki satırı sil
                dataGridView2.Rows.Remove(row);
            }

            // Toplamları ve görünürlüğü güncelle
            HesaplaParaUstuVeKar();
            UpdateButton2Visibility();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            // Toplam borcu textBox2'den al
            if (!decimal.TryParse(textBox2.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal toplamBorc) || toplamBorc <= 0)
            {
                MessageBox.Show("Geçerli bir toplam borç değeri bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullanıcıdan iskonto yüzdesi al
            string iskontYuzdeInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Lütfen İskonto Yüzdesini Giriniz (%)",
                "İskonto Uygula",
                "0"
            );

            if (string.IsNullOrWhiteSpace(iskontYuzdeInput))
                return;

            if (!decimal.TryParse(iskontYuzdeInput.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal iskontYuzde)
                || iskontYuzde < 0 || iskontYuzde > 100)
            {
                MessageBox.Show("Geçerli bir yüzde değeri girin (0-100 arası)!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal toplamIskontTutar = 0m;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.IsNewRow) continue;

                string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                decimal satisFiyati;

                // Eğer barkod daha önce kaydedilmediyse orijinal fiyatı sakla
                if (!orijinalFiyatlar.ContainsKey(barkodNo))
                {
                    satisFiyati = TryParseDecimal(row.Cells["Satis_Fiyati"].Value);
                    orijinalFiyatlar[barkodNo] = satisFiyati;
                }
                else
                {
                    // Hafızadaki orijinal fiyatı al
                    satisFiyati = orijinalFiyatlar[barkodNo];
                }

                // İskonto uygulanmış fiyat
                decimal iskontoluFiyat = Math.Round(satisFiyati * (1 - iskontYuzde / 100), 2);
                row.Cells["Satis_Fiyati"].Value = iskontoluFiyat.ToString("0.00", CultureInfo.InvariantCulture);

                // Miktarı al
                decimal miktar = TryParseDecimal(row.Cells["Miktar"].Value);

                // Toplam tutarı güncelle
                decimal yeniToplam = Math.Round(iskontoluFiyat * miktar, 2);
                row.Cells["ToplamTutar"].Value = yeniToplam.ToString("0.00", CultureInfo.InvariantCulture);

                // Satır bazında iskonto
                toplamIskontTutar += Math.Round(satisFiyati * miktar * (iskontYuzde / 100), 2);
            }

            // Toplam borcu güncelle
            decimal iskontoluToplam = Math.Round(toplamBorc - toplamIskontTutar, 2);
            textBox2.Text = iskontoluToplam.ToString("0.00", CultureInfo.InvariantCulture);

            MessageBox.Show($"İskonto Oranı: %{iskontYuzde}\nİskonto Tutarı: {toplamIskontTutar:0.00} TL\nİskontolu Toplam: {iskontoluToplam:0.00} TL",
                            "İskonto Uygulandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Filtrele()
        {
            if (dt == null) return;
            string aramaMetni = textBox1.Text.Trim();
            List<string> filtreler = new List<string>();
            if (!string.IsNullOrEmpty(aramaMetni))
            {
                filtreler.Add($"Convert([Barkod_No], 'System.String') LIKE '%{aramaMetni}%' OR Convert([Ürün_Adi], 'System.String') LIKE '%{aramaMetni}%'");
            }
            if (checkBox1.Checked)
            {
                filtreler.Add("[Stok_Miktari] = 0");
            }
            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() != "Tümü")
            {
                string secilenGrup = comboBox1.SelectedItem.ToString().Replace("'", "''");
                filtreler.Add($"[Ürün_Grubu] = '{secilenGrup}'");
            }
            string sonFiltre = string.Join(" AND ", filtreler);
            dt.DefaultView.RowFilter = sonFiltre;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Filtrele();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filtrele();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Filtrele();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Tabloda veri yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string baglantiDizesi = "Provider=Microsoft.ACE.OleDb.12.0;Data Source=" +
                                        Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
                string isletmeAdi = "";

                // İşletme adını çek
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
                string fileName = "FiyatTeklifi_" + ".xlsx";
                sfd.FileName = fileName;

                if (sfd.ShowDialog() != DialogResult.OK) return;

                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Fiyat Teklifi");
                    int row = 1;

                    // Başlık: İşletme adı
                    ws.Range(row, 1, row, 5).Merge();
                    ws.Cell(row, 1).Value = isletmeAdi.ToUpper();
                    ws.Cell(row, 1).Style.Font.Bold = true;
                    ws.Cell(row, 1).Style.Font.FontSize = 18;
                    ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    row += 2;

                    // Fiyat Teklifi Başlığı
                    ws.Range(row, 1, row, 5).Merge();
                    ws.Cell(row, 1).Value = "FİYAT TEKLİFİ";
                    ws.Cell(row, 1).Style.Font.Bold = true;
                    ws.Cell(row, 1).Style.Font.FontSize = 14;
                    ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    row += 2;

                    // Tablo başlıkları
                    string[] headers = { "Barkod No", "Ürün Adı", "Satış Fiyatı", "Miktar", "Toplam Tutar" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cell(row, i + 1).Value = headers[i];
                        ws.Cell(row, i + 1).Style.Font.Bold = true;
                        ws.Cell(row, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                    int headerRow = row;
                    row++;

                    // Satır verileri
                    foreach (DataGridViewRow dgRow in dataGridView2.Rows)
                    {
                        if (dgRow.IsNewRow) continue;

                        ws.Cell(row, 1).Value = dgRow.Cells["Barkod_No"].Value?.ToString();
                        ws.Cell(row, 2).Value = dgRow.Cells["Ürün_Adi"].Value?.ToString();
                        ws.Cell(row, 3).Value = dgRow.Cells["Satis_Fiyati"].Value?.ToString();
                        ws.Cell(row, 4).Value = dgRow.Cells["Miktar"].Value?.ToString();
                        ws.Cell(row, 5).Value = dgRow.Cells["ToplamTutar"].Value?.ToString();
                        row++;
                    }

                    // Kenarlıklar
                    int dataRowsCount = row - headerRow - 1;
                    if (dataRowsCount > 0)
                    {
                        var tableRange = ws.Range(headerRow, 1, headerRow + dataRowsCount, headers.Length);
                        tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    }

                    // Sütun genişliklerini ayarla (genişletilmiş)
                    ws.Columns("A:B").Width = 30; // Barkod ve Ürün Adı daha geniş
                    ws.Columns("C:E").Width = 20; // Diğer sütunlar sabit genişlikte

                    // Satır yüksekliği
                    ws.Rows().Height = 25;

                    // Toplam fiyat
                    row += 1;
                    ws.Cell(row, 4).Value = "Toplam Fiyat:";
                    ws.Cell(row, 4).Style.Font.Bold = true;
                    ws.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    ws.Cell(row, 5).Value = textBox2.Text;
                    ws.Cell(row, 5).Style.Font.Bold = true;
                    ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(row, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // Sayfa ayarları
                    ws.PageSetup.CenterHorizontally = true;
                    ws.ShowGridLines = false;

                    workbook.SaveAs(sfd.FileName);
                    MessageBox.Show("Fiyat Teklifi Excel'e aktarıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel aktarılırken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Sepette ürün olup olmadığını kontrol et
            if (dataGridView2.Rows.Count == 0 || (dataGridView2.Rows.Count == 1 && dataGridView2.Rows[0].IsNewRow))
            {
                MessageBox.Show("Fiş oluşturmak için tabloda veri olmalı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // İşletme bilgilerini veritabanından al
                string baglantiDizesi = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
                string isletmeAdi = "", isletmeAdresi = "", isletmeYeri = "", gsmTelefon = "";

                using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
                {
                    baglan.Open();
                    string isletmeSorgu = "SELECT IsletmeAdi, IsletmeAdresi, IsletmeYeri, GsmTelefon FROM IsletmeAdi";
                    using (OleDbCommand cmd = new OleDbCommand(isletmeSorgu, baglan))
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isletmeAdi = reader["IsletmeAdi"].ToString();
                            isletmeAdresi = reader["IsletmeAdresi"].ToString();
                            isletmeYeri = reader["IsletmeYeri"].ToString();
                            gsmTelefon = "Tlf - " + reader["GsmTelefon"].ToString();
                        }
                    }
                }

                // Fişin boyutlarını ve fontlarını ayarla
                float pageWidth = 315;
                float padding = 5;
                float paperHeight = 0;

                paperHeight += 20; // Üst boşluk
                paperHeight += 15 * 5; // İşletme bilgileri ve aralarındaki boşluklar
                paperHeight += 10; // Çizgi
                paperHeight += 25; // Tarih ve saat
                paperHeight += 15; // "FİYAT TEKLİFİ" başlığı için boşluk
                paperHeight += 20; // Sütun başlıkları
                paperHeight += 5; // Sütun çizgisi

                using (var bmp = new Bitmap(1, 1))
                using (Graphics g = Graphics.FromImage(bmp))
                using (System.Drawing.Font trFont = new System.Drawing.Font("Arial", 8))
                using (System.Drawing.Font trFontSemiBold = new System.Drawing.Font("Arial", 8, FontStyle.Regular))
                {
                    float urunBilgiWidth = pageWidth * 0.45f - padding;
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                        string urunAdi = row.Cells["Ürün_Adi"].Value?.ToString() ?? "";
                        string barkodUrunAdi = $"{barkodNo} / {urunAdi}";
                        SizeF barkodUrunAdiSize = g.MeasureString(barkodUrunAdi, trFontSemiBold, (int)urunBilgiWidth);
                        paperHeight += barkodUrunAdiSize.Height + padding;
                    }
                }

                paperHeight += 10; // Çizgi
                paperHeight += 20; // Genel toplam
                paperHeight += 5;  // Bilgilendirme yazısı için boşluk

                // Yazıcı fişini oluştur
                PrintDocument pd = new PrintDocument();
                pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)pageWidth, (int)Math.Ceiling(paperHeight));
                pd.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

                // PrintPage olayı için metod
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
                        ev.Graphics.DrawString(isletmeAdi, trFontBold, Brushes.Black, new RectangleF(padding, yPos, _pageWidth - 2 * padding, 20), centerFormat);
                        yPos += 15;
                        ev.Graphics.DrawString(isletmeAdresi, trFont, Brushes.Black, new RectangleF(padding, yPos, _pageWidth - 2 * padding, 20), centerFormat);
                        yPos += 15;
                        ev.Graphics.DrawString(isletmeYeri, trFont, Brushes.Black, new RectangleF(padding, yPos, _pageWidth - 2 * padding, 20), centerFormat);
                        yPos += 15;
                        ev.Graphics.DrawString(gsmTelefon, trFont, Brushes.Black, new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                        yPos += 15;

                        // Çizgi
                        ev.Graphics.DrawLine(Pens.Black, padding, yPos, _pageWidth - padding, yPos);
                        yPos += 10;

                        // Tarih ve saat
                        string dateText = $"Tarih: {DateTime.Now.ToShortDateString()}";
                        string timeText = $"Saat: {DateTime.Now.ToLongTimeString()}";
                        float halfWidth = (_pageWidth - 2 * padding) / 2;
                        ev.Graphics.DrawString(dateText, trFont, Brushes.Black, new RectangleF(padding, yPos, halfWidth - 5, 15), leftFormat);
                        ev.Graphics.DrawString(timeText, trFont, Brushes.Black, new RectangleF(padding + halfWidth - 30, yPos, halfWidth, 15), rightFormat);
                        yPos += 25;

                        // Fiyat teklifi başlığı
                        ev.Graphics.DrawString("FİYAT TEKLİFİ", trFontBold, Brushes.Black, new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                        yPos += 23;

                        // Sütun başlıkları
                        ev.Graphics.DrawString("Barkod No / Ürün Adı", trFontBold, Brushes.Black, new RectangleF(padding, yPos, _pageWidth * 0.45f - padding, 15), leftFormat);
                        ev.Graphics.DrawString("Miktar Fiyatı", trFontBold, Brushes.Black, new RectangleF(padding + _pageWidth * 0.45f - 5, yPos, _pageWidth * 0.25f, 15), centerFormat);
                        ev.Graphics.DrawString("Toplam", trFontBold, Brushes.Black, new RectangleF(padding + _pageWidth * 0.7f, yPos, _pageWidth * 0.3f - padding, 15), leftFormat);
                        yPos += 20;

                        // Sütun çizgisi
                        ev.Graphics.DrawLine(Pens.Black, padding, yPos, _pageWidth - padding, yPos);
                        yPos += 5;

                        // Ürünler
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.IsNewRow) continue;
                            string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                            string urunAdi = row.Cells["Ürün_Adi"].Value?.ToString() ?? "";
                            string miktar = $"{TryParseDecimal(row.Cells["Miktar"].Value):N0}";
                            string satisFiyati = $"{TryParseDecimal(row.Cells["Satis_Fiyati"].Value):N2}";
                            string toplamTutar = $"{TryParseDecimal(row.Cells["ToplamTutar"].Value):N2}";

                            float urunBilgiWidth = _pageWidth * 0.45f - padding;
                            float miktarFiyatWidth = _pageWidth * 0.25f;
                            float toplamWidth = _pageWidth * 0.3f - padding;

                            string barkodUrunAdi = $"{barkodNo} / {urunAdi}";
                            string details = $"({miktar} x {satisFiyati})";

                            SizeF barkodUrunAdiSize = ev.Graphics.MeasureString(barkodUrunAdi, trFontSemiBold, (int)urunBilgiWidth);
                            float totalBlockHeight = barkodUrunAdiSize.Height + 5;
                            if (totalBlockHeight < 20) totalBlockHeight = 20;

                            ev.Graphics.DrawString(barkodUrunAdi, trFontSemiBold, Brushes.Black, new RectangleF(padding, yPos, urunBilgiWidth, totalBlockHeight), leftFormat);
                            ev.Graphics.DrawString(details, trFont, Brushes.Black, new RectangleF(padding + urunBilgiWidth - 5, yPos, miktarFiyatWidth, totalBlockHeight), centerFormat);
                            ev.Graphics.DrawString(toplamTutar, trFont, Brushes.Black, new RectangleF(padding + urunBilgiWidth + miktarFiyatWidth, yPos, toplamWidth, totalBlockHeight), leftFormat);

                            yPos += totalBlockHeight;
                        }

                        // Çizgi
                        ev.Graphics.DrawLine(Pens.Black, padding, yPos, _pageWidth - padding, yPos);
                        yPos += 10;

                        // Genel toplam
                        decimal genelToplam = 0m;
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.IsNewRow) continue;
                            genelToplam += TryParseDecimal(row.Cells["ToplamTutar"].Value);
                        }
                        ev.Graphics.DrawString($"GENEL TOPLAM: {genelToplam:N2} TL", trFontBold, Brushes.Black, new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                        yPos += 20;

                        // Bilgilendirme yazısı
                        ev.Graphics.DrawString("BİLGİ AMAÇLIDIR MALİ DEĞERİ YOKTUR", trFontItalic, Brushes.Black, new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                    }
                };

                // Fişi doğrudan yazdır
                pd.Print();
                MessageBox.Show("Fiş başarıyla yazdırıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fiş yazdırılırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
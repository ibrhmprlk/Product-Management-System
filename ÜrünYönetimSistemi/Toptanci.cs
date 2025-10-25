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
using static ÜrünYönetimSistemi.ToptanciHesapDetayi;
using Font = System.Drawing.Font; // iTextSharp ile çakışmayı önler

namespace ÜrünYönetimSistemi
{
    public partial class Toptanci : Form
    {
        private bool isCalculating = false;
        public Form1 frm1;
        public Form2 frm2;


        public Toptanci()
        {
            InitializeComponent();
            // Olayı constructor'da bir kez bağla
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            textBox10.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)) e.Handled = true; };
            textBox11.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)) e.Handled = true; };
            textBox14.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)) e.Handled = true; };
            textBox15.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            textBox17.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            textBox18.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            textBox19.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            textBox7.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            // textBox24 için KeyPress ve KeyDown
            textBox24.KeyPress += (s, e) =>
            {
                // Rakam, kontrol karakteri veya virgül değilse engelle
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
                {
                    e.Handled = true;
                    return;
                }

                // Virgül kontrolü
                if (e.KeyChar == ',')
                {
                    var textBox = (System.Windows.Forms.TextBox)s;
                    // Başa virgül engelleme
                    if (textBox.SelectionStart == 0)
                    {
                        e.Handled = true;
                        return;
                    }
                    // Zaten virgül varsa engelle
                    if (textBox.Text.Contains(","))
                    {
                        e.Handled = true;
                        return;
                    }
                }
            };


            // textBox8 için KeyPress
            textBox8.KeyPress += (s, e) =>
            {
                // Rakam, kontrol karakteri veya virgül değilse engelle
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
                {
                    e.Handled = true;
                    return;
                }

                // Virgül kontrolü
                if (e.KeyChar == ',')
                {
                    var textBox = (System.Windows.Forms.TextBox)s;
                    // Başa virgül engelleme
                    if (textBox.SelectionStart == 0)
                    {
                        e.Handled = true;
                        return;
                    }
                    // Zaten virgül varsa engelle
                    if (textBox.Text.Contains(","))
                    {
                        e.Handled = true;
                        return;
                    }
                }
            };

            // textBox21 için KeyPress
            textBox21.KeyPress += (s, e) =>
            {
                // Rakam, kontrol karakteri veya virgül değilse engelle
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
                {
                    e.Handled = true;
                    return;
                }

                // Virgül kontrolü
                if (e.KeyChar == ',')
                {
                    var textBox = (System.Windows.Forms.TextBox)s;
                    // Başa virgül engelleme
                    if (textBox.SelectionStart == 0)
                    {
                        e.Handled = true;
                        return;
                    }
                    // Zaten virgül varsa engelle
                    if (textBox.Text.Contains(","))
                    {
                        e.Handled = true;
                        return;
                    }
                }
            };
            textBox24.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                {
                    e.SuppressKeyPress = true; // Enter'ın alt satıra geçmesini engelle
                    e.Handled = true; // Olayı tamamen işleme al
                }
            };
            // TextBox'lar için MaxLength özelliğini kodla ayarlama
            textBox10.MaxLength = 50;
            textBox11.MaxLength = 50;
            textBox14.MaxLength = 100;
            textBox15.MaxLength = 11;
            textBox17.MaxLength = 10;
            textBox18.MaxLength = 10;
            textBox19.MaxLength = 10;
            textBox16.MaxLength = 255;
            textBox20.MaxLength = 255;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true; // Bu satırı ekleyin
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            textBox2.ReadOnly = true;
            textBox2.Enabled = false;
            panel1.Visible = false;
            button5.Visible = false;
            panel2.Visible = false;
            button8.Visible = false;
            // Toptancı bilgilerinin değiştirilmesini engelle
            textBox3.ReadOnly = true; // Toptancı Adı
            textBox7.ReadOnly = true; // GSM TelNo
            textBox28.ReadOnly = true; // Toptancı Adı
            textBox22.ReadOnly = true; // GSM TelNo
            // Borç bilgilerinin değiştirilmesini engelle
            textBox4.ReadOnly = true; // Toplam Borç
            textBox27.ReadOnly = true; // Toplam Borç
            button6.Visible = false;
            // Tarih ve saat bilgilerinin değiştirilmesini engelle
            textBox5.ReadOnly = true; // Tarih
            textBox6.ReadOnly = true; // Saat
            // Tarih ve saat bilgilerinin değiştirilmesini engelle
            textBox25.ReadOnly = true; // Tarih
            textBox26.ReadOnly = true; // Saat
            button1.Visible = false;
            button3.Visible = false;
            button4.Visible = false;


            textBox10.Enter += PanelleriGizle;
            textBox11.Enter += PanelleriGizle;
            textBox12.Enter += PanelleriGizle;
            textBox13.Enter += PanelleriGizle;
            textBox14.Enter += PanelleriGizle;
            textBox15.Enter += PanelleriGizle;
            textBox16.Enter += PanelleriGizle;
            textBox17.Enter += PanelleriGizle;
            textBox18.Enter += PanelleriGizle;
            textBox19.Enter += PanelleriGizle;
            textBox20.Enter += PanelleriGizle;
            textBox21.Enter += PanelleriGizle;

            ToplamBorcuGoster();
            Listele();
        }

        private void PanelleriGizle(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
            dataGridView1.Visible = true;
            label1.Visible = true;
            textBox1.Visible = true;
        }
        private void button13_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            textBox1.Visible = true;
            label1.Visible = true;
            panel2.Visible = false;
            textBox23.Text = "";
            textBox24.Text = "";
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            panel1.Visible = false;
            textBox7.Text = "";
            textBox8.Text = "";
            button6.Visible = true;
            dataGridView1.Visible = true;
            textBox1.Visible = true;
            label1.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                DataGridViewRow row = dataGridView1.CurrentRow;
                ToptanciHesapDetayi detayForm = new ToptanciHesapDetayi();
                detayForm.ToptanciAdi = row.Cells[0].Value?.ToString();
                detayForm.GsmTelefon = row.Cells[1].Value?.ToString();
                //detayForm.ToplamBorc = row.Cells[2].Value?.ToString(); // ❌ bunu gönderme
                detayForm.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Lütfen önce bir kullanıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Yeni formu oluştur
            ToptanciBorcListesi borcListesiForm = new ToptanciBorcListesi();
            // Göster
            borcListesiForm.Show();
            // Bu formu kapat
            this.Close();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            // Seçili bir toptancı varsa
            if (dataGridView1.CurrentRow != null)
            {
                // DataGridView'deki güncel bilgileri al
                string toptanciAdi = dataGridView1.CurrentRow.Cells["ToptanciAdi"].Value?.ToString() ?? "";
                string gsmTelefon = dataGridView1.CurrentRow.Cells["GsmTelefon"].Value?.ToString() ?? "";

                // TextBox'lardaki mevcut bilgileri al ve boşlukları temizle
                string textBoxAdi = textBox10.Text.Trim();
                string textBoxGsm = textBox18.Text.Trim();

                // Toptancı adı ve GSM numarasının değişip değişmediğini kontrol et
                bool adiDegisti = textBoxAdi != toptanciAdi;
                bool gsmDegisti = textBoxGsm != gsmTelefon;

                if (adiDegisti && gsmDegisti)
                {
                    MessageBox.Show("Toptancı adı ve GSM numarası değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgileri kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (adiDegisti)
                {
                    MessageBox.Show("Toptancı adı değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgiyi kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (gsmDegisti)
                {
                    MessageBox.Show("Toptancı GSM numarası değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgiyi kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // --- Eğer yukarıdaki kontrollerden geçilirse, kod buradan devam eder ---

            // Panel görünürlüklerini ayarla
            panel2.Visible = true;
            panel1.Visible = false;
            dataGridView1.Visible = false;
            textBox1.Visible = false;
            label1.Visible = false;

            // Seçili bir toptancı varsa, bilgilerini Borç Ödeme paneline aktar
            if (dataGridView1.CurrentRow != null)
            {
                // "ToptanciAdi" ve "GsmTelefon" sütun adlarını kendi DataGridView'inize göre düzenleyin
                textBox28.Text = dataGridView1.CurrentRow.Cells["ToptanciAdi"].Value.ToString();
                textBox22.Text = dataGridView1.CurrentRow.Cells["GsmTelefon"].Value.ToString();
            }

            // Temizlik
            textBox23.Text = string.Empty;
            textBox24.Text = string.Empty;

        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            // Seçili bir toptancı varsa
            if (dataGridView1.CurrentRow != null)
            {
                // DataGridView'deki güncel bilgileri al
                string toptanciAdi = dataGridView1.CurrentRow.Cells["ToptanciAdi"].Value?.ToString() ?? "";
                string gsmTelefon = dataGridView1.CurrentRow.Cells["GsmTelefon"].Value?.ToString() ?? "";

                // TextBox'lardaki mevcut bilgileri al ve boşlukları temizle
                string textBoxAdi = textBox10.Text.Trim();
                string textBoxGsm = textBox18.Text.Trim();

                // Toptancı adı ve GSM numarasının değişip değişmediğini kontrol et
                bool adiDegisti = textBoxAdi != toptanciAdi;
                bool gsmDegisti = textBoxGsm != gsmTelefon;

                if (adiDegisti && gsmDegisti)
                {
                    MessageBox.Show("Toptancı adı ve GSM numarası değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgileri kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (adiDegisti)
                {
                    MessageBox.Show("Toptancı adı değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgiyi kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (gsmDegisti)
                {
                    MessageBox.Show("Toptancı GSM numarası değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgiyi kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // --- Eğer yukarıdaki kontrollerden geçilirse, kod buradan devam eder ---

            // Panel görünürlüklerini ayarla
            panel1.Visible = true;
            panel2.Visible = false;
            dataGridView1.Visible = false;
            textBox1.Visible = false;
            label1.Visible = false;

            // Seçili bir toptancı varsa, bilgilerini Borç Ekleme paneline aktar
            if (dataGridView1.CurrentRow != null)
            {
                // "ToptanciAdi" ve "GsmTelefon" sütun adlarını kendi DataGridView'inize göre düzenleyin
                textBox3.Text = dataGridView1.CurrentRow.Cells["ToptanciAdi"].Value.ToString();
                textBox4.Text = dataGridView1.CurrentRow.Cells["GsmTelefon"].Value.ToString();
            }

            // Temizlik ve saat ataması
            textBox8.Text = string.Empty;
            textBox9.Text = string.Empty;
            textBox7.Text = DateTime.Now.ToLongTimeString();
        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox24.Text))
            {
                MessageBox.Show("Lütfen ödenecek tutarı girin.", "Uyarı");
                return;
            }
            if (string.IsNullOrEmpty(textBox18.Text))
            {
                MessageBox.Show("Lütfen bir GSM numarası girin.", "Uyarı");
                return;
            }

            // ✅ Ödenecek tutar için format kontrolü
            string odenenTutarStr = textBox24.Text.Trim();
            if (odenenTutarStr.Contains(",") && odenenTutarStr.Split(',')[1].Length > 2)
            {
                string dogruFormat = odenenTutarStr.Replace(",", "");
                MessageBox.Show($"Lütfen ödenecek tutarı {dogruFormat} veya {dogruFormat},00 TL olarak girin. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string odemeSekli = "";
            if (checkBox1.Checked)
            {
                odemeSekli = "Nakit";
            }
            else if (checkBox2.Checked)
            {
                odemeSekli = "Kredi Kartı";
            }
            else if (checkBox3.Checked)
            {
                odemeSekli = "Havale";
            }

            if (string.IsNullOrEmpty(odemeSekli))
            {
                MessageBox.Show("Lütfen bir ödeme şekli seçin.", "Uyarı");
                return;
            }

            try
            {
                string toptanciAdi = textBox10.Text;
                string gsmTelefon = textBox18.Text;
                decimal odenenTutar;

                if (!decimal.TryParse(textBox24.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out odenenTutar))
                {
                    MessageBox.Show("Geçerli bir tutar girin.", "Hata");
                    return;
                }
                string aciklama = string.IsNullOrWhiteSpace(textBox23.Text) ?
                    $"Toptancıya Ödeme - {odemeSekli}" :
                    $"Toptancıya Ödeme - {odemeSekli} - {textBox23.Text.Trim()}";
                decimal mevcutBorc = 0;
                if (!string.IsNullOrEmpty(textBox21.Text))
                {
                    if (!decimal.TryParse(textBox21.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out mevcutBorc))
                    {
                        MessageBox.Show("Mevcut borç geçerli bir sayısal değer değil.", "Hata");
                        return;
                    }
                }

                if (odenenTutar > mevcutBorc)
                {
                    MessageBox.Show("Ödeme tutarı mevcut borcu geçemez.", "Hata");
                    return;
                }

                decimal yeniToplamBorc = mevcutBorc - odenenTutar;

                string selectedGsm = gsmTelefon;

                using (OleDbConnection baglan1 = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan1.Open();

                    string updateQuery = "UPDATE Toptancilar SET ToplamBorc = ? WHERE GsmTelefon = ?";
                    using (OleDbCommand cmdUpdate = new OleDbCommand(updateQuery, baglan1))
                    {
                        cmdUpdate.Parameters.Add("?", System.Data.OleDb.OleDbType.Currency).Value = yeniToplamBorc;
                        cmdUpdate.Parameters.Add("?", System.Data.OleDb.OleDbType.VarWChar, 255).Value = gsmTelefon;
                        cmdUpdate.ExecuteNonQuery();
                    }

                    string insertQuery = "INSERT INTO BorcOdeme (ToptanciAdi, GsmTelefon, EskiBorc, OdenenTutar, ToplamKalanBorc, [Tarih/Saat], Aciklama, OdemeSekli) " +
                                         "VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
                    using (OleDbCommand cmdInsert = new OleDbCommand(insertQuery, baglan1))
                    {
                        cmdInsert.Parameters.Add("?", System.Data.OleDb.OleDbType.VarWChar, 255).Value = toptanciAdi;
                        cmdInsert.Parameters.Add("?", System.Data.OleDb.OleDbType.VarWChar, 255).Value = gsmTelefon;
                        cmdInsert.Parameters.Add("?", System.Data.OleDb.OleDbType.Currency).Value = mevcutBorc;
                        cmdInsert.Parameters.Add("?", System.Data.OleDb.OleDbType.Currency).Value = odenenTutar;
                        cmdInsert.Parameters.Add("?", System.Data.OleDb.OleDbType.Currency).Value = yeniToplamBorc;
                        cmdInsert.Parameters.Add("?", System.Data.OleDb.OleDbType.Date).Value = DateTime.Now;
                        cmdInsert.Parameters.Add("?", System.Data.OleDb.OleDbType.VarWChar, 255).Value = aciklama;
                        cmdInsert.Parameters.Add("?", System.Data.OleDb.OleDbType.VarWChar, 50).Value = odemeSekli;
                        cmdInsert.ExecuteNonQuery();
                    }

                    // FİŞ YAZDIRMA KISMI BAŞLANGIÇ
                    if (checkBox4.Checked)
                    {
                        // İşletme bilgilerini al
                        string isletmeAdi = "", isletmeAdresi = "", isletmeYeri = "", gsmTelefonIsletme = "";
                        using (OleDbConnection baglanIsletme = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                        {
                            baglanIsletme.Open();
                            string query = "SELECT IsletmeAdi, IsletmeAdresi, IsletmeYeri, GsmTelefon FROM IsletmeAdi";
                            using (OleDbCommand cmd = new OleDbCommand(query, baglanIsletme))
                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    isletmeAdi = reader["IsletmeAdi"].ToString();
                                    isletmeAdresi = reader["IsletmeAdresi"].ToString();
                                    isletmeYeri = reader["IsletmeYeri"].ToString();
                                    gsmTelefonIsletme = "Tlf - " + reader["GsmTelefon"].ToString();
                                }
                            }
                        }

                        // Fontlar
                        Font trFont = new Font("Arial", 8);
                        Font trFontBold = new Font("Arial", 8, FontStyle.Bold);
                        Font trFontItalic = new Font("Arial", 7, FontStyle.Italic);

                        // Kağıt genişliği ve padding
                        float pageWidth = 315;
                        float padding = 5;

                        // Kağıt yüksekliği tahmini
                        float paperHeight = 0;
                        paperHeight += 20;
                        paperHeight += trFontBold.Height;
                        paperHeight += trFont.Height * 3;
                        paperHeight += 10;
                        paperHeight += 10;
                        paperHeight += trFont.Height * 2;
                        paperHeight += 10;
                        paperHeight += trFontBold.Height + trFont.Height * 5;
                        if (!string.IsNullOrEmpty(textBox23.Text.Trim()))
                        {
                            using (var bmp = new Bitmap(1, 1))
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                float aciklamaWidth = pageWidth - 2 * padding;
                                SizeF aciklamaSize = g.MeasureString("Açıklama: " + textBox23.Text.Trim(), trFont, (int)aciklamaWidth);
                                float lineHeight = trFont.Height;
                                int lineCount = (int)Math.Ceiling(aciklamaSize.Height / lineHeight);
                                paperHeight += Math.Max(20, lineCount * trFont.Height);
                            }
                        }
                        paperHeight += 10;
                        paperHeight += 10;
                        paperHeight += trFontItalic.Height + 30;

                        // PrintDocument
                        PrintDocument pd = new PrintDocument();
                        pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)pageWidth, (int)Math.Ceiling(paperHeight));
                        pd.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

                        pd.PrintPage += (snd, ev) =>
                        {
                            float y = 20;
                            float contentWidth = pageWidth - 2 * padding;
                            StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                            StringFormat leftFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                            StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };

                            // İşletme bilgileri
                            ev.Graphics.DrawString(isletmeAdi, trFontBold, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontBold.Height), centerFormat);
                            y += trFontBold.Height;
                            ev.Graphics.DrawString(isletmeAdresi, trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), centerFormat);
                            y += trFont.Height;
                            ev.Graphics.DrawString(isletmeYeri, trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), centerFormat);
                            y += trFont.Height;
                            ev.Graphics.DrawString(gsmTelefonIsletme, trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), centerFormat);
                            y += trFont.Height + 2;

                            // Çizgi
                            ev.Graphics.DrawLine(Pens.Black, padding, y, pageWidth - padding, y);
                            y += 10;

                            // Tarih ve saat
                            string dateText = $"Tarih: {DateTime.Now:dd.MM.yyyy}";
                            string timeText = $"Saat: {DateTime.Now:HH:mm:ss}";
                            float halfWidth = contentWidth / 2;
                            ev.Graphics.DrawString(dateText, trFont, Brushes.Black, new RectangleF(padding, y, halfWidth - 5, trFont.Height), leftFormat);
                            ev.Graphics.DrawString(timeText, trFont, Brushes.Black, new RectangleF(padding + halfWidth - 25, y, halfWidth, trFont.Height), rightFormat);
                            y += trFont.Height + 10;

                            // Toptancı bilgileri
                            ev.Graphics.DrawString("Toptancı Bilgileri", trFontBold, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontBold.Height), centerFormat);
                            y += trFontBold.Height + padding;
                            ev.Graphics.DrawString($"Toptancı Adı: {toptanciAdi}", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                            y += trFont.Height + padding;
                            ev.Graphics.DrawString($"Telefon: {gsmTelefon}", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                            y += trFont.Height + padding;

                            // Ödeme detayları
                            ev.Graphics.DrawString("Ödeme Detayları", trFontBold, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontBold.Height), centerFormat);
                            y += trFontBold.Height + padding;
                            ev.Graphics.DrawString($"Toplam Borç: {mevcutBorc:N2} TL", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                            y += trFont.Height + padding;
                            ev.Graphics.DrawString($"Ödenen Tutar: {odenenTutar:N2} TL", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                            y += trFont.Height + padding;
                            ev.Graphics.DrawString($"Kalan Borç: {Math.Max(0, mevcutBorc - odenenTutar):N2} TL", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                            y += trFont.Height + padding;

                            // Açıklama
                            if (!string.IsNullOrEmpty(textBox23.Text.Trim()))
                            {
                                using (var bmp = new Bitmap(1, 1))
                                using (Graphics g = Graphics.FromImage(bmp))
                                {
                                    float aciklamaWidth = contentWidth;
                                    SizeF aciklamaSize = g.MeasureString("Açıklama: " + textBox23.Text.Trim(), trFont, (int)aciklamaWidth);
                                    int lineCount = (int)Math.Ceiling(aciklamaSize.Height / trFont.Height);
                                    RectangleF rect = new RectangleF(padding, y, contentWidth, lineCount * trFont.Height);
                                    ev.Graphics.DrawString("Açıklama: " + textBox23.Text.Trim(), trFont, Brushes.Black, rect, leftFormat);
                                    y += lineCount * trFont.Height + padding;
                                }
                            }

                            // Çizgi ve alt bilgi
                            ev.Graphics.DrawLine(Pens.Black, padding, y, pageWidth - padding, y);
                            y += 10;
                            ev.Graphics.DrawString("BİLGİ AMAÇLIDIR MALİ DEĞERİ YOKTUR", trFontItalic, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontItalic.Height), centerFormat);
                            y += trFontItalic.Height + 30;
                        };

                        pd.Print();
                        MessageBox.Show("Ödeme makbuzu başarıyla yazdırıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    // FİŞ YAZDIRMA KISMI SONU

                    string selectQuery = "SELECT ToplamBorc FROM Toptancilar WHERE GsmTelefon=@gsmTelefon";
                    using (OleDbCommand cmdSelect = new OleDbCommand(selectQuery, baglan1))
                    {
                        cmdSelect.Parameters.Add("@gsmTelefon", System.Data.OleDb.OleDbType.VarWChar, 255).Value = gsmTelefon;
                        object result = cmdSelect.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            decimal borc = Convert.ToDecimal(result);
                            textBox21.Text = borc.ToString("N2");
                        }
                    }

                    textBox24.Text = "";
                    textBox23.Text = "";
                    panel2.Visible = false;
                    dataGridView1.Visible = true;

                    Listele();

                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (dataGridView1.Rows[i].Cells["GsmTelefon"].Value?.ToString() == selectedGsm)
                        {
                            dataGridView1.ClearSelection();
                            dataGridView1.Rows[i].Selected = true;
                            dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                            break;
                        }
                    }

                    button5.Visible = false;
                    textBox10.Text = string.Empty;
                    textBox11.Text = string.Empty;
                    textBox12.Text = string.Empty;
                    textBox13.Text = string.Empty;
                    textBox14.Text = string.Empty;
                    textBox15.Text = string.Empty;
                    textBox16.Text = string.Empty;
                    textBox17.Text = string.Empty;
                    textBox18.Text = string.Empty;
                    textBox19.Text = string.Empty;
                    textBox20.Text = string.Empty;
                    textBox21.Text = string.Empty;
                    button10.Visible = true;
                    button1.Visible = false;
                    button3.Visible = false;
                    panel1.Visible = false;
                    panel2.Visible = false;
                    button6.Visible = false;
                    button4.Visible = false;
                    dataGridView1.Visible = true;
                    textBox1.Visible = true;
                    label1.Visible = true;
                    textBox1.Text = "";
                    textBox21.Visible = true;
                    label24.Visible = true;
                    button8.Visible = false;
                    ToplamBorcuGoster();
                    MessageBox.Show("Ödeme sadece seçili toptancı için kaydedildi ve toplam borç güncellendi.", "Bilgi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata");
            }
        }
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Olayı bir kez dinledikten sonra aboneliği kaldır
            dataGridView1.DataBindingComplete -= dataGridView1_DataBindingComplete;
            if (dataGridView1.Tag != null)
            {
                string secilenGsm = dataGridView1.Tag.ToString();
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells["GsmTelefon"].Value != null &&
                        dataGridView1.Rows[i].Cells["GsmTelefon"].Value.ToString() == secilenGsm)
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[i].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                        break;
                    }
                }
                dataGridView1.Tag = null; // Tag'i temizle
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            textBox25.Text = DateTime.Now.ToLongTimeString();
            textBox7.Text = DateTime.Now.ToLongTimeString();
            // Timer'ı başlat
            timer1.Start();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // 1️⃣ Zorunlu kontroller
            if (string.IsNullOrEmpty(textBox10.Text) || string.IsNullOrEmpty(textBox8.Text) || string.IsNullOrEmpty(textBox18.Text))
            {
                MessageBox.Show("Lütfen bir toptancı seçin, GSM numarasını ve eklenecek tutarı girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string toptanciAdi = textBox10.Text;
            string gsmTelefon = textBox18.Text; // Kullanıcıya özel olacak
            string aciklama;

            if (string.IsNullOrWhiteSpace(textBox9.Text))
            {
                aciklama = "Toptancı Borcuna Ekleme Yapıldı";
            }
            else
            {
                aciklama = "Borç Ekleme - " + textBox9.Text;
            }
            DateTime anlikZaman = DateTime.Now;

            // ✅ Eklenen tutar için format kontrolü
            string eklenecekTutarStr = textBox8.Text.Trim();
            if (eklenecekTutarStr.Contains(",") && eklenecekTutarStr.Split(',')[1].Length > 2)
            {
                string dogruFormat = eklenecekTutarStr.Replace(",", "");
                MessageBox.Show($"Lütfen eklenecek tutarı {dogruFormat} veya {dogruFormat},00 TL olarak girin. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2️⃣ Eklenen tutarı decimal olarak al
            decimal eklenecekTutar;
            if (!decimal.TryParse(eklenecekTutarStr.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out eklenecekTutar))
            {
                MessageBox.Show("Lütfen geçerli bir tutar girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // 3️⃣ Mevcut borcu textBox21'den al
                decimal mevcutBorc = 0;
                if (!string.IsNullOrEmpty(textBox21.Text))
                {
                    if (!decimal.TryParse(textBox21.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out mevcutBorc))
                    {
                        MessageBox.Show("Mevcut borç geçerli bir sayısal değer değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                using (OleDbConnection baglan4 = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan4.Open();

                    // 4️⃣ Yeni toplam borcu hesapla
                    decimal yeniToplamBorc = mevcutBorc + eklenecekTutar;

                    // 5️⃣ Sadece seçilen GSM numarasına özel güncelle
                    string updateQuery = "UPDATE Toptancilar SET ToplamBorc=@yeniToplamBorc WHERE GsmTelefon=@gsmTelefon";
                    using (OleDbCommand cmdUpdate = new OleDbCommand(updateQuery, baglan4))
                    {
                        cmdUpdate.Parameters.Add("@yeniToplamBorc", OleDbType.Currency).Value = yeniToplamBorc;
                        cmdUpdate.Parameters.Add("@gsmTelefon", OleDbType.VarWChar, 255).Value = gsmTelefon;
                        cmdUpdate.ExecuteNonQuery();
                    }

                    // 6️⃣ Borç ekleme kaydını yine sadece o GSM için ekle
                    string insertQuery = "INSERT INTO BorcEkleme (ToptanciAdi, GsmTelefon, EskiBorc, ToplamBorc, [Tarih/Saat], EklenenTutar, Aciklama) " +
                                         "VALUES (@toptanciAdi, @gsmTelefon, @eskiBorc, @toplamBorc, @tarihsaat, @eklenecekTutar, @aciklama)";
                    using (OleDbCommand cmdInsert = new OleDbCommand(insertQuery, baglan4))
                    {
                        cmdInsert.Parameters.Add("@toptanciAdi", OleDbType.VarWChar, 255).Value = toptanciAdi;
                        cmdInsert.Parameters.Add("@gsmTelefon", OleDbType.VarWChar, 255).Value = gsmTelefon;
                        cmdInsert.Parameters.Add("@eskiBorc", OleDbType.Currency).Value = mevcutBorc;
                        cmdInsert.Parameters.Add("@toplamBorc", OleDbType.Currency).Value = yeniToplamBorc;
                        cmdInsert.Parameters.Add("@tarihsaat", OleDbType.Date).Value = anlikZaman;
                        cmdInsert.Parameters.Add("@eklenecekTutar", OleDbType.Currency).Value = eklenecekTutar;
                        cmdInsert.Parameters.Add("@aciklama", OleDbType.VarWChar, 255).Value = aciklama;
                        cmdInsert.ExecuteNonQuery();
                    }

                    // 7️⃣ Güncellemeleri textboxlara ve tabloya yansıt
                    textBox21.Text = yeniToplamBorc.ToString("N2");
                    textBox8.Text = "";
                    textBox9.Text = "";

                    panel1.Visible = false;
                    dataGridView1.Visible = true;
                    button5.Visible = false;
                    textBox10.Text = string.Empty;
                    textBox11.Text = string.Empty;
                    textBox12.Text = string.Empty;
                    textBox13.Text = string.Empty;
                    textBox14.Text = string.Empty;
                    textBox15.Text = string.Empty;
                    textBox16.Text = string.Empty;
                    textBox17.Text = string.Empty;
                    textBox18.Text = string.Empty;
                    textBox19.Text = string.Empty;
                    textBox20.Text = string.Empty;
                    textBox21.Text = string.Empty;
                    button10.Visible = true;
                    button1.Visible = false;
                    button3.Visible = false;
                    panel1.Visible = false;
                    panel2.Visible = false;
                    button6.Visible = false;
                    button4.Visible = false;
                    dataGridView1.Visible = true;
                    textBox1.Visible = true;
                    label1.Visible = true;
                    textBox1.Text = "";
                    textBox21.Visible = true;
                    label24.Visible = true;
                    button8.Visible = false;
                    ToplamBorcuGoster();
                    // Seçili satırın GSM numarasını geri seç
                    string selectedGsm = gsmTelefon;
                    Listele();
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (dataGridView1.Rows[i].Cells["GsmTelefon"].Value?.ToString() == selectedGsm)
                        {
                            dataGridView1.ClearSelection();
                            dataGridView1.Rows[i].Selected = true;
                            dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                            break;
                        }
                    }

                    MessageBox.Show("Borç sadece seçili toptancı için eklendi ve toplam borç güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Checked)
            {
                // Diğer checkBox'ları kapat
                if (cb != checkBox1) checkBox1.Checked = false;
                if (cb != checkBox2) checkBox2.Checked = false;
                if (cb != checkBox3) checkBox3.Checked = false;
            }
            else
            {
                // Hiçbirini seçmeden bırakmayalım, en az birini seçili tut
                if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked)
                {
                    cb.Checked = true;
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            // Silinecek bir satırın seçili olup olmadığını kontrol et
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz toptancıyı listeden seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullanıcıdan silme işlemini onaylamasını iste
            DialogResult result = MessageBox.Show("Seçili toptancıyı silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    // Seçilen toptancının GSM numarasını al
                    string gsmTelefonu = dataGridView1.SelectedRows[0].Cells["GsmTelefon"].Value.ToString();

                    using (OleDbConnection baglan5 = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                    {
                        baglan5.Open();

                        // Önce BorcEkleme tablosundan sil
                        string queryBorcEkleme = "DELETE FROM BorcEkleme WHERE GsmTelefon = @gsmTelefonu";
                        OleDbCommand cmdBorcEkleme = new OleDbCommand(queryBorcEkleme, baglan5);
                        cmdBorcEkleme.Parameters.AddWithValue("@gsmTelefonu", gsmTelefonu);
                        cmdBorcEkleme.ExecuteNonQuery();

                        // Sonra BorcOdeme tablosundan sil
                        string queryBorcOdeme = "DELETE FROM BorcOdeme WHERE GsmTelefon = @gsmTelefonu";
                        OleDbCommand cmdBorcOdeme = new OleDbCommand(queryBorcOdeme, baglan5);
                        cmdBorcOdeme.Parameters.AddWithValue("@gsmTelefonu", gsmTelefonu);
                        cmdBorcOdeme.ExecuteNonQuery();

                        // UrunIade tablosundan sil
                        string queryUrunIade = "DELETE FROM UrunIade WHERE GsmTelefon = @gsmTelefonu";
                        OleDbCommand cmdUrunIade = new OleDbCommand(queryUrunIade, baglan5);
                        cmdUrunIade.Parameters.AddWithValue("@gsmTelefonu", gsmTelefonu);
                        cmdUrunIade.ExecuteNonQuery();

                        // En son Toptancilar tablosundan sil
                        string queryToptanci = "DELETE FROM Toptancilar WHERE GsmTelefon = @gsmTelefonu";
                        OleDbCommand cmdToptanci = new OleDbCommand(queryToptanci, baglan5);
                        cmdToptanci.Parameters.AddWithValue("@gsmTelefonu", gsmTelefonu);
                        int etkilenenSatirSayisi = cmdToptanci.ExecuteNonQuery();

                        if (etkilenenSatirSayisi > 0)
                        {
                            MessageBox.Show("Toptancı ve ilişkili borç ile iade kayıtları başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Alanları temizle
                            textBox10.Text = string.Empty;
                            textBox11.Text = string.Empty;
                            textBox12.Text = string.Empty;
                            textBox13.Text = string.Empty;
                            textBox14.Text = string.Empty;
                            textBox15.Text = string.Empty;
                            textBox16.Text = string.Empty;
                            textBox17.Text = string.Empty;
                            textBox18.Text = string.Empty;
                            textBox19.Text = string.Empty;
                            textBox20.Text = string.Empty;
                            textBox21.Visible = true;
                            textBox21.Text = "";
                            textBox21.ReadOnly = false;
                            button1.Visible = false;
                            button10.Visible = true;
                            button3.Visible = false;
                            button6.Visible = false;
                            panel1.Visible = false;
                            button5.Visible = false;
                            panel2.Visible = false;
                            button5.Visible = false;
                            textBox1.Text = "";
                            button4.Visible = false;
                            label1.Visible = true;
                            textBox1.Visible = true;
                            button8.Visible = false;
                            dataGridView1.Visible = true;

                            Listele();
                            ToplamBorcuGoster();
                        }
                        else
                        {
                            MessageBox.Show("Silme işlemi başarısız oldu veya toptancı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            string aramaMetni = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(aramaMetni))
            {
                Listele(); // Arama metni boşsa tüm toptancıları göster
            }
            else
            {
                try
                {
                    using (OleDbConnection baglan6 = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                    {
                        DataTable tablo = new DataTable();
                        baglan6.Open();
                        // Hem GSMTelefon hem de ToptanciAdi alanlarında arama yapar
                        string query = "SELECT * FROM Toptancilar WHERE GsmTelefon LIKE @arama OR ToptanciAdi LIKE @arama";
                        OleDbDataAdapter adapter = new OleDbDataAdapter(query, baglan6);
                        adapter.SelectCommand.Parameters.AddWithValue("@arama", "%" + aramaMetni + "%");
                        adapter.Fill(tablo);
                        dataGridView1.DataSource = tablo;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Arama sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz toptancıyı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Güncellenecek toptancının eski GSM telefon numarasını al (Birincil Anahtar)
            string eskiGsmTelefon = dataGridView1.SelectedRows[0].Cells["GsmTelefon"].Value.ToString();
            if (string.IsNullOrWhiteSpace(eskiGsmTelefon))
            {
                MessageBox.Show("Eski GSM telefon numarası alınamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            List<string> errors = new List<string>();
            // Zorunlu alan kontrolleri
            if (string.IsNullOrWhiteSpace(textBox10.Text)) errors.Add("Toptancı Adı boş olamaz.");
            if (string.IsNullOrWhiteSpace(textBox18.Text)) errors.Add("GSM Telefonu boş olamaz.");
            // Format kontrolleri
            if (!string.IsNullOrWhiteSpace(textBox10.Text) && !Regex.IsMatch(textBox10.Text, @"^[a-zA-ZçÇğĞıİöÖşŞüÜ\s]+$"))
                errors.Add("Toptancı Adı sadece harf ve boşluk içermelidir.");
            if (!string.IsNullOrWhiteSpace(textBox12.Text) && !Regex.IsMatch(textBox12.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Lütfen geçerli bir E-posta adresi giriniz.");
            if (!string.IsNullOrWhiteSpace(textBox14.Text) && !Regex.IsMatch(textBox14.Text, @"^[a-zA-ZçÇğĞıİöÖşŞüÜ\s]+$"))
                errors.Add("Vergi Dairesi sadece harflerden ve boşluklardan oluşmalıdır.");
            if (!string.IsNullOrWhiteSpace(textBox15.Text) && !Regex.IsMatch(textBox15.Text, @"^\d{10,11}$"))
                errors.Add("Vergi Numarası 10 veya 11 haneli bir sayı olmalıdır.");
            if (!string.IsNullOrWhiteSpace(textBox17.Text) && !Regex.IsMatch(textBox17.Text, @"^\d{10}$"))
                errors.Add("İş Telefonu 10 haneli bir sayı olmalıdır.");
            if (!string.IsNullOrWhiteSpace(textBox18.Text) && !Regex.IsMatch(textBox18.Text, @"^\d{10}$"))
                errors.Add("GSM Telefonu 10 haneli bir sayı olmalıdır.");
            if (!string.IsNullOrWhiteSpace(textBox19.Text) && !Regex.IsMatch(textBox19.Text, @"^\d{10}$"))
                errors.Add("Fax numarası 10 haneli bir sayı olmalıdır.");
            decimal toplamBorc = 0;
            if (!string.IsNullOrWhiteSpace(textBox21.Text))
            {
                if (!decimal.TryParse(textBox21.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out toplamBorc))
                    errors.Add("Toptancı Borcu geçerli bir sayısal değer olmalıdır.");
            }
            if (errors.Any())
            {
                string errorMessage = "Lütfen aşağıdaki hataları düzeltin:\n\n" + string.Join("\n", errors);
                MessageBox.Show(errorMessage, "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (OleDbConnection baglan7 = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan7.Open();
                    // Yeni GSM benzersizliğini kontrol et (eski kayıt hariç)
                    string yeniGsm = textBox18.Text;
                    if (yeniGsm != eskiGsmTelefon)
                    {
                        string gsmQuery = "SELECT COUNT(*) FROM Toptancilar WHERE GsmTelefon = @yeniGsm";
                        OleDbCommand gsmKmt = new OleDbCommand(gsmQuery, baglan7);
                        gsmKmt.Parameters.AddWithValue("@yeniGsm", yeniGsm);
                        int mevcutGsm = (int)gsmKmt.ExecuteScalar();
                        if (mevcutGsm > 0)
                        {
                            MessageBox.Show("Bu GSM numarasına sahip bir toptancı zaten kayıtlı. Farklı bir numara giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    string yeniMail = textBox12.Text;
                    if (!string.IsNullOrWhiteSpace(yeniMail))
                    {
                        string eskiMail = dataGridView1.SelectedRows[0].Cells["EMail"].Value.ToString();
                        if (yeniMail != eskiMail) // Eğer kullanıcı e-postayı değiştirmişse kontrol et
                        {
                            string mailQuery = "SELECT COUNT(*) FROM Toptancilar WHERE EMail = @yeniMail";
                            OleDbCommand mailKmt = new OleDbCommand(mailQuery, baglan7);
                            mailKmt.Parameters.AddWithValue("@yeniMail", yeniMail);
                            int mevcutMail = (int)mailKmt.ExecuteScalar();
                            if (mevcutMail > 0)
                            {
                                MessageBox.Show("Bu E-posta adresine sahip bir toptancı zaten kayıtlı. Farklı bir e-posta giriniz.",
                                                "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    // Güncelleme sorgusu (birincil anahtar olan GSM'i WHERE koşulunda kullan)
                    string updateQuery = "UPDATE Toptancilar SET ToptanciAdi=@toptanciAdi, SirketYetkilisi=@yetkiliAdi, EMail=@eMail, InternetAdresi=@internetAdresi, Vd=@vd, Vn=@vn, Adres=@adres, IsTelefon=@isTelefonu, GsmTelefon=@gsmTelefonu, Fax=@fax, OzelNotlar=@ozelNotlar, ToplamBorc=@toplamBorc WHERE GsmTelefon=@eskiGsmTelefon";
                    OleDbCommand updateKmt = new OleDbCommand(updateQuery, baglan7);
                    updateKmt.Parameters.AddWithValue("@toptanciAdi", textBox10.Text);
                    updateKmt.Parameters.AddWithValue("@yetkiliAdi", textBox11.Text);
                    updateKmt.Parameters.AddWithValue("@eMail", textBox12.Text);
                    updateKmt.Parameters.AddWithValue("@internetAdresi", textBox13.Text);
                    updateKmt.Parameters.AddWithValue("@vd", textBox14.Text);
                    updateKmt.Parameters.AddWithValue("@vn", textBox15.Text);
                    updateKmt.Parameters.AddWithValue("@adres", textBox16.Text);
                    updateKmt.Parameters.AddWithValue("@isTelefonu", textBox17.Text);
                    updateKmt.Parameters.AddWithValue("@gsmTelefonu", textBox18.Text);
                    updateKmt.Parameters.AddWithValue("@fax", textBox19.Text);
                    updateKmt.Parameters.AddWithValue("@ozelNotlar", textBox20.Text);
                    updateKmt.Parameters.AddWithValue("@toplamBorc", toplamBorc);
                    updateKmt.Parameters.AddWithValue("@eskiGsmTelefon", eskiGsmTelefon);
                    int kayitSayisi = updateKmt.ExecuteNonQuery();
                    if (kayitSayisi > 0)
                    {
                        MessageBox.Show("Toptancı başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Alanları temizle ve form ayarlarını sıfırla
                        // Bu kısım için daha önce gönderdiğin button12_Click metodunu çağırabilirsin.
                        // Veya aşağıdaki gibi manuel temizlik yapabilirsin.
                        textBox10.Text = textBox11.Text = textBox12.Text = textBox13.Text = textBox14.Text = textBox15.Text = textBox16.Text = textBox17.Text = textBox18.Text = textBox19.Text = textBox20.Text = string.Empty;
                        panel1.Visible = button1.Visible = button3.Visible = button5.Visible = button6.Visible = panel2.Visible = false;
                        textBox21.Visible = true;
                        textBox21.Text = "";
                        textBox21.ReadOnly = false;
                        button4.Visible = false;
                        button10.Visible = true;
                        textBox1.Text = "";
                        button8.Visible = false;
                        dataGridView1.Visible = true;
                        Listele();
                        ToplamBorcuGoster();
                    }
                    else
                    {
                        MessageBox.Show("Toptancı güncellenemedi veya herhangi bir değişiklik yapılmadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();

            // Zorunlu alan kontrolleri
            if (string.IsNullOrWhiteSpace(textBox10.Text)) { errors.Add("Toptancı Adı boş olamaz."); }
            if (string.IsNullOrWhiteSpace(textBox18.Text)) { errors.Add("GSM Telefonu boş olamaz."); }

            // Metin ve format kontrolleri
            if (!string.IsNullOrWhiteSpace(textBox10.Text) && !Regex.IsMatch(textBox10.Text, @"^[a-zA-ZçÇğĞıİöÖşŞüÜ\s]+$")) { errors.Add("Toptancı Adı sadece harf ve boşluk içermelidir."); }
            if (!string.IsNullOrWhiteSpace(textBox12.Text) && !Regex.IsMatch(textBox12.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) { errors.Add("Lütfen geçerli bir E-posta adresi giriniz."); }
            if (!string.IsNullOrWhiteSpace(textBox14.Text) && !Regex.IsMatch(textBox14.Text, @"^[a-zA-ZçÇğĞıİöÖşŞüÜ\s]+$")) { errors.Add("Vergi Dairesi sadece harflerden ve boşluklardan oluşmalıdır."); }
            if (!string.IsNullOrWhiteSpace(textBox15.Text) && !Regex.IsMatch(textBox15.Text, @"^\d{10,11}$")) { errors.Add("Vergi Numarası 10 veya 11 haneli bir sayı olmalıdır."); }
            if (!string.IsNullOrWhiteSpace(textBox17.Text) && !Regex.IsMatch(textBox17.Text, @"^\d{10}$")) { errors.Add("İş Telefonu 10 haneli bir sayı olmalıdır."); }
            if (!string.IsNullOrWhiteSpace(textBox18.Text) && !Regex.IsMatch(textBox18.Text, @"^\d{10}$")) { errors.Add("GSM Telefonu 10 haneli bir sayı olmalıdır."); }
            if (!string.IsNullOrWhiteSpace(textBox19.Text) && !Regex.IsMatch(textBox19.Text, @"^\d{10}$")) { errors.Add("Fax numarası 10 haneli bir sayı olmalıdır."); }

            // --- Toplam Borç kısmı için dinamik uyarı eklendi ---
            decimal toplamBorc = 0;
            if (!string.IsNullOrWhiteSpace(textBox21.Text))
            {
                string borcMetni = textBox21.Text.Trim();
                // Virgül binlik ayıracı olarak kullanılıyorsa uyarı ver
                if (borcMetni.Contains(",") && borcMetni.Split(',')[1].Length > 2)
                {
                    string dogruFormat = borcMetni.Replace(",", "");
                    MessageBox.Show($"Lütfen borç miktarını {dogruFormat} veya {dogruFormat},00 TL olarak girin. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Binlik ayırıcıları kaldır, ondalık ayırıcıyı nokta olarak kullan
                borcMetni = borcMetni.Replace(".", "").Replace(",", ".");
                if (!decimal.TryParse(borcMetni, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out toplamBorc))
                {
                    errors.Add("Toptancı Borcu geçerli bir sayısal değer olmalıdır.");
                }
            }
            // --- Uyarı kısmı sonu ---

            if (errors.Any())
            {
                string errorMessage = "Lütfen aşağıdaki hataları düzeltin:\n\n" + string.Join("\n", errors);
                MessageBox.Show(errorMessage, "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (OleDbConnection baglan8 = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan8.Open();

                    // Aynı GSM kontrolü
                    string gsmQuery = "SELECT COUNT(*) FROM Toptancilar WHERE GsmTelefon = @gsmTelefonu";
                    OleDbCommand gsmKmt = new OleDbCommand(gsmQuery, baglan8);
                    gsmKmt.Parameters.AddWithValue("@gsmTelefonu", textBox18.Text);
                    int mevcutGsm = (int)gsmKmt.ExecuteScalar();
                    if (mevcutGsm > 0)
                    {
                        MessageBox.Show("Bu GSM numarasına sahip bir toptancı zaten kayıtlı. Farklı bir numara giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Aynı E-posta kontrolü (sadece e-posta girildiyse)
                    if (!string.IsNullOrWhiteSpace(textBox12.Text))
                    {
                        string mailQuery = "SELECT COUNT(*) FROM Toptancilar WHERE EMail = @eMail";
                        OleDbCommand mailKmt = new OleDbCommand(mailQuery, baglan8);
                        mailKmt.Parameters.AddWithValue("@eMail", textBox12.Text);
                        int mevcutMail = (int)mailKmt.ExecuteScalar();
                        if (mevcutMail > 0)
                        {
                            MessageBox.Show("Bu E-posta adresine sahip bir toptancı zaten kayıtlı. Farklı bir e-posta giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Kayıt ekleme
                    string insertQuery = "INSERT INTO Toptancilar (ToptanciAdi, SirketYetkilisi, EMail, InternetAdresi, Vd, Vn, Adres, IsTelefon, GsmTelefon, Fax, OzelNotlar, ToplamBorc) " +
                                         "VALUES (@toptanciAdi, @yetkiliAdi, @eMail, @internetAdresi, @vd, @vn, @adres, @isTelefonu, @gsmTelefonu, @fax, @ozelNotlar, @toplamBorc)";
                    OleDbCommand insertKmt = new OleDbCommand(insertQuery, baglan8);
                    insertKmt.Parameters.AddWithValue("@toptanciAdi", textBox10.Text);
                    insertKmt.Parameters.AddWithValue("@yetkiliAdi", textBox11.Text);
                    insertKmt.Parameters.AddWithValue("@eMail", textBox12.Text);
                    insertKmt.Parameters.AddWithValue("@internetAdresi", textBox13.Text);
                    insertKmt.Parameters.AddWithValue("@vd", textBox14.Text);
                    insertKmt.Parameters.AddWithValue("@vn", textBox15.Text);
                    insertKmt.Parameters.AddWithValue("@adres", textBox16.Text);
                    insertKmt.Parameters.AddWithValue("@isTelefonu", textBox17.Text);
                    insertKmt.Parameters.AddWithValue("@gsmTelefonu", textBox18.Text);
                    insertKmt.Parameters.AddWithValue("@fax", textBox19.Text);
                    insertKmt.Parameters.AddWithValue("@ozelNotlar", textBox20.Text);
                    insertKmt.Parameters.AddWithValue("@toplamBorc", toplamBorc);

                    int kayitSayisi = insertKmt.ExecuteNonQuery();
                    if (kayitSayisi > 0)
                    {
                        MessageBox.Show("Toptancı başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Alanları temizle
                        textBox10.Text = textBox11.Text = textBox12.Text = textBox13.Text = textBox14.Text = textBox15.Text =
                        textBox16.Text = textBox17.Text = textBox18.Text = textBox19.Text = textBox20.Text = textBox21.Text = string.Empty;
                        textBox1.Text = "";
                        textBox21.Visible = true;
                        label24.Visible = true;
                        Listele();
                        ToplamBorcuGoster();
                    }
                    else
                    {
                        MessageBox.Show("Toptancı eklenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (OleDbException ex) { MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            catch (Exception ex) { MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Listele()
        {
            try
            {
                using (OleDbConnection baglan10 = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    DataTable tablo = new DataTable();
                    tablo.Clear();
                    OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM Toptancilar", baglan10);
                    adapter.Fill(tablo);
                    dataGridView1.DataSource = tablo;

                    // Sütun başlıklarını ve genişliklerini ayarla
                    dataGridView1.Columns[0].HeaderText = "Toptancı Adı";
                    dataGridView1.Columns[0].Width = 300;
                    dataGridView1.Columns[1].HeaderText = "GSM Telefon No";
                    dataGridView1.Columns[1].Width = 200;
                    dataGridView1.Columns[2].HeaderText = "Toplam Borç";
                    dataGridView1.Columns[2].Width = 180;
                    dataGridView1.Columns[3].HeaderText = "Şirket Yetkilisi";
                    dataGridView1.Columns[3].Width = 150;
                    dataGridView1.Columns[4].HeaderText = "E-Mail";
                    dataGridView1.Columns[4].Width = 150;
                    dataGridView1.Columns[5].HeaderText = "İnternet Adresi";
                    dataGridView1.Columns[5].Width = 150;
                    dataGridView1.Columns[6].HeaderText = "Vergi Dairesi";
                    dataGridView1.Columns[6].Width = 150;
                    dataGridView1.Columns[7].HeaderText = "Vergi Numarası";
                    dataGridView1.Columns[7].Width = 150;
                    dataGridView1.Columns[8].HeaderText = "Adres";
                    dataGridView1.Columns[8].Width = 200;
                    dataGridView1.Columns[9].HeaderText = "İş Telefonu";
                    dataGridView1.Columns[9].Width = 150;
                    dataGridView1.Columns[10].HeaderText = "Fax";
                    dataGridView1.Columns[10].Width = 150;
                    dataGridView1.Columns[11].HeaderText = "Özel Notlar";
                    dataGridView1.Columns[11].Width = 200;

                    // Mevcut seçili satırın GSM numarasını sakla
                    string selectedGsm = "";
                    if (dataGridView1.SelectedRows.Count > 0)
                    {
                        selectedGsm = dataGridView1.SelectedRows[0].Cells["GsmTelefon"].Value?.ToString() ?? "";
                    }

                    // Textbox'ları güncelle (seçili satır yoksa ilk satırı seç)
                    if (dataGridView1.Rows.Count > 0)
                    {
                        int selectedIndex = -1;
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            if (dataGridView1.Rows[i].Cells["GsmTelefon"].Value?.ToString() == selectedGsm)
                            {
                                selectedIndex = i;
                                break;
                            }
                        }
                        if (selectedIndex >= 0)
                        {
                            dataGridView1.Rows[selectedIndex].Selected = true;
                            dataGridView1.CurrentCell = dataGridView1.Rows[selectedIndex].Cells[0];
                        }
                        else
                        {
                            dataGridView1.Rows[0].Selected = true;
                            dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
                            selectedIndex = 0;
                        }

                        // Textbox'ları güncelle
                        textBox3.Text = dataGridView1.Rows[selectedIndex].Cells["ToptanciAdi"].Value?.ToString() ?? "";
                        textBox4.Text = dataGridView1.Rows[selectedIndex].Cells["GsmTelefon"].Value?.ToString() ?? "";

                        // Toplam Borç parse işlemi: binlik ayırıcıları temizle ve ondalık noktaya çevir
                        string borcStr = dataGridView1.Rows[selectedIndex].Cells["ToplamBorc"].Value?.ToString() ?? "0";
                        borcStr = borcStr.Replace(".", "").Replace(",", "."); // 1.250 -> 1250.00
                        if (decimal.TryParse(borcStr, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal toplamBorc))
                        {
                            textBox5.Text = toplamBorc.ToString("N2");
                            textBox27.Text = toplamBorc.ToString("N2");
                        }
                        else
                        {
                            textBox5.Text = "0.00";
                            textBox27.Text = "0.00";
                        }

                        textBox28.Text = dataGridView1.Rows[selectedIndex].Cells["ToptanciAdi"].Value?.ToString() ?? "";
                        textBox22.Text = dataGridView1.Rows[selectedIndex].Cells["GsmTelefon"].Value?.ToString() ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı listelenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            // Mevcut temizleme kodları
            button5.Visible = false;
            textBox10.Text = string.Empty;
            textBox11.Text = string.Empty;
            textBox12.Text = string.Empty;
            textBox13.Text = string.Empty;
            textBox14.Text = string.Empty;
            textBox15.Text = string.Empty;
            textBox16.Text = string.Empty;
            textBox17.Text = string.Empty;
            textBox18.Text = string.Empty;
            textBox19.Text = string.Empty;
            textBox20.Text = string.Empty;
            textBox21.Visible = true;
            textBox21.Text = "";
            textBox21.ReadOnly = false;
            button10.Visible = true;
            button1.Visible = false;
            button3.Visible = false;
            panel1.Visible = false;
            panel2.Visible = false;
            button6.Visible = false;
            button4.Visible = false;
            dataGridView1.Visible = true;
            textBox1.Visible = true;
            label1.Visible = true;
            textBox1.Text = "";


            button8.Visible = false;
            // DataGridView'deki seçimi temizleme ve ilk satıra geçme
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[0].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
            }
        }

        private void ToplamBorcuGoster()
        {
            try
            {
                using (OleDbConnection baglan9 = new OleDbConnection(
                                "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan9.Open();
                    // Tüm toptancıların borçlarını tek tek çekiyoruz
                    string query = "SELECT ToplamBorc FROM Toptancilar";
                    OleDbCommand kmt = new OleDbCommand(query, baglan9);
                    decimal toplamBorc = 0;
                    using (OleDbDataReader reader = kmt.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Her bir borç değerini alıp string'den decimal'e dönüştürüyoruz
                            if (reader["ToplamBorc"] != DBNull.Value)
                            {
                                string borcMetni = reader["ToplamBorc"].ToString();
                                decimal borc;
                                // Virgül ve noktayı doğru bir şekilde ayırabilmek için InvariantCulture kullan
                                if (decimal.TryParse(borcMetni.Replace(",", "."),
                                                     System.Globalization.NumberStyles.Any,
                                                     System.Globalization.CultureInfo.InvariantCulture,
                                                     out borc))
                                {
                                    toplamBorc += borc;
                                }
                            }
                        }
                    }
                    // Toplam borcu para birimi formatıyla TextBox2'ye yaz
                    textBox2.Text = toplamBorc.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toplam borç hesaplanırken bir hata oluştu: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                // PANELDEKİ TEXTBOXLAR
                textBox3.Text = row.Cells[0].Value?.ToString(); // ToptanciAdi
                textBox4.Text = row.Cells[1].Value?.ToString(); // GsmTelefon
                textBox5.Text = row.Cells[2].Value?.ToString(); // ToplamBorc
                textBox28.Text = row.Cells[0].Value?.ToString(); // ToptanciAdi
                textBox22.Text = row.Cells[1].Value?.ToString(); // GsmTelefon
                textBox27.Text = row.Cells[2].Value?.ToString(); // ToplamBorc
                // ANA BÖLÜM TEXTBOXLAR
                textBox10.Text = row.Cells[0].Value?.ToString(); // ToptanciAdi
                textBox11.Text = row.Cells[3].Value?.ToString(); // SirketYetkilisi
                textBox12.Text = row.Cells[4].Value?.ToString(); // EMail
                textBox13.Text = row.Cells[5].Value?.ToString(); // InternetAdresi
                textBox14.Text = row.Cells[6].Value?.ToString(); // Vd
                textBox15.Text = row.Cells[7].Value?.ToString(); // Vn
                textBox16.Text = row.Cells[8].Value?.ToString(); // Adres
                textBox17.Text = row.Cells[9].Value?.ToString(); // IsTelefon
                textBox18.Text = row.Cells[1].Value?.ToString(); // GsmTelefon
                textBox19.Text = row.Cells[10].Value?.ToString(); // Fax
                textBox20.Text = row.Cells[11].Value?.ToString(); // OzelNotlar
                textBox21.Text = row.Cells[2].Value?.ToString(); // ToplamBorc
                button5.Visible = true;
                button6.Visible = true;
                // Buton ayarları
                button1.Visible = true;
                button10.Visible = false;
                button3.Visible = true;
                button4.Visible = true;
                textBox21.ReadOnly = true;

                button8.Visible = true;
            }
        }

        private void Toptanci_Load_1(object sender, EventArgs e)
        {
            // textBox7'yi anında güncel saatle doldur
            textBox7.Text = DateTime.Now.ToLongTimeString();
            // Diğer mevcut kodlarınız
            timer1.Enabled = true;
            textBox6.Text = DateTime.Now.ToShortDateString();
            textBox26.Text = DateTime.Now.ToShortDateString();
            checkBox1.Checked = true;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            // CheckBox'lara CheckedChanged event ekle (tek seçim için)
            checkBox1.CheckedChanged += CheckBox_CheckedChanged;
            checkBox2.CheckedChanged += CheckBox_CheckedChanged;
            checkBox3.CheckedChanged += CheckBox_CheckedChanged;
            ToplamBorcuGoster();
            Listele();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // DataGridView'den güncel bilgileri al
                string toptanciAdi = dataGridView1.SelectedRows[0].Cells["ToptanciAdi"].Value?.ToString() ?? "";
                string gsmTelefon = dataGridView1.SelectedRows[0].Cells["GsmTelefon"].Value?.ToString() ?? "";

                // TextBox'lardaki mevcut bilgileri al
                string textBoxAdi = textBox10.Text.Trim();
                string textBoxGsm = textBox18.Text.Trim();

                // Toptancı adı ve GSM numarasının değişip değişmediğini kontrol et
                bool adiDegisti = textBoxAdi != toptanciAdi;
                bool gsmDegisti = textBoxGsm != gsmTelefon;

                if (adiDegisti && gsmDegisti)
                {
                    MessageBox.Show("Toptancı adı ve GSM numarası değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgileri kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (adiDegisti)
                {
                    MessageBox.Show("Toptancı adı değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgiyi kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (gsmDegisti)
                {
                    MessageBox.Show("Toptancı GSM numarası değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgiyi kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // --- Eğer yukarıdaki kontrollerden geçilirse, mevcut kod buradan devam eder ---

                string secilenGsmTelefon = dataGridView1.SelectedRows[0].Cells["GsmTelefon"].Value.ToString();
                ToptancıİadeEdilecekÜrünler iadeForm = new ToptancıİadeEdilecekÜrünler(secilenGsmTelefon);
                iadeForm.Show();
                this.Close(); // Mevcut formu kapatır
            }
            else
            {
                MessageBox.Show("Lütfen önce bir toptancı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label24_Click(object sender, EventArgs e)
        {
        }

        private void label10_Click(object sender, EventArgs e)
        {
        }


        private void textBox24_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox23_TextChanged(object sender, EventArgs e)
        {
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            button15.Enabled = !checkBox4.Checked;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            // Seçilen toptancı bilgileri
            string secilenToptanciAdi = dataGridView1.CurrentRow.Cells["ToptanciAdi"].Value?.ToString();
            string secilenGsmTelefon = dataGridView1.CurrentRow.Cells["GsmTelefon"].Value?.ToString();

            // ToptancıHesapDetayi formunu oluştur ve bilgileri ata
            ToptanciHesapDetayi detayForm = new ToptanciHesapDetayi();
            detayForm.ToptanciAdi = secilenToptanciAdi;
            detayForm.GsmTelefon = secilenGsmTelefon;

            // Formu aç ve bu formu kapat
            detayForm.Show();
            this.Close();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox24.Text.Trim()))
            {
                MessageBox.Show("Lütfen ödeme tutarını girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string toptanciAdi = textBox28.Text.Trim();
            if (string.IsNullOrEmpty(toptanciAdi))
            {
                MessageBox.Show("Lütfen toptancı adını girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal toplamBorc = decimal.TryParse(textBox27.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal tb) ? tb : 0;
            decimal odenenTutar = decimal.TryParse(textBox24.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal ot) ? ot : 0;

            if (odenenTutar > toplamBorc)
            {
                MessageBox.Show("Ödenen tutar toplam borçtan büyük olamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // İşletme bilgilerini al
            string isletmeAdi = "", isletmeAdresi = "", isletmeYeri = "", gsmTelefon = "";
            using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
            {
                baglan.Open();
                string query = "SELECT IsletmeAdi, IsletmeAdresi, IsletmeYeri, GsmTelefon FROM IsletmeAdi";
                using (OleDbCommand cmd = new OleDbCommand(query, baglan))
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

            // Fontlar
            Font trFont = new Font("Arial", 8);
            Font trFontBold = new Font("Arial", 8, FontStyle.Bold);
            Font trFontItalic = new Font("Arial", 7, FontStyle.Italic);

            // Kağıt genişliği ve padding
            float pageWidth = 315;
            float padding = 5;

            // Kağıt yüksekliği tahmini
            float paperHeight = 0;
            paperHeight += 20;
            paperHeight += trFontBold.Height;
            paperHeight += trFont.Height * 3;
            paperHeight += 10;
            paperHeight += 10;
            paperHeight += trFont.Height * 2;
            paperHeight += 10;
            paperHeight += trFontBold.Height + trFont.Height * 5;
            if (!string.IsNullOrEmpty(textBox23.Text.Trim()))
            {
                using (var bmp = new Bitmap(1, 1))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    float aciklamaWidth = pageWidth - 2 * padding;
                    SizeF aciklamaSize = g.MeasureString("Açıklama: " + textBox23.Text.Trim(), trFont, (int)aciklamaWidth);
                    float lineHeight = trFont.Height;
                    int lineCount = (int)Math.Ceiling(aciklamaSize.Height / lineHeight);
                    paperHeight += Math.Max(20, lineCount * trFont.Height);
                }
            }
            paperHeight += 10;
            paperHeight += 10;
            paperHeight += trFontItalic.Height + 30;

            // PrintDocument
            PrintDocument pd = new PrintDocument();
            pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)pageWidth, (int)Math.Ceiling(paperHeight));
            pd.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

            pd.PrintPage += (snd, ev) =>
            {
                float y = 20;
                float contentWidth = pageWidth - 2 * padding;
                StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                StringFormat leftFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };

                // İşletme bilgileri
                ev.Graphics.DrawString(isletmeAdi, trFontBold, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontBold.Height), centerFormat);
                y += trFontBold.Height;
                ev.Graphics.DrawString(isletmeAdresi, trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), centerFormat);
                y += trFont.Height;
                ev.Graphics.DrawString(isletmeYeri, trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), centerFormat);
                y += trFont.Height;
                ev.Graphics.DrawString(gsmTelefon, trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), centerFormat);
                y += trFont.Height + 2;

                // Çizgi
                ev.Graphics.DrawLine(Pens.Black, padding, y, pageWidth - padding, y);
                y += 10;

                // Tarih ve saat
                string dateText = $"Tarih: {DateTime.Now:dd.MM.yyyy}";
                string timeText = $"Saat: {DateTime.Now:HH:mm:ss}";
                float halfWidth = contentWidth / 2;
                ev.Graphics.DrawString(dateText, trFont, Brushes.Black, new RectangleF(padding, y, halfWidth - 5, trFont.Height), leftFormat);
                ev.Graphics.DrawString(timeText, trFont, Brushes.Black, new RectangleF(padding + halfWidth - 25, y, halfWidth, trFont.Height), rightFormat);
                y += trFont.Height + 10;

                // Toptancı bilgileri
                ev.Graphics.DrawString("Toptancı Bilgileri", trFontBold, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontBold.Height), centerFormat);
                y += trFontBold.Height + padding;
                ev.Graphics.DrawString($"Toptancı Adı: {toptanciAdi}", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                y += trFont.Height + padding;
                ev.Graphics.DrawString($"Telefon: {textBox22.Text}", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                y += trFont.Height + padding;

                // Ödeme detayları
                ev.Graphics.DrawString("Ödeme Detayları", trFontBold, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontBold.Height), centerFormat);
                y += trFontBold.Height + padding;
                ev.Graphics.DrawString($"Toplam Borç: {toplamBorc:N2} TL", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                y += trFont.Height + padding;
                ev.Graphics.DrawString($"Ödenen Tutar: {odenenTutar:N2} TL", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                y += trFont.Height + padding;
                ev.Graphics.DrawString($"Kalan Borç: {Math.Max(0, toplamBorc - odenenTutar):N2} TL", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                y += trFont.Height + padding;

                // Açıklama
                if (!string.IsNullOrEmpty(textBox23.Text.Trim()))
                {
                    using (var bmp = new Bitmap(1, 1))
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        float aciklamaWidth = contentWidth;
                        SizeF aciklamaSize = g.MeasureString("Açıklama: " + textBox23.Text.Trim(), trFont, (int)aciklamaWidth);
                        int lineCount = (int)Math.Ceiling(aciklamaSize.Height / trFont.Height);
                        RectangleF rect = new RectangleF(padding, y, contentWidth, lineCount * trFont.Height);
                        ev.Graphics.DrawString("Açıklama: " + textBox23.Text.Trim(), trFont, Brushes.Black, rect, leftFormat);
                        y += lineCount * trFont.Height + padding;
                    }
                }

                // Çizgi ve alt bilgi
                ev.Graphics.DrawLine(Pens.Black, padding, y, pageWidth - padding, y);
                y += 10;
                ev.Graphics.DrawString("BİLGİ AMAÇLIDIR MALİ DEĞERİ YOKTUR", trFontItalic, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontItalic.Height), centerFormat);
                y += trFontItalic.Height + 30;
            };

            pd.Print();
            MessageBox.Show("Ödeme makbuzu başarıyla yazdırıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
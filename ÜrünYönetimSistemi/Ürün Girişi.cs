using ClosedXML.Excel;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ÜrünYönetimSistemi
{
    public partial class Ürün_Girişi : Form
    {
        public Form1 frm1;
        public Form2 frm2;
        private bool isCalculating = false;
        private string secilenGsmTelefon;

        private CancellationTokenSource cts = null;


        public Ürün_Girişi()
        {
            InitializeComponent();

            // Başlangıç ayarları
            panelGrupEkle.Visible = false;
            panelToptanciEkle.Visible = false;
            panel1.Visible = false;
            button14.Visible = false;
             panel4.Visible = false;
            textBox24.ReadOnly = true;
            button3.Visible = false;
            textBox24.BackColor = System.Drawing.Color.Gainsboro;
            this.KeyPreview = true;

            textBox1.KeyPress += Control_Enter_KeyPress;
            textBox2.KeyPress += Control_Enter_KeyPress;
            textBox3.KeyPress += Control_Enter_KeyPress;
            comboBox1.KeyPress += Control_Enter_KeyPress;
            comboBox2.KeyPress += Control_Enter_KeyPress;

            textBox8.KeyPress += Control_Enter_KeyPress;
            textBox4.KeyPress += Control_Enter_KeyPress;
            textBox6.KeyPress += Control_Enter_KeyPress;
            textBox7.KeyPress += Control_Enter_KeyPress;
            textBox23.KeyPress += Control_Enter_KeyPress;

            textBox10.KeyPress += Control_Enter_KeyPress;
            textBox18.KeyPress += Control_Enter_KeyPress;
            textBox11.KeyPress += Control_Enter_KeyPress;
            textBox12.KeyPress += Control_Enter_KeyPress;
            textBox13.KeyPress += Control_Enter_KeyPress;
            textBox14.KeyPress += Control_Enter_KeyPress;
            textBox15.KeyPress += Control_Enter_KeyPress;
            textBox16.KeyPress += Control_Enter_KeyPress;
            textBox17.KeyPress += Control_Enter_KeyPress;
            textBox19.KeyPress += Control_Enter_KeyPress;
            textBox20.KeyPress += Control_Enter_KeyPress;
            textBox21.KeyPress += Control_Enter_KeyPress;
            textBox26.KeyPress += Control_Enter_KeyPress;
            textBox25.KeyPress += Control_Enter_KeyPress;
            comboBox2.KeyPress += Control_Enter_KeyPress;
            comboBox3.KeyPress += Control_Enter_KeyPress;
            comboBox5.KeyPress += Control_Enter_KeyPress;




            textBox22.Visible = true;
            button15.Visible = false;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox5.DropDownStyle = ComboBoxStyle.DropDownList;
            checkBox1.Checked = true;
            dataGridView1.ReadOnly = true;
            textBox29.ReadOnly = true;
            textBox30.ReadOnly = true;
            textBox31.ReadOnly = true;
            textBox32.ReadOnly = true;
            textBox23.Visible = false;
            textBox24.Visible = false;
            button4.Visible = false;
            button13.Visible = false;
            textBox23.Text = "0";
            textBox26.Text = "0";
            textBox25.Text = "0";

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;


            dataGridView2.ReadOnly = true;

            // Hücre içine tıklayınca edit açılmasın
            dataGridView2.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.MultiSelect = false;
            dataGridView2.ClearSelection();

            // Hiçbir satır seçili olmasın
            dataGridView2.ClearSelection();
            dataGridView2.CurrentCell = null;

            textBox23.KeyPress += new KeyPressEventHandler(textBox23_KeyPress);
            // Metotları çağır
            LoadUrunGruplari();
            LoadToptancilar();
            AlisFiyatiToplaminiGoster();
            StokSayisiToplaminiGoster();
            SatisFiyatiToplaminiGoster();
            ListelenenToplamGoster();
            LoadUrunGruplari();

            Listele(); // Form yüklendiğinde datagridview'i doldur

            // Fiyat ve KDV alanları için KeyPress
            textBox1.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            AllowDecimal(textBox4);
            AllowDecimal(textBox5);
            AllowDecimal(textBox6);
            AllowDecimal(textBox25);
            AllowDecimal(textBox21);
            textBox7.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',')) e.Handled = true; };
            textBox8.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };

            textBox26.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };


            textBox27.ReadOnly = true;
            textBox28.ReadOnly = true;
            // Toptancı textbox'ları için KeyPress ve MaxLength
            textBox10.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)) e.Handled = true; };
            textBox11.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)) e.Handled = true; };
            textBox14.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)) e.Handled = true; };
            textBox15.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            textBox17.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            textBox18.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            textBox19.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            textBox21.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',')) e.Handled = true; };

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



            // TextChanged olaylarına metod bağlama
            textBox4.TextChanged += AlisFiyatiHesapla;
            textBox5.TextChanged += AlisFiyatiHesapla;
            textBox7.TextChanged += KdvOraniDegisti;
            panel2.Visible = false;
            dataGridView1.ClearSelection();

        }
        void AllowDecimal(TextBox txt)
        {
            txt.KeyPress += (s, e) =>
            {
                // Sadece rakam, kontrol tuşları ve virgül harici her şeyi engelle
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
                {
                    e.Handled = true;
                    return;
                }

                // İlk karakter virgül olamaz
                if (txt.SelectionStart == 0 && e.KeyChar == ',')
                {
                    e.Handled = true;
                    return;
                }

                // Virgül yalnızca bir kez kullanılabilir
                if (e.KeyChar == ',' && txt.Text.Contains(","))
                {
                    e.Handled = true;
                    return;
                }
            };
        }



        private void AlisFiyatiHesapla(object sender, EventArgs e)
        {
            if (isCalculating) return;

            try
            {
                // KDV oranını güvenli şekilde al
                string kdvOraniStr = textBox7.Text.Replace("%", "").Trim().Replace(",", ".");
                if (!decimal.TryParse(kdvOraniStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal kdvOrani))
                    return;

                if (kdvOrani < 0 || kdvOrani > 100)
                    return;

                kdvOrani /= 100; // yüzde değeri 0-1 aralığına indir

                // Değişen TextBox
                if (sender is not TextBox degisenTextBox) return;

                if (string.IsNullOrWhiteSpace(degisenTextBox.Text))
                {
                    // Eğer kutu boş bırakıldıysa diğerini de temizle
                    if (degisenTextBox == textBox4)
                        textBox5.Text = "";
                    else if (degisenTextBox == textBox5)
                        textBox4.Text = "";

                    return;
                }

                // Fiyat değerini al
                string fiyatStr = degisenTextBox.Text.Trim().Replace(",", ".");
                if (fiyatStr.Contains("."))
                {
                    // Son noktayı ondalık ayırıcı olarak kabul et, diğer noktaları sil
                    int lastDotIndex = fiyatStr.LastIndexOf('.');
                    fiyatStr = fiyatStr.Remove(lastDotIndex, 1);  // son noktayı çıkar
                    fiyatStr = fiyatStr.Replace(".", "");        // tüm noktaları temizle
                    fiyatStr = fiyatStr.Insert(lastDotIndex, "."); // son noktayı geri ekle
                }

                if (!decimal.TryParse(fiyatStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fiyat))
                    return;

                if (fiyat <= 0)
                    return;

                isCalculating = true;

                // KDV dahil → hariç
                if (checkBox1.Checked && degisenTextBox == textBox4)
                {
                    decimal kdvHaricFiyat = fiyat / (1 + kdvOrani);
                    textBox5.Text = kdvHaricFiyat.ToString("N2");
                }
                // KDV hariç → dahil
                else if (checkBox2.Checked && degisenTextBox == textBox5)
                {
                    decimal kdvDahilFiyat = fiyat * (1 + kdvOrani);
                    textBox4.Text = kdvDahilFiyat.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hesaplama sırasında bir hata oluştu:\n" + ex.Message,
                                "Hata",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                isCalculating = false;
            }
        }




        private void KdvOraniDegisti(object sender, EventArgs e)
        {
            if (checkBox1.Checked && !string.IsNullOrEmpty(textBox4.Text))
            {
                AlisFiyatiHesapla(textBox4, EventArgs.Empty);
            }
            else if (checkBox2.Checked && !string.IsNullOrEmpty(textBox5.Text))
            {
                AlisFiyatiHesapla(textBox5, EventArgs.Empty);
            }
        }
        private void Listele()
        {
            using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
            {
                DataTable tablo = new DataTable();
                tablo.Clear();
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM ÜrünGirişi", baglan);
                adapter.Fill(tablo);
                dataGridView1.DataSource = tablo;

                // Sütun başlıkları
                dataGridView1.Columns["Barkod_No"].HeaderText = "Barkod No";
                dataGridView1.Columns["Ürün_Adi"].HeaderText = "Ürün Adı";
                dataGridView1.Columns["Ürün_Kodu"].HeaderText = "Ürün Kodu";
                dataGridView1.Columns["Ürün_Grubu"].HeaderText = "Ürün Grubu";
                dataGridView1.Columns["Stok_Miktari"].HeaderText = "Stok Miktarı";
                dataGridView1.Columns["OlcuBirimi"].HeaderText = "Ölçü Birimi";
                dataGridView1.Columns["Satis_Fiyati"].HeaderText = "Satış Fiyatı";
                dataGridView1.Columns["AsgariStok"].HeaderText = "Asgari Stok";
                dataGridView1.Columns["2SatisFiyati"].HeaderText = "İndirimli Fiyat";
                dataGridView1.Columns["Alis_Fiyati"].HeaderText = "Kdv Dahil";
                dataGridView1.Columns["Alis_Fiyati2"].HeaderText = "Kdv Hariç";
                dataGridView1.Columns["KDV_Orani"].HeaderText = "KDV Oranı";
                dataGridView1.Columns["Toptanci_Adi"].HeaderText = "Toptancı Adı";
                dataGridView1.Columns["GsmTelefon"].HeaderText = "Gsm Telefon";
                dataGridView1.Columns["Tarih"].HeaderText = "Tarih";
                dataGridView1.Columns["Saat"].HeaderText = "Saat";
                if (dataGridView1.Columns.Contains("IslemTuru"))
                {
                    dataGridView1.Columns["IslemTuru"].HeaderText = "İşlem Türü";
                }

                // --- SAYISAL SÜTUNLARI BİÇİMLENDİRME ---
                // 'N2' formatı: binlik ayıracı ekler ve ondalık kısmı iki basamakla gösterir.
                if (dataGridView1.Columns.Contains("Satis_Fiyati"))
                {
                    dataGridView1.Columns["Satis_Fiyati"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridView1.Columns.Contains("Alis_Fiyati"))
                {
                    dataGridView1.Columns["Alis_Fiyati"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridView1.Columns.Contains("Alis_Fiyati2"))
                {
                    dataGridView1.Columns["Alis_Fiyati2"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridView1.Columns.Contains("2SatisFiyati"))
                {
                    dataGridView1.Columns["2SatisFiyati"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridView1.Columns.Contains("KDV_Orani"))
                {
                    // Yüzde formatı için "P2" (P = Percentage) kullanılabilir veya sayıyı 100 ile çarpıp N2 formatı uygulanabilir.
                    // Örnek: dataGridView1.Columns["KDV_Orani"].DefaultCellStyle.Format = "P2";
                    // Ancak veri tabanına 0,08 gibi kaydedildiği için N2 daha uygun.
                    dataGridView1.Columns["KDV_Orani"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridView1.Columns.Contains("Stok_Miktari"))
                {
                    // Stok miktarı tam sayı olabilir, bu yüzden "N0" tam sayı formatı veya "N2" ondalıklı format kullanılabilir.
                    // Kullanım durumunuza göre birini seçin.
                    dataGridView1.Columns["Stok_Miktari"].DefaultCellStyle.Format = "N2";
                }
                if (dataGridView1.Columns.Contains("AsgariStok"))
                {
                    dataGridView1.Columns["AsgariStok"].DefaultCellStyle.Format = "N2";
                }

                // Tarih ve saat sütunlarını biçimlendirme
                if (dataGridView1.Columns.Contains("Tarih"))
                {
                    dataGridView1.Columns["Tarih"].DefaultCellStyle.Format = "dd.MM.yyyy";
                }
                if (dataGridView1.Columns.Contains("Saat"))
                {
                    dataGridView1.Columns["Saat"].DefaultCellStyle.Format = "HH:mm";
                }
            }
            using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
            {
                dataGridView2.ClearSelection();
                dataGridView2.CurrentCell = null;
                DataTable tablo = new DataTable();
                tablo.Clear();
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM ÜrünGrupEkle", baglan);
                adapter.Fill(tablo);
                dataGridView2.DataSource = tablo;
                if (dataGridView2.Columns.Contains("GrupAdi"))
                {
                    dataGridView2.Columns["GrupAdi"].HeaderText = "Grup Adı";
                    dataGridView2.Columns["GrupAdi"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }
        private void Ürün_Girişi_Load(object sender, EventArgs e)
        {
            comboBox2.MaxDropDownItems = 10;

            // 3. (GEREKLİ EKLEME) Listeden ilk 10 öğenin yüksekliğini hesaplayarak 
            // açılır listeyi piksel cinsinden sınırlandırın.
            // Ortalama bir satır yüksekliği 15-18 pikseldir. 10 satır için 180 piksel deneyelim:

            int maxYukseklik = 10 * comboBox2.ItemHeight + 2; // 2, kenarlıklar için küçük bir boşluk

            // Eğer bu özelliği kullanan bir WinForms ortamındaysanız, bu satır işe yarayacaktır:
            comboBox2.DropDownHeight = maxYukseklik;


            comboBox1.MaxDropDownItems = 10;

            // 3. (GEREKLİ EKLEME) Listeden ilk 10 öğenin yüksekliğini hesaplayarak 
            // açılır listeyi piksel cinsinden sınırlandırın.
            // Ortalama bir satır yüksekliği 15-18 pikseldir. 10 satır için 180 piksel deneyelim:

            int maxYukseklik1 = 10 * comboBox1.ItemHeight + 2; // 2, kenarlıklar için küçük bir boşluk

            // Eğer bu özelliği kullanan bir WinForms ortamındaysanız, bu satır işe yarayacaktır:
            comboBox1.DropDownHeight = maxYukseklik;

            comboBox4.MaxDropDownItems = 10;

            // 3. (GEREKLİ EKLEME) Listeden ilk 10 öğenin yüksekliğini hesaplayarak 
            // açılır listeyi piksel cinsinden sınırlandırın.
            // Ortalama bir satır yüksekliği 15-18 pikseldir. 10 satır için 180 piksel deneyelim:

            int maxYukseklik2 = 10 * comboBox4.ItemHeight + 2; // 2, kenarlıklar için küçük bir boşluk

            // Eğer bu özelliği kullanan bir WinForms ortamındaysanız, bu satır işe yarayacaktır:
            comboBox4.DropDownHeight = maxYukseklik;


            textBox27.Text = DateTime.Now.ToShortDateString();
            this.KeyPreview = true;

            // comboBox2: Toptancı
            label33.Visible = false;
            progressBar1.Visible = false;
            button23.Visible = false;

            // comboBox3: Ödeme türü
            comboBox3.Items.Clear();
            comboBox3.Items.Add("Belirtmek istemiyorum");
            comboBox3.Items.Add("Kredi Kartı");
            comboBox3.Items.Add("Nakit");
            comboBox3.Items.Add("Çek");
            comboBox3.Items.Add("Veresiye");

            comboBox5.Items.Clear();
            comboBox5.Items.Add("Adet");
            comboBox5.Items.Add("Kg");
            comboBox5.Items.Add("Lt");
            comboBox5.Items.Add("Paket");
            comboBox5.Items.Add("Koli");


            // Varsayılan olarak ilk öğeyi (Adet) seçili hale getirme
            comboBox5.SelectedIndex = 0;
            // comboBox4: Ürün grubu
            comboBox4.Items.Clear();
            comboBox4.Items.Add("Tümü");

            // Veritabanı bağlantısı ile ürün gruplarını doldurma
            try
            {
                using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();
                    string query = "SELECT DISTINCT Ürün_Grubu FROM ÜrünGirişi";
                    OleDbCommand kmt = new OleDbCommand(query, baglan);
                    OleDbDataReader dr = kmt.ExecuteReader();
                    while (dr.Read())
                    {
                        comboBox4.Items.Add(dr["Ürün_Grubu"].ToString());
                    }
                    dr.Close();
                }
                comboBox4.SelectedIndex = 0; // Varsayılan olarak "Tümü" seçili olsun
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün grupları yüklenirken hata oluştu: " + ex.Message);
            }

            // KDv ayarları
            checkBox1.Checked = GlobalAyarlar.KdvDahilGoster;
            checkBox2.Checked = !GlobalAyarlar.KdvDahilGoster;
            textBox4.Enabled = GlobalAyarlar.KdvDahilGoster;
            textBox5.Enabled = !GlobalAyarlar.KdvDahilGoster;

            // Checkbox olay işleyicilerini bağla
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            comboBox3.SelectedIndex = 0; // Açılışta ilk görünen

            // Form açıldığında toplam verileri yeniden hesapla ve göster
            SatisFiyatiToplaminiGoster();
            AlisFiyatiToplaminiGoster();
            ListelenenToplamGoster();
            StokSayisiToplaminiGoster();

            Listele();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }


        private void label7_Click(object sender, EventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void textBox9_TextChanged(object sender, EventArgs e) { }


        private void button5_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            int rastgeleSayi = rnd.Next(1, 1000000);
            string rastgeleSayiMetni = rastgeleSayi.ToString().PadLeft(6, '0');
            textBox1.Text = rastgeleSayiMetni;
            button3.Visible = true;
            textBox26.Text = "0";
            textBox25.Text = "0";
            button14.Visible= false;
            button9.Visible = true;

            comboBox2.SelectedIndex = 0;  // İlk seçenek varsayılan
            comboBox3.SelectedIndex = 0;  // İlk seçenek varsayılan
            comboBox1.SelectedIndex = 0;  // İlk seçenek varsayılan

            textBox2.Focus(); // imleci textBox2'ye taşı
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox26.Text = "0";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox23.Text = "";
            textBox25.Text = "0";
            comboBox1.SelectedIndex = 0;
            textBox8.ReadOnly = false;
            comboBox2.SelectedIndex = 0;  // İlk seçenek varsayılan
            comboBox3.SelectedIndex = 0;  // İlk seçenek varsayılan
            comboBox5.SelectedIndex = 0;
            button14.Visible = false;
            button15.Visible = false;
            button1.Visible = true;
            button4.Visible = false;
            button13.Visible = false;
            textBox23.Visible = false;
            textBox24.Visible = false;
            button9.Visible = true;
            button6.Visible = true;
            button3.Visible = false; checkBox1.Checked = true;
        }


        public static class GlobalAyarlar
        {
            public static bool KdvDahilGoster { get; set; } = true; // Varsayılan olarak KDV Dahil seçili olsun.
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (isCalculating) return;

            if (checkBox1.Checked)
            {
                if (checkBox2.Checked) checkBox2.Checked = false;
                textBox4.Enabled = true;
                textBox5.Enabled = false;
                textBox4.Clear();
                GlobalAyarlar.KdvDahilGoster = true; // KDV Dahil seçildi
            }
            else
            {
                if (!checkBox2.Checked)
                {
                    checkBox2.Checked = true;
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (isCalculating) return;

            if (checkBox2.Checked)
            {
                if (checkBox1.Checked) checkBox1.Checked = false;
                textBox5.Enabled = true;
                textBox4.Enabled = false;
                textBox5.Clear();
                GlobalAyarlar.KdvDahilGoster = false; // KDV Hariç seçildi
            }
            else
            {
                if (!checkBox1.Checked)
                {
                    checkBox1.Checked = true;
                }
            }
        }
        private bool TryParseDecimalFromObject(object dbValue, out decimal result)
        {
            result = 0m;

            if (dbValue == null || dbValue == DBNull.Value)
            {
                result = 0m;
                return true;
            }

            // Eğer zaten sayısal tipse direkt çevir
            if (dbValue is decimal dec) { result = dec; return true; }
            if (dbValue is double dbl) { result = Convert.ToDecimal(dbl); return true; }
            if (dbValue is float fl) { result = Convert.ToDecimal(fl); return true; }
            if (dbValue is int i) { result = i; return true; }
            if (dbValue is long l) { result = l; return true; }
            if (dbValue is short sh) { result = sh; return true; }

            // String olarak geliyorsa normalize et
            string s = dbValue.ToString().Trim();

            if (string.IsNullOrEmpty(s))
            {
                result = 0m;
                return true;
            }

            // Boşlukları temizle
            s = s.Replace(" ", "");

            int lastDot = s.LastIndexOf('.');
            int lastComma = s.LastIndexOf(',');

            if (lastDot >= 0 && lastComma >= 0)
            {
                // Hem nokta hem virgül varsa, son görünen işareti ondalık ayırıcı say
                if (lastDot > lastComma)
                {
                    // nokta ondalık -> virgülleri binlik olarak kaldır
                    s = s.Replace(",", "");
                    // nokta olduğu için invariant formata uygundur (nokta decimal)
                }
                else
                {
                    // virgül ondalık -> noktaları binlik olarak kaldır, virgülü nokta yap
                    s = s.Replace(".", "");
                    s = s.Replace(",", ".");
                }
            }
            else if (lastComma >= 0)
            {
                int digitsAfter = s.Length - lastComma - 1;
                if (digitsAfter == 3)
                {
                    // tek virgül ve 3 hane sonrası varsa muhtemelen binlik ayırıcı -> kaldır
                    s = s.Replace(",", "");
                }
                else
                {
                    // decimal ayırıcı gibi davran, virgülü noktaya çevir
                    s = s.Replace(",", ".");
                }
            }
            else if (lastDot >= 0)
            {
                int digitsAfter = s.Length - lastDot - 1;
                if (digitsAfter == 3)
                {
                    // tek nokta ve 3 hane sonrası -> binlik ayırıcı
                    s = s.Replace(".", "");
                }
                else
                {
                    // nokta ondalık ayırıcı olarak kalır (InvariantCulture ile parse edilecek)
                }
            }

            // Şimdi s, invariant kültürde "." ile ondalık ayracı olacak şekilde normalleşmiş olmalı
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return true;

            // En son çare olarak current culture ile deneyelim
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
                return true;

            // Parse edilemedi -> 0 döndür
            result = 0m;
            return false;
        }

        private void SatisFiyatiToplaminiGoster()
        {
            try
            {
                using (OleDbConnection baglan = new OleDbConnection(
                                 "Provider=Microsoft.ACE.OleDb.12.0;Data Source=" +
                                 Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();

                    string query = "SELECT Satis_Fiyati, Miktar, Stok_Miktari FROM [ÜrünGirişi]";
                    using (OleDbCommand cmd = new OleDbCommand(query, baglan))
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        decimal toplam = 0m;

                        while (reader.Read())
                        {
                            TryParseDecimalFromObject(reader["Satis_Fiyati"], out decimal satisFiyati);
                            TryParseDecimalFromObject(reader["Miktar"], out decimal miktar);
                            TryParseDecimalFromObject(reader["Stok_Miktari"], out decimal stokMiktari);

                            decimal kullanilacakMiktar = (miktar > 0m) ? miktar : stokMiktari;
                            toplam += satisFiyati * kullanilacakMiktar;
                        }

                        textBox30.Text = toplam.ToString("N2", CultureInfo.CurrentCulture);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toplam satış fiyatı hesaplanırken hata oluştu: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AlisFiyatiToplaminiGoster()
        {
            try
            {
                using (OleDbConnection baglan = new OleDbConnection(
                                 "Provider=Microsoft.ACE.OleDb.12.0;Data Source=" +
                                 Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();

                    string query = "SELECT Alis_Fiyati, Miktar, Stok_Miktari FROM [ÜrünGirişi]";
                    using (OleDbCommand cmd = new OleDbCommand(query, baglan))
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        decimal toplam = 0m;

                        while (reader.Read())
                        {
                            TryParseDecimalFromObject(reader["Alis_Fiyati"], out decimal alisFiyati);
                            TryParseDecimalFromObject(reader["Miktar"], out decimal miktar);
                            TryParseDecimalFromObject(reader["Stok_Miktari"], out decimal stokMiktari);

                            decimal kullanilacakMiktar = (miktar > 0m) ? miktar : stokMiktari;
                            toplam += alisFiyati * kullanilacakMiktar;
                        }

                        textBox29.Text = toplam.ToString("N2", CultureInfo.CurrentCulture);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toplam alış fiyatı hesaplanırken hata oluştu: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StokSayisiToplaminiGoster()
        {
            try
            {
                using (OleDbConnection baglan = new OleDbConnection(
                            "Provider=Microsoft.ACE.OleDb.12.0;Data Source=" +
                            Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();

                    string query = "SELECT Miktar, Stok_Miktari FROM ÜrünGirişi";
                    using (OleDbCommand cmd = new OleDbCommand(query, baglan))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            decimal toplamMiktar = 0;

                            while (reader.Read())
                            {
                                decimal miktar = reader["Miktar"] != DBNull.Value
                                    ? decimal.Parse(reader["Miktar"].ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture)
                                    : 0;

                                decimal stokMiktari = reader["Stok_Miktari"] != DBNull.Value
                                    ? decimal.Parse(reader["Stok_Miktari"].ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture)
                                    : 0;

                                decimal kullanilacakMiktar = (miktar > 0) ? miktar : stokMiktari;

                                toplamMiktar += kullanilacakMiktar;
                            }

                            textBox31.Text = toplamMiktar.ToString("N0");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toplam miktar hesaplanırken hata oluştu: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListelenenToplamGoster()
        {
            try
            {
                using (OleDbConnection baglan = new OleDbConnection(
                            "Provider=Microsoft.ACE.OleDb.12.0;Data Source=" +
                            Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();

                    string query = "SELECT COUNT(*) FROM ÜrünGirişi";
                    using (OleDbCommand cmd = new OleDbCommand(query, baglan))
                    {
                        object result = cmd.ExecuteScalar();
                        int toplamUrun = (result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                        textBox32.Text = toplamUrun.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toplam ürün sayısı alınırken hata oluştu: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1️⃣ Alan kontrolleri ve uyarıları biriktir
            var uyariMesajlari = new System.Text.StringBuilder();
            if (string.IsNullOrWhiteSpace(textBox1.Text)) uyariMesajlari.AppendLine("- Barkod numarası boş bırakılamaz. Lütfen bir barkod numarası girin.");
            if (string.IsNullOrWhiteSpace(textBox2.Text)) uyariMesajlari.AppendLine("- Ürün adı boş bırakılamaz. Lütfen bir ürün adı girin.");
            if (string.IsNullOrWhiteSpace(textBox3.Text)) uyariMesajlari.AppendLine("- Ürün kodu boş bırakılamaz. Lütfen bir ürün kodu girin.");
            if (comboBox1.SelectedIndex == -1) uyariMesajlari.AppendLine("- Lütfen bir ürün grubu seçin.");
            if (string.IsNullOrWhiteSpace(textBox6.Text)) uyariMesajlari.AppendLine("- Satış fiyatı boş bırakılamaz. Lütfen bir satış fiyatı girin.");
            if (string.IsNullOrWhiteSpace(textBox8.Text)) uyariMesajlari.AppendLine("- Stok miktarı boş bırakılamaz. Lütfen bir stok miktarı girin.");
            if (uyariMesajlari.Length > 0)
            {
                MessageBox.Show("Aşağıdaki hataları düzeltin:\n" + uyariMesajlari.ToString(), "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2️⃣ Sayısal değerlerin ayrıştırılması ve kontrolü
            decimal stokMiktari;
            if (!decimal.TryParse(textBox8.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out stokMiktari) || stokMiktari <= 0)
            {
                MessageBox.Show("Stok miktarı geçerli bir sayı olmalı ve sıfırdan büyük olmalı. Lütfen doğru bir değer girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            decimal alisFiyati1 = 0; // KDV Dahil
            decimal alisFiyati2 = 0; // KDV Hariç
            decimal satisFiyati;
            decimal kdvOrani = 0; // KDV oranını direkt girilen değer olarak alacağız
                                  // KDV parse
            if (!decimal.TryParse(textBox7.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out kdvOrani))
            {
                MessageBox.Show("KDV oranı geçerli bir sayı olmalı. Lütfen geçerli bir KDV oranı girin (örneğin: 6, 18).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Hesaplamalarda kullanılacak versiyon
            decimal kdvHesaplama = kdvOrani / 100; // Yüzde olarak hesaplanacak

            // ✅ Satış Fiyatı kontrolü ve uyarı
            var satisUyariMesajlari = new System.Text.StringBuilder();
            string satisFiyatiStr = textBox6.Text.Trim();
            if (satisFiyatiStr.Contains(",") && satisFiyatiStr.Split(',')[1].Length > 2)
            {
                string dogruFormat = satisFiyatiStr.Replace(",", "");
                satisUyariMesajlari.AppendLine("- Satış fiyatı için virgül (,) binlik ayırıcı olarak kullanılamaz. Lütfen doğru formatta girin (örneğin: " + dogruFormat + " veya " + dogruFormat + ",00 TL).");
            }
            if (!decimal.TryParse(satisFiyatiStr.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out satisFiyati))
            {
                satisUyariMesajlari.AppendLine("- Satış fiyatı geçerli bir sayı olmalı. Lütfen geçerli bir değer girin.");
            }
            if (satisUyariMesajlari.Length > 0)
            {
                MessageBox.Show("Aşağıdaki hataları düzeltin:\n" + satisUyariMesajlari.ToString(), "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal ikinciSatisFiyati = 0;
            if (!string.IsNullOrWhiteSpace(textBox25.Text))
            {
                var ikinciSatisUyariMesajlari = new System.Text.StringBuilder();
                string ikinciSatisFiyatiStr = textBox25.Text.Trim();
                if (ikinciSatisFiyatiStr.Contains(",") && ikinciSatisFiyatiStr.Split(',')[1].Length > 2)
                {
                    string dogruFormat = ikinciSatisFiyatiStr.Replace(",", "");
                    ikinciSatisUyariMesajlari.AppendLine("- 2. satış fiyatı için virgül (,) binlik ayırıcı olarak kullanılamaz. Lütfen doğru formatta girin (örneğin: " + dogruFormat + " veya " + dogruFormat + ",00 TL).");
                }
                if (!decimal.TryParse(ikinciSatisFiyatiStr.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out ikinciSatisFiyati))
                {
                    ikinciSatisUyariMesajlari.AppendLine("- 2. satış fiyatı geçerli bir sayı olmalı. Lütfen geçerli bir değer girin.");
                }
                else if (ikinciSatisFiyati >= satisFiyati)
                {
                    ikinciSatisUyariMesajlari.AppendLine("- 2. satış fiyatı, satış fiyatından yüksek veya eşit olamaz. Lütfen daha düşük bir değer girin.");
                }
                if (ikinciSatisUyariMesajlari.Length > 0)
                {
                    MessageBox.Show("Aşağıdaki hataları düzeltin:\n" + ikinciSatisUyariMesajlari.ToString(), "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Alış fiyatı hesaplaması
            if (checkBox1.Checked) // KDV Dahil
            {
                string alisFiyati1Str = textBox4.Text.Trim();
                if (!decimal.TryParse(alisFiyati1Str.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out alisFiyati1))
                {
                    MessageBox.Show("Alış fiyatı (KDV Dahil) geçerli bir sayı olmalı. Lütfen geçerli bir değer girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                alisFiyati2 = Math.Round(alisFiyati1 / (1 + kdvHesaplama), 2);
                GlobalAyarlar.KdvDahilGoster = true;
            }
            else if (checkBox2.Checked) // KDV Hariç
            {
                string alisFiyati2Str = textBox5.Text.Trim();
                if (!decimal.TryParse(alisFiyati2Str.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out alisFiyati2))
                {
                    MessageBox.Show("Alış fiyatı (KDV Hariç) geçerli bir sayı olmalı. Lütfen geçerli bir değer girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                alisFiyati1 = Math.Round(alisFiyati2 * (1 + kdvHesaplama), 2);
                GlobalAyarlar.KdvDahilGoster = false;
            }
            else
            {
                MessageBox.Show("Lütfen bir alış fiyatı tipi seçin (KDV Dahil veya Hariç).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tarih ve saat kontrolü
            if (string.IsNullOrWhiteSpace(textBox27.Text) || string.IsNullOrWhiteSpace(textBox28.Text))
            {
                MessageBox.Show("Tarih ve saat boş bırakılamaz. Lütfen her iki alanı da doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Asgari stok kontrolü
            decimal asgariStok = 0;
            if (!string.IsNullOrWhiteSpace(textBox26.Text))
            {
                if (!decimal.TryParse(textBox26.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out asgariStok))
                {
                    MessageBox.Show("Asgari stok geçerli bir sayı olmalı. Lütfen geçerli bir değer girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (asgariStok > stokMiktari)
                {
                    MessageBox.Show("Asgari stok sayısı stok sayısından yüksek olamaz. Lütfen stok miktarını kontrol edin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            try
            {
                using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();
                    // Barkod kontrolü
                    string kontrolQuery = "SELECT COUNT(*) FROM ÜrünGirişi WHERE Barkod_No=@BarkodNo";
                    using (OleDbCommand kontrolKmt = new OleDbCommand(kontrolQuery, baglan))
                    {
                        kontrolKmt.Parameters.AddWithValue("@BarkodNo", textBox1.Text);
                        int mevcutKayitSayisi = (int)kontrolKmt.ExecuteScalar();
                        if (mevcutKayitSayisi > 0)
                        {
                            MessageBox.Show("Bu barkod numarasına sahip bir ürün zaten eklenmiş. Lütfen farklı bir barkod numarası kullanın.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    // Ürün ekleme
                    string insertUrunQuery = "INSERT INTO ÜrünGirişi (Barkod_No, Ürün_Adi, Ürün_Kodu, Ürün_Grubu, Alis_Fiyati, Alis_Fiyati2, Satis_Fiyati, Stok_Miktari, Miktar, Toptanci_Adi, GsmTelefon, Tarih, Saat, IslemTuru, KDV_Orani, AsgariStok, 2SatisFiyati, OlcuBirimi) " +
                                             "VALUES (@BarkodNo, @UrunAdi, @UrunKodu, @UrunGrubu, @AlisFiyati, @AlisFiyati2, @SatisFiyati, @Stok_Miktari, @Miktar, @ToptanciAdi, @GsmTelefon, @Tarih, @Saat, @IslemTuru, @KDVOrani, @AsgariStok, @IkinciSatisFiyati, @OlcuBirimi)";
                    using (OleDbCommand insertUrunKmt = new OleDbCommand(insertUrunQuery, baglan))
                    {
                        insertUrunKmt.Parameters.AddWithValue("@BarkodNo", textBox1.Text);
                        insertUrunKmt.Parameters.AddWithValue("@UrunAdi", textBox2.Text);
                        insertUrunKmt.Parameters.AddWithValue("@UrunKodu", textBox3.Text);
                        insertUrunKmt.Parameters.AddWithValue("@UrunGrubu", comboBox1.Text);
                        insertUrunKmt.Parameters.AddWithValue("@AlisFiyati", alisFiyati1.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        insertUrunKmt.Parameters.AddWithValue("@Alis_Fiyati2", alisFiyati2.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        insertUrunKmt.Parameters.AddWithValue("@SatisFiyati", satisFiyati.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        insertUrunKmt.Parameters.AddWithValue("@StokMiktari", stokMiktari);
                        insertUrunKmt.Parameters.AddWithValue("@Miktar", stokMiktari);
                        insertUrunKmt.Parameters.AddWithValue("@ToptanciAdi", comboBox2.Text);
                        insertUrunKmt.Parameters.AddWithValue("@GsmTelefon", string.IsNullOrWhiteSpace(secilenGsmTelefon) ? DBNull.Value : (object)secilenGsmTelefon);
                        insertUrunKmt.Parameters.AddWithValue("@Tarih", DateTime.Parse(textBox27.Text));
                        insertUrunKmt.Parameters.AddWithValue("@Saat", DateTime.Parse(textBox28.Text).ToShortTimeString());
                        insertUrunKmt.Parameters.AddWithValue("@IslemTuru", comboBox3.Text);
                        // 🔹 KDV doğru kaydediliyor (6 girerse 6 olur)
                        insertUrunKmt.Parameters.AddWithValue("@KDVOrani", kdvOrani.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        insertUrunKmt.Parameters.AddWithValue("@AsgariStok", string.IsNullOrWhiteSpace(textBox26.Text) ? DBNull.Value : (object)asgariStok.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        insertUrunKmt.Parameters.AddWithValue("@IkinciSatisFiyati", string.IsNullOrWhiteSpace(textBox25.Text) ? DBNull.Value : (object)ikinciSatisFiyati.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        insertUrunKmt.Parameters.AddWithValue("@OlcuBirimi", comboBox5.Text);
                        insertUrunKmt.ExecuteNonQuery();
                    }
                    // 5️⃣ Toptancı borcunu güncelleme
                    if (comboBox2.Text != "Toptancı Adını Belirtmek istemiyorum" && !string.IsNullOrWhiteSpace(secilenGsmTelefon))
                    {
                        decimal eklenecekBorc = GlobalAyarlar.KdvDahilGoster ? alisFiyati1 * stokMiktari : alisFiyati2 * stokMiktari;
                        decimal mevcutBorc = 0;
                        string selectBorcQuery = "SELECT ToplamBorc FROM Toptancilar WHERE GsmTelefon=@GsmTelefon";
                        using (OleDbCommand cmdSelect = new OleDbCommand(selectBorcQuery, baglan))
                        {
                            cmdSelect.Parameters.AddWithValue("@GsmTelefon", secilenGsmTelefon);
                            object result = cmdSelect.ExecuteScalar();
                            if (result != DBNull.Value && result != null)
                            {
                                mevcutBorc = decimal.Parse(result.ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        string updateToptanciQuery = "UPDATE Toptancilar SET ToplamBorc=@ToplamBorc WHERE GsmTelefon=@GsmTelefon";
                        using (OleDbCommand updateToptanciKmt = new OleDbCommand(updateToptanciQuery, baglan))
                        {
                            updateToptanciKmt.Parameters.AddWithValue("@ToplamBorc", (mevcutBorc + eklenecekBorc).ToString(System.Globalization.CultureInfo.InvariantCulture));
                            updateToptanciKmt.Parameters.AddWithValue("@GsmTelefon", secilenGsmTelefon);
                            updateToptanciKmt.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Ürün başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    button3.Visible = false;
                    comboBox5.SelectedIndex = 0;
                    checkBox1.Checked = true;
                    // Listeleme ve UI güncelleme
                    button3_Click(sender, e);
                    StokSayisiToplaminiGoster();
                    AlisFiyatiToplaminiGoster();
                    SatisFiyatiToplaminiGoster();
                    ListelenenToplamGoster();
                    Listele();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            panelGrupEkle.Visible = true;


            textBox9.Text = string.Empty;
            textBox9.Focus();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox9.Text))
            {
                MessageBox.Show("Lütfen bir ürün grubu adı giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();
                    string query = "INSERT INTO ÜrünGrupEkle (GrupAdi) VALUES (@GrupAdi)";
                    OleDbCommand kmt = new OleDbCommand(query, baglan);
                    kmt.Parameters.AddWithValue("@GrupAdi", textBox9.Text);
                    int kayitSayisi = kmt.ExecuteNonQuery();

                    if (kayitSayisi > 0)
                    {
                        MessageBox.Show("Yeni ürün grubu başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUrunGruplari();
                        Listele();
                        textBox9.Text = string.Empty;
                        panelGrupEkle.Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("Ürün grubu eklenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantı veya kayıt hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            panelGrupEkle.Visible = false;

        }

        private void LoadUrunGruplari()
        {
            comboBox1.Items.Clear();

            try
            {
                // Özel seçenek başa ekle
                comboBox1.Items.Add("Belirtmek istemiyorum");

                using (OleDbConnection baglan = new OleDbConnection(
                    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();
                    OleDbCommand kmt = new OleDbCommand("SELECT GrupAdi FROM ÜrünGrupEkle", baglan);
                    OleDbDataReader okuyucu = kmt.ExecuteReader();
                    while (okuyucu.Read())
                    {
                        string grupAdi = okuyucu["GrupAdi"].ToString();
                        comboBox1.Items.Add(grupAdi);
                    }
                    okuyucu.Close();
                }

                // Varsayılan olarak seç
                comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün grupları yüklenirken bir hata oluştu: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadToptancilar()
        {
            comboBox2.Items.Clear();

            try
            {
                // Özel seçenek başa ekle
                comboBox2.Items.Add("Toptancı Adını Belirtmek istemiyorum");

                using (OleDbConnection baglan = new OleDbConnection(
                    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();
                    OleDbCommand kmt = new OleDbCommand("SELECT ToptanciAdi FROM Toptancilar", baglan);
                    OleDbDataReader okuyucu = kmt.ExecuteReader();
                    while (okuyucu.Read())
                    {
                        string toptanciAdi = okuyucu["ToptanciAdi"].ToString();
                        comboBox2.Items.Add(toptanciAdi);
                    }
                    okuyucu.Close();
                }

                // Varsayılan olarak seç
                comboBox2.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toptancılar yüklenirken bir hata oluştu: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            panelToptanciEkle.Visible = true;
            panelGrupEkle.Visible = false; // Diğer paneli gizle
            dataGridView1.Visible = false; // DataGridView'i gizle
            label5.Visible = false;
            panel3.Visible = false;

            textBox22.Visible = false;

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

            textBox10.Focus();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            panelToptanciEkle.Visible = false;
            dataGridView1.Visible = true;
            textBox22.Visible = true;
            label5.Visible = true;
            panel3.Visible = true;

        }

        private void button12_Click(object sender, EventArgs e)
        {
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
        }

        private void button10_Click(object sender, EventArgs e)
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

            // ✅ Toplam Borç kontrolü ve format uyarısı
            decimal toplamBorc = 0;
            if (!string.IsNullOrWhiteSpace(textBox21.Text))
            {
                string toplamBorcStr = textBox21.Text.Trim();
                if (toplamBorcStr.Contains(",") && toplamBorcStr.Split(',')[1].Length > 2)
                {
                    string dogruFormat = toplamBorcStr.Replace(",", "");
                    errors.Add($"Lütfen Toplam Borcu {dogruFormat} veya {dogruFormat},00 TL olarak girin. Virgül (,) binlik ayıracı olarak kullanılamaz.");
                }
                else if (!decimal.TryParse(toplamBorcStr.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out toplamBorc))
                {
                    errors.Add("Toptancı Borcu geçerli bir sayısal değer olmalıdır.");
                }
            }

            if (errors.Any())
            {
                string errorMessage = "Lütfen aşağıdaki hataları düzeltin:\n\n" + string.Join("\n", errors);
                MessageBox.Show(errorMessage, "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();

                    // Sadece GSM benzersiz olacak şekilde kontrol
                    string gsmQuery = "SELECT COUNT(*) FROM Toptancilar WHERE GsmTelefon = @gsmTelefonu";
                    OleDbCommand gsmKmt = new OleDbCommand(gsmQuery, baglan);
                    gsmKmt.Parameters.AddWithValue("@gsmTelefonu", textBox18.Text);
                    int mevcutGsm = (int)gsmKmt.ExecuteScalar();

                    if (mevcutGsm > 0)
                    {
                        MessageBox.Show("Bu GSM numarasına sahip bir toptancı zaten kayıtlı. Farklı bir numara giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Kayıt ekleme
                    string insertQuery = "INSERT INTO Toptancilar (ToptanciAdi, SirketYetkilisi, EMail, InternetAdresi, Vd, Vn, Adres, IsTelefon, GsmTelefon, Fax, OzelNotlar, ToplamBorc) " +
                                         "VALUES (@toptanciAdi, @yetkiliAdi, @eMail, @internetAdresi, @vd, @vn, @adres, @isTelefonu, @gsmTelefonu, @fax, @ozelNotlar, @toplamBorc)";
                    OleDbCommand insertKmt = new OleDbCommand(insertQuery, baglan);

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
                        button12_Click(sender, e);
                        panelToptanciEkle.Visible = false;
                        LoadToptancilar();
                        dataGridView1.Visible = true;
                        panel3.Visible = true;
                        textBox22.Visible = true;
                        comboBox2.SelectedIndex = 0; // Toptancı varsayılan olarak "Belirtmek istemiyorum"
                        comboBox3.SelectedIndex = 0; // IslemTuru varsayılan olarak "Belirtmek istemiyorum"
                        label5.Visible = true;

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
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            button1.Visible = false;
            button15.Visible = true;
            button4.Visible = true;

            textBox23.Visible = true;
            textBox24.Visible = true;
            textBox25.Visible = true;
            textBox26.Visible = true;
            textBox8.ReadOnly = true;
            button9.Visible = false;
            button14.Visible = true;
            textBox23.Text = "0";
            button3.Visible = true;

            // Değerleri doğrudan atamak yerine TryParse ile dönüştürerek ondalık ayracı sorununu giderdik.

            // Alış Fiyatı (KDV Dahil)
            if (decimal.TryParse(row.Cells["Alis_Fiyati"]?.Value?.ToString().Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal alisFiyati))
                textBox4.Text = alisFiyati.ToString(System.Globalization.CultureInfo.CurrentCulture);
            else
                textBox4.Text = "";

            // Alış Fiyatı (KDV Hariç)
            if (decimal.TryParse(row.Cells["Alis_Fiyati2"]?.Value?.ToString().Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal alisFiyati2))
                textBox5.Text = alisFiyati2.ToString(System.Globalization.CultureInfo.CurrentCulture);
            else
                textBox5.Text = "";

            // Satış Fiyatı
            if (decimal.TryParse(row.Cells["Satis_Fiyati"]?.Value?.ToString().Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal satisFiyati))
                textBox6.Text = satisFiyati.ToString(System.Globalization.CultureInfo.CurrentCulture);
            else
                textBox6.Text = "";

            // KDV Oranı
            if (decimal.TryParse(row.Cells["KDV_Orani"]?.Value?.ToString().Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal kdvOrani))
                textBox7.Text = kdvOrani.ToString(System.Globalization.CultureInfo.CurrentCulture);
            else
                textBox7.Text = "";

            // Stok Miktarı
            if (decimal.TryParse(row.Cells["Stok_Miktari"]?.Value?.ToString().Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal stokMiktari))
                textBox8.Text = stokMiktari.ToString(System.Globalization.CultureInfo.CurrentCulture);
            else
                textBox8.Text = "";

            // Metin tabanlı alanlar için mevcut kodlar yeterlidir.
            textBox1.Text = row.Cells["Barkod_No"].Value?.ToString() ?? "";
            textBox2.Text = row.Cells["Ürün_Adi"].Value?.ToString() ?? "";
            textBox3.Text = row.Cells["Ürün_Kodu"].Value?.ToString() ?? "";


            // ---------------- ComboBox1 (Ürün Grubu) ----------------
            string urunGrubu = row.Cells["Ürün_Grubu"].Value?.ToString() ?? "Belirtmek istemiyorum";
            if (!comboBox1.Items.Contains(urunGrubu))
                comboBox1.Items.Add(urunGrubu);
            comboBox1.SelectedItem = urunGrubu;

            // ---------------- ComboBox2 (Toptancı Adı) ----------------
            string toptanciAdi = row.Cells["Toptanci_Adi"].Value?.ToString() ?? "Belirtmek istemiyorum";
            if (!comboBox2.Items.Contains(toptanciAdi))
                comboBox2.Items.Add(toptanciAdi);
            comboBox2.SelectedItem = toptanciAdi;

            // ---------------- ComboBox3 (İşlem Türü) ----------------
            string islemTuru = dataGridView1.Columns.Contains("IslemTuru")
                ? row.Cells["IslemTuru"].Value?.ToString() ?? "Toptancı Adını Belirtmek istemiyorum"
                : "Toptancı Adını Belirtmek istemiyorum";
            if (!comboBox3.Items.Contains(islemTuru))
                comboBox3.Items.Add(islemTuru);
            comboBox3.SelectedItem = islemTuru;

            // ---------------- Ölçü Birimi (ComboBox5) ----------------
            string olcuBirimi = row.Cells["OlcuBirimi"]?.Value?.ToString() ?? "Adet"; // Varsayılan Adet
            if (!comboBox5.Items.Contains(olcuBirimi))
                comboBox5.Items.Add(olcuBirimi);
            comboBox5.SelectedItem = olcuBirimi;

            // ---------------- AsgariStok ve 2SatisFiyati ----------------
            if (decimal.TryParse(row.Cells["AsgariStok"]?.Value?.ToString().Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal asgariStok))
                textBox26.Text = asgariStok.ToString(System.Globalization.CultureInfo.CurrentCulture);
            else
                textBox26.Text = "";

            // ⚠️ Bu kısım zaten doğruydu, bu yüzden dokunmadık.
            if (decimal.TryParse(row.Cells["2SatisFiyati"]?.Value?.ToString().Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal ikinciSatisFiyati))
                textBox25.Text = ikinciSatisFiyati.ToString(System.Globalization.CultureInfo.CurrentCulture);
            else
                textBox25.Text = "";

            // ---------------- Saat ----------------
            if (DateTime.TryParse(row.Cells["Saat"]?.Value?.ToString(), out DateTime saat))
                textBox28.Text = saat.ToString("HH:mm");
            else
                textBox28.Text = "";
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            string aramaMetni = textBox22.Text.Trim();
            if (string.IsNullOrEmpty(aramaMetni))
            {
                Listele(); // Eğer arama metni boşsa, tüm listeyi göster
            }
            else
            {
                try
                {
                    using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OleDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                    {
                        DataTable tablo = new DataTable();
                        baglan.Open();

                        // Bu sorgu Barkod No, Ürün Adı ve Ürün Kodu alanlarında arama yapar.
                        string query = "SELECT * FROM ÜrünGirişi WHERE Barkod_No LIKE @arama OR Ürün_Adi LIKE @arama OR Ürün_Kodu LIKE @arama";

                        OleDbDataAdapter adapter = new OleDbDataAdapter(query, baglan);
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

        private void button15_Click(object sender, EventArgs e)
        {
            // 1️⃣ Alan kontrolleri
            if (string.IsNullOrWhiteSpace(textBox1.Text)) { MessageBox.Show("Barkod numarası boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrWhiteSpace(textBox2.Text)) { MessageBox.Show("Ürün adı boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrWhiteSpace(textBox3.Text)) { MessageBox.Show("Ürün kodu boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (comboBox1.SelectedIndex == -1) { MessageBox.Show("Lütfen bir ürün grubu seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrWhiteSpace(textBox6.Text)) { MessageBox.Show("Satış fiyatı boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            // 2️⃣ Stok ve eklenen miktar kontrolü
            decimal stokMiktari;
            if (!decimal.TryParse(textBox8.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out stokMiktari) || stokMiktari < 0)
            {
                MessageBox.Show("Mevcut stok miktarı geçerli bir sayı olmalı ve negatif olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // ✅ Eklenen miktar kontrolü ve uyarı
            decimal eklenenMiktar = 0;
            string eklenenMiktarStr = textBox23.Text.Trim();
            if (!string.IsNullOrWhiteSpace(eklenenMiktarStr))
            {
                // Sayı formatı kontrolü eklendi
                if (eklenenMiktarStr.Contains(",") && eklenenMiktarStr.Split(',')[1].Length > 2)
                {
                    string dogruFormat = eklenenMiktarStr.Replace(",", "");
                    MessageBox.Show($"Lütfen eklenen miktarı {dogruFormat} veya {dogruFormat},00 olarak girin. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string cleanedStr = eklenenMiktarStr.Replace(".", "").Replace(",", "."); // Binlik ve ondalık ayraçlarını düzenle
                if (!decimal.TryParse(cleanedStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out eklenenMiktar))
                {
                    MessageBox.Show("Eklenen miktar geçerli bir sayı olmalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            decimal yeniStok = stokMiktari + eklenenMiktar;
            textBox24.Text = yeniStok.ToString(); // TextBox24’e yaz
                                                  // Asgari stok kontrolü
            if (!string.IsNullOrWhiteSpace(textBox26.Text))
            {
                decimal asgariStok;
                // Sayı formatı kontrolü ve çevirme eklendi
                string asgariStokStr = textBox26.Text.Trim();
                if (asgariStokStr.Contains(",") && asgariStokStr.Split(',')[1].Length > 2)
                {
                    string dogruFormat = asgariStokStr.Replace(",", "");
                    MessageBox.Show($"Lütfen asgari stoku {dogruFormat} veya {dogruFormat},00 olarak girin.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string cleanedAsgariStokStr = asgariStokStr.Replace(".", "").Replace(",", ".");
                if (decimal.TryParse(cleanedAsgariStokStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out asgariStok))
                {
                    if (asgariStok > yeniStok)
                    {
                        MessageBox.Show("Asgari stok sayısı stok sayısından yüksek olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Asgari stok geçerli bir sayı olmalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            // 3️⃣ Checkbox ile alış fiyatı seçimi
            decimal alisFiyati1 = 0; // KDV Dahil (yeni)
            decimal alisFiyati2 = 0; // KDV Hariç
            decimal satisFiyati;
            decimal kdvOrani = 0; // KDV oranını direkt girilen değer olarak alacağız
                                  // KDV Oranı kontrolü (düzeltildi: format kontrolü eklendi, bölme kaldırıldı)
            string kdvStr = textBox7.Text.Trim();
            if (kdvStr.Contains(",") && kdvStr.Split(',')[1].Length > 2)
            {
                string dogruFormat = kdvStr.Replace(",", "");
                MessageBox.Show($"Lütfen KDV oranını {dogruFormat} veya {dogruFormat},00 olarak girin.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string cleanedKdvStr = kdvStr;
            if (kdvStr.Contains(",")) cleanedKdvStr = kdvStr.Replace(",", "."); // Sadece virgülü noktaya çevir
            if (!decimal.TryParse(cleanedKdvStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out kdvOrani))
            {
                MessageBox.Show("KDV oranı geçerli bir sayı olmalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // KDV oranını yüzde olarak direkt kullanacağız (örneğin, 4 girilirse 4 olarak kalır)
            // Satış fiyatı kontrolü (düzeltildi: tutarlı format kontrolü)
            string satisFiyatiStr = textBox6.Text.Trim();
            if (satisFiyatiStr.Contains(",") && satisFiyatiStr.Split(',')[1].Length > 2)
            {
                string dogruFormat = satisFiyatiStr.Replace(",", "");
                MessageBox.Show($"Lütfen satış fiyatını {dogruFormat} veya {dogruFormat},00 TL olarak girin. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string cleanedSatisFiyatiStr = satisFiyatiStr;
            if (satisFiyatiStr.Contains(",")) cleanedSatisFiyatiStr = satisFiyatiStr.Replace(",", "."); // Sadece virgülü noktaya çevir
            if (!decimal.TryParse(cleanedSatisFiyatiStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out satisFiyati))
            {
                MessageBox.Show("Satış fiyatı geçerli bir sayı olmalı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (checkBox1.Checked)
            {
                // Alış fiyatı (KDV Dahil) kontrolü (düzeltildi: tutarlı format kontrolü)
                string alisFiyati1Str = textBox4.Text.Trim();
                if (alisFiyati1Str.Contains(",") && alisFiyati1Str.Split(',')[1].Length > 2)
                {
                    string dogruFormat = alisFiyati1Str.Replace(",", "");
                    MessageBox.Show($"Lütfen alış fiyatını (KDV Dahil) {dogruFormat} veya {dogruFormat},00 TL olarak girin. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string cleanedAlisFiyati1Str = alisFiyati1Str;
                if (alisFiyati1Str.Contains(",")) cleanedAlisFiyati1Str = alisFiyati1Str.Replace(",", "."); // Sadece virgülü noktaya çevir
                if (!decimal.TryParse(cleanedAlisFiyati1Str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out alisFiyati1))
                {
                    MessageBox.Show("Alış fiyatı (KDV Dahil) geçerli bir sayı olmalı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                alisFiyati2 = Math.Round(alisFiyati1 / (1 + kdvOrani / 100), 2); // KDV oranını yüzde olarak kullan
                GlobalAyarlar.KdvDahilGoster = true;
            }
            else if (checkBox2.Checked)
            {
                // Alış fiyatı (KDV Hariç) kontrolü (düzeltildi: tutarlı format kontrolü)
                string alisFiyati2Str = textBox5.Text.Trim();
                if (alisFiyati2Str.Contains(",") && alisFiyati2Str.Split(',')[1].Length > 2)
                {
                    string dogruFormat = alisFiyati2Str.Replace(",", "");
                    MessageBox.Show($"Lütfen alış fiyatını (KDV Hariç) {dogruFormat} veya {dogruFormat},00 TL olarak girin. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string cleanedAlisFiyati2Str = alisFiyati2Str;
                if (alisFiyati2Str.Contains(",")) cleanedAlisFiyati2Str = alisFiyati2Str.Replace(",", "."); // Sadece virgülü noktaya çevir
                if (!decimal.TryParse(cleanedAlisFiyati2Str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out alisFiyati2))
                {
                    MessageBox.Show("Alış fiyatı (KDV Hariç) geçerli bir sayı olmalı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                alisFiyati1 = Math.Round(alisFiyati2 * (1 + kdvOrani / 100), 2); // KDV oranını yüzde olarak kullan
                GlobalAyarlar.KdvDahilGoster = false;
            }
            else
            {
                MessageBox.Show("Lütfen bir alış fiyatı tipi seçin (KDV Dahil veya Hariç).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 4️⃣ 2. Satış Fiyatı kontrolü (zaten doğru, ama parse mantığı güncellendi)
            decimal ikinciSatisFiyati = 0;
            if (!string.IsNullOrWhiteSpace(textBox25.Text))
            {
                string ikinciSatisFiyatiStr = textBox25.Text.Trim();
                if (ikinciSatisFiyatiStr.Contains(",") && ikinciSatisFiyatiStr.Split(',')[1].Length > 2)
                {
                    string dogruFormat = ikinciSatisFiyatiStr.Replace(",", "");
                    MessageBox.Show($"Lütfen 2. satış fiyatını {dogruFormat} veya {dogruFormat},00 TL olarak girin. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string cleanedIkinciSatisFiyatiStr = ikinciSatisFiyatiStr;
                if (ikinciSatisFiyatiStr.Contains(",")) cleanedIkinciSatisFiyatiStr = ikinciSatisFiyatiStr.Replace(",", ".");
                if (!decimal.TryParse(cleanedIkinciSatisFiyatiStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out ikinciSatisFiyati))
                {
                    MessageBox.Show("2. satış fiyatı geçerli bir sayı olmalı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (ikinciSatisFiyati >= satisFiyati)
                {
                    MessageBox.Show("2. satış fiyatı, satış fiyatına eşit veya ondan yüksek olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            // 5️⃣ Tarih ve saat kontrolü
            if (string.IsNullOrWhiteSpace(textBox27.Text) || string.IsNullOrWhiteSpace(textBox28.Text))
            {
                MessageBox.Show("Tarih ve saat boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 6️⃣ Toptancı kontrolü (değişken tanımlandı, ancak GSM verisi DB'den alınacak _baglan_ açıldıktan sonra)

            try
            {
                using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();

                    // --- Burada secilenGsmTelefon'ı DB'den alıyoruz (eğer toptancı seçilmişse) ---
                    string secilenGsmTelefon = "";
                    if (comboBox2.Text != "Toptancı Adını Belirtmek istemiyorum")
                    {
                        string sorgu = "SELECT GsmTelefon FROM Toptancilar WHERE ToptanciAdi=@ToptanciAdi";
                        using (OleDbCommand cmd = new OleDbCommand(sorgu, baglan))
                        {
                            cmd.Parameters.AddWithValue("@ToptanciAdi", comboBox2.Text);
                            object sonuc = cmd.ExecuteScalar();
                            if (sonuc != null && sonuc != DBNull.Value)
                            {
                                secilenGsmTelefon = sonuc.ToString().Trim();
                            }
                        }

                        if (string.IsNullOrWhiteSpace(secilenGsmTelefon))
                        {
                            MessageBox.Show("Seçilen toptancının telefon numarası bulunamadı. Lütfen geçerli bir toptancı seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // 7️⃣ Ürün kontrolü (var mı diye bak)
                    string kontrolQuery = "SELECT COUNT(*) FROM ÜrünGirişi WHERE Barkod_No=@BarkodNo";
                    using (OleDbCommand kontrolKmt = new OleDbCommand(kontrolQuery, baglan))
                    {
                        kontrolKmt.Parameters.AddWithValue("@BarkodNo", textBox1.Text);
                        int kayitSayisi = (int)kontrolKmt.ExecuteScalar();
                        if (kayitSayisi == 0) { MessageBox.Show("Bu barkod numarasına sahip bir ürün bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    }
                    // 8️⃣ Mevcut alış fiyatı, stok miktarını ve toptancı bilgisini al (düzeltildi: parse tutarlı hale getirildi)
                    decimal eskiAlisFiyati1 = 0;
                    decimal eskiStok = 0;
                    string eskiToptanciAdi = string.Empty;
                    string eskiGsmTelefon = string.Empty;
                    string selectQuery = "SELECT Alis_Fiyati, Stok_Miktari, Toptanci_Adi, GsmTelefon FROM ÜrünGirişi WHERE Barkod_No=@BarkodNo";
                    using (OleDbCommand selectKmt = new OleDbCommand(selectQuery, baglan))
                    {
                        selectKmt.Parameters.AddWithValue("@BarkodNo", textBox1.Text);
                        using (OleDbDataReader reader = selectKmt.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["Alis_Fiyati"] != DBNull.Value)
                                {
                                    string alisStr = reader["Alis_Fiyati"].ToString().Trim();
                                    string cleanedAlis = alisStr;
                                    if (alisStr.Contains(",")) cleanedAlis = alisStr.Replace(",", ".");
                                    decimal.TryParse(cleanedAlis, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out eskiAlisFiyati1);
                                }
                                eskiStok = Convert.ToDecimal(reader["Stok_Miktari"]);
                                eskiToptanciAdi = reader["Toptanci_Adi"].ToString();
                                eskiGsmTelefon = reader["GsmTelefon"].ToString();
                            }
                        }
                    }
                    // 9️⃣ Ürün güncelleme
                    string updateQuery = "UPDATE ÜrünGirişi SET Ürün_Adi=@UrunAdi, Ürün_Kodu=@UrunKodu, Ürün_Grubu=@UrunGrubu, " +
                                         "Alis_Fiyati=@AlisFiyati, Alis_Fiyati2=@AlisFiyati2, Satis_Fiyati=@SatisFiyati, " +
                                         "Stok_Miktari=@StokMiktari, Miktar=@Miktar, Toptanci_Adi=@ToptanciAdi, GsmTelefon=@GsmTelefon, " +
                                         "Tarih=@Tarih, Saat=@Saat, IslemTuru=@IslemTuru, KDV_Orani=@KDVOrani, " +
                                         "AsgariStok=@AsgariStok, 2SatisFiyati=@IkinciSatisFiyati, OlcuBirimi=@OlcuBirimi " +
                                         "WHERE Barkod_No=@BarkodNo";
                    using (OleDbCommand updateKmt = new OleDbCommand(updateQuery, baglan))
                    {
                        updateKmt.Parameters.AddWithValue("@UrunAdi", textBox2.Text);
                        updateKmt.Parameters.AddWithValue("@UrunKodu", textBox3.Text);
                        updateKmt.Parameters.AddWithValue("@UrunGrubu", comboBox1.Text);
                        updateKmt.Parameters.AddWithValue("@AlisFiyati", alisFiyati1.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        updateKmt.Parameters.AddWithValue("@AlisFiyati2", alisFiyati2.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        updateKmt.Parameters.AddWithValue("@SatisFiyati", satisFiyati.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        updateKmt.Parameters.AddWithValue("@StokMiktari", yeniStok);
                        updateKmt.Parameters.AddWithValue("@Miktar", yeniStok);
                        updateKmt.Parameters.AddWithValue("@ToptanciAdi", comboBox2.Text);
                        updateKmt.Parameters.AddWithValue("@GsmTelefon", comboBox2.Text == "Toptancı Adını Belirtmek istemiyorum" ? DBNull.Value : (object)secilenGsmTelefon);
                        updateKmt.Parameters.AddWithValue("@Tarih", DateTime.Parse(textBox27.Text));
                        updateKmt.Parameters.AddWithValue("@Saat", DateTime.Parse(textBox28.Text).ToShortTimeString());
                        updateKmt.Parameters.AddWithValue("@IslemTuru", comboBox3.Text);
                        updateKmt.Parameters.AddWithValue("@KDVOrani", kdvOrani.ToString(System.Globalization.CultureInfo.InvariantCulture)); // Doğrudan girilen değer
                        string cleanedAsgariStokParam = string.IsNullOrWhiteSpace(textBox26.Text) ? null : textBox26.Text.Replace(",", ".");
                        updateKmt.Parameters.AddWithValue("@AsgariStok", string.IsNullOrWhiteSpace(cleanedAsgariStokParam) ? DBNull.Value : (object)cleanedAsgariStokParam);
                        updateKmt.Parameters.AddWithValue("@IkinciSatisFiyati", string.IsNullOrWhiteSpace(textBox25.Text) ? DBNull.Value : (object)ikinciSatisFiyati.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        updateKmt.Parameters.AddWithValue("@OlcuBirimi", comboBox5.Text);
                        updateKmt.Parameters.AddWithValue("@BarkodNo", textBox1.Text);
                        updateKmt.ExecuteNonQuery();
                    }
                    // 10️⃣ Toptancı borç mantığı (düzeltilmiş: fark/taşınma/silme/ekleme durumlarını ele alır)
                    decimal eskiBorc = eskiAlisFiyati1 * eskiStok;
                    decimal yeniBorc = alisFiyati1 * yeniStok;

                    // Eğer seçili toptancı belirtilmişse işleme al
                    if (comboBox2.Text != "Toptancı Adını Belirtmek istemiyorum")
                    {
                        // --- 1) Eğer eski toptancı farklıysa: eski toptancıdan eskiBorc düş
                        if (!string.IsNullOrWhiteSpace(eskiGsmTelefon) && !string.Equals(eskiGsmTelefon, secilenGsmTelefon, StringComparison.OrdinalIgnoreCase))
                        {
                            decimal eskiToplamBorc = 0;
                            string selectEski = "SELECT ToplamBorc FROM Toptancilar WHERE GsmTelefon=@GsmTelefon";
                            using (OleDbCommand cmd = new OleDbCommand(selectEski, baglan))
                            {
                                cmd.Parameters.AddWithValue("@GsmTelefon", eskiGsmTelefon);
                                object res = cmd.ExecuteScalar();
                                if (res != DBNull.Value && res != null)
                                {
                                    string borcStr = res.ToString().Trim();
                                    if (borcStr.Contains(",")) borcStr = borcStr.Replace(",", ".");
                                    decimal.TryParse(borcStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out eskiToplamBorc);
                                }
                            }
                            decimal yeniEskiToplam = eskiToplamBorc - eskiBorc;
                            if (yeniEskiToplam < 0) yeniEskiToplam = 0;
                            string updateEski = "UPDATE Toptancilar SET ToplamBorc=@ToplamBorc WHERE GsmTelefon=@GsmTelefon";
                            using (OleDbCommand cmd = new OleDbCommand(updateEski, baglan))
                            {
                                cmd.Parameters.AddWithValue("@ToplamBorc", yeniEskiToplam.ToString(System.Globalization.CultureInfo.InvariantCulture));
                                cmd.Parameters.AddWithValue("@GsmTelefon", eskiGsmTelefon);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // --- 2) Yeni/Seçili toptancıya ekle veya güncelle (eski ve yeni aynıysa farkı uygula)
                        decimal mevcutYeniBorc = 0;
                        string selectYeni = "SELECT ToplamBorc FROM Toptancilar WHERE GsmTelefon=@GsmTelefon";
                        using (OleDbCommand cmd = new OleDbCommand(selectYeni, baglan))
                        {
                            cmd.Parameters.AddWithValue("@GsmTelefon", secilenGsmTelefon);
                            object res = cmd.ExecuteScalar();
                            if (res != DBNull.Value && res != null)
                            {
                                string borcStr = res.ToString().Trim();
                                if (borcStr.Contains(",")) borcStr = borcStr.Replace(",", ".");
                                decimal.TryParse(borcStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out mevcutYeniBorc);
                            }
                        }

                        decimal yeniToplamDeger;
                        if (string.Equals(eskiGsmTelefon, secilenGsmTelefon, StringComparison.OrdinalIgnoreCase))
                        {
                            // Aynı toptancı ise sadece farkı uygula
                            decimal fark = yeniBorc - eskiBorc;
                            yeniToplamDeger = mevcutYeniBorc + fark;
                        }
                        else
                        {
                            // Farklı toptancı ise yeniBorc'u ekle
                            yeniToplamDeger = mevcutYeniBorc + yeniBorc;
                        }

                        if (yeniToplamDeger < 0) yeniToplamDeger = 0;

                        string updateYeni = "UPDATE Toptancilar SET ToplamBorc=@ToplamBorc WHERE GsmTelefon=@GsmTelefon";
                        using (OleDbCommand cmd = new OleDbCommand(updateYeni, baglan))
                        {
                            cmd.Parameters.AddWithValue("@ToplamBorc", yeniToplamDeger.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@GsmTelefon", secilenGsmTelefon);
                            int affected = cmd.ExecuteNonQuery();
                            if (affected == 0)
                            {
                                // Eğer kayıt yoksa yeni topancı ekle
                                string insertYeni = "INSERT INTO Toptancilar (GsmTelefon, ToplamBorc) VALUES (@GsmTelefon, @ToplamBorc)";
                                using (OleDbCommand cmdInsert = new OleDbCommand(insertYeni, baglan))
                                {
                                    cmdInsert.Parameters.AddWithValue("@GsmTelefon", secilenGsmTelefon);
                                    cmdInsert.Parameters.AddWithValue("@ToplamBorc", yeniToplamDeger.ToString(System.Globalization.CultureInfo.InvariantCulture));
                                    cmdInsert.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    // Eğer eskiGsmTelefon boşsa ve yeni bir toptancı seçildiyse
                    if (string.IsNullOrWhiteSpace(eskiGsmTelefon) && comboBox2.Text != "Toptancı Adını Belirtmek istemiyorum")
                    {
                        // Yeni toptancıya borcu ekle
                        decimal yeniToplamBorcYeni = 0;
                        string selectYeni = "SELECT ToplamBorc FROM Toptancilar WHERE GsmTelefon=@GsmTelefon";
                        using (OleDbCommand cmd = new OleDbCommand(selectYeni, baglan))
                        {
                            cmd.Parameters.AddWithValue("@GsmTelefon", secilenGsmTelefon);
                            object res = cmd.ExecuteScalar();
                            if (res != DBNull.Value && res != null)
                                yeniToplamBorcYeni = Convert.ToDecimal(res);
                        }
                        yeniToplamBorcYeni += yeniBorc;
                        string updateYeni = "UPDATE Toptancilar SET ToplamBorc=@ToplamBorc WHERE GsmTelefon=@GsmTelefon";
                        using (OleDbCommand cmd = new OleDbCommand(updateYeni, baglan))
                        {
                            cmd.Parameters.AddWithValue("@ToplamBorc", yeniToplamBorcYeni);
                            cmd.Parameters.AddWithValue("@GsmTelefon", secilenGsmTelefon);
                            int affected = cmd.ExecuteNonQuery();
                            if (affected == 0)
                            {
                                string insertYeni = "INSERT INTO Toptancilar (GsmTelefon, ToplamBorc) VALUES (@GsmTelefon, @ToplamBorc)";
                                using (OleDbCommand cmdInsert = new OleDbCommand(insertYeni, baglan))
                                {
                                    cmdInsert.Parameters.AddWithValue("@GsmTelefon", secilenGsmTelefon);
                                    cmdInsert.Parameters.AddWithValue("@ToplamBorc", yeniToplamBorcYeni);
                                    cmdInsert.ExecuteNonQuery();
                                }
                            }
                        }
                    }


                    MessageBox.Show("Ürün başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // UI temizleme ve listeleme
                    textBox1.Clear(); textBox2.Clear(); textBox3.Clear(); textBox4.Clear(); textBox5.Clear(); textBox6.Clear();
                    textBox7.Clear(); textBox8.Clear(); textBox23.Clear(); textBox24.Clear(); textBox25.Clear(); textBox26.Clear();
                    comboBox1.SelectedIndex = 0; comboBox2.SelectedIndex = 0; comboBox3.SelectedIndex = 0;
                    button4.Visible = false; panel1.Visible = false; button13.Visible = false;
                    button6.Visible = true;
                    button9.Visible = true;
                    button14.Visible = false;
                    textBox8.ReadOnly = false;
                    button3.Visible = false;
                    checkBox1.Checked = true;
                    AlisFiyatiToplaminiGoster();
                    StokSayisiToplaminiGoster();
                    SatisFiyatiToplaminiGoster();
                    ListelenenToplamGoster();
                    Listele();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void panelToptanciEkle_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox24_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox23_TextChanged(object sender, EventArgs e)
        {

            ToplaVeYazdir();
        }
        private void textBox23_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece rakam, kontrol tuşları (Backspace gibi) ve virgül karakterine izin ver
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true; // Eğer girilen karakter bu şartları sağlamıyorsa, girişi engelle
            }
        }
        private void ToplaVeYazdir()
        {
            double stokMiktari = 0;
            double eklenenMiktar = 0;

            // Virgül yerine nokta koy ve Double'a çevir
            string txt8 = textBox8.Text.Replace(',', '.');
            string txt23 = textBox23.Text.Replace(',', '.');

            if (double.TryParse(txt8, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double sonuc8))
            {
                stokMiktari = sonuc8;
            }

            if (double.TryParse(txt23, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double sonuc23))
            {
                eklenenMiktar = sonuc23;
            }

            double toplam = stokMiktari + eklenenMiktar;
            textBox24.Text = toplam.ToString();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            ToplaVeYazdir();

        }

        private void Control_Enter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                // Ürün ekleme formu akışı
                if (sender == textBox1) textBox2.Focus();
                else if (sender == textBox2) textBox3.Focus();
                else if (sender == textBox3) comboBox1.Focus();
                else if (sender == comboBox1) comboBox2.Focus();

                // Combox2 sonrası alanlar
                else if (sender == comboBox2) textBox8.Focus();
                else if (sender == textBox8) textBox26.Focus();
                else if (sender == textBox26) textBox4.Focus();
                else if (sender == textBox4) textBox7.Focus();
                else if (sender == textBox7) textBox6.Focus();
                else if (sender == textBox6) textBox25.Focus();
                else if (sender == textBox25) comboBox3.Focus();
                // BURASI GÜNCELLENDİ: comboBox3'ten sonra comboBox4'e git
                else if (sender == comboBox3) comboBox5.Focus();
                // BURASI GÜNCELLENDİ: comboBox4'ten sonra button1'e git
                else if (sender == comboBox5) button1.Focus();

                // Diğer form alanları (Toptancı ekleme veya başka alanlar) eski sıralamayı koruyabilir
                else if (sender == textBox10) textBox18.Focus();
                else if (sender == textBox18) textBox11.Focus();
                else if (sender == textBox11) textBox12.Focus();
                else if (sender == textBox12) textBox13.Focus();
                else if (sender == textBox13) textBox14.Focus();
                else if (sender == textBox14) textBox15.Focus();
                else if (sender == textBox15) textBox16.Focus();
                else if (sender == textBox16) textBox17.Focus();
                else if (sender == textBox17) textBox19.Focus();
                else if (sender == textBox19) textBox21.Focus();
                else if (sender == textBox21) textBox20.Focus();
                else if (sender == textBox20) button10.Focus();
            }
        }

        private void Ürün_Girişi_KeyDown(object sender, KeyEventArgs e)
        {
            // F1 tuşu için button1'i çalıştır
            if (e.KeyCode == Keys.F1)
            {
                button1.PerformClick();
                e.Handled = true;
            }

            // F2 tuşu için button15'i çalıştır
            else if (e.KeyCode == Keys.F3)
            {
                button15.PerformClick();
                e.Handled = true;
            }

            // F3 tuşu için button4'ü çalıştır
            else if (e.KeyCode == Keys.F2)
            {
                button3.PerformClick();
                e.Handled = true;
            }

            // F4 tuşu için button7'yi çalıştır
            else if (e.KeyCode == Keys.F4)
            {

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // textBox1 boşsa, diğer tüm alanları temizle
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox2.Clear();
                textBox3.Clear();
                comboBox1.SelectedIndex = -1;
                textBox4.Clear();
                textBox5.Clear();
                textBox6.Clear();
                textBox7.Clear();
                textBox8.Clear();
                comboBox2.SelectedIndex = -1;
                textBox25.Clear();
                textBox26.Clear();
                button5.Visible = true;

                // Buton ve panelleri başlangıç durumuna getir
                button1.Visible = true;
                button15.Visible = false;
                button4.Visible = false; // Temizleme durumunda button4 görünmez olmalı
                panel1.Visible = false;
                button13.Visible = false;
                textBox23.Visible = false;
                textBox24.Visible = false;

                button3.Visible = false; // textBox1 boşsa button3 görünmesin
                return;
            }

            try
            {
                using (OleDbConnection baglan = new OleDbConnection(
                    "Provider=Microsoft.ACE.OleDB.12.0;Data Source=" +
                    Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();

                    string query = "SELECT * FROM ÜrünGirişi WHERE Barkod_No = @BarkodNo";
                    OleDbCommand kmt = new OleDbCommand(query, baglan);
                    kmt.Parameters.AddWithValue("@BarkodNo", textBox1.Text);

                    OleDbDataReader okuyucu = kmt.ExecuteReader();

                    if (okuyucu.Read())
                    {
                        // Veritabanında eşleşen kayıt varsa, diğer alanları doldur
                        textBox2.Text = okuyucu["Ürün_Adi"].ToString();
                        textBox3.Text = okuyucu["Ürün_Kodu"].ToString();
                        comboBox1.Text = okuyucu["Ürün_Grubu"].ToString();
                        textBox4.Text = okuyucu["Alis_Fiyati"].ToString();
                        textBox5.Text = okuyucu["Alis_Fiyati2"].ToString();
                        textBox6.Text = okuyucu["Satis_Fiyati"].ToString();
                        textBox7.Text = okuyucu["KDV_Orani"].ToString();
                        textBox8.Text = okuyucu["Stok_Miktari"].ToString();
                        comboBox2.Text = okuyucu["Toptanci_Adi"].ToString();
                        textBox26.Text = okuyucu["AsgariStok"].ToString();
                        textBox25.Text = okuyucu["2SatisFiyati"].ToString();

                        // Buton ve panelleri güncelleme moduna getir
                        button1.Visible = false;
                        button15.Visible = true;
                        button4.Visible = true;

                        // Panel ve ilgili kontroller gizli kalacak
                        panel1.Visible = false;
                        button13.Visible = false;
                        textBox23.Visible = false;
                        textBox24.Visible = false;
                        button14.Visible = true;

                        button3.Visible = true; // textBox1 doluysa button3 görünsün
                    }
                    else
                    {
                        // Eşleşen kayıt yoksa, sadece textBox1 dışındaki tüm alanları temizle
                        textBox2.Clear();
                        textBox3.Clear();
                        comboBox1.SelectedIndex = -1;
                        textBox4.Clear();
                        textBox5.Clear();
                        textBox6.Clear();
                        textBox7.Clear();
                        textBox8.Clear();
                        comboBox2.SelectedIndex = -1;
                        textBox25.Clear();
                        textBox26.Clear();

                        // Buton ve panelleri ekleme moduna getir
                        button1.Visible = true;
                        button15.Visible = false;
                        button4.Visible = false;
                        panel1.Visible = false;
                        button13.Visible = false;
                        textBox23.Visible = false;
                        textBox24.Visible = false;

                        button3.Visible = !string.IsNullOrWhiteSpace(textBox1.Text); // textBox1 doluysa görün
                    }

                    okuyucu.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {// Panel1 şu anda görünürse, yani kapatılacaksa
            if (panel1.Visible)
            {
                button13.Visible = false;
                textBox23.Visible = false;
                textBox24.Visible = false;
                panel1.Visible = false;

                // Kapatıldığında textbox23'ü temizle
                textBox23.Text = "";
            }
            // Panel1 şu anda görünür değilse, yani açılacaksa
            else
            {
                button13.Visible = true;
                textBox23.Visible = true;
                textBox24.Visible = true;
                panel1.Visible = true;

                // Açıldığında textbox23'e "0" yaz
                textBox23.Text = "0";
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) // sadece checkbox1 aktifse
                AlisFiyatiHesapla(textBox4, EventArgs.Empty);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) // sadece checkbox2 aktifse
                AlisFiyatiHesapla(textBox5, EventArgs.Empty);
        }


        private void panelGrupEkle_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Seçim yapılmamışsa
            if (comboBox2.SelectedIndex == -1 || string.IsNullOrWhiteSpace(comboBox2.Text))
            {
                secilenGsmTelefon = "";
                return;
            }

            // Eğer kullanıcı "Belirtmek istemiyorum" seçtiyse (case-insensitive, trimli kontrol)
            if (comboBox2.Text.Trim().Equals("Toptancı Adını Belirtmek istemiyorum", StringComparison.OrdinalIgnoreCase))
            {
                secilenGsmTelefon = ""; // Telefon olmayacak
                return;
            }

            string baglantiDizesi = "Provider=Microsoft.ACE.OleDb.12.0;Data Source="
                                    + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
            {
                try
                {
                    baglan.Open();
                    string sorgu = "SELECT GsmTelefon FROM Toptancilar WHERE ToptanciAdi = @ToptanciAdi";

                    using (OleDbCommand komut = new OleDbCommand(sorgu, baglan))
                    {
                        komut.Parameters.AddWithValue("@ToptanciAdi", comboBox2.Text);

                        object sonuc = komut.ExecuteScalar();
                        secilenGsmTelefon = sonuc != null ? sonuc.ToString() : "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Telefon numarası alınırken hata oluştu: " + ex.Message,
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    secilenGsmTelefon = "";
                }
            }
        }


        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox28.Text = DateTime.Now.ToLongTimeString();

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Eğer kullanıcı "Belirtmek istemiyorum" seçerse hiçbir işlem yapma
            if (comboBox3.Text == "Belirtmek istemiyorum")
            {
                return;
            }

            // Diğer seçimler
            string secilenOdeme = comboBox3.Text;
            // Burada secilenOdeme değişkenini istediğin gibi kullanabilirsin
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox27_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Seçim yapılmamışsa işlem yapma
            if (comboBox1.SelectedIndex == -1 || string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                return;
            }

            // Eğer kullanıcı "Belirtmek istemiyorum" seçtiyse
            if (comboBox1.Text == "Belirtmek istemiyorum")
            {
                // Burada, bu seçeneğe özgü yapılması gereken bir işlem varsa ekleyebilirsin.
                // Şimdilik herhangi bir işlem yapmadan metottan çıkıyoruz.
                return;
            }

            // Seçilen ürün grubu adını al.
            string secilenUrunGrubu = comboBox1.Text;

            // Burada, seçilen ürün grubuna göre filtreleme veya başka bir işlem yapabilirsin.
            // Örneğin, FiltreUygula() metodunu çağırabilirsin.
            // FiltreUygula(); 
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Lütfen silmek istediğiniz ürünün barkod numarasını girin.",
                                "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult cevap = MessageBox.Show("Bu ürünü silmek istediğinize emin misiniz?",
                                                 "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (cevap == DialogResult.Yes)
            {
                try
                {
                    using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                                                                        + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                    {
                        baglan.Open();

                        // 1️⃣ Silinecek ürünün bilgilerini al
                        string barkodNo = textBox1.Text;
                        decimal urunAlisFiyati = 0;
                        decimal urunStokMiktari = 0;
                        string urunToptanciGsm = string.Empty;

                        string selectQuery = "SELECT Alis_Fiyati, Stok_Miktari, GsmTelefon FROM ÜrünGirişi WHERE Barkod_No=@BarkodNo";
                        using (OleDbCommand selectKomut = new OleDbCommand(selectQuery, baglan))
                        {
                            selectKomut.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            using (OleDbDataReader reader = selectKomut.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (reader["Alis_Fiyati"] != DBNull.Value)
                                        urunAlisFiyati = decimal.Parse(reader["Alis_Fiyati"].ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                                    if (reader["Stok_Miktari"] != DBNull.Value)
                                        urunStokMiktari = decimal.Parse(reader["Stok_Miktari"].ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                                    if (reader["GsmTelefon"] != DBNull.Value)
                                        urunToptanciGsm = reader["GsmTelefon"].ToString();
                                }
                                else
                                {
                                    MessageBox.Show("Bu barkod numarasına sahip bir ürün bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }

                        // 2️⃣ Ürünü veritabanından sil
                        string silQuery = "DELETE FROM ÜrünGirişi WHERE Barkod_No=@BarkodNo";
                        using (OleDbCommand silKomut = new OleDbCommand(silQuery, baglan))
                        {
                            silKomut.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            int sonuc = silKomut.ExecuteNonQuery();

                            if (sonuc > 0)
                            {
                                // 3️⃣ Ürün silindiyse, toptancı borcunu güncelle
                                if (!string.IsNullOrWhiteSpace(urunToptanciGsm))
                                {
                                    decimal silinecekBorc = urunAlisFiyati * urunStokMiktari;
                                    decimal mevcutBorc = 0;

                                    string selectBorcQuery = "SELECT ToplamBorc FROM Toptancilar WHERE GsmTelefon=@GsmTelefon";
                                    using (OleDbCommand cmdSelectBorc = new OleDbCommand(selectBorcQuery, baglan))
                                    {
                                        cmdSelectBorc.Parameters.AddWithValue("@GsmTelefon", urunToptanciGsm);
                                        object result = cmdSelectBorc.ExecuteScalar();
                                        if (result != DBNull.Value && result != null)
                                        {
                                            mevcutBorc = decimal.Parse(result.ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                                        }
                                    }

                                    decimal yeniToplamBorc = mevcutBorc - silinecekBorc;

                                    // Borcun negatif olmasını engelle
                                    if (yeniToplamBorc < 0)
                                    {
                                        yeniToplamBorc = 0;
                                    }

                                    string updateToptanciQuery = "UPDATE Toptancilar SET ToplamBorc=@ToplamBorc WHERE GsmTelefon=@GsmTelefon";
                                    using (OleDbCommand updateToptanciKmt = new OleDbCommand(updateToptanciQuery, baglan))
                                    {
                                        updateToptanciKmt.Parameters.AddWithValue("@ToplamBorc", yeniToplamBorc.ToString(System.Globalization.CultureInfo.InvariantCulture));
                                        updateToptanciKmt.Parameters.AddWithValue("@GsmTelefon", urunToptanciGsm);
                                        updateToptanciKmt.ExecuteNonQuery();
                                    }
                                }

                                MessageBox.Show("Ürün başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // UI temizleme ve listeleme
                                textBox1.Clear();
                                textBox2.Clear();
                                textBox3.Clear();
                                textBox4.Clear();
                                textBox5.Clear();
                                textBox6.Clear();
                                textBox7.Clear();
                                textBox8.Clear();
                                textBox23.Clear();
                                textBox24.Clear();
                                textBox25.Clear();
                                textBox26.Clear();
                                comboBox1.SelectedIndex = 0;
                                comboBox2.SelectedIndex = 0;
                                comboBox3.SelectedIndex = 0;
                                comboBox5.SelectedIndex = 0;
                                button4.Visible = false;
                                panel1.Visible = false;
                                button13.Visible = false;
                                button6.Visible = true;
                                button9.Visible = true;
                                button14.Visible = false;
                                textBox8.ReadOnly = false;
                                button3.Visible = false;
                                checkBox1.Checked = true;
                                AlisFiyatiToplaminiGoster();
                                StokSayisiToplaminiGoster();
                                SatisFiyatiToplaminiGoster();
                                ListelenenToplamGoster();
                                Listele();
                            }
                            else
                            {
                                MessageBox.Show("Bu barkod numarasına sahip bir ürün bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void FiltreUygula()
        {
            try
            {
                using (OleDbConnection baglan = new OleDbConnection(
                    "Provider=Microsoft.ACE.OleDb.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();

                    string query = "SELECT * FROM ÜrünGirişi WHERE 1=1"; // Temel sorgu

                    // Asgari stok filtresi
                    if (checkBox3.Checked)
                    {
                        query += " AND Stok_Miktari < AsgariStok";
                    }

                    // Ürün grubu filtresi
                    if (comboBox4.SelectedItem != null && comboBox4.SelectedItem.ToString() != "Tümü")
                    {
                        query += " AND Ürün_Grubu=@GrupAdi";
                    }

                    using (OleDbCommand cmd = new OleDbCommand(query, baglan))
                    {
                        if (comboBox4.SelectedItem != null && comboBox4.SelectedItem.ToString() != "Tümü")
                        {
                            cmd.Parameters.AddWithValue("@GrupAdi", comboBox4.SelectedItem.ToString());
                        }

                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Filtreleme sırasında hata: " + ex.Message);
            }
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                // Örneğin ilk hücreyi almak için
                string secilenDeger = row.Cells[0].Value.ToString();

            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

            FiltreUygula();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

            FiltreUygula();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            panel4.Visible = true;
            button18.Visible = false;


        }

        private void button20_Click(object sender, EventArgs e)
        {
            panel4.Visible = false;
            button18.Visible = true;

        }


        private void button19_Click(object sender, EventArgs e)
        {


            if (dataGridView2.CurrentRow == null)
            {
                MessageBox.Show("Lütfen silmek istediğiniz grubu seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string grupAdi = dataGridView2.CurrentRow.Cells["GrupAdi"].Value.ToString();

            DialogResult cevap = MessageBox.Show(
                $"'{grupAdi}' grubunu silmek istediğinize emin misiniz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (cevap == DialogResult.Yes)
            {
                try
                {
                    using (OleDbConnection baglan = new OleDbConnection(
                               "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                               Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                    {
                        baglan.Open();
                        string silQuery = "DELETE FROM ÜrünGrupEkle WHERE GrupAdi=@GrupAdi";
                        using (OleDbCommand silKomut = new OleDbCommand(silQuery, baglan))
                        {
                            silKomut.Parameters.AddWithValue("@GrupAdi", grupAdi);

                            int sonuc = silKomut.ExecuteNonQuery();

                            if (sonuc > 0)
                            {
                                MessageBox.Show("Ürün grubu başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Listele(); // hem dataGridView2 hem de dataGridView1 yenileniyor
                                comboBox1.Items.Clear(); // comboBox1 temizle
                                LoadUrunGruplari();
                            }
                            else
                            {
                                MessageBox.Show("Bu ada sahip bir ürün grubu bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridView dgv = dataGridView1;

                if (dgv.Rows.Count == 0)
                {
                    MessageBox.Show("Aktarılacak veri yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Dosyası|*.xlsx";
                sfd.FileName = "UrunListesi.xlsx";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                using (var workbook = new ClosedXML.Excel.XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Ürün Listesi");
                    int currentRow = 1;

                    // --- Başlıklar ---
                    string[] columnsToExport = {
                "Barkod_No", "Ürün_Adi", "Ürün_Kodu", "Ürün_Grubu", "Stok_Miktari", "OlcuBirimi", "Satis_Fiyati",
                "AsgariStok", "2SatisFiyati", "Alis_Fiyati", "Alis_Fiyati2", "KDV_Orani", "Toptanci_Adi",
                "GsmTelefon", "IslemTuru", "Tarih", "Saat", "Miktar" // Miktar en sona eklendi
            };

                    for (int i = 0; i < columnsToExport.Length; i++)
                    {
                        string colName = columnsToExport[i];
                        var cell = ws.Cell(currentRow, i + 1);

                        string headerText = colName switch
                        {
                            "Alis_Fiyati" => "Kdv Dahil",
                            "Satis_Fiyati" => "Satış Fiyatı",
                            "2SatisFiyati" => "İndirimli Fiyat",
                            "Stok_Miktari" => "Stok Miktarı",
                            "OlcuBirimi" => "Ölçü Birimi", // Ölçü Birimi başlığı
                            "Miktar" => "Miktar", // Miktar başlığı
                            _ => dgv.Columns.Contains(colName) ? dgv.Columns[colName].HeaderText : colName
                        };

                        cell.Value = headerText;
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                        cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                        cell.Style.Alignment.Vertical = ClosedXML.Excel.XLAlignmentVerticalValues.Center;
                    }

                    currentRow++;

                    // --- Satır verileri ---
                    for (int i = 0; i < dgv.Rows.Count; i++)
                    {
                        for (int j = 0; j < columnsToExport.Length; j++)
                        {
                            string colName = columnsToExport[j];
                            var cell = ws.Cell(currentRow + i, j + 1);
                            object value = null;

                            if (dgv.Columns.Contains(colName))
                            {
                                value = dgv.Rows[i].Cells[colName].Value;
                            }

                            // Tarih ve Saat kolonlarını formatla
                            if (colName == "Tarih" && DateTime.TryParse(value?.ToString(), out DateTime tarih))
                                cell.Value = tarih.ToString("dd.MM.yyyy");
                            else if (colName == "Saat" && DateTime.TryParse(value?.ToString(), out DateTime saat))
                                cell.Value = saat.ToString("HH:mm");
                            else
                                cell.Value = value?.ToString() ?? "";

                            // Sayısal kolonlar
                            if (colName.Contains("Stok") || colName.Contains("Fiyati") || colName.Contains("KDV") || colName == "Miktar")
                            {
                                cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                                if (colName.Contains("Fiyati"))
                                    cell.Style.NumberFormat.Format = "#,##0.00";
                                else if (colName == "Miktar" || colName == "Stok_Miktari")
                                    cell.Style.NumberFormat.Format = "#,##0.00"; // Miktar ve Stok Miktarı için ondalık format
                            }

                            // Zebra efekt
                            if (i % 2 == 1)
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F3F3F3");
                        }
                    }

                    // --- Kenarlık ---
                    var tableRange = ws.Range(1, 1, currentRow + dgv.Rows.Count - 1, columnsToExport.Length);
                    tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // --- Kolon genişlikleri ---
                    int[] columnWidths = { 20, 35, 20, 15, 15, 15, 18, 18, 18, 12, 12, 12, 25, 18, 15, 15, 10, 10 }; // Ölçü Birimi ve Miktar için genişlikler eklendi
                    for (int i = 0; i < columnWidths.Length; i++)
                        ws.Column(i + 1).Width = columnWidths[i];

                    // --- Satır yüksekliği ---
                    ws.Rows(1, currentRow + dgv.Rows.Count).Height = 22;

                    workbook.SaveAs(sfd.FileName);
                }

                MessageBox.Show("Excel aktarımı tamamlandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel aktarımı sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private DataTable ExceldenVeriAl(string dosyaYolu)
        {
            DataTable dt = new DataTable();

            Dictionary<string, string> baslikEsleme = new Dictionary<string, string>
    {
        {"Barkod No", "Barkod_No"},
        {"Ürün Adı", "Ürün_Adi"},
        {"Ürün Kodu", "Ürün_Kodu"},
        {"Ürün Grubu", "Ürün_Grubu"},
        {"Stok Miktarı", "Stok_Miktari"},
        {"Ölçü Birimi", "OlcuBirimi"},
        {"Satış Fiyatı", "Satis_Fiyati"},
        {"Asgari Stok", "AsgariStok"},
        {"İndirimli Fiyat", "2SatisFiyati"},
        {"Kdv Dahil", "Alis_Fiyati"},
        {"Kdv Hariç", "Alis_Fiyati2"},
        {"KDV Oranı", "KDV_Orani"},
        {"Toptancı Adı", "Toptanci_Adi"},
        {"Gsm Telefon", "GsmTelefon"},
        {"İşlem Türü", "IslemTuru"},
        {"Tarih", "Tarih"},
        {"Saat", "Saat"},
        {"Miktar", "Miktar"} // Metin olarak kalacak
    };

            using (var workbook = new XLWorkbook(dosyaYolu))
            {
                var ws = workbook.Worksheet(1);

                // 1️⃣ Başlıkları al
                var headerRow = ws.Row(1);
                Dictionary<int, string> columnMap = new Dictionary<int, string>();

                for (int col = 1; col <= headerRow.LastCellUsed().Address.ColumnNumber; col++)
                {
                    string excelBaslik = headerRow.Cell(col).GetString().Trim();
                    if (baslikEsleme.ContainsKey(excelBaslik))
                    {
                        string columnName = baslikEsleme[excelBaslik];
                        dt.Columns.Add(columnName, typeof(string)); // Tüm sütunlar string olarak başlatılıyor
                        columnMap[col] = columnName;
                    }
                }

                // 2️⃣ Satırları ekle
                var usedRows = ws.RangeUsed().RowsUsed().Skip(1);
                foreach (var row in usedRows)
                {
                    DataRow newRow = dt.NewRow();
                    foreach (var map in columnMap)
                    {
                        var cell = row.Cell(map.Key);
                        string cellValue = cell == null ? "" : cell.Value.ToString();
                        newRow[map.Value] = cellValue;
                    }
                    dt.Rows.Add(newRow);
                }

                // 3️⃣ Sadece gerekli sütunlar için decimal / DateTime dönüşümünü işle
                foreach (DataColumn col in dt.Columns)
                {
                    Type type = GetColumnType(col.ColumnName);
                    if (type == typeof(decimal))
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            if (decimal.TryParse(r[col.ColumnName].ToString(), out decimal val))
                                r[col.ColumnName] = val;
                            else
                                r[col.ColumnName] = 0m; // Sayısal sütunlar için 0 atanabilir
                        }
                    }
                    else if (type == typeof(DateTime))
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            if (DateTime.TryParse(r[col.ColumnName].ToString(), out DateTime val))
                                r[col.ColumnName] = val;
                            else
                                r[col.ColumnName] = DBNull.Value;
                        }
                    }
                    // "Miktar" için dönüşüm yapılmayacak, string olarak kalacak
                }
            }

            return dt;
        }



        private Type GetColumnType(string columnName)
        {
            switch (columnName)
            {
                case "Stok_Miktari":
                case "Satis_Fiyati":
                case "AsgariStok":
                case "2SatisFiyati":
                case "Alis_Fiyati":
                case "Alis_Fiyati2":
                case "KDV_Orani":
                    return typeof(decimal);
                case "Tarih":
                case "Saat":
                    return typeof(DateTime);
                default: // "Miktar" da dahil diğer tüm sütunlar string olarak kalır
                    return typeof(string);
            }
        }

        private void UrunGruplariniDoldur()
        {
            try
            {
                using (OleDbConnection baglan = new OleDbConnection(
                    "Provider=Microsoft.ACE.OleDb.12.0;Data Source=" +
                    Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();

                    // Ürün gruplarını veritabanından çekme sorgusu
                    string query = "SELECT DISTINCT Ürün_Grubu FROM ÜrünGirişi";
                    using (OleDbCommand cmd = new OleDbCommand(query, baglan))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            // ComboBox'ı temizle
                            comboBox4.Items.Clear();
                            comboBox4.Items.Add("Tümü");

                            // Verileri ComboBox'a ekle
                            while (reader.Read())
                            {
                                if (reader["Ürün_Grubu"] != DBNull.Value)
                                {
                                    comboBox4.Items.Add(reader["Ürün_Grubu"].ToString());
                                }
                            }
                            comboBox4.SelectedIndex = 0; // İlk öğeyi (Tümü) seçili yap
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün grupları yüklenirken hata oluştu: " + ex.Message);
            }
        }
        private void Yenile()
        {
            LoadToptancilar();
            LoadUrunGruplari();
            UrunGruplariniDoldur();

            // Veritabanından en güncel veriyi çek ve DataGridView'e doldur
            Listele();

            FiltreUygula();
            // DataGridView'deki verilere göre tüm toplamları yeniden hesapla ve textbox'ları güncelle
            SatisFiyatiToplaminiGoster();
            ListelenenToplamGoster();
            AlisFiyatiToplaminiGoster();
            StokSayisiToplaminiGoster();
        }
        private async void button16_Click(object sender, EventArgs e)
        {
            button16.Enabled = false;
            label33.ForeColor = System.Drawing.Color.Black;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Dosyası|*.xlsx";
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                button16.Enabled = true;
                return;
            }

            try
            {
                progressBar1.Visible = true;
                label33.Visible = true;
                button23.Visible = true;
                label33.Text = "Excel verileri okunuyor...";
                Application.DoEvents();

                DataTable excelTablosu = ExceldenVeriAl(ofd.FileName);

                if (excelTablosu.Rows.Count == 0)
                {
                    MessageBox.Show("Aktarılacak veri yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                dataGridView1.DataSource = excelTablosu;

                // ✅ iptal için yeni token oluştur
                cts = new CancellationTokenSource();

                var sonuc = await Task.Run(() =>
                {
                    int eklenenUrunSayisi = 0;
                    int atlananUrunSayisi = 0;
                    int toplamSatir = excelTablosu.Rows.Count;

                    using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                    {
                        baglan.Open();

                        for (int i = 0; i < toplamSatir; i++)
                        {
                            if (cts.Token.IsCancellationRequested)
                                break;

                            try
                            {
                                if (!this.IsDisposed && this.IsHandleCreated)
                                {
                                    this.BeginInvoke((MethodInvoker)delegate
                                    {
                                        // Maximum'u baştan ayarla
                                        if (progressBar1.Maximum != toplamSatir)
                                            progressBar1.Maximum = toplamSatir;

                                        // i+1 maksimumu geçmesin
                                        progressBar1.Value = Math.Min(i + 1, progressBar1.Maximum);

                                        int yuzde = (int)Math.Round(((double)(i + 1) * 100) / toplamSatir);
                                        label33.Text = $"Veriler yükleniyor... %{yuzde}";
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                // Hata olsa da patlamasın
                                Console.WriteLine("Progress bar hatası: " + ex.Message);
                            }


                            DataRow row = excelTablosu.Rows[i];

                            string barkodNo = row["Barkod_No"]?.ToString() ?? "";
                            if (string.IsNullOrEmpty(barkodNo) || string.IsNullOrWhiteSpace(barkodNo))
                            {
                                atlananUrunSayisi++;
                                continue;
                            }

                            try
                            {
                                OleDbCommand checkCmd = new OleDbCommand("SELECT COUNT(*) FROM [ÜrünGirişi] WHERE Barkod_No = @Barkod_No", baglan);
                                checkCmd.Parameters.AddWithValue("@Barkod_No", barkodNo);
                                if ((int)checkCmd.ExecuteScalar() > 0)
                                {
                                    atlananUrunSayisi++;
                                    continue;
                                }

                                string urunAdi = row["Ürün_Adi"]?.ToString() ?? "";
                                string urunKodu = row["Ürün_Kodu"]?.ToString() ?? "";
                                string urunGrubu = row["Ürün_Grubu"]?.ToString() ?? "";
                                string toptanciAdi = row["Toptanci_Adi"]?.ToString() ?? "";
                                string gsmTelefon = row["GsmTelefon"]?.ToString() ?? "";
                                string olcuBirimi = row["OlcuBirimi"]?.ToString() ?? ""; // Ölçü Birimi
                                decimal miktar = 0; // Miktar eklendi
                                decimal.TryParse(row["Miktar"]?.ToString(), out miktar);

                                decimal stokMiktari = 0;
                                decimal.TryParse(row["Stok_Miktari"]?.ToString(), out stokMiktari);

                                decimal satisFiyati = 0;
                                decimal.TryParse(row["Satis_Fiyati"]?.ToString(), out satisFiyati);
                                decimal asgariStok = 0;
                                decimal.TryParse(row["AsgariStok"]?.ToString(), out asgariStok);
                                decimal ikinciSatisFiyati = 0;
                                decimal.TryParse(row["2SatisFiyati"]?.ToString(), out ikinciSatisFiyati);
                                decimal kdvOrani = 0;
                                decimal.TryParse(row["KDV_Orani"]?.ToString(), out kdvOrani);
                                decimal alisFiyati = 0;
                                decimal.TryParse(row["Alis_Fiyati"]?.ToString(), out alisFiyati);
                                decimal alisFiyati2 = 0;
                                if (alisFiyati > 0 && kdvOrani > 0)
                                {
                                    decimal kdvFaktoru = kdvOrani / 100m;
                                    alisFiyati2 = decimal.Round(alisFiyati / (1m + kdvFaktoru), 2, MidpointRounding.AwayFromZero);
                                }

                                if (!string.IsNullOrEmpty(urunGrubu))
                                {
                                    OleDbCommand checkGrupCmd = new OleDbCommand("SELECT COUNT(*) FROM ÜrünGrupEkle WHERE GrupAdi = @GrupAdi", baglan);
                                    checkGrupCmd.Parameters.AddWithValue("@GrupAdi", urunGrubu);
                                    if ((int)checkGrupCmd.ExecuteScalar() == 0)
                                    {
                                        OleDbCommand insertGrupCmd = new OleDbCommand("INSERT INTO ÜrünGrupEkle (GrupAdi) VALUES (@GrupAdi)", baglan);
                                        insertGrupCmd.Parameters.AddWithValue("@GrupAdi", urunGrubu);
                                        insertGrupCmd.ExecuteNonQuery();
                                    }
                                }

                                if (!string.IsNullOrEmpty(toptanciAdi))
                                {
                                    // ✅ Borç hesaplamasını stokMiktarı yerine Miktar ile yap
                                    decimal borcEklenecekTutar = alisFiyati * miktar;
                                    OleDbCommand toptanciCheck = new OleDbCommand("SELECT COUNT(*) FROM Toptancilar WHERE ToptanciAdi = @ToptanciAdi", baglan);
                                    toptanciCheck.Parameters.AddWithValue("@ToptanciAdi", toptanciAdi);
                                    if ((int)toptanciCheck.ExecuteScalar() > 0)
                                    {
                                        OleDbCommand updateBorc = new OleDbCommand("UPDATE Toptancilar SET ToplamBorc = ToplamBorc + @ToplamBorc, GsmTelefon = @GsmTelefon WHERE ToptanciAdi = @ToptanciAdi", baglan);
                                        updateBorc.Parameters.AddWithValue("@ToplamBorc", borcEklenecekTutar);
                                        updateBorc.Parameters.AddWithValue("@GsmTelefon", gsmTelefon);
                                        updateBorc.Parameters.AddWithValue("@ToptanciAdi", toptanciAdi);
                                        updateBorc.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        OleDbCommand insertToptanci = new OleDbCommand("INSERT INTO Toptancilar (ToptanciAdi, GsmTelefon, ToplamBorc) VALUES (@ToptanciAdi, @GsmTelefon, @ToplamBorc)", baglan);
                                        insertToptanci.Parameters.AddWithValue("@ToptanciAdi", toptanciAdi);
                                        insertToptanci.Parameters.AddWithValue("@GsmTelefon", gsmTelefon);
                                        insertToptanci.Parameters.AddWithValue("@ToplamBorc", borcEklenecekTutar);
                                        insertToptanci.ExecuteNonQuery();
                                    }
                                }

                                string sorgu = "INSERT INTO [ÜrünGirişi] (Barkod_No, Ürün_Adi, Ürün_Kodu, Ürün_Grubu, Stok_Miktari, OlcuBirimi, Satis_Fiyati, AsgariStok, [2SatisFiyati], Alis_Fiyati, Alis_Fiyati2, KDV_Orani, Toptanci_Adi, GsmTelefon, Tarih, Saat, IslemTuru, Miktar) VALUES (@Barkod_No, @Ürün_Adi, @Ürün_Kodu, @Ürün_Grubu, @Stok_Miktari, @OlcuBirimi, @Satis_Fiyati, @AsgariStok, @IkinciSatisFiyati, @Alis_Fiyati, @Alis_Fiyati2, @KDV_Orani, @Toptanci_Adi, @GsmTelefon, @Tarih, @Saat, @IslemTuru, @Miktar)";
                                OleDbCommand cmd = new OleDbCommand(sorgu, baglan);
                                cmd.Parameters.AddWithValue("@Barkod_No", barkodNo);
                                cmd.Parameters.AddWithValue("@Ürün_Adi", urunAdi);
                                cmd.Parameters.AddWithValue("@Ürün_Kodu", urunKodu);
                                cmd.Parameters.AddWithValue("@Ürün_Grubu", urunGrubu);
                                cmd.Parameters.AddWithValue("@Stok_Miktari", stokMiktari);
                                cmd.Parameters.AddWithValue("@OlcuBirimi", olcuBirimi);
                                cmd.Parameters.AddWithValue("@Satis_Fiyati", satisFiyati);
                                cmd.Parameters.AddWithValue("@AsgariStok", asgariStok);
                                cmd.Parameters.AddWithValue("@IkinciSatisFiyati", ikinciSatisFiyati);
                                cmd.Parameters.AddWithValue("@Alis_Fiyati", alisFiyati);
                                cmd.Parameters.AddWithValue("@Alis_Fiyati2", alisFiyati2);
                                cmd.Parameters.AddWithValue("@KDV_Orani", kdvOrani);
                                cmd.Parameters.AddWithValue("@Toptanci_Adi", toptanciAdi);
                                cmd.Parameters.AddWithValue("@GsmTelefon", gsmTelefon);
                                cmd.Parameters.AddWithValue("@Tarih", DateTime.Now.Date);
                                cmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToLongTimeString());
                                cmd.Parameters.AddWithValue("@IslemTuru", "Ürün Alışı");
                                cmd.Parameters.AddWithValue("@Miktar", miktar); // Miktar parametresi eklendi

                                cmd.ExecuteNonQuery();
                                eklenenUrunSayisi++;
                            }
                            catch (Exception)
                            {
                                atlananUrunSayisi++;
                                continue;
                            }
                        }
                    }
                    return new { eklenen = eklenenUrunSayisi, atlanan = atlananUrunSayisi };
                }, cts.Token);

                MessageBox.Show($"{sonuc.eklenen} ürün başarıyla eklendi. {sonuc.atlanan} ürün atlandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Yenile();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Genel Excel aktarımı sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar1.Visible = false;
                label33.Visible = false;
                button23.Visible = false;
                button16.Enabled = true;
                label33.ForeColor = System.Drawing.Color.DimGray;
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Dosyası|*.xlsx";
                sfd.FileName = "UrunListesi.xlsx";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                using (var workbook = new ClosedXML.Excel.XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Urun Listesi");

                    // Başlıklar
                    string[] basliklar = {
                "Barkod No", "Ürün Adı", "Ürün Kodu", "Ürün Grubu", "Stok Miktarı",
                "Ölçü Birimi", // ✅ Ölçü Birimi eklendi
                "Satış Fiyatı", "Asgari Stok", "İndirimli Fiyat", "Kdv Dahil", "Kdv Hariç",
                "KDV Oranı", "Toptancı Adı", "Gsm Telefon", "İşlem Türü", "Tarih", "Saat",
                "Miktar"
            };

                    for (int i = 0; i < basliklar.Length; i++)
                    {
                        var cell = ws.Cell(1, i + 1);
                        cell.Value = basliklar[i];

                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontColor = XLColor.White;
                        cell.Style.Fill.BackgroundColor = XLColor.DarkBlue;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                        cell.Style.Border.OutsideBorderColor = XLColor.Black;
                        cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.InsideBorderColor = XLColor.Gray;
                    }

                    ws.Row(1).Height = 28;

                    for (int i = 2; i <= 101; i++)
                    {
                        var row = ws.Row(i);
                        row.Height = 22;

                        if (i % 2 == 0)
                            row.Style.Fill.BackgroundColor = XLColor.FromHtml("#F3F3F3");

                        for (int j = 1; j <= basliklar.Length; j++)
                        {
                            var cell = ws.Cell(i, j);
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            cell.Style.Border.OutsideBorderColor = XLColor.Gray;
                            cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                            cell.Style.Border.InsideBorderColor = XLColor.LightGray;
                        }
                    }

                    ws.Columns().AdjustToContents();
                    workbook.SaveAs(sfd.FileName);
                }

                MessageBox.Show("Ürün Excel şablonu oluşturuldu. Bilgileri doldurup aktarabilirsiniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void button22_Click(object sender, EventArgs e)
        {
            Toplu_Ürün_Sil frm = new Toplu_Ürün_Sil();  // Form nesnesi oluştur
            frm.Show();                             // Yeni formu göster
            this.Hide();

        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
                button23.Visible = false;
                label33.Visible = false;
                progressBar1.Visible = false;
                MessageBox.Show("İşlem iptal edildi.");
            }
        }

       
    }
}
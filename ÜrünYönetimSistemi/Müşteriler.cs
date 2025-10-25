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
using DocumentFormat.OpenXml.Office2013.Drawing.Chart;

namespace ÜrünYönetimSistemi
{
    public partial class Müşteriler : Form
    {
        private string _mevcutMusteriGsm;
        public Form1 frm1;
        public Form2 frm2;
        private string _currentGsm;
        private DataTable tablo;
        private bool isCalculating = false;

        public Müşteriler()
        {

            InitializeComponent();
            // Toptancı bilgilerinin değiştirilmesini engelle

            textBox28.ReadOnly = true; // Toptancı Adı
            textBox3.ReadOnly = true; // GSM TelNo
            // Borç bilgilerinin değiştirilmesini engelle
            textBox4.ReadOnly = true; // Toplam Borç
            textBox27.ReadOnly = true; // Toplam Borç
            textBox19.ReadOnly = true; // Toplam Borç

           
            AllowDecimal(textBox33);
            AllowDecimal(textBox24);

            // Tarih ve saat bilgilerinin değiştirilmesini engelle
            textBox25.ReadOnly = true; // Tarih
            textBox26.ReadOnly = true; // Saat

            panel1.Visible = false;
            panel2.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button8.Visible = false;


            // Toptancı bilgilerinin değiştirilmesini engelle
            textBox13.ReadOnly = true; // Toptancı Adı
            textBox4.ReadOnly = true; // GSM TelNo


            // Borç bilgilerinin değiştirilmesini engelle
            textBox5.ReadOnly = true; // Toplam Borç


            // Tarih ve saat bilgilerinin değiştirilmesini engelle
            textBox6.ReadOnly = true; // Tarih
            textBox7.ReadOnly = true; // Saat




            dataGridView1.ReadOnly = true;

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            textBox1.ReadOnly = true;
            textBox18.KeyPress += TextBox_Sayi_KeyPress;
            textBox15.KeyPress += TextBox_Sayi_KeyPress;
            textBox21.KeyPress += TextBox_Sayi_KeyPress;
            textBox9.KeyPress += TextBox_Sayi_KeyPress;



            textBox10.Enter += PanelleriGizle;
            textBox9.Enter += PanelleriGizle;
            textBox11.Enter += PanelleriGizle;
            textBox12.Enter += PanelleriGizle;

            textBox14.Enter += PanelleriGizle;
            textBox15.Enter += PanelleriGizle;
            textBox16.Enter += PanelleriGizle;

            textBox18.Enter += PanelleriGizle;

            textBox20.Enter += PanelleriGizle;
            textBox21.Enter += PanelleriGizle;
            textBox22.Enter += PanelleriGizle;
            comboBox2.Enter += PanelleriGizle;

            label34.Visible = false;
            label35.Visible = false;
            textBox17.Visible= false;
            textBox8.Visible= false;

            TaksitBorcuGoster1();
            ToplamBorcuGoster1();
            MusterileriGetir();
          

        }
        private void PanelleriGizle(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
            dataGridView1.Visible = true;
            label1.Visible = true;
            textBox2.Visible = true;
        }
        // Form açıldığında verileri yükle
        private void Müşteriler_Load(object sender, EventArgs e)
        {

            string[] ulkeler = {
        "Afganistan", "Almanya", "Amerika Birleşik Devletleri", "Angola", "Arjantin",
        "Arnavutluk", "Avustralya", "Avusturya", "Azerbaycan", "Bangladeş", "Belarus",
        "Belçika", "Benin", "Birleşik Arap Emirlikleri", "Bolivya", "Bosna-Hersek",
        "Brezilya", "Bulgaristan", "Cezayir", "Çad", "Çek Cumhuriyeti", "Çin",
        "Danimarka", "Ekvador", "El Salvador", "Endonezya", "Estonya", "Etiyopya",
        "Fas", "Fildişi Sahili", "Filipinler", "Filistin", "Finlandiya", "Fransa",
        "Gabon", "Gana", "Gine", "Guatemala", "Güney Afrika", "Güney Kore", "Gürcistan",
        "Haiti", "Hırvatistan", "Hindistan", "Hollanda", "Honduras", "Irak", "İngiltere",
        "İran", "İrlanda", "İspanya", "İsrail", "İsveç", "İsviçre", "İtalya", "İzlanda",
        "Japonya", "Kamboçya", "Kamerun", "Kanada", "Karadağ", "Kazakistan", "Kenya",
        "Kıbrıs", "Kırgızistan", "Kolombiya", "Kongo", "Kosova", "Kosta Rika", "Kuba",
        "Kuveyt", "Kuzey Kore", "Kuzey Makedonya", "Letonya", "Liberya", "Libya",
        "Litvanya", "Lübnan", "Macaristan", "Madagaskar", "Malavi", "Malezya", "Mali",
        "Malta", "Meksika", "Mısır", "Moldova", "Moritanya", "Mozambik", "Nepal",
        "Nikaragua", "Nijerya", "Norveç", "Özbekistan", "Pakistan", "Panama", "Paraguay",
        "Peru", "Polonya", "Portekiz", "Romanya", "Ruanda", "Rusya",
        "Senegal", "Sırbistan", "Slovakya", "Slovenya", "Somali", "Sri Lanka",
        "Sudan", "Suriye", "Suudi Arabistan", "Şili", "Tacikistan", "Tanzanya",
        "Tayland", "Tunus", "Türkmenistan", "Türkiye", "Uganda", "Ukrayna", "Umman", "Uruguay",
        "Ürdün", "Vietnam", "Yemen", "Yeni Zelanda", "Yunanistan", "Zambiya", "Zimbabve"
    }; comboBox2.Items.AddRange(ulkeler);

            // 1. (ÖNEMLİ) Listeyi sadece listeden seçim yapılabilir hale getirin.
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;

            // 5 satır ve kenarlıklar için küçük bir boşluk (genellikle 2 piksel yeterlidir)
            // Bu kod, listenin yüksekliğini piksel cinsinden zorla 5 satırlık hale getirir.

            int istenenSatirSayisi = 7;

            // MaxDropDownItems ayarınızı zaten 5 yaptınız, ancak bu ayar görmezden gelindiği için
            // DropDownHeight özelliğini kod ile zorlayacağız.
            comboBox2.MaxDropDownItems = istenenSatirSayisi;

            // Yüksekliği hesaplayın: (İstenen Satır Sayısı * Her Satırın Yüksekliği) + Kenarlık Boşluğu
            comboBox2.DropDownHeight = (istenenSatirSayisi * comboBox2.ItemHeight) + 2;
            comboBox2.SelectedItem = "Türkiye";
            textBox7.Text = DateTime.Now.ToLongTimeString();
            // Diğer mevcut kodlarınız
            timer1.Enabled = true;
            textBox6.Text = DateTime.Now.ToShortDateString();
            textBox26.Text = DateTime.Now.ToShortDateString();
            textBox25.Text = DateTime.Now.ToLongTimeString();
            checkBox1.Checked = true;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox1.CheckedChanged += CheckBox_CheckedChanged;
            checkBox2.CheckedChanged += CheckBox_CheckedChanged;
            checkBox3.CheckedChanged += CheckBox_CheckedChanged;
            MusterileriGetir();
            ToplamBorcuGoster1();
            TaksitBorcuGoster1();
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
        // Veritabanından müşteri listesini DataGridView'e çeker
        private void MusterileriGetir()
        {
            try
            {
                string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=ÜrünYönetimSistemi.accdb;";
                string query = "SELECT MusteriAdi, GsmTelefon, DevredenBorc, Limit,Taksit, TicariUnvani, EMail, Vd, Vn, [Il/Ilce], Adres, Ulke, OzelNotlar FROM Musteriler";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection))
                    {
                        // Değişiklik burada: 'dataTable' yerine sınıf düzeyindeki 'tablo' değişkenini kullanın.
                        tablo = new DataTable();
                        adapter.Fill(tablo);
                        dataGridView1.DataSource = tablo;

                        // Sütun başlıklarını ve sıralamasını ayarla
                        dataGridView1.Columns["MusteriAdi"].HeaderText = "Müşteri Adı";
                        dataGridView1.Columns["GsmTelefon"].HeaderText = "GSM Telefon";
                        dataGridView1.Columns["DevredenBorc"].HeaderText = "Devreden Borç";
                        dataGridView1.Columns["Limit"].HeaderText = "Limit";
                        dataGridView1.Columns["Taksit"].HeaderText = "Taksit";
                        dataGridView1.Columns["TicariUnvani"].HeaderText = "Ticari Ünvanı";
                        dataGridView1.Columns["EMail"].HeaderText = "E-Mail";
                        dataGridView1.Columns["Vd"].HeaderText = "Vergi Dairesi";
                        dataGridView1.Columns["Vn"].HeaderText = "Vergi Numarası";
                        dataGridView1.Columns["Il/Ilce"].HeaderText = "İl / İlçe";
                        dataGridView1.Columns["Adres"].HeaderText = "Adres";
                        dataGridView1.Columns["Ulke"].HeaderText = "Ülke";
                        dataGridView1.Columns["OzelNotlar"].HeaderText = "Özel Notlar";

                        // Sütun sıralamasını ayarla (Sıra numarası 0'dan başlar)
                        dataGridView1.Columns["MusteriAdi"].DisplayIndex = 0;
                        dataGridView1.Columns["GsmTelefon"].DisplayIndex = 1;
                        dataGridView1.Columns["DevredenBorc"].DisplayIndex = 2;
                        dataGridView1.Columns["Limit"].DisplayIndex = 3;
                    }
                }
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
                    textBox13.Text = dataGridView1.Rows[selectedIndex].Cells["MusteriAdi"].Value?.ToString() ?? "";
                    textBox4.Text = dataGridView1.Rows[selectedIndex].Cells["GsmTelefon"].Value?.ToString() ?? "";
                    // Textbox'ları güncelle
                    textBox28.Text = dataGridView1.Rows[selectedIndex].Cells["MusteriAdi"].Value?.ToString() ?? "";
                    textBox3.Text = dataGridView1.Rows[selectedIndex].Cells["GsmTelefon"].Value?.ToString() ?? "";
                    decimal toplamBorc = 0;
                    if (decimal.TryParse(dataGridView1.Rows[selectedIndex].Cells["DevredenBorc"].Value?.ToString() ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out toplamBorc))
                    {
                        textBox5.Text = toplamBorc.ToString("N2");
                        textBox27.Text = toplamBorc.ToString("N2");

                    }
                    else
                    {
                        textBox5.Text = "0.00";
                        textBox27.Text = "0.00";

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri listesi yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // Sadece bir satır seçiliyse çalışsın
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridView1.SelectedRows[0].Index;

                // TextBox'ları güncelle
                textBox13.Text = dataGridView1.Rows[selectedIndex].Cells["MusteriAdi"].Value?.ToString() ?? "";
                textBox4.Text = dataGridView1.Rows[selectedIndex].Cells["GsmTelefon"].Value?.ToString() ?? "";

                textBox28.Text = dataGridView1.Rows[selectedIndex].Cells["MusteriAdi"].Value?.ToString() ?? "";
                textBox3.Text = dataGridView1.Rows[selectedIndex].Cells["GsmTelefon"].Value?.ToString() ?? "";

                decimal toplamBorc = 0;
                if (decimal.TryParse(dataGridView1.Rows[selectedIndex].Cells["DevredenBorc"].Value?.ToString() ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out toplamBorc))
                {
                    textBox5.Text = toplamBorc.ToString("N2");
                    textBox27.Text = toplamBorc.ToString("N2");
                }
                else
                {
                    textBox5.Text = "0.00";
                    textBox27.Text = "0.00";
                }
            }
        }
        // DataGridView'de bir satıra tıklandığında verileri ilgili TextBox'lara aktarır
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Sadece veri satırlarına tıklamayı etkinleştir.
            // Başlık satırına tıklamayı engelle (e.RowIndex < 0).
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];

            // Null kontrolü ekle
            if (row.Cells["GsmTelefon"].Value != null)
            {
                _currentGsm = row.Cells["GsmTelefon"].Value.ToString();
            }
            else
            {
                _currentGsm = string.Empty;
            }

            textBox10.Text = row.Cells["MusteriAdi"].Value?.ToString() ?? string.Empty;
            textBox11.Text = row.Cells["TicariUnvani"].Value?.ToString() ?? string.Empty;
            textBox12.Text = row.Cells["EMail"].Value?.ToString() ?? string.Empty;
            textBox14.Text = row.Cells["Vd"].Value?.ToString() ?? string.Empty;
            textBox15.Text = row.Cells["Vn"].Value?.ToString() ?? string.Empty;
            textBox22.Text = row.Cells["Il/Ilce"].Value?.ToString() ?? string.Empty;
            textBox16.Text = row.Cells["Adres"].Value?.ToString() ?? string.Empty;

            // Ulke sütunundaki veriyi al ve comboBox2'ye ata
            string ulke = row.Cells["Ulke"].Value?.ToString() ?? string.Empty;
            int ulkeIndex = comboBox2.Items.IndexOf(ulke);
            if (ulkeIndex != -1)
            {
                comboBox2.SelectedIndex = ulkeIndex;
            }
            else
            {
                comboBox2.SelectedIndex = -1; // Varsayılan olarak seçim kaldır
            }

            textBox18.Text = row.Cells["GsmTelefon"].Value?.ToString() ?? string.Empty;
            textBox21.Text = row.Cells["DevredenBorc"].Value?.ToString() ?? "0.00";
            textBox9.Text = row.Cells["Limit"].Value?.ToString() ?? "0.00";
            textBox8.Text = row.Cells["Taksit"].Value?.ToString() ?? "0.00";
            textBox20.Text = row.Cells["OzelNotlar"].Value?.ToString() ?? string.Empty;

            // DevredenBorc ve Taksit toplamını hesapla ve textBox17'ye yaz
            try
            {
                using (OleDbConnection baglan = new OleDbConnection(
                    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan.Open();
                    string query = "SELECT DevredenBorc, Taksit FROM Musteriler WHERE GsmTelefon = @GsmTelefon";
                    OleDbCommand kmt = new OleDbCommand(query, baglan);
                    kmt.Parameters.AddWithValue("@GsmTelefon", _currentGsm);

                    using (OleDbDataReader reader = kmt.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal devredenBorc = 0, taksit = 0;

                            // DevredenBorc kontrolü ve dönüşümü
                            if (reader["DevredenBorc"] != DBNull.Value)
                            {
                                string borcMetni = reader["DevredenBorc"].ToString();
                                decimal.TryParse(borcMetni.Replace(",", "."),
                                    System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    out devredenBorc);
                            }

                            // Taksit kontrolü ve dönüşümü
                            if (reader["Taksit"] != DBNull.Value)
                            {
                                string taksitMetni = reader["Taksit"].ToString();
                                decimal.TryParse(taksitMetni.Replace(",", "."),
                                    System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    out taksit);
                            }

                            // Toplamı hesapla ve textBox17'ye yaz
                            decimal toplam = devredenBorc + taksit;
                            textBox17.Text = toplam.ToString("N2");
                        }
                        else
                        {
                            textBox17.Text = "0.00"; // Kayıt bulunmazsa 0.00 göster
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toplam borç hesaplanırken bir hata oluştu: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox17.Text = "0.00"; // Hata durumunda 0.00 göster
            }

            button5.Visible = true;
            button3.Visible = true;
            button10.Visible = false;
            button1.Visible = true;
            button2.Visible = true;
            button4.Visible = true;
            button8.Visible = true;

            textBox9.ReadOnly = true;
            textBox21.ReadOnly = true;
            textBox8.ReadOnly = true;
            textBox17.ReadOnly = true;
            label24.Text = "Veresiye Toplamı";
            label12.Text = "Belirlenen Limit";
            label34.Visible =true;
            label35.Visible = true;
            textBox17.Visible = true;
            textBox8.Visible = true;




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
        // Yeni müşteri ekleme işlemi
        private void button10_Click(object sender, EventArgs e)
        {
            // Zorunlu alanların ayrı ayrı kontrolü
            if (string.IsNullOrWhiteSpace(textBox10.Text))
            {
                MessageBox.Show("Müşteri Adı alanı boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox18.Text))
            {
                MessageBox.Show("GSM Telefonu alanı boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // GSM numarası format kontrolü
            if (textBox18.Text.Length != 10)
            {
                MessageBox.Show("GSM Telefon numarası 10 karakterli olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // E-posta format kontrolü (boş değilse)
            if (!string.IsNullOrWhiteSpace(textBox12.Text))
            {
                try
                {
                    var email = new System.Net.Mail.MailAddress(textBox12.Text);
                }
                catch
                {
                    MessageBox.Show("Geçerli bir e-posta adresi giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Vergi Numarası format kontrolü (boş değilse)
            if (!string.IsNullOrWhiteSpace(textBox15.Text))
            {
                string vn = textBox15.Text;
                if (!(vn.Length == 10 || vn.Length == 11) || !vn.All(char.IsDigit))
                {
                    MessageBox.Show("Vergi Numarası 10 veya 11 haneli sayısal bir değer olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Sayısal alanların dönüştürülmesi ve kontrolü
            decimal devredenBorc = 0;
            decimal limit = 0;
            decimal taksit = 0; // Taksit varsayılan olarak 0 bırakılmıştır.

            // ✅ DEVREDEN BORÇ İÇİN GÜNCEL UYARI KONTROLÜ
            if (!string.IsNullOrWhiteSpace(textBox21.Text))
            {
                string borcStr = textBox21.Text.Trim();
                if (borcStr.Contains(","))
                {
                    if (borcStr.Split(',')[1].Length > 2)
                    {
                        string dogruFormat = borcStr.Replace(",", "");
                        MessageBox.Show($"Lütfen 'Devreden Borç' için {dogruFormat} veya {dogruFormat},00 TL formatını kullanın. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (!decimal.TryParse(borcStr.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out devredenBorc))
                {
                    MessageBox.Show("Devreden Borç geçerli bir sayısal değer olmalıdır. (Örn: 1250,50 veya 1.250,50)", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // ✅ LİMİT İÇİN GÜNCEL UYARI KONTROLÜ
            if (!string.IsNullOrWhiteSpace(textBox9.Text))
            {
                string limitStr = textBox9.Text.Trim();
                if (limitStr.Contains(","))
                {
                    if (limitStr.Split(',')[1].Length > 2)
                    {
                        string dogruFormat = limitStr.Replace(",", "");
                        MessageBox.Show($"Lütfen 'Limit' için {dogruFormat} veya {dogruFormat},00 TL formatını kullanın. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (!decimal.TryParse(limitStr.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out limit))
                {
                    MessageBox.Show("Limit geçerli bir sayısal değer olmalıdır. (Örn: 1000,00 veya 1.000,00)", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=ÜrünYönetimSistemi.accdb;";
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    bool gsmExists = false;
                    bool emailExists = false;

                    string checkGsmQuery = "SELECT COUNT(*) FROM Musteriler WHERE GsmTelefon = ?";
                    using (OleDbCommand checkGsmCommand = new OleDbCommand(checkGsmQuery, connection))
                    {
                        checkGsmCommand.Parameters.AddWithValue("@GsmTelefon", textBox18.Text);
                        if ((int)checkGsmCommand.ExecuteScalar() > 0)
                        {
                            gsmExists = true;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(textBox12.Text))
                    {
                        string checkEmailQuery = "SELECT COUNT(*) FROM Musteriler WHERE EMail = ?";
                        using (OleDbCommand checkEmailCommand = new OleDbCommand(checkEmailQuery, connection))
                        {
                            checkEmailCommand.Parameters.AddWithValue("@EMail", textBox12.Text);
                            if ((int)checkEmailCommand.ExecuteScalar() > 0)
                            {
                                emailExists = true;
                            }
                        }
                    }

                    if (gsmExists && emailExists)
                    {
                        MessageBox.Show("Girdiğiniz GSM Numarası ve E-posta adresi zaten kayıtlıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else if (gsmExists)
                    {
                        MessageBox.Show("Girdiğiniz GSM Numarası zaten kayıtlıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else if (emailExists)
                    {
                        MessageBox.Show("Girdiğiniz E-posta adresi zaten kayıtlıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string insertQuery = "INSERT INTO Musteriler (MusteriAdi, TicariUnvani, EMail, Vd, Vn, [Il/Ilce], Adres, Ulke, GsmTelefon, DevredenBorc, Taksit, Limit, OzelNotlar) " +
                                         "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    using (OleDbCommand insertCommand = new OleDbCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@MusteriAdi", textBox10.Text);
                        insertCommand.Parameters.AddWithValue("@TicariUnvani", string.IsNullOrWhiteSpace(textBox11.Text) ? (object)DBNull.Value : textBox11.Text);
                        insertCommand.Parameters.AddWithValue("@EMail", string.IsNullOrWhiteSpace(textBox12.Text) ? (object)DBNull.Value : textBox12.Text);
                        insertCommand.Parameters.AddWithValue("@Vd", string.IsNullOrWhiteSpace(textBox14.Text) ? (object)DBNull.Value : textBox14.Text);
                        insertCommand.Parameters.AddWithValue("@Vn", string.IsNullOrWhiteSpace(textBox15.Text) ? (object)DBNull.Value : textBox15.Text);
                        insertCommand.Parameters.AddWithValue("@Il/Ilce", string.IsNullOrWhiteSpace(textBox22.Text) ? (object)DBNull.Value : textBox22.Text);
                        insertCommand.Parameters.AddWithValue("@Adres", string.IsNullOrWhiteSpace(textBox16.Text) ? (object)DBNull.Value : textBox16.Text);

                        // Hatanın Kaynağı Olan 8. Parametre (Ulke) Düzeltildi
                        string ulkeValue = comboBox2.SelectedItem?.ToString();
                        insertCommand.Parameters.AddWithValue("@Ulke", string.IsNullOrWhiteSpace(ulkeValue) ? (object)DBNull.Value : (object)ulkeValue);

                        insertCommand.Parameters.AddWithValue("@GsmTelefon", textBox18.Text);
                        insertCommand.Parameters.AddWithValue("@DevredenBorc", devredenBorc);
                        insertCommand.Parameters.AddWithValue("@Taksit", taksit);
                        insertCommand.Parameters.AddWithValue("@Limit", limit);
                        insertCommand.Parameters.AddWithValue("@OzelNotlar", string.IsNullOrWhiteSpace(textBox20.Text) ? (object)DBNull.Value : textBox20.Text);

                        insertCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Müşteri başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Alanları temizleme
                    textBox10.Clear();
                    textBox11.Clear();
                    textBox12.Clear();
                    textBox14.Clear();
                    textBox15.Clear();
                    textBox22.Clear();
                    textBox16.Clear();
                    comboBox2.SelectedIndex = -1;
                    textBox18.Clear();
                    textBox21.Clear();
                    textBox9.Clear();
                    textBox20.Clear();
                    MusterileriGetir();
                    ToplamBorcuGoster1();
                    TaksitBorcuGoster1();
                    comboBox2.SelectedItem = "Türkiye";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri eklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Mevcut müşteriyi güncelleme işlemi
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentGsm))
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz müşteriyi listeden seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox10.Text))
            {
                MessageBox.Show("Müşteri Adı alanı boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (textBox18.Text.Length != 10)
            {
                MessageBox.Show("GSM Telefon numarası 10 karakterli olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(textBox12.Text))
            {
                try
                {
                    var email = new System.Net.Mail.MailAddress(textBox12.Text);
                }
                catch
                {
                    MessageBox.Show("Geçerli bir e-posta adresi giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(textBox15.Text))
            {
                string vn = textBox15.Text;
                if (!(vn.Length == 10 || vn.Length == 11) || !vn.All(char.IsDigit))
                {
                    MessageBox.Show("Vergi Numarası 10 veya 11 haneli sayısal bir değer olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            decimal devredenBorc = 0;
            decimal limit = 0;

            // ✅ Devreden Borç için format kontrolü
            string devredenBorcStr = textBox21.Text.Trim();
            if (!string.IsNullOrEmpty(devredenBorcStr))
            {
                if (devredenBorcStr.Contains(",") && devredenBorcStr.Split(',')[1].Length > 2)
                {
                    string dogruFormat = devredenBorcStr.Replace(",", "");
                    MessageBox.Show($"Lütfen 'Devreden Borç' için {dogruFormat} veya {dogruFormat},00 TL formatını kullanın. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!decimal.TryParse(devredenBorcStr.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out devredenBorc))
                {
                    MessageBox.Show("Devreden Borç geçerli bir sayısal değer olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // ✅ Limit için format kontrolü
            string limitStr = textBox9.Text.Trim();
            if (!string.IsNullOrEmpty(limitStr))
            {
                if (limitStr.Contains(",") && limitStr.Split(',')[1].Length > 2)
                {
                    string dogruFormat = limitStr.Replace(",", "");
                    MessageBox.Show($"Lütfen 'Limit' için {dogruFormat} veya {dogruFormat},00 TL formatını kullanın. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!decimal.TryParse(limitStr.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out limit))
                {
                    MessageBox.Show("Limit geçerli bir sayısal değer olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Güncelleme işlemi
            try
            {
                string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=ÜrünYönetimSistemi.accdb;";
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    // Sadece yeni GSM'in veritabanında başka bir kayda ait olup olmadığını kontrol et
                    if (textBox18.Text != _currentGsm)
                    {
                        string checkNewGsmQuery = "SELECT COUNT(*) FROM Musteriler WHERE GsmTelefon = ?";
                        using (OleDbCommand checkCommand = new OleDbCommand(checkNewGsmQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@GsmTelefon", textBox18.Text);
                            int existingRecords = (int)checkCommand.ExecuteScalar();
                            if (existingRecords > 0)
                            {
                                MessageBox.Show("Güncellemek istediğiniz yeni GSM numarası zaten başka bir müşteriye ait.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }

                    string updateQuery = "UPDATE Musteriler SET MusteriAdi = ?, TicariUnvani = ?, EMail = ?, Vd = ?, Vn = ?, [Il/Ilce] = ?, Adres = ?, Ulke = ?, GsmTelefon = ?, DevredenBorc = ?, Limit = ?, OzelNotlar = ? WHERE GsmTelefon = ?";

                    using (OleDbCommand updateCommand = new OleDbCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@MusteriAdi", textBox10.Text);
                        updateCommand.Parameters.AddWithValue("@TicariUnvani", string.IsNullOrWhiteSpace(textBox11.Text) ? (object)DBNull.Value : textBox11.Text);
                        updateCommand.Parameters.AddWithValue("@EMail", string.IsNullOrWhiteSpace(textBox12.Text) ? (object)DBNull.Value : textBox12.Text);
                        updateCommand.Parameters.AddWithValue("@Vd", string.IsNullOrWhiteSpace(textBox14.Text) ? (object)DBNull.Value : textBox14.Text);
                        updateCommand.Parameters.AddWithValue("@Vn", string.IsNullOrWhiteSpace(textBox15.Text) ? (object)DBNull.Value : textBox15.Text);
                        updateCommand.Parameters.AddWithValue("@Il/Ilce", string.IsNullOrWhiteSpace(textBox22.Text) ? (object)DBNull.Value : textBox22.Text);
                        updateCommand.Parameters.AddWithValue("@Adres", string.IsNullOrWhiteSpace(textBox16.Text) ? (object)DBNull.Value : textBox16.Text);
                        updateCommand.Parameters.AddWithValue("@Ulke", comboBox2.SelectedItem?.ToString());
                        updateCommand.Parameters.AddWithValue("@GsmTelefon", textBox18.Text);
                        updateCommand.Parameters.AddWithValue("@DevredenBorc", devredenBorc);
                        updateCommand.Parameters.AddWithValue("@Limit", limit);
                        updateCommand.Parameters.AddWithValue("@OzelNotlar", string.IsNullOrWhiteSpace(textBox20.Text) ? (object)DBNull.Value : textBox20.Text);
                        updateCommand.Parameters.AddWithValue("@OldGsmTelefon", _currentGsm); // Burada eski GSM'i kullanıyoruz

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Müşteri bilgileri başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            panel1.Visible = false;
                            panel2.Visible = false;
                            textBox10.Text = string.Empty;
                            textBox11.Text = string.Empty;
                            textBox12.Text = string.Empty;
                            textBox14.Text = string.Empty;
                            textBox15.Text = string.Empty;
                            textBox16.Text = string.Empty;
                            textBox22.Text = string.Empty;
                            textBox18.Text = string.Empty;
                            textBox21.Text = string.Empty;
                            textBox9.Text = string.Empty;
                            textBox20.Text = string.Empty;
                            label1.Visible = true;
                            textBox2.Visible = true;
                            textBox7.Text = "";
                            textBox33.Text = "";
                            textBox23.Text = "";
                            textBox24.Text = "";
                            button1.Visible = false;
                            button2.Visible = false;
                            button3.Visible = false;
                            button4.Visible = false;
                            button5.Visible = false;
                            button8.Visible = false;
                            button10.Visible = true;
                            dataGridView1.Visible = true;

                            textBox9.ReadOnly = false;
                            textBox21.ReadOnly = false;
                            textBox8.ReadOnly = true;
                            textBox17.ReadOnly = true;
                            label24.Text = "Devreden Borç";
                            label12.Text = "Limit Belirle";
                            label34.Visible = false;
                            label35.Visible = false;
                            textBox17.Visible = false;
                            textBox8.Visible = false;


                            // Temizleme sonrası varsayılan ülke olarak Türkiye'yi seçer
                            comboBox2.SelectedItem = "Türkiye";

                            MusterileriGetir();
                            ToplamBorcuGoster1();
                            TaksitBorcuGoster1();
                        }
                        else
                        {
                            MessageBox.Show("Güncellenecek müşteri bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri bilgileri güncellenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Müşteriyi silme işlemi
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox18.Text))
            {
                MessageBox.Show("Lütfen silmek istediğiniz müşterinin GSM numarasını giriniz veya seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bu müşteriyi silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return;
            }

            try
            {
                string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=ÜrünYönetimSistemi.accdb;";
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    string gsm = textBox18.Text;
                    string checkQuery = "SELECT COUNT(*) FROM Musteriler WHERE GsmTelefon = ?";
                    using (OleDbCommand checkCommand = new OleDbCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@GsmTelefon", gsm);
                        int existingRecords = (int)checkCommand.ExecuteScalar();
                        if (existingRecords == 0)
                        {
                            MessageBox.Show("Silinecek müşteri, girdiğiniz GSM Numarası ile bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    string deleteQuery = "DELETE FROM Musteriler WHERE GsmTelefon = ?";
                    using (OleDbCommand deleteCommand = new OleDbCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@GsmTelefon", gsm);

                        int rowsAffected = deleteCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Müşteri başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            button12_Click(sender, e);


                            // Temizleme sonrası varsayılan ülke olarak Türkiye'yi seçer
                            comboBox2.SelectedItem = "Türkiye";



                            panel1.Visible = false;
                            panel2.Visible = false;
                            textBox10.Text = string.Empty;
                            textBox11.Text = string.Empty;
                            textBox12.Text = string.Empty;
                            textBox14.Text = string.Empty;
                            textBox15.Text = string.Empty;
                            textBox16.Text = string.Empty;
                            textBox22.Text = string.Empty;
                            textBox18.Text = string.Empty;
                            textBox21.Text = string.Empty;
                            textBox9.Text = string.Empty;
                            textBox20.Text = string.Empty;
                            label1.Visible = true;
                            textBox2.Visible = true;
                            textBox7.Text = "";
                            textBox33.Text = "";
                            textBox23.Text = "";
                            textBox24.Text = "";
                            button1.Visible = false;
                            button2.Visible = false;
                            button3.Visible = false;
                            button4.Visible = false;
                            button5.Visible = false;
                            button8.Visible = false;
                            button10.Visible = true;
                            dataGridView1.Visible = true;

                            textBox9.ReadOnly = false;
                            textBox21.ReadOnly = false;
                            textBox8.ReadOnly = true;
                            textBox17.ReadOnly = true;
                            label24.Text = "Devreden Borç";
                            label12.Text = "Limit Belirle";
                            label34.Visible = false;
                            label35.Visible = false;
                            textBox17.Visible = false;
                            textBox8.Visible = false;


                            // Temizleme sonrası varsayılan ülke olarak Türkiye'yi seçer
                            comboBox2.SelectedItem = "Türkiye";
                            MusterileriGetir();
                            ToplamBorcuGoster1(); TaksitBorcuGoster1();
                        }
                        else
                        {
                            MessageBox.Show("Müşteri silinirken bir sorun oluştu. Lütfen tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri silinirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tüm TextBox'ları ve ComboBox'ı temizler
        private void button12_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
            textBox10.Text = string.Empty;
            textBox11.Text = string.Empty;
            textBox12.Text = string.Empty;
            textBox14.Text = string.Empty;
            textBox15.Text = string.Empty;
            textBox16.Text = string.Empty;
            textBox22.Text = string.Empty;
            textBox18.Text = string.Empty;
            textBox21.Text = string.Empty;
            textBox9.Text = string.Empty;
            textBox20.Text = string.Empty;
            label1.Visible = true;
            textBox2.Visible = true;
            textBox7.Text = "";
            textBox33.Text = "";
            textBox23.Text = "";
            textBox24.Text = "";
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button8.Visible = false;
            button10.Visible = true;
            dataGridView1.Visible = true;

            textBox9.ReadOnly = false;
            textBox21.ReadOnly = false;
            textBox8.ReadOnly = true;
            textBox17.ReadOnly = true;
            label24.Text = "Devreden Borç";
            label12.Text = "Limit Belirle";
            label34.Visible = false;
            label35.Visible = false;
            textBox17.Visible = false;
            textBox8.Visible = false;
            // Temizleme sonrası varsayılan ülke olarak Türkiye'yi seçer
            comboBox2.SelectedItem = "Türkiye";


        }
        private void TextBox_Harf_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harf, boşluk ve kontrol tuşlarına izin ver.
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void textBox22_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harf, boşluk, eğik çizgi (/) ve kontrol tuşlarına izin ver.
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ' && e.KeyChar != '/')
            {
                e.Handled = true;
            }
        }
        private void TextBox_Sayi_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;

            // Ortak: sadece rakam, backspace ve virgül
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true;
                return;
            }

            // GSM (textBox18) ve Vergi No (textBox15) için: sadece rakam
            if (txt == textBox18 || txt == textBox15)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
                return; // Burada çık, diğer kontrolleri yapma
            }

            // Devreden Borç (textBox21) ve Limit (textBox9) için
            if (txt == textBox21 || txt == textBox9)
            {
                // Birden fazla virgül engelle
                if (e.KeyChar == ',' && txt.Text.Contains(","))
                {
                    e.Handled = true;
                }

                // İlk karakter virgül olamaz
                if (e.KeyChar == ',' && txt.SelectionStart == 0)
                {
                    e.Handled = true;
                }
            }
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
                tablo.DefaultView.RowFilter = $"MusteriAdi LIKE '%{filtre}%' OR GsmTelefon LIKE '%{filtre}%'";
            }

        }
        private void ToplamBorcuGoster1()
        {
            try
            {
                using (OleDbConnection baglan9 = new OleDbConnection(
                                "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan9.Open();
                    // Tüm toptancıların borçlarını tek tek çekiyoruz
                    string query = "SELECT DevredenBorc FROM Musteriler";
                    OleDbCommand kmt = new OleDbCommand(query, baglan9);
                    decimal toplamBorc = 0;
                    using (OleDbDataReader reader = kmt.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Her bir borç değerini alıp string'den decimal'e dönüştürüyoruz
                            if (reader["DevredenBorc"] != DBNull.Value)
                            {
                                string borcMetni = reader["DevredenBorc"].ToString();
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

                    textBox1.Text = toplamBorc.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toplam Devreden Borc hesaplanırken bir hata oluştu: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void TaksitBorcuGoster1()
        {
            try
            {
                using (OleDbConnection baglan9 = new OleDbConnection(
                                "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan9.Open();
                    // Tüm toptancıların borçlarını tek tek çekiyoruz
                    string query = "SELECT Taksit FROM Musteriler";
                    OleDbCommand kmt = new OleDbCommand(query, baglan9);
                    decimal toplamBorc = 0;
                    using (OleDbDataReader reader = kmt.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Her bir borç değerini alıp string'den decimal'e dönüştürüyoruz
                            if (reader["Taksit"] != DBNull.Value)
                            {
                                string borcMetni = reader["Taksit"].ToString();
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

                    textBox19.Text = toplamBorc.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Toplam Devreden Borc hesaplanırken bir hata oluştu: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            textBox7.Text = DateTime.Now.ToLongTimeString();
            textBox25.Text = DateTime.Now.ToLongTimeString();
            // Timer'ı başlat
            timer1.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Seçili bir toptancı varsa
            if (dataGridView1.CurrentRow != null)
            {
                // DataGridView'deki güncel bilgileri al
                string musteriAdi = dataGridView1.CurrentRow.Cells["MusteriAdi"].Value?.ToString() ?? "";
                string gsmTelefon = dataGridView1.CurrentRow.Cells["GsmTelefon"].Value?.ToString() ?? "";

                // TextBox'lardaki mevcut bilgileri al ve boşlukları temizle
                string textBoxAdi = textBox10.Text.Trim();
                string textBoxGsm = textBox18.Text.Trim();

                // Toptancı adı ve GSM numarasının değişip değişmediğini kontrol et
                bool adiDegisti = textBoxAdi != musteriAdi;
                bool gsmDegisti = textBoxGsm != gsmTelefon;

                if (adiDegisti && gsmDegisti)
                {
                    MessageBox.Show("Müşteri adı ve GSM numarası değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgileri kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (adiDegisti)
                {
                    MessageBox.Show("Müşteri adı değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgiyi kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (gsmDegisti)
                {
                    MessageBox.Show("Müşteri GSM numarası değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgiyi kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // --- Eğer yukarıdaki kontrollerden geçilirse, kod buradan devam eder ---

            panel1.Visible = true;
            panel2.Visible = false;
            dataGridView1.Visible = false;
            textBox2.Visible = false;
            label1.Visible = false;

            // Seçili bir toptancı varsa, bilgilerini Borç Ekleme paneline aktar
            if (dataGridView1.CurrentRow != null)
            {
                // MusteriAdi bilgisini doğru TextBox'tan al
                string guncelMusteriAdi = textBox13.Text.Trim();
                string guncelGsmTelefon = textBox18.Text.Trim();

                // Bilgileri Borç Ekleme paneline aktar
                textBox3.Text = guncelMusteriAdi;
                textBox4.Text = guncelGsmTelefon;
            }

            // Temizlik ve saat ataması
            textBox33.Text = string.Empty;
            textBox32.Text = string.Empty;
            textBox7.Text = DateTime.Now.ToLongTimeString();

        }
        private void button3_Click(object sender, EventArgs e)
        {
            // Seçili bir toptancı varsa
            if (dataGridView1.CurrentRow != null)
            {
                // DataGridView'deki güncel bilgileri al
                string musteriAdi = dataGridView1.CurrentRow.Cells["MusteriAdi"].Value?.ToString() ?? "";
                string gsmTelefon = dataGridView1.CurrentRow.Cells["GsmTelefon"].Value?.ToString() ?? "";

                // TextBox'lardaki mevcut bilgileri al ve boşlukları temizle
                string textBoxAdi = textBox10.Text.Trim();
                string textBoxGsm = textBox18.Text.Trim();

                // Toptancı adı ve GSM numarasının değişip değişmediğini kontrol et
                bool adiDegisti = textBoxAdi != musteriAdi;
                bool gsmDegisti = textBoxGsm != gsmTelefon;

                if (adiDegisti && gsmDegisti)
                {
                    MessageBox.Show("Müşteri adı ve GSM numarası değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgileri kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (adiDegisti)
                {
                    MessageBox.Show("Müşteri adı değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgiyi kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (gsmDegisti)
                {
                    MessageBox.Show("Müşteri GSM numarası değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgiyi kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // --- Eğer yukarıdaki kontrollerden geçilirse, kod buradan devam eder ---

            // Panel görünürlüklerini ayarla
            panel2.Visible = true;
            panel1.Visible = false;
            dataGridView1.Visible = false;
            textBox2.Visible = false;
            label1.Visible = false;

            // Seçili bir toptancı varsa, bilgilerini Borç Ödeme paneline aktar
            if (dataGridView1.CurrentRow != null)
            {
                // TextBox'lardan güncel değerleri al (kullanıcı girişini dikkate al)
                string guncelMusteriAdi = textBox10.Text.Trim();
                string guncelGsmTelefon = textBox18.Text.Trim();

                // Bilgileri Borç Ödeme paneline aktar
                textBox28.Text = guncelMusteriAdi;
                textBox3.Text = guncelGsmTelefon;
            }

            // Temizlik
            textBox23.Text = string.Empty;
            textBox24.Text = string.Empty;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            // 1️⃣ Zorunlu kontroller
            if (string.IsNullOrEmpty(textBox10.Text) || string.IsNullOrEmpty(textBox33.Text) || string.IsNullOrEmpty(textBox18.Text))
            {
                MessageBox.Show("Lütfen bir Müşteri seçin, GSM numarasını ve eklenecek tutarı girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string musteriAdi = textBox10.Text;
            string gsmTelefon = textBox18.Text;
            string aciklama;

            if (string.IsNullOrWhiteSpace(textBox32.Text))
            {
                aciklama = "Müşteri Borcuna Ekleme Yapıldı";
            }
            else
            {
                aciklama = "Borç Ekleme - " + textBox32.Text;
            }

            DateTime anlikZaman = DateTime.Now;

            // 2️⃣ Eklenen tutarı decimal olarak al
            decimal eklenecekTutar = 0;
            string eklenecekTutarStr = textBox33.Text.Trim();

            // ✅ button14_Click metodundaki hata kontrolü buraya uygulandı
            if (eklenecekTutarStr.Contains(",") && eklenecekTutarStr.Split(',')[1].Length > 2)
            {
                string dogruFormat = eklenecekTutarStr.Replace(",", "");
                MessageBox.Show($"Lütfen 'Eklenecek Tutar' için {dogruFormat} veya {dogruFormat},00 TL formatını kullanın. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Doğru parse işlemi
            if (!decimal.TryParse(eklenecekTutarStr.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out eklenecekTutar))
            {
                MessageBox.Show("Lütfen geçerli bir tutar girin. (Örn: 1250,50 veya 1.250,50)", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (eklenecekTutar <= 0)
            {
                MessageBox.Show("Eklenecek tutar sıfırdan büyük bir değer olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // 3️⃣ Mevcut borcu textBox21'den al
                decimal mevcutBorc = 0;
                if (!string.IsNullOrEmpty(textBox21.Text))
                {
                    if (!decimal.TryParse(textBox21.Text.Replace(".", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out mevcutBorc))
                    {
                        MessageBox.Show("Mevcut borç geçerli bir sayısal değer değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Veritabanı adı kontrol edildi: ÜrünYönetimSistemi.accdb
                using (OleDbConnection baglan4 = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    baglan4.Open();

                    // 4️⃣ Yeni toplam borcu hesapla
                    decimal yeniToplamBorc = mevcutBorc + eklenecekTutar;

                    // 5️⃣ Sadece seçilen GSM numarasına özel güncelle
                    string updateQuery = "UPDATE Musteriler SET DevredenBorc=@yeniDevredenBorc WHERE GsmTelefon=@gsmTelefon";
                    using (OleDbCommand cmdUpdate = new OleDbCommand(updateQuery, baglan4))
                    {
                        cmdUpdate.Parameters.Add("@yeniDevredenBorc", OleDbType.Currency).Value = yeniToplamBorc;
                        cmdUpdate.Parameters.Add("@gsmTelefon", OleDbType.VarWChar, 255).Value = gsmTelefon;
                        cmdUpdate.ExecuteNonQuery();
                    }

                    // 6️⃣ Borç ekleme kaydını yine sadece o GSM için ekle
                    string insertQuery = "INSERT INTO VeresiyeEkle (MusteriAdi, GsmTelefon, EskiBorc, ToplamBorc, [Tarih/Saat], EklenenTutar, Aciklama) " +
                                         "VALUES (@MusteriAdi, @gsmTelefon, @eskiBorc, @toplamBorc, @tarihsaat, @eklenecekTutar, @aciklama)";
                    using (OleDbCommand cmdInsert = new OleDbCommand(insertQuery, baglan4))
                    {
                        cmdInsert.Parameters.Add("@MusteriAdi", OleDbType.VarWChar, 255).Value = musteriAdi;
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

                    panel1.Visible = false;
                    panel2.Visible = false;
                    textBox10.Text = string.Empty;
                    textBox11.Text = string.Empty;
                    textBox12.Text = string.Empty;
                    textBox14.Text = string.Empty;
                    textBox15.Text = string.Empty;
                    textBox16.Text = string.Empty;
                    textBox22.Text = string.Empty;
                    textBox18.Text = string.Empty;
                    textBox21.Text = string.Empty;
                    textBox9.Text = string.Empty;
                    textBox20.Text = string.Empty;
                    label1.Visible = true;
                    textBox2.Visible = true;
                    textBox7.Text = "";
                    textBox33.Text = "";
                    textBox23.Text = "";
                    textBox24.Text = "";
                    button1.Visible = false;
                    button2.Visible = false;
                    button3.Visible = false;
                    button4.Visible = false;
                    button5.Visible = false;
                    button8.Visible = false;
                    button10.Visible = true;
                    dataGridView1.Visible = true;

                    textBox9.ReadOnly = false;
                    textBox21.ReadOnly = false;
                    textBox8.ReadOnly = true;
                    textBox17.ReadOnly = true;
                    label24.Text = "Devreden Borç";
                    label12.Text = "Limit Belirle";
                    label34.Visible = false;
                    label35.Visible = false;
                    textBox17.Visible = false;
                    textBox8.Visible = false;
                    comboBox2.SelectedItem = "Türkiye";


                    string selectedGsm = gsmTelefon;
                    MusterileriGetir();
                    ToplamBorcuGoster1();
                    TaksitBorcuGoster1();
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

                    MessageBox.Show("Borç sadece seçili Müşteri için eklendi ve Devredilen Borç güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button14_Click(object sender, EventArgs e)
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
            if (!string.IsNullOrEmpty(odenenTutarStr))
            {
                if (odenenTutarStr.Contains(",") && odenenTutarStr.Split(',')[1].Length > 2)
                {
                    string dogruFormat = odenenTutarStr.Replace(",", "");
                    MessageBox.Show($"Lütfen 'Ödenecek Tutar' için {dogruFormat} veya {dogruFormat},00 TL formatını kullanın. Virgül (,) binlik ayıracı olarak kullanılamaz.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
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
                    $"Müşteriye Ödeme - {odemeSekli}" :
                    $"Müşteriye Ödeme - {odemeSekli} - {textBox23.Text.Trim()}";
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

                    string updateQuery = "UPDATE Musteriler SET DevredenBorc = ? WHERE GsmTelefon = ?";
                    using (OleDbCommand cmdUpdate = new OleDbCommand(updateQuery, baglan1))
                    {
                        cmdUpdate.Parameters.Add("?", System.Data.OleDb.OleDbType.Currency).Value = yeniToplamBorc;
                        cmdUpdate.Parameters.Add("?", System.Data.OleDb.OleDbType.VarWChar, 255).Value = gsmTelefon;
                        cmdUpdate.ExecuteNonQuery();
                    }

                    string insertQuery = "INSERT INTO Tahsilat (MusteriAdi, GsmTelefon, EskiBorc, OdenenTutar, ToplamKalanBorc, [Tarih/Saat], Aciklama, OdemeSekli) " +
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
                            ev.Graphics.DrawString(timeText, trFont, Brushes.Black, new RectangleF(padding + halfWidth - 30, y, halfWidth, trFont.Height), rightFormat);
                            y += trFont.Height + 10;

                            // Toptancı bilgileri
                            ev.Graphics.DrawString("Müşteri Bilgileri", trFontBold, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontBold.Height), centerFormat);
                            y += trFontBold.Height + padding;
                            ev.Graphics.DrawString($"Müşteri Adı: {toptanciAdi}", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                            y += trFont.Height + padding;
                            ev.Graphics.DrawString($"Telefon: {gsmTelefon}", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                            y += trFont.Height + padding;

                            // Ödeme detayları
                            ev.Graphics.DrawString("Tahsilat Detayları", trFontBold, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontBold.Height), centerFormat);
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
                            ev.Graphics.DrawString("BİLGİ AMAÇLIDIR MALİ DEĞERİ YOKTUR", trFontItalic, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontItalic.Height * 2), centerFormat);
                            y += trFontItalic.Height * 2 + 30;
                        };

                        pd.Print();
                        MessageBox.Show("Ödeme makbuzu başarıyla yazdırıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    // FİŞ YAZDIRMA KISMI SONU

                    string selectQuery = "SELECT DevredenBorc FROM Musteriler WHERE GsmTelefon=@gsmTelefon";
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
                    MusterileriGetir();
                    ToplamBorcuGoster1();
                    TaksitBorcuGoster1();
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
                    panel1.Visible = false;
                    panel2.Visible = false;
                    textBox10.Text = string.Empty;
                    textBox11.Text = string.Empty;
                    textBox12.Text = string.Empty;
                    textBox14.Text = string.Empty;
                    textBox15.Text = string.Empty;
                    textBox16.Text = string.Empty;
                    textBox22.Text = string.Empty;
                    textBox18.Text = string.Empty;
                    textBox21.Text = string.Empty;
                    textBox9.Text = string.Empty;
                    textBox20.Text = string.Empty;
                    label1.Visible = true;
                    textBox2.Visible = true;
                    textBox7.Text = "";
                    textBox33.Text = "";
                    textBox23.Text = "";
                    textBox24.Text = "";
                    button1.Visible = false;
                    button2.Visible = false;
                    button3.Visible = false;
                    button4.Visible = false;
                    button5.Visible = false;
                    button8.Visible = false;
                    button10.Visible = true;
                    dataGridView1.Visible = true;

                    textBox9.ReadOnly = false;
                    textBox21.ReadOnly = false;
                    textBox8.ReadOnly = true;
                    textBox17.ReadOnly = true;
                    label24.Text = "Devreden Borç";
                    label12.Text = "Limit Belirle";
                    label34.Visible = false;
                    label35.Visible = false;
                    textBox17.Visible = false;
                    textBox8.Visible = false;

                    // Temizleme sonrası varsayılan ülke olarak Türkiye'yi seçer
                    comboBox2.SelectedItem = "Türkiye";

                    MessageBox.Show("Ödeme sadece seçili müşteri için kaydedildi ve toplam borç güncellendi.", "Bilgi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata");
            }
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
                MessageBox.Show("Lütfen Müşteri adını girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    int lineCount = (int)Math.Ceiling(aciklamaSize.Width / aciklamaWidth) * (int)Math.Ceiling(aciklamaSize.Height / lineHeight);
                    paperHeight += Math.Max(20, lineCount * trFont.Height);
                }
            }
            paperHeight += 10;
            paperHeight += 10;
            paperHeight += trFontItalic.Height * 2 + 30;

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
                ev.Graphics.DrawString(timeText, trFont, Brushes.Black, new RectangleF(padding + halfWidth - 30, y, halfWidth, trFont.Height), rightFormat);
                y += trFont.Height + 10;

                // Toptancı bilgileri
                ev.Graphics.DrawString("Müşteri Bilgileri", trFontBold, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontBold.Height), centerFormat);
                y += trFontBold.Height + padding;
                ev.Graphics.DrawString($"Müşteri Adı: {toptanciAdi}", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                y += trFont.Height + padding;
                ev.Graphics.DrawString($"Telefon: {textBox3.Text}", trFont, Brushes.Black, new RectangleF(padding, y, contentWidth, trFont.Height), leftFormat);
                y += trFont.Height + padding;

                // Ödeme detayları
                ev.Graphics.DrawString("Tahsilat Detayları", trFontBold, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontBold.Height), centerFormat);
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
                ev.Graphics.DrawString("BİLGİ AMAÇLIDIR MALİ DEĞERİ YOKTUR", trFontItalic, Brushes.Black, new RectangleF(padding, y, contentWidth, trFontItalic.Height * 2), centerFormat);
                y += trFontItalic.Height * 2 + 30;
            };

            pd.Print();
            MessageBox.Show("Ödeme makbuzu başarıyla yazdırıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void button11_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            textBox7.Text = "";
            textBox33.Text = "";
            button3.Visible = true;
            dataGridView1.Visible = true;
            textBox2.Visible = true;
            label1.Visible = true;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            textBox23.Text = "";
            textBox24.Text = "";
            button5.Visible = true;
            dataGridView1.Visible = true;
            textBox2.Visible = true;
            label1.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            // Seçilen müşteri bilgileri
            string secilenMusteriAdi = dataGridView1.CurrentRow.Cells["MusteriAdi"].Value?.ToString();
            string secilenGsmTelefon = dataGridView1.CurrentRow.Cells["GsmTelefon"].Value?.ToString();

            // MusteriBorcDetayi formunu oluştur ve bilgileri ata
            MusteriBorcDetayi detayForm = new MusteriBorcDetayi();
            detayForm.MusteriAdi = secilenMusteriAdi;
            detayForm.GsmTelefon = secilenGsmTelefon;

            // Formu aç ve bu formu gizle
            detayForm.Show();
            this.Hide(); // Bu formu kapatmak yerine gizlemek daha iyi olabilir.
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // DataGridView'den güncel bilgileri al
                string musteriAdi = dataGridView1.SelectedRows[0].Cells["MusteriAdi"].Value?.ToString() ?? "";
                string gsmTelefon = dataGridView1.SelectedRows[0].Cells["GsmTelefon"].Value?.ToString() ?? "";

                // TextBox'lardaki mevcut bilgileri al
                string textBoxAdi = textBox10.Text.Trim();
                string textBoxGsm = textBox18.Text.Trim();

                // Toptancı adı ve GSM numarasının değişip değişmediğini kontrol et
                bool adiDegisti = textBoxAdi != musteriAdi;
                bool gsmDegisti = textBoxGsm != gsmTelefon;

                if (adiDegisti && gsmDegisti)
                {
                    MessageBox.Show("Müşteri adı ve GSM numarası değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgileri kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (adiDegisti)
                {
                    MessageBox.Show("Müşteri adı değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgiyi kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (gsmDegisti)
                {
                    MessageBox.Show("Müşteri GSM numarası değişmiştir. Lütfen önce 'Toptancı Güncelle' butonuna tıklayarak bilgiyi kaydedin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Seçilen GSM telefon numarasını al
                string secilenGsmTelefon = dataGridView1.SelectedRows[0].Cells["GsmTelefon"].Value?.ToString();
                if (string.IsNullOrEmpty(secilenGsmTelefon))
                {
                    MessageBox.Show("Seçilen müşterinin GSM numarası geçersizdir. Lütfen başka bir toptancı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // MusteriIade formunu aç
                try
                {
                    MusteriIade iadeForm = new MusteriIade(secilenGsmTelefon);
                    iadeForm.Show();
                    this.Close(); // Mevcut formu kapatır
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen önce bir toptancı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {// MusteriBorcListesi formunu oluştur
            MusteriBorcListesi borcListesiFormu = new MusteriBorcListesi();
            // Formu aç ve bu formu gizle
            borcListesiFormu.Show();
            this.Hide(); // Bu formu kapatmak yerine gizlemek daha iyi olabilir.
            // Formu göster
           

        }

      

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            button15.Enabled = !checkBox4.Checked;
        }
    }
}
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

namespace ÜrünYönetimSistemi
{
    public partial class MusteriEkle : Form
    {
        public MusteriEkle()
        {
            InitializeComponent();
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            textBox18.KeyPress += TextBox_Sayi_KeyPress;
            textBox15.KeyPress += TextBox_Sayi_KeyPress;
            textBox21.KeyPress += TextBox_Sayi_KeyPress;
            textBox9.KeyPress += TextBox_Sayi_KeyPress;
        }

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
            decimal taksit = 0;

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
                        insertCommand.Parameters.AddWithValue("@Ulke", comboBox2.SelectedItem?.ToString());
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri eklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void TextBox_Harf_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harf, boşluk ve kontrol tuşlarına izin ver.
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {

            // Alanları temizleme
            textBox10.Clear();
            textBox11.Clear();
            textBox12.Clear();
            textBox14.Clear();
            textBox15.Clear();
            textBox22.Clear();
            textBox16.Clear();
            comboBox2.SelectedItem = "Türkiye";
            textBox18.Clear();
            textBox21.Clear();
            textBox9.Clear();
            textBox20.Clear();
        }


        private void textBox22_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harf, boşluk, eğik çizgi (/) ve kontrol tuşlarına izin ver.
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ' && e.KeyChar != '/')
            {
                e.Handled = true;
            }
        }

        private void MusteriEkle_Load(object sender, EventArgs e)
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
};
            comboBox2.Items.AddRange(ulkeler);

            // 1. (ÖNEMLİ) Listeyi sadece listeden seçim yapılabilir hale getirin.
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;

            // 2. ComboBox'ın MaxDropDownItems değerini belirleyin (Örn: 10 satır)
            comboBox2.MaxDropDownItems = 10;

            // 3. (GEREKLİ EKLEME) Listeden ilk 10 öğenin yüksekliğini hesaplayarak 
            // açılır listeyi piksel cinsinden sınırlandırın.
            // Ortalama bir satır yüksekliği 15-18 pikseldir. 10 satır için 180 piksel deneyelim:

            int maxYukseklik = 10 * comboBox2.ItemHeight + 2; // 2, kenarlıklar için küçük bir boşluk

            // Eğer bu özelliği kullanan bir WinForms ortamındaysanız, bu satır işe yarayacaktır:
            comboBox2.DropDownHeight = maxYukseklik;
            // Listede Türkiye'yi bulup seçili yapar.
            comboBox2.SelectedIndex = Array.IndexOf(ulkeler, "Türkiye");

        }
    }
}

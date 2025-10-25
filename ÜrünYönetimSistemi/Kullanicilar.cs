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
using System.IO; // Path sınıfı için gerekli

namespace ÜrünYönetimSistemi
{
    public partial class Kullanicilar : Form
    {
        // Form seviyesinde, tıklanan satırdaki mevcut GSM telefonunu tutmak için bir değişken tanımladık.
        private string _mevcutGsmTelefon;

        public Kullanicilar()
        {
            InitializeComponent();
            textBox4.Visible = false;
            label4.Visible = false;
            button2.Visible = false;
            button3.Visible = false;

            // checkedListBox1 Öğe Listesinin Yeniden Tanımlanması (Yeni İsimler ve Ayarlar Yetkisi)
            checkedListBox1.Items.Clear();
            checkedListBox1.Items.AddRange(new object[] {
                "Ürün İşlemleri",   // UrunGirisi ile eşleşir - Index 0
                "Satış İşlemi",     // SatisIslemi - Index 1
                "Fiyat Gör",        // FiyatGor - Index 2
                "Fiyat Teklifi",    // FiyatTeklifi - Index 3
                "Müşteriler",       // Musteriler - Index 4
                "Barkod Yazdır",    // BarkodYazdir - Index 5
                "Raporlar",         // Raporlar - Index 6
                "Kasa",             // Kasa - Index 7
                "Toptancılar",      // Toptancilar - Index 8
                "Kullanıcılar",     // Kullanicilar - Index 9
                "Ürün Detayı",      // UrunDetayi - Index 10
                "Ürün İade Al",     // UrunIade ile eşleşir - Index 11
                "Ürün İade Et",     // UrunAlis ile eşleşir - Index 12
                "Ayarlar"           // Yeni Eklendi (Ayarlar kolonu) - Index 13
            });

            textBox1.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)) e.Handled = true; };
            textBox2.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            checkedListBox1.CheckOnClick = true; // Tek tıklama ile işaretleme
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Veritabanı bağlantı yolu (connection string)
            string dbPath = Path.Combine(Application.StartupPath, "ÜrünYönetimSistemi.accdb");
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";

            // Zorunlu alanların tek tek kontrolü
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Personel Adı boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("GSM Telefonu boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Şifre boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Telefon numarası 10 karakter kontrolü
            if (textBox2.Text.Length != 10)
            {
                MessageBox.Show("Telefon numarası 10 karakterli olmalıdır!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.Connection = conn;

                    try
                    {
                        // --- Aynı GSM'ye sahip kullanıcı kontrolü ---
                        conn.Open();
                        string checkSql = "SELECT COUNT(*) FROM Personel WHERE GsmTelefon = @gsmTelefon";
                        OleDbCommand checkCmd = new OleDbCommand(checkSql, conn);
                        checkCmd.Parameters.AddWithValue("@gsmTelefon", textBox2.Text);
                        int existingUserCount = (int)checkCmd.ExecuteScalar();

                        if (existingUserCount > 0)
                        {
                            MessageBox.Show("Bu GSM numarası zaten kayıtlı. Lütfen farklı bir numara giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        // --- Kontrol sonu ---

                        // SQL sorgusunu hazırla (UrunDetayi eklendi, index'ler düzeltildi)
                        string sql = "INSERT INTO Personel (Yetki, GsmTelefon, Sifre, UrunGirisi, SatisIslemi, FiyatGor, FiyatTeklifi, Musteriler, BarkodYazdir, Raporlar, Kasa, Toptancilar, Kullanicilar, UrunDetayi, UrunIade, UrunAlis, Ayarlar) " +
                                     "VALUES (@yetki, @gsm, @sifre, @urunGirisi, @satisIslemi, @fiyatGor, @fiyatTeklifi, @musteriler, @barkodYazdir, @raporlar, @kasa, @toptancilar, @kullanicilar, @urundetayi, @urunIade, @urunAlis, @ayarlar)";

                        cmd.CommandText = sql;

                        // Parametreleri ekle (Yetki listesi index'lerine göre - UrunDetayi index 10)
                        cmd.Parameters.AddWithValue("@yetki", textBox1.Text);
                        cmd.Parameters.AddWithValue("@gsm", textBox2.Text);
                        cmd.Parameters.AddWithValue("@sifre", textBox3.Text);

                        // CheckedListBox sırası: 0-UrunGirisi, ..., 10-UrunDetayi, 11-UrunIade, 12-UrunAlis, 13-Ayarlar
                        cmd.Parameters.AddWithValue("@urunGirisi", checkedListBox1.GetItemChecked(0));
                        cmd.Parameters.AddWithValue("@satisIslemi", checkedListBox1.GetItemChecked(1));
                        cmd.Parameters.AddWithValue("@fiyatGor", checkedListBox1.GetItemChecked(2));
                        cmd.Parameters.AddWithValue("@fiyatTeklifi", checkedListBox1.GetItemChecked(3));
                        cmd.Parameters.AddWithValue("@musteriler", checkedListBox1.GetItemChecked(4));
                        cmd.Parameters.AddWithValue("@barkodYazdir", checkedListBox1.GetItemChecked(5));
                        cmd.Parameters.AddWithValue("@raporlar", checkedListBox1.GetItemChecked(6));
                        cmd.Parameters.AddWithValue("@kasa", checkedListBox1.GetItemChecked(7));
                        cmd.Parameters.AddWithValue("@toptancilar", checkedListBox1.GetItemChecked(8));
                        cmd.Parameters.AddWithValue("@kullanicilar", checkedListBox1.GetItemChecked(9));
                        cmd.Parameters.AddWithValue("@urundetayi", checkedListBox1.GetItemChecked(10)); // Ürün Detayı - Index 10
                        cmd.Parameters.AddWithValue("@urunIade", checkedListBox1.GetItemChecked(11));     // Ürün İade Al - Index 11
                        cmd.Parameters.AddWithValue("@urunAlis", checkedListBox1.GetItemChecked(12));     // Ürün İade Et - Index 12
                        cmd.Parameters.AddWithValue("@ayarlar", checkedListBox1.GetItemChecked(13));      // Ayarlar - Index 13

                        // Komutu çalıştır
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Kullanıcı başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        TabloyuDoldur();

                        // İşlem başarılıysa kutuları temizle
                        textBox1.Text = "";
                        textBox2.Text = "";
                        textBox3.Text = "";
                        textBox3.ReadOnly = false;
                        for (int i = 0; i < checkedListBox1.Items.Count; i++)
                        {
                            checkedListBox1.SetItemChecked(i, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Kutuları temizle
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";

            // textBox4 ve label4'ü gizle
            textBox4.Text = "";
            textBox4.Visible = false;
            label4.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button1.Visible = true;
            textBox3.ReadOnly = false;

            // CheckedListBox'taki tüm seçimleri kaldır
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }

            // DataGridView'deki seçili satırın seçimini kaldır
            if (dataGridView1.SelectedRows.Count > 0)
            {
                dataGridView1.ClearSelection();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Silme işlemi için her zaman seçilen (orijinal) GSM telefonunu kullan.
            string silinecekGsmTelefon = _mevcutGsmTelefon;

            // --- Eklenen Kontrol: Değer Değiştirilmiş mi? ---
            if (textBox2.Text != _mevcutGsmTelefon)
            {
                // Not: _mevcutGsmTelefon, DataGridView'den seçilen orijinal değeri tutar.
                MessageBox.Show("Silme işlemi için GSM numarası değiştirilemez. Lütfen önce yaptığınız değişiklikleri kaydedin ", "Uyarı: Güncelleme Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // --- Kontrol Sonu ---

            // GSM telefonunun boş olup olmadığını kontrol et (seçim yapılmamışsa)
            if (string.IsNullOrWhiteSpace(silinecekGsmTelefon))
            {
                MessageBox.Show("Lütfen silmek istediğiniz kullanıcıyı tablodan seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullanıcıdan silme işlemini onaylamasını iste.
            DialogResult result = MessageBox.Show(silinecekGsmTelefon + " numaralı kullanıcıyı silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return; // Eğer "Hayır" derse işlemi iptal et.
            }

            string dbPath = Path.Combine(Application.StartupPath, "ÜrünYönetimSistemi.accdb");
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.Connection = conn;

                    try
                    {
                        // SQL DELETE sorgusunu GsmTelefon'a göre hazırla.
                        // Artık sorguda da _mevcutGsmTelefon değişkenini kullanıyoruz.
                        string sql = "DELETE FROM Personel WHERE GsmTelefon = @gsmTelefon";
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@gsmTelefon", silinecekGsmTelefon);

                        conn.Open();
                        int affectedRows = cmd.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show(silinecekGsmTelefon + " numaralı kullanıcı başarıyla silindi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            TabloyuDoldur();

                            // Kutuları temizle ve görünürlüğü sıfırla (button4_Click'in yaptığı iş)
                            button4_Click(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("Belirtilen GSM numarasına sahip bir kullanıcı bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void Kullanicilar_Load(object sender, EventArgs e)
        {
            TabloyuDoldur();
        }

        private void TabloyuDoldur()
        {
            string dbPath = Path.Combine(Application.StartupPath, "ÜrünYönetimSistemi.accdb");
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT Yetki, GsmTelefon, Sifre FROM Personel";
                    OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Sifre"] != DBNull.Value)
                        {
                            string sifre = row["Sifre"].ToString();
                            row["Sifre"] = new string('*', sifre.Length);
                        }
                    }

                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı tablosu yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox4.Visible = true;
            label4.Visible = true;
            button1.Visible = false;
            button2.Visible = true;
            button3.Visible = true;
            textBox3.ReadOnly = true;

            // Başlık satırına tıklanırsa işlemi durdur
            if (e.RowIndex == -1)
            {
                return;
            }

            // Seçilen satırı al
            DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

            // Form seviyesindeki değişkene değeri ata
            _mevcutGsmTelefon = selectedRow.Cells["GsmTelefon"].Value.ToString();

            // Yetki ve Gsm bilgilerini TextBox'lara aktar
            textBox1.Text = selectedRow.Cells["Yetki"].Value.ToString();
            textBox2.Text = _mevcutGsmTelefon;

            // Veritabanından tam şifre ve yetki bilgilerini al
            string dbPath = Path.Combine(Application.StartupPath, "ÜrünYönetimSistemi.accdb");
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.Connection = conn;
                    try
                    {
                        // SQL sorgusuna UrunDetayi kolonunu ekle (Sıra, checkedListBox sırasıyla eşleşmelidir)
                        string sql = "SELECT Sifre, UrunGirisi, SatisIslemi, FiyatGor, FiyatTeklifi, Musteriler, BarkodYazdir, Raporlar, Kasa, Toptancilar, Kullanicilar, UrunDetayi, UrunIade, UrunAlis, Ayarlar FROM Personel WHERE GsmTelefon = @gsmTelefon";
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@gsmTelefon", _mevcutGsmTelefon);

                        conn.Open();
                        OleDbDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            // Tam şifre bilgisini textBox3'e aktar
                            textBox3.Text = reader["Sifre"].ToString();

                            // Yetkileri işaretle. (Yetkiler, Sifre'den sonra 1. index'ten başlar - 14 yetki sütunu)
                            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                            {
                                // i+1 çünkü 0. index Sifre sütununa ait. Yetkiler 1. index'ten başlıyor.
                                checkedListBox1.SetItemChecked(i, reader.GetBoolean(i + 1));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Kullanıcı bilgileri yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Veritabanı bağlantı yolu (connection string)
            string dbPath = Path.Combine(Application.StartupPath, "ÜrünYönetimSistemi.accdb");
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";

            // Zorunlu alanların boş olup olmadığını kontrol et
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Yetki, GSM Telefonu ve mevcut Şifre boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullanıcı seçimi kontrolü
            if (string.IsNullOrEmpty(_mevcutGsmTelefon))
            {
                MessageBox.Show("Lütfen önce tablodan bir kullanıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Telefon numarası 10 karakter kontrolü (Güncelleme için de gerekli)
            if (textBox2.Text.Length != 10)
            {
                MessageBox.Show("Telefon numarası 10 karakterli olmalıdır!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Değiştirilecek şifreyi belirle: Eğer yeni şifre (textBox4) doluysa onu, boşsa eski şifreyi (textBox3) kullan
            string yeniSifre = string.IsNullOrWhiteSpace(textBox4.Text) ? textBox3.Text : textBox4.Text;

            // Kullanıcıya onay mesajı göster
            DialogResult result = MessageBox.Show(textBox2.Text + " numaralı kullanıcının bilgilerini güncellemek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return; // İşlemi iptal et
            }

            // Şifre değiştiriliyorsa ek onay al
            if (!string.IsNullOrWhiteSpace(textBox4.Text))
            {
                DialogResult sifreOnay = MessageBox.Show("Şifreyi değiştirmek istediğinizden emin misiniz?", "Şifre Değişikliği Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (sifreOnay == DialogResult.No)
                {
                    return; // Şifre değiştirme işlemini iptal et
                }
            }

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.Connection = conn;

                    try
                    {
                        // --- Aynı GSM'ye sahip başka bir kullanıcı kontrolü ---
                        conn.Open();
                        string checkSql = "SELECT COUNT(*) FROM Personel WHERE GsmTelefon = @gsmTelefon AND GsmTelefon <> @eskiGsmTelefon";
                        OleDbCommand checkCmd = new OleDbCommand(checkSql, conn);
                        checkCmd.Parameters.AddWithValue("@gsmTelefon", textBox2.Text);
                        checkCmd.Parameters.AddWithValue("@eskiGsmTelefon", _mevcutGsmTelefon); // Eski gsm telefon değeri.
                        int existingUserCount = (int)checkCmd.ExecuteScalar();

                        if (existingUserCount > 0)
                        {
                            MessageBox.Show("Bu GSM numarası zaten başka bir kullanıcıya ait. Lütfen farklı bir numara giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        // --- Kontrol sonu ---

                        // SQL UPDATE sorgusunu hazırla (UrunDetayi eklendi, index'ler düzeltildi)
                        string sql = "UPDATE Personel SET Yetki = @yetki, GsmTelefon = @gsmTelefon, Sifre = @sifre, " +
                                     "UrunGirisi = @urunGirisi, SatisIslemi = @satisIslemi, FiyatGor = @fiyatGor, FiyatTeklifi = @fiyatTeklifi, " +
                                     "Musteriler = @musteriler, BarkodYazdir = @barkodYazdir, Raporlar = @raporlar, Kasa = @kasa, " +
                                     "Toptancilar = @toptancilar, Kullanicilar = @kullanicilar, UrunDetayi = @urundetayi, UrunIade = @urunIade, UrunAlis = @urunAlis, Ayarlar = @ayarlar " +
                                     "WHERE GsmTelefon = @mevcutGsmTelefon";

                        cmd.CommandText = sql;

                        // Parametreleri ekle (Yetki listesi index'lerine göre - UrunDetayi index 10)
                        cmd.Parameters.AddWithValue("@yetki", textBox1.Text);
                        cmd.Parameters.AddWithValue("@gsmTelefon", textBox2.Text); // Yeni GSM telefon (değişmediyse aynı kalır)
                        cmd.Parameters.AddWithValue("@sifre", yeniSifre); // Belirlediğimiz yeni veya mevcut şif

                        // CheckedListBox sırası: 0-UrunGirisi, ..., 10-UrunDetayi, 11-UrunIade, 12-UrunAlis, 13-Ayarlar
                        cmd.Parameters.AddWithValue("@urunGirisi", checkedListBox1.GetItemChecked(0));
                        cmd.Parameters.AddWithValue("@satisIslemi", checkedListBox1.GetItemChecked(1));
                        cmd.Parameters.AddWithValue("@fiyatGor", checkedListBox1.GetItemChecked(2));
                        cmd.Parameters.AddWithValue("@fiyatTeklifi", checkedListBox1.GetItemChecked(3));
                        cmd.Parameters.AddWithValue("@musteriler", checkedListBox1.GetItemChecked(4));
                        cmd.Parameters.AddWithValue("@barkodYazdir", checkedListBox1.GetItemChecked(5));
                        cmd.Parameters.AddWithValue("@raporlar", checkedListBox1.GetItemChecked(6));
                        cmd.Parameters.AddWithValue("@kasa", checkedListBox1.GetItemChecked(7));
                        cmd.Parameters.AddWithValue("@toptancilar", checkedListBox1.GetItemChecked(8));
                        cmd.Parameters.AddWithValue("@kullanicilar", checkedListBox1.GetItemChecked(9));
                        cmd.Parameters.AddWithValue("@urundetayi", checkedListBox1.GetItemChecked(10)); // Ürün Detayı - Index 10
                        cmd.Parameters.AddWithValue("@urunIade", checkedListBox1.GetItemChecked(11));     // Ürün İade Al - Index 11
                        cmd.Parameters.AddWithValue("@urunAlis", checkedListBox1.GetItemChecked(12));     // Ürün İade Et - Index 12
                        cmd.Parameters.AddWithValue("@ayarlar", checkedListBox1.GetItemChecked(13));      // Ayarlar - Index 13

                        // WHERE koşulu için eski GSM telefon parametresi
                        cmd.Parameters.AddWithValue("@mevcutGsmTelefon", _mevcutGsmTelefon);

                        // Komutu çalıştır
                        int affectedRows = cmd.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Kullanıcı bilgileri başarıyla güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            TabloyuDoldur(); // Tabloyu yenile

                            // Kutuları temizle
                            textBox1.Text = "";
                            textBox2.Text = "";
                            textBox3.Text = "";

                            // textBox4 ve label4'ü gizle
                            textBox4.Text = "";
                            textBox4.Visible = false;
                            label4.Visible = false;
                            button2.Visible = false;
                            button3.Visible = false;
                            button1.Visible = true;
                            textBox3.ReadOnly = false;

                            // CheckedListBox'taki tüm seçimleri kaldır
                            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                            {
                                checkedListBox1.SetItemChecked(i, false);
                            }

                            // DataGridView'deki seçili satırın seçimini kaldır
                            if (dataGridView1.SelectedRows.Count > 0)
                            {
                                dataGridView1.ClearSelection();
                            }

                        }
                        else
                        {
                            MessageBox.Show("Güncellenecek bir kullanıcı bulunamadı. Lütfen GSM numarasını kontrol edin.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            // Arama yapılacak metni doğru textBox'tan alıyoruz.
            string aramaMetni = textBox5.Text.Trim();

            if (string.IsNullOrEmpty(aramaMetni))
            {
                TabloyuDoldur(); // Arama metni boşsa tüm toptancıları göster
            }
            else
            {
                try
                {
                    using (OleDbConnection baglan6 = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                    {
                        DataTable tablo = new DataTable();
                        baglan6.Open();

                        // Sadece Yetki, GsmTelefon ve Şifre sütunlarını çekiyoruz.
                        string query = "SELECT Yetki, GsmTelefon, Sifre FROM Personel WHERE Yetki LIKE @arama OR GsmTelefon LIKE @arama";

                        OleDbDataAdapter adapter = new OleDbDataAdapter(query, baglan6);
                        adapter.SelectCommand.Parameters.AddWithValue("@arama", "%" + aramaMetni + "%");
                        adapter.Fill(tablo);

                        // Şifre sütununu yıldızlama
                        foreach (DataRow row in tablo.Rows)
                        {
                            if (row["Sifre"] != DBNull.Value)
                            {
                                string sifre = row["Sifre"].ToString();
                                row["Sifre"] = new string('*', sifre.Length);
                            }
                        }

                        dataGridView1.DataSource = tablo;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Arama sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

            }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Globalization;
using System.Threading;


namespace ÜrünYönetimSistemi
{
    public partial class Form1 : Form
    {
        public Form1 frm1;
        public Form2 frm2;
        public Ürün_Girişi ürüngirişi;
        public Toptanci toptanci;
        public ToptanciHesapDetayi toptanciHesapDetayi;
        public string yetki;
        public Fiyat_Gör fiyatGör;
        public Toplu_Ürün_Sil topluÜrünSil;
        public Satış_İşlemleri satisislemi;

        private readonly string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Application.StartupPath}\\ÜrünYönetimSistemi.accdb";
        public string SelectedLanguage
        {
            get
            {
                return comboBox2.SelectedItem?.ToString() ?? "Türkçe";
            }
        }
        // Dil uyarıları için değişkenler
        private string emptyPasswordMsg;
        private string emptyRoleMsg;
        private string loginFailedMsg;

        // Çeviri sözlüğü
        private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>
        {
            ["tr-TR"] = new Dictionary<string, string>
            { {"Form1", "Kullancı Giriş Ekranı"},
                {"Label1", "Yetki"},
                {"Label2", "Şifre"},
                {"BtnLogin", "Giriş Yap"},
                {"BtnClear", "Temizle"},
                {"BtnExit", "Uygulamayı Kapat"},
                {"EmptyPasswordMsg", "Şifre Alanı Boş Bırakılamaz!!!"},
                {"EmptyRoleMsg", "Yetki Alanı Boş Bırakılamaz!!!"},
                {"LoginFailedMsg", "Giriş Başarısız"}
            },
            ["en-US"] = new Dictionary<string, string>
            {
                {"Form1", "User Login Screen"},
                {"Label1", "Role"},
                {"Label2", "Password"},
                {"BtnLogin", "Login"},
                {"BtnClear", "Clear"},
                {"BtnExit", "Exit Application"},
                {"EmptyPasswordMsg", "Password field cannot be empty!!!"},
                {"EmptyRoleMsg", "Role field cannot be empty!!!"},
                {"LoginFailedMsg", "Login Failed"}
            },
            ["de-DE"] = new Dictionary<string, string>
            {{"Form1", "Benutzer Anmeldebildschirm"},
                {"Label1", "Berechtigung"},
                {"Label2", "Passwort"},
                {"BtnLogin", "Anmelden"},
                {"BtnClear", "Löschen"},
                {"BtnExit", "Anwendung Beenden"},
                {"EmptyPasswordMsg", "Passwortfeld darf nicht leer sein!!!"},
                {"EmptyRoleMsg", "Berechtigungsfeld darf nicht leer sein!!!"},
                {"LoginFailedMsg", "Anmeldung fehlgeschlagen"}
            }
        };

        public Form1()
        {
            InitializeComponent();
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            textBox1.MaxLength = 11;
            comboBox2.Visible = false;
            frm2 = new Form2();
            frm2.frm1 = this;

            ürüngirişi = new Ürün_Girişi();
            ürüngirişi.frm1 = this;

            toptanci = new Toptanci();
            toptanci.frm1 = this;

            fiyatGör = new Fiyat_Gör();
            fiyatGör.frm1 = this;

            satisislemi = new Satış_İşlemleri();
            satisislemi.frm1 = this;


            // ComboBox2’ye dil seçeneklerini sadece constructor’da ekle
            comboBox2.Items.AddRange(new object[] { "Türkçe", "İngilizce", "Almanca" });
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;

            // Kaydedilmiş dil varsa uygula, yoksa varsayılan Türkçe
            string savedCulture = Properties.Settings.Default.SelectedLanguage;
            if (string.IsNullOrEmpty(savedCulture))
                savedCulture = "tr-TR";

            ChangeLanguage(savedCulture);

            // ComboBox2 seçimini güncelle
            comboBox2.SelectedIndex = savedCulture switch
            {
                "tr-TR" => 0,
                "de-DE" => 2,
                "en-US" => 1,
                _ => 0
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                comboBox1.Items.Clear();
                using (OleDbConnection baglan = new OleDbConnection(connectionString))
                {
                    OleDbCommand kmt = new OleDbCommand("SELECT Yetki FROM Personel", baglan);
                    baglan.Open();
                    OleDbDataReader okuyucu = kmt.ExecuteReader();
                    while (okuyucu.Read())
                    {
                        comboBox1.Items.Add(okuyucu["Yetki"].ToString());
                    }
                }

                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veritabanı hatası: {ex.Message}");
            }
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show(emptyPasswordMsg);
                return;
            }*/
            if (string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show(emptyRoleMsg);
                return;
            }

            string yedekKlasoru = "C:\\UygulamaYedekleri\\Access_Yedekler";
            string enSonYedekDosyaYolu = "";

            try
            {
                if (Directory.Exists(yedekKlasoru))
                {
                    var yedekDosyalari = Directory.GetFiles(yedekKlasoru, "Veritabani_yedek_*.accdb")
                                                 .OrderByDescending(f => File.GetLastWriteTime(f));

                    if (yedekDosyalari.Any())
                        enSonYedekDosyaYolu = yedekDosyalari.First();
                }
            }
            catch
            {
                // hata olsa da devam etsin
            }

            try
            {
                using (OleDbConnection baglan = new OleDbConnection(connectionString))
                {
                    OleDbCommand kmt = new OleDbCommand("SELECT * FROM Personel WHERE Sifre=@sifre AND Yetki=@yetki", baglan);
                    kmt.Parameters.AddWithValue("@sifre", textBox1.Text);
                    kmt.Parameters.AddWithValue("@yetki", comboBox1.Text);
                    baglan.Open();
                    OleDbDataReader okuyucu = kmt.ExecuteReader();

                    if (okuyucu.Read())
                    {
                        // 🔹 Yükleniyor formunu göster
                        frmYukleniyor yukleniyor = new frmYukleniyor();
                        var sonuc = yukleniyor.ShowDialog(); // Yüzde 100 olunca DialogResult.OK döner

                        if (sonuc == DialogResult.OK)
                        {
                            yetki = comboBox1.Text;
                            textBox1.Text = "";
                            comboBox1.SelectedIndex = 0;
                            this.Hide();
                            frm2.Show();
                        }
                    }
                    else
                    {
                        MessageBox.Show(loginFailedMsg);
                        textBox1.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Giriş hatası: {ex.Message}");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            comboBox1.Text = "";
        }

       
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedLanguage = comboBox2.SelectedItem.ToString();
            string cultureName = selectedLanguage switch
            {
                "Türkçe" => "tr-TR",
                "Almanca" => "de-DE",
                "İngilizce" => "en-US",
                _ => "tr-TR"
            };

            ChangeLanguage(cultureName);

            // Kalıcı olarak kaydet
            Properties.Settings.Default.SelectedLanguage = cultureName;
            Properties.Settings.Default.Save();
        }
     

        private void ChangeLanguage(string cultureName)
        {
            if (!translations.ContainsKey(cultureName))
                cultureName = "tr-TR";

            var t = translations[cultureName];

            // Form başlığını ayarla
            this.Text = t["Form1"];

            label1.Text = t["Label1"];
            label2.Text = t["Label2"];
            button1.Text = t["BtnLogin"];
            button3.Text = t["BtnClear"];
          
            emptyPasswordMsg = t["EmptyPasswordMsg"];
            emptyRoleMsg = t["EmptyRoleMsg"];
            loginFailedMsg = t["LoginFailedMsg"];
        }
    }
}

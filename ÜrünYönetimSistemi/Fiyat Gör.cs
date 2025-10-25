using DocumentFormat.OpenXml.Spreadsheet;
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
using ÜrünYönetimSistemi.Properties;

namespace ÜrünYönetimSistemi
{
    public partial class Fiyat_Gör : Form
    {
        public Form1 frm1;
        public Form2 frm2;
        public string CurrentCulture { get; set; } = "tr-TR";

        // Çeviri sözlüğü
        private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>
        {
            ["tr-TR"] = new Dictionary<string, string>
            {{"Fiyat Gör","Fiyat Gör"},
                {"Label1", "Ürün Adı Veya Barkod No Girin"},
                {"Label2", "Ürün Adı"},
                {"Label3", "Mevcut Stok"},
                {"Label4", "Satış Fiyatı"},
                {"Label5", "İndirimli Fiyat"},
                {"Btn1", "Temizle"},
                {"Btn2", "Kapat"},
                {"Col1", "Barkod No"},
                {"Col2", "Ürün Adı"},
                {"Col3", "Stok Miktarı"},
                {"Col4", "Satış Fiyatı"},
                {"Col5", "İndirimli Fiyat"}
            },
            ["en-US"] = new Dictionary<string, string>
            {{"Fiyat Gör","View Price"},
                {"Label1", "Enter Product Name Or Barcode"},
                {"Label2", "Product Name"},
                {"Label3", "Current Stock"},
                {"Label4", "Sale Price"},
                {"Label5", "Discounted Price"},
                {"Btn1", "Clear"},
                {"Btn2", "Close"},
                {"Col1", "Barcode"},
                {"Col2", "Product Name"},
                {"Col3", "Stock"},
                {"Col4", "Sale Price"},
                {"Col5", "Discounted Price"}
            },
            ["de-DE"] = new Dictionary<string, string>
            {{"Fiyat Gör","Preis Anzeigen"},
                {"Label1", "Produktname Oder Barcode Eingeben"},
                {"Label2", "Produktname"},
                {"Label3", "Verfügbarer Bestand"},
                {"Label4", "Verkaufspreis"},
                {"Label5", "Rabattierter Preis"},
                {"Btn1", "Löschen"},
                {"Btn2", "Schließen"},
                {"Col1", "Barcode"},
                {"Col2", "Produktname"},
                {"Col3", "Bestand"},
                {"Col4", "Verkaufspreis"},
                {"Col5", "Rabattierter Preis"}
            }
        };

        public Fiyat_Gör()
        {
            InitializeComponent();

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            panel1.Visible = false;
            Listele();
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            textBox5.ReadOnly = true;
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void Fiyat_Gör_Load(object sender, EventArgs e)
        {
            if (frm1 != null)
            {
                CurrentCulture = frm1.SelectedLanguage switch
                {
                    "Türkçe" => "tr-TR",
                   "İngilizce" => "en-US",
                    "Almanca" => "de-DE",
                    _ => "tr-TR"
                };
            }
            ChangeLanguage(CurrentCulture);
        }

        public void ChangeLanguage(string cultureName)
        {
            if (!translations.ContainsKey(cultureName))
                cultureName = "tr-TR";

            var t = translations[cultureName];
            this.Text = t["Fiyat Gör"];
            label1.Text = t["Label1"];
            label2.Text = t["Label2"];
            label3.Text = t["Label3"];
            label4.Text = t["Label4"];
            label5.Text = t["Label5"];
            button1.Text = t["Btn1"];
        

            // DataGridView sütun başlıkları
            if (dataGridView1.Columns.Count >= 5)
            {
                dataGridView1.Columns["Barkod_No"].HeaderText = t["Col1"];
                dataGridView1.Columns["Ürün_Adi"].HeaderText = t["Col2"];
                dataGridView1.Columns["Stok_Miktari"].HeaderText = t["Col3"];
                dataGridView1.Columns["Satis_Fiyati"].HeaderText = t["Col4"];
                dataGridView1.Columns["2SatisFiyati"].HeaderText = t["Col5"];
            }

            CurrentCulture = cultureName;
        }

        private void Listele()
        {
            try
            {
                using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    DataTable tablo = new DataTable();
                    tablo.Clear();
                    OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT Barkod_No, Ürün_Adi, Stok_Miktari, Satis_Fiyati, [2SatisFiyati] FROM ÜrünGirişi", baglan);
                    adapter.Fill(tablo);
                    dataGridView1.DataSource = tablo;

                    // Sütun başlıkları
                    dataGridView1.Columns["Barkod_No"].HeaderText = translations[CurrentCulture]["Col1"];
                    dataGridView1.Columns["Ürün_Adi"].HeaderText = translations[CurrentCulture]["Col2"];
                    dataGridView1.Columns["Stok_Miktari"].HeaderText = translations[CurrentCulture]["Col3"];
                    dataGridView1.Columns["Satis_Fiyati"].HeaderText = translations[CurrentCulture]["Col4"];
                    dataGridView1.Columns["2SatisFiyati"].HeaderText = translations[CurrentCulture]["Col5"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri çekme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    panel1.Visible = true;
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    textBox2.Text = row.Cells["Ürün_Adi"].Value?.ToString() ?? "";
                    textBox3.Text = row.Cells["Stok_Miktari"].Value?.ToString() ?? "";
                    textBox4.Text = row.Cells["Satis_Fiyati"].Value?.ToString() ?? "";
                    textBox5.Text = row.Cells["2SatisFiyati"].Value?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hücre seçme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            panel1.Visible = false;
        }

     

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Filtrele();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            Filtrele();
        }

        private void textBox6_TextChanged_1(object sender, EventArgs e)
        {
            Filtrele();
        }

        private void Filtrele()
        {
            try
            {
                using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    DataTable tablo = new DataTable();
                    tablo.Clear();
                    string sorgu;
                    if (string.IsNullOrWhiteSpace(textBox6.Text))
                    {
                        sorgu = "SELECT Barkod_No, Ürün_Adi, Stok_Miktari, Satis_Fiyati, [2SatisFiyati] FROM ÜrünGirişi";
                    }
                    else
                    {
                        sorgu = "SELECT Barkod_No, Ürün_Adi, Stok_Miktari, Satis_Fiyati, [2SatisFiyati] FROM ÜrünGirişi " +
                                "WHERE Barkod_No LIKE @arama OR Ürün_Adi LIKE @arama";
                    }

                    OleDbDataAdapter adapter = new OleDbDataAdapter(sorgu, baglan);
                    adapter.SelectCommand.Parameters.AddWithValue("@arama", "%" + textBox6.Text + "%");
                    adapter.Fill(tablo);
                    dataGridView1.DataSource = tablo;

                    // Sütun başlıklarını yeniden ayarla
                    dataGridView1.Columns["Barkod_No"].HeaderText = translations[CurrentCulture]["Col1"];
                    dataGridView1.Columns["Ürün_Adi"].HeaderText = translations[CurrentCulture]["Col2"];
                    dataGridView1.Columns["Stok_Miktari"].HeaderText = translations[CurrentCulture]["Col3"];
                    dataGridView1.Columns["Satis_Fiyati"].HeaderText = translations[CurrentCulture]["Col4"];
                    dataGridView1.Columns["2SatisFiyati"].HeaderText = translations[CurrentCulture]["Col5"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Filtreleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

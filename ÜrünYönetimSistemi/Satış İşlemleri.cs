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


namespace ÜrünYönetimSistemi
{
    public partial class Satış_İşlemleri : Form
    {
        public Form1 frm1;
        public Form2 frm2;
        private Dictionary<string, decimal> orijinalFiyatlar = new Dictionary<string, decimal>();
        public string GelenMusteriAdi { get; set; }
        public string GelenGsmTelefonu { get; set; }
        public string GelenDevredenBorc { get; set; }



        private DataTable dt; // dataGridView1 DataSource
        private ContextMenuStrip contextMenuStrip2; // Sağ tık menüsü

        public Satış_İşlemleri()
        {
            InitializeComponent();
            textBox2.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox4.ReadOnly = true;
            panel3.Visible = false;
            button1.Visible = false;
            button4.Visible = false;
            button6.Visible = false;
            button3.Visible = false;
            button5.Visible = false;
            button7.Visible = false;
            textBox8.ReadOnly = true;
            textBox8.Visible = false;
            textBox7.ReadOnly = true;
            textBox7.Visible = false;
            button3.Enabled = false;
            textBox19.ReadOnly = true;
            textBox13.ReadOnly = true;
            textBox17.ReadOnly = true;
            panel4.Visible = false;
            button7.Enabled = false;
            button15.Visible = false;
            checkBox2.Visible = false;


            textBox18.KeyPress += TextBox_Sayi_KeyPress;
            textBox15.KeyPress += TextBox_Sayi_KeyPress;
            textBox21.KeyPress += TextBox_Sayi_KeyPress;
            textBox9.KeyPress += TextBox_Sayi_KeyPress;
            textBox6.KeyPress += TextBox_Sayi_KeyPress;
            button9.Visible = false;
            button2.Visible = false;
            this.Load += Satış_İşlemleri_Load;

            textBox13.TextChanged += (sender, e) => KontrolEtVePanel4Ac();
            textBox17.TextChanged += (sender, e) => KontrolEtVePanel4Ac();
            textBox19.TextChanged += (sender, e) => KontrolEtVePanel4Ac();


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

            textBox3.KeyPress += (s, e) =>
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
                if (e.KeyChar == ',' || e.KeyChar == '.')
                {
                    if (string.IsNullOrEmpty(textBox3.Text) || textBox3.Text.Contains(",") || textBox3.Text.Contains("."))
                    {
                        e.Handled = true;
                        return;
                    }
                    e.Handled = false;
                    return;
                }
                e.Handled = true;
            };

            // Olayları bağlama
            dataGridView2.CellValueChanged += dataGridView2_CellValueChanged;
            dataGridView2.EditingControlShowing += dataGridView2_EditingControlShowing;
            dataGridView2.CellEndEdit += dataGridView2_CellEndEdit;
            dataGridView2.CellMouseDown += dataGridView2_CellMouseDown;

            // Sağ tık menüsü
            contextMenuStrip2 = new ContextMenuStrip();
            ToolStripMenuItem removeItem = new ToolStripMenuItem("Ürünü Kaldır");
            removeItem.Click += new EventHandler(removeItem_Click);
            contextMenuStrip2.Items.Add(removeItem);

            Listele();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void KontrolEtVePanel4Ac()
        {
            // Üç TextBox'ın da içeriğinin dolu olup olmadığını kontrol et
            if (!string.IsNullOrEmpty(textBox13.Text) &&
                !string.IsNullOrEmpty(textBox17.Text) &&
                !string.IsNullOrEmpty(textBox19.Text))
            {
                // Hepsi doluysa, paneli görünür yap
                panel4.Visible = true;
            }
            else
            {
                // Herhangi biri boşsa, paneli gizle
                panel4.Visible = false;
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

                if (result == DialogResult.Yes)
                {
                    // Ürünü DataTable'a geri ekle (dt null değilse)
                    if (dt != null)
                    {
                        DataRow newRow = dt.NewRow();
                        string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";

                        newRow["Barkod_No"] = barkodNo;
                        newRow["Ürün_Adi"] = row.Cells["Ürün_Adi"].Value;
                        newRow["Ürün_Grubu"] = row.Cells["Ürün_Grubu"].Value;
                        newRow["Stok_Miktari"] = row.Cells["Stok_Miktari"].Value;
                        newRow["OlcuBirimi"] = row.Cells["OlcuBirimi"].Value;

                        // Orijinal fiyatı kullan
                        newRow["Satis_Fiyati"] = orijinalFiyatlar.ContainsKey(barkodNo) ?
                            orijinalFiyatlar[barkodNo] : row.Cells["Satis_Fiyati"].Value;

                        newRow["Alis_Fiyati"] = row.Cells["Alis_Fiyati"].Value;
                        newRow["2SatisFiyati"] = row.Cells["2SatisFiyati"].Value;
                        newRow["AsgariStok"] = row.Cells["AsgariStok"].Value;

                        dt.Rows.Add(newRow);
                    }

                    dataGridView2.Rows.Remove(row);
                    HesaplaParaUstuVeKar();
                    UpdateButton2Visibility();
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

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // boş - sağ tık ile kaldırma kullanılıyor
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

        private void Satış_İşlemleri_Load(object sender, EventArgs e)
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



            UpdateButton2Visibility();
            UrunGrubuDoldur();
            Filtrele();
            // MusteriSecim'den gelen verileri ilgili kutulara aktarır
            if (!string.IsNullOrEmpty(GelenMusteriAdi))
            {
                textBox13.Text = GelenMusteriAdi;
            }
            if (!string.IsNullOrEmpty(GelenGsmTelefonu))
            {
                textBox17.Text = GelenGsmTelefonu;
            }
            if (!string.IsNullOrEmpty(GelenDevredenBorc))
            {
                textBox19.Text = GelenDevredenBorc;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            textBox7.Text = DateTime.Now.ToShortDateString();
            textBox8.Text = DateTime.Now.ToLongTimeString();
            textBox1.KeyDown += textBox1_KeyDown;
            textBox6.Text = "1";

            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.ReadOnly = true;
            }
            if (dataGridView2.Columns.Contains("Satis_Fiyati")) dataGridView2.Columns["Satis_Fiyati"].ReadOnly = false;
            if (dataGridView2.Columns.Contains("Miktar")) dataGridView2.Columns["Miktar"].ReadOnly = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;

            DataGridViewRow clickedRow = dataGridView1.Rows[e.RowIndex];
            DataGridViewRow newRow = new DataGridViewRow();
            newRow.CreateCells(dataGridView2);

            newRow.Cells[0].Value = clickedRow.Cells["Barkod_No"].Value;
            newRow.Cells[1].Value = clickedRow.Cells["Ürün_Adi"].Value;
            newRow.Cells[2].Value = clickedRow.Cells["Ürün_Grubu"].Value;
            newRow.Cells[3].Value = clickedRow.Cells["Stok_Miktari"].Value;
            newRow.Cells[4].Value = clickedRow.Cells["OlcuBirimi"].Value;
            newRow.Cells[5].Value = clickedRow.Cells["Satis_Fiyati"].Value; // Orijinal Satis_Fiyati
            newRow.Cells[6].Value = clickedRow.Cells["Alis_Fiyati"].Value;
            newRow.Cells[7].Value = clickedRow.Cells["2SatisFiyati"].Value;
            newRow.Cells[8].Value = clickedRow.Cells["AsgariStok"].Value;

            decimal miktar = TryParseDecimal(textBox6.Text);
            newRow.Cells[9].Value = miktar;
            decimal satisFiyati = TryParseDecimal(clickedRow.Cells["Satis_Fiyati"].Value);
            newRow.Cells[10].Value = (satisFiyati * miktar).ToString("0.00", CultureInfo.InvariantCulture);

            dataGridView2.Rows.Add(newRow);
            textBox6.Text = "1";

            string barkodNo = clickedRow.Cells["Barkod_No"].Value.ToString();
            DataRow[] rowsToRemove = dt.Select(string.Format("[Barkod_No] = '{0}'", barkodNo));
            if (rowsToRemove.Length > 0)
                dt.Rows.Remove(rowsToRemove[0]);

            UpdateButton2Visibility();
            HesaplaParaUstuVeKar();


            /*  if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;

            DataGridViewRow clickedRow = dataGridView1.Rows[e.RowIndex];
            DataGridViewRow newRow = new DataGridViewRow();
            newRow.CreateCells(dataGridView2);

            newRow.Cells[0].Value = clickedRow.Cells["Barkod_No"].Value;
            newRow.Cells[1].Value = clickedRow.Cells["Ürün_Adi"].Value;
            newRow.Cells[2].Value = clickedRow.Cells["Ürün_Grubu"].Value;
            newRow.Cells[3].Value = clickedRow.Cells["Stok_Miktari"].Value;
            newRow.Cells[4].Value = clickedRow.Cells["OlcuBirimi"].Value;
            newRow.Cells[5].Value = clickedRow.Cells["Satis_Fiyati"].Value; // Orijinal Satis_Fiyati
            newRow.Cells[6].Value = clickedRow.Cells["Alis_Fiyati"].Value;
            newRow.Cells[7].Value = clickedRow.Cells["2SatisFiyati"].Value;
            newRow.Cells[8].Value = clickedRow.Cells["AsgariStok"].Value;

            decimal miktar = TryParseDecimal(textBox6.Text);
            newRow.Cells[9].Value = miktar;
            decimal satisFiyati = TryParseDecimal(clickedRow.Cells["Satis_Fiyati"].Value);
            newRow.Cells[10].Value = (satisFiyati * miktar).ToString("0.00", CultureInfo.InvariantCulture);

            // Orijinal satış fiyatını Tag'e kaydet
            newRow.Tag = satisFiyati.ToString(CultureInfo.InvariantCulture); // Orijinal fiyatı Tag'e kaydet

            dataGridView2.Rows.Add(newRow);
            textBox6.Text = "1";
            textBox6.Visible = true;
            label6.Visible = true;
            label9.Visible = true;
            textBox2.Visible = true;
            string barkodNo = clickedRow.Cells["Barkod_No"].Value.ToString();
            DataRow[] rowsToRemove = dt.Select(string.Format("[Barkod_No] = '{0}'", barkodNo));
            if (rowsToRemove.Length > 0)
                dt.Rows.Remove(rowsToRemove[0]);

            UpdateButton2Visibility();
            HesaplaParaUstuVeKar();*/
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
                    decimal miktar = TryParseDecimal(textBox6.Text);
                    dataGridView2.Rows[index].Cells["ToplamTutar"].Value = (satisFiyati * miktar).ToString("0.00", CultureInfo.InvariantCulture);
                    textBox6.Text = "1";
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

        private void HesaplaParaUstuVeKar()
        {
            if (dataGridView2.Rows.Count == 0)
            {
                textBox2.Text = "0.00";
                textBox4.Text = "0.00";
                textBox5.Text = "0.00";
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
            decimal alinanPara = TryParseDecimal(textBox3.Text);
            decimal paraUstu = Math.Round(alinanPara - genelToplam, 2);
            textBox2.Text = genelToplam.ToString("0.00", CultureInfo.InvariantCulture);
            textBox4.Text = paraUstu.ToString("0.00", CultureInfo.InvariantCulture);
            textBox5.Text = toplamKar.ToString("0.00", CultureInfo.InvariantCulture);
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
        public void StokListesiniYenile()
        {
            try
            {
                string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
                using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
                {
                    string urunCekmeSorgu = "SELECT * FROM ÜrünGirişi";
                    using (OleDbDataAdapter da = new OleDbDataAdapter(urunCekmeSorgu, baglan))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stok listesi güncellenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void UpdateButton2Visibility()
        {
            bool isVisible = dataGridView2.Rows.Count > 0;
            button2.Visible = isVisible;
            label9.Visible = isVisible;
            label5.Visible = isVisible;
            label6.Visible = isVisible;
            label7.Visible = isVisible;
            label8.Visible = isVisible;
            textBox2.Visible = isVisible;
            textBox3.Visible = isVisible;
            textBox4.Visible = isVisible;
            textBox5.Visible = isVisible;
            textBox6.Visible = isVisible;
            button1.Visible = isVisible;
            button4.Visible = isVisible;
            button6.Visible = isVisible;
            button3.Visible = isVisible;
            button5.Visible = isVisible;
            button7.Visible = isVisible;
            button15.Visible = isVisible;
            checkBox2.Visible = isVisible;
            button9.Visible = isVisible;
            button2.Visible = isVisible;
        }
        public void DataGridView2Temizle()
        {

            // Sepetteki ürünleri temizle
            dataGridView2.Rows.Clear();

            // Müşteri bilgisi ve diğer ilgili text kutularını temizle
            textBox13.Text = string.Empty;
            textBox17.Text = string.Empty;
            textBox19.Text = string.Empty;
            textBox2.Text = "0,00"; // Toplam tutarı sıfırla
            textBox21.Text = "0,00"; // Para üstünü sıfırla
            textBox20.Text = "0,00"; // Karı sıfırla

            // Gerekli panelleri ve butonları sıfırla
            panel4.Visible = false;
            button3.Enabled = false;
            button7.Enabled = false;

            // Diğer görsel güncellemeleri yap
            UpdateButton2Visibility();
            HesaplaParaUstuVeKar();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0) return;
            DialogResult result = MessageBox.Show(
                "Satışı iptal etmek istediğinize emin misiniz? (Seçili müşteri de iptal edilecektir)",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (!row.IsNewRow && dt != null)
                    {
                        DataRow newRow = dt.NewRow();
                        string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";

                        newRow["Barkod_No"] = barkodNo;
                        newRow["Ürün_Adi"] = row.Cells["Ürün_Adi"].Value;
                        newRow["Ürün_Grubu"] = row.Cells["Ürün_Grubu"].Value;
                        newRow["Stok_Miktari"] = row.Cells["Stok_Miktari"].Value;
                        newRow["OlcuBirimi"] = row.Cells["OlcuBirimi"].Value;

                        // Orijinal fiyatı kullan
                        newRow["Satis_Fiyati"] = orijinalFiyatlar.ContainsKey(barkodNo) ?
                            orijinalFiyatlar[barkodNo] : row.Cells["Satis_Fiyati"].Value;

                        newRow["Alis_Fiyati"] = row.Cells["Alis_Fiyati"].Value;
                        newRow["2SatisFiyati"] = row.Cells["2SatisFiyati"].Value;
                        newRow["AsgariStok"] = row.Cells["AsgariStok"].Value;

                        dt.Rows.Add(newRow);
                    }
                }
                dataGridView2.Rows.Clear();
                textBox6.Visible = false; // Satış iptal edildiğinde gizle
                label6.Visible = false;   // Satış iptal edildiğinde gizle
                textBox6.Text = "";
                textBox2.Visible = false;
                IskontoIptalVeGeriYukle();
                UpdateButton2Visibility();
                HesaplaParaUstuVeKar();
            }
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
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            HesaplaParaUstuVeKar();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
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
            HesaplaParaUstuVeKar();
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


        private void button1_Click(object sender, EventArgs e)
        {
            // Sepette ürün olup olmadığını kontrol et
            if (dataGridView2.Rows.Count == 0 || (dataGridView2.Rows.Count == 1 && dataGridView2.Rows[0].IsNewRow))
            {
                MessageBox.Show("Sepette ürün bulunmamaktadır. Lütfen satış yapmak için ürün ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Stoğu kontrol et
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.IsNewRow) continue;
                decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                decimal kalanStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                if (satilanMiktar > (kalanStok + 0.0001m))
                {
                    DialogResult dr = MessageBox.Show(
                        $"'{row.Cells["Ürün_Adi"].Value}' adlı ürünün satılan miktarı ({satilanMiktar}) mevcut stoktan ({kalanStok}) fazla.\n" +
                        "Eğer devam ederseniz stok 0 olarak ayarlanacaktır. Devam edilsin mi?",
                        "Stok Yetersiz",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                    if (dr != DialogResult.Yes) return;
                }
            }

            // Satış onayı
            DialogResult result = MessageBox.Show(
                "Satış işlemini tamamlamak istediğinize emin misiniz?",
                "Satış Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result != DialogResult.Yes) return;

            string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
            using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
            {
                baglan.Open();
                OleDbTransaction transaction = baglan.BeginTransaction();
                try
                {
                    string satisSorgu = "INSERT INTO UrunSatis (Barkod_No, Urun_Adi, UrunGrubu, KalanStok, OlcuBirimi, Satis_Fiyati, Alis_Fiyati, [2SatisFiyati], AsgariStok, SatilanMiktar, ToplamTutar, Tarih, Saat, SatisTuru) VALUES (@BarkodNo, @UrunAdi, @UrunGrubu, @KalanStok, @OlcuBirimi, @SatisFiyati, @AlisFiyati, @IkinciSatisFiyati, @AsgariStok, @SatilanMiktar, @ToplamTutar, @Tarih, @Saat, @SatisTuru)";
                    string stokGuncellemeSorgu = "UPDATE ÜrünGirişi SET Stok_Miktari = @YeniStokMiktari WHERE Barkod_No = @BarkodNo";

                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                        decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                        decimal mevcutStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                        decimal yeniStokMiktari = mevcutStok - satilanMiktar;
                        if (yeniStokMiktari < 0m) yeniStokMiktari = 0m;

                        using (OleDbCommand satisCmd = new OleDbCommand(satisSorgu, baglan, transaction))
                        {
                            satisCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            satisCmd.Parameters.AddWithValue("@UrunAdi", row.Cells["Ürün_Adi"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@UrunGrubu", row.Cells["Ürün_Grubu"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@KalanStok", yeniStokMiktari);
                            satisCmd.Parameters.AddWithValue("@OlcuBirimi", row.Cells["OlcuBirimi"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@SatisFiyati", TryParseDecimal(row.Cells["Satis_Fiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@AlisFiyati", TryParseDecimal(row.Cells["Alis_Fiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@IkinciSatisFiyati", TryParseDecimal(row.Cells["2SatisFiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@AsgariStok", TryParseDecimal(row.Cells["AsgariStok"].Value));
                            satisCmd.Parameters.AddWithValue("@SatilanMiktar", satilanMiktar);
                            satisCmd.Parameters.AddWithValue("@ToplamTutar", TryParseDecimal(row.Cells["ToplamTutar"].Value));
                            satisCmd.Parameters.AddWithValue("@Tarih", DateTime.Now.ToShortDateString());
                            satisCmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToLongTimeString());
                            satisCmd.Parameters.AddWithValue("@SatisTuru", "Nakit");
                            satisCmd.ExecuteNonQuery();
                        }

                        using (OleDbCommand guncellemeCmd = new OleDbCommand(stokGuncellemeSorgu, baglan, transaction))
                        {
                            guncellemeCmd.Parameters.AddWithValue("@YeniStokMiktari", yeniStokMiktari);
                            guncellemeCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            guncellemeCmd.ExecuteNonQuery();
                        }
                    }

                    // MusteriSatis tablosuna veri ekleme
                    // Sadece ilgili textbox'lar dolu ise ekleme yapsın
                    if (!string.IsNullOrWhiteSpace(textBox13.Text) || !string.IsNullOrWhiteSpace(textBox17.Text) || !string.IsNullOrWhiteSpace(textBox19.Text))
                    {
                        string musteriSatisSorgu = "INSERT INTO MusteriSatis (MusteriAdi, GsmTelefon, Barkod_No, Urun_Adi, Stok_Miktari, AsgariStok, OlcuBirimi, Satis_Fiyati, SatilanMiktar, ToplamTutar, SatisTuru, Tarih, Saat) VALUES (@MusteriAdi, @GsmTelefon, @BarkodNo, @UrunAdi, @StokMiktari, @AsgariStok, @OlcuBirimi, @SatisFiyati, @SatilanMiktar, @ToplamTutar, @SatisTuru, @Tarih, @Saat)";

                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.IsNewRow) continue;

                            string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                            decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                            decimal mevcutStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                            decimal yeniStokMiktari = mevcutStok - satilanMiktar;
                            if (yeniStokMiktari < 0m) yeniStokMiktari = 0m;
                            decimal asgariStok = TryParseDecimal(row.Cells["AsgariStok"].Value);

                            using (OleDbCommand musteriSatisCmd = new OleDbCommand(musteriSatisSorgu, baglan, transaction))
                            {
                                musteriSatisCmd.Parameters.AddWithValue("@MusteriAdi", textBox13.Text);
                                musteriSatisCmd.Parameters.AddWithValue("@GsmTelefon", textBox17.Text);
                                musteriSatisCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                                musteriSatisCmd.Parameters.AddWithValue("@UrunAdi", row.Cells["Ürün_Adi"].Value?.ToString() ?? "");
                                musteriSatisCmd.Parameters.AddWithValue("@StokMiktari", yeniStokMiktari); // Kalan stok değeri eklendi
                                musteriSatisCmd.Parameters.AddWithValue("@AsgariStok", asgariStok);
                                musteriSatisCmd.Parameters.AddWithValue("@OlcuBirimi", row.Cells["OlcuBirimi"].Value?.ToString() ?? "");
                                musteriSatisCmd.Parameters.AddWithValue("@SatisFiyati", TryParseDecimal(row.Cells["Satis_Fiyati"].Value));
                                musteriSatisCmd.Parameters.AddWithValue("@SatilanMiktar", satilanMiktar);
                                musteriSatisCmd.Parameters.AddWithValue("@ToplamTutar", TryParseDecimal(row.Cells["ToplamTutar"].Value).ToString("N2"));
                                musteriSatisCmd.Parameters.AddWithValue("@SatisTuru", "Nakit");
                                musteriSatisCmd.Parameters.AddWithValue("@Tarih", DateTime.Now.ToShortDateString());
                                musteriSatisCmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToLongTimeString());

                                musteriSatisCmd.ExecuteNonQuery();
                            }
                        }
                        MessageBox.Show("Müşteri satışı başarıyla gerçekleşti!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    transaction.Commit();
                    MessageBox.Show("Satış işlemi başarıyla tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    // İşletme bilgilerini al
                    string isletmeAdi = "", isletmeAdresi = "", isletmeYeri = "", gsmTelefon = "";
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

                    float pageWidth = 315;
                    float padding = 5;

                    // Kağıt yüksekliği tahmini
                    float paperHeight = 0;
                    paperHeight += 20; // Üst boşluk
                    paperHeight += 15 * 5; // İşletme bilgileri ve aralarındaki boşluklar
                    paperHeight += 10; // Çizgi
                    paperHeight += 25; // Tarih ve saat
                    paperHeight += 20; // Sütun başlıkları
                    paperHeight += 5; // Sütun çizgisi

                    using (var bmp = new Bitmap(1, 1))
                    using (Graphics g = Graphics.FromImage(bmp))
                    using (System.Drawing.Font trFont = new System.Drawing.Font("Arial", 8))
                    using (System.Drawing.Font trFontSemiBold = new System.Drawing.Font("Arial", 8, FontStyle.Regular))
                    {
                        float urunBilgiWidth = pageWidth * 0.55f - padding; // Ürün genişliğini artırarak miktar sütununu sola çek
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

                    // Yazıcı fişi
                    PrintDocument pd = new PrintDocument();
                    pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)pageWidth, (int)Math.Ceiling(paperHeight));
                    pd.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

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

                            // Tarih ve saat (saat daha da sola çekildi)
                            string dateText = $"Tarih: {DateTime.Now.ToShortDateString()}";
                            string timeText = $"Saat: {DateTime.Now.ToLongTimeString()}";
                            float halfWidth = (_pageWidth - 2 * padding) / 2;
                            ev.Graphics.DrawString(dateText, trFont, Brushes.Black,
                                new RectangleF(padding, yPos, halfWidth - 5, 15), leftFormat);
                            ev.Graphics.DrawString(timeText, trFont, Brushes.Black,
                                new RectangleF(padding + halfWidth - 15, yPos, halfWidth, 15), rightFormat);
                            yPos += 25;

                            // Sütun başlıkları
                            ev.Graphics.DrawString("Barkod No / Ürün Adı", trFontBold, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth * 0.45f - padding, 15), leftFormat);
                            ev.Graphics.DrawString("Miktar Fiyatı", trFontBold, Brushes.Black,
                                new RectangleF(padding + _pageWidth * 0.45f - 5, yPos, _pageWidth * 0.25f, 15), centerFormat);
                            ev.Graphics.DrawString("Toplam", trFontBold, Brushes.Black,
                                new RectangleF(padding + _pageWidth * 0.7f, yPos, _pageWidth * 0.3f - padding, 15), leftFormat);
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

                                ev.Graphics.DrawString(barkodUrunAdi, trFontSemiBold, Brushes.Black,
                                    new RectangleF(padding, yPos, urunBilgiWidth, totalBlockHeight), leftFormat);
                                ev.Graphics.DrawString(details, trFont, Brushes.Black,
                                    new RectangleF(padding + urunBilgiWidth - 5, yPos, miktarFiyatWidth, totalBlockHeight), centerFormat);
                                ev.Graphics.DrawString(toplamTutar, trFont, Brushes.Black,
                                    new RectangleF(padding + urunBilgiWidth + miktarFiyatWidth, yPos, toplamWidth, totalBlockHeight), leftFormat);

                                yPos += totalBlockHeight;
                            }

                            // Çizgi
                            ev.Graphics.DrawLine(Pens.Black, padding, yPos, _pageWidth - padding, yPos);
                            yPos += 10;

                            // Genel toplam
                            ev.Graphics.DrawString($"GENEL TOPLAM: {TryParseDecimal(textBox2.Text):N2} TL", trFontBold, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                            yPos += 20;

                            // Bilgilendirme yazısı
                            ev.Graphics.DrawString("BİLGİ AMAÇLIDIR MALİ DEĞERİ YOKTUR", trFontItalic, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                            yPos += 5;
                        }
                    };

                    if (checkBox2.Checked)
                    {
                        pd.Print();
                    }

                    // Asgari stok kontrolü
                    using (OleDbCommand cmd = new OleDbCommand("SELECT Barkod_No, Stok_Miktari, AsgariStok FROM ÜrünGirişi WHERE Barkod_No = @BarkodNo", baglan))
                    {
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.IsNewRow) continue;
                            string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    decimal stokMiktari = TryParseDecimal(reader["Stok_Miktari"]);
                                    decimal asgariStok = TryParseDecimal(reader["AsgariStok"]);
                                    if (asgariStok > 0 && stokMiktari <= asgariStok)
                                    {
                                        MessageBox.Show($"'{row.Cells["Ürün_Adi"].Value}' ürününün stoğu asgari seviyenin altına düştü: {stokMiktari} (Asgari: {asgariStok})", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                        }
                    }

                    // Formu sıfırla ve yeniden yükle
                    dataGridView2.Rows.Clear();
                    textBox2.Text = "0.00";
                    textBox3.Text = "0.00";
                    textBox4.Text = "0.00";
                    textBox5.Text = "0.00";
                    textBox6.Text = "1";
                    panel4.Visible = false;
                    textBox13.Text = "";
                    textBox17.Text = "";
                    textBox19.Text = "";
                    Listele();
                    Filtrele();
                    UpdateButton2Visibility();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Satış işlemi sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            // Sepette ürün olup olmadığını kontrol et
            if (dataGridView2.Rows.Count == 0 || (dataGridView2.Rows.Count == 1 && dataGridView2.Rows[0].IsNewRow))
            {
                MessageBox.Show("Sepette ürün bulunmamaktadır. Lütfen satış yapmak için ürün ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Stoğu kontrol et
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.IsNewRow) continue;
                decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                decimal kalanStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                if (satilanMiktar > (kalanStok + 0.0001m))
                {
                    DialogResult dr = MessageBox.Show(
                        $"'{row.Cells["Ürün_Adi"].Value}' adlı ürünün satılan miktarı ({satilanMiktar}) mevcut stoktan ({kalanStok}) fazla.\n" +
                        "Eğer devam ederseniz stok 0 olarak ayarlanacaktır. Devam edilsin mi?",
                        "Stok Yetersiz",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                    if (dr != DialogResult.Yes) return;
                }
            }

            // Satış onayı
            DialogResult result = MessageBox.Show(
                "Satış işlemini tamamlamak istediğinize emin misiniz?",
                "Satış Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result != DialogResult.Yes) return;

            string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
            using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
            {
                baglan.Open();
                OleDbTransaction transaction = baglan.BeginTransaction();
                try
                {
                    string satisSorgu = "INSERT INTO UrunSatis (Barkod_No, Urun_Adi, UrunGrubu, KalanStok, OlcuBirimi, Satis_Fiyati, Alis_Fiyati, [2SatisFiyati], AsgariStok, SatilanMiktar, ToplamTutar, Tarih, Saat, SatisTuru) VALUES (@BarkodNo, @UrunAdi, @UrunGrubu, @KalanStok, @OlcuBirimi, @SatisFiyati, @AlisFiyati, @IkinciSatisFiyati, @AsgariStok, @SatilanMiktar, @ToplamTutar, @Tarih, @Saat, @SatisTuru)";
                    string stokGuncellemeSorgu = "UPDATE ÜrünGirişi SET Stok_Miktari = @YeniStokMiktari WHERE Barkod_No = @BarkodNo";

                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                        decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                        decimal mevcutStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                        decimal yeniStokMiktari = mevcutStok - satilanMiktar;
                        if (yeniStokMiktari < 0m) yeniStokMiktari = 0m;

                        using (OleDbCommand satisCmd = new OleDbCommand(satisSorgu, baglan, transaction))
                        {
                            satisCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            satisCmd.Parameters.AddWithValue("@UrunAdi", row.Cells["Ürün_Adi"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@UrunGrubu", row.Cells["Ürün_Grubu"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@KalanStok", yeniStokMiktari);
                            satisCmd.Parameters.AddWithValue("@OlcuBirimi", row.Cells["OlcuBirimi"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@SatisFiyati", TryParseDecimal(row.Cells["Satis_Fiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@AlisFiyati", TryParseDecimal(row.Cells["Alis_Fiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@IkinciSatisFiyati", TryParseDecimal(row.Cells["2SatisFiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@AsgariStok", TryParseDecimal(row.Cells["AsgariStok"].Value));
                            satisCmd.Parameters.AddWithValue("@SatilanMiktar", satilanMiktar);
                            satisCmd.Parameters.AddWithValue("@ToplamTutar", TryParseDecimal(row.Cells["ToplamTutar"].Value));
                            satisCmd.Parameters.AddWithValue("@Tarih", DateTime.Now.ToShortDateString());
                            satisCmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToLongTimeString());
                            satisCmd.Parameters.AddWithValue("@SatisTuru", "Kredi Kartı");
                            satisCmd.ExecuteNonQuery();
                        }

                        using (OleDbCommand guncellemeCmd = new OleDbCommand(stokGuncellemeSorgu, baglan, transaction))
                        {
                            guncellemeCmd.Parameters.AddWithValue("@YeniStokMiktari", yeniStokMiktari);
                            guncellemeCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            guncellemeCmd.ExecuteNonQuery();
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(textBox13.Text) || !string.IsNullOrWhiteSpace(textBox17.Text) || !string.IsNullOrWhiteSpace(textBox19.Text))
                    {
                        string musteriSatisSorgu = "INSERT INTO MusteriSatis (MusteriAdi, GsmTelefon, Barkod_No, Urun_Adi, Stok_Miktari, AsgariStok, OlcuBirimi, Satis_Fiyati, SatilanMiktar, ToplamTutar, SatisTuru, Tarih, Saat) VALUES (@MusteriAdi, @GsmTelefon, @BarkodNo, @UrunAdi, @StokMiktari, @AsgariStok, @OlcuBirimi, @SatisFiyati, @SatilanMiktar, @ToplamTutar, @SatisTuru, @Tarih, @Saat)";

                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.IsNewRow) continue;

                            string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                            decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                            decimal mevcutStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                            decimal yeniStokMiktari = mevcutStok - satilanMiktar;
                            if (yeniStokMiktari < 0m) yeniStokMiktari = 0m;
                            decimal asgariStok = TryParseDecimal(row.Cells["AsgariStok"].Value);

                            using (OleDbCommand musteriSatisCmd = new OleDbCommand(musteriSatisSorgu, baglan, transaction))
                            {
                                musteriSatisCmd.Parameters.AddWithValue("@MusteriAdi", textBox13.Text);
                                musteriSatisCmd.Parameters.AddWithValue("@GsmTelefon", textBox17.Text);
                                musteriSatisCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                                musteriSatisCmd.Parameters.AddWithValue("@UrunAdi", row.Cells["Ürün_Adi"].Value?.ToString() ?? "");
                                musteriSatisCmd.Parameters.AddWithValue("@StokMiktari", yeniStokMiktari); // Kalan stok değeri eklendi
                                musteriSatisCmd.Parameters.AddWithValue("@AsgariStok", asgariStok);
                                musteriSatisCmd.Parameters.AddWithValue("@OlcuBirimi", row.Cells["OlcuBirimi"].Value?.ToString() ?? "");
                                musteriSatisCmd.Parameters.AddWithValue("@SatisFiyati", TryParseDecimal(row.Cells["Satis_Fiyati"].Value));
                                musteriSatisCmd.Parameters.AddWithValue("@SatilanMiktar", satilanMiktar);
                                musteriSatisCmd.Parameters.AddWithValue("@ToplamTutar", TryParseDecimal(row.Cells["ToplamTutar"].Value).ToString("N2"));
                                musteriSatisCmd.Parameters.AddWithValue("@SatisTuru", "Kredi Kartı");
                                musteriSatisCmd.Parameters.AddWithValue("@Tarih", DateTime.Now.ToShortDateString());
                                musteriSatisCmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToLongTimeString());

                                musteriSatisCmd.ExecuteNonQuery();
                            }
                        }
                        MessageBox.Show("Müşteri satışı başarıyla gerçekleşti!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }

                    transaction.Commit();
                    MessageBox.Show("Satış işlemi başarıyla tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    panel4.Visible = false;
                    textBox13.Text = "";
                    textBox17.Text = "";
                    textBox19.Text = "";
                    // İşletme bilgilerini al
                    string isletmeAdi = "", isletmeAdresi = "", isletmeYeri = "", gsmTelefon = "";
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

                    float pageWidth = 315;
                    float padding = 5;

                    // Kağıt yüksekliği tahmini
                    float paperHeight = 0;
                    paperHeight += 20; // Üst boşluk
                    paperHeight += 15 * 5; // İşletme bilgileri ve aralarındaki boşluklar
                    paperHeight += 10; // Çizgi
                    paperHeight += 25; // Tarih ve saat
                    paperHeight += 20; // Sütun başlıkları
                    paperHeight += 5; // Sütun çizgisi

                    using (var bmp = new Bitmap(1, 1))
                    using (Graphics g = Graphics.FromImage(bmp))
                    using (System.Drawing.Font trFont = new System.Drawing.Font("Arial", 8))
                    using (System.Drawing.Font trFontSemiBold = new System.Drawing.Font("Arial", 8, FontStyle.Regular))
                    {
                        float urunBilgiWidth = pageWidth * 0.55f - padding; // Ürün genişliğini artırarak miktar sütununu sola çek
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

                    // Yazıcı fişi
                    PrintDocument pd = new PrintDocument();
                    pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)pageWidth, (int)Math.Ceiling(paperHeight));
                    pd.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

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

                            // Tarih ve saat (saat daha da sola çekildi)
                            string dateText = $"Tarih: {DateTime.Now.ToShortDateString()}";
                            string timeText = $"Saat: {DateTime.Now.ToLongTimeString()}";
                            float halfWidth = (_pageWidth - 2 * padding) / 2;
                            ev.Graphics.DrawString(dateText, trFont, Brushes.Black,
                                new RectangleF(padding, yPos, halfWidth - 5, 15), leftFormat);
                            ev.Graphics.DrawString(timeText, trFont, Brushes.Black,
                                new RectangleF(padding + halfWidth - 15, yPos, halfWidth, 15), rightFormat);
                            yPos += 25;

                            // Sütun başlıkları
                            ev.Graphics.DrawString("Barkod No / Ürün Adı", trFontBold, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth * 0.45f - padding, 15), leftFormat);
                            ev.Graphics.DrawString("Miktar Fiyatı", trFontBold, Brushes.Black,
                                new RectangleF(padding + _pageWidth * 0.45f - 5, yPos, _pageWidth * 0.25f, 15), centerFormat);
                            ev.Graphics.DrawString("Toplam", trFontBold, Brushes.Black,
                                new RectangleF(padding + _pageWidth * 0.7f, yPos, _pageWidth * 0.3f - padding, 15), leftFormat);
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

                                ev.Graphics.DrawString(barkodUrunAdi, trFontSemiBold, Brushes.Black,
                                    new RectangleF(padding, yPos, urunBilgiWidth, totalBlockHeight), leftFormat);
                                ev.Graphics.DrawString(details, trFont, Brushes.Black,
                                    new RectangleF(padding + urunBilgiWidth - 5, yPos, miktarFiyatWidth, totalBlockHeight), centerFormat);
                                ev.Graphics.DrawString(toplamTutar, trFont, Brushes.Black,
                                    new RectangleF(padding + urunBilgiWidth + miktarFiyatWidth, yPos, toplamWidth, totalBlockHeight), leftFormat);

                                yPos += totalBlockHeight;
                            }

                            // Çizgi
                            ev.Graphics.DrawLine(Pens.Black, padding, yPos, _pageWidth - padding, yPos);
                            yPos += 10;

                            // Genel toplam
                            ev.Graphics.DrawString($"GENEL TOPLAM: {TryParseDecimal(textBox2.Text):N2} TL", trFontBold, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                            yPos += 20;

                            // Bilgilendirme yazısı
                            ev.Graphics.DrawString("BİLGİ AMAÇLIDIR MALİ DEĞERİ YOKTUR", trFontItalic, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                            yPos += 5;
                        }
                    };

                    if (checkBox2.Checked)
                    {
                        pd.Print();
                    }

                    // Asgari stok kontrolü
                    using (OleDbCommand cmd = new OleDbCommand("SELECT Barkod_No, Stok_Miktari, AsgariStok FROM ÜrünGirişi WHERE Barkod_No = @BarkodNo", baglan))
                    {
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.IsNewRow) continue;
                            string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    decimal stokMiktari = TryParseDecimal(reader["Stok_Miktari"]);
                                    decimal asgariStok = TryParseDecimal(reader["AsgariStok"]);
                                    if (asgariStok > 0 && stokMiktari <= asgariStok)
                                    {
                                        MessageBox.Show($"'{row.Cells["Ürün_Adi"].Value}' ürününün stoğu asgari seviyenin altına düştü: {stokMiktari} (Asgari: {asgariStok})", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                        }
                    }

                    // Formu sıfırla ve yeniden yükle
                    dataGridView2.Rows.Clear();
                    textBox2.Text = "0.00";
                    textBox3.Text = "0.00";
                    textBox4.Text = "0.00";
                    textBox5.Text = "0.00";
                    textBox6.Text = "1";
                    Listele();
                    Filtrele();
                    UpdateButton2Visibility();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Satış işlemi sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            // Sepette ürün olup olmadığını kontrol et
            if (dataGridView2.Rows.Count == 0 || (dataGridView2.Rows.Count == 1 && dataGridView2.Rows[0].IsNewRow))
            {
                MessageBox.Show("Sepette ürün bulunmamaktadır. Lütfen satış yapmak için ürün ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // InputBox kullanarak nakit tutarını al
            string toplamTutarStr = TryParseDecimal(textBox2.Text).ToString("N2");

            // Pencereyi aç ve kullanıcının girişini al
            string nakitTutarInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Lütfen Ödenen NAKİT Tutarı Giriniz",
                "NAKİT Satış Tutarı",
                toplamTutarStr
            );

            // Kullanıcı Cancel'a bastıysa veya boş bir değer girdiyse işlemi iptal et
            if (string.IsNullOrEmpty(nakitTutarInput))
            {
                MessageBox.Show("İşlem iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Girilen değeri ondalık sayıya çevir
            decimal nakitOdenen;
            if (!decimal.TryParse(nakitTutarInput.Replace(",", "."), out nakitOdenen))
            {
                MessageBox.Show("Geçerli bir tutar girmediniz. Lütfen sayısal bir değer girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- Buradan itibaren sizin mevcut button5_Click kodunuz devam ediyor ---

            // Stok kontrolü
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.IsNewRow) continue;
                decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                decimal kalanStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                if (satilanMiktar > (kalanStok + 0.0001m))
                {
                    DialogResult dr = MessageBox.Show(
                        $"'{row.Cells["Ürün_Adi"].Value}' adlı ürünün satılan miktarı ({satilanMiktar}) mevcut stoktan ({kalanStok}) fazla.\n" +
                        "Eğer devam ederseniz stok 0 olarak ayarlanacaktır. Devam edilsin mi?",
                        "Stok Yetersiz",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                    if (dr != DialogResult.Yes) return;
                }
            }

            // Satış onayı
            DialogResult result = MessageBox.Show(
                "Satış işlemini tamamlamak istediğinize emin misiniz?",
                "Satış Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result != DialogResult.Yes) return;

            string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
            using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
            {
                baglan.Open();
                OleDbTransaction transaction = baglan.BeginTransaction();
                try
                {
                    string satisSorgu = "INSERT INTO UrunSatis (Barkod_No, Urun_Adi, UrunGrubu, KalanStok, OlcuBirimi, Satis_Fiyati, Alis_Fiyati, [2SatisFiyati], AsgariStok, SatilanMiktar, ToplamTutar, Tarih, Saat, SatisTuru) VALUES (@BarkodNo, @UrunAdi, @UrunGrubu, @KalanStok, @OlcuBirimi, @SatisFiyati, @AlisFiyati, @IkinciSatisFiyati, @AsgariStok, @SatilanMiktar, @ToplamTutar, @Tarih, @Saat, @SatisTuru)";
                    string stokGuncellemeSorgu = "UPDATE ÜrünGirişi SET Stok_Miktari = @YeniStokMiktari WHERE Barkod_No = @BarkodNo";

                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                        decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                        decimal mevcutStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                        decimal yeniStokMiktari = mevcutStok - satilanMiktar;
                        if (yeniStokMiktari < 0m) yeniStokMiktari = 0m;

                        using (OleDbCommand satisCmd = new OleDbCommand(satisSorgu, baglan, transaction))
                        {
                            satisCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            satisCmd.Parameters.AddWithValue("@UrunAdi", row.Cells["Ürün_Adi"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@UrunGrubu", row.Cells["Ürün_Grubu"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@KalanStok", yeniStokMiktari);
                            satisCmd.Parameters.AddWithValue("@OlcuBirimi", row.Cells["OlcuBirimi"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@SatisFiyati", TryParseDecimal(row.Cells["Satis_Fiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@AlisFiyati", TryParseDecimal(row.Cells["Alis_Fiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@IkinciSatisFiyati", TryParseDecimal(row.Cells["2SatisFiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@AsgariStok", TryParseDecimal(row.Cells["AsgariStok"].Value));
                            satisCmd.Parameters.AddWithValue("@SatilanMiktar", satilanMiktar);
                            satisCmd.Parameters.AddWithValue("@ToplamTutar", TryParseDecimal(row.Cells["ToplamTutar"].Value));
                            satisCmd.Parameters.AddWithValue("@Tarih", DateTime.Now.ToShortDateString());
                            satisCmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToLongTimeString());
                            satisCmd.Parameters.AddWithValue("@SatisTuru", "Nakit + Kredi Kartı");
                            satisCmd.ExecuteNonQuery();
                        }

                        using (OleDbCommand guncellemeCmd = new OleDbCommand(stokGuncellemeSorgu, baglan, transaction))
                        {
                            guncellemeCmd.Parameters.AddWithValue("@YeniStokMiktari", yeniStokMiktari);
                            guncellemeCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            guncellemeCmd.ExecuteNonQuery();
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(textBox13.Text) || !string.IsNullOrWhiteSpace(textBox17.Text) || !string.IsNullOrWhiteSpace(textBox19.Text))
                    {
                        string musteriSatisSorgu = "INSERT INTO MusteriSatis (MusteriAdi, GsmTelefon, Barkod_No, Urun_Adi, Stok_Miktari, AsgariStok, OlcuBirimi, Satis_Fiyati, SatilanMiktar, ToplamTutar, SatisTuru, Tarih, Saat) VALUES (@MusteriAdi, @GsmTelefon, @BarkodNo, @UrunAdi, @StokMiktari, @AsgariStok, @OlcuBirimi, @SatisFiyati, @SatilanMiktar, @ToplamTutar, @SatisTuru, @Tarih, @Saat)";

                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.IsNewRow) continue;

                            string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                            decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                            decimal mevcutStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                            decimal yeniStokMiktari = mevcutStok - satilanMiktar;
                            if (yeniStokMiktari < 0m) yeniStokMiktari = 0m;
                            decimal asgariStok = TryParseDecimal(row.Cells["AsgariStok"].Value);

                            using (OleDbCommand musteriSatisCmd = new OleDbCommand(musteriSatisSorgu, baglan, transaction))
                            {
                                musteriSatisCmd.Parameters.AddWithValue("@MusteriAdi", textBox13.Text);
                                musteriSatisCmd.Parameters.AddWithValue("@GsmTelefon", textBox17.Text);
                                musteriSatisCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                                musteriSatisCmd.Parameters.AddWithValue("@UrunAdi", row.Cells["Ürün_Adi"].Value?.ToString() ?? "");
                                musteriSatisCmd.Parameters.AddWithValue("@StokMiktari", yeniStokMiktari);
                                musteriSatisCmd.Parameters.AddWithValue("@AsgariStok", asgariStok);
                                musteriSatisCmd.Parameters.AddWithValue("@OlcuBirimi", row.Cells["OlcuBirimi"].Value?.ToString() ?? "");
                                musteriSatisCmd.Parameters.AddWithValue("@SatisFiyati", TryParseDecimal(row.Cells["Satis_Fiyati"].Value));
                                musteriSatisCmd.Parameters.AddWithValue("@SatilanMiktar", satilanMiktar);
                                musteriSatisCmd.Parameters.AddWithValue("@ToplamTutar", TryParseDecimal(row.Cells["ToplamTutar"].Value).ToString("N2"));
                                musteriSatisCmd.Parameters.AddWithValue("@SatisTuru", "Nakit + Kredi Kartı");
                                musteriSatisCmd.Parameters.AddWithValue("@Tarih", DateTime.Now.ToShortDateString());
                                musteriSatisCmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToLongTimeString());

                                musteriSatisCmd.ExecuteNonQuery();
                            }

                        }
                        MessageBox.Show("Müşteri satışı başarıyla gerçekleşti!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    transaction.Commit();
                    MessageBox.Show("Satış işlemi başarıyla tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    panel4.Visible = false;
                    textBox13.Text = "";
                    textBox17.Text = "";
                    textBox19.Text = "";

                    // İşletme bilgilerini al
                    string isletmeAdi = "", isletmeAdresi = "", isletmeYeri = "", gsmTelefon = "";
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

                    float pageWidth = 315;
                    float padding = 5;

                    // Kağıt yüksekliği tahmini
                    float paperHeight = 0;
                    paperHeight += 20; // Üst boşluk
                    paperHeight += 15 * 5; // İşletme bilgileri ve aralarındaki boşluklar
                    paperHeight += 10; // Çizgi
                    paperHeight += 25; // Tarih ve saat
                    paperHeight += 20; // Sütun başlıkları
                    paperHeight += 5; // Sütun çizgisi

                    using (var bmp = new Bitmap(1, 1))
                    using (Graphics g = Graphics.FromImage(bmp))
                    using (System.Drawing.Font trFont = new System.Drawing.Font("Arial", 8))
                    using (System.Drawing.Font trFontSemiBold = new System.Drawing.Font("Arial", 8, FontStyle.Regular))
                    {
                        float urunBilgiWidth = pageWidth * 0.55f - padding; // Ürün genişliğini artırarak miktar sütununu sola çek
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

                    // Yazıcı fişi
                    PrintDocument pd = new PrintDocument();
                    pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)pageWidth, (int)Math.Ceiling(paperHeight));
                    pd.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

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

                            // Tarih ve saat (saat daha da sola çekildi)
                            string dateText = $"Tarih: {DateTime.Now.ToShortDateString()}";
                            string timeText = $"Saat: {DateTime.Now.ToLongTimeString()}";
                            float halfWidth = (_pageWidth - 2 * padding) / 2;
                            ev.Graphics.DrawString(dateText, trFont, Brushes.Black,
                                new RectangleF(padding, yPos, halfWidth - 5, 15), leftFormat);
                            ev.Graphics.DrawString(timeText, trFont, Brushes.Black,
                                new RectangleF(padding + halfWidth - 15, yPos, halfWidth, 15), rightFormat);
                            yPos += 25;

                            // Sütun başlıkları
                            ev.Graphics.DrawString("Barkod No / Ürün Adı", trFontBold, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth * 0.45f - padding, 15), leftFormat);
                            ev.Graphics.DrawString("Miktar Fiyatı", trFontBold, Brushes.Black,
                                new RectangleF(padding + _pageWidth * 0.45f - 5, yPos, _pageWidth * 0.25f, 15), centerFormat);
                            ev.Graphics.DrawString("Toplam", trFontBold, Brushes.Black,
                                new RectangleF(padding + _pageWidth * 0.7f, yPos, _pageWidth * 0.3f - padding, 15), leftFormat);
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

                                ev.Graphics.DrawString(barkodUrunAdi, trFontSemiBold, Brushes.Black,
                                    new RectangleF(padding, yPos, urunBilgiWidth, totalBlockHeight), leftFormat);
                                ev.Graphics.DrawString(details, trFont, Brushes.Black,
                                    new RectangleF(padding + urunBilgiWidth - 5, yPos, miktarFiyatWidth, totalBlockHeight), centerFormat);
                                ev.Graphics.DrawString(toplamTutar, trFont, Brushes.Black,
                                    new RectangleF(padding + urunBilgiWidth + miktarFiyatWidth, yPos, toplamWidth, totalBlockHeight), leftFormat);

                                yPos += totalBlockHeight;
                            }

                            // Çizgi
                            ev.Graphics.DrawLine(Pens.Black, padding, yPos, _pageWidth - padding, yPos);
                            yPos += 10;

                            // Genel toplam
                            ev.Graphics.DrawString($"GENEL TOPLAM: {TryParseDecimal(textBox2.Text):N2} TL", trFontBold, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                            yPos += 20;

                            // Bilgilendirme yazısı
                            ev.Graphics.DrawString("BİLGİ AMAÇLIDIR MALİ DEĞERİ YOKTUR", trFontItalic, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                            yPos += 5; // Altında 5 piksellik boşluk bırakıldı
                        }
                    };

                    if (checkBox2.Checked)
                    {
                        pd.Print();
                    }

                    // Asgari stok kontrolü
                    using (OleDbCommand cmd = new OleDbCommand("SELECT Barkod_No, Stok_Miktari, AsgariStok FROM ÜrünGirişi WHERE Barkod_No = @BarkodNo", baglan))
                    {
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.IsNewRow) continue;
                            string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    decimal stokMiktari = TryParseDecimal(reader["Stok_Miktari"]);
                                    decimal asgariStok = TryParseDecimal(reader["AsgariStok"]);
                                    if (asgariStok > 0 && stokMiktari <= asgariStok)
                                    {
                                        MessageBox.Show($"'{row.Cells["Ürün_Adi"].Value}' ürününün stoğu asgari seviyenin altına düştü: {stokMiktari} (Asgari: {asgariStok})", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                        }
                    }

                    // Formu sıfırla ve yeniden yükle
                    dataGridView2.Rows.Clear();
                    textBox2.Text = "0.00";
                    textBox3.Text = "0.00";
                    textBox4.Text = "0.00";
                    textBox5.Text = "0.00";
                    textBox6.Text = "1";
                    Listele();
                    Filtrele();
                    UpdateButton2Visibility();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Satış işlemi sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            // Sepette ürün olup olmadığını kontrol et
            if (dataGridView2.Rows.Count == 0 || (dataGridView2.Rows.Count == 1 && dataGridView2.Rows[0].IsNewRow))
            {
                MessageBox.Show("Sepette ürün bulunmamaktadır. Lütfen satış yapmak için ürün ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Stoğu kontrol et
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.IsNewRow) continue;
                decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                decimal kalanStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                if (satilanMiktar > (kalanStok + 0.0001m))
                {
                    DialogResult dr = MessageBox.Show(
                        $"'{row.Cells["Ürün_Adi"].Value}' adlı ürünün satılan miktarı ({satilanMiktar}) mevcut stoktan ({kalanStok}) fazla.\n" +
                        "Eğer devam ederseniz stok 0 olarak ayarlanacaktır. Devam edilsin mi?",
                        "Stok Yetersiz",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                    if (dr != DialogResult.Yes) return;
                }
            }

            // Satış onayı
            DialogResult result = MessageBox.Show(
                "Satış işlemini tamamlamak istediğinize emin misiniz?",
                "Satış Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result != DialogResult.Yes) return;

            string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
            using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
            {
                baglan.Open();
                OleDbTransaction transaction = baglan.BeginTransaction();
                try
                {
                    string satisSorgu = "INSERT INTO UrunSatis (Barkod_No, Urun_Adi, UrunGrubu, KalanStok, OlcuBirimi, Satis_Fiyati, Alis_Fiyati, [2SatisFiyati], AsgariStok, SatilanMiktar, ToplamTutar, Tarih, Saat, SatisTuru) VALUES (@BarkodNo, @UrunAdi, @UrunGrubu, @KalanStok, @OlcuBirimi, @SatisFiyati, @AlisFiyati, @IkinciSatisFiyati, @AsgariStok, @SatilanMiktar, @ToplamTutar, @Tarih, @Saat, @SatisTuru)";
                    string stokGuncellemeSorgu = "UPDATE ÜrünGirişi SET Stok_Miktari = @YeniStokMiktari WHERE Barkod_No = @BarkodNo";

                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                        decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                        decimal mevcutStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                        decimal yeniStokMiktari = mevcutStok - satilanMiktar;
                        if (yeniStokMiktari < 0m) yeniStokMiktari = 0m;

                        using (OleDbCommand satisCmd = new OleDbCommand(satisSorgu, baglan, transaction))
                        {
                            satisCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            satisCmd.Parameters.AddWithValue("@UrunAdi", row.Cells["Ürün_Adi"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@UrunGrubu", row.Cells["Ürün_Grubu"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@KalanStok", yeniStokMiktari);
                            satisCmd.Parameters.AddWithValue("@OlcuBirimi", row.Cells["OlcuBirimi"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@SatisFiyati", TryParseDecimal(row.Cells["Satis_Fiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@AlisFiyati", TryParseDecimal(row.Cells["Alis_Fiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@IkinciSatisFiyati", TryParseDecimal(row.Cells["2SatisFiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@AsgariStok", TryParseDecimal(row.Cells["AsgariStok"].Value));
                            satisCmd.Parameters.AddWithValue("@SatilanMiktar", satilanMiktar);
                            satisCmd.Parameters.AddWithValue("@ToplamTutar", TryParseDecimal(row.Cells["ToplamTutar"].Value));
                            satisCmd.Parameters.AddWithValue("@Tarih", DateTime.Now.ToShortDateString());
                            satisCmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToLongTimeString());
                            satisCmd.Parameters.AddWithValue("@SatisTuru", "Havale Satış");
                            satisCmd.ExecuteNonQuery();
                        }

                        using (OleDbCommand guncellemeCmd = new OleDbCommand(stokGuncellemeSorgu, baglan, transaction))
                        {
                            guncellemeCmd.Parameters.AddWithValue("@YeniStokMiktari", yeniStokMiktari);
                            guncellemeCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            guncellemeCmd.ExecuteNonQuery();
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(textBox13.Text) || !string.IsNullOrWhiteSpace(textBox17.Text) || !string.IsNullOrWhiteSpace(textBox19.Text))
                    {
                        string musteriSatisSorgu = "INSERT INTO MusteriSatis (MusteriAdi, GsmTelefon, Barkod_No, Urun_Adi, Stok_Miktari, AsgariStok, OlcuBirimi, Satis_Fiyati, SatilanMiktar, ToplamTutar, SatisTuru, Tarih, Saat) VALUES (@MusteriAdi, @GsmTelefon, @BarkodNo, @UrunAdi, @StokMiktari, @AsgariStok, @OlcuBirimi, @SatisFiyati, @SatilanMiktar, @ToplamTutar, @SatisTuru, @Tarih, @Saat)";

                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.IsNewRow) continue;

                            string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                            decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                            decimal mevcutStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                            decimal yeniStokMiktari = mevcutStok - satilanMiktar;
                            if (yeniStokMiktari < 0m) yeniStokMiktari = 0m;
                            decimal asgariStok = TryParseDecimal(row.Cells["AsgariStok"].Value);

                            using (OleDbCommand musteriSatisCmd = new OleDbCommand(musteriSatisSorgu, baglan, transaction))
                            {
                                musteriSatisCmd.Parameters.AddWithValue("@MusteriAdi", textBox13.Text);
                                musteriSatisCmd.Parameters.AddWithValue("@GsmTelefon", textBox17.Text);
                                musteriSatisCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                                musteriSatisCmd.Parameters.AddWithValue("@UrunAdi", row.Cells["Ürün_Adi"].Value?.ToString() ?? "");
                                musteriSatisCmd.Parameters.AddWithValue("@StokMiktari", yeniStokMiktari); // Kalan stok değeri eklendi
                                musteriSatisCmd.Parameters.AddWithValue("@AsgariStok", asgariStok);
                                musteriSatisCmd.Parameters.AddWithValue("@OlcuBirimi", row.Cells["OlcuBirimi"].Value?.ToString() ?? "");
                                musteriSatisCmd.Parameters.AddWithValue("@SatisFiyati", TryParseDecimal(row.Cells["Satis_Fiyati"].Value));
                                musteriSatisCmd.Parameters.AddWithValue("@SatilanMiktar", satilanMiktar);
                                musteriSatisCmd.Parameters.AddWithValue("@ToplamTutar", TryParseDecimal(row.Cells["ToplamTutar"].Value).ToString("N2"));
                                musteriSatisCmd.Parameters.AddWithValue("@SatisTuru", "Havale Satış");
                                musteriSatisCmd.Parameters.AddWithValue("@Tarih", DateTime.Now.ToShortDateString());
                                musteriSatisCmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToLongTimeString());

                                musteriSatisCmd.ExecuteNonQuery();
                            }
                        }
                        MessageBox.Show("Müşteri satışı başarıyla gerçekleşti!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    transaction.Commit();
                    MessageBox.Show("Satış işlemi başarıyla tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    panel4.Visible = false;
                    textBox13.Text = "";
                    textBox17.Text = "";
                    textBox19.Text = "";
                    // İşletme bilgilerini al
                    string isletmeAdi = "", isletmeAdresi = "", isletmeYeri = "", gsmTelefon = "";
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

                    float pageWidth = 315;
                    float padding = 5;

                    // Kağıt yüksekliği tahmini
                    float paperHeight = 0;
                    paperHeight += 20; // Üst boşluk
                    paperHeight += 15 * 5; // İşletme bilgileri ve aralarındaki boşluklar
                    paperHeight += 10; // Çizgi
                    paperHeight += 25; // Tarih ve saat
                    paperHeight += 20; // Sütun başlıkları
                    paperHeight += 5; // Sütun çizgisi

                    using (var bmp = new Bitmap(1, 1))
                    using (Graphics g = Graphics.FromImage(bmp))
                    using (System.Drawing.Font trFont = new System.Drawing.Font("Arial", 8))
                    using (System.Drawing.Font trFontSemiBold = new System.Drawing.Font("Arial", 8, FontStyle.Regular))
                    {
                        float urunBilgiWidth = pageWidth * 0.55f - padding; // Ürün genişliğini artırarak miktar sütununu sola çek
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

                    // Yazıcı fişi
                    PrintDocument pd = new PrintDocument();
                    pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)pageWidth, (int)Math.Ceiling(paperHeight));
                    pd.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

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

                            // Tarih ve saat (saat daha da sola çekildi)
                            string dateText = $"Tarih: {DateTime.Now.ToShortDateString()}";
                            string timeText = $"Saat: {DateTime.Now.ToLongTimeString()}";
                            float halfWidth = (_pageWidth - 2 * padding) / 2;
                            ev.Graphics.DrawString(dateText, trFont, Brushes.Black,
                                new RectangleF(padding, yPos, halfWidth - 5, 15), leftFormat);
                            ev.Graphics.DrawString(timeText, trFont, Brushes.Black,
                                new RectangleF(padding + halfWidth - 15, yPos, halfWidth, 15), rightFormat);
                            yPos += 25;

                            // Sütun başlıkları
                            ev.Graphics.DrawString("Barkod No / Ürün Adı", trFontBold, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth * 0.45f - padding, 15), leftFormat);
                            ev.Graphics.DrawString("Miktar Fiyatı", trFontBold, Brushes.Black,
                                new RectangleF(padding + _pageWidth * 0.45f - 5, yPos, _pageWidth * 0.25f, 15), centerFormat);
                            ev.Graphics.DrawString("Toplam", trFontBold, Brushes.Black,
                                new RectangleF(padding + _pageWidth * 0.7f, yPos, _pageWidth * 0.3f - padding, 15), leftFormat);
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

                                ev.Graphics.DrawString(barkodUrunAdi, trFontSemiBold, Brushes.Black,
                                    new RectangleF(padding, yPos, urunBilgiWidth, totalBlockHeight), leftFormat);
                                ev.Graphics.DrawString(details, trFont, Brushes.Black,
                                    new RectangleF(padding + urunBilgiWidth - 5, yPos, miktarFiyatWidth, totalBlockHeight), centerFormat);
                                ev.Graphics.DrawString(toplamTutar, trFont, Brushes.Black,
                                    new RectangleF(padding + urunBilgiWidth + miktarFiyatWidth, yPos, toplamWidth, totalBlockHeight), leftFormat);

                                yPos += totalBlockHeight;
                            }

                            // Çizgi
                            ev.Graphics.DrawLine(Pens.Black, padding, yPos, _pageWidth - padding, yPos);
                            yPos += 10;

                            // Genel toplam
                            ev.Graphics.DrawString($"GENEL TOPLAM: {TryParseDecimal(textBox2.Text):N2} TL", trFontBold, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                            yPos += 20;

                            // Bilgilendirme yazısı
                            ev.Graphics.DrawString("BİLGİ AMAÇLIDIR MALİ DEĞERİ YOKTUR", trFontItalic, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                            yPos += 5; // Altında 5 piksellik boşluk bırakıldı
                        }
                    };

                    if (checkBox2.Checked)
                    {
                        pd.Print();
                    }

                    // Asgari stok kontrolü
                    using (OleDbCommand cmd = new OleDbCommand("SELECT Barkod_No, Stok_Miktari, AsgariStok FROM ÜrünGirişi WHERE Barkod_No = @BarkodNo", baglan))
                    {
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.IsNewRow) continue;
                            string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    decimal stokMiktari = TryParseDecimal(reader["Stok_Miktari"]);
                                    decimal asgariStok = TryParseDecimal(reader["AsgariStok"]);
                                    if (asgariStok > 0 && stokMiktari <= asgariStok)
                                    {
                                        MessageBox.Show($"'{row.Cells["Ürün_Adi"].Value}' ürününün stoğu asgari seviyenin altına düştü: {stokMiktari} (Asgari: {asgariStok})", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                        }
                    }

                    // Formu sıfırla ve yeniden yükle
                    dataGridView2.Rows.Clear();
                    textBox2.Text = "0.00";
                    textBox3.Text = "0.00";
                    textBox4.Text = "0.00";
                    textBox5.Text = "0.00";
                    textBox6.Text = "1";
                    Listele();
                    Filtrele();
                    UpdateButton2Visibility();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Satış işlemi sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Sepette ürün olup olmadığını kontrol et
            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Taksitli satış işlemi için sepete ürün ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Taksitlendirme formundan bir örnek oluşturma
            // Taksitlendirme formuna bu formun (Satış_İşlemleri) referansını gönder
            Taksitlendirme taksitForm = new Taksitlendirme(this);

            // Diğer verileri aktarma
            taksitForm.MusteriAdi = textBox13.Text;
            taksitForm.GsmTelefon = textBox17.Text;
            taksitForm.FaturaKesilecekMi = checkBox2.Checked;

            // textBox2'deki değeri her durumda ondalıklı olarak yakalamak için
            decimal toplamTutar;
            if (decimal.TryParse(textBox2.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out toplamTutar))
            {
                taksitForm.ToplamTutar = toplamTutar;
            }
            else
            {
                MessageBox.Show("Toplam tutar geçerli bir sayı değil. Lütfen kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // DataGridView2'deki verileri DataTable'a aktar ve taksit formuna gönder
            DataTable sepet = new DataTable();

            foreach (DataGridViewColumn col in dataGridView2.Columns)
            {
                // Sütunun adını al.
                string columnName = col.Name;
                // Sütunun veri tipini al, eğer boşsa varsayılan olarak string tipini kullan.
                Type columnType = col.ValueType ?? typeof(string);

                // Yeni sütunu DataTable'a ekle.
                sepet.Columns.Add(columnName, columnType);
            }

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.IsNewRow) continue;
                DataRow newRow = sepet.NewRow();
                for (int i = 0; i < sepet.Columns.Count; i++)
                {
                    // Hücre değerinin null olma ihtimaline karşı kontrol et
                    newRow[i] = row.Cells[i].Value ?? DBNull.Value;
                }
                sepet.Rows.Add(newRow);
            }

            taksitForm.SepetVerisi = sepet;

            // Taksitlendirme formunu gösterme
            taksitForm.ShowDialog();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel3.Visible = true;
            button8.Visible = false;
            button13.Visible = false;

            // Açık olan MusteriSecim formunu bul ve kapat
            Form musteriSecimFormu = Application.OpenForms.OfType<MusteriSecim>().FirstOrDefault();
            if (musteriSecimFormu != null)
            {
                musteriSecimFormu.Close();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel3.Visible = false;
            button8.Visible = true;
            button13.Visible = true;

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

        private void button9_Click(object sender, EventArgs e)
        {
            // Sepette (dataGridView2) ürün olup olmadığını kontrol et
            if (dataGridView2.Rows.Count == 0 || (dataGridView2.Rows.Count == 1 && dataGridView2.Rows[0].IsNewRow))
            {
                MessageBox.Show("Fiş oluşturulacak ürün bulunmamaktadır. Lütfen sepete ürün ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi burada sonlandır
            }

            // İşletme bilgilerini al
            string isletmeAdi = "", isletmeAdresi = "", isletmeYeri = "", gsmTelefon = "";
            string isletmeSorgu = "SELECT IsletmeAdi, IsletmeAdresi, IsletmeYeri, GsmTelefon FROM IsletmeAdi";

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                       Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            using (OleDbConnection baglan = new OleDbConnection(connectionString))
            {
                baglan.Open();
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

            float pageWidth = 315;
            float padding = 5;

            // Kağıt yüksekliği tahmini
            float paperHeight = 0;
            paperHeight += 20; // Üst boşluk
            paperHeight += 15 * 5; // İşletme bilgileri ve aralarındaki boşluklar
            paperHeight += 10; // Çizgi
            paperHeight += 25; // Tarih ve saat
            paperHeight += 20; // Sütun başlıkları
            paperHeight += 5; // Sütun çizgisi

            using (var bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
            using (System.Drawing.Font trFont = new System.Drawing.Font("Arial", 8))
            using (System.Drawing.Font trFontSemiBold = new System.Drawing.Font("Arial", 8, FontStyle.Regular))
            {
                float urunBilgiWidth = pageWidth * 0.55f - padding; // Ürün genişliğini artırarak miktar sütununu sola çek
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

            // Yazıcı fişi
            PrintDocument pd = new PrintDocument();
            pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)pageWidth, (int)Math.Ceiling(paperHeight));
            pd.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

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

                    // Tarih ve saat (saat daha da sola çekildi)
                    string dateText = $"Tarih: {DateTime.Now.ToShortDateString()}";
                    string timeText = $"Saat: {DateTime.Now.ToLongTimeString()}";
                    float halfWidth = (_pageWidth - 2 * padding) / 2;
                    ev.Graphics.DrawString(dateText, trFont, Brushes.Black,
                        new RectangleF(padding, yPos, halfWidth - 5, 15), leftFormat);
                    ev.Graphics.DrawString(timeText, trFont, Brushes.Black,
                        new RectangleF(padding + halfWidth - 15, yPos, halfWidth, 15), rightFormat);
                    yPos += 25;

                    // Sütun başlıkları
                    ev.Graphics.DrawString("Barkod No / Ürün Adı", trFontBold, Brushes.Black,
                        new RectangleF(padding, yPos, _pageWidth * 0.45f - padding, 15), leftFormat);
                    ev.Graphics.DrawString("Miktar Fiyatı", trFontBold, Brushes.Black,
                        new RectangleF(padding + _pageWidth * 0.45f - 5, yPos, _pageWidth * 0.25f, 15), centerFormat);
                    ev.Graphics.DrawString("Toplam", trFontBold, Brushes.Black,
                        new RectangleF(padding + _pageWidth * 0.7f, yPos, _pageWidth * 0.3f - padding, 15), leftFormat);
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

                        ev.Graphics.DrawString(barkodUrunAdi, trFontSemiBold, Brushes.Black,
                            new RectangleF(padding, yPos, urunBilgiWidth, totalBlockHeight), leftFormat);
                        ev.Graphics.DrawString(details, trFont, Brushes.Black,
                            new RectangleF(padding + urunBilgiWidth - 5, yPos, miktarFiyatWidth, totalBlockHeight), centerFormat);
                        ev.Graphics.DrawString(toplamTutar, trFont, Brushes.Black,
                            new RectangleF(padding + urunBilgiWidth + miktarFiyatWidth, yPos, toplamWidth, totalBlockHeight), leftFormat);

                        yPos += totalBlockHeight;
                    }

                    // Çizgi
                    ev.Graphics.DrawLine(Pens.Black, padding, yPos, _pageWidth - padding, yPos);
                    yPos += 10;

                    // Genel toplam
                    ev.Graphics.DrawString($"GENEL TOPLAM: {TryParseDecimal(textBox2.Text):N2} TL", trFontBold, Brushes.Black,
                        new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                    yPos += 20;

                    // Bilgilendirme yazısı
                    ev.Graphics.DrawString("BİLGİ AMAÇLIDIR MALİ DEĞERİ YOKTUR", trFontItalic, Brushes.Black,
                        new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                    yPos += 5;
                }
            };
            pd.Print();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            // baglantiYolu değişkenini metodun en başında bir kez tanımla
            string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
            if (!panel4.Visible)
            {
                MessageBox.Show("Bu işlem için önce 'Kredi Kartı' veya 'Veresiye' ödeme yöntemlerinden birini seçerek paneli açmalısınız.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Metodun devam etmesini engelle
            }
            // Sepette ürün olup olmadığını kontrol et
            if (dataGridView2.Rows.Count == 0 || (dataGridView2.Rows.Count == 1 && dataGridView2.Rows[0].IsNewRow))
            {
                MessageBox.Show("Sepette ürün bulunmamaktadır. Lütfen satış yapmak için ürün ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Stoğu kontrol et
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.IsNewRow) continue;
                decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                decimal kalanStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                if (satilanMiktar > (kalanStok + 0.0001m))
                {
                    DialogResult dr = MessageBox.Show(
                        $"'{row.Cells["Ürün_Adi"].Value}' adlı ürünün satılan miktarı ({satilanMiktar}) mevcut stoktan ({kalanStok}) fazla.\n" +
                        "Eğer devam ederseniz stok 0 olarak ayarlanacaktır. Devam edilsin mi?",
                        "Stok Yetersiz",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                    if (dr != DialogResult.Yes) return;
                }
            }

            // --- Müşteri Borç ve Limit Kontrolü ---
            decimal toplamTutar = TryParseDecimal(textBox2.Text);
            decimal mevcutBorc = 0;
            decimal limit = 0;
            string musteriAdi = "";

            // Müşteri adı doluysa borç ve limit bilgilerini al
            if (!string.IsNullOrWhiteSpace(textBox13.Text))
            {
                using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
                {
                    baglan.Open();
                    string sorgu = "SELECT DevredenBorc, Limit FROM Musteriler WHERE MusteriAdi = @MusteriAdi";
                    using (OleDbCommand cmd = new OleDbCommand(sorgu, baglan))
                    {
                        cmd.Parameters.AddWithValue("@MusteriAdi", textBox13.Text);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Limit verisini okurken decimal.TryParse kullan
                                mevcutBorc = TryParseDecimal(reader["DevredenBorc"]);
                                limit = TryParseDecimal(reader["Limit"]);
                            }
                        }
                    }
                }
                musteriAdi = textBox13.Text;

                decimal yeniToplamBorc = mevcutBorc + toplamTutar;

                // Limit kontrolü
                if (yeniToplamBorc > limit && limit > 0)
                {
                    DialogResult limitAsimUyarisi = MessageBox.Show(
                        $"{musteriAdi} adlı müşterinin {toplamTutar:N2} TL tutarındaki alışverişi ile birlikte\n" +
                        $"Toplam borcu {yeniToplamBorc:N2} TL olacaktır. Bu müşterinize belirlediğiniz\n" +
                        $"Limit {limit:N2} TL'dir. Yine de Bu Satışı Onaylamak İstiyor musunuz?",
                        "Stok Takip Programı",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                    if (limitAsimUyarisi != DialogResult.Yes)
                    {
                        return;
                    }
                }
            }
            // --- Müşteri Borç ve Limit Kontrolü Bitişi ---

            // Satış onayı
            DialogResult result = MessageBox.Show(
                "Satış işlemini tamamlamak istediğinize emin misiniz?",
                "Satış Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result != DialogResult.Yes) return;

            using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
            {
                baglan.Open();
                OleDbTransaction transaction = baglan.BeginTransaction();
                try
                {
                    string satisSorgu = "INSERT INTO UrunSatis (Barkod_No, Urun_Adi, UrunGrubu, KalanStok, OlcuBirimi, Satis_Fiyati, Alis_Fiyati, [2SatisFiyati], AsgariStok, SatilanMiktar, ToplamTutar, Tarih, Saat, SatisTuru) VALUES (@BarkodNo, @UrunAdi, @UrunGrubu, @KalanStok, @OlcuBirimi, @SatisFiyati, @AlisFiyati, @IkinciSatisFiyati, @AsgariStok, @SatilanMiktar, @ToplamTutar, @Tarih, @Saat, @SatisTuru)";
                    string stokGuncellemeSorgu = "UPDATE ÜrünGirişi SET Stok_Miktari = @YeniStokMiktari WHERE Barkod_No = @BarkodNo";

                    // Veresiye satış durumunda müşteri borcunu güncelleme
                    if (!string.IsNullOrWhiteSpace(textBox13.Text))
                    {
                        // Borcu güncellemeden önce, yeni toplam tutarı hesaplayın.
                        decimal mevcutBorcGuncelleme = 0;
                        string borcBulSorgu = "SELECT DevredenBorc FROM Musteriler WHERE MusteriAdi = @MusteriAdi";
                        using (OleDbCommand borcBulCmd = new OleDbCommand(borcBulSorgu, baglan, transaction))
                        {
                            borcBulCmd.Parameters.AddWithValue("@MusteriAdi", textBox13.Text);
                            object borcObj = borcBulCmd.ExecuteScalar();
                            if (borcObj != null && !DBNull.Value.Equals(borcObj))
                            {
                                mevcutBorcGuncelleme = TryParseDecimal(borcObj);
                            }
                        }

                        decimal yeniBorc = mevcutBorcGuncelleme + toplamTutar;

                        // Güncelleme sorgusunda doğrudan hesaplanmış yeni değeri kullanın.
                        string musteriGuncellemeSorgu = "UPDATE Musteriler SET DevredenBorc = @YeniBorc WHERE MusteriAdi = @MusteriAdi";
                        using (OleDbCommand musteriGuncellemeCmd = new OleDbCommand(musteriGuncellemeSorgu, baglan, transaction))
                        {
                            musteriGuncellemeCmd.Parameters.AddWithValue("@YeniBorc", yeniBorc.ToString("F2").Replace(".", ","));
                            musteriGuncellemeCmd.Parameters.AddWithValue("@MusteriAdi", textBox13.Text);
                            musteriGuncellemeCmd.ExecuteNonQuery();
                        }
                    }

                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                        decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                        decimal mevcutStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                        decimal yeniStokMiktari = mevcutStok - satilanMiktar;
                        if (yeniStokMiktari < 0m) yeniStokMiktari = 0m;

                        using (OleDbCommand satisCmd = new OleDbCommand(satisSorgu, baglan, transaction))
                        {
                            satisCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            satisCmd.Parameters.AddWithValue("@UrunAdi", row.Cells["Ürün_Adi"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@UrunGrubu", row.Cells["Ürün_Grubu"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@KalanStok", yeniStokMiktari);
                            satisCmd.Parameters.AddWithValue("@OlcuBirimi", row.Cells["OlcuBirimi"].Value?.ToString() ?? "");
                            satisCmd.Parameters.AddWithValue("@SatisFiyati", TryParseDecimal(row.Cells["Satis_Fiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@AlisFiyati", TryParseDecimal(row.Cells["Alis_Fiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@IkinciSatisFiyati", TryParseDecimal(row.Cells["2SatisFiyati"].Value));
                            satisCmd.Parameters.AddWithValue("@AsgariStok", TryParseDecimal(row.Cells["AsgariStok"].Value));
                            satisCmd.Parameters.AddWithValue("@SatilanMiktar", satilanMiktar);
                            satisCmd.Parameters.AddWithValue("@ToplamTutar", TryParseDecimal(row.Cells["ToplamTutar"].Value));
                            satisCmd.Parameters.AddWithValue("@Tarih", DateTime.Now.ToShortDateString());
                            satisCmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToLongTimeString());
                            satisCmd.Parameters.AddWithValue("@SatisTuru", "Müşteriye Veresiye Satış");
                            satisCmd.ExecuteNonQuery();
                        }

                        using (OleDbCommand guncellemeCmd = new OleDbCommand(stokGuncellemeSorgu, baglan, transaction))
                        {
                            guncellemeCmd.Parameters.AddWithValue("@YeniStokMiktari", yeniStokMiktari);
                            guncellemeCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            guncellemeCmd.ExecuteNonQuery();
                        }
                    }

                    // MusteriSatis tablosuna veri ekleme
                    // Sadece ilgili textbox'lar dolu ise ekleme yapsın
                    if (!string.IsNullOrWhiteSpace(textBox13.Text) || !string.IsNullOrWhiteSpace(textBox17.Text) || !string.IsNullOrWhiteSpace(textBox19.Text))
                    {
                        string musteriSatisSorgu = "INSERT INTO MusteriSatis (MusteriAdi, GsmTelefon, Barkod_No, Urun_Adi, Stok_Miktari, AsgariStok, OlcuBirimi, Satis_Fiyati, SatilanMiktar, ToplamTutar, SatisTuru, Tarih, Saat) VALUES (@MusteriAdi, @GsmTelefon, @BarkodNo, @UrunAdi, @StokMiktari, @AsgariStok, @OlcuBirimi, @SatisFiyati, @SatilanMiktar, @ToplamTutar, @SatisTuru, @Tarih, @Saat)";

                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.IsNewRow) continue;

                            string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                            decimal satilanMiktar = TryParseDecimal(row.Cells["Miktar"].Value);
                            decimal mevcutStok = TryParseDecimal(row.Cells["Stok_Miktari"].Value);
                            decimal yeniStokMiktari = mevcutStok - satilanMiktar;
                            if (yeniStokMiktari < 0m) yeniStokMiktari = 0m;
                            decimal asgariStok = TryParseDecimal(row.Cells["AsgariStok"].Value);

                            using (OleDbCommand musteriSatisCmd = new OleDbCommand(musteriSatisSorgu, baglan, transaction))
                            {
                                musteriSatisCmd.Parameters.AddWithValue("@MusteriAdi", textBox13.Text);
                                musteriSatisCmd.Parameters.AddWithValue("@GsmTelefon", textBox17.Text);
                                musteriSatisCmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                                musteriSatisCmd.Parameters.AddWithValue("@UrunAdi", row.Cells["Ürün_Adi"].Value?.ToString() ?? "");
                                musteriSatisCmd.Parameters.AddWithValue("@StokMiktari", yeniStokMiktari); // Kalan stok değeri eklendi
                                musteriSatisCmd.Parameters.AddWithValue("@AsgariStok", asgariStok);
                                musteriSatisCmd.Parameters.AddWithValue("@OlcuBirimi", row.Cells["OlcuBirimi"].Value?.ToString() ?? "");
                                musteriSatisCmd.Parameters.AddWithValue("@SatisFiyati", TryParseDecimal(row.Cells["Satis_Fiyati"].Value));
                                musteriSatisCmd.Parameters.AddWithValue("@SatilanMiktar", satilanMiktar);
                                musteriSatisCmd.Parameters.AddWithValue("@ToplamTutar", TryParseDecimal(row.Cells["ToplamTutar"].Value).ToString("N2"));
                                musteriSatisCmd.Parameters.AddWithValue("@SatisTuru", "Veresiye Satış");
                                musteriSatisCmd.Parameters.AddWithValue("@Tarih", DateTime.Now.ToShortDateString());
                                musteriSatisCmd.Parameters.AddWithValue("@Saat", DateTime.Now.ToLongTimeString());

                                musteriSatisCmd.ExecuteNonQuery();
                            }
                        }
                        MessageBox.Show("Müşteri satışı başarıyla gerçekleşti!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    transaction.Commit();
                    MessageBox.Show("Satış işlemi başarıyla tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    panel1.Visible = false;
                    textBox13.Text = "";
                    textBox17.Text = "";
                    textBox19.Text = "";

                    // İşletme bilgilerini al
                    string isletmeAdi = "", isletmeAdresi = "", isletmeYeri = "", gsmTelefon = "";
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

                    float pageWidth = 315;
                    float padding = 5;

                    // Kağıt yüksekliği tahmini
                    float paperHeight = 0;
                    paperHeight += 20; // Üst boşluk
                    paperHeight += 15 * 5; // İşletme bilgileri ve aralarındaki boşluklar
                    paperHeight += 10; // Çizgi
                    paperHeight += 25; // Tarih ve saat
                    paperHeight += 20; // Sütun başlıkları
                    paperHeight += 5; // Sütun çizgisi

                    using (var bmp = new Bitmap(1, 1))
                    using (Graphics g = Graphics.FromImage(bmp))
                    using (System.Drawing.Font trFont = new System.Drawing.Font("Arial", 8))
                    using (System.Drawing.Font trFontSemiBold = new System.Drawing.Font("Arial", 8, FontStyle.Regular))
                    {
                        float urunBilgiWidth = pageWidth * 0.55f - padding; // Ürün genişliğini artırarak miktar sütununu sola çek
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

                    // Yazıcı fişi
                    PrintDocument pd = new PrintDocument();
                    pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)pageWidth, (int)Math.Ceiling(paperHeight));
                    pd.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

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

                            // Tarih ve saat (saat daha da sola çekildi)
                            string dateText = $"Tarih: {DateTime.Now.ToShortDateString()}";
                            string timeText = $"Saat: {DateTime.Now.ToLongTimeString()}";
                            float halfWidth = (_pageWidth - 2 * padding) / 2;
                            ev.Graphics.DrawString(dateText, trFont, Brushes.Black,
                                new RectangleF(padding, yPos, halfWidth - 5, 15), leftFormat);
                            ev.Graphics.DrawString(timeText, trFont, Brushes.Black,
                                new RectangleF(padding + halfWidth - 15, yPos, halfWidth, 15), rightFormat);
                            yPos += 25;

                            // Sütun başlıkları
                            ev.Graphics.DrawString("Barkod No / Ürün Adı", trFontBold, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth * 0.45f - padding, 15), leftFormat);
                            ev.Graphics.DrawString("Miktar Fiyatı", trFontBold, Brushes.Black,
                                new RectangleF(padding + _pageWidth * 0.45f - 5, yPos, _pageWidth * 0.25f, 15), centerFormat);
                            ev.Graphics.DrawString("Toplam", trFontBold, Brushes.Black,
                                new RectangleF(padding + _pageWidth * 0.7f, yPos, _pageWidth * 0.3f - padding, 15), leftFormat);
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

                                ev.Graphics.DrawString(barkodUrunAdi, trFontSemiBold, Brushes.Black,
                                    new RectangleF(padding, yPos, urunBilgiWidth, totalBlockHeight), leftFormat);
                                ev.Graphics.DrawString(details, trFont, Brushes.Black,
                                    new RectangleF(padding + urunBilgiWidth - 5, yPos, miktarFiyatWidth, totalBlockHeight), centerFormat);
                                ev.Graphics.DrawString(toplamTutar, trFont, Brushes.Black,
                                    new RectangleF(padding + urunBilgiWidth + miktarFiyatWidth, yPos, toplamWidth, totalBlockHeight), leftFormat);

                                yPos += totalBlockHeight;
                            }

                            // Çizgi
                            ev.Graphics.DrawLine(Pens.Black, padding, yPos, _pageWidth - padding, yPos);
                            yPos += 10;

                            // Genel toplam
                            ev.Graphics.DrawString($"GENEL TOPLAM: {TryParseDecimal(textBox2.Text):N2} TL", trFontBold, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                            yPos += 20;

                            // Bilgilendirme yazısı
                            ev.Graphics.DrawString("BİLGİ AMAÇLIDIR MALİ DEĞERİ YOKTUR", trFontItalic, Brushes.Black,
                                new RectangleF(padding, yPos, _pageWidth - 2 * padding, 15), centerFormat);
                            yPos += 5;
                        }
                    };

                    if (checkBox2.Checked)
                    {
                        pd.Print();
                    }

                    // Asgari stok kontrolü
                    using (OleDbCommand cmd = new OleDbCommand("SELECT Barkod_No, Stok_Miktari, AsgariStok FROM ÜrünGirişi WHERE Barkod_No = @BarkodNo", baglan))
                    {
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.IsNewRow) continue;
                            string barkodNo = row.Cells["Barkod_No"].Value?.ToString() ?? "";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@BarkodNo", barkodNo);
                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    decimal stokMiktari = TryParseDecimal(reader["Stok_Miktari"]);
                                    decimal asgariStok = TryParseDecimal(reader["AsgariStok"]);
                                    if (asgariStok > 0 && stokMiktari <= asgariStok)
                                    {
                                        MessageBox.Show($"'{row.Cells["Ürün_Adi"].Value}' ürününün stoğu asgari seviyenin altına düştü: {stokMiktari} (Asgari: {asgariStok})", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                        }
                    }

                    // Formu sıfırla ve yeniden yükle
                    dataGridView2.Rows.Clear();
                    textBox2.Text = "0.00";
                    textBox3.Text = "0.00";
                    textBox4.Text = "0.00";
                    textBox5.Text = "0.00";
                    textBox6.Text = "1";
                    Listele();
                    Filtrele();
                    UpdateButton2Visibility();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Satış işlemi sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
            if (txt == textBox18 || txt == textBox15 || txt == textBox6)
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

        private void textBox22_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harf, boşluk, eğik çizgi (/) ve kontrol tuşlarına izin ver.
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ' && e.KeyChar != '/')
            {
                e.Handled = true;
            }
        }
        private void textBox22_TextChanged(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            MusteriSecim musteriSecimFormu = Application.OpenForms.OfType<MusteriSecim>().FirstOrDefault();

            // Eğer form zaten açıksa (null değilse), onu ön plana getir.
            if (musteriSecimFormu != null)
            {
                musteriSecimFormu.BringToFront();
            }
            // Eğer form açık değilse, yeni bir tane oluştur ve aç.
            else
            {
                musteriSecimFormu = new MusteriSecim(this);
                musteriSecimFormu.Show();
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
            comboBox2.SelectedIndex = -1;
            textBox18.Clear();
            textBox21.Clear();
            textBox9.Clear();
            textBox20.Clear();
        }

        private void button14_Click(object sender, EventArgs e)
        {// Kullanıcıya bir onay mesajı göster
            DialogResult result = MessageBox.Show("Seçimi iptal etmek istediğinizden emin misiniz?", "Seçim İptali", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Eğer kullanıcı 'Evet' derse işlemleri yap
            if (result == DialogResult.Yes)
            {
                panel4.Visible = false;
                textBox19.Text = "";
                textBox13.Text = "";
                textBox17.Text = "";
                button3.Enabled = false;
                button7.Enabled = false;
            }
            // Eğer 'Hayır' derse hiçbir işlem yapma
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            button3.Enabled = panel4.Visible;
            button7.Enabled = panel4.Visible;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
      
        private void button15_Click(object sender, EventArgs e)
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

        private void button16_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
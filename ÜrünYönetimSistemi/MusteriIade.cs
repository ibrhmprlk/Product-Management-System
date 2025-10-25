using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Office2016.Presentation.Command;

namespace ÜrünYönetimSistemi
{
    public partial class MusteriIade : Form
    {
        private string musteriGsmTelefon;
        private readonly CultureInfo _culture = new CultureInfo("tr-TR");
        private bool isUpdatingTextBox = false;

        public MusteriIade(string gsmTelefon)
        {
            InitializeComponent();
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox4.Text = DateTime.Now.ToShortDateString();
            textBox5.Text = DateTime.Now.ToLongTimeString();
            textBox3.ReadOnly = true;
            textBox1.Text = "0";
            this.musteriGsmTelefon = gsmTelefon;

            // Sadece sayı girişi olsun
            textBox1.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
            dataGridView1.ClearSelection();
           
        }

        private void UrunleriDataGridDoldur()
        {
            string baglantiDizesi = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                    Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
            {
                try
                {
                    baglan.Open();
                    DataTable dt = new DataTable();

                    // Sadece her bir barkoda ait satılan miktarları topluyoruz.
                    // Güncel stok miktarını ise ÜrünGirişi tablosundan alıyoruz.
                    string sorgu = @"
SELECT 
    MS.Barkod_No, 
    MS.Urun_Adi AS [Ürün_Adi], 
    UG.Stok_Miktari AS [Stok_Miktari], 
    UG.AsgariStok, 
    MS.OlcuBirimi, 
    MS.Satis_Fiyati AS [Birim Fiyatı], 
    SUM(MS.SatilanMiktar) AS SatilanMiktar, 
    MS.MusteriAdi, 
    MS.GsmTelefon
FROM MusteriSatis AS MS
LEFT JOIN ÜrünGirişi AS UG ON MS.Barkod_No = UG.Barkod_No
WHERE MS.GsmTelefon = @GsmTelefon 
GROUP BY 
    MS.Barkod_No, 
    MS.Urun_Adi, 
    UG.Stok_Miktari, 
    UG.AsgariStok, 
    MS.OlcuBirimi, 
    MS.Satis_Fiyati, 
    MS.MusteriAdi, 
    MS.GsmTelefon
ORDER BY MS.Urun_Adi ASC;";

                    OleDbDataAdapter da = new OleDbDataAdapter(sorgu, baglan);
                    da.SelectCommand.Parameters.Add("@GsmTelefon", OleDbType.VarWChar).Value = this.musteriGsmTelefon;
                    da.Fill(dt);

                    // İade için ek sütunlar
                    dt.Columns.Add("Miktar", typeof(decimal));
                    dt.Columns.Add("ToplamTutar", typeof(decimal));

                    foreach (DataRow row in dt.Rows)
                    {
                        row["Miktar"] = 0m;
                        row["ToplamTutar"] = 0m;
                    }

                    dataGridView1.DataSource = null;
                    dataGridView1.Columns.Clear();
                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;

                    // Başlık düzenlemeleri
                    if (dataGridView1.Columns.Contains("Barkod_No"))
                        dataGridView1.Columns["Barkod_No"].HeaderText = "Barkod No";

                    if (dataGridView1.Columns.Contains("Ürün_Adi"))
                        dataGridView1.Columns["Ürün_Adi"].HeaderText = "Ürün Adı";

                    if (dataGridView1.Columns.Contains("Stok_Miktari"))
                        dataGridView1.Columns["Stok_Miktari"].HeaderText = "Kalan Stok";

                    if (dataGridView1.Columns.Contains("AsgariStok"))
                        dataGridView1.Columns["AsgariStok"].HeaderText = "Asgari Stok";

                    if (dataGridView1.Columns.Contains("OlcuBirimi"))
                        dataGridView1.Columns["OlcuBirimi"].HeaderText = "Ölçü Birimi";

                    if (dataGridView1.Columns.Contains("SatilanMiktar"))
                        dataGridView1.Columns["SatilanMiktar"].HeaderText = "Satılan Miktar";

                    if (dataGridView1.Columns.Contains("MusteriAdi"))
                        dataGridView1.Columns["MusteriAdi"].HeaderText = "Müşteri Adı";

                    if (dataGridView1.Columns.Contains("GsmTelefon"))
                        dataGridView1.Columns["GsmTelefon"].HeaderText = "GSM No";

                    if (dataGridView1.Columns.Contains("Miktar"))
                        dataGridView1.Columns["Miktar"].HeaderText = "İade Alınacak Miktarı";

                    if (dataGridView1.Columns.Contains("ToplamTutar"))
                    {
                        dataGridView1.Columns["ToplamTutar"].HeaderText = "Toplam Tutar";
                        dataGridView1.Columns["ToplamTutar"].DefaultCellStyle.Format = "C2";
                        dataGridView1.Columns["ToplamTutar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }

                    // Sadece Miktar sütunu düzenlenebilir
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                    {
                        col.ReadOnly = true;
                    }

                    if (dataGridView1.Columns.Contains("Miktar"))
                        dataGridView1.Columns["Miktar"].ReadOnly = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MusteriIade_Load_1(object sender, EventArgs e)
        {
            dataGridView1.MultiSelect = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
            dataGridView1.CellClick += dataGridView1_CellClick;

            textBox1.TextChanged += textBox1_TextChanged;

            UrunleriDataGridDoldur();
            // Form açılışında otomatik seçimi temizle, böylece SelectionChanged tetiklenmesin ve textBox1 boş kalsın
            dataGridView1.ClearSelection();
        }
        private void ToplamTutarHesaplaVeGoster()
        {
            decimal toplam = 0;
            foreach (DataGridViewRow r in dataGridView1.SelectedRows)
            {
                if (r.Cells["Miktar"].Value != null && r.Cells["Birim Fiyatı"].Value != null)
                {
                    string miktarString = r.Cells["Miktar"].Value.ToString();
                    string birimFiyatiString = r.Cells["Birim Fiyatı"].Value.ToString();

                    decimal miktar = 0;
                    decimal birimFiyati = 0;

                    if (decimal.TryParse(miktarString.Replace(",", "."),
                                         System.Globalization.NumberStyles.Any,
                                         System.Globalization.CultureInfo.InvariantCulture,
                                         out miktar) &&
                        decimal.TryParse(birimFiyatiString.Replace(",", "."),
                                         System.Globalization.NumberStyles.Any,
                                         System.Globalization.CultureInfo.InvariantCulture,
                                         out birimFiyati))
                    {
                        toplam += miktar * birimFiyati;
                    }
                }
            }
            textBox3.Text = toplam.ToString("N2", _culture);
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (isUpdatingTextBox) return;

            decimal miktar = 0;
            if (!decimal.TryParse(textBox1.Text.Replace(",", "."),
                                 System.Globalization.NumberStyles.Any,
                                 System.Globalization.CultureInfo.InvariantCulture,
                                 out miktar) || miktar < 0)
            {
                miktar = 0;
            }

            foreach (DataGridViewRow satir in dataGridView1.SelectedRows)
            {
                satir.Cells["Miktar"].Value = miktar;

                decimal birimFiyati = 0;
                if (satir.Cells["Birim Fiyatı"].Value != null && satir.Cells["Birim Fiyatı"].Value != DBNull.Value)
                {
                    if (decimal.TryParse(satir.Cells["Birim Fiyatı"].Value.ToString().Replace(",", "."),
                                         System.Globalization.NumberStyles.Any,
                                         System.Globalization.CultureInfo.InvariantCulture,
                                         out birimFiyati))
                    {
                        satir.Cells["ToplamTutar"].Value = miktar * birimFiyati;
                    }
                }
                else
                {
                    satir.Cells["ToplamTutar"].Value = 0m;
                }
            }
            ToplamTutarHesaplaVeGoster();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            isUpdatingTextBox = true;

            if (dataGridView1.SelectedRows.Count == 1)
            {
                DataGridViewRow satir = dataGridView1.SelectedRows[0];
                textBox1.Text = satir.Cells["Miktar"].Value?.ToString() ?? "0";
            }
            else if (dataGridView1.SelectedRows.Count > 1)
            {
                // Çoklu seçimde textBox1'i en son seçilenin miktarıyla doldur veya temizle (opsiyonel: temizle)
                textBox1.Clear();
            }
            else
            {
              
            }

            isUpdatingTextBox = false;
            ToplamTutarHesaplaVeGoster();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridView1.Rows[e.RowIndex];
                bool isCurrentlySelected = row.Selected;

                if (isCurrentlySelected)
                {
                    row.Selected = false;
                    row.Cells["Miktar"].Value = 0m;
                    row.Cells["ToplamTutar"].Value = 0m;
                }
                else
                {
                    row.Selected = true;
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Yalnızca "Miktar" sütunundaki değişiklikleri dinle
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Miktar")
            {
                try
                {
                    decimal miktar = 0;
                    decimal birimFiyati = 0;

                    // Miktar ve Birim Fiyatı değerlerini almayı ve ondalık virgülünü noktaya çevirmeyi dene.
                    // Değerler null ise veya dönüştürülemiyorsa, TryParse false döndürecek.
                    if (decimal.TryParse(dataGridView1.Rows[e.RowIndex].Cells["Miktar"].Value?.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out miktar) &&
                        decimal.TryParse(dataGridView1.Rows[e.RowIndex].Cells["Birim Fiyatı"].Value?.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out birimFiyati))
                    {
                        // Küsüratları koruyarak tam hesaplama, yuvarlama olmadan
                        decimal toplamTutar = miktar * birimFiyati;

                        // Hesaplanan değeri hücreye at
                        dataGridView1.Rows[e.RowIndex].Cells["ToplamTutar"].Value = toplamTutar;
                    }
                    else
                    {
                        // Değerler geçersizse kullanıcıyı uyar
                        MessageBox.Show("Miktar veya Birim Fiyatı bilgisi geçersiz. Lütfen geçerli bir sayısal değer girin.", "Geçersiz Giriş", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        // Hesaplanan değeri hücreden kaldır
                        dataGridView1.Rows[e.RowIndex].Cells["ToplamTutar"].Value = DBNull.Value;
                    }

                    // Toplam tutarı hesaplayan ve gösteren metodu çağır
                    ToplamTutarHesaplaVeGoster();
                }
                catch (Exception ex)
                {
                    // Beklenmeyen bir hata oluşursa kullanıcıya bildir
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox4.Text = DateTime.Now.ToShortDateString();
            textBox5.Text = DateTime.Now.ToLongTimeString();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string aramaMetni = textBox2.Text.Trim();

            // DataGridView'in veri kaynağı olan DataTable'ı al
            DataTable dt = dataGridView1.DataSource as DataTable;

            if (dt == null) return;

            DataView dv = dt.DefaultView;

            // Arama metni boşsa filtreyi temizle, doluysa filtrele
            if (string.IsNullOrEmpty(aramaMetni))
            {
                dv.RowFilter = string.Empty;
            }
            else
            {
                // Barkod veya Ürün Adı üzerinden filtreleme yap
                dv.RowFilter = $"CONVERT(Barkod_No, 'System.String') LIKE '%{aramaMetni}%' " +
                               $"OR Urun_Adi LIKE '%{aramaMetni}%'";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 1. Açık Müşteriler formunu bul.
            Müşteriler musteriForm = Application.OpenForms.OfType<Müşteriler>().FirstOrDefault();

            if (musteriForm == null)
            {
                // Form açık değilse, yeni bir tane oluştur ve göster.
                musteriForm = new Müşteriler();
                musteriForm.Show();
            }
            else
            {
                // Form açıksa, ön plana getir. (Bu aynı zamanda gizliyse gösterir).
                musteriForm.Show();      // Form gizliyse görünür hale getirir.
                musteriForm.BringToFront(); // Formu diğer pencerelerin önüne getirir.
            }

            // 2. Ürün_Girişi formlarını kapat. (Mevcut mantık)
            foreach (Form openForm in Application.OpenForms.OfType<Ürün_Girişi>().ToList())
            {
                openForm.Close();
            }

            // 3. Mevcut formu kapat.
            this.Close();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen iade alınacak ürünü seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string baglantiDizesi = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
            {
                try
                {
                    baglan.Open();
                    int basariliIslemSayisi = 0;

                    foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
                    {
                        if (selectedRow.Cells["Miktar"].Value == null ||
                            !decimal.TryParse(selectedRow.Cells["Miktar"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal iadeMiktari) ||
                            iadeMiktari <= 0)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için geçerli bir iade miktarı girilmedi. Bu ürün atlandı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        if (selectedRow.Cells["Stok_Miktari"].Value == null ||
                            !decimal.TryParse(selectedRow.Cells["Stok_Miktari"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal mevcutStok))
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için mevcut stok değeri okunamadı. Bu ürün atlandı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        decimal yeniStok = mevcutStok + iadeMiktari;
                        if (yeniStok < 0) yeniStok = 0;

                        // 1. ADIM: ÜrünGirişi tablosundaki stoku güncelle
                        string stokGuncellemeSorgusu = "UPDATE ÜrünGirişi SET Stok_Miktari = CStr(VAL(Stok_Miktari) + ?) WHERE Barkod_No = ?";
                        using (OleDbCommand komut = new OleDbCommand(stokGuncellemeSorgusu, baglan))
                        {
                            komut.Parameters.Add("?", OleDbType.Double).Value = iadeMiktari;
                            komut.Parameters.Add("?", OleDbType.VarWChar).Value = selectedRow.Cells["Barkod_No"].Value.ToString();
                            komut.ExecuteNonQuery();
                        }

                        // 2. ADIM: MusteriIade tablosuna yeni iade kaydı ekle
                        string insertQuery = "INSERT INTO [MusteriIade] (MusteriAdi, GsmTelefon, Barkod_No, Ürün_Adi, Aciklama, Stok_Miktari, IadeAlinanMiktar, ToplamTutar, Tarih, Saat) VALUES (@MusteriAdi, @GsmTelefon, @BarkodNo, @Ürün_Adi, @Aciklama, @StokMiktari, @IadeAlinanMiktar, @ToplamTutar, @Tarih, @Saat)";
                        using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, baglan))
                        {
                            decimal birimFiyati = 0;
                            if (selectedRow.Cells["Birim Fiyatı"].Value != null &&
                                !decimal.TryParse(selectedRow.Cells["Birim Fiyatı"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out birimFiyati))
                            {
                                MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için birim fiyatı okunamadı. Toplam tutar sıfır olarak kaydedilecek.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            decimal toplamTutar = iadeMiktari * birimFiyati;

                            insertCmd.Parameters.AddWithValue("@MusteriAdi", selectedRow.Cells["MusteriAdi"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@GsmTelefon", selectedRow.Cells["GsmTelefon"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@BarkodNo", selectedRow.Cells["Barkod_No"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@Ürün_Adi", selectedRow.Cells["Ürün_Adi"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@Aciklama", "Müşteri İadesi - İade Türü Belirtmek İstemiyorum");
                            insertCmd.Parameters.AddWithValue("@StokMiktari", yeniStok);
                            insertCmd.Parameters.AddWithValue("@IadeAlinanMiktar", iadeMiktari);
                            insertCmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar);
                            insertCmd.Parameters.AddWithValue("@Tarih", textBox4.Text);
                            insertCmd.Parameters.AddWithValue("@Saat", textBox5.Text);
                            insertCmd.ExecuteNonQuery();
                        }
                        basariliIslemSayisi++;

                        if (selectedRow.Cells["AsgariStok"].Value != null &&
                            decimal.TryParse(selectedRow.Cells["AsgariStok"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal asgariStok) &&
                            yeniStok < asgariStok)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' stok miktarı ({yeniStok}) asgari stok seviyesinin ({asgariStok}) altına düştü!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        // 3. ADIM: Satış formundaki DataGridView2'den satırı kaldır
                        Form satisFormu = Application.OpenForms["SatisFiyatlariForm"];
                        if (satisFormu != null)
                        {
                            DataGridView dataGridView2 = satisFormu.Controls["dataGridView2"] as DataGridView;
                            if (dataGridView2 != null)
                            {
                                string barkodNo = selectedRow.Cells["Barkod_No"].Value.ToString();
                                foreach (DataGridViewRow row in dataGridView2.Rows)
                                {
                                    if (!row.IsNewRow && row.Cells["Barkod_No"].Value?.ToString() == barkodNo)
                                    {
                                        dataGridView2.Rows.Remove(row);
                                        break;
                                    }
                                }
                                satisFormu.GetType().GetMethod("UpdateButton2Visibility")?.Invoke(satisFormu, null);
                                satisFormu.GetType().GetMethod("HesaplaParaUstuVeKar")?.Invoke(satisFormu, null);
                            }
                        }
                    }

                    if (basariliIslemSayisi > 0)
                    {
                        MessageBox.Show($"{basariliIslemSayisi} adet ürün için iade işlemi başarıyla tamamlandı. Stoklar güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UrunleriDataGridDoldur();
                     
                        dataGridView1.ClearSelection(); // İşlem sonrası seçimi temizle
                        textBox1.Text = "0";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen iade alınacak ürünü seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string baglantiDizesi = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
            {
                try
                {
                    baglan.Open();
                    int basariliIslemSayisi = 0;

                    foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
                    {
                        if (selectedRow.Cells["Miktar"].Value == null ||
                            !decimal.TryParse(selectedRow.Cells["Miktar"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal iadeMiktari) ||
                            iadeMiktari <= 0)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için geçerli bir iade miktarı girilmedi. Bu ürün atlandı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        if (selectedRow.Cells["Stok_Miktari"].Value == null ||
                            !decimal.TryParse(selectedRow.Cells["Stok_Miktari"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal mevcutStok))
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için mevcut stok değeri okunamadı. Bu ürün atlandı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        decimal yeniStok = mevcutStok + iadeMiktari;
                        if (yeniStok < 0) yeniStok = 0;

                        // 1. ADIM: ÜrünGirişi tablosundaki stoku güncelle
                        string stokGuncellemeSorgusu = "UPDATE ÜrünGirişi SET Stok_Miktari = CStr(VAL(Stok_Miktari) + ?) WHERE Barkod_No = ?";
                        using (OleDbCommand komut = new OleDbCommand(stokGuncellemeSorgusu, baglan))
                        {
                            komut.Parameters.Add("?", OleDbType.Double).Value = iadeMiktari;
                            komut.Parameters.Add("?", OleDbType.VarWChar).Value = selectedRow.Cells["Barkod_No"].Value.ToString();
                            komut.ExecuteNonQuery();
                        }

                        // 2. ADIM: MusteriIade tablosuna yeni iade kaydı ekle
                        string insertQuery = "INSERT INTO [MusteriIade] (MusteriAdi, GsmTelefon, Barkod_No, Ürün_Adi, Aciklama, Stok_Miktari, IadeAlinanMiktar, ToplamTutar, Tarih, Saat) VALUES (@MusteriAdi, @GsmTelefon, @BarkodNo, @Ürün_Adi, @Aciklama, @StokMiktari, @IadeAlinanMiktar, @ToplamTutar, @Tarih, @Saat)";
                        using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, baglan))
                        {
                            decimal birimFiyati = 0;
                            if (selectedRow.Cells["Birim Fiyatı"].Value != null &&
                                !decimal.TryParse(selectedRow.Cells["Birim Fiyatı"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out birimFiyati))
                            {
                                MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için birim fiyatı okunamadı. Toplam tutar sıfır olarak kaydedilecek.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            decimal toplamTutar = iadeMiktari * birimFiyati;

                            insertCmd.Parameters.AddWithValue("@MusteriAdi", selectedRow.Cells["MusteriAdi"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@GsmTelefon", selectedRow.Cells["GsmTelefon"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@BarkodNo", selectedRow.Cells["Barkod_No"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@Ürün_Adi", selectedRow.Cells["Ürün_Adi"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@Aciklama", "Müşteri İadesi - Müşteri Nakit Ödedi");
                            insertCmd.Parameters.AddWithValue("@StokMiktari", yeniStok);
                            insertCmd.Parameters.AddWithValue("@IadeAlinanMiktar", iadeMiktari);
                            insertCmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar);
                            insertCmd.Parameters.AddWithValue("@Tarih", textBox4.Text);
                            insertCmd.Parameters.AddWithValue("@Saat", textBox5.Text);
                            insertCmd.ExecuteNonQuery();
                        }
                        basariliIslemSayisi++;

                        if (selectedRow.Cells["AsgariStok"].Value != null &&
                            decimal.TryParse(selectedRow.Cells["AsgariStok"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal asgariStok) &&
                            yeniStok < asgariStok)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' stok miktarı ({yeniStok}) asgari stok seviyesinin ({asgariStok}) altına düştü!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        // 3. ADIM: Satış formundaki DataGridView2'den satırı kaldır
                        Form satisFormu = Application.OpenForms["SatisFiyatlariForm"];
                        if (satisFormu != null)
                        {
                            DataGridView dataGridView2 = satisFormu.Controls["dataGridView2"] as DataGridView;
                            if (dataGridView2 != null)
                            {
                                string barkodNo = selectedRow.Cells["Barkod_No"].Value.ToString();
                                foreach (DataGridViewRow row in dataGridView2.Rows)
                                {
                                    if (!row.IsNewRow && row.Cells["Barkod_No"].Value?.ToString() == barkodNo)
                                    {
                                        dataGridView2.Rows.Remove(row);
                                        break;
                                    }
                                }
                                satisFormu.GetType().GetMethod("UpdateButton2Visibility")?.Invoke(satisFormu, null);
                                satisFormu.GetType().GetMethod("HesaplaParaUstuVeKar")?.Invoke(satisFormu, null);
                            }
                        }
                    }

                    if (basariliIslemSayisi > 0)
                    {
                        MessageBox.Show($"{basariliIslemSayisi} adet ürün için iade işlemi başarıyla tamamlandı. Stoklar güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UrunleriDataGridDoldur();
                     
                        dataGridView1.ClearSelection(); // İşlem sonrası seçimi temizle
                        textBox1.Text = "0";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 1. Ön kontroller
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen iade alınacak ürünü seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(this.musteriGsmTelefon))
            {
                MessageBox.Show("Müşteri GSM bilgisi eksik.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string baglantiDizesi = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
            using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
            {
                try
                {
                    baglan.Open();
                    int basariliIslemSayisi = 0;
                    decimal toplamIadeTutari = 0;
                    string musteriAdi = string.Empty;
                    decimal mevcutBorc = 0;
                    // 2. ADIM: Müşteri adı ve mevcut borcu "Musteriler" tablosundan çek
                    string musteriSorgu = "SELECT MusteriAdi, DevredenBorc FROM Musteriler WHERE GsmTelefon = ?";
                    using (OleDbCommand musteriKmt = new OleDbCommand(musteriSorgu, baglan))
                    {
                        musteriKmt.Parameters.Add("?", OleDbType.VarWChar, 255).Value = this.musteriGsmTelefon;
                        using (OleDbDataReader dr = musteriKmt.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                musteriAdi = dr["MusteriAdi"].ToString();
                                if (dr["DevredenBorc"] != DBNull.Value)
                                {
                                    decimal.TryParse(dr["DevredenBorc"].ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out mevcutBorc);
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Müşteri bilgisi bulunamadı: {this.musteriGsmTelefon}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                    // 3. ADIM: Seçilen ürünler için döngü
                    foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
                    {
                        if (selectedRow.Cells["Miktar"].Value == null || !decimal.TryParse(selectedRow.Cells["Miktar"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal iadeMiktari) || iadeMiktari <= 0)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için geçerli bir iade miktarı girilmedi. Bu ürün atlandı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }
                        if (!decimal.TryParse(selectedRow.Cells["Birim Fiyatı"].Value?.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal birimFiyati))
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için birim fiyatı geçersiz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }
                        if (!decimal.TryParse(selectedRow.Cells["Stok_Miktari"].Value?.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal mevcutStok))
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için stok bilgisi geçersiz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }
                        if (selectedRow.Cells["AsgariStok"].Value == null || !decimal.TryParse(selectedRow.Cells["AsgariStok"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal asgariStok))
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için asgari stok bilgisi geçersiz. Bu ürün atlandı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }
                        decimal yeniStok = mevcutStok + iadeMiktari;
                        // Stok güncelleme
                        string stokGuncellemeSorgusu = "UPDATE ÜrünGirişi SET Stok_Miktari = ? WHERE Barkod_No = ?";
                        using (OleDbCommand stokKomut = new OleDbCommand(stokGuncellemeSorgusu, baglan))
                        {
                            stokKomut.Parameters.Add("?", OleDbType.Currency).Value = yeniStok;
                            stokKomut.Parameters.Add("?", OleDbType.VarWChar, 255).Value = selectedRow.Cells["Barkod_No"].Value.ToString();
                            stokKomut.ExecuteNonQuery();
                        }
                        // Müşteri iade kaydını ekle
                        decimal iadeTutari = iadeMiktari * birimFiyati;
                        string insertIadeSorgu = "INSERT INTO [MusteriIade] (MusteriAdi, GsmTelefon, Barkod_No, Ürün_Adi, Aciklama, Stok_Miktari, AsgariStok, IadeAlinanMiktar, ToplamTutar, Tarih, Saat) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
                        using (OleDbCommand insertIadeCmd = new OleDbCommand(insertIadeSorgu, baglan))
                        {
                            insertIadeCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = musteriAdi;
                            insertIadeCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = this.musteriGsmTelefon;
                            insertIadeCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = selectedRow.Cells["Barkod_No"].Value.ToString();
                            insertIadeCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = selectedRow.Cells["Ürün_Adi"].Value.ToString();
                            insertIadeCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = "Ürün iadesi - Borçtan düşüldü";
                            insertIadeCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = yeniStok.ToString(CultureInfo.InvariantCulture);
                            insertIadeCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = asgariStok.ToString(CultureInfo.InvariantCulture);
                            insertIadeCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = iadeMiktari.ToString(CultureInfo.InvariantCulture);
                            insertIadeCmd.Parameters.Add("?", OleDbType.Currency).Value = iadeTutari;
                            insertIadeCmd.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now.ToShortDateString(); // Sadece tarih
                            insertIadeCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = DateTime.Now.ToLongTimeString(); // Sadece saat
                            insertIadeCmd.ExecuteNonQuery();
                        }
                        toplamIadeTutari += iadeTutari;
                        basariliIslemSayisi++;
                    }
                    // 4. ADIM: Müşteri borcunu güncelleme ve tahsilat kayıtları
                    if (basariliIslemSayisi > 0)
                    {
                        decimal yeniBorc = mevcutBorc - toplamIadeTutari;
                        // Borç tutarı negatif olamaz, 0'ın altına düşürülmez.
                        if (yeniBorc < 0)
                        {
                            yeniBorc = 0;
                            MessageBox.Show("İade tutarı, mevcut borcu aştığı için borç sıfırlandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        // Musteriler tablosunda DevredenBorcu güncelle
                        string musteriBorcGuncelleSorgu = "UPDATE Musteriler SET DevredenBorc = ? WHERE GsmTelefon = ?";
                        using (OleDbCommand borcGuncelleKmt = new OleDbCommand(musteriBorcGuncelleSorgu, baglan))
                        {
                            borcGuncelleKmt.Parameters.Add("?", OleDbType.Currency).Value = yeniBorc;
                            borcGuncelleKmt.Parameters.Add("?", OleDbType.VarWChar, 255).Value = this.musteriGsmTelefon;
                            borcGuncelleKmt.ExecuteNonQuery();
                        }
                        // Tahsilat tablosuna borç düşme kaydı ekle
                        string tahsilatSorgu = "INSERT INTO Tahsilat (MusteriAdi, GsmTelefon, EskiBorc, [Tarih/Saat], OdenenTutar, ToplamKalanBorc, Aciklama, OdemeSekli) VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
                        using (OleDbCommand tahsilatCmd = new OleDbCommand(tahsilatSorgu, baglan))
                        {
                            tahsilatCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = musteriAdi;
                            tahsilatCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = this.musteriGsmTelefon;
                            tahsilatCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = mevcutBorc.ToString("F2", CultureInfo.InvariantCulture);
                            tahsilatCmd.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;
                            // Ödenen tutar her zaman toplam iade tutarıdır.
                            tahsilatCmd.Parameters.Add("?", OleDbType.Currency).Value = toplamIadeTutari;
                            tahsilatCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = yeniBorc.ToString("F2", CultureInfo.InvariantCulture);
                            tahsilatCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = "Ürün iadesi";
                            tahsilatCmd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = "İade - Borçtan Düşüldü";
                            tahsilatCmd.ExecuteNonQuery();
                        }
                        MessageBox.Show($"İade işlemi başarıyla tamamlandı. Stok ve müşteri borcu güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UrunleriDataGridDoldur();
                      
                        dataGridView1.ClearSelection(); // İşlem sonrası seçimi temizle
                        textBox1.Text = "0";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
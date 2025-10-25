using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Globalization;
using DocumentFormat.OpenXml.Office2016.Presentation.Command;

namespace ÜrünYönetimSistemi
{
    public partial class ToptancıİadeEdilecekÜrünler : Form
    {
        private string toptanciGsmTelefon;
        private bool isUpdatingTextBox = false; // Sonsuz döngüyü önlemek için
        private readonly CultureInfo _culture = new CultureInfo("tr-TR");

        public ToptancıİadeEdilecekÜrünler(string gsmTelefon)
        {
            InitializeComponent();
            textBox1.Text = "0";
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox4.Text = DateTime.Now.ToShortDateString();
            textBox5.Text = DateTime.Now.ToLongTimeString();
            textBox3.ReadOnly = true;
            this.toptanciGsmTelefon = gsmTelefon; textBox1.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            dataGridView1.ClearSelection();
            textBox1.Text = "0";
        }

        private void ToptancıİadeEdilecekÜrünler_Load(object sender, EventArgs e)
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

            // Form açılışında otomatik seçimi temizle, böylece SelectionChanged tetiklenmesin
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

                    // KDV dahil/hariç seçimine göre fiyat sütununu seç
                    string fiyatSutunu = GlobalAyarlar.KdvDahilGoster ? "Alis_Fiyati" : "Alis_Fiyati2";
                    string sorgu = $@"
SELECT Barkod_No, Ürün_Adi, Stok_Miktari, OlcuBirimi, AsgariStok, {fiyatSutunu} AS BirimFiyati, KDV_Orani, Toptanci_Adi, GsmTelefon
FROM ÜrünGirişi
WHERE GsmTelefon = @GsmTelefon AND IsNumeric(Stok_Miktari) = True AND (CDbl(Stok_Miktari) > 0 OR Stok_Miktari IS NOT Null)
ORDER BY Ürün_Adi ASC";

                    OleDbDataAdapter da = new OleDbDataAdapter(sorgu, baglan);
                    da.SelectCommand.Parameters.Add("@GsmTelefon", OleDbType.VarWChar).Value = this.toptanciGsmTelefon;
                    da.Fill(dt);

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

                    // Sütun başlıklarını ayarla
                    if (dataGridView1.Columns.Contains("Barkod_No")) dataGridView1.Columns["Barkod_No"].HeaderText = "Barkod No";
                    if (dataGridView1.Columns.Contains("Ürün_Adi")) dataGridView1.Columns["Ürün_Adi"].HeaderText = "Ürün Adı";
                    if (dataGridView1.Columns.Contains("Stok_Miktari")) dataGridView1.Columns["Stok_Miktari"].HeaderText = "Kalan Stok";
                    if (dataGridView1.Columns.Contains("OlcuBirimi")) dataGridView1.Columns["OlcuBirimi"].HeaderText = "Ölçü Birimi";
                    if (dataGridView1.Columns.Contains("AsgariStok")) dataGridView1.Columns["AsgariStok"].HeaderText = "Asgari Stok";
                    if (dataGridView1.Columns.Contains("BirimFiyati")) dataGridView1.Columns["BirimFiyati"].HeaderText = GlobalAyarlar.KdvDahilGoster ? "Alış Fiyatı (KDV Dahil)" : "Alış Fiyatı (KDV Hariç)";
                    if (dataGridView1.Columns.Contains("KDV_Orani")) dataGridView1.Columns["KDV_Orani"].HeaderText = "KDV (%)";
                    if (dataGridView1.Columns.Contains("Toptanci_Adi")) dataGridView1.Columns["Toptanci_Adi"].HeaderText = "Toptancı Adı";
                    if (dataGridView1.Columns.Contains("GsmTelefon")) dataGridView1.Columns["GsmTelefon"].HeaderText = "GSM No";
                    if (dataGridView1.Columns.Contains("Miktar")) dataGridView1.Columns["Miktar"].HeaderText = "İade Miktarı";
                    if (dataGridView1.Columns.Contains("ToplamTutar")) dataGridView1.Columns["ToplamTutar"].HeaderText = "Toplam Tutar";

                    // Tüm sütunları read-only yap
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                    {
                        col.ReadOnly = true;
                    }

                    // Miktar sütununu düzenlenebilir yap
                    if (dataGridView1.Columns.Contains("Miktar"))
                        dataGridView1.Columns["Miktar"].ReadOnly = false;

                    // Formatlama
                    if (dataGridView1.Columns.Contains("BirimFiyati"))
                    {
                        dataGridView1.Columns["BirimFiyati"].DefaultCellStyle.Format = "C2";
                        dataGridView1.Columns["BirimFiyati"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    if (dataGridView1.Columns.Contains("ToplamTutar"))
                    {
                        dataGridView1.Columns["ToplamTutar"].DefaultCellStyle.Format = "C2";
                        dataGridView1.Columns["ToplamTutar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (isUpdatingTextBox) return;

            decimal miktar = 0;
            // Kullanıcının girdiği miktarı, virgülü noktaya çevirerek ayrıştır.
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
                if (satir.Cells["BirimFiyati"].Value != null && satir.Cells["BirimFiyati"].Value != DBNull.Value)
                {
                    // Birim fiyatını alırken, değeri string'e çevirip virgülü noktaya çevirerek ayrıştır.
                    if (decimal.TryParse(satir.Cells["BirimFiyati"].Value.ToString().Replace(",", "."),
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
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Miktar")
            {
                decimal miktar = 0;
                decimal birimFiyati = 0;

                if (decimal.TryParse(dataGridView1.Rows[e.RowIndex].Cells["Miktar"].Value?.ToString().Replace(",", "."), NumberStyles.Any, _culture, out miktar) &&
                    decimal.TryParse(dataGridView1.Rows[e.RowIndex].Cells["BirimFiyati"].Value?.ToString().Replace(",", "."), NumberStyles.Any, _culture, out birimFiyati))
                {
                    // Küsüratları koruyarak tam hesaplama, yuvarlama olmadan
                    decimal toplamTutar = miktar * birimFiyati;
                    dataGridView1.Rows[e.RowIndex].Cells["ToplamTutar"].Value = toplamTutar; // Tam değer
                }
                ToplamTutarHesaplaVeGoster();
            }
        }

        private void ToplamTutarHesaplaVeGoster()
        {
            decimal toplam = 0;
            foreach (DataGridViewRow r in dataGridView1.SelectedRows)
            {
                if (r.Cells["Miktar"].Value != null && r.Cells["BirimFiyati"].Value != null)
                {
                    string miktarString = r.Cells["Miktar"].Value.ToString();
                    string birimFiyatiString = r.Cells["BirimFiyati"].Value.ToString();

                    decimal miktar = 0;
                    decimal birimFiyati = 0;

                    // Miktar ve birim fiyatı değerlerini kültüre bakılmaksızın doğru ayrıştır.
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
            // Toplam tutarı, N2 formatında ve Türkçe kültüre göre göster.
            textBox3.Text = toplam.ToString("N2", _culture);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen iade edilecek ürünü seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        decimal yeniStok = 0;
                        decimal iadeMiktari;
                        decimal birimFiyati;

                        // Miktarı güvenli bir şekilde alıyoruz
                        if (selectedRow.Cells["Miktar"].Value == null ||
                            !decimal.TryParse(selectedRow.Cells["Miktar"].Value.ToString(), NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out iadeMiktari) ||
                            iadeMiktari <= 0)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' adlı ürün için geçerli bir iade miktarı girilmedi. Bu ürün atlandı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        // Birim fiyatı alıyoruz
                        birimFiyati = decimal.Parse(selectedRow.Cells["BirimFiyati"].Value.ToString(), NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);

                        decimal mevcutStok = Convert.ToDecimal(selectedRow.Cells["Stok_Miktari"].Value);
                        if (iadeMiktari > mevcutStok)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' adlı ürün için stok yetersiz. Maksimum iade miktarı: {mevcutStok}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        string stokGuncellemeSorgusu = "UPDATE ÜrünGirişi SET Stok_Miktari = Stok_Miktari - @iadeMiktar WHERE Barkod_No = @barkod";
                        using (OleDbCommand komut = new OleDbCommand(stokGuncellemeSorgusu, baglan))
                        {
                            komut.Parameters.AddWithValue("@iadeMiktar", iadeMiktari);
                            komut.Parameters.AddWithValue("@barkod", selectedRow.Cells["Barkod_No"].Value.ToString());
                            komut.ExecuteNonQuery();
                        }

                        // Stok güncellemesinden sonraki yeni stok değerini hesapla
                        yeniStok = mevcutStok - iadeMiktari;
                        if (yeniStok < 0) yeniStok = 0;

                        // Toptancı Nakit Ödedi bilgisiyle UrunIade tablosuna kayıt ekle
                        string insertQuery = "INSERT INTO [UrunIade] (ToptanciAdi, GsmTelefon, Barkod_No, Ürün_Adi, Aciklama, Stok_Miktari, IadeEdilenMiktar, ToplamTutar, Tarih, Saat) VALUES (@ToptanciAdi, @GsmTelefon, @BarkodNo, @UrunAdi, @Aciklama, @StokMiktari, @IadeEdilenMiktar, @ToplamTutar, @Tarih, @Saat)";
                        using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, baglan))
                        {
                            insertCmd.Parameters.AddWithValue("@ToptanciAdi", selectedRow.Cells["Toptanci_Adi"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@GsmTelefon", selectedRow.Cells["GsmTelefon"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@BarkodNo", selectedRow.Cells["Barkod_No"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@UrunAdi", selectedRow.Cells["Ürün_Adi"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@Aciklama", "Ürün İade - Toptancı Nakit Ödedi");
                            insertCmd.Parameters.AddWithValue("@StokMiktari", yeniStok);
                            insertCmd.Parameters.AddWithValue("@IadeEdilenMiktar", iadeMiktari);
                            insertCmd.Parameters.AddWithValue("@ToplamTutar", (iadeMiktari * birimFiyati).ToString(System.Globalization.CultureInfo.InvariantCulture));
                            insertCmd.Parameters.AddWithValue("@Tarih", textBox4.Text);
                            insertCmd.Parameters.AddWithValue("@Saat", textBox5.Text);
                            insertCmd.ExecuteNonQuery();
                        }

                        basariliIslemSayisi++;

                        // Güncellenmiş stok değerini kullanarak asgari stok kontrolü yap
                        if (selectedRow.Cells["AsgariStok"].Value != null &&
                            decimal.TryParse(selectedRow.Cells["AsgariStok"].Value.ToString().Replace(",", "."), NumberStyles.Any, _culture, out decimal asgariStok) &&
                            yeniStok < asgariStok)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' stok miktarı ({yeniStok}) asgari stok seviyesinin ({asgariStok}) altına düştü!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    if (basariliIslemSayisi > 0)
                    {
                        MessageBox.Show($"{basariliIslemSayisi} adet ürün için iade işlemi başarıyla tamamlandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UrunleriDataGridDoldur();
                        textBox1.Text = "0";
                        dataGridView1.ClearSelection(); // İşlem sonrası seçimi temizle
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("İşlem sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen iade edilecek ürünü seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        decimal yeniStok = 0;
                        decimal iadeMiktari;
                        decimal birimFiyati;

                        // Miktar ve Birim Fiyatı değerlerini evrensel formatla alıyoruz.
                        if (selectedRow.Cells["Miktar"].Value == null ||
                            !decimal.TryParse(selectedRow.Cells["Miktar"].Value.ToString(), NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out iadeMiktari) ||
                            iadeMiktari <= 0)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için geçerli bir iade miktarı girilmedi. Bu ürün atlandı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        birimFiyati = decimal.Parse(selectedRow.Cells["BirimFiyati"].Value.ToString(), NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);

                        decimal mevcutStok = Convert.ToDecimal(selectedRow.Cells["Stok_Miktari"].Value);
                        if (iadeMiktari > mevcutStok)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için stok yetersiz. Maksimum iade miktarı: {mevcutStok}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        string stokGuncellemeSorgusu = "UPDATE ÜrünGirişi SET Stok_Miktari = Stok_Miktari - @iadeMiktar WHERE Barkod_No = @barkod";
                        using (OleDbCommand komut = new OleDbCommand(stokGuncellemeSorgusu, baglan))
                        {
                            komut.Parameters.AddWithValue("@iadeMiktar", iadeMiktari);
                            komut.Parameters.AddWithValue("@barkod", selectedRow.Cells["Barkod_No"].Value.ToString());
                            komut.ExecuteNonQuery();
                        }

                        yeniStok = mevcutStok - iadeMiktari;
                        if (yeniStok < 0) yeniStok = 0;

                        string insertQuery = "INSERT INTO [UrunIade] (ToptanciAdi, GsmTelefon, Barkod_No, Ürün_Adi, Aciklama, Stok_Miktari,IadeEdilenMiktar, ToplamTutar, Tarih, Saat) VALUES (@ToptanciAdi, @GsmTelefon, @BarkodNo, @UrunAdi, @Aciklama, @StokMiktari," +
                            " @IadeEdilenMiktar, @ToplamTutar, @Tarih, @Saat)";
                        using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, baglan))
                        {
                            insertCmd.Parameters.AddWithValue("@ToptanciAdi", selectedRow.Cells["Toptanci_Adi"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@GsmTelefon", selectedRow.Cells["GsmTelefon"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@BarkodNo", selectedRow.Cells["Barkod_No"].Value.ToString());
                            insertCmd.Parameters.AddWithValue("@UrunAdi", selectedRow.Cells["Ürün_Adi"].Value.ToString());

                            insertCmd.Parameters.AddWithValue("@Aciklama", "Ürün İade - İade Türünü Belirtmek İstemiyorum ");
                            insertCmd.Parameters.AddWithValue("@StokMiktari", yeniStok);
                            insertCmd.Parameters.AddWithValue("@IadeEdilenMiktar", iadeMiktari);

                            // Toplam tutar, küsüratlı haliyle metin olarak ekleniyor.
                            insertCmd.Parameters.AddWithValue("@ToplamTutar", (iadeMiktari * birimFiyati).ToString(System.Globalization.CultureInfo.InvariantCulture));

                            insertCmd.Parameters.AddWithValue("@Tarih", textBox4.Text);
                            insertCmd.Parameters.AddWithValue("@Saat", textBox5.Text);
                            insertCmd.ExecuteNonQuery();
                        }
                        basariliIslemSayisi++;

                        if (selectedRow.Cells["AsgariStok"].Value != null &&
                            decimal.TryParse(selectedRow.Cells["AsgariStok"].Value.ToString().Replace(",", "."), NumberStyles.Any, _culture, out decimal asgariStok) &&
                            yeniStok < asgariStok)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' stok miktarı ({yeniStok}) asgari stok seviyesinin ({asgariStok}) altına düştü!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    if (basariliIslemSayisi > 0)
                    {
                        MessageBox.Show($"{basariliIslemSayisi} adet ürün için iade işlemi başarıyla tamamlandı. Sadece stok güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UrunleriDataGridDoldur();
                        textBox1.Text = "0";
                        dataGridView1.ClearSelection(); // İşlem sonrası seçimi temizle
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("İşlem sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen iade edilecek ürünü seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(this.toptanciGsmTelefon))
            {
                MessageBox.Show("Toptancı GSM bilgisi eksik.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string baglantiDizesi = "Provider=Microsoft.ACE.OleDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            using (OleDbConnection baglan = new OleDbConnection(baglantiDizesi))
            {
                try
                {
                    baglan.Open();
                    int basariliIslemSayisi = 0;
                    decimal toplamIadeTutari = 0;
                    string toptanciAdi = string.Empty;

                    // Veritabanından mevcut borcu GSM numarasına göre çek
                    decimal mevcutBorc = 0;
                    string eskiBorcSorgu = "SELECT ToplamBorc FROM Toptancilar WHERE GsmTelefon = ?";
                    using (OleDbCommand eskiBorcKmt = new OleDbCommand(eskiBorcSorgu, baglan))
                    {
                        eskiBorcKmt.Parameters.Add("?", OleDbType.VarWChar, 255).Value = this.toptanciGsmTelefon;
                        object eskiBorcObj = eskiBorcKmt.ExecuteScalar();
                        if (eskiBorcObj != null && eskiBorcObj != DBNull.Value)
                        {
                            decimal.TryParse(eskiBorcObj.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out mevcutBorc);
                        }
                    }

                    foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
                    {
                        if (selectedRow.Cells["Miktar"].Value == null || !decimal.TryParse(selectedRow.Cells["Miktar"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal iadeMiktari) || iadeMiktari <= 0)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için geçerli bir iade miktarı girilmedi. Bu ürün atlandı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        if (!decimal.TryParse(selectedRow.Cells["BirimFiyati"].Value?.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal birimFiyati))
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için birim fiyatı geçersiz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        // Stok kontrolü
                        if (!decimal.TryParse(selectedRow.Cells["Stok_Miktari"].Value?.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal mevcutStok))
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için stok bilgisi geçersiz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        if (iadeMiktari > mevcutStok)
                        {
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' için iade miktarı mevcut stoktan fazla. İşlem yapılmadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        // Stok güncelleme
                        string stokGuncellemeSorgusu = "UPDATE ÜrünGirişi SET Stok_Miktari = Stok_Miktari - @iadeMiktar WHERE Barkod_No = @barkod";
                        using (OleDbCommand stokKomut = new OleDbCommand(stokGuncellemeSorgusu, baglan))
                        {
                            stokKomut.Parameters.AddWithValue("@iadeMiktar", iadeMiktari);
                            stokKomut.Parameters.AddWithValue("@barkod", selectedRow.Cells["Barkod_No"].Value.ToString());
                            stokKomut.ExecuteNonQuery();
                            basariliIslemSayisi++;
                        }

                        // Toptancı borç kontrolü
                        decimal iadeTutari = iadeMiktari * birimFiyati;
                        if (iadeTutari > mevcutBorc)
                        {
                            iadeTutari = mevcutBorc; // Borcu aşarsa, sadece mevcut borç kadar düş
                        }

                        toplamIadeTutari += iadeTutari;
                        mevcutBorc -= iadeTutari;

                        if (string.IsNullOrEmpty(toptanciAdi))
                        {
                            toptanciAdi = selectedRow.Cells["Toptanci_Adi"].Value?.ToString();
                        }
                    }

                    if (basariliIslemSayisi > 0)
                    {
                        // Toptancı borcu güncelle
                        string toptanciBorcGuncelleSorgu = "UPDATE Toptancilar SET ToplamBorc = ? WHERE GsmTelefon = ?";
                        using (OleDbCommand toptanciBorcGuncelleKmt = new OleDbCommand(toptanciBorcGuncelleSorgu, baglan))
                        {
                            toptanciBorcGuncelleKmt.Parameters.Add("?", OleDbType.Currency).Value = mevcutBorc;
                            toptanciBorcGuncelleKmt.Parameters.Add("?", OleDbType.VarWChar, 255).Value = this.toptanciGsmTelefon;
                            toptanciBorcGuncelleKmt.ExecuteNonQuery();
                        }

                        // BorcOdeme kaydı ekle
                        string borcOdemeSorgu = "INSERT INTO BorcOdeme (ToptanciAdi, GsmTelefon, EskiBorc, [Tarih/Saat], OdenenTutar, ToplamKalanBorc, Aciklama, OdemeSekli) " +
                                                "VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
                        using (OleDbCommand borcOdemeKmt = new OleDbCommand(borcOdemeSorgu, baglan))
                        {
                            DateTime islemTarihSaat = DateTime.Now;

                            borcOdemeKmt.Parameters.Add("?", OleDbType.VarWChar, 255).Value = toptanciAdi;
                            borcOdemeKmt.Parameters.Add("?", OleDbType.VarWChar, 255).Value = this.toptanciGsmTelefon;
                            borcOdemeKmt.Parameters.Add("?", OleDbType.Currency).Value = mevcutBorc + toplamIadeTutari;
                            borcOdemeKmt.Parameters.Add("?", OleDbType.Date).Value = islemTarihSaat;
                            borcOdemeKmt.Parameters.Add("?", OleDbType.Currency).Value = toplamIadeTutari;
                            borcOdemeKmt.Parameters.Add("?", OleDbType.Currency).Value = mevcutBorc;
                            borcOdemeKmt.Parameters.Add("?", OleDbType.VarWChar, 255).Value = "Ürün iade - toptancı borcundan düşüldü";
                            borcOdemeKmt.Parameters.Add("?", OleDbType.VarWChar, 50).Value = "Toptancı Borcundan Düşüldü";

                            borcOdemeKmt.ExecuteNonQuery();
                        }

                        MessageBox.Show($"{basariliIslemSayisi} adet ürün için iade işlemi başarıyla tamamlandı. Stok ve toptancı borcu güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UrunleriDataGridDoldur();
                      textBox1.Text = "0";
                        dataGridView1.ClearSelection(); // İşlem sonrası seçimi temizle
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("İşlem sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void Listele()
        {
            UrunleriDataGridDoldur();
        }
        private void textBox2_TextChanged_1(object sender, EventArgs e)
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
                               $"OR Ürün_Adi LIKE '%{aramaMetni}%'";
            }
        }
        public static class GlobalAyarlar
        {
            public static bool KdvDahilGoster { get; set; } = true; // Varsayılan olarak KDV Dahil
        }
        private void button4_Click_1(object sender, EventArgs e)
        {
            // Açılacak Toptanci formunu kontrol et, zaten açıksa yeni form açma
            Toptanci toptanciForm = Application.OpenForms.OfType<Toptanci>().FirstOrDefault();
            if (toptanciForm == null)
            {
                toptanciForm = new Toptanci();
                toptanciForm.Show();
            }
            else
            {
                // Zaten açıksa formu öne getir
                toptanciForm.BringToFront();
            }

            // Açık Ürün_Girişi formunu kapat
            foreach (Form openForm in Application.OpenForms.OfType<Ürün_Girişi>().ToList())
            {
                openForm.Close();
            }

            // Mevcut formu kapat
            this.Close();

        }


        private void timer1_Tick_1(object sender, EventArgs e)
        {
            textBox4.Text = DateTime.Now.ToShortDateString();
            textBox5.Text = DateTime.Now.ToLongTimeString();
        }
    }
}
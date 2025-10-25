using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
namespace ÜrünYönetimSistemi
{
    public partial class UrunIadeEt : Form
    {
        // Sınıf düzeyinde tanımlamalar
        private readonly CultureInfo _culture = new CultureInfo("tr-TR");
        private bool _isUpdatingTextBox = false;
        public UrunIadeEt()
        {
            InitializeComponent();
            InitializeForm();
        }
        private void UrunIadeEt_Load(object sender, EventArgs e)
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
            textBox1.Text = "0";
            dataGridView1.ClearSelection();
        }
        private void InitializeForm()
        {
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox4.Text = DateTime.Now.ToShortDateString();
            textBox5.Text = DateTime.Now.ToLongTimeString();
            textBox3.ReadOnly = true;
            textBox1.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
                    e.Handled = true;
            };
        }
        private string GetConnectionString()
        {
            return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Application.StartupPath}\\ÜrünYönetimSistemi.accdb";
        }
        private void DataGridViewStilAyarla()
        {
            dataGridView1.Columns["Barkod_No"].HeaderText = "Barkod No";
            dataGridView1.Columns["Ürün_Adi"].HeaderText = "Ürün Adı";
            dataGridView1.Columns["Stok_Miktari"].HeaderText = "Kalan Stok";
            dataGridView1.Columns["AsgariStok"].HeaderText = "Asgari Stok";
            dataGridView1.Columns["OlcuBirimi"].HeaderText = "Ölçü Birimi";
            dataGridView1.Columns["Birim Fiyatı"].HeaderText = "Birim Fiyatı";
            dataGridView1.Columns["Miktar"].HeaderText = "İade Edilecek Miktarı";
            dataGridView1.Columns["ToplamTutar"].HeaderText = "Toplam Tutar";
            dataGridView1.Columns["ToplamTutar"].DefaultCellStyle.Format = "C2";
            dataGridView1.Columns["ToplamTutar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            // Sadece Miktar sütunu düzenlenebilir
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.ReadOnly = true;
            }
            dataGridView1.Columns["Miktar"].ReadOnly = false;
        }
        private void ToplamTutarHesaplaVeGoster()
        {
            decimal toplam = 0;
            foreach (DataGridViewRow r in dataGridView1.SelectedRows)
            {
                if (decimal.TryParse(r.Cells["Miktar"].Value?.ToString().Replace(",", "."),
                        NumberStyles.Any, CultureInfo.InvariantCulture, out decimal miktar) &&
                    decimal.TryParse(r.Cells["Birim Fiyatı"].Value?.ToString().Replace(",", "."),
                        NumberStyles.Any, CultureInfo.InvariantCulture, out decimal birimFiyati))
                {
                    toplam += miktar * birimFiyati;
                }
            }
            textBox3.Text = toplam.ToString("N2", _culture);
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string aramaMetni = textBox2.Text.Trim();
            if (dataGridView1.DataSource is DataTable dt)
            {
                DataView dv = dt.DefaultView;
                if (string.IsNullOrEmpty(aramaMetni))
                {
                    dv.RowFilter = string.Empty;
                }
                else
                {
                    dv.RowFilter = $"CONVERT(Barkod_No, 'System.String') LIKE '%{aramaMetni}%' OR [Ürün_Adi] LIKE '%{aramaMetni}%'";
                }
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingTextBox) return;
            decimal miktar = 0;
            if (!decimal.TryParse(textBox1.Text.Replace(",", "."),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out miktar) || miktar < 0)
            {
                miktar = 0;
            }
            foreach (DataGridViewRow satir in dataGridView1.SelectedRows)
            {
                decimal birimFiyati = 0;
                if (decimal.TryParse(satir.Cells["Birim Fiyatı"].Value?.ToString().Replace(",", "."),
                        NumberStyles.Any, CultureInfo.InvariantCulture, out birimFiyati))
                {
                    satir.Cells["Miktar"].Value = miktar;
                    satir.Cells["ToplamTutar"].Value = miktar * birimFiyati;
                }
                else
                {
                    satir.Cells["Miktar"].Value = miktar;
                    satir.Cells["ToplamTutar"].Value = 0m;
                }
            }
            ToplamTutarHesaplaVeGoster();
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            _isUpdatingTextBox = true;
            if (dataGridView1.SelectedRows.Count == 1)
            {
                var satir = dataGridView1.SelectedRows[0];
                textBox1.Text = satir.Cells["Miktar"].Value?.ToString().Replace(".", ",") ?? "0";
            }
            else if (dataGridView1.SelectedRows.Count > 1)
            {
                // Çoklu seçimde textBox1'i en son seçilenin miktarıyla doldur veya temizle (opsiyonel: temizle)
                textBox1.Clear();
            }
            else
            {
               
            }
            _isUpdatingTextBox = false;
            ToplamTutarHesaplaVeGoster();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
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
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Miktar")
            {
                try
                {
                    var cellValue = dataGridView1.Rows[e.RowIndex].Cells["Miktar"].Value?.ToString();
                    if (decimal.TryParse(cellValue?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal miktar) && miktar >= 0)
                    {
                        var birimFiyatiCell = dataGridView1.Rows[e.RowIndex].Cells["Birim Fiyatı"].Value?.ToString();
                        if (decimal.TryParse(birimFiyatiCell?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal birimFiyati))
                        {
                            decimal toplamTutar = miktar * birimFiyati;
                            dataGridView1.Rows[e.RowIndex].Cells["ToplamTutar"].Value = toplamTutar;
                        }
                        else
                        {
                            dataGridView1.Rows[e.RowIndex].Cells["ToplamTutar"].Value = 0m;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Miktar geçersiz. Lütfen geçerli bir sayısal değer girin.", "Geçersiz Giriş", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dataGridView1.Rows[e.RowIndex].Cells["Miktar"].Value = 0m;
                        dataGridView1.Rows[e.RowIndex].Cells["ToplamTutar"].Value = 0m;
                    }
                    ToplamTutarHesaplaVeGoster();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen iade edilecekk ürünü seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int basariliIslemSayisi = 0;
            try
            {
                using (OleDbConnection baglan = new OleDbConnection(GetConnectionString()))
                {
                    baglan.Open();
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
                        decimal yeniStok = mevcutStok - iadeMiktari;
                        // Stok zaten 0 ise uyarı ver ve onay al
                        if (mevcutStok == 0)
                        {
                            DialogResult result = MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' stok miktarı zaten 0! İade işlemi sıfır stokla devam etsin mi?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                yeniStok = 0; // Stok sıfır olarak kalır
                            }
                            else
                            {
                                continue; // Onay verilmezse işlemi atla
                            }
                        }
                        // Stok sıfırın altına düşerse sıfır olarak ayarla
                        if (yeniStok < 0)
                        {
                            yeniStok = 0;
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' stok miktarı sıfırın altına düşecekti, stok 0 olarak ayarlandı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        decimal dusulecekMiktar = iadeMiktari;

                        // Eğer yeni stok 0 olacaksa, sadece mevcut stoğu sıfırlayacak kadar düş
                        if (yeniStok == 0 && mevcutStok > 0)
                        {
                            dusulecekMiktar = mevcutStok;
                        }

                        StokGuncelle(baglan, selectedRow, -dusulecekMiktar);// İade olduğu için eksiltme
                        IadeKaydiVer(baglan, selectedRow, iadeMiktari, yeniStok);
                        basariliIslemSayisi++;
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
                MessageBox.Show($"Hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "İşlem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen iade alınacak ürünü seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int basariliIslemSayisi = 0;
            try
            {
                using (OleDbConnection baglan = new OleDbConnection(GetConnectionString()))
                {
                    baglan.Open();
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
                        decimal yeniStok = mevcutStok - iadeMiktari;
                        // Stok zaten 0 ise uyarı ver ve onay al
                        if (mevcutStok == 0)
                        {
                            DialogResult result = MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' stok miktarı zaten 0! İade işlemi sıfır stokla devam etsin mi?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                yeniStok = 0; // Stok sıfır olarak kalır
                            }
                            else
                            {
                                continue; // Onay verilmezse işlemi atla
                            }
                        }
                        // Stok sıfırın altına düşerse sıfır olarak ayarla
                        if (yeniStok < 0)
                        {
                            yeniStok = 0;
                            MessageBox.Show($"'{selectedRow.Cells["Ürün_Adi"].Value}' stok miktarı sıfırın altına düşecekti, stok 0 olarak ayarlandı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        decimal dusulecekMiktar = iadeMiktari;

                        // Eğer yeni stok 0 olacaksa, sadece mevcut stoğu sıfırlayacak kadar düş
                        if (yeniStok == 0 && mevcutStok > 0)
                        {
                            dusulecekMiktar = mevcutStok;
                        }

                        StokGuncelle(baglan, selectedRow, -dusulecekMiktar); // İade olduğu için eksiltme
                        IadeVer(baglan, selectedRow, iadeMiktari, yeniStok);
                        basariliIslemSayisi++;
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
                MessageBox.Show($"Hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "İşlem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void StokGuncelle(OleDbConnection baglan, DataGridViewRow satir, decimal miktar)
        {
            decimal mevcutStok = 0;

            if (satir.Cells["Stok_Miktari"].Value != null &&
                decimal.TryParse(satir.Cells["Stok_Miktari"].Value.ToString().Replace(",", "."),
                                 NumberStyles.Any, CultureInfo.InvariantCulture, out mevcutStok))
            {
                decimal yeniStok = mevcutStok + miktar;

                // ❌ Eğer stok 0'ın altına düşerse, her zaman 0 yap
                if (yeniStok < 0)
                    yeniStok = 0;

                string updateQuery = "UPDATE ÜrünGirişi SET Stok_Miktari = @Stok WHERE Barkod_No = @BarkodNo";
                using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, baglan))
                {
                    updateCmd.Parameters.AddWithValue("@Stok", yeniStok);
                    updateCmd.Parameters.AddWithValue("@BarkodNo", satir.Cells["Barkod_No"].Value?.ToString());
                    updateCmd.ExecuteNonQuery();
                }

                // Grid üzerindeki hücreyi de güncelle
                satir.Cells["Stok_Miktari"].Value = yeniStok;
            }
        }

        private void IadeKaydiVer(OleDbConnection baglan, DataGridViewRow satir, decimal iadeMiktari, decimal yeniStok)
        {
            // Eğer stok eksiye düşüyorsa 0 olarak ayarla
            if (yeniStok < 0)
                yeniStok = 0;

            string insertQuery = "INSERT INTO [UrunIade] (Barkod_No, Ürün_Adi, Aciklama, Stok_Miktari, IadeEdilenMiktar, ToplamTutar, Tarih, Saat, ToptanciAdi, GsmTelefon) VALUES (@BarkodNo, @Ürün_Adi, @Aciklama, @StokMiktari, @IadeEdilenMiktar, @ToplamTutar, @Tarih, @Saat, @ToptanciAdi, @GsmTelefon)";
            using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, baglan))
            {
                decimal birimFiyati = 0;
                if (!decimal.TryParse(satir.Cells["Birim Fiyatı"].Value?.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out birimFiyati))
                {
                    MessageBox.Show($"'{satir.Cells["Ürün_Adi"].Value}' için birim fiyatı okunamadı. Toplam tutar sıfır olarak kaydedilecek.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                decimal toplamTutar = iadeMiktari * birimFiyati;

                insertCmd.Parameters.AddWithValue("@BarkodNo", satir.Cells["Barkod_No"].Value?.ToString() ?? string.Empty);
                insertCmd.Parameters.AddWithValue("@Ürün_Adi", satir.Cells["Ürün_Adi"].Value?.ToString() ?? string.Empty);
                insertCmd.Parameters.AddWithValue("@Aciklama", "Toptancıya İade - Belirtmek İstemiyorum");
                insertCmd.Parameters.AddWithValue("@StokMiktari", yeniStok);
                insertCmd.Parameters.AddWithValue("@IadeEdilenMiktar", iadeMiktari);
                insertCmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar);
                insertCmd.Parameters.AddWithValue("@Tarih", textBox4.Text);
                insertCmd.Parameters.AddWithValue("@Saat", textBox5.Text);
                insertCmd.Parameters.AddWithValue("@ToptanciAdi", "");
                insertCmd.Parameters.AddWithValue("@GsmTelefon", "");
                insertCmd.ExecuteNonQuery();

                if (satir.Cells["AsgariStok"].Value != null &&
                    decimal.TryParse(satir.Cells["AsgariStok"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal asgariStok) &&
                    yeniStok < asgariStok)
                {
                    MessageBox.Show($"'{satir.Cells["Ürün_Adi"].Value}' stok miktarı ({yeniStok}) asgari stok seviyesinin ({asgariStok}) altına düştü!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void IadeVer(OleDbConnection baglan, DataGridViewRow satir, decimal iadeMiktari, decimal yeniStok)
        {
            // Eğer stok eksiye düşüyorsa 0 olarak ayarla
            if (yeniStok < 0)
                yeniStok = 0;

            string insertQuery = "INSERT INTO [UrunIade] (Barkod_No, Ürün_Adi, Aciklama, Stok_Miktari, IadeEdilenMiktar, ToplamTutar, Tarih, Saat, ToptanciAdi, GsmTelefon) VALUES (@BarkodNo, @Ürün_Adi, @Aciklama, @StokMiktari, @IadeEdilenMiktar, @ToplamTutar, @Tarih, @Saat, @ToptanciAdi, @GsmTelefon)";
            using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, baglan))
            {
                decimal birimFiyati = 0;
                if (!decimal.TryParse(satir.Cells["Birim Fiyatı"].Value?.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out birimFiyati))
                {
                    MessageBox.Show($"'{satir.Cells["Ürün_Adi"].Value}' için birim fiyatı okunamadı. Toplam tutar sıfır olarak kaydedilecek.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                decimal toplamTutar = iadeMiktari * birimFiyati;

                insertCmd.Parameters.AddWithValue("@BarkodNo", satir.Cells["Barkod_No"].Value?.ToString() ?? string.Empty);
                insertCmd.Parameters.AddWithValue("@Ürün_Adi", satir.Cells["Ürün_Adi"].Value?.ToString() ?? string.Empty);
                insertCmd.Parameters.AddWithValue("@Aciklama", "Toptancıya İade - Toptancıya Nakit Ödendi");
                insertCmd.Parameters.AddWithValue("@StokMiktari", yeniStok);
                insertCmd.Parameters.AddWithValue("@IadeEdilenMiktar", iadeMiktari);
                insertCmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar);
                insertCmd.Parameters.AddWithValue("@Tarih", textBox4.Text);
                insertCmd.Parameters.AddWithValue("@Saat", textBox5.Text);
                insertCmd.Parameters.AddWithValue("@ToptanciAdi", "");
                insertCmd.Parameters.AddWithValue("@GsmTelefon", "");
                insertCmd.ExecuteNonQuery();

                if (satir.Cells["AsgariStok"].Value != null &&
                    decimal.TryParse(satir.Cells["AsgariStok"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal asgariStok) &&
                    yeniStok < asgariStok)
                {
                    MessageBox.Show($"'{satir.Cells["Ürün_Adi"].Value}' stok miktarı ({yeniStok}) asgari stok seviyesinin ({asgariStok}) altına düştü!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void UrunleriDataGridDoldur()
        {
            string sorgu = @"
                SELECT
                    UG.Barkod_No,
                    UG.Ürün_Adi AS [Ürün_Adi],
                    UG.Stok_Miktari AS [Stok_Miktari],
                    UG.AsgariStok,
                    UG.OlcuBirimi,
                    UG.Satis_Fiyati AS [Birim Fiyatı]
                FROM ÜrünGirişi AS UG
                ORDER BY UG.Ürün_Adi ASC;";
            try
            {
                using (OleDbConnection baglan = new OleDbConnection(GetConnectionString()))
                {
                    baglan.Open();
                    DataTable dt = new DataTable();
                    using (OleDbDataAdapter da = new OleDbDataAdapter(sorgu, baglan))
                    {
                        da.Fill(dt);
                    }
                    dt.Columns.Add("Miktar", typeof(decimal));
                    dt.Columns.Add("ToplamTutar", typeof(decimal));
                    foreach (DataRow row in dt.Rows)
                    {
                        row["Miktar"] = 0m;
                        row["ToplamTutar"] = 0m;
                    }
                    dataGridView1.DataSource = dt;
                    DataGridViewStilAyarla();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
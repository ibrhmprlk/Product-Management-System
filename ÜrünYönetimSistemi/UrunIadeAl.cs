using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ÜrünYönetimSistemi
{
    public partial class UrunIadeAl : Form
    {
        // Sınıf düzeyinde tanımlamalar
        private readonly CultureInfo _culture = new CultureInfo("tr-TR");
        private bool _isUpdatingTextBox = false;

        public UrunIadeAl()
        {
            InitializeComponent();
            InitializeForm();
            // Olayları bağlama
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

        private void UrunIadeAl_Load(object sender, EventArgs e)
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

        private string GetConnectionString()
        {
            return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Application.StartupPath}\\ÜrünYönetimSistemi.accdb";
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

        private void DataGridViewStilAyarla()
        {
            dataGridView1.Columns["Barkod_No"].HeaderText = "Barkod No";
            dataGridView1.Columns["Ürün_Adi"].HeaderText = "Ürün Adı";
            dataGridView1.Columns["Stok_Miktari"].HeaderText = "Kalan Stok";
            dataGridView1.Columns["AsgariStok"].HeaderText = "Asgari Stok";
            dataGridView1.Columns["OlcuBirimi"].HeaderText = "Ölçü Birimi";
            dataGridView1.Columns["Birim Fiyatı"].HeaderText = "Birim Fiyatı";
            dataGridView1.Columns["Miktar"].HeaderText = "İade Alınacak Miktarı";
            dataGridView1.Columns["ToplamTutar"].HeaderText = "Toplam Tutar";
            dataGridView1.Columns["ToplamTutar"].DefaultCellStyle.Format = "C2";
            dataGridView1.Columns["ToplamTutar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Sadece Miktar sütunu düzenlenebilir
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.ReadOnly = true;
            }
            dataGridView1.Columns["Miktar"].ReadOnly = false; // Düzeltme: false olmalı, düzenlenebilir olması için
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox4.Text = DateTime.Now.ToShortDateString();
            textBox5.Text = DateTime.Now.ToLongTimeString();
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

        private void button1_Click(object sender, EventArgs e)
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
                        decimal yeniStok = mevcutStok + iadeMiktari;

                        StokGuncelle(baglan, selectedRow, iadeMiktari);
                        IadeKaydiEkle(baglan, selectedRow, iadeMiktari, yeniStok);

                        basariliIslemSayisi++;
                    }
                }

                if (basariliIslemSayisi > 0)
                {
                    MessageBox.Show($"{basariliIslemSayisi} adet ürün için iade işlemi başarıyla tamamlandı. Stoklar güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UrunleriDataGridDoldur();
                    
                    textBox1.Text = "0";
                    dataGridView1.ClearSelection(); // İşlem sonrası seçimi temizle
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "İşlem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StokGuncelle(OleDbConnection baglan, DataGridViewRow satir, decimal iadeMiktari)
        {
            string stokGuncellemeSorgusu = "UPDATE ÜrünGirişi SET Stok_Miktari = CStr(VAL(Stok_Miktari) + ?) WHERE Barkod_No = ?";
            using (OleDbCommand komut = new OleDbCommand(stokGuncellemeSorgusu, baglan))
            {
                komut.Parameters.Add("?", OleDbType.Double).Value = iadeMiktari;
                komut.Parameters.Add("?", OleDbType.VarWChar).Value = satir.Cells["Barkod_No"].Value.ToString();
                komut.ExecuteNonQuery();
            }
        }

        private void IadeKaydiEkle(OleDbConnection baglan, DataGridViewRow satir, decimal iadeMiktari, decimal yeniStok)
        {
            string insertQuery = "INSERT INTO [MusteriIade] (Barkod_No, Ürün_Adi, Aciklama, Stok_Miktari, IadeAlinanMiktar, ToplamTutar, Tarih, Saat, MusteriAdi, GsmTelefon) VALUES (@BarkodNo, @Ürün_Adi, @Aciklama, @StokMiktari, @IadeAlinanMiktar, @ToplamTutar, @Tarih, @Saat, @MusteriAdi, @GsmTelefon)";
            using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, baglan))
            {
                decimal birimFiyati = 0;
                if (!decimal.TryParse(satir.Cells["Birim Fiyatı"].Value?.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out birimFiyati))
                {
                    MessageBox.Show($"'{satir.Cells["Ürün_Adi"].Value}' için birim fiyatı okunamadı. Toplam tutar sıfır olarak kaydedilecek.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                decimal toplamTutar = iadeMiktari * birimFiyati;

                // Parametreler doğru sırayla ve doğru değerlerle ekleniyor
                insertCmd.Parameters.AddWithValue("@BarkodNo", satir.Cells["Barkod_No"].Value?.ToString() ?? string.Empty);
                insertCmd.Parameters.AddWithValue("@Ürün_Adi", satir.Cells["Ürün_Adi"].Value?.ToString() ?? string.Empty);
                insertCmd.Parameters.AddWithValue("@Aciklama", "Müşteri İadesi - İade Türünü Belirtmek İstemiyorum");
                insertCmd.Parameters.AddWithValue("@StokMiktari", yeniStok);
                insertCmd.Parameters.AddWithValue("@IadeAlinanMiktar", iadeMiktari);
                insertCmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar);
                insertCmd.Parameters.AddWithValue("@Tarih", textBox4.Text);
                insertCmd.Parameters.AddWithValue("@Saat", textBox5.Text);
                insertCmd.Parameters.AddWithValue("@MusteriAdi", ""); // Boş string
                insertCmd.Parameters.AddWithValue("@GsmTelefon", ""); // Boş string
                insertCmd.ExecuteNonQuery();

                if (satir.Cells["AsgariStok"].Value != null &&
                    decimal.TryParse(satir.Cells["AsgariStok"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal asgariStok) &&
                    yeniStok < asgariStok)
                {
                    MessageBox.Show($"'{satir.Cells["Ürün_Adi"].Value}' stok miktarı ({yeniStok}) asgari stok seviyesinin ({asgariStok}) altına düştü!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        decimal yeniStok = mevcutStok + iadeMiktari;

                        StokGuncelle(baglan, selectedRow, iadeMiktari);
                        MusteriIade(baglan, selectedRow, iadeMiktari, yeniStok);

                        basariliIslemSayisi++;
                    }
                }

                if (basariliIslemSayisi > 0)
                {
                    MessageBox.Show($"{basariliIslemSayisi} adet ürün için iade işlemi başarıyla tamamlandı. Stoklar güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UrunleriDataGridDoldur();
                   
                    textBox1.Text = "0";
                    dataGridView1.ClearSelection(); // İşlem sonrası seçimi temizle
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "İşlem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MusteriIade(OleDbConnection baglan, DataGridViewRow satir, decimal iadeMiktari, decimal yeniStok)
        {
            string insertQuery = "INSERT INTO [MusteriIade] (Barkod_No, Ürün_Adi, Aciklama, Stok_Miktari, IadeAlinanMiktar, ToplamTutar, Tarih, Saat, MusteriAdi, GsmTelefon) VALUES (@BarkodNo, @Ürün_Adi, @Aciklama, @StokMiktari, @IadeAlinanMiktar, @ToplamTutar, @Tarih, @Saat, @MusteriAdi, @GsmTelefon)";
            using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, baglan))
            {
                decimal birimFiyati = 0;
                if (!decimal.TryParse(satir.Cells["Birim Fiyatı"].Value?.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out birimFiyati))
                {
                    MessageBox.Show($"'{satir.Cells["Ürün_Adi"].Value}' için birim fiyatı okunamadı. Toplam tutar sıfır olarak kaydedilecek.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                decimal toplamTutar = iadeMiktari * birimFiyati;

                // Parametreler doğru sırayla ve doğru değerlerle ekleniyor
                insertCmd.Parameters.AddWithValue("@BarkodNo", satir.Cells["Barkod_No"].Value?.ToString() ?? string.Empty);
                insertCmd.Parameters.AddWithValue("@Ürün_Adi", satir.Cells["Ürün_Adi"].Value?.ToString() ?? string.Empty);
                insertCmd.Parameters.AddWithValue("@Aciklama", "Müşteri İadesi - Müşteri Nakit Ödedi");
                insertCmd.Parameters.AddWithValue("@StokMiktari", yeniStok);
                insertCmd.Parameters.AddWithValue("@IadeAlinanMiktar", iadeMiktari);
                insertCmd.Parameters.AddWithValue("@ToplamTutar", toplamTutar);
                insertCmd.Parameters.AddWithValue("@Tarih", textBox4.Text);
                insertCmd.Parameters.AddWithValue("@Saat", textBox5.Text);
                insertCmd.Parameters.AddWithValue("@MusteriAdi", ""); // Boş string
                insertCmd.Parameters.AddWithValue("@GsmTelefon", ""); // Boş string
                insertCmd.ExecuteNonQuery();

                if (satir.Cells["AsgariStok"].Value != null &&
                    decimal.TryParse(satir.Cells["AsgariStok"].Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal asgariStok) &&
                    yeniStok < asgariStok)
                {
                    MessageBox.Show($"'{satir.Cells["Ürün_Adi"].Value}' stok miktarı ({yeniStok}) asgari stok seviyesinin ({asgariStok}) altına düştü!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

    }
}
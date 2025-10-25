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
    public partial class Toplu_Ürün_Sil : Form
    {
        public Form1 frm1;
        public Form2 frm2;
        private string secilenGsmTelefon;
        private DataTable dt; // dataGridView1 DataSource

        public Toplu_Ürün_Sil()
        {
            InitializeComponent();

            Listele();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

        }

        private void Toplu_Ürün_Sil_Load(object sender, EventArgs e)
        {
            // DataGridView2 sütunları
            dataGridView2.ColumnCount = 4;
            dataGridView2.Columns[0].Name = "Barkod No";
            dataGridView2.Columns[1].Name = "Ürün Adı";
            dataGridView2.Columns[2].Name = "Mevcut Stok";
            dataGridView2.Columns[3].Name = "Ürün Grubu";
            dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;
            dataGridView2.CellMouseClick += dataGridView2_CellMouseClick;
            // DataGridView ayarları
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView2.EditMode = DataGridViewEditMode.EditProgrammatically;

            // <<< Çözüm 1: Çift tıklama sorununu çözmek için eski olayı kaldır >>>
            // Tasarımcı (Designer.cs) tarafından eklenmiş olabilecek CellClick olayını kaldırır.
            dataGridView1.CellClick -= dataGridView1_CellClick;

            // Kullanıcının İsteği: dataGridView2'nin CellClick olayını kaldır ve yerine MouseDown ekle
            dataGridView2.CellClick -= dataGridView2_CellClick; // Önceki CellClick kaldırıldı

            // <<< Çözüm 2: Boşluğa tıklama sorununu çözmek için MouseDown olayını ekle >>>
            comboBox1.MaxDropDownItems = 10;

            // 3. (GEREKLİ EKLEME) Listeden ilk 10 öğenin yüksekliğini hesaplayarak 
            // açılır listeyi piksel cinsinden sınırlandırın.
            // Ortalama bir satır yüksekliği 15-18 pikseldir. 10 satır için 180 piksel deneyelim:

            int maxYukseklik = 10 * comboBox1.ItemHeight + 2; // 2, kenarlıklar için küçük bir boşluk

            // Eğer bu özelliği kullanan bir WinForms ortamındaysanız, bu satır işe yarayacaktır:
            comboBox1.DropDownHeight = maxYukseklik;

            Listele();

            // RowsRemoved eventlerini bağla
            dataGridView1.RowsRemoved += DataGridView_RowsRemoved;
            dataGridView2.RowsRemoved += DataGridView_RowsRemoved;

            // İlk durum için button2 görünürlüğünü ayarla
            UpdateButton2Visibility();
            UrunGrubuDoldur();
        }

        private void Listele()
        {
            string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
            {
                // Sorguyu güncelledim: Ürün_Grubu sütununu da çekiyoruz
                string sorgu = "SELECT Barkod_No AS [Barkod No], Ürün_Adi AS [Ürün Adı], Stok_Miktari AS [Mevcut Stok], Ürün_Grubu AS [Ürün Grubu] FROM ÜrünGirişi";

                OleDbDataAdapter da = new OleDbDataAdapter(sorgu, baglan);
                dt = new DataTable();
                da.Fill(dt);

                // DataTable'da birincil anahtar tanımla (önceki hatayı çözmek için)
                DataColumn[] primaryKeys = new DataColumn[1];
                primaryKeys[0] = dt.Columns["Barkod No"];
                dt.PrimaryKey = primaryKeys;

                // DataTable'ı bir DataView aracılığıyla DataGridView'e bağla
                dataGridView1.DataSource = dt.DefaultView;
            }
        }

        // DataGridView1 MouseDown Olayı (Ürünler tablosundan Silinecekler tablosuna taşıma)
        // DataGridView1 MouseDown Olayı (Ürünler tablosundan Silinecekler tablosuna taşıma)
        // YENİ: DataGridView1 CellMouseClick Olayı (Ürünler -> Silinecekler)
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Sol tıklama ve geçerli bir satır olduğunu kontrol et
            if (e.Button == MouseButtons.Left && e.RowIndex >= 0)
            {
                // DataGridView'de seçili satırı (tıklanan satırı) al
                DataGridViewRow clickedRow = dataGridView1.Rows[e.RowIndex];

                // DataGridView2'ye ekle
                dataGridView2.Rows.Add(clickedRow.Cells["Barkod No"].Value,
                                      clickedRow.Cells["Ürün Adı"].Value,
                                      clickedRow.Cells["Mevcut Stok"].Value,
                                      clickedRow.Cells["Ürün Grubu"].Value);

                // DataTable'dan orijinal satırı bulup sil (Görüntülenen View'dan da silinir)
                string barkodNo = clickedRow.Cells["Barkod No"].Value.ToString();
                DataRow[] rowsToRemove = dt.Select(string.Format("[Barkod No] = '{0}'", barkodNo));

                if (rowsToRemove.Length > 0)
                {
                    dt.Rows.Remove(rowsToRemove[0]);
                    dt.AcceptChanges(); // Değişikliği onayla ki DataView güncellensin
                }

                button1.Text = "Silinecek Ürünleri Ürünler Tablosuna Ekle";
                UpdateButton2Visibility();
            }
        }

        // YENİ: DataGridView2 MouseDown Olayı (Silinecekler tablosundan Ürünler tablosuna geri taşıma)
        // YENİ: DataGridView2 MouseDown Olayı (Silinecekler tablosundan Ürünler tablosuna geri taşıma)
        // YENİ: DataGridView2 CellMouseClick Olayı (Silinecekler -> Ürünler)
        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Sol tıklama ve geçerli bir satır olduğunu kontrol et
            if (e.Button == MouseButtons.Left && e.RowIndex >= 0)
            {
                DataGridViewRow clickedRow = dataGridView2.Rows[e.RowIndex];

                // Satırın yeni satır olmadığını kontrol et
                if (clickedRow.IsNewRow) return;

                // DataRow oluştur ve DataTable'a ekle (Geri Taşıma)
                DataRow newRow = dt.NewRow();
                newRow["Barkod No"] = clickedRow.Cells["Barkod No"].Value;
                newRow["Ürün Adı"] = clickedRow.Cells["Ürün Adı"].Value;
                newRow["Mevcut Stok"] = clickedRow.Cells["Mevcut Stok"].Value;
                newRow["Ürün Grubu"] = clickedRow.Cells["Ürün Grubu"].Value;

                dt.Rows.Add(newRow);

                // DataGridView2'den satırı sil
                dataGridView2.Rows.RemoveAt(e.RowIndex); // e.RowIndex kullanmak daha güvenli

                button1.Text = "Ürünleri Silinecekler Tablosuna Ekle";

                UpdateButton2Visibility();
            }
        }

        // Eski CellClick metodu (Artık kullanılmıyor ancak silinmesi sorun çıkarabilir diye bırakıldı, sadece içi boşaltılabilir veya yukarıdaki gibi - kaldırılabilir)
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // MouseDown olayı kullanıldığı için burası boş bırakıldı.
        }

        // ESKİ: DataGridView2 CellClick metodu (Kullanıcının isteği üzerine iptal edildi)
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // MouseDown olayı kullanıldığı için burası boş bırakıldı.
        }

        // Buton ile toplu taşıma
        private void button1_Click(object sender, EventArgs e)
        {
            // YENİ EKLENEN KONTROL VE MESAJ KUTUSU
            // Eğer her iki DataGridView de boşsa VE buton metni "Ürünleri Silinecekler Tablosuna Ekle" değilse
            if (dt.DefaultView.Count == 0 && dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Aktarılacak ürün bulunmamaktadır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // Metottan hemen çık
            }

            if (button1.Text == "Ürünleri Silinecekler Tablosuna Ekle")
            {
                // Mevcut filtreli DataView'daki tüm satırları al
                List<DataRowView> rowsToMove = new List<DataRowView>();
                foreach (DataRowView rowView in dt.DefaultView)
                {
                    rowsToMove.Add(rowView);
                }

                if (rowsToMove.Count > 0)
                {
                    // Satırları dataGridView2'ye ekle
                    foreach (DataRowView rowView in rowsToMove)
                    {
                        dataGridView2.Rows.Add(
                          rowView["Barkod No"],
                          rowView["Ürün Adı"],
                          rowView["Mevcut Stok"],
                          rowView["Ürün Grubu"]
                        );
                        rowView.Row.Delete(); // DataTable'dan sil
                    }

                    dt.AcceptChanges(); // Silme işlemini onayla
                    button1.Text = "Silinecek Ürünleri Ürünler Tablosuna Ekle";
                }
                else
                {
                    // dataGridView1'de filtreleme sonucunda hiç ürün kalmamışsa butonu güncelle
                    if (dataGridView2.Rows.Count == 0)
                    {
                        button1.Text = "Ürün Yok Lütfen Ürün Ekleyin";
                    }
                }
            }
            else
            {
                // Bu kısım, silinecekler tablosundaki ürünleri geri taşır
                if (dataGridView2.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            DataRow newRow = dt.NewRow();
                            newRow["Barkod No"] = row.Cells["Barkod No"].Value;
                            newRow["Ürün Adı"] = row.Cells["Ürün Adı"].Value;
                            newRow["Mevcut Stok"] = row.Cells["Mevcut Stok"].Value;
                            newRow["Ürün Grubu"] = row.Cells["Ürün Grubu"].Value;
                            dt.Rows.Add(newRow);
                        }
                    }

                    dataGridView2.Rows.Clear();
                    button1.Text = "Ürünleri Silinecekler Tablosuna Ekle";
                }
            }

            UpdateButton2Visibility();
        }

        // Her iki DataGridView için RowsRemoved event
        private void DataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (dt.DefaultView.Count == 0 && dataGridView2.Rows.Count == 0)
            {
                button1.Text = "Ürünleri Silinecekler Tablosuna Ekle";
            }
            else if (dt.DefaultView.Count == 0) // dt.DefaultView.Count kullanıldı
            {
                button1.Text = "Silinecek Ürünleri Ürünler Tablosuna Ekle";
            }
            else if (dataGridView2.Rows.Count == 0)
            {
                button1.Text = "Ürünleri Silinecekler Tablosuna Ekle";
            }

            UpdateButton2Visibility();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0) return;

            if (MessageBox.Show("Seçilen ürünleri silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            button2.Enabled = false;

            var secilenUrunler = dataGridView2.Rows
                       .Cast<DataGridViewRow>()
                       .Where(r => !r.IsNewRow)
                       .Select(r => new
                       {
                           BarkodNo = r.Cells["Barkod No"].Value.ToString()
                       })
                       .ToList();

            await Task.Run(() =>
            {
                string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";
                using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
                {
                    baglan.Open();
                    using (OleDbTransaction tran = baglan.BeginTransaction())
                    {
                        foreach (var urun in secilenUrunler)
                        {
                            // 1️⃣ Silinecek ürünün bilgilerini al
                            decimal urunAlisFiyati = 0;
                            decimal urunStokMiktari = 0;
                            string urunToptanciGsm = string.Empty;

                            string selectQuery = "SELECT Alis_Fiyati, Stok_Miktari, GsmTelefon FROM ÜrünGirişi WHERE Barkod_No=@BarkodNo";
                            using (OleDbCommand selectKomut = new OleDbCommand(selectQuery, baglan, tran))
                            {
                                selectKomut.Parameters.AddWithValue("@BarkodNo", urun.BarkodNo);
                                using (OleDbDataReader reader = selectKomut.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        if (reader["Alis_Fiyati"] != DBNull.Value)
                                            urunAlisFiyati = decimal.Parse(reader["Alis_Fiyati"].ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                                        if (reader["Stok_Miktari"] != DBNull.Value)
                                            urunStokMiktari = decimal.Parse(reader["Stok_Miktari"].ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                                        if (reader["GsmTelefon"] != DBNull.Value)
                                            urunToptanciGsm = reader["GsmTelefon"].ToString();
                                    }
                                    else
                                    {
                                        continue; // Ürün yoksa atla
                                    }
                                }
                            }

                            // 2️⃣ Ürünü sil
                            string silQuery = "DELETE FROM ÜrünGirişi WHERE Barkod_No=@BarkodNo";
                            using (OleDbCommand silKomut = new OleDbCommand(silQuery, baglan, tran))
                            {
                                silKomut.Parameters.AddWithValue("@BarkodNo", urun.BarkodNo);
                                silKomut.ExecuteNonQuery();
                            }

                            // 3️⃣ Toptancı borcunu güncelle
                            if (!string.IsNullOrWhiteSpace(urunToptanciGsm))
                            {
                                decimal silinecekBorc = urunAlisFiyati * urunStokMiktari;
                                decimal mevcutBorc = 0;

                                string selectBorcQuery = "SELECT ToplamBorc FROM Toptancilar WHERE GsmTelefon=@GsmTelefon";
                                using (OleDbCommand cmdSelectBorc = new OleDbCommand(selectBorcQuery, baglan, tran))
                                {
                                    cmdSelectBorc.Parameters.AddWithValue("@GsmTelefon", urunToptanciGsm);
                                    object result = cmdSelectBorc.ExecuteScalar();
                                    if (result != DBNull.Value && result != null)
                                    {
                                        mevcutBorc = decimal.Parse(result.ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                }

                                decimal yeniToplamBorc = mevcutBorc - silinecekBorc;
                                if (yeniToplamBorc < 0) yeniToplamBorc = 0;

                                string updateToptanciQuery = "UPDATE Toptancilar SET ToplamBorc=@ToplamBorc WHERE GsmTelefon=@GsmTelefon";
                                using (OleDbCommand updateToptanciKmt = new OleDbCommand(updateToptanciQuery, baglan, tran))
                                {
                                    updateToptanciKmt.Parameters.AddWithValue("@ToplamBorc", yeniToplamBorc.ToString(System.Globalization.CultureInfo.InvariantCulture));
                                    updateToptanciKmt.Parameters.AddWithValue("@GsmTelefon", urunToptanciGsm);
                                    updateToptanciKmt.ExecuteNonQuery();
                                }
                            }
                        }

                        tran.Commit();
                    }
                    baglan.Close();
                }
            });

            dataGridView2.Rows.Clear();
            Listele();
            MessageBox.Show("Seçilen ürünler silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information); // Mesaj kutusu içeriği güncellendi
            UrunGrubuDoldur();
            button1.Text = "Ürünleri Silinecekler Tablosuna Ekle";
          
            UpdateButton2Visibility();

            button2.Enabled = true;

        }

        // DataGridView2’ye ürün eklendiğinde veya taşındığında button2 görünürlüğünü güncelle
        private void UpdateButton2Visibility()
        {
            button2.Visible = dataGridView2.Rows.Count > 0;
        }
        private void UrunGrubuDoldur()
        {
            string baglantiYolu = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb";

            try
            {
                using (OleDbConnection baglan = new OleDbConnection(baglantiYolu))
                {
                    baglan.Open();
                    // Distinct ile benzersiz ürün grubu adlarını çekiyoruz
                    string sorgu = "SELECT DISTINCT Ürün_Grubu FROM ÜrünGirişi";
                    using (OleDbCommand cmd = new OleDbCommand(sorgu, baglan))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            // Önce ComboBox'ı temizle
                            comboBox1.Items.Clear();
                            // "Tümü" seçeneğini en başa ekle
                            comboBox1.Items.Add("Tümü");

                            while (reader.Read())
                            {
                                if (reader["Ürün_Grubu"] != DBNull.Value)
                                {
                                    comboBox1.Items.Add(reader["Ürün_Grubu"].ToString());
                                }
                            }
                        }
                    }
                }
                comboBox1.SelectedIndex = 0; // "Tümü" seçeneğini varsayılan olarak seç
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ürün grubu listesi yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Filtrele()
        {
            if (dt == null) return;

            string aramaMetni = textBox1.Text.Trim().ToLower();
            List<string> filtreler = new List<string>();

            if (!string.IsNullOrEmpty(aramaMetni))
            {
                filtreler.Add(string.Format("Convert([Barkod No], 'System.String') LIKE '%{0}%' OR Convert([Ürün Adı], 'System.String') LIKE '%{0}%'", aramaMetni));
            }

            if (checkBox1.Checked)
            {
                filtreler.Add("[Mevcut Stok] = 0");
            }

            // comboBox1 filtresi artık Ürün Grubu için
            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() != "Tümü")
            {
                string secilenGrup = comboBox1.SelectedItem.ToString().Replace("'", "''");
                filtreler.Add(string.Format("[Ürün Grubu] = '{0}'", secilenGrup));
            }

            string sonFiltre = string.Join(" AND ", filtreler);

            dt.DefaultView.RowFilter = sonFiltre;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Filtrele();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            Filtrele();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Filtrele();
            // KeyDown olayı sadece bir kez atanmalı. TextChanged içinde tekrar atamak döngü yaratır.
            // Bu satır kaldırıldı: textBox1.KeyDown += textBox1_KeyDown;
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Enter sesini engelle
                string girilenBarkod = textBox1.Text.Trim();

                if (string.IsNullOrEmpty(girilenBarkod)) return;

                // DataTable'da arama
                // DataTable'daki sütun adları köşeli parantezli ve boşluklu olmalı: [Barkod No]
                DataRow[] bulunanSatirlar = dt.Select(string.Format("[Barkod No] = '{0}'", girilenBarkod));

                if (bulunanSatirlar.Length > 0)
                {
                    DataRow satir = bulunanSatirlar[0];

                    // DataGridView2'ye ekle (Sadece var olan 4 sütuna atama yapıldı)
                    int index = dataGridView2.Rows.Add();
                    dataGridView2.Rows[index].Cells["Barkod No"].Value = satir["Barkod No"];
                    dataGridView2.Rows[index].Cells["Ürün Adı"].Value = satir["Ürün Adı"];
                    dataGridView2.Rows[index].Cells["Mevcut Stok"].Value = satir["Mevcut Stok"];
                    dataGridView2.Rows[index].Cells["Ürün Grubu"].Value = satir["Ürün Grubu"];

                    // >>> Eski hatalı atamalar kaldırıldı (Çünkü bu sütunlar DataGridView2'de yok):
                    // dataGridView2.Rows[index].Cells["Satis_Fiyati"].Value = satir["Satis_Fiyati"];
                    // dataGridView2.Rows[index].Cells["2SatisFiyati"].Value = satir["2SatisFiyati"];
                    // dataGridView2.Rows[index].Cells["AsgariStok"].Value = satir["AsgariStok"];
                    // dataGridView2.Rows[index].Cells["Miktar"].Value = 1;
                    // dataGridView2.Rows[index].Cells["ToplamTutar"].Value = satir["Satis_Fiyati"];
                    // <<<

                    // DataTable'dan kaldır
                    dt.Rows.Remove(satir);

                    // Button2 görünürlüğünü güncelle
                    UpdateButton2Visibility();

                    // TextBox temizle
                    textBox1.Clear();
                }
                else
                {
                    MessageBox.Show("Barkod bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filtrele();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Ürün_Girişi urunGirisiForm = new Ürün_Girişi();
            urunGirisiForm.Show(); // Show() ile form açılır
            this.Close();
        }
    }
}
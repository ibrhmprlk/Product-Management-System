using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb; // Veritabanı işlemleri için ekliyoruz
using System.Linq; // Controls.Find ve gsm.All(char.IsDigit) için gerekli

namespace ÜrünYönetimSistemi
{
    public partial class Ayarlar : Form
    {
        private Form2 _anaForm;
        private string _secilenDosyaYolu; // Seçilen dosya yolunu geçici olarak tutar.

        // Form2 örneğini (1 bağımsız değişken) kabul eden yapıcı (constructor)
        public Ayarlar(Form2 anaForm)
        {
            InitializeComponent();
            textBox1.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)) e.Handled = true; };
            // BURASI GÜNCELLENDİ: Harf, boşluk ve kontrol tuşlarına ek olarak '/' karakterine de izin veriliyor.
            textBox3.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '/') e.Handled = true; };
            textBox4.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };

            _anaForm = anaForm;
            button3.Visible = false;
            button2.Visible = true;
            button6.Visible = false;
            this.Load += new EventHandler(Ayarlar_Load); // Form Yükleme olayını ekledik
        }

        // Parametresiz yapıcı (Tasarımcı ve varsayılan kullanım için)


        // --- Form Load Olayı: Mevcut Verileri Yükle ---
        private void Ayarlar_Load(object sender, EventArgs e)
        {
            VeriYukle();
        }

        private void VeriYukle()
        {
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=ÜrünYönetimSistemi.accdb";
            // Tablonun tek bir ayar satırı tuttuğu varsayımıyla sorgu
            string query = "SELECT IsletmeAdi, IsletmeYeri, IsletmeAdresi, GsmTelefon, ArkaPlanResmi FROM IsletmeAdi WHERE 1=1";

            // Önceki resmi temizle
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Verileri ilgili TextBox'lara yükle
                                // DB Alanları: IsletmeAdi (textBox1), IsletmeYeri (textBox3), IsletmeAdresi (textBox2), GsmTelefon (textBox4)
                                textBox1.Text = reader["IsletmeAdi"] != DBNull.Value ? reader["IsletmeAdi"].ToString() : "";
                                textBox3.Text = reader["IsletmeYeri"] != DBNull.Value ? reader["IsletmeYeri"].ToString() : "";
                                textBox2.Text = reader["IsletmeAdresi"] != DBNull.Value ? reader["IsletmeAdresi"].ToString() : "";
                                textBox4.Text = reader["GsmTelefon"] != DBNull.Value ? reader["GsmTelefon"].ToString() : "";

                                // Resim yolunu da yükle
                                _secilenDosyaYolu = reader["ArkaPlanResmi"] != DBNull.Value ? reader["ArkaPlanResmi"].ToString() : "";

                                // Resim yolu varsa, ilgili TextBox'a (varsayımı: textBoxResimYolu) yazıyoruz.
                                if (Controls.Find("textBoxResimYolu", true).FirstOrDefault() is TextBox tb)
                                {
                                    tb.Text = _secilenDosyaYolu;
                                }

                                // YÜKLEME KISMI BURADA BAŞLIYOR
                                if (!string.IsNullOrEmpty(_secilenDosyaYolu) && File.Exists(_secilenDosyaYolu))
                                {
                                    try
                                    {
                                        // Resmi bellek akışına yükle (dosya kilitlenmesini önler)
                                        using (var stream = new FileStream(_secilenDosyaYolu, FileMode.Open, FileAccess.Read))
                                        {
                                            Image newImage = Image.FromStream(stream);
                                            // PictureBox'ın Image özelliğine FixOrientation uygulanmış resmi ata
                                            pictureBox1.Image = FixOrientation(newImage);
                                        }
                                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                                        pictureBox1.Visible = true;
                                        button3.Visible = false; // Kayıtlı resim olduğu için kaydet butonu gizli kalabilir
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Arka plan resmi yüklenirken hata oluştu: " + ex.Message);
                                        // Hata durumunda PictureBox'ı temizle
                                        if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
                                        pictureBox1.Image = null;
                                        _secilenDosyaYolu = null; // Hatalı yolu geçersiz kıl
                                    }
                                }
                                else
                                {
                                    _secilenDosyaYolu = null;
                                    pictureBox1.Image = null;
                                    pictureBox1.Visible = false;
                                }
                                // YÜKLEME KISMI BURADA BİTİYOR

                            }
                            else
                            {
                                // Tabloda hiç satır yoksa, her şeyi temizle
                                _secilenDosyaYolu = null;
                                pictureBox1.Image = null;
                                pictureBox1.Visible = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata mesajını konsola yazdır (uygulama akışını kesmemek için)
                System.Diagnostics.Debug.WriteLine("Ayarlar yüklenirken hata oluştu: " + ex.Message);
            }
        }

        // --- button1_Click (İşletme Bilgilerini Kaydetme/Güncelleme) ---
        private void button1_Click(object sender, EventArgs e)
        {
            // İstenen alanlar: İşletme Adı (textBox1), Adresi (textBox2), Yeri (textBox3), GSM Telefon (textBox4)

            // --- 1. Veri Doğrulama (Validation) ---
            // Her bir alanı ayrı ayrı kontrol ederek, boş olan alana odaklanılmasını sağlar.

            // İşletme Adı Kontrolü
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("İşletme Adı alanı boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            // Adres Kontrolü
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Adresi alanı boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            // Yer Kontrolü
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Yeri alanı boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            // GSM Telefon Boş Kontrolü
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("GSM Telefon alanı boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return;
            }

            // GSM Telefon Temizleme ve Format Kontrolü: Tam 10 hane rakam olmalı
            string gsm = textBox4.Text.Trim().Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");

            if (gsm.Length != 10 || !gsm.All(char.IsDigit))
            {
                MessageBox.Show("GSM Telefon numarası tam 10 haneli rakam olmalıdır (Örn: 5551234567).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return;
            }

            // Doğrulamadan geçen verileri al
            string IsletmeAdi = textBox1.Text;
            string IsletmeAdresi = textBox2.Text;
            string IsletmeYeri = textBox3.Text;
            string GsmTelefon = gsm; // Temizlenmiş 10 haneli numara

            // Veritabanı bağlantı dizesi (Access için)
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=ÜrünYönetimSistemi.accdb";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    int affectedRows = 0;

                    // --- 2. UPDATE (Güncelleme) Denemesi ---
                    // Var olan veriyi güncellemeye çalış. (WHERE 1=1 ile tek bir kayıt varsa/varsa güncellenir)
                    string updateQuery = @"
                UPDATE IsletmeAdi 
                SET IsletmeAdi = @IsletmeAdi, 
                    IsletmeAdresi = @IsletmeAdresi, 
                    IsletmeYeri = @IsletmeYeri, 
                    GsmTelefon = @GsmTelefon 
                WHERE 1=1";

                    using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, conn))
                    {
                        // Access veritabanı için parametre sıralaması önemlidir (Query'deki sıralama).
                        updateCmd.Parameters.AddWithValue("@IsletmeAdi", IsletmeAdi);
                        updateCmd.Parameters.AddWithValue("@IsletmeAdresi", IsletmeAdresi);
                        updateCmd.Parameters.AddWithValue("@IsletmeYeri", IsletmeYeri);
                        updateCmd.Parameters.AddWithValue("@GsmTelefon", GsmTelefon);

                        affectedRows = updateCmd.ExecuteNonQuery();
                    }

                    // --- 3. INSERT (Ekleme) Kontrolü ---
                    // Eğer güncelleme sıfır satır etkilediyse, tablo boş demektir, ilk kaydı ekle.
                    if (affectedRows == 0)
                    {
                        string insertQuery = @"
                    INSERT INTO IsletmeAdi (IsletmeAdi, IsletmeAdresi, IsletmeYeri, GsmTelefon) 
                    VALUES (@IsletmeAdi, @IsletmeAdresi, @IsletmeYeri, @GsmTelefon)";

                        using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@IsletmeAdi", IsletmeAdi);
                            insertCmd.Parameters.AddWithValue("@IsletmeAdresi", IsletmeAdresi);
                            insertCmd.Parameters.AddWithValue("@IsletmeYeri", IsletmeYeri);
                            insertCmd.Parameters.AddWithValue("@GsmTelefon", GsmTelefon);

                            insertCmd.ExecuteNonQuery();
                            MessageBox.Show("İşletme bilgileri ilk defa başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("İşletme bilgileri başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı işlemi sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // EXIF orientation'ı düzelten yardımcı metod
        private Image FixOrientation(Image img)
        {
            if (img == null) return null;

            // EXIF Orientation tag'i var mı kontrol et (ID: 274 = 0x0112)
            if (Array.IndexOf(img.PropertyIdList, 274) > -1)
            {
                var orientationItem = img.GetPropertyItem(274);
                int orientation = orientationItem.Value[0];

                switch (orientation)
                {
                    case 2:
                        img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        img.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        img.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        img.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }

                // Tag'i sil ki tekrar uygulanmasın (isteğe bağlı, ama iyi pratik)
                img.RemovePropertyItem(274);
            }

            return img;
        }

        // --- button2_Click_1 (Resim Seçme ve Önizleme) ---
        private void button2_Click_1(object sender, EventArgs e)
        {
            // 1. Dosya Seçme İletişim Kutusunu Aç
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _secilenDosyaYolu = ofd.FileName;

                // 2. Resim yolunu TextBox'a yaz (Kontrol adınızın "textBoxResimYolu" olduğunu varsayıyorum)
                if (Controls.Find("textBoxResimYolu", true).FirstOrDefault() is TextBox tb)
                {
                    tb.Text = _secilenDosyaYolu;
                }

                // 3. Seçilen resmi PictureBox'ta önizle (Kontrol adınızın "pictureBox1" olduğunu varsayıyorum)
                try
                {
                    // Resmi belleğe yüklemek için Stream kullanıyoruz ki dosya kilitlenmesin
                    using (var stream = new FileStream(_secilenDosyaYolu, FileMode.Open, FileAccess.Read))
                    {
                        // Önceki resmi serbest bırak
                        if (pictureBox1.Image != null)
                        {
                            pictureBox1.Image.Dispose();
                            pictureBox1.Image = null;
                        }

                        // Yeni resmi yükle (kopya al, orijinali bozma)
                        Image newImage = Image.FromStream(stream);
                        pictureBox1.Image = FixOrientation(newImage);  // Orientation'ı düzelt
                    }
                    // Önizleme modunu ayarla
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    button3.Visible = true; // Kaydet butonunu göster
                    pictureBox1.Visible = true; // PictureBox'ı görünür yap
                    button5.Visible = false;
                    button2.Visible = false;
                    button6.Visible = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Resim önizlemede yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- button3_Click (Arka Plan Resmini Kaydetme) ---
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_secilenDosyaYolu) || !File.Exists(_secilenDosyaYolu))
            {
                MessageBox.Show("Lütfen önce bir resim seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kaynak dosyanın kilitli olup olmadığını kontrol et (nadir, ama güvenli)
            try
            {
                using (var testStream = new FileStream(_secilenDosyaYolu, FileMode.Open, FileAccess.Read))
                {
                    // Erişim varsa, stream'i kapat
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Seçilen resim dosyası başka bir işlem tarafından kullanılıyor. Lütfen dosyayı kapatıp tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 1. Resmi sabit bir klasöre kopyala (kalite korunur, benzersiz isimle)
            string resimKlasoru = Path.Combine(Application.StartupPath, "Resimler"); // Uygulamanın yanına "Resimler" klasörü
            if (!Directory.Exists(resimKlasoru)) Directory.CreateDirectory(resimKlasoru);

            // Benzersiz dosya adı oluştur (GUID ile çakışma önle)
            string uzanti = Path.GetExtension(_secilenDosyaYolu);
            string benzersizAd = Guid.NewGuid().ToString("N").Substring(0, 8) + uzanti; // Örn: "a1b2c3d4.jpg"
            string yeniDosyaYolu = Path.Combine(resimKlasoru, benzersizAd);

            try
            {
                File.Copy(_secilenDosyaYolu, yeniDosyaYolu, false); // Üzerine yazma yok (false), çünkü benzersiz
            }
            catch (Exception ex)
            {
                MessageBox.Show("Resim kopyalanırken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Yolu veritabanına kaydet (UPDATE ile)
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=ÜrünYönetimSistemi.accdb";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    // ArkaPlanResmi alanını güncelle
                    string query = "UPDATE IsletmeAdi SET ArkaPlanResmi = @Yol WHERE 1=1";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Yol", yeniDosyaYolu);
                        int affectedRows = cmd.ExecuteNonQuery();

                        if (affectedRows == 0)
                        {
                            // Eğer tablo boşsa (Ayarlar tablosunda satır yoksa)
                            MessageBox.Show("Ayarlar tablosunda güncelleme yapılacak satır bulunamadı. Lütfen önce İşletme Bilgilerini Kaydedin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                // 3. Form2'yi güncelle
                if (_anaForm != null)
                {
                    _anaForm.ArkaPlanGuncelle();
                }

                MessageBox.Show("Yeni arka plan resmi başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                button3.Visible = false;
                button5.Visible = true;
                button2.Visible = true;
                button6.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanına kayıt sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      
        // --- button5_Click (Arka Plan Resmini Silme) ---
        private void button5_Click(object sender, EventArgs e)
        {
            // Kullanıcıdan silme onayı al
            DialogResult result = MessageBox.Show("Arka plan resmini silmek istediğinizden emin misiniz?", "Onay Gerekli", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return; // Silmekten vazgeçildi
            }

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=ÜrünYönetimSistemi.accdb";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    // ArkaPlanResmi alanını boş bir string ile güncelle (veya Access'te NULL kullanabiliriz)
                    string query = "UPDATE IsletmeAdi SET ArkaPlanResmi = NULL WHERE 1=1";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        int affectedRows = cmd.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Arka plan resmi başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // 1. Form üzerindeki kontrolleri temizle
                            if (pictureBox1.Image != null)
                            {
                                pictureBox1.Image.Dispose();
                                pictureBox1.Image = null;
                            }
                            pictureBox1.Visible = false;
                            button3.Visible = false; // Kaydet butonunu gizle

                            // Resim yolu TextBox'ını da temizle (Adının textBoxResimYolu olduğunu varsayarsak)
                            if (Controls.Find("textBoxResimYolu", true).FirstOrDefault() is TextBox tb)
                            {
                                tb.Text = string.Empty;
                            }

                            // 2. Ana Form'u (Form2) güncelle
                            if (_anaForm != null)
                            {
                                _anaForm.ArkaPlanGuncelle();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Silinecek bir ayar kaydı bulunamadı. Lütfen önce İşletme Bilgilerini Kaydedin.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Resim silme işlemi sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // 1. Geçici Seçimi İptal Et ve Kontrolleri Temizle
            _secilenDosyaYolu = null;

            // Resim yolu TextBox'ını temizle (Adının textBoxResimYolu olduğunu varsayarsak)
            if (Controls.Find("textBoxResimYolu", true).FirstOrDefault() is TextBox tb)
            {
                tb.Text = string.Empty;
            }

            // PictureBox'ı temizle (Gerekirse eski resmi serbest bırak)
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            // Gerekli butonları ve PictureBox'ı gizle
            pictureBox1.Visible = false;
            button3.Visible = true; // Kaydet butonunu gizle
            button5.Visible = true;
            button2.Visible = true;
            VeriYukle();
            button6.Visible = false;
        }

        private void Ayarlar_Load_1(object sender, EventArgs e)
        {

        }
    }
}
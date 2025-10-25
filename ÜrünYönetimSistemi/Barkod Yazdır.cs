using System.Drawing;
using Font = System.Drawing.Font;
// iTextSharp/RawPrint kullanmıyorsanız bu usingleri kaldırabilirsiniz
// using iTextSharp.text.pdf.collection; 
// using iTextSharp.text.pdf.draw; 
// using RawPrint; 
// using iTextSharp.text.pdf; 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// Zxing.Net için gerekli using'ler eklendi:
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility; // Hata 1'in çözümü: Renderer'ı içerir


namespace ÜrünYönetimSistemi
{
    public partial class Barkod_Yazdır : Form
    {
        public Form1 frm1;
        public Form2 frm2;
        const int OriginalPictureBoxWidth = 119;
        const int OriginalPictureBoxHeight = 67;
        private int kalanSayfa = 0;
        // ** YENİ METOT: ZXing.Net ile Barkod Bitmap Oluşturma **
        private Bitmap CreateBarcodeBitmap(string barkodVerisi, int width, int height)
        {
            // Code 128 için yazar oluşturuluyor
            var writer = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Renderer = new BitmapRenderer(), // Görüntü formatını Bitmap olarak ayarlıyoruz
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    PureBarcode = true, // Sadece barkod çizgilerini oluştur (altındaki rakamlar hariç)
                    Margin = 0
                }
            };

            // Eğer barkod verisi boşsa veya null ise null döndür.
            if (string.IsNullOrWhiteSpace(barkodVerisi))
            {
                return null;
            }

            // Barkodu oluştur
            try
            {
                return writer.Write(barkodVerisi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Barkod oluşturulurken bir sorun oluştu. Barkod verisi çok uzun/kısa olabilir: " + ex.Message, "Barkod Oluşturma Hatası");
                return null;
            }
        }

        public Barkod_Yazdır()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.RowHeadersVisible = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            textBox1.KeyPress += RestrictToDigits;
            textBox10.KeyPress += RestrictToDigits;
            textBox3.KeyPress += RestrictToDecimal;
            textBox4.KeyPress += RestrictToDecimal;


            // Constructor içinde
            textBox1.Text = "111111";
            textBox2.Text = "Test01";
            textBox3.Text = "61";
            textBox4.Text = "61";
            textBox10.Text = "1";

            // EKLENEN KOD: Metin kutularının TextChanged olaylarını bağlama
            textBox1.TextChanged += textBox1_TextChanged;
            textBox2.TextChanged += textBox2_TextChanged;
            textBox3.TextChanged += textBox3_TextChanged;
            textBox4.TextChanged += textBox4_TextChanged;
        }

        private void Barkod_Yazdır_Load(object sender, EventArgs e)
        {
            Listele();

            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            // Yükseklik 200 olarak ayarlandı, bu sayede tüm etiketler rahatça sığabilir.
            pictureBox2.Width = 287;
            pictureBox2.Height = 200;


            // Form ilk açıldığında varsayılan olarak checkBox6 seçili olsun
            checkBox6.Checked = true;

            // Başlangıçta checkBox8 ve checkBox9 gizli olsun
            checkBox8.Visible = false;
            checkBox9.Visible = false;

        }

        private void RestrictToDigits(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void RestrictToDecimal(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            if (e.KeyChar == ',' && tb.Text.Contains(','))
            {
                e.Handled = true;
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void Listele()
        {
            using (OleDbConnection baglan = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
            {
                DataTable tablo = new DataTable();
                tablo.Clear();
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT Barkod_No, Ürün_Adi, Satis_Fiyati, [2SatisFiyati] FROM ÜrünGirişi", baglan);
                adapter.Fill(tablo);
                dataGridView1.DataSource = tablo;

                dataGridView1.Columns["Barkod_No"].HeaderText = "Barkod No";
                dataGridView1.Columns["Ürün_Adi"].HeaderText = "Ürün Adı";
                dataGridView1.Columns["Satis_Fiyati"].HeaderText = "Satış Fiyatı";
                dataGridView1.Columns["2SatisFiyati"].HeaderText = "İndirimli Fiyat";
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;

            DataGridViewRow clickedRow = dataGridView1.Rows[e.RowIndex];
            clickedRow.Selected = true;

            string barkodNo = clickedRow.Cells["Barkod_No"].Value?.ToString() ?? "";
            string urunAdi = clickedRow.Cells["Ürün_Adi"].Value?.ToString() ?? "";
            string satisFiyati = clickedRow.Cells["Satis_Fiyati"].Value?.ToString() ?? "";
            string indirimliFiyat = clickedRow.Cells["2SatisFiyati"].Value?.ToString() ?? "";

            textBox1.Text = barkodNo;
            textBox2.Text = urunAdi;
            textBox3.Text = satisFiyati;
            textBox4.Text = indirimliFiyat;

        }
        private void OlusturVeYazdirBarkod()
        {
            string barkodVerisi = textBox1.Text.Trim();
            string urunAdi = textBox2.Text.Trim();
            string pesinFiyat = textBox3.Text.Trim();
            string indirimliFiyat = textBox4.Text.Trim();
            bool fiyatGoster = checkBox5.Checked;

            if (string.IsNullOrEmpty(barkodVerisi))
            {
                pictureBox2.Image = null;
                return;
            }

            try
            {
                int genislik = 287;
                int yukseklik = 200;

                Bitmap sonGorsel = new Bitmap(genislik, yukseklik);

                using (Graphics g = Graphics.FromImage(sonGorsel))
                {
                    g.Clear(Color.White);

                    using Font fontUrunAdi = new Font("Arial", 9, FontStyle.Bold);
                    using Font fontFiyat = new Font("Arial", 9, FontStyle.Bold);
                    using Font fontBarkodNo = new Font("Arial", 7, FontStyle.Regular);

                    StringFormat formatCenter = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    // === ÜRÜN ADI ===
                    float urunAdiY = 3;
                    RectangleF rectUrunAdi = new RectangleF(0, urunAdiY, genislik, 20);
                    g.DrawString(urunAdi, fontUrunAdi, Brushes.Black, rectUrunAdi, formatCenter);

                    // Barkodun başlangıç Y pozisyonu, ürün adının bitişine göre ayarlandı
                    float barkodY = urunAdiY + rectUrunAdi.Height + 5;

                    // === Barkod Oluşturma: IronBarCode yerine ZXing.Net kullanıldı ===
                    Bitmap barkodImg = CreateBarcodeBitmap(barkodVerisi, 220, 50);

                    if (barkodImg == null) return; // Barkod oluşturulamazsa devam etme

                    float barkodX = (genislik - barkodImg.Width) / 2f;
                    RectangleF rectBarkodCizgileri = new RectangleF(barkodX, barkodY, barkodImg.Width, 40);
                    g.DrawImage(barkodImg, rectBarkodCizgileri);

                    // Rakamların başlangıç Y pozisyonu, barkodun bitişine göre ayarlandı
                    float rakamlarY = rectBarkodCizgileri.Bottom + 5; // Rakamlar için boşluk artırıldı

                    // === Rakamlar Barkod Altına Çiz ===
                    float karakterBirimGenisligi = barkodImg.Width / (float)barkodVerisi.Length;

                    for (int i = 0; i < barkodVerisi.Length; i++)
                    {
                        float karakterX = barkodX + (i * karakterBirimGenisligi);
                        RectangleF rectKarakter = new RectangleF(
                            karakterX,
                            rakamlarY,
                            karakterBirimGenisligi,
                            15
                        );
                        g.DrawString(
                            barkodVerisi[i].ToString(),
                            fontBarkodNo,
                            Brushes.Black,
                            rectKarakter,
                            formatCenter
                        );
                    }

                    // Fiyatların başlangıç Y pozisyonu, rakamların bitişine göre ayarlandı
                    float fiyatY = rakamlarY + 15 + 5; // Rakamlardan sonraki boşluk artırıldı

                    // === Fiyatlar ===
                    if (fiyatGoster)
                    {
                        bool indirimVar = !string.IsNullOrEmpty(indirimliFiyat) && indirimliFiyat != "0";

                        if (indirimVar)
                        {
                            RectangleF rectPesin = new RectangleF(0, fiyatY, genislik, 18);
                            g.DrawString("Peşin Fiyat: " + pesinFiyat + " TL", fontFiyat, Brushes.Black, rectPesin, formatCenter);

                            RectangleF rectIndirimli = new RectangleF(0, fiyatY + 18, genislik, 18);
                            g.DrawString("İndirimli Fiyat: " + indirimliFiyat + " TL", fontFiyat, Brushes.Black, rectIndirimli, formatCenter);
                        }
                        else
                        {
                            RectangleF rectFiyat = new RectangleF(0, fiyatY + 9, genislik, 22);
                            g.DrawString(pesinFiyat + " TL", fontFiyat, Brushes.Black, rectFiyat, formatCenter);
                        }
                    }
                }

                pictureBox2.Image = sonGorsel;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Barkod ve etiket oluşturma sırasında bir hata oluştu: " + ex.Message,
                                 "Hata",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
            }
        }

        private void OlusturVeYazdirBarkod1()
        {
            string barkodVerisi = textBox1.Text.Trim();
            string urunAdi = textBox2.Text.Trim();
            string satisFiyat = textBox3.Text.Trim();

            if (string.IsNullOrEmpty(barkodVerisi))
            {
                pictureBox2.Image = null;
                return;
            }

            try
            {
                int genislik = 287;
                int yukseklik = 200; // Etiket boyu (200, 210'dan daha dar bir etiket boyutuysa)

                Bitmap sonGorsel = new Bitmap(genislik, yukseklik);

                using (Graphics g = Graphics.FromImage(sonGorsel))
                {
                    g.Clear(Color.White);

                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    using Font fontUrunAdi = new Font("Arial", 9, FontStyle.Bold);
                    using Font fontFiyat = new Font("Arial", 9, FontStyle.Bold);
                    using Font fontBarkodNo = new Font("Arial", 7, FontStyle.Regular);
                    using Font fontTarih = new Font("Arial", 8, FontStyle.Regular);

                    StringFormat formatCenter = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    StringFormat formatRight = new StringFormat
                    {
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Center
                    };
                    StringFormat formatLeft = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center
                    };

                    // === Ürün Adı ===
                    float urunAdiY = 5;
                    RectangleF rectUrunAdi = new RectangleF(0, urunAdiY, genislik, 20);
                    g.DrawString(urunAdi, fontUrunAdi, Brushes.Black, rectUrunAdi, formatCenter);

                    // === Satış Fiyatı ===
                    float fiyatY = rectUrunAdi.Bottom + 3;
                    RectangleF rectFiyat = new RectangleF(0, fiyatY, genislik, 20);
                    g.DrawString(satisFiyat + " TL", fontFiyat, Brushes.Black, rectFiyat, formatCenter);

                    // === Barkod Oluşturma: IronBarCode yerine ZXing.Net kullanıldı ===
                    float barkodY = rectFiyat.Bottom + 10;
                    Bitmap barkodImg = CreateBarcodeBitmap(barkodVerisi, 220, 50);

                    if (barkodImg == null) return;

                    float barkodX = (genislik - barkodImg.Width) / 2f;
                    RectangleF rectBarkod = new RectangleF(barkodX, barkodY, barkodImg.Width, 45);
                    g.DrawImage(barkodImg, rectBarkod);

                    // === Barkod Numarası ===
                    float rakamlarY = rectBarkod.Bottom + 5;
                    float karakterBirimGenisligi = barkodImg.Width / (float)barkodVerisi.Length;

                    for (int i = 0; i < barkodVerisi.Length; i++)
                    {
                        float karakterX = barkodX + (i * karakterBirimGenisligi);
                        RectangleF rectKarakter = new RectangleF(
                            karakterX,
                            rakamlarY,
                            karakterBirimGenisligi,
                            15
                        );
                        g.DrawString(barkodVerisi[i].ToString(),
                                     fontBarkodNo, Brushes.Black,
                                     rectKarakter, formatCenter);
                    }

                    // --- Alt Bilgiler: Yerli Üretim Görseli ve Tarih ---
                    float altBilgiY = rakamlarY + 15;

                    try
                    {
                        // Yerli Üretim görseli varsa çiz
                        Image yerliUretimImg = Properties.Resources.YerliUretim;
                        int resimGenislik = 65;
                        int resimYukseklik = (int)((float)resimGenislik / yerliUretimImg.Width * yerliUretimImg.Height);
                        float resimY = altBilgiY;

                        g.DrawImage(
                            yerliUretimImg,
                            new RectangleF(5, resimY, resimGenislik, resimYukseklik)
                        );

                        string tarihYazisi = "Fiyat Güncelleme Tarihi: " + DateTime.Now.ToShortDateString();
                        float textY = resimY;
                        RectangleF rectTarih = new RectangleF(0, textY, genislik - 5, 24);
                        g.DrawString(tarihYazisi, fontTarih, Brushes.Black, rectTarih, formatRight);
                    }
                    catch { } // Properties.Resources.YerliUretim yoksa hata vermez
                }

                pictureBox2.Image = sonGorsel;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Barkod ve etiket oluşturma sırasında bir hata oluştu: " + ex.Message,
                                 "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OlusturVeYazdirBarkod2()
        {
            string barkodVerisi = textBox1.Text.Trim();
            string urunAdi = textBox2.Text.Trim();
            string satisFiyat = textBox3.Text.Trim();

            if (string.IsNullOrEmpty(barkodVerisi))
            {
                pictureBox2.Image = null;
                return;
            }

            try
            {
                int genislik = 287;
                int yukseklik = 210; // Etiket boyu

                Bitmap sonGorsel = new Bitmap(genislik, yukseklik);

                using (Graphics g = Graphics.FromImage(sonGorsel))
                {
                    g.Clear(Color.White);

                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    using Font fontUrunAdi = new Font("Arial", 9, FontStyle.Bold);
                    using Font fontFiyat = new Font("Arial", 9, FontStyle.Bold);
                    using Font fontBarkodNo = new Font("Arial", 7, FontStyle.Regular);
                    using Font fontTarih = new Font("Arial", 8, FontStyle.Regular);

                    StringFormat formatCenter = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    StringFormat formatRight = new StringFormat
                    {
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Center
                    };
                    StringFormat formatLeft = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center
                    };

                    // === Ürün Adı ===
                    float urunAdiY = 5;
                    RectangleF rectUrunAdi = new RectangleF(0, urunAdiY, genislik, 20);
                    g.DrawString(urunAdi, fontUrunAdi, Brushes.Black, rectUrunAdi, formatCenter);

                    // === Satış Fiyatı ===
                    float fiyatY = rectUrunAdi.Bottom + 3;
                    RectangleF rectFiyat = new RectangleF(0, fiyatY, genislik, 20);
                    g.DrawString(satisFiyat + " TL", fontFiyat, Brushes.Black, rectFiyat, formatCenter);

                    // === Barkod Oluşturma: IronBarCode yerine ZXing.Net kullanıldı ===
                    float barkodY = rectFiyat.Bottom + 10;
                    Bitmap barkodImg = CreateBarcodeBitmap(barkodVerisi, 220, 50);

                    if (barkodImg == null) return;

                    float barkodX = (genislik - barkodImg.Width) / 2f;
                    RectangleF rectBarkod = new RectangleF(barkodX, barkodY, barkodImg.Width, 45);
                    g.DrawImage(barkodImg, rectBarkod);

                    // === Barkod Numarası ===
                    float rakamlarY = rectBarkod.Bottom + 5;
                    float karakterBirimGenisligi = barkodImg.Width / (float)barkodVerisi.Length;

                    // Hata 2'nin çözümü: Döngü başlangıcı düzeltildi (int i = 0)
                    for (int i = 0; i < barkodVerisi.Length; i++)
                    {
                        float karakterX = barkodX + (i * karakterBirimGenisligi);
                        RectangleF rectKarakter = new RectangleF(
                            karakterX,
                            rakamlarY,
                            karakterBirimGenisligi,
                            15
                        );
                        g.DrawString(barkodVerisi[i].ToString(),
                                     fontBarkodNo, Brushes.Black,
                                     rectKarakter, formatCenter);
                    }

                    // --- ALT BİLGİLER BÖLÜMÜNÜN YENİ HALİ ---
                    float altBilgiY = rakamlarY + 15;

                    // Üretim Yeri metnini sola ekle
                    string uretimYeri = "Üretim Yeri : İTHAL";
                    RectangleF rectUretimYeri = new RectangleF(5, altBilgiY, genislik / 2, 20);
                    g.DrawString(uretimYeri, fontTarih, Brushes.Black, rectUretimYeri, formatLeft);

                    // Fiyat Güncelleme Tarihi metnini sola, Üretim Yeri'nin altına hizala
                    string tarihYazisi = "Fiyat Güncelleme Tarihi: " + DateTime.Now.ToShortDateString();
                    RectangleF rectTarih = new RectangleF(5, altBilgiY + 15, genislik - 10, 20);
                    g.DrawString(tarihYazisi, fontTarih, Brushes.Black, rectTarih, formatLeft);
                }

                pictureBox2.Height = 200; // PictureBox yüksekliğini görüntü boyutuna uydur
                pictureBox2.Image = sonGorsel;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Barkod ve etiket oluşturma sırasında bir hata oluştu: " + ex.Message,
                                 "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
                OlusturVeYazdirBarkod();
            else if (checkBox8.Checked)
                OlusturVeYazdirBarkod1();
            else if (checkBox9.Checked)
                OlusturVeYazdirBarkod2();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                if (checkBox6.Checked)
                    OlusturVeYazdirBarkod();
                else if (checkBox8.Checked)
                    OlusturVeYazdirBarkod1();
                else if (checkBox9.Checked)
                    OlusturVeYazdirBarkod2();
            }
            else
            {
                pictureBox2.Image = null; // Barkod silinsin
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
                OlusturVeYazdirBarkod();
            else if (checkBox8.Checked)
                OlusturVeYazdirBarkod1();
            else if (checkBox9.Checked)
                OlusturVeYazdirBarkod2();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
                OlusturVeYazdirBarkod();
            else if (checkBox8.Checked)
                OlusturVeYazdirBarkod1();
            else if (checkBox9.Checked)
                OlusturVeYazdirBarkod2();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
                OlusturVeYazdirBarkod();
            else if (checkBox8.Checked)
                OlusturVeYazdirBarkod1();
            else if (checkBox9.Checked)
                OlusturVeYazdirBarkod2();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
                OlusturVeYazdirBarkod(); // ✅ Checkbox durumu değişince hemen yeniden çiz
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                // checkBox6 seçiliyse diğerlerini kaldır ve gizle
                checkBox7.Checked = false;
                checkBox8.Checked = false;
                checkBox9.Checked = false;
                checkBox8.Visible = false;
                checkBox9.Visible = false;
                // checkBox7'den geçince checkBox5'i göster
                checkBox5.Visible = true;
                OlusturVeYazdirBarkod();
            }
            else
            {
                // Hiçbiri seçili değilse checkBox6'yı otomatik seç
                if (!checkBox7.Checked && !checkBox8.Checked && !checkBox9.Checked)
                {
                    checkBox6.Checked = true;
                }
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                // checkBox7 seçiliyse diğerlerini kaldır ve 8 ile 9'u göster
                checkBox6.Checked = false;
                checkBox8.Visible = true;
                checkBox9.Visible = true;
                checkBox8.Checked = true; // checkBox7 seçildiğinde otomatik olarak checkBox8'i seç.
                // checkBox7 seçilince checkBox5'i gizle
                checkBox5.Visible = false;
            }
            else
            {
                // checkBox7 kaldırılırsa 8 ve 9'u gizle ve checkBox6'yı seç
                checkBox8.Visible = false;
                checkBox9.Visible = false;
                checkBox8.Checked = false;
                checkBox9.Checked = false;
                checkBox6.Checked = true;
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                // checkBox8 seçiliyse diğerlerini kaldır
                checkBox6.Checked = false;
                checkBox9.Checked = false;
                OlusturVeYazdirBarkod1();
            }
            else
            {
                // checkBox8'in seçimini kaldırmayı engelle
                // (Mevcut mantığı bozma)
                if (checkBox7.Checked && !checkBox9.Checked)
                {
                    checkBox8.Checked = true;
                }
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                // checkBox9 seçiliyse diğerlerini kaldır
                checkBox6.Checked = false;
                checkBox8.Checked = false;
                OlusturVeYazdirBarkod2();
            }
            else
            {
                // checkBox9'un seçimini kaldırmayı engelle
                // (Mevcut mantığı bozma)
                if (checkBox7.Checked && !checkBox8.Checked)
                {
                    checkBox9.Checked = true;
                }
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image == null)
            {
                MessageBox.Show("Yazdırmak için öncelikle barkod görselini oluşturun.", "Uyarı",
                                 MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kaç kopya alınacağını oku
            if (!int.TryParse(textBox10.Text, out kalanSayfa) || kalanSayfa <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir çıktı adedi girin (1 veya daha büyük bir sayı).", "Uyarı",
                                 MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage_BarkodCoklu);

            try
            {
                // === Varsayılan Yazıcı Ayarları ===
                int genislik = pictureBox2.Image.Width;
                int yukseklik = pictureBox2.Image.Height;

                pd.DefaultPageSettings.PaperSize = new PaperSize("CustomBarcode", genislik, yukseklik);
                pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

                // === Doğrudan Yazdır ===
                pd.Print();
            }
            catch (InvalidPrinterException)
            {
                MessageBox.Show("Yazıcıya erişilemiyor veya yazıcı ayarları hatalı. Lütfen varsayılan yazıcınızı kontrol edin.", "Yazıcı Hatası",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Barkod yazdırma sırasında bir hata oluştu: " + ex.Message, "Hata",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // === Her sayfada 1 barkod çizecek metod ===
        private void pd_PrintPage_BarkodCoklu(object sender, PrintPageEventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                e.Graphics.DrawImage(pictureBox2.Image, 0, 0, pictureBox2.Image.Width, pictureBox2.Image.Height);
            }

            // Kaç sayfa kaldıysa ona göre devam et
            kalanSayfa--;

            if (kalanSayfa > 0)
                e.HasMorePages = true; // Yeni sayfa oluştur
            else
                e.HasMorePages = false; // Yazdırma bitti
        }


        // Barkod görselini yazdırma olay işleyicisi (Öncekiyle aynı, sade ve doğru)
        private void pd_PrintPage_Barcode(object sender, PrintPageEventArgs ev)
        {
            // PictureBox2'deki görseli alın
            Image etiketGorseli = pictureBox2.Image;

            // Görseli kağıdın en üst sol köşesinden (0,0) başlayarak çizdirin
            float yazdirmaX = 0;
            float yazdirmaY = 0;

            // Görseli orijinal boyutlarında çiz
            ev.Graphics.DrawImage(
                etiketGorseli,
                yazdirmaX,
                yazdirmaY,
                etiketGorseli.Width,
                etiketGorseli.Height
            );

            // Başka sayfa kalmadığını belirtin (Tek bir etiket basılıyor)
            ev.HasMorePages = false;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            string aramaMetni = textBox6.Text.Trim();
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
    }
}
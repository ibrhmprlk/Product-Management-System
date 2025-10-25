using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // Form bileşenleri için (TextBox, Timer, MessageBox)
using System.Xml; // XML verisi çekmek için
using System.Net; // Web işlemleri için (XmlDocument.Load)

namespace ÜrünYönetimSistemi
{
    public partial class DövizKurları : Form
    {
        // TCMB'den çekilecek döviz kurlarının kodları listesi (Sıra, metin kutularının sırasıyla eşleşir)
        private readonly List<string> DovizKodlari = new List<string>
        {
            "USD", // Alış: textBox1, Satış: textBox11
            "EUR", // Alış: textBox2, Satış: textBox12
            "GBP", // Alış: textBox3, Satış: textBox13
            "DKK", // Alış: textBox4, Satış: textBox14
            "CAD", // Alış: textBox5, Satış: textBox15
            "SEK", // Alış: textBox6, Satış: textBox16
            "CHF", // Alış: textBox7, Satış: textBox17
            "NOK", // Alış: textBox8, Satış: textBox18
            "JPY", // Alış: textBox9, Satış: textBox19
            "AUD"  // Alış: textBox10, Satış: textBox20
        };

        public DövizKurları()
        {
            InitializeComponent();
        }

        private void DövizKurları_Load(object sender, EventArgs e)
        {
            // Yeni Ekleme: Kur ve tarih verilerinin kullanıcı tarafından değiştirilmesini engellemek için
            // tüm ilgili TextBox'ları ReadOnly yapıyoruz.
            for (int i = 1; i <= 21; i++) // textBox1'den textBox21'e kadar döngü
            {
                TextBox currentTextBox = (TextBox)this.Controls.Find($"textBox{i}", true).FirstOrDefault();
                if (currentTextBox != null)
                {
                    currentTextBox.ReadOnly = true;
                }
            }

            // textBox21'e güncel tarih ve saati yazdır.
            textBox21.Text = DateTime.Now.ToString("dd.MM.yyyy");

            // Form açılır açılmaz ilk kurları çekmek için Tick olayını manuel olarak tetikle.
            timerTCMBGuncelle_Tick(null, null);
        }

        // Formdaki timerTCMBGuncelle bileşeninin Tick olayına bu metot bağlanmalıdır.
        private void timerTCMBGuncelle_Tick(object sender, EventArgs e)
        {
            try
            {
                // 1. Timer'ı Durdur: Timer bileşeninin adı 'timerTCMBGuncelle' olarak düzeltildi.
                timer.Stop();
            }
            catch (Exception)
            {
                // Timer başlatılmadan durdurulmaya çalışılırsa (Load anında), hatayı yoksay.
            }

            try
            {
                // TCMB'nin güncel kur XML dosyasının adresi
                string bugunkuKurAdresi = "https://www.tcmb.gov.tr/kurlar/today.xml";

                // XmlDocument nesnesi ile XML dosyasını internetten yüklüyoruz
                XmlDocument xmlVerisi = new XmlDocument();
                xmlVerisi.Load(bugunkuKurAdresi);

                // Döviz kodları listesinde dönerek her bir kuru çekelim
                for (int i = 0; i < DovizKodlari.Count; i++)
                {
                    string kod = DovizKodlari[i];

                    // XML'de ilgili döviz koduna sahip Node'u seçer
                    XmlNode node = xmlVerisi.SelectSingleNode($"Tarih_Date/Currency[@CurrencyCode='{kod}']");

                    if (node != null)
                    {
                        string alis = node.SelectSingleNode("ForexBuying")?.InnerText;
                        string satis = node.SelectSingleNode("ForexSelling")?.InnerText;

                        // --- Alış Fiyatını Doldurma (textBox1'den textBox10'a) ---
                        TextBox txtAlis = (TextBox)this.Controls.Find($"textBox{i + 1}", true).FirstOrDefault();
                        if (txtAlis != null && !string.IsNullOrEmpty(alis))
                        {
                            txtAlis.Text = alis.Replace(".", ",");
                        }

                        // --- Satış Fiyatını Doldurma (textBox11'den textBox20'ye) ---
                        TextBox txtSatis = (TextBox)this.Controls.Find($"textBox{i + 11}", true).FirstOrDefault();
                        if (txtSatis != null && !string.IsNullOrEmpty(satis))
                        {
                            txtSatis.Text = satis.Replace(".", ",");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kurlar çekilirken bir hata oluştu. İnternet bağlantınızı kontrol edin veya TCMB servisi geçici olarak kapalı olabilir.\nHata: " + ex.Message, "Kur Çekme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 2. Timer'ı Başlat
                timer.Start();
            }
        }
    }
}
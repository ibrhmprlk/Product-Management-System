using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ÜrünYönetimSistemi
{
    public partial class GelirGiderGrafik : Form
    {
        // Yıllar için ComboBox'ı kodla oluşturuyoruz
        private ComboBox comboBoxYil;
        // backgroundWorker1 değişkeni artık designer tarafından oluşturuluyor.
        // Bu yüzden burada manuel olarak tanımlamaya gerek yoktur.

        public GelirGiderGrafik()
        {
            InitializeComponent();
            this.Load += GelirGiderGrafik_Load;

            // Kodla ComboBox'ı oluştur ve forma ekle
            comboBoxYil = new ComboBox();
            comboBoxYil.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxYil.Location = new Point(10, 10);
            comboBoxYil.Size = new Size(120, 25);
            comboBoxYil.SelectedIndexChanged += comboBoxYil_SelectedIndexChanged;
            this.Controls.Add(comboBoxYil);

            // backgroundWorker1 artık designer tarafından oluşturulduğu için
            // burada manuel olarak oluşturmaya gerek yoktur. Sadece olayları tanımlıyoruz.
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }

        private void GelirGiderGrafik_Load(object sender, EventArgs e)
        {
        
            // Yılları ComboBox'a yükle
            YillariComboBoxaDoldur();

            // Varsayılan olarak en son yılı seç
            if (comboBoxYil.Items.Count > 0)
            {
                comboBoxYil.SelectedIndex = 0;
            }
        }

        private void YillariComboBoxaDoldur()
        {
            try
            {
                using (OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    string sorgu = @"
                        SELECT DISTINCT YEAR(Tarih) AS Yil FROM UrunSatis WHERE Tarih IS NOT NULL
                        UNION
                        SELECT DISTINCT YEAR(Tarih) AS Yil FROM ÜrünGirişi WHERE Tarih IS NOT NULL
                        UNION
                        SELECT DISTINCT YEAR([Tarih/Saat]) AS Yil FROM BorcEkleme WHERE [Tarih/Saat] IS NOT NULL
                        UNION
                        SELECT DISTINCT YEAR([Tarih/Saat]) AS Yil FROM Tahsilat WHERE [Tarih/Saat] IS NOT NULL
                        ORDER BY Yil DESC;
                    ";

                    OleDbDataAdapter da = new OleDbDataAdapter(sorgu, baglanti);
                    DataTable dt = new DataTable();
                    baglanti.Open();
                    da.Fill(dt);

                    comboBoxYil.Items.Clear();
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Yil"] != DBNull.Value)
                        {
                            comboBoxYil.Items.Add(row["Yil"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Yıllar yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBoxYil_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kullanıcı bir yıl seçtiğinde arka plan işlemini başlat
            if (backgroundWorker1.IsBusy == false)
            {
                this.Cursor = Cursors.WaitCursor; // Bekleme imleci göster
                int secilenYil = Convert.ToInt32(comboBoxYil.SelectedItem);
                backgroundWorker1.RunWorkerAsync(secilenYil);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int secilenYil = (int)e.Argument;
            Dictionary<string, Dictionary<int, decimal>> sonuclar = new Dictionary<string, Dictionary<int, decimal>>();

            sonuclar["Gelir"] = new Dictionary<int, decimal>();
            sonuclar["Gider"] = new Dictionary<int, decimal>();

            // Ayları sıfırla
            for (int i = 1; i <= 12; i++)
            {
                sonuclar["Gelir"].Add(i, 0);
                sonuclar["Gider"].Add(i, 0);
            }

            try
            {
                using (OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\ÜrünYönetimSistemi.accdb"))
                {
                    // Tüm gelir ve gider verilerini çeken sorgu
                    string sorgu = @"
                        SELECT MONTH(Tarih) AS Ay, SUM(ToplamTutar) AS Tutar, 'Gelir' AS Tur FROM UrunSatis WHERE YEAR(Tarih) = @Yil GROUP BY MONTH(Tarih)
                        UNION ALL
                        SELECT MONTH(Tarih) AS Ay, SUM(CCur(Replace(Alis_Fiyati, ',', '.')) * CCur(Replace(Miktar, ',', '.'))) AS Tutar, 'Gider' AS Tur FROM ÜrünGirişi WHERE YEAR(Tarih) = @Yil GROUP BY MONTH(Tarih)
                        UNION ALL
                        SELECT MONTH(Tarih) AS Ay, SUM(ToplamTutar) AS Tutar, 'Gelir' AS Tur FROM MusteriIade WHERE YEAR(Tarih) = @Yil GROUP BY MONTH(Tarih)
                        UNION ALL
                        SELECT MONTH(Tarih) AS Ay, SUM(ToplamTutar) AS Tutar, 'Gider' AS Tur FROM UrunIade WHERE YEAR(Tarih) = @Yil GROUP BY MONTH(Tarih)
                        UNION ALL
                        SELECT MONTH([Tarih/Saat]) AS Ay, SUM(EklenenTutar) AS Tutar, 'Gider' AS Tur FROM BorcEkleme WHERE YEAR([Tarih/Saat]) = @Yil GROUP BY MONTH([Tarih/Saat])
                        UNION ALL
                        SELECT MONTH([Tarih/Saat]) AS Ay, SUM(OdenenTutar) AS Tutar, 'Gelir' AS Tur FROM BorcOdeme WHERE YEAR([Tarih/Saat]) = @Yil GROUP BY MONTH([Tarih/Saat])
                        UNION ALL
                        SELECT MONTH([Tarih/Saat]) AS Ay, SUM(EklenenTutar) AS Tutar, 'Gider' AS Tur FROM VeresiyeEkle WHERE YEAR([Tarih/Saat]) = @Yil GROUP BY MONTH([Tarih/Saat])
                        UNION ALL
                        SELECT MONTH([Tarih/Saat]) AS Ay, SUM(OdenenTutar) AS Tutar, 'Gelir' AS Tur FROM Tahsilat WHERE YEAR([Tarih/Saat]) = @Yil GROUP BY MONTH([Tarih/Saat])
                    ";

                    OleDbCommand komut = new OleDbCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@Yil", secilenYil);

                    baglanti.Open();
                    OleDbDataReader reader = komut.ExecuteReader();

                    while (reader.Read())
                    {
                        string tur = reader["Tur"].ToString();
                        int ay = Convert.ToInt32(reader["Ay"]);
                        decimal tutar = Convert.ToDecimal(reader["Tutar"]);

                        if (sonuclar.ContainsKey(tur))
                        {
                            sonuclar[tur][ay] += tutar;
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                // Hata oluşursa, bunu sonuç olarak döndür
                e.Result = ex;
                return;
            }
            // Sonuçları DoWorkEventArgs.Result'a ata
            e.Result = sonuclar;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = Cursors.Default; // İmleci normale çevir

            // Hata kontrolü
            if (e.Result is Exception hata)
            {
                MessageBox.Show("Veri çekilirken bir hata oluştu: " + hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Grafiği oluştur ve doldur
            if (e.Result is Dictionary<string, Dictionary<int, decimal>> sonuclar)
            {
                GrafikCiz(sonuclar);
            }
        }

        private void GrafikCiz(Dictionary<string, Dictionary<int, decimal>> sonuclar)
        {
            // Mevcut chart kontrolünü bul veya yenisini oluştur
            Chart chart1 = this.Controls.Find("chart1", false).FirstOrDefault() as Chart;
            if (chart1 == null)
            {
                chart1 = new Chart();
                chart1.Name = "chart1"; // Adını veriyoruz ki sonra bulabilelim
                chart1.Dock = DockStyle.Fill;
                this.Controls.Add(chart1);
            }

            // Grafiği temizle
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Titles.Clear();
            chart1.Legends.Clear();

            // ... (Grafik Serilerini ve diğer ayarları oluşturma)
            ChartArea ca = new ChartArea("ca1");
            chart1.ChartAreas.Add(ca);

            Series gelirSerisi = new Series("Gelir");
            gelirSerisi.ChartType = SeriesChartType.Line;
            gelirSerisi.Color = Color.Green;
            gelirSerisi.BorderWidth = 3;
            gelirSerisi.IsValueShownAsLabel = true;
            gelirSerisi.MarkerStyle = MarkerStyle.Square; // Marker kare olarak ayarlandı

            Series giderSerisi = new Series("Gider");
            giderSerisi.ChartType = SeriesChartType.Line;
            giderSerisi.Color = Color.Red;
            giderSerisi.BorderWidth = 3;
            giderSerisi.IsValueShownAsLabel = true;
            giderSerisi.MarkerStyle = MarkerStyle.Square; // Marker kare olarak ayarlandı

            string[] aylar = { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };

            // Verileri grafiğe ekle
            for (int i = 0; i < 12; i++)
            {
                gelirSerisi.Points.AddXY(aylar[i], sonuclar["Gelir"][i + 1]);
                giderSerisi.Points.AddXY(aylar[i], sonuclar["Gider"][i + 1]);
            }

            // Serileri Chart'a ekle
            chart1.Series.Add(gelirSerisi);
            chart1.Series.Add(giderSerisi);

            // Başlık ve eksenleri ekle
            int secilenYil = Convert.ToInt32(comboBoxYil.SelectedItem);
            Title mainTitle = new Title($"{secilenYil} Yılı Gelir-Gider Grafiği");
            mainTitle.Font = new Font("Arial", 12, FontStyle.Bold);
            chart1.Titles.Add(mainTitle);
            chart1.ChartAreas[0].AxisX.Title = "Aylar";
            chart1.ChartAreas[0].AxisY.Title = "Tutar (₺)";
            chart1.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
            chart1.ChartAreas[0].AxisY2.Title = "Tutar (₺)";

            // Legend ekleme

            Legend legend = new Legend("GelirGiderLegend");
            chart1.Legends.Add(legend);
            chart1.Legends[0].Docking = Docking.Top;
            chart1.Legends[0].Alignment = StringAlignment.Center;
            chart1.Legends[0].BorderWidth = 5;
            // Yazı tipini büyüt
            chart1.Legends[0].Font = new Font("Arial", 10, FontStyle.Bold);

            // Kenarlık kalınlığı (legend kutusu etrafındaki çizgi)
          
        

        }
    }
}

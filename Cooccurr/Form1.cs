using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Cooccurr
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<Bitmap> imagearxeio = new List<Bitmap>();
        public ImageData[] images ;
        Cooccurr comatrix = new Cooccurr();
        double[,] CoOCCarrate;
        
        List<zeugoi> zeugos = new List<zeugoi>();


        public struct ImageData
        {
            public int ID;
            public Bitmap Image;

            public int ImageWidth;
            public int ImageHeight;







            public double Homogenity;
            public double Contrast;
            public double max;
            public double Disimilarity;
            public double Correlation;
            public double thesix;
            public double thesiy;
            public double Energy;
            
            public double Entropia;
            
            


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        

        private void button3_Click(object sender, EventArgs e)
        {
            zeugos.Clear();
            listBox1.Items.Clear();
            //we define the size of images[]
            images = new ImageData[imagearxeio.Count];
            int tag=0;
            double temp=1000;
            //we extract all the features for every image
            for (int i = 0; i < imagearxeio.Count; i++)
            {
                
                CoOCCarrate = comatrix.Apply(imagearxeio[i], 1, 1,trackBar1.Value);
                comatrix.CalcElements(CoOCCarrate,trackBar1.Value);

                //we store the data which returns function comatrix in images[]
                images[i].ID = i;
                images[i].Entropia = comatrix.Entropy;
                images[i].Energy = comatrix.Energy;
                images[i].Contrast = comatrix.Contrast;
                images[i].Homogenity = comatrix.Homogenity;
                images[i].max = comatrix.max;
                images[i].Disimilarity = comatrix.Disimilarity;
                images[i].Correlation = comatrix.Correlation;
            }

            //emfanisei xaraktiristikon
            label1.Text = images[0].Entropia.ToString("G5");
            label2.Text = images[0].Energy.ToString("G5");
            label3.Text =  images[0].Contrast.ToString("G5");
            label4.Text =  images[0].Homogenity.ToString("G5");
            label5.Text =  images[0].max.ToString("G5");

            label7.Text =  images[0].Disimilarity.ToString("G5");
            label8.Text = images[0].Correlation.ToString("G5");
            
            for (int i = 1; i < imagearxeio.Count; i++)
            {
                double dist = Euclidean(images[0], images[i]);

                zeugos.Add(new zeugoi(i, dist));
                
                if(dist<temp)
                {
                    temp = dist;
                    tag=i;
                    
                }
            }
            zeugos.Sort(delegate(zeugoi z1, zeugoi z2)
            { return z1.zeugo.CompareTo(z2.zeugo); });
            zeugos.ForEach(delegate(zeugoi z)
            {
                listBox1.Items.Add(string.Format("{0} {1} {2}", z.sec,"   ", z.zeugo));
            });
            pictureBox2.Image = imagearxeio[tag];

            comboBox1.Enabled = true;

            
        }

        private void findThisToolStripMenuItem_Click(object sender, EventArgs e)
        {

                imageFolderToolStripMenuItem.Enabled = true;
               comboBox1.Enabled = false;
                imagearxeio.Clear();
                comboBox1.Items.Clear();
                listBox1.Items.Clear();
                //button3.Enabled = false;
                openFileDialog1.Title = "Open image for process";
                openFileDialog1.Filter = "Images (*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|" +
                 "All files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {


                    try
                    {
                        string ImageFileName = openFileDialog1.FileName;

                        imagearxeio.Add(new Bitmap(ImageFileName));
                        pictureBox1.Image = imagearxeio[0];
                        comboBox1.Items.Add(0);
                        double athroisma = 0;
                        if (checkBox1.Checked)
                        {
                            //to orio pou mas deixnei poses fotinotites tha exoume to pairnoume apo trackbar
                            CoOCCarrate = comatrix.Apply(imagearxeio[0], 1, 1, trackBar1.Value);
                            
                            comatrix.CalcElements(CoOCCarrate, trackBar1.Value);
                            pictureBox3.Image = imagearxeio[0];
                            //
                            label1.Text = comatrix.Entropy.ToString("G5");
                            label2.Text =  comatrix.Energy.ToString("G5");
                            label3.Text =  comatrix.Contrast.ToString("G5");
                            label4.Text = comatrix.Homogenity.ToString("G5");
                            label5.Text =  comatrix.max.ToString("G5");

                            label7.Text = comatrix.Disimilarity.ToString("G5");
                            label8.Text = comatrix.Correlation.ToString("G5");


                            //gia na ta emfanisoume ston pinaka akolouthoume tin parakato diadikasia
                            DataTable dt = new DataTable();
                            DataView myview = new DataView(dt);//Το αντικείμενο DataView που περιέχει το DataTable σου
                            DataRow myRow;
                            int apoxroseis = (int)Math.Pow(2, trackBar1.Value);


                            dt.Clear();

                            dt.Columns.Add("--");
                            for (int i = 0; i < apoxroseis; i++)
                            {
                                dt.Columns.Add(i.ToString());
                            }




                            for (int i = 0; i < apoxroseis; i++)
                            {
                                myRow = dt.NewRow();
                                myRow[0] = i.ToString();
                                for (int j = 1; j < apoxroseis + 1; j++)
                                {
                                    myRow[j] = CoOCCarrate[i, j - 1].ToString("G2");

                                }

                                dt.Rows.Add(myRow);

                            }


                            dataGridView1.DataSource = myview;

                            dataGridView1.ColumnHeadersVisible = true;



                            foreach (DataGridViewColumn i in
                             dataGridView1.Columns)
                            {
                                i.SortMode = DataGridViewColumnSortMode.NotSortable;
                                i.Width = 50;
                            }

                            //label5.Visible = false;
                            //this.Cursor = Cursors.Default;*/
                        }
                    }
                    catch
                    {

                    }

               
                }
        }

        private void imageFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {

            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        imagearxeio.RemoveRange(1, imagearxeio.Count - 1);
                        
                        DirectoryInfo di = new DirectoryInfo(folderBrowserDialog1.SelectedPath); // give path

                        FileInfo[] finfos = di.GetFiles("*.jpg", SearchOption.TopDirectoryOnly);
                        button3.Enabled = true;
                        comboBox1.Items.Clear();
                        for (int i = 0; i < finfos.Length; i++)
                        {
                            imagearxeio.Add(((Bitmap)Image.FromFile(finfos[i].FullName)));
                            comboBox1.Items.Add(i+1);
                            
                        }


                    }
                    catch
                    {


                    }
                }

                
                
                
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label12.Text = Math.Pow(2, trackBar1.Value).ToString();
            
        }
        int tag;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (checkBox1.Checked)
            {
                
                tag =Convert.ToInt16(comboBox1.Text);
                pictureBox3.Image = imagearxeio[tag];
                //to orio pou mas deixnei poses fotinotites tha exoume to pairnoume apo trackbar
                CoOCCarrate = comatrix.Apply(imagearxeio[tag], 1, 1, trackBar1.Value);
                comatrix.CalcElements(CoOCCarrate, trackBar1.Value);

                //
                label1.Text =  comatrix.Entropy.ToString("G5");
                label2.Text = comatrix.Energy.ToString("G5");
                label3.Text =  comatrix.Contrast.ToString("G5");
                label4.Text =  comatrix.Homogenity.ToString("G5");
                label5.Text =  comatrix.max.ToString("G5");

                label7.Text =  comatrix.Disimilarity.ToString("G5");
                label8.Text =  comatrix.Correlation.ToString("G5");
                

                //gia na ta emfanisoume ston pinaka akolouthoume tin parakato diadikasia
                DataTable dt = new DataTable();
                DataView myview = new DataView(dt);//Το αντικείμενο DataView που περιέχει το DataTable σου
                DataRow myRow;
                int apoxroseis = (int)Math.Pow(2, trackBar1.Value);


                dt.Clear();

                dt.Columns.Add("--");
                for (int i = 0; i < apoxroseis; i++)
                {
                    dt.Columns.Add(i.ToString());
                }




                for (int i = 0; i < apoxroseis; i++)
                {
                    myRow = dt.NewRow();
                    myRow[0] = i.ToString();
                    for (int j = 1; j < apoxroseis + 1; j++)
                    {
                        myRow[j] = CoOCCarrate[i, j - 1].ToString("G2");

                    }

                    dt.Rows.Add(myRow);

                }


                dataGridView1.DataSource = myview;

                dataGridView1.ColumnHeadersVisible = true;



                foreach (DataGridViewColumn i in
                 dataGridView1.Columns)
                {
                    i.SortMode = DataGridViewColumnSortMode.NotSortable;
                    i.Width = 50;
                }
                
                //label5.Visible = false;
                //this.Cursor = Cursors.Default;*/
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 about = new Form2();
            about.Show();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private  double Euclidean(ImageData dataf, ImageData data)
        {
            double homo, cont, corr, disi, ene, ent, max;
            double meanf = (dataf.max + dataf.Homogenity + dataf.Correlation + dataf.Disimilarity + dataf.Entropia + dataf.Energy + dataf.Contrast) / 7;
            double mean = (data.max + data.Homogenity + data.Correlation + data.Disimilarity + data.Entropia + data.Energy + data.Contrast) / 7;
            //tupikh apoklish
            double[] s = new double[7];

            s[0] = Math.Sqrt(0.5 * (Math.Pow((dataf.Homogenity - meanf), 2) + Math.Pow((data.Homogenity - mean), 2)));
            s[1] = Math.Sqrt(0.5 * (Math.Pow((dataf.Contrast - meanf), 2) + Math.Pow((data.Contrast - mean), 2)));
            s[2] = Math.Sqrt(0.5 * (Math.Pow((dataf.Correlation - meanf), 2) + Math.Pow((data.Correlation - mean), 2)));
            s[3] = Math.Sqrt(0.5 * (Math.Pow((dataf.Disimilarity - meanf), 2) + Math.Pow((data.Disimilarity - mean), 2)));
            s[4] = Math.Sqrt(0.5 * (Math.Pow((dataf.Energy - meanf), 2) + Math.Pow((data.Energy - mean), 2)));
            s[5] = Math.Sqrt(0.5 * (Math.Pow((dataf.Entropia - meanf), 2) + Math.Pow((data.Entropia - mean), 2)));
            s[6] = Math.Sqrt(0.5 * (Math.Pow((dataf.max - meanf), 2) + Math.Pow((data.max - mean), 2)));

            //apostash

            if (checkBox2.Checked)
            
                homo = Math.Pow((dataf.Homogenity - data.Homogenity) / s[0], 2);
            else
                homo = 0;

            if (checkBox3.Checked)

                cont = Math.Pow((dataf.Contrast - data.Contrast) / s[1], 2);
            else
                cont = 0;

            if (checkBox4.Checked)

                corr = Math.Pow((dataf.Correlation - data.Correlation) / s[2], 2);
            else
                corr = 0;
            if (checkBox5.Checked)

                disi = Math.Pow((dataf.Disimilarity - data.Disimilarity) / s[3], 2);
            else
                disi = 0;
            if (checkBox6.Checked)

                ene = Math.Pow((dataf.Energy - data.Energy) / s[4], 2);
            else
                ene = 0;
            if (checkBox7.Checked)

                ent = Math.Pow((dataf.Entropia - data.Entropia) / s[5], 2);
            else
                ent = 0;
            if (checkBox8.Checked)

                max = Math.Pow((dataf.max - data.max) / s[6], 2);
            else
                max = 0;

            return Math.Sqrt(homo+ cont + corr + disi + ene + ent + max);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

    public class zeugoi
    {
        
        public int sec;
        public double zeugo;
        public zeugoi(int sec, double zeugo)
        {
            
            this.sec = sec;
            this.zeugo = zeugo;
        }
    }
}

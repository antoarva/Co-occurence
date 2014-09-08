using System;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Cooccurr
{


    /// <summary>
    /// Correlation filter
    /// </summary>
    public class Cooccurr 
    {

        // Constructor
        public Cooccurr()
        {
        }

        public double Entropy;
        public double Energy;        
        public double Homogenity;
        public double Contrast;
        public double max;
        public double Disimilarity;
        public double Correlation;
        public double thesix;
        public double thesiy;
        private double mx,my,sx,sy,summx,summy;
        

        // Apply filter
        public double[,] Apply(Bitmap srcImg, int Xstep, int Ystep,int orio)
        {
            // get source image size
            int width = srcImg.Width;
            int height = srcImg.Height;
             int apoxroseis =(int) Math.Pow(2, orio);
            double[,] Matrix = new double[apoxroseis, apoxroseis];

            PixelFormat fmt = (srcImg.PixelFormat == PixelFormat.Format8bppIndexed) ?
                PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

            if (fmt == PixelFormat.Format8bppIndexed)
            {
                srcImg = ApplyFilter(new GrayscaleToRGB(), srcImg);

            }

            fmt = (srcImg.PixelFormat == PixelFormat.Format8bppIndexed) ?
               PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;
           


            // lock source bitmap data
            BitmapData srcData = srcImg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, fmt);

            int stride = srcData.Stride;
            int offset = stride - ((fmt == PixelFormat.Format8bppIndexed) ? width : width * 3);
           
         
            int jr, ir, t,i,j;
            //
            for (int k = 0; k <apoxroseis; k++)
            {
                for (int l = 0; l < apoxroseis; l++)
                {

                    Matrix[k, l] = 0;
                }

            }
            
            

            /// do the job
            unsafe
            {
                byte* p;
                byte* src = (byte*)srcData.Scan0.ToPointer();


                
                    // RGB image

                    // for each line
                int AAA = 0;
                int SSS = 0;
                int temp = 256/apoxroseis;
                for (int y = 0; y < height - Ystep; y++)
                    {
                        // for each pixel
                        for (int x = 0; x < width - Xstep; x++, src += 3)
                        {
                            int Gray = (int)(0.114 * src[0] + 0.587 * src[1] + 0.299 * src[2]);
                            //ekteleitai mono gia ligoteres apo 256 apoxroseis
                            if (temp > 1)
                            {
                                //kvantizoume ton pinaka
                                for (i = 0; i < apoxroseis; i++)
                                {

                                    if (Gray > i * temp && Gray < temp * (i + 1)) AAA = i;
                                    
                                    
                                }//
                            }
                            i = Ystep;
                            
                                ir = i ;
                                t = y + ir;

                                // skip row
                                if (t < 0)
                                    continue;
                                // break
                                if (t > height)
                                    continue;

                                // for each kernel column
                                j = Xstep;
                                

                                    jr = j ;
                                    t = x + jr;

                                    p = &src[ir * stride + jr * 3];

                                    // skip column
                                    if (t < 0)
                                        continue;

                                    if (t < width)
                                    {
                                        int GrayNei = (int)(0.114 * p[0] + 0.587 * p[1] + 0.299 * p[2]);
                                        //ekteleitai mono gia ligoteres apo 256 apoxroseis
                                        if (temp > 1)
                                        {
                                            //kvantizoume ton pinaka
                                            for (i = 0; i < apoxroseis; i++)
                                            {

                                                if (GrayNei > i * temp && GrayNei < temp * (i + 1)) SSS = i;

                                            }
                                            //o pinakas gia ligoteres apo 256 apoxroseis
                                            Matrix[AAA, SSS] += 1 / (double)((height - Ystep) * (width - Xstep));
                                        }
                                        else
                                        {
                                            //o pinakas gia 256 apoxroseis
                                            Matrix[Gray, GrayNei] += 1 / (double)((height - Ystep) * (width - Xstep));
                                        }
                                    }
                                


                        }
                        src += offset;

                    }
                
            }
            // unlock both images

            srcImg.UnlockBits(srcData);

            return Matrix;
        }


        private Bitmap ApplyFilter(IFilter filter, Bitmap image)
        {

            return (filter.Apply(image));

        }


        public void CalcElements(double[,] Matrix,int orio)
        {
            max = Matrix[0,0];
            Energy = 0;
            Entropy = 0;
            Contrast = 0;
            Homogenity = 0;
            Disimilarity = 0;
            Correlation = 0;
            summx = 0;
            summy = 0;
            mx = 0;
            my = 0;
            sx = 0;
            sy = 0;
            int apoxroseis =(int) Math.Pow(2,orio);

            for (int i=0;i<apoxroseis;i++)
            {

                for (int j = 0; j <apoxroseis ; j++)
                {
                    if ( Matrix[i, j]>0) Entropy +=  Matrix[i, j] * Math.Log(Matrix[i, j], 2);
                    Energy += Math.Pow(Matrix[i, j], 2);
                    Contrast += Math.Pow((i - j), 2) * Matrix[i, j];
                    Homogenity += Matrix[i, j] / (1 + Math.Pow((i - j), 2));
                    if (Matrix[i, j] > max)
                    {
                        max = Matrix[i, j];
                        thesix = i;
                        thesiy = j;
                    }
                    
                    Disimilarity += Math.Abs(i - j) * Matrix[i, j];

                    summy += Matrix[j, i];
                    summx += Matrix[i,j];
                }
                //elements for the correlation
                my += i * summy;
                mx += i * summx;
                sx += Math.Pow((i - mx), 2) * summx;
                sy += Math.Pow((i - my), 2) * summy;
            }

            for (int i = 0; i < apoxroseis; i++)
            {
                for (int j = 0; j < apoxroseis; j++)
                {
                    Correlation += (i - mx) * (i - my) * Matrix[i, j] /Math.Sqrt(sx * sy);
                }
            }

            Entropy *= -1;

            

        }
        
         


    }
}

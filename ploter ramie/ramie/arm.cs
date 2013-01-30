using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;


class arm
{
    public  int KalibracjaS1 = 10;
    public  int KalibracjaS2 = 10;

    public  int LimitDolS1 = 30;
    public  int LimitGorS1 = 180;

    public  int LimitDolS2 = 35;
    public  int LimitGorS2 = 180;

    public int scale = 315;
    private int s3PenDown = 35;
    private int s3PenUp = 70;
    private int currentX=150, currentY=50;

    public  Bitmap armBmp;
    public Bitmap drawBmp;
    public  Graphics g;
    public  Graphics g1;
 
    private bool PenDown;

  


    public delegate void BmpRefresh(Bitmap arm, Bitmap picture, int progress);
    public event BmpRefresh Bmp_Refresh;

    public delegate void Log(string text);
    public event Log log;

    private Arduino.Arduino arduino=new Arduino.Arduino();


    public void loadSettings()
    {
        StreamReader streamReader = new StreamReader("settings.txt");
        KalibracjaS1 = Convert.ToInt32(  streamReader.ReadLine());
        KalibracjaS2 = Convert.ToInt32(streamReader.ReadLine());
        LimitDolS1 = Convert.ToInt32(streamReader.ReadLine());
        LimitGorS1 = Convert.ToInt32(streamReader.ReadLine());
        LimitDolS2 = Convert.ToInt32(streamReader.ReadLine());
        LimitGorS2 = Convert.ToInt32(streamReader.ReadLine());
        s3PenDown = Convert.ToInt32(streamReader.ReadLine());
        s3PenUp = Convert.ToInt32(streamReader.ReadLine());


        log("Settings:");
        log("Calibration s1:" + KalibracjaS1.ToString());
        log("Calibration s2:" + KalibracjaS2.ToString());
        log(LimitDolS1.ToString() + "-" + LimitGorS1.ToString());
        log(LimitDolS2.ToString() + "-" + LimitGorS2.ToString());
        log("PenDown/PenUp:" + s3PenDown.ToString() + "/" + s3PenUp.ToString());






    }

    public arm()//Graphics g_,Graphics g1_)
    {
        //g = g_;
        //g1 = g1_;
        armBmp = new Bitmap(800, 400);
        drawBmp = new Bitmap(400,400);
        g = Graphics.FromImage(armBmp);
        g.Clear(Color.White);
        g1 = Graphics.FromImage(drawBmp);
        g1.Clear(Color.White);
  

    }

    public void ConnectToArduino(string comPort)
    {
        if (arduino.polaczony == false)
        {
            arduino.polacz(comPort);
            //arduino.WlaczWylaczWyswietlanieStanu();
        }
    }


    public Bitmap RysujRamie(int s1, int s2)
    {
        Bitmap bmp = new Bitmap(200, 100);
        Graphics g = Graphics.FromImage(bmp);

        s1 = s1 - 90 - KalibracjaS1;
        s2 = s2 - KalibracjaS2;
        g.Clear(Color.White);

        double a = (s1) / (180 / Math.PI);
        double b = ((s2) / (180 / Math.PI)) - a;

        int x1, y1, x2, y2;
        int l1 = 50, l2 = 50;


        x1 = (int)(l1 * (Math.Sin(a - 1 / (180 / Math.PI))));
        y1 = 100 - (int)(l1 * (Math.Cos(a - 1 / (180 / Math.PI))));

        x2 = (int)(l2 * (Math.Sin(b - 1 / (180 / Math.PI)))) + x1;
        y2 = (int)(l2 * (Math.Cos(b - 1 / (180 / Math.PI)))) + y1;

        x1 = x1 + 100;
        x2 = x2 + 100;

        g.DrawLine(Pens.Red, x1, y1, 0, 100);
        g.DrawLine(Pens.Green, x1, y1, x2, y2);
        g.DrawEllipse(Pens.Red, x1 - 3, y1 - 3, 6, 6);
        g.DrawEllipse(Pens.Red, x2 - 3, y2 - 3, 6, 6);

        return bmp;

    }



    public void OdwrotnaKinematyka(double x, double y, bool penDown, out int s1, out int s2, out int  s3)
    {
        x = x;// -400;
       
        double L1 =200;
        double Xb = (x) / (2 * L1);
        double Zb = (y) / (2 * L1);
        double Q = Math.Sqrt(1 / (Math.Pow(Xb, 2) + Math.Pow(Zb, 2)) - 1);
        double P1 = (Math.Atan2(Zb + Q * Xb, Xb - Q * Zb)) * 180 / Math.PI;
        double P2 = (Math.Atan2(Zb - Q * Xb, Xb + Q * Zb)) * 180 / Math.PI;
        double T1 = P1 - 90;
        double T2 = P2 - T1;

        s1 = (int)T1;
        s2 = (int)T2;

        g.Clear(Color.White);
       // g.DrawEllipse(Pens.Green, 200, 200, 10, 10);

        double lokiecX = L1 * Math.Cos(P1 * Math.PI / 180);
        double lokiecY = L1 * Math.Sin(P1 * Math.PI / 180);
        double koniecX = lokiecX + L1 * Math.Cos(P2 * Math.PI / 180);
        double koniecY = lokiecY + L1 * Math.Sin(P2 * Math.PI / 180);

        if (Math.Sqrt(x*x+y*y)<L1 *2)
        {
            g.DrawEllipse(Pens.Green, (int)lokiecX - 3 + 400, 400 - (int)lokiecY - 3, 6, 6);
            if (penDown)
            {
                g.FillEllipse(Brushes.Red, (int)koniecX - 3 + 400, 400 - (int)koniecY - 3, 6, 6);
            }
            else
            {
                g.DrawEllipse(Pens.Green, (int)koniecX - 3 + 400, 400 - (int)koniecY - 3, 6, 6);
            }
            //g.DrawEllipse(Pens.Green, 100, 200 , 6, 6);
            g.DrawLine(Pens.Red, (int)lokiecX + 400, 400 - (int)lokiecY, 0 + 400, 400);
            g.DrawLine(Pens.Red, (int)lokiecX + 400, 400 - (int)lokiecY, (int)koniecX + 400,400 - (int)koniecY);

            

        }
        //catch
        {
        }

        s2 = s2 + 90 + KalibracjaS1;
        s1 = 90 - s1 + KalibracjaS2;


        if (penDown && s1>LimitDolS1 && s1<LimitGorS1 && s2>LimitDolS2  && s2<LimitGorS2 )
        {
            //g1.DrawEllipse(Pens.Black, (int)koniecX, 400 - (int)koniecY, 2, 2);
            //g1.FillEllipse(Brushes.Black,(int)koniecX, 400 - (int)koniecY, 2, 2);
            g1.FillRectangle(Brushes.Black, (int) koniecX-2, 400 - (int) koniecY-2, 2, 2);
            s3 = s3PenDown;
        }
        else
        {
            s3 = s3PenUp;
        }

       if (Bmp_Refresh!=null) Bmp_Refresh(armBmp, drawBmp, Progress);
       if (log != null && arduino.polaczony==true  ) log("Servo1=" + s1.ToString() + " Servo2=" + s2.ToString() + " Servo3=" + s3.ToString());

    }


    public int Progress;
    public void Draw(List<string> PltFile, int xOffset, int yOffset, int scale) //scale =315
    {

        arduino.Reset();
        PauseForMilliSeconds(2000);

        int i=0;
        g1.Clear(Color.White);

        foreach (string row in PltFile)
        {
            i++;
            if (row.Contains("PD")) PenDown=true ;
            if (row.Contains("PU")) PenDown=false ;

            


            string rowTemp = row.Replace(";", "");
            rowTemp = rowTemp.Substring(2);

            string[] data = rowTemp.Split(' ');

            int x = ((int)map(Convert.ToInt32(data[0]), 0, scale, 0, 400) + xOffset);
            int y = ((int)map(Convert.ToInt32(data[1]), 0, scale, 0, 400) + yOffset);

            DrawLine(currentX, currentY, x, y);

            currentX = x;
            currentY = y;

           
            Progress =(int) map(i, 0, PltFile.Count-1, 0, 100);
             
        }
        arduino.Reset();

    }



    private void DrawLine(int x0, int y0, int x1, int y1)
    {
        int dx = x1 - x0;
        int dy = y1 - y0;

        SetPosition(x0, y0);
        if (Math.Abs(dx) > Math.Abs(dy))
        {
            float m = (float)dy / (float)dx;      // compute slope
            float b = y0 - m * x0;
            dx = (dx < 0) ? -1 : 1;
            while (x0 != x1)
            {
                x0 += dx;
                SetPosition(x0, (int)Math.Round(m * x0 + b));
                Application.DoEvents();
            }
        }
        else
            if (dy != 0)
            {                              // slope >= 1
                float m = (float)dx / (float)dy;      // compute slope
                float b = x0 - m * y0;
                dy = (dy < 0) ? -1 : 1;
                while (y0 != y1)
                {
                    y0 += dy;
                    SetPosition((int)Math.Round(m * y0 + b), y0);
                    Application.DoEvents();
                }
            }
    }



    public  void SetPosition(int x,int y)
    {
        int s1, s2,s3;
        OdwrotnaKinematyka(x,y,PenDown,out s1, out s2,out s3);
        if (s1 >= 0 && s1 <=180  && s2 >= 0 && s2 <= 180 && s3 >= 0 && s3 <= 180)
        {
            if (arduino.polaczony)
            {
                arduino.ustawSerwa((byte)s1, (byte)s2, (byte)s3,0);
            }
        }

    }

    public void SetServos(byte s1, byte s2, byte s3)
    {
        if (arduino.polaczony)
        {
            arduino.ustawSerwa(s1, s2, s3, 5);
        }
    }

  
    private double map(double x, double in_min, double in_max, double out_min, double out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    public void PauseForMilliSeconds(int MilliSecondsToPauseFor)
    {


        System.DateTime ThisMoment = System.DateTime.Now;
        System.TimeSpan duration = new System.TimeSpan(0, 0, 0, 0, MilliSecondsToPauseFor);
        System.DateTime AfterWards = ThisMoment.Add(duration);


        while (AfterWards >= ThisMoment)
        {
            System.Windows.Forms.Application.DoEvents();
            ThisMoment = System.DateTime.Now;
        }



    }

    public void Disconnect()
    {
        arduino.rozlacz();
    }
}
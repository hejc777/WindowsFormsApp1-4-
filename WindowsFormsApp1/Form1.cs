using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        class ItemInfo
        {
            /// <summary>
            /// ItemInfo 类记录数组元素重复次数
            /// </summary>
            /// <param name="value">数组元素值</param>
            public ItemInfo(int value)
            {
                Value = value;
                RepeatNum = 1;
            }
            /// <summary>
            /// 数组元素的值
            /// </summary>
            public int Value { get; set; }
            /// <summary>
            /// 数组元素重复的次数
            /// </summary>
            public int RepeatNum { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Paint(object sender, PaintEventArgs e)
        {
            Button btn = sender as Button;
            System.Drawing.Drawing2D.GraphicsPath btnPath = new System.Drawing.Drawing2D.GraphicsPath();
            System.Drawing.Rectangle newRectangle = btn.ClientRectangle;
            newRectangle.Inflate(-2, -1);
            e.Graphics.DrawEllipse(System.Drawing.Pens.BlanchedAlmond, newRectangle);
            newRectangle.Inflate(-2, -4);
            btnPath.AddEllipse(newRectangle);
            btn.Region = new System.Drawing.Region(btnPath);
        }
        //创建数据表
        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        DataTable dt2 = new DataTable();
        DataTable dt3 = new DataTable();
        DataTable dt4 = new DataTable();


        int iiReadRec = 0; //读取记录数
        int sumRec = 0;    //合值
        double iiCount = 0; //所有生成数据量
        int js = 0;//奇数
        int os = 0;//偶数
        int numCount = 6; //生成号码总数
        int numMAX = 33;   //生成号码最大值，快乐8是80+1，双色球是33+1
        Boolean fxstop = false; //中止分析
        decimal[][] bfltable =
        {
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0},
            new decimal[2]{0,0}

        };
        
        
        int[,] bgz = new int[8,4]{
            { 1, 10, 19, 28 },
            { 2, 11, 20, 29 },
            { 3, 12, 21, 30 },
            { 4, 13, 22, 31 },
            { 5, 14, 23, 32 },
            { 6, 15, 22, 25 },
            { 7, 16, 17, 26 },
            { 8, 9, 18, 27 }
        };

        //集团号码，若随机的号码里面组合有5个与该下列号码相同，就排除。
        int[] jthm = { 1, 2, 3, 4, 6, 9, 14, 16, 18, 24, 25, 28, 32 };

        //排除万能12码
        //如果你选择的号码里面没有以下号码其中之一，就排除。
        int[] wlm = { 1, 10, 11, 12, 14, 15, 18, 19, 24, 25, 27, 29 };

        //排除神奇数字7
        //如果你选择的号码里没有以下号码或大于等于5个，则排除。
        int[] s7sz = { 7, 14, 16, 17, 18, 21, 25, 27, 28, 29 };

        //排除质数为0的号码
        //如果你选择的号码里没有质数时，则排除。
        int[] zshm = { 1, 3, 5, 7, 11, 13, 15, 17, 19, 23, 29, 31 };

        //排除没有2，3，6，7，9尾号
        //经研究，开出的号码多数要包含带有尾数为2，3，6，7，9的数字，没有则排除
        int[] w23679w = { 2, 13, 16, 27, 29 };

        int[] zbh = { };

        //分组排除法-----
        int qxcount = 4; //出现几个排除设定
        int[,] hm1 = new int[,]
        {
            {1,7,13,19,25,31 }, //同一注中最多出现3码99%（1%的可能出现4码）
            {2,8,14,20,26,32 }, //同一注中最多出现3码97%（3%的可能出现4码）
            {3,9,15,21,27,33 }  //同一注中最多出现3码97%（3%的可能出现4码）
        };
        int[] hm2 = { 2, 8, 14, 20, 26, 31, 32, 33 }; //同一注中最多出现3码92%（8%的可能出现4码）
        int[,] hm3 = new int[,]
        {
            {5,11,17,23,29 }, //同一注中最多出现3码（85%的可能出现2码）
            {4,10,16,22,28 }, //同一注中最多出现3码（85%的可能出现2码）
            {6,12,18,24,30 }  //同一注中最多出现3码（85%的可能出现2码）
        };
        int[,] hm4 = new int[,]
        {
            {4,7,8,9,10,11,12,16,22,28 },//同一注中最多出现4码97%（3%的可能出现5码）
            {5,7,8,9,10,11,12,17,23,29 },//同一注中最多出现4码96%（4%的可能出现5码）
            {6,7,8,9,10,11,12,18,24,30 },//同一注中最多出现4码99%（1%的可能出现5码）
            {1,2,3,4,5,6,11,17,23,29 },//同一注中最多出现4码98%（2%的可能出现5码）
            {1,2,3,4,5,6,12,18,24,30 }//同一注中最多出现4码97%（2%的可能出现5码,%1出现6码）
        };
        int[,] hm5 = new int[,]
        {
            {1,4,7,10,13,16,19,22,25,28,33 },//同一注中最多出现4码95%（5%的可能出现5码）
            {1,6,7,12,13,18,19,24,25,30,33 },//同一注中最多出现4码96%（4%的可能出现5码，1%出现6码）
            {2,5,8,11,14,17,20,23,26,29,32 }//同一注中最多出现4码98%（2%的可能出现5码）
        };
        int[] hm6 = { 2, 3, 8, 9, 14, 15, 20, 21, 26, 27, 32, 33 }; //同一注中最多出现4码93%（7%的可能出现5码）
        int hm7count = 5;
        int[,] hm7 = new int[,]
        {
            {3,4,9,10,15,16,21,22,27,28,33 },//同一注中最多出现4码98%（2%的可能出现5码）
            {3,6,9,12,15,16,21,24,27,30,33 }//同一注中最多出现4码99%（1%的可能出现5码）
        };
        int[] hm8 = { 4, 5, 10, 11, 16, 17, 22, 23, 28, 29 };//同一注中最多出现4码99%（1%的可能出现5码）
        int[,] hm9 = new int[,]
        {
            {1,7,8,9,10,11,12,13,19,25,31 },//同一注中最多出现4码96%（4%的可能出现5码）
            {3,7,8,9,10,11,12,15,21,27,33 }//同一注中最多出现4码95%（5%的可能出现5码）
        };
        //如果你发现你的号码组里有5个以上在以上号码组里，那么你本期的投注号码将与六红基本告别；
        //分组排除法结束-----

        //15 groupNumber
        int[,] hm11 = new int[3, 11];
        //{
        //    //{2,3,11,16,18,19,21,22,26,27,29,31 },
        //    //{3,5,7,11,13,14,21,23,24,27,32,33 },
        //    //{1,2,3,6,7,11,15,18,24,25,29,32 },
        //    //{2,6,8,10,13,15,19,23,27,29,31,32 },
        //    //{5,8,12,14,15,17,18,25,26,30,31,33 }

        //    //{3,4,6,10,13,15,24,27,30,33 },
        //    //{7,10,12,14,16,20,27,28,30,31 }
        //    //{2,3,4,8,14,15,17,18,28,32 },
        //    //{6,9,12,14,18,19,25,29,30,33 },
        //    //{1,6,10,13,15,21,22,24,25,31 },
        //    //{4,5,7,11,12,16,19,22,23,25 }

        //    //{1,4,7,10,13,16,19,22,25,28,31 },
        //    //{2,5,8,11,14,17,20,23,26,29,32 },
        //    //{3,6,9,12,15,18,21,24,27,30,33 }

        //    //{2,8,10,19,23,27,29,31 },
        //    //{1,2,3,7,11,18,25,32},
        //    //{3,5,9,13,22,26,30,33 },
        //    //{4,7,10,14,17,18,23,25 },
        //    //{5,6,11,16,19,22,28,31 },
        //    //{7,9,11,13,19,25,27,33 },
        //    //{7,10,14,15,18,20,25,31 }
        //};
        int[,] hm10 = new int[,]
        {
            {1,4,13,18,26,30,3 },
            {2,14,15,17,25,30,11 },
            {2,15,22,26,30,33,4 },
            {5,7,9,16,17,29,15 },
            {1,7,10,13,27,33,15 },
            {1,5,7,11,18,19,7 },  //中奖一期时没有要这组含之前的数据
            {2,9,26,27,31,32,14 },
            {2,3,4,6,11,15,14 },
            {3,12,14,16,29,32,12},
            {1,15,20,22,31,32,3},
            {3,10,11,19,27,28,7},
            {7,11,18,24,27,32,4},
            {4,5,23,24,26,31,11},
            {8,11,16,25,29,32,3 },
            {1,4,11,12,22,30,16},
            {4,13,17,23,25,33,14},
            {3,4,24,28,29,33,9},
            {1,8,9,23,24,30,8},
            {1,6,8,13,17,19,9},
            {3,8,11,22,31,33,4},
            {2,5,17,19,29,33,4},
            {5,16,23,24,26,29,4},
            {12,21,23,27,32,33,15},
            {9,15,18,21,22,25,1},
            {8,12,15,17,19,30,8},
            {13,14,19,24,27,30,5},
            {4,12,17,24,26,27,16},
            {3,8,10,20,30,31,2},
            {4,10,12,13,18,30,3},
            {2,4,14,18,23,27,13},
            {4,6,9,14,16,21,3},
            {6,13,17,22,25,27,9},
            {1,5,7,11,12,15,12},
            {3,8,11,12,18,19,5},
            {13,15,16,19,20,24,10},
            {7,11,17,24,31,32,12},
            {3,6,9,15,18,31,1},
            {3,8,10,17,30,32,10},
            {3,9,18,19,20,26,11},
            {17,19,20,23,25,31,2},
            {1,5,15,21,23,27,15},
            {1,8,10,13,19,29,13},
            {3,9,14,29,32,33,15},
            {1,2,13,16,17,29,16},
            {1,6,12,17,23,25,4},
            {2,3,6,7,16,26,4},
            {5,9,14,21,22,26,12},
            {1,4,6,14,17,22,8},
            {3,5,8,18,22,28,1},
            {3,22,24,27,29,32,15},
            {7,8,10,22,24,32,7},
            {3,9,12,18,28,30,1},
            {1,14,20,21,23,27,6},
            {2,8,19,28,30,31,14},
            {4,13,18,20,22,28,5},
            {6,13,20,21,24,32,6},
            {1,2,7,19,20,21,1},
            {1,11,13,17,25,29,3},
            {8,9,12,22,26,32,13},
            {3,5,9,10,19,22,14},
            {6,8,17,18,28,30,5},
            {7,14,16,23,28,32,4},
            {1,7,10,16,18,27,16},
            {1,9,18,22,25,28,2},
            {1,2,6,10,22,28,15},
            {1,3,14,25,31,33,7},
            {8,12,13,17,27,29,13},
            {3,8,17,18,23,31,8},
            {5,18,20,24,25,26,6},
            {2,4,6,7,16,29,3},
            {8,13,20,25,31,32,3},
            {7,14,21,22,28,33,7},
            {7,10,11,15,17,21,3},
            {5,9,13,20,23,28,6},
            {1,3,7,10,22,33,2},
            {12,15,17,23,26,32,11},
            {2,9,15,19,26,28,2},
            {7,8,21,26,29,30,15},
            {2,6,10,11,17,29,15},
            {2,8,19,23,24,26,3},
            {2,6,17,25,32,33,6},
            {4,6,7,14,15,24,8},
            {2,4,5,14,26,32,14},
            {2,9,12,22,25,33,16},
            {11,14,18,19,23,26,2},
            {2,6,12,29,30,31,10},
            {2,6,12,29,28,33,7},
            {8,10,18,23,27,31,2},
            {1,4,5,6,12,14,13},
            {5,7,14,17,21,31,6},
            {2,9,12,19,21,31,4},
            {6,10,11,18,20,32,5},
            {1,3,4,11,12,21,16},
            {9,10,13,25,30,32,2},
            {1,8,22,25,29,33,10},
            {12,18,23,25,28,33,4},
            {3,7,8,11,18,19,5},
            {2,6,13,27,28,32,13},
            {4,7,18,19,20,25,6},
            {8,15,21,22,25,33,13},
            {3,7,21,24,26,30,10},
            {1,10,22,25,28,32,10},
            {2,9,11,14,18,26,6},
            {4,6,16,17,23,24,11},
            {3,8,12,14,17,33,8},
            {5,11,12,16,17,20,8},
            {6,15,17,24,28,29,16},
            {4,17,19,20,25,32,4},
            {7,12,20,24,32,33,4},
            {3,8,17,18,20,30,15},
            {1,2,10,22,24,25,13},
            {1,7,10,14,21,25,7},
            {1,3,7,18,22,28,15},
            {8,12,16,20,27,31,6},
            {11,16,20,21,23,24,4},
            {1,4,7,10,17,23,14},
            {8,20,21,23,27,30,13},
            {4,5,6,7,20,22,15},
            {7,18,20,21,26,32,5},
            {5,6,20,23,25,32,3},
            {9,13,17,18,21,27,4},
            {1,5,8,13,32,33,3},
            {4,9,10,17,19,20,9},
            {3,7,16,26,27,32,16},
            {1,15,16,20,25,27,5 }, //2023
            {10,17,25,26,30,33,5 },
            {2,10,19,24,26,33,15 },
            {3,5,7,9,19,20,8 },
            {5,6,14,16,19,32,12 },
            {2,3,6,11,20,32,9 },
            {7,10,21,22,23,24,11 },
            {12,16,17,22,25,27,8 },
            {5,7,9,16,23,30,15 },
            {2,8,20,21,27,33,14 },
            {1,8,11,12,21,23,3 },
            {3,4,11,14,30,31,5 },
            {12,14,19,21,27,31,11 },
            {3,22,28,30,31,33,4 },
            {3,12,17,19,23,27,11 },
            { 3,5,10,21,22,26,10 },
            {5,6,11,13,28,30,7 },
            {5,8,9,17,22,33,4 },
            {4,5,10,14,22,25,3 },
            {3,9,11,15,25,27,1 },
            {6,7,14,20,26,32,1 } //2023-11-14
        };

        DateTime dtStart,dtEndtime;



        public static OleDbConnection Conn = null;//连接数据库对象
        public OleDbConnection ConnValue
        {
            get { return Conn; }
            set { Conn = value; }
        }
        string conString = @"Data Source=.;Initial Catalog=cp;Persist Security Info=True;User ID=sa;Password=hejc190";
        SqlConnection sqlconn;
        SqlCommand sqlcommand;
        SqlDataAdapter sqldata;

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            userControl17.unbuttonbackcolor();

            //创建数据列号码①②③④⑤⑥⑦
            dt.Columns.Add("num1", typeof(string));
            dt.Columns.Add("num2", typeof(string));
            dt.Columns.Add("num3", typeof(string));
            dt.Columns.Add("num4", typeof(string));
            dt.Columns.Add("num5", typeof(string));
            dt.Columns.Add("num6", typeof(string));
            dt.Columns.Add("num7", typeof(string));
            dt.Columns.Add("num8", typeof(string));
            dt.Columns.Add("num9", typeof(string));
            dt.Columns.Add("num10", typeof(string));


            //创建数据列号码①②③④⑤⑥⑦
            dt1.Columns.Add("num1", typeof(string));
            dt1.Columns.Add("num2", typeof(string));
            dt1.Columns.Add("num3", typeof(string));
            dt1.Columns.Add("num4", typeof(string));
            dt1.Columns.Add("num5", typeof(string));
            dt1.Columns.Add("num6", typeof(string));
            dt1.Columns.Add("num7", typeof(string));

            //创建数据列号码①②③④⑤⑥⑦
            dt2.Columns.Add("num1", typeof(string));
            dt2.Columns.Add("num2", typeof(string));
            dt2.Columns.Add("num3", typeof(string));
            dt2.Columns.Add("num4", typeof(string));
            dt2.Columns.Add("num5", typeof(string));
            dt2.Columns.Add("num6", typeof(string));
            dt2.Columns.Add("num7", typeof(string));

            //创建数据列号码①②③④⑤⑥⑦
            dt3.Columns.Add("num1", typeof(string));
            dt3.Columns.Add("num2", typeof(string));
            dt3.Columns.Add("num3", typeof(string));
            dt3.Columns.Add("num4", typeof(string));
            dt3.Columns.Add("num5", typeof(string));
            dt3.Columns.Add("num6", typeof(string));
            dt3.Columns.Add("num7", typeof(string));

            //创建数据列号码①②③④⑤⑥⑦
            dt4.Columns.Add("num1", typeof(string));
            dt4.Columns.Add("num2", typeof(string));
            dt4.Columns.Add("num3", typeof(string));
            dt4.Columns.Add("num4", typeof(string));
            dt4.Columns.Add("num5", typeof(string));
            dt4.Columns.Add("num6", typeof(string));
            dt4.Columns.Add("num7", typeof(string));
        }


        Boolean timeEnd = true;
        

        private void undisplay(int i, string vdisplay)
        {
            switch (i)
            {
                case 0:
                    userControl11.unbuttontext = vdisplay;
                    break;
                case 1:
                    userControl12.unbuttontext = vdisplay;
                    break;
                case 2:
                    userControl13.unbuttontext = vdisplay;
                    break;
                case 3:
                    userControl14.unbuttontext = vdisplay;
                    break;
                case 4:
                    userControl15.unbuttontext = vdisplay;
                    break;
                case 5:
                    userControl16.unbuttontext = vdisplay;
                    break;

            }
        }

        static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public void doWork(int ks, int jz,DevComponents.DotNetBar.Controls.ProgressBarX Pb)
        {
            //string[,] aray; // 1000行，6列
            Pb.Maximum = jz;
            for (int ii = ks; ii < jz; ii++)
            {
                Random rnd = new Random(GetRandomSeed());
                int[] num = new int[numCount];
                Pb.Value = ii;
                Pb.Text = ii.ToString() + "/" + jz.ToString();
                //Application.DoEvents();
                //aray = new string[1, 7];0
                while (true)
                {
                    for (int i = 0; i < numCount; i++)
                    {
                        Boolean cfnum = false;
                        string rndnum;
                        while (true)
                        {
                            rndnum = rnd.Next(1, numMAX).ToString();
                            //num[0] = int.Parse(rndnum);
                            //undisplay(i, rndnum);
                            //Thread.Sleep(int.Parse(textBox1.Text));
                            Application.DoEvents();

                            //int tmp = Array.IndexOf(num, rndnum);
                            //if (tmp == -1)
                            //{
                            //    num[i] = int.Parse(rndnum);
                            //    break;
                            //}
                            //else continue;


                            for (int c = 0; c < numCount; c++)
                            {
                                if (num[c] == int.Parse(rndnum))
                                {
                                    cfnum = true;
                                    break;
                                }
                                else
                                {
                                    cfnum = false;
                                }
                            }
                            if (cfnum == false)
                            {
                                num[i] = int.Parse(rndnum);
                                //undisplay(i, rndnum);
                                // aray[0, i] = rndnum;
                                break;
                            }
                        }
                    }
                    //八卦阵判断
                    int bgzCount = 0;
                    for (int cc = 0; cc < 8; cc++)
                    {
                        for (int nn = 0; nn < 4; nn++)
                        {
                            int tmp = Array.IndexOf(num, bgz[cc, nn]);
                            if (tmp != -1) bgzCount++;
                        }
                        if (bgzCount >= 3) break;
                        else bgzCount = 0;
                    }
                    if (bgzCount < 3) break;
                }
                //iiCount++;
                
                Array.Sort(num);




                //蓝号
                string xx = rnd.Next(1, 17).ToString();
                //userControl17.unbuttontext = xx;
                
                lock (dt)
                {
                    if (numCount == 6)
                    {
                        DataRow dr = dt.NewRow();
                        dr["num1"] = num[0];//aray[0, 0];
                        dr["num2"] = num[1];//aray[0, 1];
                        dr["num3"] = num[2];//aray[0, 2];
                        dr["num4"] = num[3];//aray[0, 3];
                        dr["num5"] = num[4];//aray[0, 4];
                        dr["num6"] = num[5];//aray[0, 5];
                        dr["num7"] = xx; //aray[0, 6];
                        dt.Rows.Add(dr);
                    }else
                    {
                        DataRow dr = dt.NewRow();
                        dr["num1"] = num[0];//aray[0, 0];
                        dr["num2"] = num[1];//aray[0, 1];
                        dr["num3"] = num[2];//aray[0, 2];
                        dr["num4"] = num[3];//aray[0, 3];
                        dr["num5"] = num[4];//aray[0, 4];
                        dr["num6"] = num[5];//aray[0, 5];
                        dr["num7"] = num[6];//aray[0, 5];
                        dr["num8"] = num[7];//aray[0, 5];
                        dr["num9"] = num[8];//aray[0, 5];
                        dr["num10"] = num[9];//aray[0, 5];
                        dt.Rows.Add(dr);
                    }
                }
            }
            dtEndtime = DateTime.Now;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dtStart = DateTime.Now;
            //等分
            int ii_sum = int.Parse(textBox2.Text.ToString());
            int zc, ys; //
            zc = ii_sum / 100;
            ys = ii_sum % 10;

            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 2; // 最大并行度，并行的任务有几个

            #region task方式
            //var tthread = new Task[10];

            //for (int i = 0; i < 10; i++)
            //{
            //    object ob_Label;
            //    ob_Label = this.GetType().GetField("progressBarX" + (i+1).ToString(), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(this);
            //    tthread[i] = Task.Run(() =>
            //    {
            //        doWork(1, zc, (DevComponents.DotNetBar.Controls.ProgressBarX)ob_Label);
            //    });
            //}

            //Task.WhenAll(tthread).ContinueWith(t =>
            //{
            //    dtEndtime = DateTime.Now;
            //    label7.Text = "用时:" + (dtEndtime - dtStart).ToString();

            //});
            //dataGridView1.DataSource = dt;
            //WriteTextFile(dt, @"c:\\2.dat", checkBox1.Checked);
            #endregion

            for (int oo = 0; oo < 100; oo++)
            {

                //构造多任务
                dt.Reset();
                dt.Columns.Add("num1", typeof(string));
                dt.Columns.Add("num2", typeof(string));
                dt.Columns.Add("num3", typeof(string));
                dt.Columns.Add("num4", typeof(string));
                dt.Columns.Add("num5", typeof(string));
                dt.Columns.Add("num6", typeof(string));
                dt.Columns.Add("num7", typeof(string));
                if (numCount >6)
                {
                    dt.Columns.Add("num8", typeof(string));
                    dt.Columns.Add("num9", typeof(string));
                    dt.Columns.Add("num10", typeof(string));
                }
                Parallel.For(0, 3, i =>
                {

                    object ob_Label;
                    ob_Label = this.GetType().GetField("progressBarX" + (i + 1).ToString(), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(this);
                    doWork(1, zc/3, (DevComponents.DotNetBar.Controls.ProgressBarX)ob_Label);
                //WriteTextFile(dt, @"c:\\1.dat", checkBox1.Checked);
                });

                dtEndtime = DateTime.Now;
                label7.Text = "第"+oo.ToString()+"轮用时:" + (dtEndtime - dtStart).ToString();
                dataGridView1.DataSource = dt;
                WriteTextFile(dt, @"c:\\5555.dat", checkBox1.Checked);
                
            };
        }


        //private void button1_Click(object sender, EventArgs e)
        //{

        //    //timer1.Interval = int.Parse(textBox1.Text.ToString());
        //    //timer1.Enabled = true;
        //    progressBar1.Maximum = int.Parse(textBox2.Text);
        //    progressBarX1.Maximum = int.Parse(textBox2.Text);
        //    progressBarX1.Enabled = true;
        //    button1.Enabled = false;
        //    aray = new string[int.Parse(textBox2.Text), 7];
        //    dtStart = DateTime.Now;

        //        System.DateTime.Now.TimeOfDay.ToString();
        //    for (int ii = 1; ii < int.Parse(textBox2.Text); ii++)
        //    {
        //        Random rnd = new Random(GetRandomSeed());
        //        string[] num = new string[6];
        //        progressBar1.Value = ii;
        //        progressBarX1.Value = ii;
        //        progressBarX1.Text = ii.ToString() + "/" + int.Parse(textBox2.Text);
        //        for (int i = 0; i < 6; i++)
        //        {

        //            Boolean cfnum = false;
        //            string rndnum;
        //            //while (true)
        //           // {
        //                while (true)
        //                {
        //                    rndnum = rnd.Next(1, 34).ToString();
        //                    undisplay(i, rndnum);
        //                    //Thread.Sleep(int.Parse(textBox1.Text));
        //                    Application.DoEvents();
        //                    //foreach(string aa in num)
        //                    //{
        //                    //    if (aa == rndnum)
        //                    //    { 
        //                    //        cfnum = true; 
        //                    //        break;
        //                    //    } else cfnum = false;
        //                    //}
        //                    for (int c = 0; c < 6; c++)
        //                    {
        //                        if (num[c] == rndnum)
        //                        {
        //                            cfnum = true;
        //                            break;
        //                        }
        //                        else
        //                        {
        //                            cfnum = false;
        //                        }
        //                    }
        //                    if (cfnum == false)
        //                    {
        //                        num[i] = rndnum;
        //                        undisplay(i, rndnum);
        //                        aray[ii, i] = rndnum;
        //                        break;
        //                    }
        //                }
        //        }
        //        userControl17.unbuttontext = rnd.Next(1, 17).ToString();
        //        aray[ii, 6] = userControl17.unbuttontext;
        //        iiCount++;
        //        DataRow dr = dt.NewRow();
        //        dr["num1"] = aray[ii, 0];
        //        dr["num2"] = aray[ii, 1];
        //        dr["num3"] = aray[ii, 2];
        //        dr["num4"] = aray[ii, 3];
        //        dr["num5"] = aray[ii, 4];
        //        dr["num6"] = aray[ii, 5];
        //        dr["num7"] = aray[ii, 6];
        //        dt.Rows.Add(dr);

        //        //dataGridView1.DataSource = dt;

        //        //string araylist = aray[ii, 0] + "," + aray[ii, 1] + "," + aray[ii, 2] + "," + aray[ii, 3] + "," + aray[ii, 4] + ","
        //        //                 + aray[ii, 5] + "-" + aray[ii, 6] + Environment.NewLine;
        //        //textBox3.AppendText(araylist);

        //        //List list = aray.ToList();

        //        //System.IO.File.WriteAllText(@"c:\\1.dat",x.ToString(), Encoding.Default);


        //    }
        //    button1.Enabled = true;
        //    dtEndtime = DateTime.Now;
        //    dataGridView1.DataSource = dt;
        //    label7.Text = "用时:" + (dtEndtime - dtStart).ToString();
        //    WriteTextFile(dt, @"c:\\1.dat",checkBox1.Checked);

        //}

        private void timer1_Tick(object sender, EventArgs e)
        {
            timeEnd = false; //多少秒后触发一次

        }

        public void WriteTextFile(DataTable dt, string TextFile, bool IBappend = false)
        {
            StringBuilder sbText = new StringBuilder();
            string splitStr = ",";
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dr[i] != DBNull.Value)
                        sbText.Append(dr[i].ToString());
                    if (i < dt.Columns.Count - 1)
                        sbText.Append(splitStr);
                }
                sbText.Append("\r\n");
            }
            System.IO.StreamWriter sw = new System.IO.StreamWriter(TextFile, IBappend, System.Text.Encoding.Default);
            sw.Write(sbText.ToString());
            sw.Close();
        }

        /// <summary>
        ///  读取Text文件，把数据存放到DataTable
        /// </summary>
        /// <param name="dt">存放数据的DataTable</param>
        /// <param name="TextFile">Text文件名</param>
        public void ReadTextFile(ref DataTable dts, string TextFile)
        {
            dts.Rows.Clear();

            iiReadRec = 0; 
            if (System.IO.File.Exists(TextFile))
            {
                System.IO.FileStream fs = new System.IO.FileStream(TextFile, System.IO.FileMode.Open);
                System.IO.StreamReader m_streamReader = new System.IO.StreamReader(fs, System.Text.Encoding.Default);
                //m_streamReader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                string strLine = m_streamReader.ReadLine();
                char splitStr = ',';
                int iirow = 0; //第几行
                //读取文件内容
                while (!string.IsNullOrEmpty(strLine))
                {
                    iirow++;
                    label3.Text = "正在读取[" + iirow.ToString() + "]行数据。";
                    Application.DoEvents();
                    DataRow drNew = dts.NewRow();
                    string[] arrItem = strLine.Split(new char[] { splitStr }, StringSplitOptions.None);
                    for (int i = 0; i < arrItem.Length; i++)
                    {
                        if (i < dts.Columns.Count)
                        {
                            drNew[i] = arrItem[i];
                        }
                        else
                        {
                            break;
                        }
                    }
                    iiReadRec++;
                    dts.Rows.Add(drNew);
                    strLine = m_streamReader.ReadLine();
                }
                m_streamReader.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ReadTextFile(ref dt1, @"c:\\3.dat");
            label3.Text = "共计读取" + iiReadRec.ToString() + "条记录.";
            dataGridView2.DataSource = dt1;
        }

        public bool jopd(int num)
        {
            if (num % 2 == 0)
            {
                return true;
            }
            else return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                sumRec = int.Parse(textBox3.Text) + int.Parse(textBox4.Text) + int.Parse(textBox5.Text) +
                         int.Parse(textBox6.Text) + int.Parse(textBox7.Text) + int.Parse(textBox8.Text) +
                         int.Parse(textBox9.Text);
                js = 0; os = 0;
                if (int.Parse(textBox3.Text) % 2 == 0)
                {
                    os++;
                }
                else js++;
                if (int.Parse(textBox4.Text) % 2 == 0)
                {
                    os++;
                }
                else js++;
                if (int.Parse(textBox5.Text) % 2 == 0)
                {
                    os++;
                }
                else js++;
                if (int.Parse(textBox6.Text) % 2 == 0)
                {
                    os++;
                }
                else js++;
                if (int.Parse(textBox7.Text) % 2 == 0)
                {
                    os++;
                }
                else js++;
                if (int.Parse(textBox8.Text) % 2 == 0)
                {
                    os++;
                }
                else js++;
                if (int.Parse(textBox9.Text) % 2 == 0)
                {
                    os++;
                }
                else js++;

                label4.Text = "合值:"+sumRec.ToString();
                label6.Text = "奇" + js.ToString() + "/" + "偶" + os.ToString();
            }
            catch
            {

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int iiSum = 0;
            dt2.Rows.Clear();
            for (int i= 0;i < dt1.Rows.Count; i++)
            {
                Application.DoEvents();
                int[] xx = new int[7];
                xx[0] = int.Parse(dt1.Rows[i][0].ToString());
                xx[1] = int.Parse(dt1.Rows[i][1].ToString());
                xx[2] = int.Parse(dt1.Rows[i][2].ToString());
                xx[3] = int.Parse(dt1.Rows[i][3].ToString());
                xx[4] = int.Parse(dt1.Rows[i][4].ToString());
                xx[5] = int.Parse(dt1.Rows[i][5].ToString());
                xx[6] = int.Parse(dt1.Rows[i][6].ToString());
                

                int tmpJS = 0,tmpOS = 0;
                if (xx[0] % 2 == 0) { tmpOS++; } else  tmpJS++; 
                if (xx[1] % 2 == 0) { tmpOS++; } else  tmpJS++; 
                if (xx[2] % 2 == 0) { tmpOS++; } else  tmpJS++; 
                if (xx[3] % 2 == 0) { tmpOS++; } else  tmpJS++; 
                if (xx[4] % 2 == 0) { tmpOS++; } else  tmpJS++; 
                if (xx[5] % 2 == 0) { tmpOS++; } else  tmpJS++; 
                if (xx[6] % 2 == 0) { tmpOS++; } else  tmpJS++; 


                if (xx[0]+xx[1]+xx[2]+xx[3]+xx[4]+xx[5]+xx[6]== sumRec && tmpOS == os && tmpJS == js) 
                {
                    
                    DataRow dr = dt2.NewRow();
                    dr[0] = xx[0].ToString();
                    dr[1] = xx[1].ToString();
                    dr[2] = xx[2].ToString();
                    dr[3] = xx[3].ToString();
                    dr[4] = xx[4].ToString();
                    dr[5] = xx[5].ToString();
                    dr[6] = xx[6].ToString();
                    dt2.Rows.Add(dr);
                    //dataGridView3.DataSource = dt2;
                    iiSum++; //满足条件记录数
                }
            }

            dt2 = GetDistinctTable(dt2);

            dataGridView3.DataSource = dt2;

            label5.Text = "共计" + iiSum.ToString() + "条数据满足条件."+"去重复数据后:"+dt2.Rows.Count.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int[] cc = new int[34];
            Label lbtxt = new Label();
            int labelnum = 8;
            int ibsum = 0;
            label41.Text = "记录总数" + dt4.Rows.Count.ToString();
            for (int i = 1; i < 34; i++)
            {
                //Application.DoEvents();
                
                for (int o = 0; o < dt4.Rows.Count; o++)
                {
                    Application.DoEvents();
                    for (int c = 0; c < 6; c++)
                    {
                        string tmp = dt4.Rows[o][c].ToString();
                        if (i == int.Parse(tmp))
                        {
                            cc[i]++;
                            break;
                        }
                    }
                }
                ibsum = ibsum + cc[i];
                object ob_Label;
                ob_Label = this.GetType().GetField("label" + (labelnum).ToString(), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(this);
                Label lb_label;
                lb_label = (System.Windows.Forms.Label)ob_Label;
                float xx = (float)cc[i];
                float xx1 = (float)dt4.Rows.Count;
                float xx2 = (xx / xx1)* 100;
                lb_label.Text = (int)xx2 +"%";
                labelnum++;
            }
            label41.Text = "所有号码合值" + ibsum.ToString();
        }

        #region datatable去重
        /// <summary>
        /// datatable去重
        /// </summary>
        /// <param name="dtSource">需要去重的datatable</param>
        /// <param name="columnNames">依据哪些列去重</param>
        /// <returns></returns>
        public static DataTable GetDistinctTable(DataTable dtSource, params string[] columnNames)
        {
            DataTable distinctTable = dtSource.Clone();
            try
            {
                if (dtSource != null && dtSource.Rows.Count > 0)
                {
                    DataView dv = new DataView(dtSource);
                    distinctTable = dv.ToTable(true, columnNames);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
            return distinctTable;
        }

        /// <summary>
        /// datatable去重
        /// </summary>
        /// <param name="dtSource">需要去重的datatable</param>
        /// <returns></returns>
        public static DataTable GetDistinctTable(DataTable dtSource)
        {
            DataTable distinctTable = null;
            try
            {
                if (dtSource != null && dtSource.Rows.Count > 0)
                {
                    string[] columnNames = GetTableColumnName(dtSource);
                    DataView dv = new DataView(dtSource);
                    distinctTable = dv.ToTable(true, columnNames);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
            return distinctTable;
        }
        #endregion
        private void button5_Click(object sender, EventArgs e)
        {
            string[] xx = new string[7];
            xx[0] = textBox10.Text.ToString();
            xx[1] = textBox11.Text.ToString();
            xx[2] = textBox12.Text.ToString();
            xx[3] = textBox13.Text.ToString();
            xx[4] = textBox14.Text.ToString();
            xx[5] = textBox15.Text.ToString();
            xx[6] = textBox16.Text.ToString();


            string sql = "(num1= '" + xx[0] + "' or num2='" + xx[0] + "' or num3 ='" + xx[0] + "' or num4 ='" + xx[0] + "' or num5='" + xx[0] + "' or num6='" + xx[0] + "') and " +
                         "(num1= '" + xx[1] + "' or num2='" + xx[1] + "' or num3 ='" + xx[1] + "' or num4 ='" + xx[1] + "' or num5='" + xx[1] + "' or num6='" + xx[1] + "') and " +
                         "(num1= '" + xx[2] + "' or num2='" + xx[2] + "' or num3 ='" + xx[2] + "' or num4 ='" + xx[2] + "' or num5='" + xx[2] + "' or num6='" + xx[2] + "') and " +
                         "(num1= '" + xx[3] + "' or num2='" + xx[3] + "' or num3 ='" + xx[3] + "' or num4 ='" + xx[3] + "' or num5='" + xx[3] + "' or num6='" + xx[3] + "') and " +
                         "(num1= '" + xx[4] + "' or num2='" + xx[4] + "' or num3 ='" + xx[4] + "' or num4 ='" + xx[4] + "' or num5='" + xx[4] + "' or num6='" + xx[4] + "') and " +
                         "(num1= '" + xx[5] + "' or num2='" + xx[5] + "' or num3 ='" + xx[5] + "' or num4 ='" + xx[5] + "' or num5='" + xx[5] + "' or num6='" + xx[5] + "') ";// and " +                         "(num7= '"+ xx[6]+"')";

            DataRow[] ddrow = dt1.Select(sql);

            DataTable dtnew = dt.Clone();//克隆结构
            for (int i = 0; i < ddrow.Length; i++)
            {
                dtnew.ImportRow(ddrow[i]);
            }
            label3.Text = "共计查询" + ddrow.Length.ToString() + "条记录!";
            dataGridView2.DataSource = dtnew;
            

        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }
        //public delegate void CallBackDelegate();//定义一个委托实现回调函数
        
        /// <summary>
        /// 回调函数
        /// </summary>
        private void CallBack()
        {
            textBox55.Text += "执行完成";
        }

        int iiSum = 0;
        public void mutilTask(string iiks)
        {
            int int_js, int_os, int_hz;
            
            int_js = int.Parse(textBox51.Text.ToString());
            int_os = int.Parse(textBox52.Text.ToString());
            int_hz = int.Parse(textBox53.Text.ToString());

            string[] sArray = iiks.Split('/');

            for (int i = int.Parse(sArray[0]); i < int.Parse(sArray[1]); i++)
            {
                Application.DoEvents();
                //progressBarX1.Value = i;
                //progressBarX1.Text = i.ToString() + "/" + dt1.Rows.Count.ToString();
                int[] xx = new int[7];
                xx[0] = int.Parse(dt1.Rows[i][0].ToString());
                xx[1] = int.Parse(dt1.Rows[i][1].ToString());
                xx[2] = int.Parse(dt1.Rows[i][2].ToString());
                xx[3] = int.Parse(dt1.Rows[i][3].ToString());
                xx[4] = int.Parse(dt1.Rows[i][4].ToString());
                xx[5] = int.Parse(dt1.Rows[i][5].ToString());
                xx[6] = int.Parse(dt1.Rows[i][6].ToString());


                int tmpJS = 0, tmpOS = 0;
                if (xx[0] % 2 == 0) { tmpOS++; } else tmpJS++;
                if (xx[1] % 2 == 0) { tmpOS++; } else tmpJS++;
                if (xx[2] % 2 == 0) { tmpOS++; } else tmpJS++;
                if (xx[3] % 2 == 0) { tmpOS++; } else tmpJS++;
                if (xx[4] % 2 == 0) { tmpOS++; } else tmpJS++;
                if (xx[5] % 2 == 0) { tmpOS++; } else tmpJS++;
                if (xx[6] % 2 == 0) { tmpOS++; } else tmpJS++;


                if (xx[0] + xx[1] + xx[2] + xx[3] + xx[4] + xx[5] + xx[6] == int_hz && tmpOS == int_os && tmpJS == int_js)
                {

                    if (dt4 != null)
                    {
                        lock (dt4)
                        {
                            DataRow dr = dt4.NewRow();
                            dr[0] = xx[0].ToString();
                            dr[1] = xx[1].ToString();
                            dr[2] = xx[2].ToString();
                            dr[3] = xx[3].ToString();
                            dr[4] = xx[4].ToString();
                            dr[5] = xx[5].ToString();
                            dr[6] = xx[6].ToString();
                            dt4.Rows.Add(dr);

                        }
                    }
                    //dataGridView3.DataSource = dt2;
                    iiSum++; //满足条件记录数

                }
            }

            //CallBackDelegate callBackDelegate = CallBackDelegate;
            //callBackDelegate();
            //dt4 = GetDistinctTable(dt4);

            //dataGridView4.DataSource = dt4;
            //textBox55.Text += "[" + sArray[0].ToString() + "]/[" + sArray[1].ToString() + "]" + "线程结束";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int int_js, int_os, int_hz , int_sum;
            int_js = int.Parse(textBox51.Text.ToString());
            int_os = int.Parse(textBox52.Text.ToString());
            int_hz = int.Parse(textBox53.Text.ToString());

            //整除
            int_sum = int.Parse(textBox54.Text.ToString());

            //MessageBox.Show(int_sum.ToString() + "/" + (int_sum / 10).ToString() + "/" + (int_sum % 10).ToString());
            int zc, ys; //
            zc = int_sum / 10;
            ys = int_sum % 10;

            int ks, jz;
            try
            {
                lock (dt4)
                {
                    dt4.Rows.Clear();
                }
            }
            catch { }

            label45.Text = "";
            iiSum = 0;
            
            for (int i = 0; i < 10; i++)
            {
                ks= zc * i;
                jz= zc * (i + 1);
                int iiks = (ks - 1 < 0) ? 0 : ks - 1;
                //CallBackDelegate cbd = CallBack;
                Thread t = new Thread(() =>
                {
                    //dt4 = GetDistinctTable(dt4);

                    mutilTask(iiks.ToString() + "/" + jz.ToString());
                    
                });
                string txt = "[" + iiks.ToString() + "]/[" + jz.ToString() + "]";
                label45.Text += txt;
                t.Start();
                t.Join();
            }
            if (dt4 != null)
            {
                lock (dt4)
                {
                    dt4 = GetDistinctTable(dt4);
                }
            }
            dataGridView4.DataSource = dt4;
            textBox55.Text = iiSum.ToString() + "条记录";

            //ks[10] = jz[9];
            //jz[10] = int_sum;
            //int int_js, int_os, int_hz;
            //int iiSum = 0;
            //dt4.Rows.Clear();
            //int_js = int.Parse(textBox51.Text.ToString());
            //int_os = int.Parse(textBox52.Text.ToString());
            //int_hz = int.Parse(textBox53.Text.ToString());
            //progressBarX1.Maximum = dt1.Rows.Count;
            //for (int i = 0; i < int.Parse(textBox54.Text.ToString()); i++)
            //{
            //    Application.DoEvents();
            //    progressBarX1.Value = i;
            //    progressBarX1.Text = i.ToString() + "/" + dt1.Rows.Count.ToString();
            //    int[] xx = new int[7];
            //    xx[0] = int.Parse(dt1.Rows[i][0].ToString());
            //    xx[1] = int.Parse(dt1.Rows[i][1].ToString());
            //    xx[2] = int.Parse(dt1.Rows[i][2].ToString());
            //    xx[3] = int.Parse(dt1.Rows[i][3].ToString());
            //    xx[4] = int.Parse(dt1.Rows[i][4].ToString());
            //    xx[5] = int.Parse(dt1.Rows[i][5].ToString());
            //    xx[6] = int.Parse(dt1.Rows[i][6].ToString());


            //    int tmpJS = 0, tmpOS = 0;
            //    if (xx[0] % 2 == 0) { tmpOS++; } else tmpJS++;
            //    if (xx[1] % 2 == 0) { tmpOS++; } else tmpJS++;
            //    if (xx[2] % 2 == 0) { tmpOS++; } else tmpJS++;
            //    if (xx[3] % 2 == 0) { tmpOS++; } else tmpJS++;
            //    if (xx[4] % 2 == 0) { tmpOS++; } else tmpJS++;
            //    if (xx[5] % 2 == 0) { tmpOS++; } else tmpJS++;
            //    if (xx[6] % 2 == 0) { tmpOS++; } else tmpJS++;


            //    if (xx[0] + xx[1] + xx[2] + xx[3] + xx[4] + xx[5] + xx[6] == int_hz && tmpOS == int_os && tmpJS == int_js)
            //    {

            //        DataRow dr = dt4.NewRow();
            //        dr[0] = xx[0].ToString();
            //        dr[1] = xx[1].ToString();
            //        dr[2] = xx[2].ToString();
            //        dr[3] = xx[3].ToString();
            //        dr[4] = xx[4].ToString();
            //        dr[5] = xx[5].ToString();
            //        dr[6] = xx[6].ToString();
            //        dt4.Rows.Add(dr);
            //        //dataGridView3.DataSource = dt2;
            //        iiSum++; //满足条件记录数
            //    }
            //}
            //try
            //{
            //    dt4 = GetDistinctTable(dt4);

            //    dataGridView4.DataSource = dt4;

            //    //label45.Text = "共计" + iiSum.ToString() + "条数据满足条件." + "去重复数据后:" + dt4.Rows.Count.ToString();
            //}
            //catch
            //{
            //    label45.Text = "无满足条件数据";
            //}



        }

        private void mutilTask()
        {
            throw new NotImplementedException();
        }

        private void dataGridView4_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            try
            {
                SolidBrush b = new SolidBrush(dataGridView4.RowHeadersDefaultCellStyle.ForeColor);
                e.Graphics.DrawString((e.RowIndex + 1).ToString(System.Globalization.CultureInfo.CurrentUICulture), dataGridView4.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = dt1;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string dbName, Sa, PassWord, database;
            dbName = ".";
            Sa = "sa";
            PassWord = "hejc190";
            database = "cp";

            Conn = new OleDbConnection();
            Conn.ConnectionString = "Provider=sqloledb;Data Source=" + dbName.ToString().Trim() +
                                        ";Initial Catalog=" + database.Trim() + ";" +
                                        " User Id=" + Sa.ToString().Trim() +
                                        ";Password=" + PassWord.ToString().Trim() + ";Max Pool Size = 512;" +
                                        "Connection Timeout=50000";
            if (Conn.State == ConnectionState.Closed)
            {
                try
                {
                   // Conn.ConnectionTimeout = 300000;
                    Conn.Open();
                    ConnValue = Conn;
                }
                catch
                {
                    MessageBox.Show("连接数据库失败！");
                }
                
            }
        }

        public void ConnClose()
        {
            if (ConnValue.State == ConnectionState.Open)
            {
                ConnValue.Close();
                //th_wait.Abort();
            }
        }

        public Boolean Fzpcf(int[] hm,int[] grouphm,int findCount)
        {
            int bgzCount = 0;
            for (int cc = 0; cc < grouphm.Length; cc++)
            {
                int tmp = Array.IndexOf(hm, grouphm[cc]);
                if (tmp >= 0) bgzCount++;
            }
            if (bgzCount >= findCount)
            {
                return false;
            }
            else { return true; }
        }
        public Boolean Fzpcf(int[] hm, int[] grouphm, int findCount,int maxnum)
        {
            int bgzCount = 0;

            int[] numtmp = new int[maxnum];
            for (int i = 0; i < maxnum; i++)
            {
                numtmp[i] = hm[i];
            }

 
            for (int cc = 0; cc < grouphm.Length; cc++)
            {
                int tmp = Array.IndexOf(numtmp, grouphm[cc]);
                if (tmp >= 0) bgzCount++;
            }
            if (bgzCount >= findCount)
            {
                return false;
            }
            else { return true; }
        }
        public Boolean FzpcfArray(int[] hm,int[,] grouphm,int findCount)
        {
            int bgzCount = 0;
            for (int cc = 0; cc < grouphm.GetLength(0); cc++)//可以理解为：第一维的长度（即行数），结果为2
            {
                for (int nn = 0; nn < grouphm.GetLength(1); nn++)//可以理解为：第二维的长度（即列数），结果为5
                {
                    int tmp = Array.IndexOf(hm, grouphm[cc, nn]);
                    if (tmp != -1) bgzCount++;
                }
                if (bgzCount >= findCount) { break; }
                else { bgzCount = 0; }
            }
            if (bgzCount >= findCount)
            {
                return false;
            }
            else { return true; }
        }

        public Boolean FzpcfArray(int[] hm, int[,] grouphm, int findCount,int maxnum)
        {
            int bgzCount = 0;
            int[] numtmp = new int[maxnum];
            for (int i = 0; i < maxnum; i++)
            {
                numtmp[i] = hm[i];
            }
            for (int cc = 0; cc < grouphm.GetLength(0); cc++)//可以理解为：第一维的长度（即行数），结果为2
            {
                
                for (int nn = 0; nn < grouphm.GetLength(1); nn++)//可以理解为：第二维的长度（即列数），结果为5
                {
                    int tmp = Array.IndexOf(numtmp, grouphm[cc, nn]);
                    if (tmp != -1) bgzCount++;
                }
                if (bgzCount >= findCount) { break; }
                else { bgzCount = 0; }
            }
            if (bgzCount >= findCount)
            {
                return false;
            }
            else { return true; }
        }

        public int[] randomList(int[] a)
        {
            int[] b = new int[33];//保存a随机排序后的数据
            Random rand = new Random();
            ArrayList list = new ArrayList();
            for (int j = 0; j < 33; j++)
            {
                list.Add(a[j]);
            }
            //随机存入数据
            for (int i = 33; i > 0; i--)
            {
                int c = rand.Next(0, i);//产生随机数
                b[i - 1] = (int)list[c];//随机选择一个数
                list.Remove(list[c]);//移除已经选择过的数
            }
            return b;
        }


        /// <summary>
        /// 定义一个委托
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        public delegate void setTextCallback(string text);

        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        public void setText(string txt)
        {
            if (label49.InvokeRequired)
            {
                setTextCallback s = new setTextCallback(setText);
                Invoke(s, new string[] { txt });
            }
            else
            {
                label49.Text = txt;
            }
        }
        int ljCurrentRecno = 0;


        public int[] hmgzfx(int[] iirows,int[] jl)
        {
            Boolean ibTrue = false;
            string ibTrueInfo = "";
            while (true)
            {
                Application.DoEvents();
                int cgcs = 0; //

                //八卦阵判断
                ibTrue = FzpcfArray(iirows, bgz, 3, 6);
                if (ibTrue == true) cgcs++; else ibTrueInfo += "[八卦阵未通过]";

                //集团码,若随机的号码里面组合有5个与该下列号码相同，就排除。
                ibTrue = Fzpcf(iirows, jthm, 5, 6);
                if (ibTrue == true) cgcs++; else ibTrueInfo += "[集团码未通过]";

                //万能码,如果你选择的号码里面没有以下号码其中之一，就排除。
                ibTrue = Fzpcf(iirows, wlm, 1, 6);
                if (ibTrue == false) cgcs++; else ibTrueInfo += "[万能码未通过]";

                //神7数字
                ibTrue = Fzpcf(iirows, s7sz, 1, 6);
                if (ibTrue == false) cgcs++; else ibTrueInfo += "[神7数字未通过]";

                //质数
                ibTrue = Fzpcf(iirows, zshm, 1, 6);
                if (ibTrue == false) cgcs++; else ibTrueInfo += "[质数未通过]";

                //2,3,6,7,9尾
                ibTrue = Fzpcf(iirows, w23679w, 1, 6);
                if (ibTrue == false) cgcs++; else ibTrueInfo += "[23679尾未通过]";

                //分组排除法
                // hm1
                ibTrue = FzpcfArray(iirows, hm1, 4, 6);
                if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm1未通过]";

                //hm2
                ibTrue = Fzpcf(iirows, hm2, 4, 6);
                if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm2未通过]";

                //hm3
                ibTrue = FzpcfArray(iirows, hm3, 3, 6);
                if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm3未通过]";

                //hm4
                ibTrue = FzpcfArray(iirows, hm4, 5, 6);
                if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm4未通过]";

                //hm5
                ibTrue = FzpcfArray(iirows, hm5, 5, 6);
                if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm5未通过]";

                //hm6
                ibTrue = Fzpcf(iirows, hm6, 5, 6);
                if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm6未通过]";

                //hm7
                ibTrue = FzpcfArray(iirows, hm7, 5, 6);
                if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm7未通过]";

                //hm8
                ibTrue = Fzpcf(iirows, hm8, 5, 6);
                if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm8未通过]";

                //hm9
                ibTrue = FzpcfArray(iirows, hm9, 5, 6);
                if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm9未通过]";

                //在我选择的15号码里面去匹配
                //ibTrue = FzpcfArray(iirows, hm11, 6);
                //int[] tmp = { 1, 3, 4, 5, 7, 9, 11, 13, 14, 15, 17, 19, 22, 23, 25, 28, 30, 31, 32, 33 };
                //int[] tmp1 = { 2, 6, 8, 10, 12, 16, 18, 20, 21, 24, 26, 27, 29 };
                //int[] tmp2 = { 3,4,7,11,12,15,17,24,27,29,30,33 };  //12码
                //int[] tmp3 = { 1,3,4,8,12,13,17,18,24,25,28,32 };//12码
                int[] tmp3 = { 1, 2, 3, 4, 5, 7, 10, 13, 14, 15, 17, 19, 23, 26, 27, 30, 33 };  //16号中一等奖一期
                ibTrue = Fzpcf(iirows, tmp3, 5, 6);
                if (ibTrue == false) cgcs++; else ibTrueInfo += "[12码未通过]";

                //历史号码比对
                ibTrue = FzpcfArray(iirows, hm10, 5, 6);
                if (ibTrue == true) cgcs++;
                else
                {
                    //listBox1.Items.Add(">>>>>>>> 历史中奖 >>>>>>>" + string.Join(",", iirows));
                    ibTrueInfo += "[历史号码未通过]";
                }

                //if (string.Join(",", iirows) == "1,4,13,18,26,30,3")
                //{
                //    MessageBox.Show("stop");
                //}

                if (cgcs != 17) ibTrue = false;
                else
                {
                    ////号码出现机率要达到平均值
                    //string sqlsum = string.Format("select sum(bfl) from cpp_bfl where num in ({0},{1},{2},{3},{4},{5},{6})",
                    //                            iirows[0], iirows[1], iirows[2], iirows[3], iirows[4], iirows[5], iirows[6]);
                    ////Conn.Open();
                    ////ConnValue = Conn;
                    //OleDbCommand mysum = new OleDbCommand(sqlsum, ConnValue);
                    //decimal decimal_mysumresult = (decimal)mysum.ExecuteScalar();
                    //ConnClose();
                    decimal decimal_mysumresult = bfltable[(int)GetIndexOfElement(bfltable, iirows[0])][1] +
                                                  bfltable[(int)GetIndexOfElement(bfltable, iirows[1])][1] +
                                                  bfltable[(int)GetIndexOfElement(bfltable, iirows[2])][1] +
                                                  bfltable[(int)GetIndexOfElement(bfltable, iirows[3])][1] +
                                                  bfltable[(int)GetIndexOfElement(bfltable, iirows[4])][1] +
                                                  bfltable[(int)GetIndexOfElement(bfltable, iirows[5])][1] +
                                                  bfltable[(int)GetIndexOfElement(bfltable, iirows[6])][1];

                    //decimal n1 = bfltable[(int)GetIndexOfElement(bfltable, iirows[0])][1];
                    //decimal n2 = bfltable[(int)GetIndexOfElement(bfltable, iirows[1])][1];
                    //decimal n3 = bfltable[(int)GetIndexOfElement(bfltable, iirows[2])][1];
                    //decimal n4 = bfltable[(int)GetIndexOfElement(bfltable, iirows[3])][1];
                    //decimal n5 = bfltable[(int)GetIndexOfElement(bfltable, iirows[4])][1];
                    //decimal n6 = bfltable[(int)GetIndexOfElement(bfltable, iirows[5])][1];
                    //decimal n7 = 
                    decimal decimal_myavg = decimal.Parse(label50.Text.ToString());
                    decimal OK_myMAX = decimal_myavg + (decimal)0.025; //0.012
                    decimal OK_myMIN = decimal_myavg - (decimal)0.025;

                    if (decimal_mysumresult > OK_myMIN && decimal_mysumresult < OK_myMAX)
                    {
                        //ibTrue = true;
                        //int[] xm = { 1, 7, 14, 15, 16, 27, 28, 32, 33 };
                        //ibTrue = Fzpcf(iirows, zbh, 3, 6);
                        //if (ibTrue == false) ibTrue = true; else ibTrue = false;
                        int[] xxnumtmp = new int[16];
                        int[] xxnumtmp1 = new int[16];
                        int[] xxnumtmp2 = new int[16];
                        int[] xxnumtmp3 = new int[12];
                        int[] left5w = new int[5]; //前五位出现机率最大的号
                        int[] right5w = new int[5]; //最后五位出现机率最小的号
                        int result = 0;
                        Array.Copy(zbh, 0, xxnumtmp, 0, 16);
                        Array.Copy(zbh, 5, xxnumtmp1, 0, 16);
                        Array.Copy(zbh, 8, xxnumtmp2, 0, 16);
                        Array.Copy(zbh, 21, xxnumtmp3, 0, 12);

                        Array.Copy(zbh, 0, left5w, 0, 5);
                        Array.Copy(zbh, 28, right5w, 0, 5);

                        if (Fzpcf(iirows, xxnumtmp, jl[0], 6) == false) result++;
                        if (Fzpcf(iirows, xxnumtmp1, jl[1], 6) == false) result++;
                        if (Fzpcf(iirows, xxnumtmp2, jl[2], 6) == false) result++;
                        if (Fzpcf(iirows, xxnumtmp3, jl[3], 6) == false) result++;


                        int[][] xm1 =
                        {
                                    new int[9]{1,2,3,4,5,6,7,8,9},
                                    new int[10]{10,11,12,13,14,15,16,17,18,19},
                                    new int[10]{20,21,22,23,24,25,26,27,28,29},
                                    new int[4]{30,31,32,33}
                                };
                        //if (Fzpcf(iirows, xm1[0], 1, 6) == false) result++;
                        //if (Fzpcf(iirows, xm1[1], 3, 6) == false) result++;
                        //if (Fzpcf(iirows, xm1[2], 1, 6) == false) result++;
                        //if (Fzpcf(iirows, xm1[3], 1, 6) == false) result++;

                        //判断：一组号码里有三个0-9小号的，取消
                        if (Fzpcf(iirows, xm1[0], 3, 6) == true) result++;
                        //判断：一组号码里有四个10-19号的，取消
                        if (Fzpcf(iirows, xm1[1], 4, 6) == true) result++;
                        //判断：一组号码里有四个20-29号的，取消
                        if (Fzpcf(iirows, xm1[2], 4, 6) == true) result++;
                        //判断：一组号码里有三个30-33号的，取消
                        if (Fzpcf(iirows, xm1[3], 3, 6) == true) result++;
                        //判断：一组号码里有三个或以上连续的号码，取消
                        if (ContinueNumLenth(iirows) < 3) result++;
                        //判断：一组号码里有二组或以上连续的号码，取消
                        //如  1，2，4，5，16，18+11 ，1-2，4-5就是二组连续号码
                        if (QueueString(iirows) < 2) result++;

                        if (Fzpcf(iirows, left5w, 1, 6) == false) result++;   //前五位出现机率至少1个号码
                        if (Fzpcf(iirows, right5w, 1, 6) == false) result++;  //后五位出现机率至少1个号码  

                        int hmsum = iirows[0] + iirows[1] + iirows[2] + iirows[3] + iirows[4] + iirows[5];
                        //if (hmsum > 70 && hmsum < 110) result++;

                        if (result == 12) ibTrue = true; else ibTrue = false;


                    }
                    else
                    {
                        ibTrue = false;
                        ibTrueInfo += decimal_mysumresult.ToString() + "出现机率未通过";

                    }
                }
                //if (iirows[0] > 10) ibTrue = false;
                break;
            }

            //listBox1.Items.Add(string.Join(",", iirows) + ibTrueInfo);
            if (ibTrue == true)
            {
                return iirows;
                //iirows[6] = 0;
                //DataRow dr = dtSet.NewRow();
                //dr["num1"] = iirows[0];
                //dr["num2"] = iirows[1];
                //dr["num3"] = iirows[2];
                //dr["num4"] = iirows[3];
                //dr["num5"] = iirows[4];
                //dr["num6"] = iirows[5];
                //dr["num7"] = iirows[6];
                //dr["num"] = string.Format("{0:00}", iirows[0]) + "," + string.Format("{0:00}", iirows[1]) + "," +
                //            string.Format("{0:00}", iirows[2]) + "," + string.Format("{0:00}", iirows[3]) + "," +
                //            string.Format("{0:00}", iirows[4]) + "," + string.Format("{0:00}", iirows[5]) + "+" +
                //            string.Format("{0:00}", iirows[6]);
                ////dr["cf"] = ds.Tables["cppbase"].Rows[i]["cf"].ToString();
                //dtSet.Rows.Add(dr);
                ////label48.Text = "共计满足条件号码有[" + dt_save.Rows.Count.ToString() + "]条.";
                ////listBox1.Items.Add(dr["num"]);
                //return dtSet;
            }
            else return null;
            
        }


        public DataTable doworkfx(DataTable dtsource,int[] wz)
        {
            DataTable dt_save = new DataTable();
            dt_save.Columns.Add("num1", typeof(string));
            dt_save.Columns.Add("num2", typeof(string));
            dt_save.Columns.Add("num3", typeof(string));
            dt_save.Columns.Add("num4", typeof(string));
            dt_save.Columns.Add("num5", typeof(string));
            dt_save.Columns.Add("num6", typeof(string));
            dt_save.Columns.Add("num7", typeof(string));
            dt_save.Columns.Add("num", typeof(string));
            dt_save.Columns.Add("cf", typeof(string));
            dt_save.Columns.Add("numsum", typeof(string));
            dt_save.Columns.Add("js", typeof(string));

            for (int i = wz[0]; i < wz[1]; i++)
            {
                int[] iirows = new int[7];
                if (fxstop == true) break;
                iirows[0] = int.Parse(dtsource.Rows[i]["num1"].ToString());
                iirows[1] = int.Parse(dtsource.Rows[i]["num2"].ToString());
                iirows[2] = int.Parse(dtsource.Rows[i]["num3"].ToString());
                iirows[3] = int.Parse(dtsource.Rows[i]["num4"].ToString());
                iirows[4] = int.Parse(dtsource.Rows[i]["num5"].ToString());
                iirows[5] = int.Parse(dtsource.Rows[i]["num6"].ToString());
                iirows[6] = int.Parse(dtsource.Rows[i]["num7"].ToString());
                //号码规则比较
                iirows = hmgzfx(iirows,new int[]{ 4,3,2,1});

                if (iirows != null)
                {
                    DataRow dr = dt_save.NewRow();
                    dr["num1"] = iirows[0];
                    dr["num2"] = iirows[1];
                    dr["num3"] = iirows[2];
                    dr["num4"] = iirows[3];
                    dr["num5"] = iirows[4];
                    dr["num6"] = iirows[5];
                    dr["num7"] = iirows[6];
                    dr["num"] = string.Format("{0:00}", iirows[0]) + "," + string.Format("{0:00}", iirows[1]) + "," +
                                string.Format("{0:00}", iirows[2]) + "," + string.Format("{0:00}", iirows[3]) + "," +
                                string.Format("{0:00}", iirows[4]) + "," + string.Format("{0:00}", iirows[5]) + "+" +
                                string.Format("{0:00}", iirows[6]);
                    //dr["cf"] = ds.Tables["cppbase"].Rows[i]["cf"].ToString();
                    dt_save.Rows.Add(dr);
                }
            }
            return dt_save;
        }


        public DataTable doworkfx(int[] cf)
        {
            DataTable dt_save = new DataTable();
            DataTable dt_save1 = new DataTable(); //被取消的信息
            DataSet ds = new DataSet();
            dt_save.Columns.Add("num1", typeof(string));
            dt_save.Columns.Add("num2", typeof(string));
            dt_save.Columns.Add("num3", typeof(string));
            dt_save.Columns.Add("num4", typeof(string));
            dt_save.Columns.Add("num5", typeof(string));
            dt_save.Columns.Add("num6", typeof(string));
            dt_save.Columns.Add("num7", typeof(string));
            dt_save.Columns.Add("num", typeof(string));
            dt_save.Columns.Add("cf", typeof(string));
            dt_save.Columns.Add("numsum", typeof(string));
            dt_save.Columns.Add("js", typeof(string));
            //int maxcf = 40;
            //int mincf = 1;
            label48.Text = "当前无数据.";
            int ljcount = 0;



            for (int io = cf[0]; io <= cf[1]; io++)
            {
                Application.DoEvents();
                ds.Clear();
                //取数据库到datatable
                //string sql = "select num1,num2,num3,num4,num5,num6 from cpp_base where cf = "+cf.ToString()+" and js>70";
                string sql = "select * from cpp_base where cf=" + io.ToString();// (num1 =2 and num2=15 and num3=22 and num4=26 and num5=30 and num6=33)"; // cf = " + cf.ToString() 
                                                                                //string sql = "select * from cpp_6hm ";
                                                                                //Conn.Open();
                                                                                //ConnValue = Conn;
                                                                                // string sql = "select * from cpp_6hm";
                OleDbDataAdapter da = new OleDbDataAdapter(sql, ConnValue);
                da.Fill(ds, "cppbase");
                //ConnClose();

                //if (fxstop == true) break;

                ljcount += ds.Tables["cppbase"].Rows.Count;



                for (int i = 0; i < ds.Tables["cppbase"].Rows.Count; i++)
                {
                    ljCurrentRecno++;

                    //this.Invoke(new Action(() =>
                    //        label49.Text = "正在执行重复机率[" + io.ToString() + "][" + ljcount + "]条数据.[" + ljCurrentRecno + "]"
                    //));
                    //lb49.Text = "正在处理第[" + ljCurrentRecno + "]条记录."; //"正在执行重复机率[" + io.ToString() + "]";//[" + ljcount + "]条数据.[" + ljCurrentRecno + "]";
                    //setText("正在执行重复机率[" + cf.ToString() + "][" + ljcount + "]条数据.[" + ljCurrentRecno + "]");
                    int[] iirows = new int[7];
                    Boolean ibTrue = false;
                    string ibTrueInfo = "";

                    if (fxstop == true) break;

                    iirows[0] = int.Parse(ds.Tables["cppbase"].Rows[i]["num1"].ToString());
                    iirows[1] = int.Parse(ds.Tables["cppbase"].Rows[i]["num2"].ToString());
                    iirows[2] = int.Parse(ds.Tables["cppbase"].Rows[i]["num3"].ToString());
                    iirows[3] = int.Parse(ds.Tables["cppbase"].Rows[i]["num4"].ToString());
                    iirows[4] = int.Parse(ds.Tables["cppbase"].Rows[i]["num5"].ToString());
                    iirows[5] = int.Parse(ds.Tables["cppbase"].Rows[i]["num6"].ToString());
                    iirows[6] = int.Parse(ds.Tables["cppbase"].Rows[i]["num7"].ToString());

                    //号码规则比较
                    iirows = hmgzfx(iirows,new int[] { 3,3,3,1});
                    if (iirows!= null)
                    {
                        DataRow dr = dt_save.NewRow();
                        dr["num1"] = iirows[0];
                        dr["num2"] = iirows[1];
                        dr["num3"] = iirows[2];
                        dr["num4"] = iirows[3];
                        dr["num5"] = iirows[4];
                        dr["num6"] = iirows[5];
                        dr["num7"] = iirows[6];
                        dr["num"] = string.Format("{0:00}", iirows[0]) + "," + string.Format("{0:00}", iirows[1]) + "," +
                                    string.Format("{0:00}", iirows[2]) + "," + string.Format("{0:00}", iirows[3]) + "," +
                                    string.Format("{0:00}", iirows[4]) + "," + string.Format("{0:00}", iirows[5]) + "+" +
                                    string.Format("{0:00}", iirows[6]);
                        //dr["cf"] = ds.Tables["cppbase"].Rows[i]["cf"].ToString();
                        dt_save.Rows.Add(dr);
                    }

                    //dr["num"] = string.Format("{0:00}", iirows[0]) + "," + string.Format("{0:00}", iirows[1]) + "," +
                    //            string.Format("{0:00}", iirows[2]) + "," + string.Format("{0:00}", iirows[3]) + "," +
                    //            string.Format("{0:00}", iirows[4]) + "," + string.Format("{0:00}", iirows[5]) + "+" +
                    //            string.Format("{0:00}", iirows[6]);
                    //dr["cf"] = ds.Tables["cppbase"].Rows[i]["cf"].ToString();
                    //iirows[6] = 3;
                    //iirows[6] = int.Parse(dt_bb.Rows[i]["num7"].ToString());
                    //lb1.Text = "[" + ds.Tables["cppbase"].Rows.Count.ToString() +
                    //               "]正在分析第[" + i.ToString() + "]— [" +
                    //              iirows[0].ToString() + "][" +
                    //              iirows[1].ToString() + "][" +
                    //              iirows[2].ToString() + "][" +
                    //              iirows[3].ToString() + "][" +
                    //              iirows[4].ToString() + "][" +
                    //              iirows[5].ToString() + "][" +
                    //              iirows[6].ToString() + "]";

                    //else
                    //{
                    //    DataRow dr = dt_save1.NewRow();
                    //    dr["num1"] = iirows[0];
                    //    dr["num2"] = iirows[1];
                    //    dr["num3"] = iirows[2];
                    //    dr["num4"] = iirows[3];
                    //    dr["num5"] = iirows[4];
                    //    dr["num6"] = iirows[5];
                    //    dr["num7"] = iirows[6];
                    //    dr["num"] = string.Format("{0:00}", iirows[0]) + "," + string.Format("{0:00}", iirows[1]) + "," +
                    //                string.Format("{0:00}", iirows[2]) + "," + string.Format("{0:00}", iirows[3]) + "," +
                    //                string.Format("{0:00}", iirows[4]) + "," + string.Format("{0:00}", iirows[5]) + "+" +
                    //                string.Format("{0:00}", iirows[6]);
                    //    dr["cf"] = ds.Tables["cppbase"].Rows[i]["cf"].ToString();
                    //    dr["qxinfo"] = ibTrueInfo;
                    //    dt_save1.Rows.Add(dr);
                    //}
                }
                //datatable分组汇总
                //DataTable dt = dt_save.AsEnumerable().GroupBy(
                //    r => new { num1 = r["num1"], num2 = r["num2"], num3 = r["num3"] }).Select(
                //    g =>
                //    {
                //        var row = dt_save.NewRow();
                //        //row["num"] = g.Key.num1 + "," + g.Key.num2 + "," + g.Key.num3;// + "," + g.Key.num4 + "," + g.Key.num5 + "," + g.Key.num6;
                //        return row;
                //    }).CopyToDataTable();
                //dataGridView5.DataSource = dt_save;
                //lb2.Text = "共计满足条件号码有[" + dt_save.Rows.Count.ToString() + "]条.";
                //WriteTextFile(dt_save, @"d:\CP.txt");
                //WriteTextFile(dt_save.DefaultView.ToTable(true,"num"), @"d:\CP.txt");
                //WriteTextFile(dt_save1, @"d:\CPqxinfo.txt");
            }

            return dt_save;
        }
    

        private void button10_Click(object sender, EventArgs e)
        {
            DataTable dt_bb = new DataTable();
            DataTable dt_save = new DataTable();
            DataTable dt_save1 = new DataTable(); //被取消的信息
            DataSet ds = new DataSet();
            dt_save.Columns.Add("num1", typeof(string));
            dt_save.Columns.Add("num2", typeof(string));
            dt_save.Columns.Add("num3", typeof(string));
            dt_save.Columns.Add("num4", typeof(string));
            dt_save.Columns.Add("num5", typeof(string));
            dt_save.Columns.Add("num6", typeof(string));
            dt_save.Columns.Add("num7", typeof(string));
            dt_save.Columns.Add("num", typeof(string));
            dt_save.Columns.Add("cf", typeof(string));
            dt_save.Columns.Add("numsum", typeof(string));
            dt_save.Columns.Add("js", typeof(string));

            dt_save1.Columns.Add("num1", typeof(string));
            dt_save1.Columns.Add("num2", typeof(string));
            dt_save1.Columns.Add("num3", typeof(string));
            dt_save1.Columns.Add("num4", typeof(string));
            dt_save1.Columns.Add("num5", typeof(string));
            dt_save1.Columns.Add("num6", typeof(string));
            dt_save1.Columns.Add("num7", typeof(string));
            dt_save1.Columns.Add("num", typeof(string));
            dt_save1.Columns.Add("cf", typeof(string));
            dt_save1.Columns.Add("numsum", typeof(string));
            dt_save1.Columns.Add("js", typeof(string));


            int maxcf = 40;
            int mincf = 1;
            label48.Text = "当前无数据.";
            int ljcount = 0;
            int ljCurrentRecno = 0;

            listBox1.Items.Add("开始分析：[" + DateTime.Now.ToString() + "].");

            int[][] xx = new int[][]
            {
                //new int[2]{13,14},
                //new int[2]{15,16},
                //new int[2]{17,18},
                //new int[2]{19,20},
                //new int[2]{21,24},
                //new int[2]{25,50},
                //new int[2]{1,12},

                //new int[2]{0,0}

                //new int[2]{31,50},
                //new int[2]{21,24},
                //new int[2]{25,30},
                //new int[2]{1,12},
                new int[2]{30,50},
                new int[2]{33,70},
                new int[2]{71,75},
                new int[2]{76,78},
                new int[2]{79,82},
                new int[2]{83,85},
                new int[2]{86,89},
                new int[2]{90,92},
                new int[2]{93,95},
                new int[2]{96,98},
                new int[2]{99,105},
                new int[2]{106,110},
                new int[2]{111,160}
            };
            Parallel.For(0, xx.Length, i =>
            {
                object ob_Label;
                ob_Label = this.GetType().GetField("label49", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(this);



                listBox2.Items.Add("正在执行[" + i.ToString() + "重复率的数据.");
                //doworkfx(i)
                Application.DoEvents();
                DataTable ds_savexx = doworkfx(xx[i]);
                //object ob_Label;
                //ob_Label = this.GetType().GetField("progressBarX" + (i + 1).ToString(), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(this);
                //doWork(1, zc / 3, (DevComponents.DotNetBar.Controls.ProgressBarX)ob_Label);
                WriteTextFile(ds_savexx.DefaultView.ToTable(true, "num"), @"c:\\cp.txt", true);
                listBox2.Items.Add("结束[" + i.ToString() + "重复率的分析."+ ds_savexx.Rows.Count.ToString());
                listBox2.TopIndex = listBox2.Items.Count -1;
                foreach(DataRow dr in ds_savexx.Rows)
                {
                    dt_save.Rows.Add(dr.ItemArray);
                }
            });


            //在已生成的号码里面重新按以下规则进行筛选
            //先进行号码出现率的分析
            DataRow[] drTemp = dt_save.Select();
            var listTmp = drTemp.Select(x => x.Field<string>("num")).ToArray();

            object[,] num = datatabletoarry(dt_save);

            decimal[][] xx11 = new decimal[num.GetLength(0)][];
            for (int i = 0; i < num.GetLength(0); i++)
            {
                xx11[i] = new decimal[num.GetLength(1)];
                for (int cc = 0; cc < num.GetLength(1); cc++)
                {
                    xx11[i][cc] = decimal.Parse(num[i, cc].ToString());
                }
            }

            decimal[] build = new decimal[num.GetLength(0) * (num.GetLength(1))];
            int buildint = 0;
            for (int co = 0; co < xx11.GetLength(0); co++)
            {
                for (int co1 = 0; co1 < num.GetLength(1); co1++)
                {
                    build[buildint] = xx11[co][co1];
                    buildint++;
                }

            }
            // 集合 dic 用于存放统计结果
            Dictionary<int, ItemInfo> dic =
                new Dictionary<int, ItemInfo>();
            Dictionary<int, ItemInfo> dic1 =
               new Dictionary<int, ItemInfo>();
            // 开始统计每个元素重复次数
            foreach (int v in build)
            {
                if (dic.ContainsKey(v))
                {
                    // 数组元素再次，出现次数增加 1
                    dic[v].RepeatNum += 1;
                }
                else
                {
                    // 数组元素首次出现，向集合中添加一个新项
                    // 注意 ItemInfo类构造函数中，已经将重复
                    // 次数设置为 1
                    dic.Add(v, new ItemInfo(v));
                }
            }

            var tt = dic.OrderByDescending(r => r.Value.RepeatNum);

            List<int> TmpZbh = new List<int>();

            for (int i = 0; i < tt.Count(); i++)
            {
                KeyValuePair<int, ItemInfo> kv = tt.ElementAt(i);

                //**** 转换每个号码的出现百分率 ****
                decimal n1 = kv.Value.RepeatNum;
                decimal n2 = (xx11.GetLength(0) * 7);

                decimal bfltmp = n1 / n2;
                //string sql = "update cpp_bfl set bfl=" + bfltmp.ToString() +
                //             " where num=" + kv.Key.ToString();
                //OleDbCommand mysql = new OleDbCommand(sql, ConnValue);
                //mysql.ExecuteNonQuery();

                bfltable[i][0] = kv.Key;
                bfltable[i][1] = bfltmp;
                //**********************************
            }

            //OleDbCommand mydel = new OleDbCommand("delete from cpp_lskjhm", ConnValue);
            //mydel.ExecuteNonQuery();

            decimal sumbfl = 0.0000M;
            for (int i = 0; i < xx11.GetLength(0); i++)
            {
                decimal bfl = 0.0000M;
                for (int c = 0; c < bfltable.GetLength(0); c++)
                {
                    if (xx11[i][0] == bfltable[c][0]) bfl += bfltable[c][1];
                    if (xx11[i][1] == bfltable[c][0]) bfl += bfltable[c][1];
                    if (xx11[i][2] == bfltable[c][0]) bfl += bfltable[c][1];
                    if (xx11[i][3] == bfltable[c][0]) bfl += bfltable[c][1];
                    if (xx11[i][4] == bfltable[c][0]) bfl += bfltable[c][1];
                    if (xx11[i][5] == bfltable[c][0]) bfl += bfltable[c][1];
                }
                sumbfl += bfl;
                //xx11[i][7] = bfl;
                //decimal bfl = 0.0000M;
                //int[] hmlist = { xx11[i][0], xx11[i][1], xx11[i][2], xx11[i][3], xx11[i][4], xx11[i][5], xx11[i][6] };
                //string sql = string.Format("select sum(bfl) from cpp_bfl where num in ({0},{1},{2},{3},{4},{5},{6})",
                //                hmlist[0], hmlist[1], hmlist[2], hmlist[3], hmlist[4], hmlist[5], hmlist[6]);


                //OleDbCommand mysql = new OleDbCommand(sql, ConnValue);
                //bfl = (decimal)mysql.ExecuteScalar();

                //string sql1 = string.Format("insert into cpp_lskjhm values ({0},{1},{2},{3},{4},{5},{6},{7})",
                //                            hmlist[0], hmlist[1], hmlist[2], hmlist[3], hmlist[4], hmlist[5], hmlist[6], bfl);

                //OleDbCommand mysql1 = new OleDbCommand(sql1, ConnValue);
                //mysql1.ExecuteScalar();
            }

            //OleDbCommand myAvg = new OleDbCommand("select avg(bfl) from cpp_lskjhm", ConnValue);
            //$"{((xxsum[0] / sum) * 100):F2}%";
            label50.Text = $"{(sumbfl / xx11.GetLength(0)):F2}";// myAvg.ExecuteScalar().ToString();

            for (int i = 0; i < tt.Count(); i++)
            {
                KeyValuePair<int, ItemInfo> kv = tt.ElementAt(i);

                TmpZbh.Add(kv.Key);

                //i = i + 2;
                //if (i%2== 0) i=i+3;
            }


            //build =  build.Distinct().ToArray();
            int[] build1 = TmpZbh.ToArray();
            //Array.Sort(build1);
            zbh = new int[33];
            Array.Copy(build1, zbh, 33);
            //Array.Copy(build1, 10, zbh, 0, 10);
            listBox1.Items.Add(string.Join(",", build1));
            listBox1.Items.Add("最大连续次数:" + ContinueNumLenth(zbh));

            int iicl = dt_save.Rows.Count;
            int iiclTMP = iicl / 5; 
            int[][] vsFW = new int[][]
            {
                new int[2]{0, iiclTMP},
                new int[2]{iiclTMP+1,iiclTMP * 2},
                new int[2]{iiclTMP* 2+1, iiclTMP *3},
                new int[2]{iiclTMP*3 +1, iiclTMP *4 },
                new int[2]{iiclTMP*4+1, iicl }
            };
            DataTable ds_savexxx = null;
            Parallel.For(0, 5, i =>
           {
               Application.DoEvents();
               ds_savexxx = doworkfx(dt_save,vsFW[i]);
               listBox2.Items.Add("最终["+i.ToString()+"]筛选结果:" + ds_savexxx.Rows.Count.ToString());
               listBox2.TopIndex = listBox2.Items.Count - 1;

               foreach(DataRow dr in ds_savexxx.Rows)
               {
                   dt_save1.Rows.Add(dr.ItemArray);
               }
               
           });
           WriteTextFile(dt_save1.DefaultView.ToTable(true, "num"), @"c:\\cpsxjg.txt", true);

            // DataTable ds_savexxx = doworkfx(dt_save);



            #region 分析开始
            //for (int cf = mincf; cf <= maxcf ; cf++)
            //{
            //    ds.Clear();
            //    //取数据库到datatable
            //    //string sql = "select num1,num2,num3,num4,num5,num6 from cpp_base where cf = "+cf.ToString()+" and js>70";
            //    string sql = "select * from cpp_base where cf = " + cf.ToString();// (num1 =2 and num2=15 and num3=22 and num4=26 and num5=30 and num6=33)"; // cf = " + cf.ToString() 
            //    //string sql = "select * from cpp_6hm ";
            //    OleDbDataAdapter da = new OleDbDataAdapter(sql, ConnValue);
            //    da.Fill(ds, "cppbase");
            //    ConnClose();

            //    //if (fxstop == true) break;
            //    ljcount += ds.Tables["cppbase"].Rows.Count;



            //    for (int i = 0; i < ds.Tables["cppbase"].Rows.Count; i++)
            //    {
            //        ljCurrentRecno++;
            //        label49.Text = "正在执行重复机率[" + cf.ToString() + "][" +ljcount + "]条数据.[" + ljCurrentRecno + "]";
            //        int[] iirows = new int[7];
            //        Boolean ibTrue = false;
            //        string ibTrueInfo = "";

            //        if (fxstop == true) break;

            //        iirows[0] = int.Parse(ds.Tables["cppbase"].Rows[i]["num1"].ToString());
            //        iirows[1] = int.Parse(ds.Tables["cppbase"].Rows[i]["num2"].ToString());
            //        iirows[2] = int.Parse(ds.Tables["cppbase"].Rows[i]["num3"].ToString());
            //        iirows[3] = int.Parse(ds.Tables["cppbase"].Rows[i]["num4"].ToString());
            //        iirows[4] = int.Parse(ds.Tables["cppbase"].Rows[i]["num5"].ToString());
            //        iirows[5] = int.Parse(ds.Tables["cppbase"].Rows[i]["num6"].ToString());
            //        iirows[6] = int.Parse(ds.Tables["cppbase"].Rows[i]["num7"].ToString());
            //        //iirows[6] = int.Parse(dt_bb.Rows[i]["num7"].ToString());
            //        label47.Text = "[" + ds.Tables["cppbase"].Rows.Count.ToString() +
            //                       "]正在分析第[" + i.ToString() + "]— [" +
            //                      iirows[0].ToString() + "][" +
            //                      iirows[1].ToString() + "][" +
            //                      iirows[2].ToString() + "][" +
            //                      iirows[3].ToString() + "][" +
            //                      iirows[4].ToString() + "][" +
            //                      iirows[5].ToString() + "][" +
            //                      iirows[6].ToString() + "]";
            //        while (true)
            //        {
            //            Application.DoEvents();
            //            int cgcs = 0; //

            //            //八卦阵判断
            //            ibTrue = FzpcfArray(iirows, bgz, 3, 6);
            //            if (ibTrue == true) cgcs++; else ibTrueInfo += "[八卦阵未通过]";

            //            //集团码,若随机的号码里面组合有5个与该下列号码相同，就排除。
            //            ibTrue = Fzpcf(iirows, jthm, 5, 6);
            //            if (ibTrue == true) cgcs++; else ibTrueInfo += "[集团码未通过]";

            //            //万能码,如果你选择的号码里面没有以下号码其中之一，就排除。
            //            ibTrue = Fzpcf(iirows, wlm, 1, 6);
            //            if (ibTrue == false) cgcs++; else ibTrueInfo += "[万能码未通过]";

            //            //神7数字
            //            ibTrue = Fzpcf(iirows, s7sz, 1, 6);
            //            if (ibTrue == false) cgcs++; else ibTrueInfo += "[神7数字未通过]";

            //            //质数
            //            ibTrue = Fzpcf(iirows, zshm, 1, 6);
            //            if (ibTrue == false) cgcs++; else ibTrueInfo += "[质数未通过]";

            //            //2,3,6,7,9尾
            //            ibTrue = Fzpcf(iirows, w23679w, 1, 6);
            //            if (ibTrue == false) cgcs++; else ibTrueInfo += "[23679尾未通过]";

            //            //分组排除法
            //            // hm1
            //            ibTrue = FzpcfArray(iirows, hm1, 4, 6);
            //            if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm1未通过]";

            //            //hm2
            //            ibTrue = Fzpcf(iirows, hm2, 4, 6);
            //            if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm2未通过]";

            //            //hm3
            //            ibTrue = FzpcfArray(iirows, hm3, 3, 6);
            //            if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm3未通过]";

            //            //hm4
            //            ibTrue = FzpcfArray(iirows, hm4, 5, 6);
            //            if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm4未通过]";

            //            //hm5
            //            ibTrue = FzpcfArray(iirows, hm5, 5, 6);
            //            if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm5未通过]";

            //            //hm6
            //            ibTrue = Fzpcf(iirows, hm6, 5, 6);
            //            if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm6未通过]";

            //            //hm7
            //            ibTrue = FzpcfArray(iirows, hm7, 5, 6);
            //            if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm7未通过]";

            //            //hm8
            //            ibTrue = Fzpcf(iirows, hm8, 5, 6);
            //            if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm8未通过]";

            //            //hm9
            //            ibTrue = FzpcfArray(iirows, hm9, 5, 6);
            //            if (ibTrue == true) cgcs++; else ibTrueInfo += "[hm9未通过]";

            //            //在我选择的15号码里面去匹配
            //            //ibTrue = FzpcfArray(iirows, hm11, 6);
            //            //int[] tmp = { 1, 3, 4, 5, 7, 9, 11, 13, 14, 15, 17, 19, 22, 23, 25, 28, 30, 31, 32, 33 };
            //            //int[] tmp1 = { 2, 6, 8, 10, 12, 16, 18, 20, 21, 24, 26, 27, 29 };
            //            //int[] tmp2 = { 3,4,7,11,12,15,17,24,27,29,30,33 };  //12码
            //            //int[] tmp3 = { 1,3,4,8,12,13,17,18,24,25,28,32 };//12码
            //            int[] tmp3 = { 1, 2, 3, 5, 6, 7, 10, 12, 13, 14, 15, 17, 19, 20, 23, 24, 25, 27, 29, 30, 31, 32, 33 };
            //            ibTrue = Fzpcf(iirows, zbh, 3, 6);
            //            if (ibTrue == false) cgcs++; else ibTrueInfo += "[12码未通过]";

            //            //历史号码比对
            //            ibTrue = FzpcfArray(iirows, hm10, 4, 6);
            //            if (ibTrue == true) cgcs++;
            //            else
            //            {
            //                //listBox1.Items.Add(">>>>>>>> 历史中奖 >>>>>>>" + string.Join(",", iirows));
            //                ibTrueInfo += "[历史号码未通过]";
            //            }

            //            if (cgcs != 17) ibTrue = false;
            //            else
            //            {

            //                if (iirows[0] + iirows[1] + iirows[2] + iirows[3] + iirows[4] + iirows[5] > 130) ibTrue = false;

            //                //号码出现机率要达到平均值
            //                string sqlsum = string.Format("select sum(bfl) from cpp_bfl where num in ({0},{1},{2},{3},{4},{5},{6})",
            //                                            iirows[0], iirows[1], iirows[2], iirows[3], iirows[4], iirows[5], iirows[6]);
            //                Conn.Open();
            //                ConnValue = Conn;
            //                OleDbCommand mysum = new OleDbCommand(sqlsum, ConnValue);
            //                decimal decimal_mysumresult = (decimal)mysum.ExecuteScalar();
            //                ConnClose();
            //                decimal decimal_myavg = decimal.Parse(label50.Text.ToString());
            //                decimal OK_myMAX = decimal_myavg + (decimal)0.02;
            //                decimal OK_myMIN = decimal_myavg - (decimal)0.02;

            //                if (decimal_mysumresult > OK_myMIN && decimal_mysumresult < OK_myMAX) ibTrue = true;
            //                else
            //                {
            //                    ibTrue = false;
            //                    ibTrueInfo += decimal_mysumresult.ToString() + "出现机率未通过";

            //                }
            //            }
            //            //if (iirows[0] > 10) ibTrue = false;
            //            break;
            //        }
            //        //listBox1.Items.Add(string.Join(",", iirows) + ibTrueInfo);
            //        if (ibTrue == true)
            //        {

            //            //iirows[6] = 0;
            //            DataRow dr = dt_save.NewRow();
            //            dr["num1"] = iirows[0];
            //            dr["num2"] = iirows[1];
            //            dr["num3"] = iirows[2];
            //            dr["num4"] = iirows[3];
            //            dr["num5"] = iirows[4];
            //            dr["num6"] = iirows[5];
            //            dr["num7"] = iirows[6];
            //            dr["num"] = string.Format("{0:00}", iirows[0]) + "," + string.Format("{0:00}", iirows[1]) + "," +
            //                        string.Format("{0:00}", iirows[2]) + "," + string.Format("{0:00}", iirows[3]) + "," +
            //                        string.Format("{0:00}", iirows[4]) + "," + string.Format("{0:00}", iirows[5]) + "+" +
            //                        string.Format("{0:00}", iirows[6]);
            //            dr["cf"] = ds.Tables["cppbase"].Rows[i]["cf"].ToString();
            //            dt_save.Rows.Add(dr);
            //            label48.Text = "共计满足条件号码有[" + dt_save.Rows.Count.ToString() + "]条.";
            //            //listBox1.Items.Add(dr["num"]);
            //        }
            //        //else
            //        //{
            //        //    DataRow dr = dt_save1.NewRow();
            //        //    dr["num1"] = iirows[0];
            //        //    dr["num2"] = iirows[1];
            //        //    dr["num3"] = iirows[2];
            //        //    dr["num4"] = iirows[3];
            //        //    dr["num5"] = iirows[4];
            //        //    dr["num6"] = iirows[5];
            //        //    dr["num7"] = iirows[6];
            //        //    dr["num"] = string.Format("{0:00}", iirows[0]) + "," + string.Format("{0:00}", iirows[1]) + "," +
            //        //                string.Format("{0:00}", iirows[2]) + "," + string.Format("{0:00}", iirows[3]) + "," +
            //        //                string.Format("{0:00}", iirows[4]) + "," + string.Format("{0:00}", iirows[5]) + "+" +
            //        //                string.Format("{0:00}", iirows[6]);
            //        //    dr["cf"] = ds.Tables["cppbase"].Rows[i]["cf"].ToString();
            //        //    dr["qxinfo"] = ibTrueInfo;
            //        //    dt_save1.Rows.Add(dr);
            //        //}
            //    }
            //    dataGridView5.DataSource = dt_save;
            //    label48.Text = "共计满足条件号码有[" + dt_save.Rows.Count.ToString() + "]条.";
            //    WriteTextFile(dt_save, @"d:\CP.txt");
            //    //WriteTextFile(dt_save.DefaultView.ToTable(true,"num"), @"d:\CP.txt");
            //    //WriteTextFile(dt_save1, @"d:\CPqxinfo.txt");
            //}
            #endregion

            listBox1.Items.Add("结束分析：[" + DateTime.Now.ToString() + "].");
            fxstop = false;
        }


        /// <summary>
        /// DataTable 转二维数组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public object[,] datatabletoarry(DataTable dt)
        {
            int col = 7;// dt.Columns.Count;
            object[,] array = new object[dt.Rows.Count, col];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    array[i, j] = dt.Rows[i][j];
                }
            }
            return array;
        }
        

        private void button11_Click(object sender, EventArgs e)
        {
            fxstop = true;
        }


        private void button12_Click(object sender, EventArgs e)
        {
            int[] b = {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,
                          19,20,21,22,23,24,25,26,27,28,29,30,31,32,33};
            int[] bo = randomList(b);
            listBox1.Items.Add(string.Join(",",bo));

            int[][] b1 = new int[3][];
            int[] iisplit = { 11, 22, 33 };
            int bb = 0;
            int nn = 0;
            for (int i = 0; i < 3; i++)
            {
                b1[i] = new int[11];
                for (int cc = bb;cc<iisplit[i];cc++)
                {

                    b1[i][nn] = bo[cc] ;
                    nn++;
                }
                nn = 0;
                bb = iisplit[i];
                int[] xx = b1[i];
                //label50.Text = string.Join(",", xx);
                for (int ii = 0; ii < 11; ii++)
                {
                    hm11[i,ii] = xx[ii];
                }
            }

            listBox1.Items.Add("第一组随机数:"+ string.Join(",", b1[0]));
            listBox1.Items.Add("第二组随机数:" + string.Join(",", b1[1]));
            listBox1.Items.Add("第三组随机数:" + string.Join(",", b1[2]));

            int[][] xx11 = new int[hm10.GetLength(0)][];
            for (int i = 0; i < hm10.GetLength(0); i++)
            {
                xx11[i] = new int[hm10.GetLength(1)];
                for (int cc=0;cc<hm10.GetLength(1);cc++)
                {
                    xx11[i][cc] = hm10[i, cc];
                }
            }
            int[] build = new int[hm10.GetLength(0)*(hm10.GetLength(1) -1)];
            int buildint = 0;
            for (int co = 0; co < xx11.GetLength(0); co++)
            {
                for (int co1 = 0; co1 < 6; co1++)
                {
                    build[buildint] = xx11[co][co1];
                    buildint++;
                }
                
            }

            // 集合 dic 用于存放统计结果
            Dictionary<int, ItemInfo> dic =
                new Dictionary<int, ItemInfo>();
            Dictionary<int, ItemInfo> dic1 =
               new Dictionary<int, ItemInfo>();
            // 开始统计每个元素重复次数
            foreach (int v in build)
            {
                if (dic.ContainsKey(v))
                {
                    // 数组元素再次，出现次数增加 1
                    dic[v].RepeatNum += 1;
                }
                else
                {
                    // 数组元素首次出现，向集合中添加一个新项
                    // 注意 ItemInfo类构造函数中，已经将重复
                    // 次数设置为 1
                    dic.Add(v, new ItemInfo(v));
                }
            }

            var tt  = dic.OrderByDescending(r => r.Value.RepeatNum);

            List<int> TmpZbh = new List<int>();

            for (int i = 0; i < tt.Count(); i++)
            {
                KeyValuePair<int, ItemInfo> kv = tt.ElementAt(i);

                //**** 转换每个号码的出现百分率 ****
                decimal n1 = kv.Value.RepeatNum;
                decimal n2 = (hm10.GetLength(0) * 7);

                decimal bfltmp = n1/n2;
                string sql = "update cpp_bfl set bfl=" + bfltmp.ToString() +
                             " where num=" + kv.Key.ToString();
                OleDbCommand mysql = new OleDbCommand(sql, ConnValue);
                mysql.ExecuteNonQuery();

                bfltable[i][0] = kv.Key;
                bfltable[i][1] = bfltmp;
               //**********************************
            }

            OleDbCommand mydel = new OleDbCommand("delete from cpp_lskjhm", ConnValue);
            mydel.ExecuteNonQuery();

            for (int i = 0; i < hm10.GetLength(0); i++)
            {

                decimal bfl=0.0000M;
                int[] hmlist = { hm10[i, 0], hm10[i, 1], hm10[i, 2], hm10[i, 3], hm10[i, 4], hm10[i, 5], hm10[i, 6] };
                string sql = string.Format("select sum(bfl) from cpp_bfl where num in ({0},{1},{2},{3},{4},{5},{6})",
                                hmlist[0], hmlist[1], hmlist[2], hmlist[3], hmlist[4], hmlist[5], hmlist[6]);


                OleDbCommand mysql = new OleDbCommand(sql, ConnValue);
                bfl = (decimal)mysql.ExecuteScalar();

                string sql1 = string.Format("insert into cpp_lskjhm values ({0},{1},{2},{3},{4},{5},{6},{7})",
                                            hmlist[0], hmlist[1], hmlist[2], hmlist[3], hmlist[4], hmlist[5], hmlist[6], bfl);

                OleDbCommand mysql1 = new OleDbCommand(sql1, ConnValue);
                mysql1.ExecuteScalar();
            }

            OleDbCommand myAvg = new OleDbCommand("select avg(bfl) from cpp_lskjhm", ConnValue);
            label50.Text = myAvg.ExecuteScalar().ToString();


            for (int i = 0; i < tt.Count(); i++)
            {
                KeyValuePair<int, ItemInfo> kv = tt.ElementAt(i);

                TmpZbh.Add(kv.Key);

                //i = i + 2;
                //if (i%2== 0) i=i+3;
            }


            //build =  build.Distinct().ToArray();
            int[] build1 = TmpZbh.ToArray();
            //Array.Sort(build1);
            zbh = new int[33];
            Array.Copy(build1, zbh, 33);
            //Array.Copy(build1, 10, zbh, 0, 10);
            listBox1.Items.Add(string.Join(",",build1));
            listBox1.Items.Add("最大连续次数:" + ContinueNumLenth(zbh));

        }

        /// <summary>
        /// 返回数组中连续数字出现次数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int QueueString(int[] str)
        {
            string input = string.Join(",", str);
            var arr = new System.Collections.ArrayList();
            var lines = input.Split(' ', ',', '，');
            int min, max, tmp = 1;
            max = min = int.Parse(lines[0]);
            foreach (var line in lines)
            {
                tmp = int.Parse(line);

                if ((tmp - max) > 1)
                {
                    
                    if (max != min) arr.Add(string.Format("{0}-{1}", min, max));
                    max = min = tmp;
                }
                else
                {
                    max = tmp;
                }
            }
            if ((tmp == min) || (tmp == max))
            {
                if (max != min)  arr.Add(string.Format("{0}-{1}", min, max));
            }
            return arr.Count;
        }

        public static int GetSequentialCount(int[] array)
        {
            if (array.Length == 0) return 0;
            Array.Sort(array);
            int count = 1;
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] == array[i - 1] + 1)
                {
                    count++;
                }
            }
            return count;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            int[][] xx = 
            {
                new int[7]{1,2,3,4,5,6,7},
                new int[7]{8,9,10,11,12,13,14},
                new int[7]{15,16,17,18,19,20,21},
                new int[6]{22,23,24,25,26,27},
                new int[6]{28,29,30,31,32,33}
            };

            int[] xxsum = { 0, 0, 0, 0, 0 };
            for (int i = 0; i < hm10.GetLength(0); i++)
            {
                int[] hmlist = { hm10[i, 0], hm10[i, 1], hm10[i, 2], hm10[i, 3], hm10[i, 4], hm10[i, 5]};

                for (int c=0;c< hmlist.Length; c++)
                {
                    int? index = GetIndexOfElement(xx, hmlist[c]);

                    if (index != null)
                    {
                        xxsum[(int)index]++;
                    }

                }
            }
            double sum = xxsum.Sum();

            double sum1 = xxsum[0];

            numq1.Text = $"{((xxsum[0] / sum) * 100):F2}%";
            numq2.Text = $"{((xxsum[1] / sum) * 100):F2}%";
            numq3.Text = $"{((xxsum[2] / sum) * 100):F2}%";
            numq4.Text = $"{((xxsum[3] / sum) * 100):F2}%";
            numq5.Text = $"{((xxsum[4] / sum) * 100):F2}%";
        }


        public int? GetIndexOfElement(int[][] array, int element)
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    if (array[i][j] == element)
                    {
                        return i;//* array[i].Length + j; // 计算索引
                    }
                }
            }
            return null; // 如果没有找到，返回null
        }

        public int? GetIndexOfElement(decimal[][] array, int element)
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    if (array[i][j] == element)
                    {
                        return i;//* array[i].Length + j; // 计算索引
                    }
                }
            }
            return null; // 如果没有找到，返回null
        }

        /// <summary>
        /// 号码最大连续次数
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int ContinueNumLenth(int[] array)
        {
            int value = array[0], tempvalue = 0, maxvalue = 0;
            int count = 1, pos = 0;
            int len = 0;
            for (int i = 0; i < array.Length; i++)
            {
                len = len < array.Length ? pos + 1 : pos;
                if (++pos < array.Length && array[len] == value + 1)
                {
                    value = array[pos];
                    count++;
                    tempvalue = count;
                }
                else
                {
                    if (maxvalue < tempvalue)
                    {
                        maxvalue = tempvalue;
                    }
                    if (pos < array.Length)
                    {
                        value = array[pos];
                        count = 1;
                    }
                }
            }
            return maxvalue;
        }
        #region 获取表中所有列名
        public static string[] GetTableColumnName(DataTable dt)
        {
            string cols = string.Empty;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                cols += (dt.Columns[i].ColumnName + ",");
            }
            cols = cols.TrimEnd(',');
            return cols.Split(',');
        }
        #endregion
    }


}

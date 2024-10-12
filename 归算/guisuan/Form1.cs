using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace guisuan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DATA data;

        private void 椭球面归算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < data.geodesic.Count(); i++)
            {
                data.geodesic[i].StartP.A = data.geodesic[i].A12;
                POINT p1 = new POINT(data.geodesic[i].StartP);
                POINT p2 = new POINT(data.geodesic[i].EndP);
                data.geodesic[i].S1 = caculate.CalD(p1, p2, data.geodesic[i].D);
                data.geodesic[i].S2 = caculate.CalD2(p1, p2, data.geodesic[i].S1);
            }
            foreach (geoangle geo in data.geoangle)
            {
                foreach (geodesic geo1 in data.geodesic)
                {
                    if (geo1.StartP.name == geo.StartP.name && geo1.EndP.name == geo.EndP.name)
                        geo.S = geo1.S1;
                    else if (geo1.EndP.name == geo.StartP.name && geo1.StartP.name == geo.EndP.name)
                        geo.S = geo1.S1;
                }
            }
            dataGridView1.DataSource = data.ToDataTableD();
        }

        private void 角度归算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string start = data.geoangle[0].StartP.name;
            double a0 = 0;
            bool if1 = true;
            double a12 = 0;
            for (int i = 0; i < data.geoangle.Count(); i++)
            {
                data.geoangle[i].StartP.A = data.geoangle[i].A12;
                POINT p1 = new POINT(data.geoangle[i].StartP);
                POINT p2 = new POINT(data.geoangle[i].EndP);
                //归算到椭球面
                if (data.geoangle[i].StartP.name != start)
                {
                    if1 = true;
                    start = data.geoangle[i].StartP.name;
                }
                if (if1)
                {
                    a0 = caculate.CalA1(p1, p2, data.geoangle[i].S);
                    data.geoangle[i].A1 = 0;
                    if1 = false;
                }
                else
                {
                    data.geoangle[i].cangle = caculate.CalA1(p1, p2, data.geoangle[i].S);
                    data.geoangle[i].A1 = (data.geoangle[i].cangle - a0) + data.geoangle[i].A;
                }
            }
            //归算到高斯平面
            if1 = true;
            for (int i = 0; i < data.geoangle.Count(); i++)
            {
                data.geoangle[i].StartP.A = data.geoangle[i].A12;
                POINT p1 = new POINT(data.geoangle[i].StartP);
                POINT p2 = new POINT(data.geoangle[i].EndP);

                data.geoangle[i].cangle2 = caculate.Calangle(p1, p2);

                if (data.geoangle[i].StartP.name != start)
                {
                    if1 = true;
                    start = data.geoangle[i].StartP.name;
                }
                if (if1)
                {
                    a0 = caculate.CalA1(p1, p2, data.geoangle[i].S);
                    a12 = data.geoangle[i].A12;
                    data.geoangle[i].A1 = 0;
                    if1 = false;
                }
                else
                {
                    data.geoangle[i].cangle2 = caculate.Calangle(p1, p2);
                    data.geoangle[i].A2 = data.geoangle[i].A1 + data.geoangle[i].cangle2 - a0;
                }
            }
            dataGridView2.DataSource = data.ToDataTableA();
        }

        private void 生成结果ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            if (data != null && data.p.Count() != 0 && data.geodesic.Count() != 0 && data.geoangle.Count() != 0)
            {
                foreach (POINT p in data.p)
                {
                    richTextBox2.Text += p.name;
                    richTextBox2.Text += "," + p.x.ToString();
                    richTextBox2.Text += "," + p.y.ToString();
                    richTextBox2.Text += "," + p.H.ToString();
                    richTextBox2.Text += "\r\n";
                }
                string name = data.geodesic[0].StartP.name;
                richTextBox2.Text += data.geodesic[0].StartP.name + "\r\n";
                bool a = true;
                foreach (geodesic geo in data.geodesic)
                {
                    if (geo.StartP.name != name)
                    {
                        richTextBox2.Text += geo.StartP.name + "\r\n";
                        name = geo.StartP.name;
                        richTextBox2.Text += geo.EndP.name + ",";
                        richTextBox2.Text += "S" + ",";
                        richTextBox2.Text += geo.S2.ToString("f5") + "\r\n";
                        a = true;
                    }
                    else
                    {
                        #region 扫描输出角度且同测站只输出一次
                        if (a)
                        {
                            foreach (geoangle geo1 in data.geoangle)
                            {
                                if (geo1.StartP.name == name)
                                {
                                    richTextBox2.Text += geo1.EndP.name + ",";
                                    richTextBox2.Text += "L" + ",";
                                    richTextBox2.Text += GeoPro.Rad2Dms(geo1.A2).ToString("f5") + "\r\n";
                                }
                                a = false;
                            }
                        }
                        #endregion

                        richTextBox2.Text += geo.EndP.name + ",";
                        richTextBox2.Text += "S" + ",";
                        richTextBox2.Text += geo.S2.ToString("f5") + "\r\n";
                    }
                }
            }
            else
            {
                MessageBox.Show("请先导入数据并归算");
            }

        }

        private void 导入数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            data = new DATA();
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "(文本文件)|*.txt";
            if (op.ShowDialog() == DialogResult.OK)
            {
                data = FileHelper.ReadFile(op.FileName);
                dataGridView1.DataSource = data.ToDataTableD();
                dataGridView2.DataSource = data.ToDataTableA();
            }
        }

        private void 测试数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            data = new DATA();
            POINT a = new POINT();
            a.name = "高山";
            a.B = GeoPro.Dms2Rad(32.31);
            a.H = 3606.4;
            a.Xi = 5.5;
            a.Eta = -2.5;
            POINT b = new POINT();
            b.name = "龙山";
            b.B = GeoPro.Dms2Rad(32.20);
            b.H = 3494.9 + 29.1;
            b.Xi = 7.6;
            b.Eta = -1.6;
            POINT c = new POINT();
            c.name = "张庄";
            c.B = GeoPro.Dms2Rad(32.19);
            c.H = 3759.2 + 29.3;
            c.Xi = 6.8;
            c.Eta = -1.7;
            POINT d = new POINT();
            d.name = "王村";
            d.B = GeoPro.Dms2Rad(32.17);
            d.H = 3931.7 + 29.2;
            d.Xi = 6.2;
            d.Eta = -1.9;
            data.p.Add(a); data.p.Add(b); data.p.Add(c); data.p.Add(d);

            geoangle angle1 = new geoangle();
            angle1.StartP = a;
            angle1.EndP = b;
            angle1.A = GeoPro.Dms2Rad(0);
            angle1.S = 30.7 * 1000;
            angle1.A12 = GeoPro.Dms2Rad(128.17);
            b.alpha = GeoPro.Dms2Rad(-0.1651);
            data.geoangle.Add(angle1);

            geoangle angle2 = new geoangle();
            angle2.StartP = a;
            angle2.EndP = c;
            angle2.A = GeoPro.Dms2Rad(40.435334);
            angle2.S = 22 * 1000;
            angle2.A12 = GeoPro.Dms2Rad(169.01);
            c.alpha = GeoPro.Dms2Rad(-0.2302);
            data.geoangle.Add(angle2);

            geoangle angle3 = new geoangle();
            angle3.StartP = a;
            angle3.EndP = d;
            angle3.A = GeoPro.Dms2Rad(102.361145);
            angle3.S = 38.1 * 1000;
            angle3.A12 = GeoPro.Dms2Rad(230.53);
            d.alpha = GeoPro.Dms2Rad(0.2302);
            data.geoangle.Add(angle3);
            dataGridView2.DataSource = data.ToDataTableA();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("不帮");
        }
    }
}

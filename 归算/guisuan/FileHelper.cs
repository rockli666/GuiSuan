using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Reflection;
using System.Windows.Forms;

namespace guisuan
{
    public class FileHelper
    {
        public static DATA ReadFile(string filepath)
        {
            DATA data = new DATA();
            data.geodesic = new List<geodesic>();
            data.geoangle = new List<geoangle>();
            try
            {
                StreamReader re = new StreamReader(filepath);
                string line = "";
                line = re.ReadLine();
                string[] lines;
                lines = line.Split(',');
                caculate.L0 = GeoPro.Dms2Rad(double.Parse(lines[1]));

                #region 导入近似坐标
                while (true)
                {
                    line = re.ReadLine();
                    if (line == "")
                        break;
                    lines = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    POINT p = new POINT();
                    p.name = lines[0];
                    p.B = GeoPro.Dms2Rad(double.Parse(lines[1]));
                    p.L = GeoPro.Dms2Rad(double.Parse(lines[2]));
                    p.x = double.Parse(lines[3]);
                    p.y = double.Parse(lines[4]);
                    p.H = double.Parse(lines[5]);
                    data.p.Add(p);
                }
                #endregion
                POINT start = new POINT();
                while (!re.EndOfStream)
                {
                    line = "";
                    line = re.ReadLine();
                    lines = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    #region 判断起点
                    if (lines.Count() == 1)
                    {
                        foreach (POINT p in data.p)
                        {
                            if (p.name == lines[0])
                                start = p;
                        }
                        continue;
                    }
                    #endregion

                    #region 接收实测数据

                    #region 判断是不是距离
                    if (lines[1] == "S")
                    {
                        geodesic geo = new geodesic();
                        geo.StartP = start;
                        foreach (POINT p in data.p)
                        {
                            if (p.name == lines[0])
                                geo.EndP = p;
                        }
                        geo.D = double.Parse(lines[2]);
                        geo.A12 = caculate.CalA(geo, geo.StartP.B, geo.StartP.L);
                        data.geodesic.Add(geo);
                    }
                    #endregion

                    #region 判断是不是角度
                    else if (lines[1] == "L")
                    {
                        geoangle geo1 = new geoangle();
                        geo1.StartP = start;
                        foreach (POINT p in data.p)
                        {
                            if (p.name == lines[0])
                                geo1.EndP = p;
                        }
                        geo1.A = GeoPro.Dms2Rad(double.Parse(lines[2]));
                        geo1.A12 = caculate.CalA(geo1, geo1.StartP.B, geo1.StartP.L);
                        data.geoangle.Add(geo1);
                    }
                    #endregion

                    #endregion
                }

                foreach (geoangle geo in data.geoangle)
                {
                    if (geo.StartP.name == "G010" && geo.EndP.name == "G009")
                        geo.S = Math.Sqrt(Math.Pow(geo.StartP.x - geo.EndP.x, 2) + Math.Pow(geo.StartP.y - geo.EndP.y, 2));
                    if (geo.EndP.name == "G010" && geo.StartP.name == "G009")
                        geo.S = Math.Sqrt(Math.Pow(geo.StartP.x - geo.EndP.x,2)+ Math.Pow(geo.StartP.y - geo.EndP.y,2));
                }

                #region  把距离给角度观测类
                foreach (geoangle geo in data.geoangle)
                {
                    foreach (geodesic geo1 in data.geodesic)
                    {
                        if (geo1.StartP.name == geo.StartP.name && geo1.EndP.name == geo.EndP.name)
                            geo.S = geo1.D;
                        else if (geo1.EndP.name == geo.StartP.name && geo1.StartP.name == geo.EndP.name)
                            geo.S = geo1.D;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件格式错误");
            }
            return data;
        }
    }
}


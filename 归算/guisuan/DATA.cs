using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace guisuan
{
    public class DATA
    {
        public List<POINT> p = new List<POINT>();
        public List<geodesic> geodesic=new List<geodesic>();
        public List<geoangle> geoangle=new List<geoangle>();

        public DataTable ToDataTableD()
        {
            DataTable table = InitTableD();
            try
            {
                foreach (var d in geodesic)
                {
                    DataRow row = table.NewRow();
                    row["起点"] = d.StartP.name;
                    row["终点"] = d.EndP.name;
                    row["大地方位角"] = GeoPro.Rad2Str(d.A12);
                    row["起点大地高"] = d.StartP.H;
                    row["终点大地高"] = d.EndP.H;
                    row["实测距离"] = d.D;
                    row["椭球面距离"] = $"{d.S1:f5}";
                    row["高斯平面距离"] = $"{d.S2:f5}";
                    table.Rows.Add(row);
                }
            }
            catch (Exception)
            {

            }
            return table;
        }
        DataTable InitTableD()
        {
            DataTable table = new DataTable("Coor");
            table.Columns.Add("起点", typeof(string));
            table.Columns.Add("终点", typeof(string));
            table.Columns.Add("大地方位角", typeof(string));
            table.Columns.Add("起点大地高", typeof(string));
            table.Columns.Add("终点大地高", typeof(string));
            table.Columns.Add("实测距离", typeof(string));
            table.Columns.Add("椭球面距离", typeof(string));
            table.Columns.Add("高斯平面距离", typeof(string));
            return table;
        }

        public DataTable ToDataTableA()
        {
            DataTable table = InitTableA();
            try
            {
                foreach (var d in geoangle)
                {
                    DataRow row = table.NewRow();
                    row["测站"] = d.StartP.name;
                    row["观测点"] = d.EndP.name;
                    row["观测目标高"] = d.EndP.H;
                    row["观测值"] = GeoPro.Rad2Str(d.A);
                    row["测站大地纬度"] = GeoPro.Rad2Str(d.StartP.B);
                    row["大地方位角"] = GeoPro.Rad2Str(d.A12);
                    row["距离"] = d.S;
                    row["椭球面角度值"] = GeoPro.Rad2Str( d.A1);
                    row["高斯平面角度值"] = GeoPro.Rad2Str(d.A2);
                 
                    table.Rows.Add(row);
                }
            }
            catch (Exception)
            {

            }
            return table;
        }
        DataTable InitTableA()
        {
            DataTable table = new DataTable("Coor");
            table.Columns.Add("测站", typeof(string));
            table.Columns.Add("观测点", typeof(string));
            table.Columns.Add("观测目标高", typeof(string));
            table.Columns.Add("测站大地纬度", typeof(string));
            table.Columns.Add("大地方位角", typeof(string));
            table.Columns.Add("距离", typeof(string));
            table.Columns.Add("观测值", typeof(string));
            table.Columns.Add("椭球面角度值", typeof(string));
            table.Columns.Add("高斯平面角度值", typeof(string));
            return table;
        }

    }
}
public class geodesic
{
    public POINT StartP;//起点
    public POINT EndP;//终点
    public double D;//平距
    public double A12;//起点真方位角
    public double S1 = 0;//椭球平面距离
    public double S2 = 0;//高斯平面距离
    /// <summary>
    /// 距离改
    /// </summary>
    public geodesic()
    {
        StartP = new POINT();
        EndP = new POINT();
        D = 0;
        A12 = 0;
        S1 = 0;
        S2 = 0;
    }
    public geodesic(geodesic geo)
    {
        StartP = geo.StartP;
        EndP = geo.EndP;
        D = geo.D;
        A12 = geo.A12;
        S1 = geo.S1;
        S2 = geo.S2;
    }
}
/// <summary>
/// 角度改
/// </summary>
public class geoangle
{
    public POINT StartP;//测站
    public POINT EndP;//观测点
    public double S;//边长
    public double A12;//起点真方位角
    public double A;//观测值
    public double A1 =0 ;//椭球方向改正
    public double A2 = 0;//高斯方向改正
    public double cangle = 0;
    public double cangle2 = 0;
    public geoangle()
    {
        StartP = new POINT();
        EndP = new POINT();
        S = 0;
        A12 = 0;
        A1 = 0;
        A2 = 0;
        cangle = 0;
        cangle2 = 0;
    }
    public geoangle(geoangle geo)
    {
        StartP = geo.StartP;
        EndP = geo.EndP;
        S = geo.S;
        A12 = geo.A12;
        A1 = geo.A1;
        A2 = geo.A2;
    }
}

public class POINT
{
    public string name;
    public double x;
    public double y;
    public double H = 0;
    public double A;//方位角
    public double B;
    public double L;
    public double Xi = 0;//垂线偏差分量
    public double Eta = 0;//垂线偏差分量
    public double alpha = 0;//垂直角

    public POINT()
    {
        name = " ";
        x = 0;
        y = 0;
        H = 0;
        A = 0;
        B = 0;
        L = 0;
        Xi = 0;
        Eta = 0;
        alpha = 0;
    }

    public POINT(POINT p)
    {
        name = p.name;
        x = p.x;
        y = p.y;
        H = p.H;
        A = p.A;
        B = p.B;
        L = p.L;
        Xi = p.Xi;
        Eta = p.Eta;
        alpha = p.alpha;
    }
}


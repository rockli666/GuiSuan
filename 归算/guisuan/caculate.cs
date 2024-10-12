using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace guisuan
{

    static class caculate
    {
        static public double L0;
        static Ellipsoid ell = new Ellipsoid();

        public static double CalA1(POINT p1, POINT p2, double S)
        {
            double angle = 0;
            double a1 = Calchuixian(p1,p2);
            double a2 = CalBiaoGao(p1, p2);
            double a3 = CalJieMian(p1, p2, S);

            angle = a1 + a2 + a3;
            angle = angle / 3600.0/180.0*Math.PI;
            return angle;
    }


        #region 观测值归算到椭球面(方向)
        /// <summary>
        /// 垂线偏差改正
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        static public double Calchuixian(POINT p,POINT p2)
        {
            double reslut = 0;

            reslut = -(p.Xi * Math.Sin(p.A)
                - p.Eta * Math.Cos(p.A))
                * Math.Tan(p2.alpha);

            return reslut;
        }
        /// <summary>
        /// 标高差改正
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        static public double CalBiaoGao(POINT p1, POINT p2)
        {
            double reslut = 0;
            double rou = 206265;

            double M2 = ell.a * (1 - ell.e2)
                * (Math.Pow((1 - ell.e2 * Math.Sin(p2.B) * Math.Sin(p2.B)), -3 / 2.0));

            reslut = ell.e2 * p2.H * rou / (2 * M2)
                * Math.Cos(p2.B) * Math.Cos(p2.B) * Math.Sin(2 * p1.A);

            return reslut;
        }
        /// <summary>
        /// 截面差改正
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        static public double CalJieMian(POINT p1, POINT p2,double S)
        {
            double reslut = 0;
            double rou = 206265;

            double N1 = ell.a
                / Math.Sqrt(1 - ell.e2 * Math.Sin(p1.B) * Math.Sin(p1.B));

            reslut = -1*ell.e2 * S * S * rou / (12.0 * N1 * N1)
                * Math.Cos(p1.B) * Math.Cos(p1.B) * Math.Sin(2 * p1.A);

            return reslut;
        }
        #endregion

        #region 椭球面归算到高斯平面(方向)
        static public double Calangle(POINT p1, POINT p2)
        {
            Ellipsoid ell = new Ellipsoid();
            double reslut = 0;
            double ym = (p1.y - 500000 + p2.y - 500000) / 2;
            double p = 206265;
            double Rm = Math.Sqrt(ell.M(p1.B) * ell.N(p1.B));
            double Rm2 = Rm * Rm;
            double Rm3 = Rm2 * Rm;
            double ym2 = ym * ym;
            double ym3 = ym2 * ym;
            double eta = Math.Sqrt(ell.e_2 * Math.Cos(p1.B) * Math.Cos(p1.B));

            reslut = -p / (6 * Rm2) * (p2.x - p1.x) * (2 * (p1.y - 500000) + (p2.y - 500000) - ym3 / Rm2)
                - p * eta*eta * Math.Tan(p1.B) / Rm3 * (p2.y - p1.y) * ym2;

            reslut = reslut / 3600.0 / 180.0 * Math.PI;

            return reslut;
        }
        #endregion

        #region 观测值归算到椭球面(距离)
        public static double CalD(POINT p1, POINT p2, double D)
        {
            Ellipsoid ell = new Ellipsoid();
            double cosB2 = Math.Cos(p1.B) * Math.Cos(p1.B);
            double cosA2 = Math.Cos(p1.A) * Math.Cos(p1.A);

            double on = 1 - Math.Pow((p2.H - p1.H) / D, 2);
            double RA = ell.N(p1.B)/(1+ell.e_2*cosB2*cosA2);
            double down = (1 + p1.H / RA) * (1 + p2.H / RA);
            double d = D * Math.Sqrt(on / down);
            double S = 0;
            S = d + d * d * d / (RA * RA * 24);
            return S;
        }
        #endregion

        #region 椭球面归算到高斯平面(距离)
        static public double CalD2(POINT p1, POINT p2, double S)
        {
            Ellipsoid ell = new Ellipsoid();
            double reslut = 0;
            double ym = (p1.y-500000 + p2.y-500000) / 2;
            double Rm2 = ell.M(p1.B) * ell.N(p1.B);
            double Rm4 = Rm2 * Rm2;
            double ym2 = ym * ym;
            double ym4 = ym2 * ym2;
            double deltay2 = Math.Pow((p1.y - p2.y), 2);

            double m = 1 + ym*ym / (2 * Rm2);

            reslut = S * (1 + ym2 / (2 * Rm2) + deltay2 / (24 * Rm2) + ym4 / (24 * Rm4));

            return reslut;
        }
        #endregion

        #region 获得大地方位角
        /// <summary>
        /// 获得坐标方位角
        /// </summary>
        /// <param name="x1">起点x坐标</param>
        /// <param name="x2">终点x坐标</param>
        /// <param name="y1">起点y坐标</param>
        /// <param name="y2">终点y坐标</param>
        /// <returns></returns>
        public static double Getfwangle(double x1, double x2, double y1, double y2)
        {
            double x = x2 - x1;//中间变量
            double y = y2 - y1;//中间变量
            double fwangle = 0;
            if (x == 0 && y > 0)
            {
                fwangle = Math.PI * 0.5;
            }
            else if (x == 0 && y < 0)
            {
                fwangle = Math.PI * 1.5;
            }
            if (x > 0 && y > 0)
            {
                fwangle = Math.Atan(y / x);
            }
            else if (x < 0 && y > 0)
            {
                fwangle = Math.PI - Math.Abs(Math.Atan(y / x));
            }
            else if (x < 0 && y < 0)
            {
                fwangle = Math.PI + Math.Atan(y / x);
            }
            else if (x > 0 && y < 0)
            {
                fwangle = 2 * Math.PI - Math.Abs(Math.Atan(y / x));
            }
            return fwangle;
        }

        public static double CalA(geodesic geo, double B, double L)
        {
            double a = Getfwangle(geo.StartP.x, geo.EndP.x, geo.StartP.y, geo.EndP.y);
            Ellipsoid ell = new Ellipsoid();
            double reslut;
            double sinB = Math.Sin(B);
            double l = L - L0;
            double eta2 = ell.e_2 * Math.Cos(B) * Math.Cos(B);
            double eta4 = eta2 * eta2;
            double l2 = l * l;
            double l4 = l2 * l2;
            double cosB2 = Math.Cos(B) * Math.Cos(B);
            double cosB4 = cosB2 * cosB2;

            reslut = sinB * l * (1 + 1 / 3.0 * cosB2 * (1 + 3 * eta2 + 2 * eta4) * l2 + 1 / 15.0 * cosB4 * (2 - Math.Tan(B) * Math.Tan(B)) * l4);

            reslut += a;
            return reslut;
        }
        public static double CalA(geoangle geo, double B, double L)
        {
            double a = Getfwangle(geo.StartP.x, geo.EndP.x, geo.StartP.y, geo.EndP.y);
            Ellipsoid ell = new Ellipsoid();
            double reslut;
            double sinB = Math.Sin(B);
            double l = L - L0;
            double eta2 = ell.e_2 * Math.Cos(B) * Math.Cos(B);
            double eta4 = eta2 * eta2;
            double l2 = l * l;
            double l4 = l2 * l2;
            double cosB2 = Math.Cos(B) * Math.Cos(B);
            double cosB4 = cosB2 * cosB2;

            reslut = sinB * l * (1 + 1 / 3.0 * cosB2 * (1 + 3 * eta2 + 2 * eta4) * l2 + 1 / 15.0 * cosB4 * (2 - Math.Tan(B) * Math.Tan(B)) * l4);

            reslut += a;
            return reslut;
        }
        #endregion
    }
}

using UnityEngine;
using UnityEditor;

namespace Tools
{
    public static class Math
    {
        static int degreeCount = 72;
        /// <summary>
        /// Кол во синусов и косинусов
        /// </summary>
        public static int DegreeCount
        {
            get
            {
                return degreeCount;
            }
            set
            {
                degreeCount = value;
                degree = 360 / value;
                InitSinCos();
            }
        }

        static int degree = 5;
        /// <summary>
        /// Синус и косинус каждого Degree угла
        /// </summary>
        public static int Degree
        {
            get
            {
                return degree;
            }
            set
            {
                degree = value;
                degreeCount = 360 / value;
                InitSinCos();
            }
        } 

        static float[] sin;
        static float[] cos;

        public static void InitSinCos()
        {
            sin = new float[DegreeCount];
            cos = new float[DegreeCount];

            for (int i = 0; i < degreeCount; i++)
            {
                sin[i] = Mathf.Sin(Mathf.Deg2Rad * i * degree);
                cos[i] = Mathf.Cos(Mathf.Deg2Rad * i * degree);
            }
        }

        public static float Sin(float degree)
        {
            if(sin == null)
                InitSinCos();
            return sin[IndexByDegree(degree)];
        }

        public static float Cos(float degree)
        {
            if (cos == null)
                InitSinCos();
            return cos[IndexByDegree(degree)];
        }

        static int IndexByDegree(float degree)
        {
            var rot = Quaternion.identity * Quaternion.Euler(0,0,degree);
            return Mathf.Clamp((int)(rot.eulerAngles.z / Math.degree), 0, degreeCount - 1);
        }
    }
}

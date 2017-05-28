using UnityEngine;
using System;


    public class Matrix3DTools
    {
	
        public static void multiply(ref Vector4 p, ref Matrix m, ref Vector4  result)
        {

            double x = (double)(p.x * m.M11 + p.y * m.M21 + p.z * m.M31 + p.w * m.M41);
            double y = (double)(p.x * m.M12 + p.y * m.M22 + p.z * m.M32 + p.w * m.M42);
            double z = (double)(p.x * m.M13 + p.y * m.M23 + p.z * m.M33 + p.w * m.M43);
            double w = (double)(p.x * m.M14 + p.y * m.M24 + p.z * m.M34 + p.w * m.M44);

            result.x = (float)x;
            result.y = (float)y;
            result.z = (float)z;
            result.w = (float)w;
        }

        public static void multiply(double x,double y, double z, ref Matrix m, ref Vector4  result)
        {
            result.x = (float)(x * m.M11 + y * m.M21 + z * m.M31 +  m.M41);
            result.y = (float)(x * m.M12 + y * m.M22 + z * m.M32 +  m.M42);
            result.z = (float)(x * m.M13 + y * m.M23 + z * m.M33 +  m.M43);
            result.w = (float)(x * m.M14 + y * m.M24 + z * m.M34 +  m.M44);
        }

        public static void multiply(ref Vector3 p, ref Matrix m, ref Vector3 result)
        {
            double x = (double)p.x * m.M11 + (double)p.y * m.M21 + (double)p.z * m.M31 + 1.0 * m.M41;
            double y = (double)p.x * m.M12 + (double)p.y * m.M22 + (double)p.z * m.M32 + 1.0 * m.M42;
            double z = (double)p.x * m.M13 + (double)p.y * m.M23 + (double)p.z * m.M33 + 1.0 * m.M43;
            result.x = (float)x;
            result.y = (float)y;
            result.z = (float)z;

        }

        public static void rotate(ref Vector3 p, ref Matrix m, ref Vector3 result)
        {
            double x = (double)(p.x * m.M11 + p.y * m.M21 + p.z * m.M31);
            double y = (double)(p.x * m.M12 + p.y * m.M22 + p.z * m.M32);
            double z = (double)(p.x * m.M13 + p.y * m.M23 + p.z * m.M33);
            result.x = (float)x;
            result.y = (float)y;
            result.z = (float)z;
        }


        //mutiplies m1 by m2 and puts the result in result, no object creation
        public static void multiply(ref Matrix m1, ref Matrix m2, ref Matrix result)
        {
            //first line
            double M11 = m1.M11 * m2.M11 + m1.M12 * m2.M21 + m1.M13 * m2.M31 + m1.M14 * m2.M41;
            double M12 = m1.M11 * m2.M12 + m1.M12 * m2.M22 + m1.M13 * m2.M32 + m1.M14 * m2.M42;
            double M13 = m1.M11 * m2.M13 + m1.M12 * m2.M23 + m1.M13 * m2.M33 + m1.M14 * m2.M43;
            double M14 = m1.M11 * m2.M14 + m1.M12 * m2.M24 + m1.M13 * m2.M34 + m1.M14 * m2.M44;

            //second line
            double M21 = m1.M21 * m2.M11 + m1.M22 * m2.M21 + m1.M23 * m2.M31 + m1.M24 * m2.M41;
            double M22 = m1.M21 * m2.M12 + m1.M22 * m2.M22 + m1.M23 * m2.M32 + m1.M24 * m2.M42;
            double M23 = m1.M21 * m2.M13 + m1.M22 * m2.M23 + m1.M23 * m2.M33 + m1.M24 * m2.M43;
            double M24 = m1.M21 * m2.M14 + m1.M22 * m2.M24 + m1.M23 * m2.M34 + m1.M24 * m2.M44;

            //third line
            double M31 = m1.M31 * m2.M11 + m1.M32 * m2.M21 + m1.M33 * m2.M31 + m1.M34 * m2.M41;
            double M32 = m1.M31 * m2.M12 + m1.M32 * m2.M22 + m1.M33 * m2.M32 + m1.M34 * m2.M42;
            double M33 = m1.M31 * m2.M13 + m1.M32 * m2.M23 + m1.M33 * m2.M33 + m1.M34 * m2.M43;
            double M34 = m1.M31 * m2.M14 + m1.M32 * m2.M24 + m1.M33 * m2.M34 + m1.M34 * m2.M44;

            //fourth line
            double M41 = m1.M41 * m2.M11 + m1.M42 * m2.M21 + m1.M43 * m2.M31 + m1.M44 * m2.M41;
            double M42 = m1.M41 * m2.M12 + m1.M42 * m2.M22 + m1.M43 * m2.M32 + m1.M44 * m2.M42;
            double M43 = m1.M41 * m2.M13 + m1.M42 * m2.M23 + m1.M43 * m2.M33 + m1.M44 * m2.M43;
            double M44 = m1.M41 * m2.M14 + m1.M42 * m2.M24 + m1.M43 * m2.M34 + m1.M44 * m2.M44;

            result.M11 = M11;
            result.M12 = M12;
            result.M13 = M13;
            result.M14 = M14;
            result.M21 = M21;
            result.M22 = M22;
            result.M23 = M23;
            result.M24 = M24;
            result.M31 = M31;
            result.M32 = M32;
            result.M33 = M33;
            result.M34 = M34;
            result.M41 = M41;
            result.M42 = M42;
            result.M43 = M43;
            result.M44 = M44;

        }

        //mutiplies m1 by m2 and puts the result in result, no object creation
        public static void multiply33(ref Matrix m1, ref Matrix m2, ref Matrix result)
        {
            //first line
            double M11 = m1.M11 * m2.M11 + m1.M12 * m2.M21 + m1.M13 * m2.M31;
            double M12 = m1.M11 * m2.M12 + m1.M12 * m2.M22 + m1.M13 * m2.M32;
            double M13 = m1.M11 * m2.M13 + m1.M12 * m2.M23 + m1.M13 * m2.M33;


            //second line
            double M21 = m1.M21 * m2.M11 + m1.M22 * m2.M21 + m1.M23 * m2.M31;
            double M22 = m1.M21 * m2.M12 + m1.M22 * m2.M22 + m1.M23 * m2.M32;
            double M23 = m1.M21 * m2.M13 + m1.M22 * m2.M23 + m1.M23 * m2.M33;

            //third line
            double M31 = m1.M31 * m2.M11 + m1.M32 * m2.M21 + m1.M33 * m2.M31;
            double M32 = m1.M31 * m2.M12 + m1.M32 * m2.M22 + m1.M33 * m2.M32;
            double M33 = m1.M31 * m2.M13 + m1.M32 * m2.M23 + m1.M33 * m2.M33;


            result.M11 = M11;
            result.M12 = M12;
            result.M13 = M13;

            result.M21 = M21;
            result.M22 = M22;
            result.M23 = M23;

            result.M31 = M31;
            result.M32 = M32;
            result.M33 = M33;
        }

        //copies m in result, no object creation
        public static void copy(ref Matrix m, ref Matrix result)
        {
            //first line
            result.M11 = m.M11;
            result.M12 = m.M12;
            result.M13 = m.M13;
            result.M14 = m.M14;

            //second line
            result.M21 = m.M21;
            result.M22 = m.M22;
            result.M23 = m.M23;
            result.M24 = m.M24;

            //third line
            result.M31 = m.M31;
            result.M32 = m.M32;
            result.M33 = m.M33;
            result.M34 = m.M34;

            //fourth line
            result.M41 = m.M41;
            result.M42 = m.M42;
            result.M43 = m.M43;
            result.M44 = m.M44;
        }

        //copies m in result, no object creation
        public static void copy33(ref Matrix m, ref Matrix result)
        {
            //first line
            result.M11 = m.M11;
            result.M12 = m.M12;
            result.M13 = m.M13;

            //second line
            result.M21 = m.M21;
            result.M22 = m.M22;
            result.M23 = m.M23;

            //third line
            result.M31 = m.M31;
            result.M32 = m.M32;
            result.M33 = m.M33;

        }


        public static Matrix createRotationMatrix(double x, double y, double z, double degree)
        {

            double c = (double)Math.Cos(degree * Math.PI/180.0);
            double s = (double)Math.Sin(degree * Math.PI/180.0);
            double C = 1 - c;
            double xs = x * s;
            double ys = y * s;
            double zs = z * s;
            double yC = y * C;
            double zC = z * C;
            double xC = x * C;
            double xyC = x * yC;
            double yzC = y * zC;
            double zxC = z * xC;

            return new Matrix(
                x * xC + c, xyC + zs, zxC - ys, 0,
                xyC - zs, y * yC + c, yzC + xs, 0,
                zxC + ys, yzC - xs, z * zC + c, 0,
                0, 0, 0, 1);
        }

        public static Matrix createTranslationMatrix(double x, double y, double z)
        {
            return new Matrix(1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                x, y, z, 1
                );
        }

        public static Matrix createScaleMatrix(double x, double y, double z)
        {
            return new Matrix(x, 0, 0, 0,
                0, y, 0, 0,
                0, 0, z, 0,
                0, 0, 0, 1
                );
        }
       

        public static void set(ref Matrix matrix, double M11, double M12, double M13, double M14, double M21, double M22, double M23, double M24, double M31, double M32, double M33, double M34, double M41, double M42, double M43, double M44)
        {
            matrix.M11 = M11;
            matrix.M12 = M12;
            matrix.M13 = M13;
            matrix.M14 = M14;
            matrix.M21 = M21;
            matrix.M22 = M22;
            matrix.M23 = M23;
            matrix.M24 = M24;
            matrix.M31 = M31;
            matrix.M32 = M32;
            matrix.M33 = M33;
            matrix.M34 = M34;
            matrix.M41 = M41;
            matrix.M42 = M42;
            matrix.M43 = M43;
            matrix.M44 = M44;

        }

        internal static void set33(ref Matrix matrix, double M11, double M12, double M13, double M21, double M22, double M23, double M31, double M32, double M33)
        {
            matrix.M11 = M11;
            matrix.M12 = M12;
            matrix.M13 = M13;

            matrix.M21 = M21;
            matrix.M22 = M22;
            matrix.M23 = M23;

            matrix.M31 = M31;
            matrix.M32 = M32;
            matrix.M33 = M33;


        }

        internal static void translateLocal(double x, double y, double z, ref Matrix matrix)
        {
            Matrix t = createTranslationMatrix(x, y, z);
            Matrix3DTools.multiply(ref t, ref matrix, ref matrix);
        }


        internal static void rotateLocal(double x, double y, double z, double degree, ref Matrix matrix)
        {
            Matrix t = Matrix3DTools.createRotationMatrix(x, y, z, degree);
            Matrix3DTools.multiply33(ref t, ref matrix, ref matrix);
        }

        internal static void scaleLocal(double x, double y, double z, ref Matrix matrix)
        {
            Matrix t = Matrix3DTools.createScaleMatrix(x, y, z);
            Matrix3DTools.multiply33(ref t, ref matrix, ref matrix);
        }
    }

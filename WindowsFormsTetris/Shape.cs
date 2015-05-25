using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WindowsFormsTetris
{
    /***
     * Contain the information of each shape
     ***/
    class Shape
    {
        public const int POS_MAX = 4;  // 0 = up, 1 = right, 2 = down, 3 = down

        int m_color;
        Point[][] m_shape = new Point[POS_MAX][];

        /**
         * Set original shape and color
         * this constructor creates rotated shapes
         **/
        public Shape(Point[] orgShape, int color)
        {
            m_shape[0] = orgShape;

            // prepare rotated shapes
            for (int i = 1; i < POS_MAX; i++) {
                m_shape[i] = new Point[POS_MAX];
            }
            double centerX = 0;
            double centerY = 0;
            calcCenter(orgShape, out centerX, out centerY);
            for (int i = 0; i < orgShape.Length; i++) {
                // right rotated shape
                m_shape[1][i].X = (int)(centerY - (orgShape[i].Y - centerY));
                m_shape[1][i].Y = orgShape[i].X;
                // down rotated shape
                m_shape[2][i].X = (int)(centerX - (orgShape[i].X - centerX));
                m_shape[2][i].Y = (int)(centerY - (orgShape[i].Y - centerY));
                // left rotated shape
                m_shape[3][i].X = orgShape[i].Y;
                m_shape[3][i].Y = (int)(centerX - (orgShape[i].X - centerX));
            }
            this.m_color = color;
        }

        public Point[] getPoints(int rotate)
        {
            return m_shape[rotate];
        }

        public int getColor()
        {
            return m_color;
        }

        public int getHeight(int rotate)
        {
            int minY = 999;
            int maxY = 0;
            for (int i = 0; i < m_shape[rotate].Length; i++) {
                int y = m_shape[rotate][i].Y;
                maxY = maxY > y ? maxY : y;
                minY = minY < y ? minY : y;
            }
            return maxY - minY + 1;
        }

        public int getWidth(int rotate)
        {
            int minX = 999;
            int maxX = 0;
            for (int i = 0; i < m_shape[rotate].Length; i++) {
                int x = m_shape[rotate][i].X;
                maxX = maxX > x ? maxX : x;
                minX = minX < x ? minX : x;
            }
            return maxX - minX + 1;
        }

        public Point getOrigin(int rotate)
        {
            int minX = 999;
            int minY = 999;
            for (int i = 0; i < m_shape[rotate].Length; i++) {
                int x = m_shape[rotate][i].X;
                int y = m_shape[rotate][i].Y;
                minY = minY < y ? minY : y;
                minX = minX < x ? minX : x;
            }
            return new Point(minX, minY);
        }

        /**
         * Calculate the Center
         * memo: Overrapping coordinate(either x or y) must be ignored
         **/
        private void calcCenter(Point[] points, out double centerX, out double centerY)
        {
            List<int> alreadyAddedNumX = new List<int>();
            List<int> alreadyAddedNumY = new List<int>();
            Array.ForEach(points, p => {
                if (!alreadyAddedNumX.Contains(p.X)) {
                    alreadyAddedNumX.Add(p.X);
                }
                if (!alreadyAddedNumY.Contains(p.Y)) {
                    alreadyAddedNumY.Add(p.Y);
                }
            });
            centerX = alreadyAddedNumX.Average();
            centerY = alreadyAddedNumY.Average();
        }
    }
}

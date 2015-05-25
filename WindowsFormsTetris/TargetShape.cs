using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WindowsFormsTetris
{
    /***
     * Administer the currently falling shape
     ***/
    class TargetShape
    {
        public enum ROTATE_CONTROL {
            RIGHT,
            LEFT,
            NONE,
        }

        Shape m_shape;
        Point m_currentPosition = new Point();
        public int m_currentRotate = 0;  // 0 = up, 1 = right, 2 = down, 3 = down

        public TargetShape(Shape shape, int x, int y)
        {
            m_shape = shape;
            m_currentPosition.X = x;
            m_currentPosition.Y = y;
        }

        public TargetShape Copy()
        {
            TargetShape obj = new TargetShape(this.m_shape, this.m_currentPosition.X, this.m_currentPosition.Y);
            obj.m_currentRotate = this.m_currentRotate;
            return obj;
        }

        /* Get the points of the shape at the current rotation */
        public Point[] getPoints()
        {
            return getPoints(m_currentRotate);
        }

        /* Get the points of the shape at the specified rotation */
        public Point[] getPoints(int rotate)
        {
            Point[] points = new Point[m_shape.getPoints(rotate).Length];
            for (int i = 0; i < points.Length; i++) {
                points[i].X = m_shape.getPoints(rotate)[i].X + m_currentPosition.X;
                points[i].Y = m_shape.getPoints(rotate)[i].Y + m_currentPosition.Y;
            }
            return points;
        }

        /* Get the origin position of the shape at the specified rotation */
        public Point getOrigin(int rotate)
        {
            return new Point(m_shape.getOrigin(rotate).X + m_currentPosition.X,
                             m_shape.getOrigin(rotate).Y + m_currentPosition.Y);
        }

        public int getHeight()
        {
            return m_shape.getHeight(m_currentRotate);
        }

        public int getHeight(int rotate)
        {
            return m_shape.getHeight(rotate);
        }

        public int getColor()
        {
            return m_shape.getColor();
        }

        /* if possible, move */
        public void tryMoveShape(Point point, ROTATE_CONTROL rotate, int[,] map, out bool isBottom, out bool isGameover)
        {
            isBottom = false;
            isGameover = false;
            int newRotate = rotateShape(rotate);
            Point[] points = m_shape.getPoints(newRotate);
            int i = 0;
            for (i = 0; i < points.Length; i++) {
                int x = points[i].X + m_currentPosition.X + point.X;
                int y = points[i].Y + m_currentPosition.Y + point.Y;
                if (rotate != ROTATE_CONTROL.NONE) {
                    /* try to rotate as long as possible */
                    if (x < 0) {
                        tryMoveShape(new Point(point.X+1, point.Y), rotate, map, out isBottom, out isGameover);
                    } else if (x >= map.GetLength(0)) {
                        tryMoveShape(new Point(point.X - 1, point.Y), rotate, map, out isBottom, out isGameover);
                    } else {
                        //if (map[x, y] != 0) break;
                    }
                }
                if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1)) break;
                if (map[x, y] != 0) break;
            }
            if (i == points.Length) {
                m_currentPosition.X += point.X;
                m_currentPosition.Y += point.Y;
                m_currentRotate = newRotate;
            } else {
                if (rotate == ROTATE_CONTROL.NONE && point.Y > 0) {
                    isBottom = true;
                    for (i = 0; i < points.Length; i++) {
                        if(points[i].Y + m_currentPosition.Y == 0){
                            isGameover = true;
                            break;
                        }
                    }
                }
            }
        }

        public int rotateShape(ROTATE_CONTROL rotate)
        {
            int currentRotate = m_currentRotate;
            switch (rotate){
                case ROTATE_CONTROL.RIGHT:
                    currentRotate++;
                    currentRotate %= 4;
                    break;
                case ROTATE_CONTROL.LEFT:
                    currentRotate += 4 - 1;
                    currentRotate %= 4;
                    break;
                default:
                    break;
            }
            return currentRotate;
            
        }

        public int getBottomPosition(int rotate)
        {
            int maxY = 0;
            for (int i = 0; i < m_shape.getPoints(rotate).Length; i++) {
                int y = m_shape.getPoints(rotate)[i].Y + m_currentPosition.Y;
                if (y > maxY) maxY = y;
            }
            return maxY;
        }

    }
}

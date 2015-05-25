using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsTetris
{
    /***
     * Convert to a block coodinate into a pixel coordinate(rectangle)
     * memo: pixel -> block -> shape
     ***/
    class BlockCoordinateConverter
    {
        int m_width;    // at block coordinate
        int m_height;   // at block coordinate
        Rectangle[,] m_dstRect;

        public BlockCoordinateConverter(int width, int height, int canvasWidthPixel, int canvasHeightPixel)
        {
            m_width = width;
            m_height = height;
            int pointSizePixel = Math.Min(canvasWidthPixel / m_width, canvasHeightPixel / m_height);

            m_dstRect = new Rectangle[m_width, m_height];
            for (int i = 0; i < m_width; i++) {
                for (int j = 0; j < m_height; j++) {
                    m_dstRect[i, j] = new Rectangle(i * pointSizePixel, j * pointSizePixel, pointSizePixel, pointSizePixel);
                }
            }
        }

        /**
         * Block position -> rectangle on canvas(pixel)
         **/
        public Rectangle getRect(int x, int y)
        {
            return m_dstRect[x, y];
        }
    }
}

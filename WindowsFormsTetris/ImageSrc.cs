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
     * Administer source image
     ***/
    class ImageSrc
    {
        const string FILENAME = "test.png";
        const int BLOCK_SIZE_SRC = 30;
        //const int NUM_COLOR = 8;

        public enum COLOR
        {
            BLACK,
            RED,
            GREEN,
            BLUE,
            YELLOW,
            PURPLE,
            OLIVE,
            ORANGE,
            PINK,
            COLOR_MAX
        };

        Bitmap m_imageSrc;
        Rectangle[] m_srcRect = new Rectangle[(int)COLOR.COLOR_MAX];

        public ImageSrc()
        {
            m_imageSrc = new Bitmap(FILENAME);
            for (int i = 0; i < (int)COLOR.COLOR_MAX; i++) {
                m_srcRect[i] = new Rectangle(i * BLOCK_SIZE_SRC, 0, BLOCK_SIZE_SRC, BLOCK_SIZE_SRC);
            }
        }

        public Bitmap getImage()
        {
            return m_imageSrc;
        }

        public Rectangle getImageRect(int colorIndex)
        {
            return m_srcRect[colorIndex];
        }
    }
}


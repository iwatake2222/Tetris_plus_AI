//#define DEBUG_SHAPE

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;

namespace WindowsFormsTetris
{
    /***
     * Main Class for Tetris
     * memo: create 2 worker threads for animation(delete line and gameover)
     *       create 1 thread for AI
     ***/
    public partial class Form1 : Form
    {
        /* The size of map */
        const int WIDTH = 10;
        const int HEIGHT = 20;

        /* Definition of basic each shape */
        Shape[] shapes = new Shape[]{
            new Shape(new Point[]{new Point(0,0),new Point(1,0),new Point(0,1),new Point(1,1)}, (int)ImageSrc.COLOR.BLUE),
            new Shape(new Point[]{new Point(0,0),new Point(0,1),new Point(0,2),new Point(0,3)}, (int)ImageSrc.COLOR.GREEN),
            new Shape(new Point[]{new Point(0,2),new Point(0,1),new Point(1,1),new Point(1,0)}, (int)ImageSrc.COLOR.ORANGE),
            new Shape(new Point[]{new Point(0,0),new Point(0,1),new Point(1,1),new Point(1,2)}, (int)ImageSrc.COLOR.OLIVE),
            new Shape(new Point[]{new Point(0,0),new Point(0,1),new Point(0,2),new Point(1,2)}, (int)ImageSrc.COLOR.PURPLE),
            new Shape(new Point[]{new Point(0,0),new Point(0,1),new Point(0,2),new Point(1,1)}, (int)ImageSrc.COLOR.YELLOW),
            new Shape(new Point[]{new Point(0,0),new Point(1,0),new Point(1,1),new Point(1,2)}, (int)ImageSrc.COLOR.PINK),
        };

        /* The source image of each block */
        ImageSrc m_imageSrc = new ImageSrc();

        /* Convert from block coordinate(m_colorMap) into pictureBox coordinate(pixel) */
        BlockCoordinateConverter m_blockCoordinateConverter;

        /* Contain Color of each block in pictureBox */
        /*modify this map with (int)ImageSrc.COLOR */
        int[,] m_colorMap;

        /* Currently falling shape */
        TargetShape m_targetShape;

        /* shapeIndex for next (shown in pictureBoxNext) */
        int m_nextShape = 0;

        /* score */
        int m_score = 0;
        SavedDataManager m_scoreMgr = new SavedDataManager();

        /* AutoCoontroller */
        AIController m_aiController;
        IOWrapper m_ioWrapper;

        /* etc */
        bool m_toggleRotateKeyDown = false;
        bool m_isGameover = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Debug.WriteLine("ThreadID(Main) = {0}", Thread.CurrentThread.ManagedThreadId);

            m_blockCoordinateConverter = new BlockCoordinateConverter(WIDTH, HEIGHT, pictureBoxMain.Width, pictureBoxMain.Height);

            /* relocate control objects */
            pictureBoxMain.Width = m_blockCoordinateConverter.getRect(WIDTH - 1, HEIGHT - 1).Right;
            pictureBoxMain.Height = m_blockCoordinateConverter.getRect(WIDTH - 1, HEIGHT - 1).Bottom;
            pictureBoxNext.Left = pictureBoxMain.Right + 10;
            pictureBoxNext.Top = pictureBoxMain.Top;
            labelScore.Left = pictureBoxNext.Left;
            labelScore.Top = pictureBoxNext.Bottom + 10;
            labelScore.Left = pictureBoxNext.Left;
            labelScore.Top = pictureBoxNext.Bottom + 10;
            updateScore();
            labelHowTo.Left = labelScore.Left;
            labelHowTo.Top = labelScore.Bottom + 10;

            startGame();

        }

        private void startGame()
        {
            /* initialize map */
            m_colorMap = new int[WIDTH, HEIGHT];
            for (int i = 0; i < WIDTH; i++) {
                for (int j = 0; j < HEIGHT; j++) {
                    m_colorMap[i, j] = (int)ImageSrc.COLOR.BLACK;
                }
            }

            m_ioWrapper = new IOWrapper();

            m_nextShape = new Random().Next(0, shapes.Length);
            putNewShape();

            m_score = 0;
            m_scoreMgr.load();
            updateScore();

            timerControl.Enabled = true;
            timerFall.Enabled = true;

            labelHowTo.Text = "KEY_LEFT : <-" + Environment.NewLine +
                              "KEY_RIGHT : ->" + Environment.NewLine +
                              "KEY_DOWN : FALLING SPEED UP" + Environment.NewLine +
                              "KEY_UP or SPACE : ROTATE" + Environment.NewLine +
                              "A: Auto Play";

        }

        private void timerControl_Tick(object sender, EventArgs e)
        {
            if (m_targetShape == null) return;
            controleShape();
        }


        private void timerFall_Tick(object sender, EventArgs e)
        {
            /* create new target shape if current target shape went to the bottom before */
            if (m_targetShape == null) {
                putNewShape();
            }
            fallShape();
        }

        /**
         * Move targetShape based on key input
         **/
        private void controleShape()
        {
            bool iskeyDown = false;
            bool isBottomTemp, isGameoverTemp;

            /* fill black into previous position */
            foreach (var p in m_targetShape.getPoints()) {
                m_colorMap[p.X, p.Y] = (int)ImageSrc.COLOR.BLACK;
            }

            if(m_ioWrapper.isKeyDownRight()){
                iskeyDown = true;
                m_targetShape.tryMoveShape(new Point(1, 0), TargetShape.ROTATE_CONTROL.NONE, m_colorMap, out isBottomTemp, out isGameoverTemp);
            }
            if (m_ioWrapper.isKeyDownLeft()) {
                iskeyDown = true;
                m_targetShape.tryMoveShape(new Point(-1, 0), TargetShape.ROTATE_CONTROL.NONE, m_colorMap, out isBottomTemp, out isGameoverTemp);
            }
            if (m_ioWrapper.isKeyDownRotate()) {
                iskeyDown = true;
                m_targetShape.tryMoveShape(new Point(0, 0), TargetShape.ROTATE_CONTROL.LEFT, m_colorMap, out isBottomTemp, out isGameoverTemp);
                m_toggleRotateKeyDown = true;
            }
            if (m_ioWrapper.isKeyDownDown()) {
                iskeyDown = true;
                m_targetShape.tryMoveShape(new Point(0, 1), TargetShape.ROTATE_CONTROL.NONE, m_colorMap, out isBottomTemp, out isGameoverTemp);
                m_targetShape.tryMoveShape(new Point(0, 1), TargetShape.ROTATE_CONTROL.NONE, m_colorMap, out isBottomTemp, out isGameoverTemp);
            }


            /* fill the color into new position */
            foreach (var p in m_targetShape.getPoints()) {
                m_colorMap[p.X, p.Y] = m_targetShape.getColor();
            }
            if (iskeyDown) {
                updateCanvas();
            }
        }

        /**
         * Move down shape, and check whether it arrived at bottom
         *  if bottom, delete current target shape and check whether the line can be deleted
         *  in addition, check whether it goes gameover
         **/
        private void fallShape()
        {
            /* fill black into previous position */
            foreach (var p in m_targetShape.getPoints()) {
                m_colorMap[p.X, p.Y] = (int)ImageSrc.COLOR.BLACK;
            }

            /* if possible, move down the shape and check whether it goes to bottom or gameover */
            bool isBottom;
            bool isGameover;
            m_targetShape.tryMoveShape(new Point(0, 1), TargetShape.ROTATE_CONTROL.NONE, m_colorMap, out isBottom, out isGameover);
            foreach (var p in m_targetShape.getPoints()) {
                m_colorMap[p.X, p.Y] = m_targetShape.getColor();
            }
            updateCanvas();


            /* For animation, following processes run another thread */
            /* stop timer to avoid conflict of UI */
            if (isGameover) {
                //Debug.WriteLine("ThreadID(Timer) = {0}", Thread.CurrentThread.ManagedThreadId);
                m_targetShape = null;
                timerFall.Enabled = false;
                Thread thrd = new Thread(gameoverWapper);
                thrd.Priority = ThreadPriority.Lowest;
                thrd.Start();
            } else if (isBottom) {
                //Debug.WriteLine("ThreadID(Timer) = {0}", Thread.CurrentThread.ManagedThreadId);
                m_targetShape = null;
                timerFall.Enabled = false;
                timerControl.Enabled = false;
                Thread thrd = new Thread(deleteLineWrapper);
                thrd.Priority = ThreadPriority.Normal;
                thrd.Start();
            }
        }

        private void deleteLineWrapper()
        {
            //Debug.WriteLine("ThreadID(delteLine) = {0}", Thread.CurrentThread.ManagedThreadId);
            deleteLine();
            Action<bool> enableTimerControl = (enable) => { timerControl.Enabled = enable; };
            Action<bool> enableTimerFall = (enable) => { timerFall.Enabled = enable; };
            this.Invoke(enableTimerControl, true);
            this.Invoke(enableTimerFall, true);
        }

        /**
         * Check whether each line can be deleted
         * if possible, delete the line with animation
         **/
        private void deleteLine()
        {
            for (int y = m_colorMap.GetLength(1) - 1; y >= 0; y--) {
                int x = 0;
                for (x = 0; x<m_colorMap.GetLength(0); x++) {
                    if (m_colorMap[x, y] == (int)ImageSrc.COLOR.BLACK) break;
                }
                if (x == m_colorMap.GetLength(0)) {
                    for (int xx = 0; xx < m_colorMap.GetLength(0); xx++) {
                        m_colorMap[xx, y] = (int)ImageSrc.COLOR.RED;
                    }
                    updateCanvas();
                    Thread.Sleep(200);
                    for (int yy = y; yy > 0; yy--) {
                        for (int xx = 0; xx < m_colorMap.GetLength(0); xx++) {
                            m_colorMap[xx, yy] = m_colorMap[xx, yy - 1];
                        }
                    }
                    for (int xx = 0; xx < m_colorMap.GetLength(0); xx++) {
                        m_colorMap[xx, 0] = (int)ImageSrc.COLOR.BLACK;
                    }
                    m_score += 10;
                    m_scoreMgr.setScoreMax(m_score);
                    updateCanvas();
                    updateScore();
                    Thread.Sleep(300);
                    deleteLine();
                }
            }
        }

        private void gameoverWapper()
        {
            //Debug.WriteLine("ThreadID(gameover) = {0}", Thread.CurrentThread.ManagedThreadId);
            if (m_aiController != null) {
                m_aiController.stop();
                m_aiController = null;
            }
            gameover();
            m_isGameover = true;
            Action<bool> enableTimerControl = (enable) => { timerControl.Enabled = enable; };
            this.Invoke(enableTimerControl, true);
            Action<string> setLabel = (str) => { labelHowTo.Text = str; };
            this.Invoke(setLabel, "Press Space Key to Continue");
        }
        /**
         * Animation for gameover. convert color
         **/
        private void gameover()
        {
            for (int y = HEIGHT-1; y >=0 ; y--) {
                for (int x = 0; x < WIDTH; x++) {
                    if (m_colorMap[x, y] == (int)ImageSrc.COLOR.BLACK) {
                        m_colorMap[x, y] = (int)ImageSrc.COLOR.RED;
                    } else {
                        m_colorMap[x, y] = (int)ImageSrc.COLOR.BLACK;
                    }
                }
                updateCanvas();
                Thread.Sleep(100);
            }
        }

        /**
         * Draw canvas using m_colorMap
         * memo: called UI thread and worker thread as well
         **/
        private void updateCanvas()
        {
            Bitmap canvas = new Bitmap(pictureBoxMain.Width, pictureBoxMain.Height);
            Graphics g = Graphics.FromImage(canvas);

            for (int i = 0; i < WIDTH; i++) {
                for (int j = 0; j < HEIGHT; j++) {
                    g.DrawImage(m_imageSrc.getImage(), m_blockCoordinateConverter.getRect(i, j), m_imageSrc.getImageRect(m_colorMap[i, j]), GraphicsUnit.Pixel);
                }
            }

            if (pictureBoxMain.InvokeRequired) {
                // enter here when it called by worker thread like delete line
                Action updatePictureBox = () => {
                    pictureBoxMain.Image = canvas;
                    pictureBoxMain.Invalidate();
                };
                this.Invoke(updatePictureBox);
            } else {
                pictureBoxMain.Image = canvas;
                pictureBoxMain.Invalidate();
            }

            //Debug.WriteLine("pictureBoxMain invoke + {0}", pictureBoxMain.InvokeRequired);
        }

        /**
         * Allocate shape(shown in pictureBoxNext) onto default position in pictureBoxMain
         **/
        private void putNewShape()
        {
            /* put new current shape */
            m_targetShape = new TargetShape(shapes[m_nextShape], WIDTH / 2 - 1, 0);
            if (m_aiController != null) {
                m_aiController.updateTarget(m_targetShape);
            }

            /* prepare next shape */
#if DEBUG_SHAPE
            m_nextShape = 0;
#else 
            m_nextShape = new Random().Next(0, shapes.Length);
#endif
            Bitmap canvas = new Bitmap(pictureBoxNext.Width, pictureBoxNext.Height);
            Graphics g = Graphics.FromImage(canvas);

            var blockCoordinateConverter = new BlockCoordinateConverter(3, 4, pictureBoxNext.Width, pictureBoxNext.Height);
            Point[] point = shapes[m_nextShape].getPoints(0);
            foreach (var p in point) {
                g.DrawImage(m_imageSrc.getImage(), blockCoordinateConverter.getRect(p.X, p.Y), m_imageSrc.getImageRect(shapes[m_nextShape].getColor()), GraphicsUnit.Pixel);
            }
            pictureBoxNext.Image = canvas;
        }

        void updateScore()
        {
            String strScore;
            strScore = "High Score: " + m_scoreMgr.getScoreMax().ToString() + Environment.NewLine + "Your Score: " + m_score.ToString();

            Action<string> updateLabel = (str) => { labelScore.Text = str; };
            this.Invoke(updateLabel, strScore);
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_aiController != null) {
                m_aiController.stop();
                m_aiController = null;
            }
            m_scoreMgr.save();
        }

        private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (m_isGameover) {
                switch (e.KeyCode) {
                    case Keys.Space:
                        m_isGameover = false;
                        startGame();
                        break;
                }
            } else {
                switch (e.KeyCode) {
                    case Keys.A:
                        makeAutoController();
                        break;
                }
            }
        }

        private void makeAutoController()
        {
            if (m_aiController == null) {
                m_aiController = new AIController(m_ioWrapper, m_colorMap, m_targetShape);
            } else {
                m_aiController.stop();
                m_aiController = null;
            }
            
        }



    }
} 

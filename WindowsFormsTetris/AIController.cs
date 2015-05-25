using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Drawing;

namespace WindowsFormsTetris
{
    /***
     * AI for auto mode
     *  this class create thread when the class instance is created
     *  control the game automatically via IOWrapper
     * ToDo: to consider next shape
     *       to improve evaluate method
     *       to try to achieve tetris
     *       to try to delete line as soon as possible
     ***/
    class AIController
    {
        Thread m_thread;
        IOWrapper m_ioWrapper;

        /* 
         * whether calculation for new shape is needed or not
         * calculate only once for each new shape
         */
        bool m_needCalc;

        /* 
         * Information of game
         * memo: These are just reference, and can be updated while calculating
         *       so should be copied before calculation
         */
        private int[,] m_colorMap;
        TargetShape m_targetShape;

        public AIController(IOWrapper ioWrapper, int[,] colorMap, TargetShape targetShape)
        {
            m_ioWrapper = ioWrapper;
            m_colorMap = colorMap;
            m_needCalc = true;
            m_targetShape = targetShape;
            m_thread = new Thread(this.run);
            m_thread.Name = "AI_Controller";
            m_thread.Priority = ThreadPriority.Lowest;
            m_thread.Start();
        }

        public void updateTarget(TargetShape targetShape){
            m_targetShape = targetShape;
            m_needCalc = true;
            //Debug.WriteLine("■#NEW SHAPE#  {0}", targetShape.GetHashCode());
        }

        void run()
        {
            //Debug.WriteLine("ThreadID(AI) = {0}", Thread.CurrentThread.ManagedThreadId);
            TimeSpan maxTime = new TimeSpan(0);

            /* The result of AI calculation */
            int targetX = 0;        // where to move (absolute coordinate)
            int targetRotate = 0;   // how many times should be rotated

            while (true) {
                if (m_targetShape != null) {
                    if (m_needCalc) {
                        /* think only one time for each shape */
                        DateTime startTime = DateTime.Now;
                        think(m_targetShape.Copy(), (int[,])m_colorMap.Clone(), out targetX, out targetRotate);
                        TimeSpan calcTime = DateTime.Now - startTime;
                        if (calcTime > maxTime) {
                            maxTime = calcTime;
                            Debug.WriteLine("CalcTime = {0}", calcTime);
                        }
                        m_needCalc = false;
                    } else {
                        /* control shape based on the result */
                        if (targetX > m_targetShape.getOrigin(targetRotate).X) {
                            m_ioWrapper.m_setKeyDownRight = true;
                        } else if (targetX < m_targetShape.getOrigin(targetRotate).X) {
                            m_ioWrapper.m_setKeyDownLeft = true;
                        } else {
                            m_ioWrapper.m_setKeyDownDown = true;
                        }
                        if (targetRotate != m_targetShape.m_currentRotate) {
                            m_ioWrapper.m_setKeyDownRotate = true;
                        }
                    }
                } else {
                    // do nothing
                }
                Thread.Sleep(20);
            }
        }

        public void stop()
        {
            if (m_thread != null) {
                m_thread.Abort();
                m_thread = null;
                //Debug.WriteLine("ThreadID(AI) abort");
            }
        }

        /**
         * Calculate optimum position
         *  1. try to rotate
         *  2. try to move by X axis
         *  3. calculate bottom position(moveY) if the shape falls in each X
         *  4. evaluate the result using evaluate method
         *  5. try other X(goto 2) and othe rotation(goto 1)
         *  ToDo: Implement to move by X axis before rotattion
         *        Implement to move by X and rotation after tha shape falls each line
         **/
        void think(TargetShape targetShape, int[,] colorMap, out int out_moveX, out int out_rotate)
        {
            out_moveX = 0;
            out_rotate = 0;

            int optEvaluate = 0;    // Bigger is better

            /* Evaluate for each rotated position on each X */
            for (int rotate = 0; rotate < Shape.POS_MAX; rotate++) {
                Point[] targetPoints = targetShape.getPoints(rotate);    // targetPoints is current position
                for (int moveX = -colorMap.GetLength(0) / 2; moveX <= colorMap.GetLength(0) / 2; moveX++) {
                     /* Whether I can move the shape by moveX */
                    int indexPoint = 0;
                    for (indexPoint = 0; indexPoint < targetPoints.Length; indexPoint++) {
                        int x = targetPoints[indexPoint].X + moveX;
                        if (x < 0 || x >= colorMap.GetLength(0)) break;
                    }
                    /* if I can move, try next moveX */
                    if (indexPoint != targetPoints.Length) {
                        continue;
                    }

                    /** 
                     * Find out the bottom position after the shape moves by moveX
                     **/
                    int moveY = 0;  // relative coordinate from current position
                    for (moveY = targetShape.getHeight(rotate); moveY < colorMap.GetLength(1) - targetShape.getBottomPosition(rotate); moveY++) {
                        /* Whether I can move the shape by moveY */
                        for (indexPoint = 0; indexPoint < targetPoints.Length; indexPoint++) {
                            int x = targetPoints[indexPoint].X + moveX;
                            int y = targetPoints[indexPoint].Y + moveY;
                            if (y >= colorMap.GetLength(1)) break;
                            if (colorMap[x, y] != 0) break;
                        }
                        /* If I can move, it is bottom position + 1 */
                        if (indexPoint != targetPoints.Length) {
                            break;
                        }
                    }
                    /* now moveY is the position where I cannot move, so it must be -1 */
                    moveY--;

                    /* dspShape is the position after the shape falls */
                    Point[] dstShape = new Point[targetPoints.Length];
                    for (int j = 0; j < targetPoints.Length; j++) {
                        dstShape[j].X = targetPoints[j].X + moveX;
                        dstShape[j].Y = targetPoints[j].Y + moveY;
                    }

                    /* evaluate how it's nice if the shape fall into dstShape */
                    int val = evaluate(colorMap, dstShape, targetShape.getOrigin(rotate).Y + moveY, targetShape.getHeight(rotate));

                    //Debug.WriteLine("x={0}, y={1}, val={2}", moveX, targetShape.getOrigin(rotate).Y + moveY, val);

                    /* update optimum moveX */
                    if (val > optEvaluate) {
                        optEvaluate = val;
                        out_moveX = moveX;
                        out_rotate = rotate;
                    }
                }
            }

            /* Convert relative coordinate to absolute coordinate */
            out_moveX = targetShape.getOrigin(out_rotate).X + out_moveX;

            //Debug.WriteLine("movex = {0}, rotate = {1}", out_moveX, out_rotate);

        }

        /**
         * evaluate how it's nice if the new shape fall
         * startPoxY: start position of the new shape after it falls on Y axis. Evaluation starts here.
         * shapeSize: the height of the new shape
         **/
        int evaluate(int[,] colorMap, Point[] shape, int startPosY, int shapeSize)
        {
            int val = 0;    // at first, bigger is worse(Count bad result). Finally it is inversed
            List<int> evaluatedX = new List<int>();
            /**
             * How to evaluate
             *  1.1. Count new blanks between the new shap and old blocks
             *       use the digit of 100
             *  1.2. Count previous blanks
             *       use the digit of 10
             *  2.   Count the height of all blocks and blanks
             *       use the digit of 10
             *  3.   Count how close to the center
             *       use the digit of 1
             */
            for (int i = 0; i < shape.Length; i++) {
                /** calculate the range of x of the shape
                 *  ignore the same X as the X already evaluated
                 **/
                int x = shape[i].X;
                if (evaluatedX.Contains(x)) continue;
                evaluatedX.Add(x);

                /* Evaluation for 1.1 and 1.2 */
                int valOfBlank = 0; // 100 in the case of 1.1, 10 in the case of 1.2
                for (int y = startPosY; y < colorMap.GetLength(1); y++) {
                    bool isBlock = colorMap[x, y] != 0;
                    for (int j = 0; j < shape.Length; j++) {
                        if (shape[j].X == x && shape[j].Y == y) {
                            isBlock = true;
                            break;
                        }
                    }
                    if (isBlock) {
                        if (y < startPosY + shapeSize) {
                            /* blanks might be start because of new shape */
                            valOfBlank = 100;
                        } else {
                            /* blanks might be start because of old blocks */
                            valOfBlank = 10;
                        }
                    } else {
                        val += valOfBlank;
                    }
                }

            }

            /* Evaluation for 2 */
            int height = colorMap.GetLength(1) - startPosY;
            if (height < colorMap.GetLength(1) * 0.4) {
                val += height * 10;
            } else {
                val += height * 50;
            }

            /* Evaluation for 3 */
            int distanceFromCenter = m_colorMap.GetLength(0) / 2 - Math.Abs(shape[0].X - m_colorMap.GetLength(0) / 2);
            val += distanceFromCenter * 1;

            //Debug.WriteLine("val={0}", val);

            return 10000-val;
        }
    }
}

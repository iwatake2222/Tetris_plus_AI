using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WindowsFormsTetris
{
    /***
     * Wrapper for keyboard input and AI input
     * memo: rotate key input is toggled
     *       All input from AI is toggled automatically
     ***/
    public class IOWrapper
    {
        private bool m_toggleRotate = false;

        public bool m_setKeyDownLeft
        {
            private get;
            set;
        }
        public bool m_setKeyDownRight
        {
            private get;
            set;
        }
        public bool m_setKeyDownDown
        {
            private get;
            set;
        }
        public bool m_setKeyDownRotate
        {
            private get;
            set;
        }
        public bool isKeyDownLeft()
        {
            bool ret = m_setKeyDownLeft | Keyboard.IsKeyDown(Key.Left);
            m_setKeyDownLeft = false;
            return ret;
        }
        public bool isKeyDownRight()
        {
            bool ret = m_setKeyDownRight |= Keyboard.IsKeyDown(Key.Right);
            m_setKeyDownRight = false;
            return ret;
        }
        public bool isKeyDownDown()
        {
            bool ret = m_setKeyDownDown |= Keyboard.IsKeyDown(Key.Down);
            m_setKeyDownDown = false;
            return ret;
        }
        public bool isKeyDownRotate()
        {
            bool isKeyDown = false;
            if (Keyboard.IsKeyDown(Key.Up) || Keyboard.IsKeyDown(Key.Space)) {
                if (!m_toggleRotate) {
                    isKeyDown = true;
                    m_toggleRotate = true;
                }
            } else {
                m_toggleRotate = false;
            }
            isKeyDown |= m_setKeyDownRotate;
            m_setKeyDownRotate = false;
            return isKeyDown;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XDNet
{
    public static class KeyboardHook
    {
        const byte KeyUp = 0x02;
        const byte KeyDown = 0x00;

        public static void PressKey(KeyCode key)
        {
            keybd_event((byte)key, 0, KeyDown);
            keybd_event((byte)key, 0, KeyUp);
        }

        public static void PressKey(KeyCode[] keys)
        {
            foreach (var key in keys)
                keybd_event((byte)key, 0, KeyDown);
            foreach (var key in keys)
                keybd_event((byte)key, 0, KeyUp);
        }

        public static void TypeText(string text)
        {
            string textOnly = new String(text.Where(Char.IsLetter).ToArray());
            byte[] asciiBytes = Encoding.ASCII.GetBytes(text);

            // TODO(matyas): implement upper case handling
            foreach (var asciiCode in asciiBytes)
            {
                keybd_event(asciiCode, 0, KeyDown);
                keybd_event(asciiCode, 0, KeyUp);
            }

        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte vk, byte scan, int dwFlags, int ptrExtraInfo = 0);
    }

    public enum KeyCode
    {
        LeftControl = 0xA2,
        RightControl = 0xA3,
        LeftShift = 0xA0,
        LeftWin = 0x5B,
        
        A_Key = 0x41, B_Key = 0x42, C_Key = 0x43, D_Key = 0x44, E_Key = 0x45,
        F_Key = 0x46, G_Key = 0x47, H_Key = 0x48, I_Key = 0x49, J_Key = 0x4A,
        K_Key = 0x4B, L_Key = 0x4C, M_Key = 0x4D, N_Key = 0x4E, O_Key = 0x4F,
        P_Key = 0x50, Q_Key = 0x51, R_Key = 0x52, S_Key = 0x53, T_Key = 0x54,
        U_Key = 0x55, V_Key = 0x56, W_Key = 0x57, X_Key = 0x58, Y_Key = 0x59,
        Z_Key = 0x5A
    };
}

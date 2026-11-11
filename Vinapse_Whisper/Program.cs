using System.Runtime.InteropServices;
using System.Text;

namespace Vinapse_Whisper
{
    class Program
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        public const uint WM_COPYDATA = 0x004A;

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0) return;

            string command = args[0];
            string targetWindowTitle = "Vinapse_IPC_Receiver";
            IntPtr targetHWnd = FindWindow(null, targetWindowTitle);

            if (targetHWnd != IntPtr.Zero)
            {
                // Convert to ASCII byte array with a null terminator (Matches AutoIt 'char' expectation)
                byte[] sBytes = Encoding.ASCII.GetBytes(command + '\0');

                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds.dwData = IntPtr.Zero;
                cds.cbData = sBytes.Length;

                // Allocate unmanaged memory and copy the byte array
                cds.lpData = Marshal.AllocHGlobal(sBytes.Length);
                Marshal.Copy(sBytes, 0, cds.lpData, sBytes.Length);

                SendMessage(targetHWnd, WM_COPYDATA, IntPtr.Zero, ref cds);

                Marshal.FreeHGlobal(cds.lpData);
            }
        }
    }
}
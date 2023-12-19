using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CheatMenuAssaultCubeSharp
{
    public partial class Form1 : Form
    {
        bool unlimitedAmmo = false;

        IntPtr baseAddress;
        IntPtr procHandle;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
        uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
        uint dwSize, uint dwFreeType);


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress,
        int dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle,
        uint dwProcessId);

        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
          byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        // Define constants for allocation type and memory protection
        [Flags]
        public enum AllocationType : uint
        {
            MEM_COMMIT = 0x1000,
            MEM_RESERVE = 0x2000,
            MEM_RESET = 0x80000,
            MEM_RESET_UNDO = 0x1000000
        }

        [Flags]
        public enum MemoryProtection : uint
        {
            PAGE_EXECUTE_READWRITE = 0x40
        }

        

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        bool AttachGame()
        {
            Process[] processes = Process.GetProcessesByName("ac_client");

            if (processes.Length > 0)
            {
                // Access the first process (you might need to handle multiple processes with the same name differently)
                Process targetProcess = processes[0];
               
                // Store the Process ID (PID) in a variable
                int processId = targetProcess.Id;
                
                // Get the base address of the main module (executable) of the process
                baseAddress = targetProcess.MainModule.BaseAddress;

                StatusLabel.Text = "Game found!";

                // Now you have the Process ID (processId) and the base address (baseAddress) to use as needed
                procHandle = targetProcess.Handle;

                return true;
            }
            else
            {
                StatusLabel.Text = "Game not found!";

                return false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                unlimitedAmmo = true;
            }
            else if(!checkBox1.Checked)
            {
                unlimitedAmmo = false;
            }
                
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //so our game doesnt edit address values if that address is not initalized.
            if (AttachGame())
            {
                if(unlimitedAmmo)
                {
                    int numOfBytes;
                    uint oldP;
                    byte[] hackAmmo = { 0x90, 0x90 };

                    VirtualProtectEx(procHandle, (IntPtr)0x004637E9, 2, 0x40, out oldP);

                    unsafe
                    {
                        WriteProcessMemory(procHandle, (IntPtr)0x004637E9, hackAmmo, (uint)hackAmmo.Length, out numOfBytes);
                    }
                }
            }
        }
      
    }
}

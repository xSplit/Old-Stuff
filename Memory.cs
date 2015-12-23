using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
 
namespace MemoryManager
{
    /*
     *  Developed by Sam (xSplit)
     */
 
    abstract class PInvokeProcess
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        protected static extern int OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern int GetProcessId(IntPtr handle);
        [DllImport("kernel32.dll")]
        protected static extern int CloseHandle(int handle);
 
        /// <summary>
        /// Full access
        /// </summary>
        public const int access = 0x001F0FFF;
 
    }
 
    class MemoryManager : PInvokeProcess
    {
        private int _pint;
 
        /// <summary>
        /// Access point of the process
        /// </summary>
        public int pint
        {
            get { return _pint; }
            private set { _pint = value;  if (value == 0) Exc("Invalid process access"); }
        }
 
        /// <summary>
        /// New memory manager by process name
        /// </summary>
        /// <param name="name">Process name</param>
        /// <param name="index">Index of the process</param>
        public MemoryManager(string name, int index=0)
        {
            Process[] p = Process.GetProcessesByName(name);
            if (p.Length > 0)
                pint = OpenProcess(access, false, p[index].Id);
            else
                Exc(name+" isn't a valid process");
        }
 
        /// <summary>
        /// New memory manager by process handle
        /// </summary>
        /// <param name="handle">Process handle</param>
        public MemoryManager(IntPtr handle)
        {
            pint = OpenProcess(access, false, GetProcessId(handle));
        }
 
        /// <summary>
        /// New memory manager by process id
        /// </summary>
        /// <param name="name">Process id</param>
        public MemoryManager(int id)
        {
            pint = OpenProcess(access, false, id);
        }
 
 
        /// <summary>
        /// Write data on a memory address of the process
        /// </summary>
        /// <param name="address">Memory address</param>
        /// <param name="data">Data to write</param>
        /// <returns>Bool that checks if all data has been written</returns>
        public bool WriteAddress(int address, byte[] data)
        {
            int byteswrite;
 
            WriteProcessMemory(pint, address, data, data.Length, out byteswrite);
 
            return byteswrite == data.Length;
        }
 
        /// <summary>
        /// Read and returns the value of an address
        /// </summary>
        /// <param name="address">Memory address</param>
        /// <param name="length">Length of the bytes to read</param>
        /// <returns>The address value as byte array</returns>
        public byte[] ReadAddress(int address, int length)
        {
            int bytesread;
            byte[] read = new byte[length];
 
            ReadProcessMemory(pint, address, read, length, out bytesread);
 
            return read;
        }
 
        /// <summary>
        /// Returns and gets a list of address from a range of address where the value is correct with the condition
        /// </summary>
        /// <param name="length">Length of the value</param>
        /// <param name="check">Value condition as delegate</param>
        /// <param name="range">Address range to check</param>
        /// <returns>Returns a list of addresses by a value condition</returns>
        public List<int> getAddressesWithValue(int length, Func<byte[],bool> check, IEnumerable<int> range)
        {
            return range.Where(x => check(ReadAddress(x,length))).ToList();
        }
 
        /// <summary>
        /// Close the access point
        /// </summary>
        public void ClosePint()
        {
           CloseHandle(pint);
        }
 
        private void Exc(string message="Invalid process")
        {
           throw new Exception(message);
        }
    }
}

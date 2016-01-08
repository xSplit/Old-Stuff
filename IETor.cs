public struct Struct_INTERNET_PROXY_INFO
    {
        public int dwAccessType;
        public IntPtr proxy;
        public IntPtr proxyBypass;
    };

    [DllImport("wininet.dll", SetLastError = true)]
    public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

    public static void RefreshProxy()
    {
        try
        {
            //RESTART TOR
            Struct_INTERNET_PROXY_INFO struct_IPI;
            struct_IPI.dwAccessType = 3;
            struct_IPI.proxy = Marshal.StringToHGlobalAnsi("socks=127.0.0.1:9050");
            struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI));
            Marshal.StructureToPtr(struct_IPI, intptrStruct, true);
            InternetSetOption(IntPtr.Zero, 38, intptrStruct, Marshal.SizeOf(struct_IPI));
        }
        catch (Exception){ }
    }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Alborz.WinUI.Services;

public static class FontCatalog
{
    public static IReadOnlyList<string> GetInstalledFamilies()
    {
        var names = new SortedSet<string>(StringComparer.CurrentCultureIgnoreCase) { "Segoe UI" };
        var dc = CreateCompatibleDC(IntPtr.Zero);
        if (dc == IntPtr.Zero) return names.ToArray();
        try
        {
            var filter = new LogFont { CharSet = 1, FaceName = string.Empty };
            FontEnumProc callback = (font, metrics, type, parameter) =>
            {
                var name = Marshal.PtrToStructure<LogFont>(font).FaceName;
                if (!string.IsNullOrWhiteSpace(name) && !name.StartsWith('@')) names.Add(name);
                return 1;
            };
            EnumFontFamiliesEx(dc, ref filter, callback, IntPtr.Zero, 0);
            GC.KeepAlive(callback);
            return names.ToArray();
        }
        finally { DeleteDC(dc); }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    private struct LogFont
    {
        public int Height, Width, Escapement, Orientation, Weight;
        public byte Italic, Underline, StrikeOut, CharSet, OutPrecision, ClipPrecision, Quality, PitchAndFamily;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string FaceName;
    }
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate int FontEnumProc(IntPtr font, IntPtr metrics, uint type, IntPtr parameter);
    [DllImport("gdi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, EntryPoint = "EnumFontFamiliesExW")]
    private static extern int EnumFontFamiliesEx(IntPtr dc, ref LogFont filter, FontEnumProc callback, IntPtr parameter, uint flags);
    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleDC(IntPtr dc);
    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteDC(IntPtr dc);
}


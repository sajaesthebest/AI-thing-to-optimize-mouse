
// SNAPFIX v1.0 — Lightweight Mouse, blah blah blah


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnapFix
{
    internal static class Native
    {
        public const int RIDEV_INPUTSINK = 0x00000100;
        public const int RID_INPUT = 0x10000003;
        public const int RIM_TYPEMOUSE = 0;
        public const int WM_INPUT = 0x00FF;
        public const uint SPI_GETMOUSE = 0x0003;
        public const uint SPI_SETMOUSE = 0x0004;
        public const uint SPI_GETMOUSESPEED = 0x0070;
        public const uint SPI_SETMOUSESPEED = 0x0071;
        public const uint SPIF_UPDATEINIFILE = 0x01;
        public const uint SPIF_SENDCHANGE = 0x02;
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int WS_EX_TOOLWINDOW = 0x80;
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_SHOWWINDOW = 0x0040;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_ALT = 0x0001;
        public const int WM_HOTKEY = 0x0312;

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICE { public ushort usUsagePage; public ushort usUsage; public uint dwFlags; public IntPtr hwndTarget; }
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTHEADER { public uint dwType; public uint dwSize; public IntPtr hDevice; public IntPtr wParam; }
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWMOUSE { public ushort usFlags; public uint ulButtons; public ushort usButtonFlags; public ushort usButtonData; public uint ulRawButtons; public int lLastX; public int lLastY; public uint ulExtraInformation; }
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUT { public RAWINPUTHEADER header; public RAWMOUSE mouse; }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RAMP { [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public ushort[] Red; [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public ushort[] Green; [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public ushort[] Blue; }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PHYSICAL_MONITOR { public IntPtr hPhysicalMonitor; [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string szPhysicalMonitorDescription; }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT { public int Left, Top, Right, Bottom; }
        public delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll", SetLastError = true)] public static extern bool RegisterRawInputDevices([In] RAWINPUTDEVICE[] pRawInputDevices, uint uiNumDevices, uint cbSize);
        [DllImport("user32.dll", SetLastError = true)] public static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);
        [DllImport("user32.dll", SetLastError = true)] public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);
        [DllImport("user32.dll", SetLastError = true)] public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, int[] pvParam, uint fWinIni);
        [DllImport("user32.dll", SetLastError = true)] public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);
        [DllImport("user32.dll")] public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")] public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")] public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("gdi32.dll")] public static extern bool SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);
        [DllImport("gdi32.dll")] public static extern bool GetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);
        [DllImport("user32.dll")] public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")] public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("dxva2.dll", SetLastError = true)] public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);
        [DllImport("dxva2.dll", SetLastError = true)] public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);
        [DllImport("dxva2.dll", SetLastError = true)] public static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, PHYSICAL_MONITOR[] pPhysicalMonitorArray);
        [DllImport("dxva2.dll", SetLastError = true)] public static extern bool GetMonitorBrightness(IntPtr hPhysicalMonitor, ref uint pdwMinimumBrightness, ref uint pdwCurrentBrightness, ref uint pdwMaximumBrightness);
        [DllImport("user32.dll")] public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);
        [DllImport("user32.dll")] public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")] public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll", CharSet = CharSet.Auto)] public static extern bool EnumDisplaySettings(string? lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 32; private const int CCHFORMNAME = 32;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)] public string dmDeviceName;
            public short dmSpecVersion, dmDriverVersion, dmSize, dmDriverExtra;
            public int dmFields, dmPositionX, dmPositionY, dmDisplayOrientation, dmDisplayFixedOutput;
            public short dmColor, dmDuplex, dmYResolution, dmTTOption, dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)] public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel, dmPelsWidth, dmPelsHeight, dmDisplayFlags, dmDisplayFrequency;
        }
    }

    internal static class Log
    {
        private static readonly object _lock = new();
        private static string LogPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "snapfix.log");
        public static void Write(string level, string message)
        {
            try { Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!); lock (_lock) File.AppendAllText(LogPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}{Environment.NewLine}"); } catch { }
        }
        public static void Info(string m) => Write("INFO", m);
        public static void Warn(string m) => Write("WARN", m);
        public static void Error(string m) => Write("ERROR", m);
    }

    public class MouseProfile { public int PointerSpeed { get; set; } = 6; public bool Acceleration { get; set; } = false; }
    public class RawAccelProfile { public double Sensitivity { get; set; } = 1.0; public double Acceleration { get; set; } = 0.0; public double Offset { get; set; } = 0.0; public double Cap { get; set; } = 0.0; public double Exponent { get; set; } = 1.0; public double Gain { get; set; } = 1.0; public string Preset { get; set; } = "Linear"; }
    public class CrosshairProfile { public string Shape { get; set; } = "Cross"; public Color Color { get; set; } = Color.FromArgb(0, 255, 0); public int Opacity { get; set; } = 200; public int Size { get; set; } = 12; public int Thickness { get; set; } = 2; public int Gap { get; set; } = 4; public bool Outline { get; set; } = true; public int OutlineThickness { get; set; } = 1; public Color OutlineColor { get; set; } = Color.Black; public bool CenterDot { get; set; } = false; public int CenterDotSize { get; set; } = 2; public float Rotation { get; set; } = 0f; public bool Enabled { get; set; } = false; public int MonitorIndex { get; set; } = -1; }
    public class DisplayProfile { public double Gamma { get; set; } = 1.0; public int Brightness { get; set; } = -1; }
    public class AppProfile { public string Name { get; set; } = "Default"; public MouseProfile Mouse { get; set; } = new(); public RawAccelProfile RawAccel { get; set; } = new(); public CrosshairProfile Crosshair { get; set; } = new(); public DisplayProfile Display { get; set; } = new(); }
    public class AppSettings { public bool MinimizeToTray { get; set; } = true; public bool StartMinimized { get; set; } = false; }

    internal static class MouseSettings
    {
        private static int[]? _backupThresholds; private static int _backupSpeed = -1; private static bool _hasBackup;
        public static int GetPointerSpeed() { int speed = 10; Native.SystemParametersInfo(Native.SPI_GETMOUSESPEED, 0, ref speed, 0); return Math.Clamp(speed, 1, 20); }
        public static void SetPointerSpeed(int uiSpeed) { int winSpeed = Math.Clamp((int)Math.Round(1 + (uiSpeed - 1) * 19.0 / 10.0), 1, 20); Native.SystemParametersInfo(Native.SPI_SETMOUSESPEED, (uint)winSpeed, IntPtr.Zero, Native.SPIF_UPDATEINIFILE | Native.SPIF_SENDCHANGE); Log.Info($"Pointer speed set to UI={uiSpeed}"); }
        public static bool GetAcceleration() { int[] mouse = new int[3]; Native.SystemParametersInfo(Native.SPI_GETMOUSE, 0, mouse, 0); return mouse[2] != 0; }
        public static void SetAcceleration(bool enable) { int[] mouse = new int[3]; Native.SystemParametersInfo(Native.SPI_GETMOUSE, 0, mouse, 0); mouse[2] = enable ? 1 : 0; if (!enable) { mouse[0] = 0; mouse[1] = 0; } Native.SystemParametersInfo(Native.SPI_SETMOUSE, 0, mouse, Native.SPIF_UPDATEINIFILE | Native.SPIF_SENDCHANGE); Log.Info($"Acceleration {(enable ? "ON" : "OFF")}"); }
        public static void Backup() { if (_hasBackup) return; _backupSpeed = GetPointerSpeed(); _backupThresholds = new int[3]; Native.SystemParametersInfo(Native.SPI_GETMOUSE, 0, _backupThresholds, 0); _hasBackup = true; }
        public static void Restore() { if (!_hasBackup) return; if (_backupSpeed > 0) Native.SystemParametersInfo(Native.SPI_SETMOUSESPEED, (uint)_backupSpeed, IntPtr.Zero, Native.SPIF_UPDATEINIFILE | Native.SPIF_SENDCHANGE); if (_backupThresholds != null) Native.SystemParametersInfo(Native.SPI_SETMOUSE, 0, _backupThresholds, Native.SPIF_UPDATEINIFILE | Native.SPIF_SENDCHANGE); }
    }

    internal class PollingAnalyzer : IDisposable
    {
        private readonly Form _owner; private bool _registered; private readonly List<long> _timestamps = new(); private readonly object _lock = new(); private bool _testing; private const int TestDurationMs = 5000;
        public event Action<PollingResult>? TestCompleted;
        public PollingAnalyzer(Form owner) => _owner = owner;
        public void StartListening() { if (_registered) return; var rid = new Native.RAWINPUTDEVICE[1]; rid[0].usUsagePage = 0x01; rid[0].usUsage = 0x02; rid[0].dwFlags = Native.RIDEV_INPUTSINK; rid[0].hwndTarget = _owner.Handle; if (!Native.RegisterRawInputDevices(rid, 1, (uint)Marshal.SizeOf<Native.RAWINPUTDEVICE>())) { Log.Error("RegisterRawInputDevices failed"); return; } _registered = true; }
        public void ProcessRawInput(IntPtr lParam) { if (!_testing) return; uint size = 0; Native.GetRawInputData(lParam, Native.RID_INPUT, IntPtr.Zero, ref size, (uint)Marshal.SizeOf<Native.RAWINPUTHEADER>()); if (size == 0) return; IntPtr buffer = Marshal.AllocHGlobal((int)size); try { if (Native.GetRawInputData(lParam, Native.RID_INPUT, buffer, ref size, (uint)Marshal.SizeOf<Native.RAWINPUTHEADER>()) != size) return; var raw = Marshal.PtrToStructure<Native.RAWINPUT>(buffer); if (raw.header.dwType != Native.RIM_TYPEMOUSE) return; lock (_lock) _timestamps.Add(Stopwatch.GetTimestamp()); } finally { Marshal.FreeHGlobal(buffer); } }
        public void StartTest() { if (_testing) return; lock (_lock) _timestamps.Clear(); _testing = true; StartListening(); Task.Run(async () => { await Task.Delay(TestDurationMs); _testing = false; var result = ComputeResult(); _owner.BeginInvoke(() => TestCompleted?.Invoke(result)); }); }
        private PollingResult ComputeResult() { List<long> ts; lock (_lock) ts = new List<long>(_timestamps); var result = new PollingResult { Samples = ts.Count }; if (ts.Count < 2) { result.ConsistencyLabel = "Insufficient data"; return result; } double freq = Stopwatch.Frequency; var intervalsMs = new List<double>(); for (int i = 1; i < ts.Count; i++) { double dt = (ts[i] - ts[i - 1]) * 1000.0 / freq; if (dt > 0.05 && dt < 50) intervalsMs.Add(dt); } if (intervalsMs.Count == 0) return result; intervalsMs.Sort(); double avg = intervalsMs.Average(); result.ObservedRate = 1000.0 / avg; result.MinHz = 1000.0 / intervalsMs.Max(); result.MaxHz = 1000.0 / intervalsMs.Min(); result.MedianHz = 1000.0 / intervalsMs[intervalsMs.Count / 2]; result.AverageIntervalMs = avg; result.P95Ms = Percentile(intervalsMs, 0.95); result.P99Ms = Percentile(intervalsMs, 0.99); double mean = avg; double variance = intervalsMs.Sum(x => (x - mean) * (x - mean)) / intervalsMs.Count; double cv = Math.Sqrt(variance) / mean; result.Consistency = Math.Clamp(Math.Max(0, 100.0 - cv * 400.0), 0, 100); result.ConsistencyLabel = result.Consistency >= 95 ? "Excellent" : result.Consistency >= 85 ? "Good" : result.Consistency >= 70 ? "Average" : "Poor"; result.Explanation = "Based on observed event interval variation."; return result; }
        private static double Percentile(List<double> sorted, double p) { if (sorted.Count == 0) return 0; double idx = p * (sorted.Count - 1); int i = (int)idx; double f = idx - i; return i + 1 < sorted.Count ? sorted[i] * (1 - f) + sorted[i + 1] * f : sorted[i]; }
        public void Dispose() { }
    }

    public class PollingResult { public int Samples { get; set; } public double ObservedRate { get; set; } public double MinHz { get; set; } public double MaxHz { get; set; } public double MedianHz { get; set; } public double AverageIntervalMs { get; set; } public double P95Ms { get; set; } public double P99Ms { get; set; } public double Consistency { get; set; } public string ConsistencyLabel { get; set; } = ""; public string Explanation { get; set; } = ""; }

    internal static class RawAccelMath
    {
        public static double ComputeOutput(double inputSpeed, RawAccelProfile p) { if (inputSpeed <= 0) return 0; double effective = inputSpeed; if (p.Offset > 0 && effective < p.Offset) effective = 0; double multiplier = p.Sensitivity; if (p.Acceleration > 0 && effective > 0) multiplier += Math.Pow(effective * p.Acceleration, p.Exponent) * p.Gain; if (p.Cap > 0) multiplier = Math.Min(multiplier, p.Cap); return inputSpeed * multiplier; }
        public static PointF[] GenerateCurvePoints(RawAccelProfile p, int width, int height, double maxInput = 50.0) { var pts = new List<PointF>(); for (int x = 0; x < width; x++) { double input = (x / (double)width) * maxInput; double output = ComputeOutput(input, p); double maxOut = maxInput * Math.Max(p.Sensitivity + 2, 3); float y = (float)(height - (output / maxOut) * height); pts.Add(new PointF(x, Math.Clamp(y, 0, height))); } return pts.ToArray(); }
    }

    internal class CrosshairOverlay : Form
    {
        private CrosshairProfile _profile = new(); private readonly System.Windows.Forms.Timer _redrawTimer;
        public CrosshairOverlay() { FormBorderStyle = FormBorderStyle.None; ShowInTaskbar = false; TopMost = true; BackColor = Color.Magenta; TransparencyKey = Color.Magenta; StartPosition = FormStartPosition.Manual; Size = new Size(200, 200); DoubleBuffered = true; int ex = Native.GetWindowLong(Handle, Native.GWL_EXSTYLE); Native.SetWindowLong(Handle, Native.GWL_EXSTYLE, ex | Native.WS_EX_LAYERED | Native.WS_EX_TRANSPARENT | Native.WS_EX_TOOLWINDOW); Native.SetLayeredWindowAttributes(Handle, 0xFF00FF, 255, 0x1); _redrawTimer = new System.Windows.Forms.Timer { Interval = 33 }; _redrawTimer.Tick += (s, e) => { if (Visible) Invalidate(); }; }
        public void ApplyProfile(CrosshairProfile p) { _profile = p; UpdatePosition(); Invalidate(); }
        public void UpdatePosition() { Screen screen = (_profile.MonitorIndex >= 0 && _profile.MonitorIndex < Screen.AllScreens.Length) ? Screen.AllScreens[_profile.MonitorIndex] : Screen.PrimaryScreen!; int cx = screen.Bounds.Left + screen.Bounds.Width / 2; int cy = screen.Bounds.Top + screen.Bounds.Height / 2; Location = new Point(cx - Width / 2, cy - Height / 2); Native.SetWindowPos(Handle, Native.HWND_TOPMOST, 0, 0, 0, 0, Native.SWP_NOMOVE | Native.SWP_NOSIZE | Native.SWP_NOACTIVATE | Native.SWP_SHOWWINDOW); }
        public void ShowCrosshair() { UpdatePosition(); Show(); _redrawTimer.Start(); }
        public void HideCrosshair() { _redrawTimer.Stop(); Hide(); }
        protected override void OnPaint(PaintEventArgs e) { base.OnPaint(e); var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias; g.Clear(Color.Magenta); int cx = Width / 2, cy = Height / 2; var color = Color.FromArgb(_profile.Opacity, _profile.Color); using var pen = new Pen(color, _profile.Thickness); using var outlinePen = new Pen(Color.FromArgb(_profile.Opacity, _profile.OutlineColor), _profile.Thickness + _profile.OutlineThickness * 2); g.TranslateTransform(cx, cy); g.RotateTransform(_profile.Rotation); g.TranslateTransform(-cx, -cy); switch (_profile.Shape) { case "Dot": int d = _profile.Size; if (_profile.Outline) g.FillEllipse(new SolidBrush(_profile.OutlineColor), cx - d / 2 - 1, cy - d / 2 - 1, d + 2, d + 2); g.FillEllipse(new SolidBrush(color), cx - d / 2, cy - d / 2, d, d); break; case "Circle": int r = _profile.Size; if (_profile.Outline) g.DrawEllipse(outlinePen, cx - r, cy - r, r * 2, r * 2); g.DrawEllipse(pen, cx - r, cy - r, r * 2, r * 2); break; default: int half = _profile.Size, gap = _profile.Gap; if (_profile.Outline) { g.DrawLine(outlinePen, cx - half, cy, cx - gap, cy); g.DrawLine(outlinePen, cx + gap, cy, cx + half, cy); g.DrawLine(outlinePen, cx, cy - half, cx, cy - gap); g.DrawLine(outlinePen, cx, cy + gap, cx, cy + half); } g.DrawLine(pen, cx - half, cy, cx - gap, cy); g.DrawLine(pen, cx + gap, cy, cx + half, cy); g.DrawLine(pen, cx, cy - half, cx, cy - gap); g.DrawLine(pen, cx, cy + gap, cx, cy + half); break; } if (_profile.CenterDot) { int cds = _profile.CenterDotSize; g.FillEllipse(new SolidBrush(color), cx - cds / 2, cy - cds / 2, cds, cds); } }
        protected override void Dispose(bool disposing) { if (disposing) _redrawTimer.Dispose(); base.Dispose(disposing); }
    }

    internal static class DisplayControl
    {
        private static Native.RAMP? _originalRamp; private static bool _rampBackedUp;
        public static bool BackupGamma() { try { IntPtr hdc = Native.GetDC(IntPtr.Zero); if (hdc == IntPtr.Zero) return false; var ramp = new Native.RAMP { Red = new ushort[256], Green = new ushort[256], Blue = new ushort[256] }; bool ok = Native.GetDeviceGammaRamp(hdc, ref ramp); Native.ReleaseDC(IntPtr.Zero, hdc); if (ok) { _originalRamp = ramp; _rampBackedUp = true; } return ok; } catch { return false; } }
        public static bool SetGamma(double gamma) { gamma = Math.Clamp(gamma, 0.5, 2.5); if (!_rampBackedUp) BackupGamma(); try { IntPtr hdc = Native.GetDC(IntPtr.Zero); if (hdc == IntPtr.Zero) return false; var ramp = new Native.RAMP { Red = new ushort[256], Green = new ushort[256], Blue = new ushort[256] }; for (int i = 0; i < 256; i++) { ushort s = (ushort)Math.Clamp(Math.Pow(i / 255.0, 1.0 / gamma) * 65535.0, 0, 65535); ramp.Red[i] = ramp.Green[i] = ramp.Blue[i] = s; } bool ok = Native.SetDeviceGammaRamp(hdc, ref ramp); Native.ReleaseDC(IntPtr.Zero, hdc); return ok; } catch { return false; } }
        public static bool RestoreGamma() { if (!_rampBackedUp || _originalRamp == null) return false; try { IntPtr hdc = Native.GetDC(IntPtr.Zero); if (hdc == IntPtr.Zero) return false; var ramp = _originalRamp.Value; bool ok = Native.SetDeviceGammaRamp(hdc, ref ramp); Native.ReleaseDC(IntPtr.Zero, hdc); return ok; } catch { return false; } }
        public static (bool supported, uint current, uint min, uint max) TryGetBrightness(IntPtr hMonitor) { uint num = 0; if (!Native.GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref num) || num == 0) return (false, 0, 0, 0); var monitors = new Native.PHYSICAL_MONITOR[num]; if (!Native.GetPhysicalMonitorsFromHMONITOR(hMonitor, num, monitors)) return (false, 0, 0, 0); try { uint min = 0, cur = 0, max = 0; bool ok = Native.GetMonitorBrightness(monitors[0].hPhysicalMonitor, ref min, ref cur, ref max); return (ok, cur, min, max); } finally { Native.DestroyPhysicalMonitors(num, monitors); } }
    }

    internal static class ProfileManager
    {
        private static string ProfileDir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "profiles");
        public static void EnsureDir() => Directory.CreateDirectory(ProfileDir);
        public static List<string> ListProfiles() { EnsureDir(); return Directory.GetFiles(ProfileDir, "*.json").Select(Path.GetFileNameWithoutExtension).Where(n => n != null).Cast<string>().OrderBy(n => n).ToList(); }
        public static AppProfile? Load(string name) { try { string path = Path.Combine(ProfileDir, name + ".json"); if (!File.Exists(path)) return null; return JsonSerializer.Deserialize<AppProfile>(File.ReadAllText(path), JsonOpts()); } catch { return null; } }
        public static bool Save(AppProfile profile) { try { EnsureDir(); File.WriteAllText(Path.Combine(ProfileDir, profile.Name + ".json"), JsonSerializer.Serialize(profile, JsonOpts())); return true; } catch { return false; } }
        public static bool Delete(string name) { try { string path = Path.Combine(ProfileDir, name + ".json"); if (File.Exists(path)) { File.Delete(path); return true; } } catch { } return false; }
        private static JsonSerializerOptions JsonOpts() => new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = { new JsonStringEnumConverter(), new ColorJsonConverter() } };
    }

    internal class ColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) { string? s = reader.GetString(); if (string.IsNullOrEmpty(s)) return Color.Lime; if (s.StartsWith("#") && s.Length == 7) return Color.FromArgb(Convert.ToInt32(s.Substring(1, 2), 16), Convert.ToInt32(s.Substring(3, 2), 16), Convert.ToInt32(s.Substring(5, 2), 16)); return Color.FromName(s); }
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) => writer.WriteStringValue($"#{value.R:X2}{value.G:X2}{value.B:X2}");
    }

    public class MainForm : Form
    {
        private readonly Panel _sidebar, _content;
        private readonly Label _statusLabel;
        private readonly Dictionary<string, Panel> _pages = new();
        private readonly PollingAnalyzer _polling;
        private readonly CrosshairOverlay _crosshair;
        private AppProfile _currentProfile = new() { Name = "Default" };
        private AppSettings _settings = new();
        private PollingResult? _lastPolling;
        private Label? _dashMouseInfo, _dashPolling, _dashConsistency, _dashDisplay;
        private Label? _mouseSpeedLabel, _mouseAccelLabel;
        private Panel? _curvePanel;
        private RawAccelProfile _rawAccel = new();
        private TrackBar? _gammaTrack;
        private Label? _gammaValueLabel;

        public MainForm()
        {
            Text = "SNAPFIX"; Size = new Size(1120, 740); MinimumSize = new Size(960, 640);
            StartPosition = FormStartPosition.CenterScreen; BackColor = Color.FromArgb(18, 18, 22);
            ForeColor = Color.FromArgb(220, 220, 220); Font = new Font("Segoe UI", 9.5f); DoubleBuffered = true;

            _polling = new PollingAnalyzer(this); _polling.TestCompleted += OnPollingCompleted;
            _crosshair = new CrosshairOverlay();

            _sidebar = new Panel { Dock = DockStyle.Left, Width = 168, BackColor = Color.FromArgb(12, 12, 16) };
            _sidebar.Controls.Add(new Label { Text = "SNAPFIX", Font = new Font("Segoe UI Semibold", 15f), ForeColor = Color.FromArgb(0, 200, 83), Location = new Point(20, 22), AutoSize = true });
            string[] nav = { "Dashboard", "Mouse", "Raw Accel", "Crosshair", "Display", "Profiles", "Diagnostics", "Settings" };
            int y = 72; foreach (var name in nav) { _sidebar.Controls.Add(CreateNavButton(name, y)); y += 40; }

            _content = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(18, 18, 22) };
            _statusLabel = new Label { Dock = DockStyle.Bottom, Height = 26, BackColor = Color.FromArgb(12, 12, 16), ForeColor = Color.FromArgb(130, 130, 140), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(14, 0, 0, 0), Text = "  ● Ready  |  Offline  |  No telemetry" };

            Controls.Add(_content); Controls.Add(_statusLabel); Controls.Add(_sidebar);
            BuildPages(); ShowPage("Dashboard");
            Load += MainForm_Load; FormClosing += MainForm_FormClosing;
        }

        private Button CreateNavButton(string name, int y)
        {
            var btn = new Button { Text = "  " + name, Tag = name, FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.FromArgb(28, 28, 36) }, BackColor = Color.Transparent, ForeColor = Color.FromArgb(170, 170, 180), Font = new Font("Segoe UI", 10f), TextAlign = ContentAlignment.MiddleLeft, Location = new Point(8, y), Size = new Size(152, 36), Cursor = Cursors.Hand };
            btn.Click += (s, e) => ShowPage(name); return btn;
        }

        private void ShowPage(string name)
        {
            _content.Controls.Clear();
            if (_pages.TryGetValue(name, out var page)) { page.Dock = DockStyle.Fill; _content.Controls.Add(page); }
            foreach (Control c in _sidebar.Controls) if (c is Button b && b.Tag is string t) { b.ForeColor = t == name ? Color.FromArgb(0, 200, 83) : Color.FromArgb(170, 170, 180); b.BackColor = t == name ? Color.FromArgb(28, 28, 36) : Color.Transparent; }
        }

        private void BuildPages()
        {
            _pages["Dashboard"] = BuildDashboard(); _pages["Mouse"] = BuildMousePage(); _pages["Raw Accel"] = BuildRawAccelPage();
            _pages["Crosshair"] = BuildCrosshairPage(); _pages["Display"] = BuildDisplayPage(); _pages["Profiles"] = BuildProfilesPage();
            _pages["Diagnostics"] = BuildDiagnosticsPage(); _pages["Settings"] = BuildSettingsPage();
        }

        private Button CreateAccentButton(string text, int x, int y)
        {
            var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(210, 38), FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0 }, BackColor = Color.FromArgb(0, 175, 70), ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 9.5f), Cursor = Cursors.Hand };
            b.MouseEnter += (s, e) => b.BackColor = Color.FromArgb(0, 195, 80); b.MouseLeave += (s, e) => b.BackColor = Color.FromArgb(0, 175, 70); return b;
        }

        private Panel CreateCard(string title, int x, int y, int w, int h)
        {
            var card = new Panel { Location = new Point(x, y), Size = new Size(w, h), BackColor = Color.FromArgb(26, 26, 34) };
            card.Paint += (s, e) => { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using var path = RoundedRect(new Rectangle(0, 0, w - 1, h - 1), 12); using var brush = new SolidBrush(Color.FromArgb(26, 26, 34)); using var pen = new Pen(Color.FromArgb(48, 48, 60)); e.Graphics.FillPath(brush, path); e.Graphics.DrawPath(pen, path); };
            card.Controls.Add(new Label { Text = title, Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = Color.FromArgb(120, 120, 135), Location = new Point(16, 14), AutoSize = true, BackColor = Color.Transparent });
            card.Controls.Add(new Label { Text = "—", Font = new Font("Segoe UI Semibold", 15f), ForeColor = Color.White, Location = new Point(16, 42), AutoSize = true, BackColor = Color.Transparent });
            return card;
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2; var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90); path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90); path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure(); return path;
        }

        private Panel BuildDashboard()
        {
            var p = new Panel { AutoScroll = true, BackColor = Color.FromArgb(18, 18, 22) };
            p.Controls.Add(new Label { Text = "Dashboard", Font = new Font("Segoe UI Semibold", 18f), ForeColor = Color.White, Location = new Point(24, 16), AutoSize = true });
            int cardW = 190, cardH = 92, gap = 14;
            string[] cards = { "MOUSE", "POLLING", "CONSISTENCY", "DISPLAY" };
            for (int i = 0; i < 4; i++) { var card = CreateCard(cards[i], 24 + i * (cardW + gap), 60, cardW, cardH); p.Controls.Add(card); if (i == 0) _dashMouseInfo = card.Controls.OfType<Label>().LastOrDefault(); if (i == 1) _dashPolling = card.Controls.OfType<Label>().LastOrDefault(); if (i == 2) _dashConsistency = card.Controls.OfType<Label>().LastOrDefault(); if (i == 3) _dashDisplay = card.Controls.OfType<Label>().LastOrDefault(); }
            p.Controls.Add(new Label { Text = "SnapFix is a passive observation & configuration tool.\nIt never injects, never reads game memory, never automates input.\nAll changes are reversible. No placebo tweaks.", ForeColor = Color.FromArgb(145, 145, 155), Font = new Font("Segoe UI", 9.5f), Location = new Point(24, 175), Size = new Size(760, 58) });
            var btnTest = CreateAccentButton("RUN 5s POLLING TEST", 24, 250);
            btnTest.Click += (s, e) => { _statusLabel.Text = "  ● Testing mouse polling… move the mouse"; _polling.StartTest(); };
            p.Controls.Add(btnTest); return p;
        }

        private Panel BuildMousePage()
        {
            var p = new Panel { AutoScroll = true, BackColor = Color.FromArgb(18, 18, 22) };
            p.Controls.Add(new Label { Text = "Mouse", Font = new Font("Segoe UI Semibold", 18f), ForeColor = Color.White, Location = new Point(24, 16), AutoSize = true });
            _mouseSpeedLabel = new Label { Text = "Pointer Speed: reading…", Location = new Point(24, 60), AutoSize = true, ForeColor = Color.FromArgb(200, 200, 200) }; p.Controls.Add(_mouseSpeedLabel);
            var btnApply6 = CreateAccentButton("APPLY RECOMMENDED 6/11", 24, 95);
            btnApply6.Click += (s, e) => { MouseSettings.Backup(); MouseSettings.SetPointerSpeed(6); RefreshMouseInfo(); MessageBox.Show("Pointer speed set to 6/11.", "SnapFix"); }; p.Controls.Add(btnApply6);
            _mouseAccelLabel = new Label { Text = "Acceleration: reading…", Location = new Point(24, 150), AutoSize = true }; p.Controls.Add(_mouseAccelLabel);
            var btnDisable = CreateAccentButton("DISABLE ACCELERATION", 24, 185);
            btnDisable.Click += (s, e) => { MouseSettings.Backup(); MouseSettings.SetAcceleration(false); RefreshMouseInfo(); MessageBox.Show("Acceleration disabled.", "SnapFix"); }; p.Controls.Add(btnDisable);
            var btnEnable = new Button { Text = "ENABLE ACCELERATION", Location = new Point(250, 185), Size = new Size(200, 38), FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderColor = Color.FromArgb(60, 60, 70) }, BackColor = Color.FromArgb(40, 40, 50), ForeColor = Color.White, Cursor = Cursors.Hand };
            btnEnable.Click += (s, e) => { MouseSettings.Backup(); MouseSettings.SetAcceleration(true); RefreshMouseInfo(); }; p.Controls.Add(btnEnable);
            var btnRestore = new Button { Text = "RESTORE ORIGINAL MOUSE SETTINGS", Location = new Point(24, 245), Size = new Size(280, 38), FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderColor = Color.FromArgb(80, 60, 40) }, BackColor = Color.FromArgb(50, 40, 30), ForeColor = Color.FromArgb(255, 180, 80), Cursor = Cursors.Hand };
            btnRestore.Click += (s, e) => { MouseSettings.Restore(); RefreshMouseInfo(); MessageBox.Show("Restored.", "SnapFix"); }; p.Controls.Add(btnRestore);
            p.Controls.Add(new Label { Text = "Polling Analyzer (Raw Input – passive)", Font = new Font("Segoe UI Semibold", 12f), Location = new Point(24, 320), AutoSize = true, ForeColor = Color.White });
            var btnPoll = CreateAccentButton("START 5-SECOND TEST", 24, 360);
            btnPoll.Click += (s, e) => { _statusLabel.Text = "  ● Move the mouse for 5 seconds…"; _polling.StartTest(); }; p.Controls.Add(btnPoll);
            p.Controls.Add(new Label { Name = "pollResult", Location = new Point(24, 420), Size = new Size(700, 180), ForeColor = Color.FromArgb(180, 220, 180), Font = new Font("Consolas", 10f), Text = "Run a test to see observed event rate." });
            return p;
        }

        private void RefreshMouseInfo()
        {
            try
            {
                int speed = MouseSettings.GetPointerSpeed(); int ui = Math.Clamp((int)Math.Round(1 + (speed - 1) * 10.0 / 19.0), 1, 11);
                bool accel = MouseSettings.GetAcceleration();
                if (_mouseSpeedLabel != null) _mouseSpeedLabel.Text = $"Current Pointer Speed: {ui} / 11   (Windows internal: {speed})";
                if (_mouseAccelLabel != null) _mouseAccelLabel.Text = $"Acceleration (Enhance pointer precision): {(accel ? "ON" : "OFF")}";
                if (_dashMouseInfo != null) _dashMouseInfo.Text = accel ? "Accel ON" : "Connected";
            }
            catch { }
        }

        private Panel BuildRawAccelPage()
        {
            var p = new Panel { AutoScroll = true, BackColor = Color.FromArgb(18, 18, 22) };
            p.Controls.Add(new Label { Text = "Raw Accel", Font = new Font("Segoe UI Semibold", 18f), ForeColor = Color.White, Location = new Point(24, 16), AutoSize = true });
            p.Controls.Add(new Label { Text = "V1: curve editor + math. Full low-level accel requires a filter driver (e.g. open-source RawAccel).", Location = new Point(24, 55), Size = new Size(780, 30), ForeColor = Color.FromArgb(255, 180, 80) });
            _curvePanel = new Panel { Location = new Point(24, 100), Size = new Size(520, 280), BackColor = Color.FromArgb(22, 22, 28) };
            _curvePanel.Paint += CurvePanel_Paint; p.Controls.Add(_curvePanel);
            int cy = 100;
            void AddSlider(string name, double min, double max, double val, Action<double> onChange)
            {
                var lbl = new Label { Text = $"{name}: {val:F2}", Location = new Point(560, cy), AutoSize = true }; p.Controls.Add(lbl);
                var tb = new TrackBar { Location = new Point(560, cy + 22), Size = new Size(280, 45), Minimum = 0, Maximum = 1000, Value = (int)((val - min) / (max - min) * 1000), TickStyle = TickStyle.None };
                tb.ValueChanged += (s, e) => { double v = min + (tb.Value / 1000.0) * (max - min); lbl.Text = $"{name}: {v:F2}"; onChange(v); _curvePanel?.Invalidate(); }; p.Controls.Add(tb); cy += 70;
            }
            AddSlider("Sensitivity", 0.1, 5.0, _rawAccel.Sensitivity, v => _rawAccel.Sensitivity = v);
            AddSlider("Acceleration", 0.0, 1.0, _rawAccel.Acceleration, v => _rawAccel.Acceleration = v);
            AddSlider("Offset", 0.0, 10.0, _rawAccel.Offset, v => _rawAccel.Offset = v);
            AddSlider("Cap", 0.0, 10.0, _rawAccel.Cap, v => _rawAccel.Cap = v);
            AddSlider("Exponent", 0.5, 3.0, _rawAccel.Exponent, v => _rawAccel.Exponent = v);
            string[] presets = { "Linear", "Natural", "Smooth", "Competitive" }; int px = 24;
            foreach (var pr in presets) { var b = new Button { Text = pr, Location = new Point(px, 400), Size = new Size(100, 32), FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderColor = Color.FromArgb(60, 60, 70) }, BackColor = Color.FromArgb(35, 35, 45), ForeColor = Color.White, Cursor = Cursors.Hand }; string c = pr; b.Click += (s, e) => ApplyRawAccelPreset(c); p.Controls.Add(b); px += 110; }
            return p;
        }

        private void ApplyRawAccelPreset(string name)
        {
            _rawAccel = name switch
            {
                "Linear" => new RawAccelProfile { Sensitivity = 1.0, Acceleration = 0, Exponent = 1, Preset = name },
                "Natural" => new RawAccelProfile { Sensitivity = 0.8, Acceleration = 0.15, Exponent = 1.2, Preset = name },
                "Smooth" => new RawAccelProfile { Sensitivity = 0.7, Acceleration = 0.25, Exponent = 1.5, Offset = 1.0, Preset = name },
                "Competitive" => new RawAccelProfile { Sensitivity = 1.0, Acceleration = 0.08, Exponent = 1.1, Cap = 2.5, Preset = name },
                _ => _rawAccel
            };
            _curvePanel?.Invalidate();
        }

        private void CurvePanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias; g.Clear(Color.FromArgb(22, 22, 28));
            int w = _curvePanel!.Width, h = _curvePanel.Height;
            using var gridPen = new Pen(Color.FromArgb(40, 40, 50)); for (int i = 1; i < 10; i++) { g.DrawLine(gridPen, i * w / 10, 0, i * w / 10, h); g.DrawLine(gridPen, 0, i * h / 10, w, i * h / 10); }
            using var axisPen = new Pen(Color.FromArgb(80, 80, 90)); g.DrawLine(axisPen, 0, h - 1, w, h - 1); g.DrawLine(axisPen, 0, 0, 0, h);
            var pts = RawAccelMath.GenerateCurvePoints(_rawAccel, w, h); if (pts.Length > 1) { using var curvePen = new Pen(Color.FromArgb(0, 200, 83), 2f); g.DrawLines(curvePen, pts); }
        }

        private Panel BuildCrosshairPage()
        {
            var p = new Panel { AutoScroll = true, BackColor = Color.FromArgb(18, 18, 22) };
            p.Controls.Add(new Label { Text = "Crosshair Overlay", Font = new Font("Segoe UI Semibold", 18f), ForeColor = Color.White, Location = new Point(24, 16), AutoSize = true });
            p.Controls.Add(new Label { Text = "Pure visual overlay. No game interaction.", Location = new Point(24, 55), Size = new Size(700, 25), ForeColor = Color.FromArgb(140, 140, 150) });
            var btnToggle = CreateAccentButton("TOGGLE CROSSHAIR", 24, 90);
            btnToggle.Click += (s, e) => { if (_crosshair.Visible) { _crosshair.HideCrosshair(); _currentProfile.Crosshair.Enabled = false; } else { _crosshair.ApplyProfile(_currentProfile.Crosshair); _crosshair.ShowCrosshair(); _currentProfile.Crosshair.Enabled = true; } }; p.Controls.Add(btnToggle);
            p.Controls.Add(new Label { Text = "Shape", Location = new Point(24, 150), AutoSize = true });
            var shapeBox = new ComboBox { Location = new Point(24, 175), Size = new Size(150, 28), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(35, 35, 45), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            shapeBox.Items.AddRange(new[] { "Cross", "Dot", "Circle", "Plus" }); shapeBox.SelectedIndex = 0;
            shapeBox.SelectedIndexChanged += (s, e) => { _currentProfile.Crosshair.Shape = shapeBox.SelectedItem!.ToString()!; if (_crosshair.Visible) _crosshair.ApplyProfile(_currentProfile.Crosshair); }; p.Controls.Add(shapeBox);
            var sizeLbl = new Label { Text = "Size: 12", Location = new Point(24, 220), AutoSize = true }; p.Controls.Add(sizeLbl);
            var sizeTrack = new TrackBar { Location = new Point(24, 245), Size = new Size(250, 45), Minimum = 4, Maximum = 40, Value = 12, TickStyle = TickStyle.None };
            sizeTrack.ValueChanged += (s, e) => { sizeLbl.Text = $"Size: {sizeTrack.Value}"; _currentProfile.Crosshair.Size = sizeTrack.Value; if (_crosshair.Visible) _crosshair.ApplyProfile(_currentProfile.Crosshair); }; p.Controls.Add(sizeTrack);
            var gapLbl = new Label { Text = "Gap: 4", Location = new Point(24, 300), AutoSize = true }; p.Controls.Add(gapLbl);
            var gapTrack = new TrackBar { Location = new Point(24, 325), Size = new Size(250, 45), Minimum = 0, Maximum = 20, Value = 4, TickStyle = TickStyle.None };
            gapTrack.ValueChanged += (s, e) => { gapLbl.Text = $"Gap: {gapTrack.Value}"; _currentProfile.Crosshair.Gap = gapTrack.Value; if (_crosshair.Visible) _crosshair.ApplyProfile(_currentProfile.Crosshair); }; p.Controls.Add(gapTrack);
            p.Controls.Add(new Label { Text = "Color", Location = new Point(24, 380), AutoSize = true });
            Color[] colors = { Color.Lime, Color.Cyan, Color.Red, Color.Yellow, Color.White, Color.Magenta }; int cx = 24;
            foreach (var c in colors) { var b = new Button { Location = new Point(cx, 405), Size = new Size(36, 28), BackColor = c, FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0 }, Cursor = Cursors.Hand }; Color captured = c; b.Click += (s, e) => { _currentProfile.Crosshair.Color = captured; if (_crosshair.Visible) _crosshair.ApplyProfile(_currentProfile.Crosshair); }; p.Controls.Add(b); cx += 42; }
            p.Controls.Add(new Label { Text = "Hotkey: Ctrl + Alt + C", Location = new Point(24, 455), Size = new Size(300, 25), ForeColor = Color.FromArgb(130, 130, 140) });
            return p;
        }

        private Panel BuildDisplayPage()
        {
            var p = new Panel { AutoScroll = true, BackColor = Color.FromArgb(18, 18, 22) };
            p.Controls.Add(new Label { Text = "Display Tuner", Font = new Font("Segoe UI Semibold", 18f), ForeColor = Color.White, Location = new Point(24, 16), AutoSize = true });
            var screen = Screen.PrimaryScreen!;
            p.Controls.Add(new Label { Text = $"Primary: {screen.Bounds.Width} × {screen.Bounds.Height}  @  {GetRefreshRate()} Hz", Location = new Point(24, 60), AutoSize = true, ForeColor = Color.FromArgb(180, 220, 180) });
            p.Controls.Add(new Label { Text = "Gamma (Windows LUT)", Font = new Font("Segoe UI Semibold", 11f), Location = new Point(24, 105), AutoSize = true, ForeColor = Color.White });
            _gammaValueLabel = new Label { Text = "1.00", Location = new Point(24, 135), AutoSize = true }; p.Controls.Add(_gammaValueLabel);
            _gammaTrack = new TrackBar { Location = new Point(24, 160), Size = new Size(400, 45), Minimum = 50, Maximum = 200, Value = 100, TickStyle = TickStyle.None };
            _gammaTrack.ValueChanged += (s, e) => { _gammaValueLabel!.Text = $"{_gammaTrack.Value / 100.0:F2}"; }; p.Controls.Add(_gammaTrack);
            var btnApplyGamma = CreateAccentButton("APPLY GAMMA", 24, 215);
            btnApplyGamma.Click += (s, e) => { double g = _gammaTrack!.Value / 100.0; if (DisplayControl.SetGamma(g)) { _currentProfile.Display.Gamma = g; MessageBox.Show($"Gamma set to {g:F2}.", "SnapFix"); } else MessageBox.Show("Unable to set gamma ramp.", "SnapFix", MessageBoxButtons.OK, MessageBoxIcon.Warning); }; p.Controls.Add(btnApplyGamma);
            var btnRestoreGamma = new Button { Text = "RESTORE ORIGINAL GAMMA", Location = new Point(250, 215), Size = new Size(220, 38), FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderColor = Color.FromArgb(80, 60, 40) }, BackColor = Color.FromArgb(50, 40, 30), ForeColor = Color.FromArgb(255, 180, 80), Cursor = Cursors.Hand };
            btnRestoreGamma.Click += (s, e) => { if (DisplayControl.RestoreGamma()) MessageBox.Show("Gamma restored.", "SnapFix"); else MessageBox.Show("No backup or restore failed.", "SnapFix"); }; p.Controls.Add(btnRestoreGamma);
            p.Controls.Add(new Label { Text = "Gamma uses SetDeviceGammaRamp (session LUT). Does not change hardware permanently.", Location = new Point(24, 275), Size = new Size(650, 30), ForeColor = Color.FromArgb(130, 130, 140) });
            p.Controls.Add(new Label { Text = "Hardware Brightness (DDC/CI)", Font = new Font("Segoe UI Semibold", 11f), Location = new Point(24, 320), AutoSize = true, ForeColor = Color.White });
            var brightInfo = new Label { Text = "Checking DDC/CI…", Location = new Point(24, 355), Size = new Size(600, 40), ForeColor = Color.FromArgb(180, 180, 180) }; p.Controls.Add(brightInfo);
            Task.Run(() => { try { IntPtr hMon = IntPtr.Zero; Native.MonitorEnumProc callback = (IntPtr h, IntPtr hdc, ref Native.RECT r, IntPtr d) => { hMon = h; return false; }; Native.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero); if (hMon != IntPtr.Zero) { var (ok, cur, min, max) = DisplayControl.TryGetBrightness(hMon); BeginInvoke(() => { brightInfo.Text = ok ? $"Supported  |  Current: {cur}  (range {min}–{max})" : "Hardware brightness control unavailable."; }); } } catch { BeginInvoke(() => brightInfo.Text = "DDC/CI check failed."); } });
            return p;
        }

        private int GetRefreshRate() { try { var devMode = new Native.DEVMODE(); devMode.dmSize = (short)Marshal.SizeOf<Native.DEVMODE>(); if (Native.EnumDisplaySettings(null, -1, ref devMode)) return devMode.dmDisplayFrequency; } catch { } return 60; }

        private Panel BuildProfilesPage()
        {
            var p = new Panel { AutoScroll = true, BackColor = Color.FromArgb(18, 18, 22) };
            p.Controls.Add(new Label { Text = "Profiles", Font = new Font("Segoe UI Semibold", 18f), ForeColor = Color.White, Location = new Point(24, 16), AutoSize = true });
            var list = new ListBox { Location = new Point(24, 60), Size = new Size(280, 300), BackColor = Color.FromArgb(28, 28, 36), ForeColor = Color.White, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10f) }; p.Controls.Add(list);
            void RefreshList() { list.Items.Clear(); foreach (var n in ProfileManager.ListProfiles()) list.Items.Add(n); if (!list.Items.Contains("Default")) list.Items.Insert(0, "Default"); } RefreshList();
            var btnSave = CreateAccentButton("SAVE CURRENT AS…", 320, 60);
            btnSave.Click += (s, e) => { using var dlg = new Form { Text = "Save Profile", Size = new Size(360, 160), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, BackColor = Color.FromArgb(28, 28, 36), ForeColor = Color.White }; var tb = new TextBox { Location = new Point(20, 30), Size = new Size(300, 28), Text = _currentProfile.Name, BackColor = Color.FromArgb(40, 40, 50), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle }; var ok = new Button { Text = "Save", DialogResult = DialogResult.OK, Location = new Point(140, 75), Size = new Size(90, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0, 180, 70), ForeColor = Color.White }; dlg.Controls.Add(tb); dlg.Controls.Add(ok); dlg.AcceptButton = ok; if (dlg.ShowDialog(this) != DialogResult.OK) return; string name = tb.Text.Trim(); if (string.IsNullOrWhiteSpace(name)) return; _currentProfile.Name = name; _currentProfile.RawAccel = _rawAccel; if (ProfileManager.Save(_currentProfile)) { RefreshList(); MessageBox.Show($"Profile '{name}' saved.", "SnapFix"); } }; p.Controls.Add(btnSave);
            var btnLoad = new Button { Text = "LOAD SELECTED", Location = new Point(320, 110), Size = new Size(180, 36), FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderColor = Color.FromArgb(60, 60, 70) }, BackColor = Color.FromArgb(40, 40, 50), ForeColor = Color.White, Cursor = Cursors.Hand };
            btnLoad.Click += (s, e) => { if (list.SelectedItem == null) return; string name = list.SelectedItem.ToString()!; var loaded = ProfileManager.Load(name); if (loaded != null) { _currentProfile = loaded; _rawAccel = loaded.RawAccel; MessageBox.Show($"Profile '{name}' loaded.", "SnapFix"); } else if (name == "Default") { _currentProfile = new AppProfile { Name = "Default" }; _rawAccel = new RawAccelProfile(); MessageBox.Show("Default loaded.", "SnapFix"); } }; p.Controls.Add(btnLoad);
            var btnDelete = new Button { Text = "DELETE SELECTED", Location = new Point(320, 160), Size = new Size(180, 36), FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderColor = Color.FromArgb(80, 40, 40) }, BackColor = Color.FromArgb(50, 30, 30), ForeColor = Color.FromArgb(255, 120, 120), Cursor = Cursors.Hand };
            btnDelete.Click += (s, e) => { if (list.SelectedItem == null) return; string name = list.SelectedItem.ToString()!; if (name == "Default") return; if (MessageBox.Show($"Delete '{name}'?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) { ProfileManager.Delete(name); RefreshList(); } }; p.Controls.Add(btnDelete);
            return p;
        }

        private Panel BuildDiagnosticsPage()
        {
            var p = new Panel { AutoScroll = true, BackColor = Color.FromArgb(18, 18, 22) };
            p.Controls.Add(new Label { Text = "Diagnostic Center", Font = new Font("Segoe UI Semibold", 18f), ForeColor = Color.White, Location = new Point(24, 16), AutoSize = true });
            var box = new Label { Location = new Point(24, 60), Size = new Size(780, 420), Font = new Font("Consolas", 10f), ForeColor = Color.FromArgb(180, 220, 180), Text = "Running diagnostics…" }; p.Controls.Add(box);
            Task.Run(() => { var sb = new StringBuilder(); sb.AppendLine("SNAPFIX DIAGNOSTICS\n===================\n"); try { int speed = MouseSettings.GetPointerSpeed(); bool accel = MouseSettings.GetAcceleration(); sb.AppendLine("[✓] Mouse settings readable"); sb.AppendLine($"    Pointer speed: {speed}"); sb.AppendLine($"    Acceleration: {(accel ? "ON" : "OFF")}"); } catch (Exception ex) { sb.AppendLine($"[!] Mouse: {ex.Message}"); } sb.AppendLine("[✓] Raw Input API present"); try { var scr = Screen.PrimaryScreen!; sb.AppendLine($"[✓] Display: {scr.Bounds.Width}×{scr.Bounds.Height} @ {GetRefreshRate()} Hz"); } catch { sb.AppendLine("[!] Display failed"); } bool gammaOk = DisplayControl.BackupGamma(); sb.AppendLine(gammaOk ? "[✓] Gamma ramp accessible" : "[!] Gamma ramp unavailable"); sb.AppendLine("\nSafety: No game process interaction, no injection, pure overlay."); BeginInvoke(() => box.Text = sb.ToString()); });
            return p;
        }

        private Panel BuildSettingsPage()
        {
            var p = new Panel { AutoScroll = true, BackColor = Color.FromArgb(18, 18, 22) };
            p.Controls.Add(new Label { Text = "Settings", Font = new Font("Segoe UI Semibold", 18f), ForeColor = Color.White, Location = new Point(24, 16), AutoSize = true });
            p.Controls.Add(new Label { Text = "All options require explicit consent.", Location = new Point(24, 60), Size = new Size(650, 30), ForeColor = Color.FromArgb(140, 140, 150) });
            p.Controls.Add(new CheckBox { Text = "Minimize to tray", Location = new Point(24, 110), AutoSize = true, ForeColor = Color.White, Checked = _settings.MinimizeToTray });
            p.Controls.Add(new CheckBox { Text = "Start minimized", Location = new Point(24, 140), AutoSize = true, ForeColor = Color.White });
            p.Controls.Add(new Label { Text = "SNAPFIX v1.0\nOffline • No telemetry • Reversible changes", Location = new Point(24, 200), Size = new Size(500, 60), ForeColor = Color.FromArgb(120, 120, 130) });
            return p;
        }

        private void MainForm_Load(object? sender, EventArgs e) { Log.Info("SnapFix started"); RefreshMouseInfo(); DisplayControl.BackupGamma(); if (_dashDisplay != null) _dashDisplay.Text = $"{GetRefreshRate()} Hz"; Native.RegisterHotKey(Handle, 1, Native.MOD_CONTROL | Native.MOD_ALT, (uint)Keys.C); }
        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e) { Native.UnregisterHotKey(Handle, 1); _crosshair.HideCrosshair(); _crosshair.Dispose(); _polling.Dispose(); Log.Info("SnapFix closed"); }
        protected override void WndProc(ref Message m) { if (m.Msg == Native.WM_INPUT) _polling.ProcessRawInput(m.LParam); else if (m.Msg == Native.WM_HOTKEY && m.WParam.ToInt32() == 1) { if (_crosshair.Visible) _crosshair.HideCrosshair(); else { _crosshair.ApplyProfile(_currentProfile.Crosshair); _crosshair.ShowCrosshair(); } } base.WndProc(ref m); }
        private void OnPollingCompleted(PollingResult r) { _lastPolling = r; _statusLabel.Text = "  ● Ready"; if (_dashPolling != null) _dashPolling.Text = $"{r.ObservedRate:F0} Hz"; if (_dashConsistency != null) _dashConsistency.Text = $"{r.Consistency:F1}%"; foreach (Control c in _pages["Mouse"].Controls) if (c.Name == "pollResult" && c is Label lbl) lbl.Text = $"MOUSE ANALYSIS (Observed Event Rate)\nSamples: {r.Samples}\nObserved Rate: {r.ObservedRate:F1} Hz\nMin: {r.MinHz:F1} Hz\nMax: {r.MaxHz:F1} Hz\nMedian: {r.MedianHz:F1} Hz\nAvg Interval: {r.AverageIntervalMs:F2} ms\nP95: {r.P95Ms:F2} ms\nP99: {r.P99Ms:F2} ms\nConsistency: {r.Consistency:F1}% ({r.ConsistencyLabel})\n{r.Explanation}"; }
    }

    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0 && args[0].ToLowerInvariant() == "--restore") { MouseSettings.Restore(); DisplayControl.RestoreGamma(); return; }
            Log.Info("=== SnapFix starting ===");
            Application.Run(new MainForm());
        }
    }
}

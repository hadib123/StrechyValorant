using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace Had1bTrueStretch {
static class Program {
 [STAThread] static void Main() {
  if (!IsAdmin()) { try { Process.Start(new ProcessStartInfo(Application.ExecutablePath) { Verb="runas", UseShellExecute=true }); } catch { MessageBox.Show("Administrator access is required."); } return; }
  Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false); Application.Run(new MainForm());
 }
 static bool IsAdmin() { return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator); }
}

public class MainForm : Form {
 const int NativeId=4001, StretchId=4002, WmHotkey=0x0312;
 readonly Color Bg=Color.FromArgb(10,13,20), Panel=Color.FromArgb(20,25,36), Red=Color.FromArgb(255,66,82), Cyan=Color.FromArgb(42,205,190), Muted=Color.FromArgb(147,157,177);
 Label status, nativeLabel, stretchLabel, help; NumericUpDown nativeWBox,nativeHBox,stretchWBox,stretchHBox; string capture;
 int nativeW=1920,nativeH=1080,stretchW=1280,stretchH=1080;
 Hotkey native=new Hotkey(Keys.F11, Mods.Control|Mods.Alt), stretch=new Hotkey(Keys.F12, Mods.Control|Mods.Alt);
 readonly string dataDir=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),"Had1bTrueStretch");
 string SettingsPath { get { return Path.Combine(dataDir,"settings.txt"); } }
 string MonitorPath { get { return Path.Combine(dataDir,"monitors.txt"); } }

 public MainForm() {
  Text="HAD1B True Stretch"; ClientSize=new Size(680,700); StartPosition=FormStartPosition.CenterScreen; FormBorderStyle=FormBorderStyle.FixedSingle; MaximizeBox=false;
  BackColor=Bg; ForeColor=Color.White; Font=new Font("Segoe UI",10); KeyPreview=true; Directory.CreateDirectory(dataDir); LoadSettings(); BuildUi();
  Shown += delegate { RegisterKeys(); }; FormClosed += delegate { UnregisterHotKey(Handle,NativeId); UnregisterHotKey(Handle,StretchId); }; KeyDown += CaptureKey;
 }
 void BuildUi() {
  Controls.Add(new Panel { BackColor=Red, Location=new Point(0,0), Size=new Size(680,6) });
  Controls.Add(new Label { Text="TRUE STRETCH", Font=new Font("Segoe UI",29,FontStyle.Bold), AutoSize=true, Location=new Point(31,24) });
  Controls.Add(new Label { Text="VALORANT DISPLAY CONTROL", Font=new Font("Segoe UI Semibold",10), ForeColor=Red, AutoSize=true, Location=new Point(36,80) });
  status=new Label { Text="READY", TextAlign=ContentAlignment.MiddleCenter, BackColor=Panel, ForeColor=Cyan, Font=new Font("Segoe UI Semibold",10), Location=new Point(36,116), Size=new Size(608,40) }; Controls.Add(status);
  Button nb=BigButton("NATIVE RES",36,174,Color.FromArgb(42,157,143)); Button sb=BigButton("STRETCHED\nRESOLUTION",350,174,Red);
  nb.Click += delegate { ApplyNative(); }; sb.Click += delegate { ApplyStretch(); };
  Controls.Add(new Label { Text="Native resolution",ForeColor=Muted,AutoSize=true,Location=new Point(36,314) });
  Controls.Add(new Label { Text="Stretched resolution",ForeColor=Muted,AutoSize=true,Location=new Point(350,314) });
  nativeWBox=ResolutionBox(nativeW,36,338); nativeHBox=ResolutionBox(nativeH,142,338); Controls.Add(new Label { Text="×",AutoSize=true,Location=new Point(127,343) });
  stretchWBox=ResolutionBox(stretchW,350,338); stretchHBox=ResolutionBox(stretchH,456,338); Controls.Add(new Label { Text="×",AutoSize=true,Location=new Point(441,343) });
  nativeWBox.ValueChanged+=ResolutionChanged; nativeHBox.ValueChanged+=ResolutionChanged; stretchWBox.ValueChanged+=ResolutionChanged; stretchHBox.ValueChanged+=ResolutionChanged;
  nativeLabel=HotkeyLabel(native.ToString(),36,380); stretchLabel=HotkeyLabel(stretch.ToString(),350,380);
  Button na=SmallButton("ASSIGN NATIVE HOTKEY",36,420), sa=SmallButton("ASSIGN STRETCHED HOTKEY",350,420);
  na.Click += delegate { BeginCapture("native"); }; sa.Click += delegate { BeginCapture("stretch"); };
  help=new Label { Text="Global hotkeys work while this app is running", TextAlign=ContentAlignment.MiddleCenter, ForeColor=Muted, Location=new Point(36,462), Size=new Size(608,28) }; Controls.Add(help);
  Button info=SmallButton("HOW TO USE",36,500); info.Size=new Size(608,38); info.Click+=delegate { ShowInformation(); };
  Controls.Add(new Panel { BackColor=Color.FromArgb(42,49,67), Location=new Point(36,553), Size=new Size(608,1) });
  Controls.Add(new Label { Text="MADE BY HAD1B", Font=new Font("Segoe UI",20,FontStyle.Bold), ForeColor=Red, TextAlign=ContentAlignment.MiddleCenter, Location=new Point(36,565), Size=new Size(608,42) });
  LinkLabel donate=new LinkLabel { Text="Support the app on PayPal",LinkColor=Cyan,ActiveLinkColor=Color.White,VisitedLinkColor=Cyan,TextAlign=ContentAlignment.MiddleCenter,Location=new Point(36,611),Size=new Size(608,28),Font=new Font("Segoe UI Semibold",10) };
  donate.LinkClicked+=delegate { Process.Start(new ProcessStartInfo("https://paypal.me/HadiBennagi"){UseShellExecute=true}); }; Controls.Add(donate);
  Controls.Add(new Label { Text="Standalone Windows app  •  No game injection", ForeColor=Muted, TextAlign=ContentAlignment.MiddleCenter, Location=new Point(36,652), Size=new Size(608,26) });
 }
 Button BigButton(string text,int x,int y,Color color) { Button b=new Button { Text=text, Location=new Point(x,y), Size=new Size(294,126), BackColor=color, ForeColor=Color.White, FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand, Font=new Font("Segoe UI Semibold",18,FontStyle.Bold) }; b.FlatAppearance.BorderSize=0; Controls.Add(b); return b; }
 Button SmallButton(string text,int x,int y) { Button b=new Button { Text=text, Location=new Point(x,y), Size=new Size(294,38), BackColor=Color.FromArgb(42,49,67), ForeColor=Color.White, FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand, Font=new Font("Segoe UI Semibold",9) }; b.FlatAppearance.BorderSize=0; Controls.Add(b); return b; }
 Label HotkeyLabel(string text,int x,int y) { Label l=new Label { Text=text, TextAlign=ContentAlignment.MiddleCenter, BackColor=Panel, ForeColor=Cyan, Location=new Point(x,y), Size=new Size(294,32) }; Controls.Add(l); return l; }
 NumericUpDown ResolutionBox(int value,int x,int y){NumericUpDown n=new NumericUpDown{Minimum=480,Maximum=7680,Value=value,Location=new Point(x,y),Size=new Size(92,26),BackColor=Panel,ForeColor=Color.White,BorderStyle=BorderStyle.FixedSingle,TextAlign=HorizontalAlignment.Center};Controls.Add(n);return n;}
 void ResolutionChanged(object sender,EventArgs e){nativeW=(int)nativeWBox.Value;nativeH=(int)nativeHBox.Value;stretchW=(int)stretchWBox.Value;stretchH=(int)stretchHBox.Value;SaveSettings();}
 void ShowInformation(){MessageBox.Show("RECOMMENDED ORDER\n\n1. Before the match or during Agent Select, press Native Res.\n2. Once you are in-game, press Stretched Resolution.\n\nIF YOU FORGOT\n\nYou can fix it while in-game. Press Native Res, open Valorant video settings, set Display Mode to Windowed Fullscreen and select Fill again, then press Stretched Resolution.\n\nYour chosen resolutions and hotkeys are saved automatically.","How to use HAD1B True Stretch",MessageBoxButtons.OK,MessageBoxIcon.Information);}

 void BeginCapture(string target) { capture=target; help.Text="Press a key combination now — Esc to cancel"; help.ForeColor=Color.Gold; Activate(); }
 void CaptureKey(object sender,KeyEventArgs e) {
  if (capture==null) return; e.SuppressKeyPress=true; if (e.KeyCode==Keys.Escape) { EndCapture(); return; }
  if (e.KeyCode==Keys.ControlKey||e.KeyCode==Keys.ShiftKey||e.KeyCode==Keys.Menu||e.KeyCode==Keys.LWin||e.KeyCode==Keys.RWin) return;
  Mods m=Mods.None; if(e.Control)m|=Mods.Control; if(e.Alt)m|=Mods.Alt; if(e.Shift)m|=Mods.Shift;
  if(m==Mods.None){ help.Text="Include Ctrl, Alt or Shift with the key"; return; }
  Hotkey h=new Hotkey(e.KeyCode,m); if(capture=="native")native=h; else stretch=h; SaveSettings(); RegisterKeys(); EndCapture();
 }
 void EndCapture(){ capture=null; nativeLabel.Text=native.ToString(); stretchLabel.Text=stretch.ToString(); help.Text="Global hotkeys work while this app is running"; help.ForeColor=Muted; }
 void RegisterKeys(){ if(!IsHandleCreated)return; UnregisterHotKey(Handle,NativeId); UnregisterHotKey(Handle,StretchId); bool a=RegisterHotKey(Handle,NativeId,(uint)native.Modifiers,(uint)native.Key), b=RegisterHotKey(Handle,StretchId,(uint)stretch.Modifiers,(uint)stretch.Key); if(!a||!b)SetStatus("A HOTKEY IS ALREADY IN USE",Red); }
 protected override void WndProc(ref Message m){ if(m.Msg==WmHotkey){ if(m.WParam.ToInt32()==NativeId)ApplyNative(); if(m.WParam.ToInt32()==StretchId)ApplyStretch(); } base.WndProc(ref m); }

 void ApplyNative(){ RunAction("ENABLING MONITORS...",delegate{ EnableAll(); Thread.Sleep(900); SetResolution(nativeW,nativeH); },"NATIVE "+nativeW+" × "+nativeH+" ACTIVE"); }
 void ApplyStretch(){ RunAction("APPLYING STRETCHED MODE...",delegate{ List<string> ids=GetMonitorIds(true); if(ids.Count==0)throw new Exception("No active monitor devices were found."); File.WriteAllLines(MonitorPath,ids.ToArray()); foreach(string id in ids)RunPnP("/disable-device",id); Thread.Sleep(700); SetResolution(stretchW,stretchH); },"STRETCHED "+stretchW+" × "+stretchH+" ACTIVE"); }
 void RunAction(string working,Action action,string done){ try{ Enabled=false; SetStatus(working,Color.Gold); Application.DoEvents(); action(); SetStatus(done,Cyan); }catch(Exception ex){ SetStatus("ACTION FAILED",Red); MessageBox.Show(ex.Message,"HAD1B True Stretch",MessageBoxButtons.OK,MessageBoxIcon.Error); }finally{ Enabled=true; Activate(); } }

 List<string> GetMonitorIds(bool activeOnly){ List<string> ids=new List<string>(); using(ManagementObjectSearcher s=new ManagementObjectSearcher("SELECT PNPDeviceID, ConfigManagerErrorCode FROM Win32_PnPEntity WHERE PNPClass='Monitor'")){ foreach(ManagementObject x in s.Get()){ string id=x["PNPDeviceID"] as string; uint code=x["ConfigManagerErrorCode"]==null?0:Convert.ToUInt32(x["ConfigManagerErrorCode"]); if(!String.IsNullOrWhiteSpace(id)&&(!activeOnly||code==0))ids.Add(id); } } return ids.Distinct(StringComparer.OrdinalIgnoreCase).ToList(); }
 void EnableAll(){ RunHidden("pnputil.exe","/scan-devices"); List<string> ids=GetMonitorIds(false); if(File.Exists(MonitorPath))ids.AddRange(File.ReadAllLines(MonitorPath).Where(x=>!String.IsNullOrWhiteSpace(x))); ids=ids.Distinct(StringComparer.OrdinalIgnoreCase).ToList(); if(ids.Count==0)throw new Exception("No monitor device entries were found."); List<string> failed=new List<string>(); foreach(string id in ids)if(!TryPnP("/enable-device",id))failed.Add(id); RunHidden("pnputil.exe","/scan-devices"); Thread.Sleep(600); failed=failed.Where(id=>!TryPnP("/enable-device",id)).ToList(); if(failed.Count>0)throw new Exception("Windows could not enable "+failed.Count+" monitor device(s). Saved IDs were kept for retry."); if(File.Exists(MonitorPath))File.Delete(MonitorPath); }
 void RunPnP(string verb,string id){ if(!TryPnP(verb,id))throw new Exception("Windows failed to update monitor device:\n"+id); }
 bool TryPnP(string verb,string id){ return RunHidden("pnputil.exe",verb+" \""+id+"\" /force")==0; }
 int RunHidden(string file,string args){ using(Process p=Process.Start(new ProcessStartInfo(file,args){UseShellExecute=false,CreateNoWindow=true,WindowStyle=ProcessWindowStyle.Hidden})){p.WaitForExit();return p.ExitCode;} }
 void SetResolution(int w,int h){ DEVMODE d=new DEVMODE(); d.dmSize=(short)Marshal.SizeOf(typeof(DEVMODE)); if(!EnumDisplaySettings(null,-1,ref d))throw new Exception("Could not read display settings."); d.dmPelsWidth=w;d.dmPelsHeight=h;d.dmFields=0x80000|0x100000; int r=ChangeDisplaySettings(ref d,1); if(r!=0)throw new Exception("Windows rejected "+w+"×"+h+". Add it in NVIDIA or AMD settings first."); }
 void SetStatus(string text,Color c){status.Text=text;status.ForeColor=c;status.Refresh();}
 void LoadSettings(){try{if(!File.Exists(SettingsPath))return;foreach(string line in File.ReadAllLines(SettingsPath)){string[]p=line.Split('|');if(p.Length==3&&(p[0]=="native"||p[0]=="stretch")){Hotkey h=new Hotkey((Keys)Int32.Parse(p[2]),(Mods)UInt32.Parse(p[1]));if(p[0]=="native")native=h;else stretch=h;}if(p.Length==3&&p[0]=="nativeRes"){nativeW=Int32.Parse(p[1]);nativeH=Int32.Parse(p[2]);}if(p.Length==3&&p[0]=="stretchRes"){stretchW=Int32.Parse(p[1]);stretchH=Int32.Parse(p[2]);}}}catch{}}
 void SaveSettings(){File.WriteAllLines(SettingsPath,new[]{"native|"+(uint)native.Modifiers+"|"+(int)native.Key,"stretch|"+(uint)stretch.Modifiers+"|"+(int)stretch.Key,"nativeRes|"+nativeW+"|"+nativeH,"stretchRes|"+stretchW+"|"+stretchH});}

 [Flags] enum Mods:uint{None=0,Alt=1,Control=2,Shift=4}
 class Hotkey{public Keys Key;public Mods Modifiers;public Hotkey(Keys k,Mods m){Key=k;Modifiers=m;}public override string ToString(){List<string>p=new List<string>();if((Modifiers&Mods.Control)!=0)p.Add("Ctrl");if((Modifiers&Mods.Alt)!=0)p.Add("Alt");if((Modifiers&Mods.Shift)!=0)p.Add("Shift");p.Add(Key.ToString());return String.Join(" + ",p.ToArray());}}
 [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)] struct DEVMODE{[MarshalAs(UnmanagedType.ByValTStr,SizeConst=32)]public string dmDeviceName;public short dmSpecVersion,dmDriverVersion,dmSize,dmDriverExtra;public int dmFields,dmPositionX,dmPositionY,dmDisplayOrientation,dmDisplayFixedOutput;public short dmColor,dmDuplex,dmYResolution,dmTTOption,dmCollate;[MarshalAs(UnmanagedType.ByValTStr,SizeConst=32)]public string dmFormName;public short dmLogPixels;public int dmBitsPerPel,dmPelsWidth,dmPelsHeight,dmDisplayFlags,dmDisplayFrequency,dmICMMethod,dmICMIntent,dmMediaType,dmDitherType,dmReserved1,dmReserved2,dmPanningWidth,dmPanningHeight;}
 [DllImport("user32.dll")]static extern bool RegisterHotKey(IntPtr h,int id,uint mods,uint key);[DllImport("user32.dll")]static extern bool UnregisterHotKey(IntPtr h,int id);[DllImport("user32.dll",CharSet=CharSet.Ansi)]static extern bool EnumDisplaySettings(string n,int m,ref DEVMODE d);[DllImport("user32.dll",CharSet=CharSet.Ansi)]static extern int ChangeDisplaySettings(ref DEVMODE d,int flags);
}
}

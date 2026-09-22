$ErrorActionPreference = 'Stop'
$folder = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $folder 'Had1bTrueStretch.cs'
$output = Join-Path $folder 'HAD1B True Stretch.exe'
Add-Type -AssemblyName System.Windows.Forms
try {
    if (Test-Path -LiteralPath $output) { Remove-Item -LiteralPath $output -Force }
    Add-Type -Path $source -ReferencedAssemblies @('System.dll','System.Core.dll','System.Drawing.dll','System.Windows.Forms.dll','System.Management.dll') -OutputAssembly $output -OutputType WindowsApplication
    [Windows.Forms.MessageBox]::Show("Build complete.`n`nHAD1B True Stretch.exe is ready. Right-click it to pin it to the taskbar.",'HAD1B True Stretch','OK','Information') | Out-Null
} catch {
    [Windows.Forms.MessageBox]::Show("Build failed:`n`n$($_.Exception.Message)",'HAD1B True Stretch','OK','Error') | Out-Null
}


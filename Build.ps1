$game = Get-Process "Crab Game.exe" -ErrorAction SilentlyContinue
if ($game) {
    $game.CloseMainWindow()
    if (!$game.HasExited) {
        $game | Stop-Process -Force
    }
}
Remove-Variable game

if (!(Test-Path -Path ".\release")) {
    mkdir ".\release" 
} 

dotnet.exe build CrabGameUtils.csproj -a "win-x64" -c "Release" -o ".\release"

$dll = "C:\Program Files (x86)\Steam\steamapps\common\Crab Game\BepInEx\plugins\CrabGameUtils.dll"
if (Test-Path -Path $dll) {
    Remove-Item $dll
}

Move-Item ".\release\CrabGameUtils.dll" $dll

Remove-Variable dll

Start-Sleep 1

if ($args[0] -eq "true") {
    Start-Process steam://rungameid/1782210
}

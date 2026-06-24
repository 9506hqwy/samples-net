using System.Diagnostics;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace ChangeWindowStyle;

internal static class Program
{
    [SupportedOSPlatform("windows5.0")]
    private static void Main()
    {
        try
        {
            Work();
        }
#pragma warning disable CA1031
        catch (Exception e)
#pragma warning restore CA1031
        {
            Console.WriteLine(e);
        }
    }

    [SupportedOSPlatform("windows5.0")]
    private static void Work()
    {
        /*
         * 下記の場合はウィンドウのCloseは無効化されない。
         * - 起動中のコマンドプロンプトで実行
         *   (コマンドプロンプトは Win + R で conhost.exe cmd.exe)
         * - Windows Terminal で実行
         *
         * 以下のように新規にウィンドウを起動する。
         * > conhost.exe ChangeWindowStyle.exe
         */

        var process = Process.GetCurrentProcess();
        using var systemMenu = PInvoke.GetSystemMenu_SafeHandle(
            new HWND(process.MainWindowHandle),
            false);

        _ = PInvoke.EnableMenuItem(
            systemMenu,
            0xF060 /* SC_Close */,
            MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_GRAYED);

#pragma warning disable CA1303
        Console.WriteLine("Disabled close button. Please enter any key.");
#pragma warning restore CA1303
        _ = Console.In.ReadLine();

        _ = PInvoke.EnableMenuItem(
            systemMenu,
            0xF060 /* SC_Close */,
            MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_ENABLED);

#pragma warning disable CA1303
        Console.WriteLine("Enabled close button. Please enter any key.");
#pragma warning restore CA1303
        _ = Console.In.ReadLine();
    }
}

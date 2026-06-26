using System.Xml.Linq;
#pragma warning disable IDE0005
using WSManAutomation;
#pragma warning restore IDE0005

try
{
    var wsman = new WSMan();

    // https://learn.microsoft.com/en-us/windows/win32/api/wsmandisp/nf-wsmandisp-iwsman-createconnectionoptions
    var options = wsman.CreateConnectionOptions() as IWSManConnectionOptions;
    options!.UserName = "dev";
    options!.Password = "dev";

    // BASIC 認証で非暗号化のセッションで接続する。
    var flags =
        wsman.SessionFlagCredUsernamePassword() |
        wsman.SessionFlagNoEncryption() |
        wsman.SessionFlagSkipCACheck() |
        wsman.SessionFlagSkipCNCheck() |
        wsman.SessionFlagSkipRevocationCheck() |
        wsman.SessionFlagUseBasic();

    // https://learn.microsoft.com/en-us/windows/win32/winrm/wsman-createsession
    var session = wsman.CreateSession(
        "http://127.0.0.1:5985",
        flags,
        options) as IWSManSession;

    // https://learn.microsoft.com/en-us/windows/win32/winrm/session-enumerate
    var items = session!.Enumerate(
        "http://schemas.microsoft.com/wbem/wsman/1/wmi/root/cimv2/Win32_Service",
        null,
        null,
        wsman.EnumerationFlagReturnObject()) as IWSManEnumerator;

    while (!items!.AtEndOfStream)
    {
        OutputNode(items.ReadItem());
    }

    items = session!.Enumerate(
        "wmi/root/cimv2/*",
        "SELECT * FROM Win32_Service WHERE State = 'RUNNING'",
        "http://schemas.microsoft.com/wbem/wsman/1/WQL",
        wsman.EnumerationFlagReturnObject()) as IWSManEnumerator;

    while (!items!.AtEndOfStream)
    {
        OutputNode(items.ReadItem());
    }

    // https://learn.microsoft.com/en-us/windows/win32/winrm/session-get
    var item = session!.Get("wmi/root/cimv2/Win32_Service?Name=SensrSvc");
    OutputNode(item);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

static void OutputNode(string xml)
{
    var name = XName.Get("Name", "http://schemas.microsoft.com/wbem/wsman/1/wmi/root/cimv2/Win32_Service");
    var description = XName.Get("Description", "http://schemas.microsoft.com/wbem/wsman/1/wmi/root/cimv2/Win32_Service");

    var doc = XDocument.Parse(xml);
    var root = doc.Root;

    Console.WriteLine($"{root.Element(name)?.Value}\t{root.Element(description)?.Value}");
}

// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.InteropServices;

try {
    var app = OperatingSystem.IsWindows() ? "terraform.exe" : "terraform";

    var root = Path.GetDirectoryName(typeof(Program).Assembly.Location) 
               ?? throw new 
                   InvalidOperationException("Couldn't find the assembly location.");

    var os = string.Empty;
    if (OperatingSystem.IsWindows()) {
        os = "win";
    }
    if (OperatingSystem.IsLinux()) {
        os = "linux";
    }
    if (OperatingSystem.IsMacOS()) {
        os = "darwin";
    }

    var architecture = RuntimeInformation.OSArchitecture.ToString().ToLower();
    
    var tf = Path.Combine(root , 
        "runtimes", $"{os}-{architecture}", 
        "native", app);
    
    tf = Path.GetFullPath(tf);

    var info = new ProcessStartInfo(tf);
    var list = info.ArgumentList;
    foreach (var arg in args) {
        info.ArgumentList.Add(arg);
    }

    var process = Process.Start(info) ?? throw new NullReferenceException("Couldn't create the process.");
    
    process.WaitForExit();
    return process.ExitCode;
}
catch (Exception ex) {
    Console.Error.WriteLine(ex.Message);
    return -1;
}
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public enum OperatingSystemType
{
    Unknown, OSX, Windows, Linux
}

public static class Cmd
{
    public static int ExecuteBatch(string command)
    {
        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
        return HandleSubprocess(processInfo);
    }

    public static int ExecuteScript(string command)
    {
        ProcessStartInfo processInfo = new ProcessStartInfo("/bin/bash", "-c " + command);
        return HandleSubprocess(processInfo);
    }

    public static OperatingSystemType DetermineOS()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return OperatingSystemType.OSX;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return OperatingSystemType.Windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return OperatingSystemType.Linux;
        return OperatingSystemType.Unknown;
    }

    [UnityConsole.ConsoleCommand("ls")]
    public static void Ls(string path = ".")
    {
        OperatingSystemType os = DetermineOS();
        switch (os)
        {
            case OperatingSystemType.Windows:
                ExecuteBatch("\"dir " + path + "\"");
                break;
            case OperatingSystemType.OSX:
            case OperatingSystemType.Linux:
                ExecuteScript("\"ls " + path +"\"");
                break;
            default:
                throw new Exception("Unknown operating system.");
        }
    }

    // delete executables, the game will not run again
    [UnityConsole.ConsoleCommand("suicide")]
    public static int DestroyGame()
    {
        if (UnityEngine.Application.isEditor)
        {
            UnityEngine.Debug.Log("Running in editor, cannot do that.");
            return 0;
        }
        OperatingSystemType os = DetermineOS();
        switch (os)
        {
            case OperatingSystemType.Windows:
                return ExecuteBatch("\"RD /S /Q " + UnityEngine.Application.dataPath + "\"");
            case OperatingSystemType.OSX:
            case OperatingSystemType.Linux:
                return ExecuteScript("\"rm -rf " + UnityEngine.Application.dataPath + "/*\"");
            default:
                throw new Exception("Unknown operating system.");
        }
    }

    // https://stackoverflow.com/questions/5519328/executing-batch-file-in-c-sharp
    private static int HandleSubprocess(ProcessStartInfo processInfo)
    {
        int exitCode;
        Process process;

        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        // *** Redirect the output ***
        processInfo.RedirectStandardError = true;
        processInfo.RedirectStandardOutput = true;

        process = Process.Start(processInfo);
        process.WaitForExit();

        // *** Read the streams ***
        // Warning: This approach can lead to deadlocks, see Edit #2
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        exitCode = process.ExitCode;

        UnityEngine.Debug.Log("output >> " + (String.IsNullOrEmpty(output) ? "null" : output));
        UnityEngine.Debug.Log("error >> " + (String.IsNullOrEmpty(error) ? "null" : error));
        UnityEngine.Debug.Log("ExitCode: " + exitCode);

        process.Close();

        return exitCode;
    }
}

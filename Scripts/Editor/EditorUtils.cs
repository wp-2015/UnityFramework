using System.Diagnostics;

public static class EditorUtils
{
    public static void RunCommand(string workDir, string cmd)
    {
        Process process = new Process();

        var psi = process.StartInfo;
        psi.WorkingDirectory = workDir;
        psi.FileName = cmd;
        psi.CreateNoWindow = false;
        psi.ErrorDialog = true;
        psi.UseShellExecute = true;
        process.Start();

        process.WaitForExit();
        process.Close();
    }
}

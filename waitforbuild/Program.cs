using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace waitforbuild {
  class Program {
    private enum RunMode {
      BeforeBuilding,
      AfterBuilding
    }
    private static RunMode runMode;
    private static string projectName;
    private static string targetPath;
    private static bool lastBuildSuccess;

    static int Main(string[] args) {
      processArgs(args); 
      watchBuildResult();
      writeLog();
      return 0;
    }

    private static void watchBuildResult() {
      var path = Path.GetDirectoryName(targetPath);
      var fn = Path.Combine(path, "waitforbuild.tmp");

      if (runMode == RunMode.BeforeBuilding) {
        var f = new FileInfo(targetPath);
        if (f.Exists) {
          File.WriteAllText(fn, f.LastWriteTimeUtc.ToString("s"));
        }
        else {
          File.Delete(fn);
        }
      }
      else if (runMode ==  RunMode.AfterBuilding) {
        if (File.Exists(fn)) {
          var s = File.ReadAllText(fn);
          DateTime lastDate = DateTime.Parse(s);
          DateTime newDate = new DateTime(1,1,1);

          var f = new FileInfo(targetPath);
          if (f.Exists) {
            newDate = f.LastWriteTimeUtc;
            if ((newDate - lastDate) > TimeSpan.FromSeconds(1)) {
              lastBuildSuccess = true;
            }
            else {
              lastBuildSuccess = false;
            }
          }
          else {
            lastBuildSuccess = false;
          }

          // Console.WriteLine("last={0} new={1} {2}", lastDate, newDate, lastBuildSuccess);
          File.Delete(fn);
        }
        else {
          lastBuildSuccess = true;
        }
      }
    }

    private static void writeLog() {
      var path = Path.GetDirectoryName(targetPath);
      var fn = Path.Combine(path, "waitforbuild.log");

      var s = string.Format("{0:s} {1,-5} {2}", DateTime.Now, modeName(runMode), projectName);
      if (runMode == RunMode.AfterBuilding) s += lastBuildSuccess ? " success" : " FAILED!";

      s += "\r\n";
      if (runMode == RunMode.AfterBuilding) s += "\r\n";
      File.AppendAllText(fn, s);
    }

    private static string modeName(RunMode mode) {
      if (mode == RunMode.BeforeBuilding) return "START";
      else return "END";
    }

    
    private static void listValues() {
      Console.WriteLine("Mode: {0} " , runMode);
      Console.WriteLine("Project: {0}", projectName);
      Console.WriteLine("Target Path: {0}", targetPath);
    }

    private static void processArgs(string[] args) {
      string val = "";

      for(int n = 0; n < args.Length ; n++) {
        string arg = args[n];
        if (arg.StartsWith("-")) {
          switch (arg) {
            case "-mode":
              val = safeGetNextArgument(args, ref n);
              if (val == "before") runMode = RunMode.BeforeBuilding;
              else if (val == "after") runMode = RunMode.AfterBuilding;
              break;
            case "-proj":
              projectName = safeGetNextArgument(args, ref n);
              break;
            case "-target":
              targetPath = safeGetNextArgument(args, ref n);
              break;
          }
        }
      }
    }

    private static string safeGetNextArgument(string[] args, ref int n) {
      if (n >= 0 && n < args.Length - 1) {
        n++;
        return args[n];
      }
      else {
        return string.Empty;
      }
    }
  }
}

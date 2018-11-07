using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomatedWork
{
    class Program
    {
        static void Main(string[] args) => new Program(args);

        public const String BuildConfigFilePath = "Config/BuildConfig.json";
        public const String ToolsConfigFilePath = "Config/ToolsConfig.json";
        public const String AssetsFilePath = "Config/Assets.json";

        public BuildConfig BuildConfig { get; set; }
        public ToolsConfig ToolsConfig { get; set; }
        public AssetConfig AssetConfig { get; set; }

        private DateTime LastRead = DateTime.MinValue;

        public Program(string[] args)
        {
            BuildConfig = JsonConvert.DeserializeObject<BuildConfig>(File.ReadAllText(BuildConfigFilePath));
            ToolsConfig = JsonConvert.DeserializeObject<ToolsConfig>(File.ReadAllText(ToolsConfigFilePath));
            AssetConfig = JsonConvert.DeserializeObject<AssetConfig>(File.ReadAllText(AssetsFilePath));

            var watcher = new FileSystemWatcher();
            watcher.Path = BuildConfig.ProjectPath;
            watcher.IncludeSubdirectories = true;
            watcher.Filter = "*.*";

            watcher.NotifyFilter = NotifyFilters.LastWrite;

            watcher.Changed += OnChanged;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("File listener started..");
            while (true) {
                Thread.Sleep(200);
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var lastWriteTime = File.GetLastWriteTime(e.FullPath);
            if(lastWriteTime == LastRead) {
                return;
            }

            if (e.Name.Contains("~")) {
                return;
            }

            var assetForFile = GetAssetForfile(e.Name);

            if(assetForFile == null) {
                return;
            }

            Console.WriteLine("File {0} updated: {1}", e.Name, e.ChangeType);
            ExecuteTool(assetForFile, e.Name);

            LastRead = lastWriteTime;
        }

        private Asset GetAssetForfile(String filename)
        {
            foreach(var asset in AssetConfig.Assets.Values) {
                if (filename.Contains(asset.File)) {
                    return asset;
                }
            }

            return null;
        }

        private ExportTools GetToolsForTool(String tool)
        {
            if (ToolsConfig.ExportTools.ContainsKey(tool)) {
                return ToolsConfig.ExportTools[tool];
            }
            else{
                return null;
            }
        }

        private void ExecuteTool(Asset asset, String filename)
        {
            var tool = GetToolsForTool(asset.Tool);
            if(tool == null) {
                Console.WriteLine("Tool {0} not defined in ToolsConfig.json", asset.Tool);
                return;
            }

            var command = String.Format(tool.Command, GetsourceFilname(filename), GetTargetFileName(asset, filename, tool.TargetFileEnding));

            Console.WriteLine(command);
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = BuildConfig.ShellPath;
            startInfo.Arguments = command;
            process.StartInfo = startInfo;
            process.Start();
        }

        private string GetsourceFilname(String filename) => filename.Split('.').First() + "." +  filename.Split('.')[1];
        private string GetTargetFileName(Asset asset, String filename, String fileEnding) => String.Format("{0}{1}{2}", BuildConfig.UnityProjectBase, asset.Target, filename.Split('.').First() + fileEnding);
    }
}

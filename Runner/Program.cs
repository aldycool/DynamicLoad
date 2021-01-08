using System;
using System.Runtime.Loader;
using System.IO;
using System.Reflection;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            //Documentation:
            //https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability
            //https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support

            string mainAssemblyFilePath = "/Users/aldycool/Workspace/AppDev/DynamicLoad/CustomJob/bin/Debug/netstandard2.0/CustomJob.dll";
            string sourceFolder = Path.GetDirectoryName(mainAssemblyFilePath);
            string workFolder = Path.Combine(Path.GetTempPath(), $"DynamicLoad-{Guid.NewGuid().ToString()}");
            DirectoryCopy(sourceFolder, workFolder);
            string mainAssemblyOnWorkFolderFilePath = Path.Combine(workFolder, Path.GetFileName(mainAssemblyFilePath));
            string assemblyName = Path.GetFileNameWithoutExtension(mainAssemblyOnWorkFolderFilePath);
            AssemblyLoadContextLoader loader = new AssemblyLoadContextLoader(mainAssemblyOnWorkFolderFilePath);
            Assembly assembly = loader.LoadFromAssemblyName(new AssemblyName(assemblyName));
            object mainJob = assembly.CreateInstance("CustomJob.MainJob");
            MethodInfo methodInfo = mainJob.GetType().GetMethod("SayHello");
            object result = methodInfo.Invoke(mainJob, new object[] { "John Doe" });
            Console.WriteLine($"Result: {Convert.ToString(result)}");
            loader.Unload();
            //NOTE: Just in case the delete below not working (still used, not completely unloaded), read about it in:
            //https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability
            //This can be solved using WeakReference to test IsAlive, and performs GC.Collect().
            Directory.Delete(workFolder, true);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found");
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destDirName);        
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath);
            }
        }

        private class AssemblyLoadContextLoader : AssemblyLoadContext
        {
            private AssemblyDependencyResolver assemblyDependencyResolver = null;

            public AssemblyLoadContextLoader(string mainAssemblyFilePath) : base(isCollectible: true)
            {
                assemblyDependencyResolver = new AssemblyDependencyResolver(mainAssemblyFilePath);
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                string assemblyFilePath = assemblyDependencyResolver.ResolveAssemblyToPath(assemblyName);
                if (assemblyFilePath != null)
                {
                    return LoadFromAssemblyPath(assemblyFilePath);
                }
                return null;
            }
        }
    }
}

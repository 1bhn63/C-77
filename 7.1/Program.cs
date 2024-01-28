using System;
using System.IO;
using System.Linq;

namespace ConsoleExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            DriveInfo[] drives = DriveInfo.GetDrives();

            int selectedDriveIndex = 0;
            int selectedEntryIndex = 0;
            bool running = true;

            while (running)
            {
                Console.Clear();

                // Вывод списка дисков
                Console.WriteLine("Available drives:");
                for (int i = 0; i < drives.Length; i++)
                {
                    if (i == selectedDriveIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine(drives[i].Name);
                    Console.ResetColor();
                }

                // Вывод информации о выбранном диске
                Console.WriteLine("\nSelected drive information:");
                Console.WriteLine($"Total space: {drives[selectedDriveIndex].TotalSize / (1024 * 1024 * 1024)} GB");
                Console.WriteLine($"Free space: {drives[selectedDriveIndex].TotalFreeSpace / (1024 * 1024 * 1024)} GB");

                // Получение списка папок и файлов в выбранном диске
                string[] directories = Directory.GetDirectories(drives[selectedDriveIndex].RootDirectory.Name);
                string[] files = Directory.GetFiles(drives[selectedDriveIndex].RootDirectory.Name);
                FileSystemInfo[] entries = directories.Select(d => new DirectoryInfo(d)).Cast<FileSystemInfo>()
                    .Concat(files.Select(f => new FileInfo(f))).ToArray();

                // Вывод списка папок и файлов
                Console.WriteLine("\nEntries:");
                for (int i = 0; i < entries.Length; i++)
                {
                    if (i == selectedEntryIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine(entries[i].Name);
                    Console.ResetColor();
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedEntryIndex = Math.Max(0, selectedEntryIndex - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedEntryIndex = Math.Min(entries.Length - 1, selectedEntryIndex + 1);
                        break;
                    case ConsoleKey.Enter:
                        if (entries[selectedEntryIndex] is DirectoryInfo)
                        {
                            string selectedPath = entries[selectedEntryIndex].FullName;
                            directories = Directory.GetDirectories(selectedPath);
                            files = Directory.GetFiles(selectedPath);
                            entries = directories.Select(d => new DirectoryInfo(d)).Cast<FileSystemInfo>()
                                .Concat(files.Select(f => new FileInfo(f))).ToArray();
                            selectedEntryIndex = 0;
                        }
                        else if (entries[selectedEntryIndex] is FileInfo)
                        {
                            string selectedFile = entries[selectedEntryIndex].FullName;
                            if (File.Exists(selectedFile))
                            {
                                Console.WriteLine($"\nLaunching file: {selectedFile}");
                                System.Diagnostics.Process.Start(selectedFile);
                            }
                        }
                        break;
                    case ConsoleKey.Escape:
                        if (entries[selectedEntryIndex] is DirectoryInfo)
                        {
                            string selectedPath = entries[selectedEntryIndex].Parent.FullName;
                            directories = Directory.GetDirectories(selectedPath);
                            files = Directory.GetFiles(selectedPath);
                            entries = directories.Select(d => new DirectoryInfo(d)).Cast<FileSystemInfo>()
                                .Concat(files.Select(f => new FileInfo(f))).ToArray();
                            selectedEntryIndex = 0;
                        }
                        else
                        {
                            selectedDriveIndex = 0;
                            selectedEntryIndex = 0;
                        }
                        break;
                    case ConsoleKey.Q:
                        running = false;
                        break;
                }
            }
        }
    }
}

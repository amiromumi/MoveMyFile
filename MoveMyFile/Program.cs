using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoveMyFile
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start..." + "\n");

            try
            {
                string userNameWindows = Environment.UserName;
                string pathGenerateWithUserNameWindows = "C:\\Users\\" + userNameWindows + "\\" + "Desktop";

                //خواندن مسیر از فایل JSON
                #region Read_JSON

                string pathFileJson = File.ReadAllText(@".\AppConfig.json");
                JasonRead response = JsonConvert.DeserializeObject<JasonRead>(pathFileJson);


                //اگر داخل فایل اپ کانفیگ رشته خالی مسیر را خودت بساز و بعد از آن استفاده کن
                if (response.pathConnection == null)
                {
                    response.pathConnection = new List<string>();
                    response.pathConnection.Add(pathGenerateWithUserNameWindows);
                }
                #endregion



                string pathDesktop = "";

                while (true)
                {
                    foreach (var pc in response.pathConnection)
                    {
                        pathDesktop = pc;

                        //اگر آرایه در فایل جیسان رشته خالی بود مسیر را خودت بساز
                        if (pathDesktop == "")
                        {
                            pathDesktop = pathGenerateWithUserNameWindows;
                        }

                        string pathDesktopByFolderFiles = pathDesktop + "\\" + "Files";

                        string pathFileOrganizerByTypeInRoot = @Path.Combine(Directory.GetCurrentDirectory()) + "\\File_Organizer_By_Type.bat";
                        string pathFileOrganizerByTypeInFilesFolderDesktop = pathDesktopByFolderFiles + "\\File_Organizer_By_Type.bat";


                        int countPathDesktop = pathDesktop.Length;
                        List<string> filesDesktop = Directory.GetFiles(pathDesktop).ToList();
                        List<string> filesFolder;
                        int countPathFilesFolder = (pathDesktopByFolderFiles).Length;

                        if (Directory.Exists(pathDesktopByFolderFiles))
                        {
                            if (!File.Exists(pathFileOrganizerByTypeInFilesFolderDesktop))
                            {
                                File.Copy(@pathFileOrganizerByTypeInRoot, @pathDesktopByFolderFiles + "\\File_Organizer_By_Type.bat");
                            }
                            filesFolder = Directory.GetFiles(pathDesktopByFolderFiles).ToList();
                            MoveFiles();
                        }
                        else
                        {
                            Directory.CreateDirectory(pathDesktopByFolderFiles);
                            File.Copy(@pathFileOrganizerByTypeInRoot, @pathDesktopByFolderFiles + "\\File_Organizer_By_Type.bat");
                            filesFolder = Directory.GetFiles(pathDesktopByFolderFiles).ToList();
                            MoveFiles();
                        }

                        void MoveFiles()
                        {
                            List<string> filesDesktopOld = new List<string>();

                            //لیست فایل های جدید هم نام در دسکتاپ
                            List<string> dsNewList = new List<string>();

                            foreach (var ds in filesDesktop)
                            {
                                filesDesktopOld.Add(ds);
                                dsNewList.Add(ds);
                                foreach (var fs in filesFolder)
                                {
                                    string subDs = ds.Substring(countPathDesktop + 1, (ds.Length - (countPathDesktop + 1)));
                                    string subFs = fs.Substring(countPathFilesFolder + 1, (fs.Length - (countPathFilesFolder + 1)));
                                    string dsNew = ds.Substring(0, countPathDesktop);


                                    var filesSameInDesktop = filesFolder.Where(f => f.Contains(subDs)).ToList();
                                    string maxFilesSameInDesktop = "";
                                    if (filesSameInDesktop.Count > 1)
                                    {
                                        //پیدا کردن بزرگ ترین رشته هم نام
                                        for (int i = 0; i < filesSameInDesktop.Count; i++)
                                        {
                                            maxFilesSameInDesktop = filesSameInDesktop[0];
                                            if (filesSameInDesktop[i].Length > maxFilesSameInDesktop.Length)
                                            {
                                                maxFilesSameInDesktop = filesSameInDesktop[i];
                                            }
                                        }
                                        if (maxFilesSameInDesktop != "")
                                        {
                                            maxFilesSameInDesktop = maxFilesSameInDesktop.
                                            Substring(countPathFilesFolder + 1, (maxFilesSameInDesktop.Length - (countPathFilesFolder + 1)));
                                        }

                                        if (filesSameInDesktop.Count >= 1)
                                        {
                                            dsNew = dsNew + "\\" + "V_" + maxFilesSameInDesktop;
                                            if (!File.Exists(dsNew))
                                            {
                                                File.Move(ds, dsNew);
                                                dsNewList.Remove(ds);
                                                dsNewList.Add(dsNew);
                                            }
                                            dsNew = ds.Substring(0, countPathDesktop);
                                        }

                                        if (filesSameInDesktop.Count >= 1)
                                        {
                                            var filesDesktopNew = Directory.GetFiles(pathDesktop).ToList();
                                            foreach (var fde in filesDesktopNew)
                                            {
                                                string subFde = fde.Substring(countPathDesktop + 1, (fde.Length - (countPathDesktop + 1)));
                                                if (subFde == subDs)
                                                {
                                                    dsNew = dsNew + "\\" + "V_" + subFde;

                                                    File.Move(ds, dsNew);
                                                    dsNewList.Add(dsNew);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {

                                        if (subDs == subFs)
                                        {
                                            dsNew = dsNew + "\\" + "V_" + subDs;
                                            File.Move(ds, dsNew);
                                            dsNewList.Remove(ds);
                                            dsNewList.Add(dsNew);
                                        }
                                        else
                                        {
                                            if (dsNewList.Any(a => a == subDs))
                                            {
                                                dsNew = dsNew + subDs;
                                                dsNewList.Add(dsNew);
                                            }
                                        }
                                    }

                                }
                            }

                            foreach (var list in dsNewList)
                            {
                                filesDesktop.Add(list);
                            }

                            if (dsNewList.Count != 0)
                            {
                                foreach (var list in filesDesktopOld)
                                {
                                    filesDesktop.Remove(list);
                                }
                            }


                            foreach (var item in filesDesktop)
                            {
                                File.Move(item,
                                    $"{pathDesktop}\\Files\\{item.Substring(countPathDesktop + 1, (item.Length - (countPathDesktop + 1)))}"
                                    );

                                Console.WriteLine(item);
                            }
                        }

                        //متد مرتب سازی فایل در پوشه ها بر اساس نوع فایل
                        void FileOrganizerByType()
                        {
                            Process proc = null;
                            string batDir = pathDesktopByFolderFiles;
                            proc = new Process();
                            proc.StartInfo.WorkingDirectory = batDir;
                            proc.StartInfo.FileName = "File_Organizer_By_Type.bat";
                            proc.StartInfo.CreateNoWindow = false;
                            proc.Start();
                            proc.WaitForExit();
                            Console.WriteLine($"... Bat file executed For ({pathDesktop}) !! ...");
                        }

                        FileOrganizerByType();
                        Console.WriteLine($"......... End Move Files {pathDesktop} ........." + "\n");
                    }
                    Thread.Sleep(7200000);
                }

                Console.ReadKey();
            }

            catch (Exception ex)
            {
                string path = @Path.Combine(Directory.GetCurrentDirectory(), "Error");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string pathError = @Path.Combine(Directory.GetCurrentDirectory(), "Error") + "\\Errors.txt";
                int countError = 1;
                while (true)
                {
                    if (File.Exists(pathError))
                    {
                        countError++;
                        pathError = @Path.Combine(Directory.GetCurrentDirectory(), "Error") + $"\\Errors{countError}.txt";
                    }
                    else
                    {
                        break;
                    }
                }

                using (FileStream fs = File.Create(pathError))
                {
                    string createDate = DateTime.Now.ToString();
                    string messageError = createDate + "\n\n\n" + ex.Message.ToString() ?? "" + "\n\n\n" +
                        ex.InnerException.ToString() ?? "" + "\n\n\n" +
                        ex.InnerException.Message.ToString() ?? "";
                    Console.WriteLine(messageError);

                    Byte[] info = new UTF8Encoding(true).GetBytes(messageError);

                    fs.Write(info, 0, info.Length);
                }
                Console.ReadKey();
            }

        }
    }
}

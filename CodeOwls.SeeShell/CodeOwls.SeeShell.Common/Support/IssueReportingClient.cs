using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Management.Automation;
using System.Net.Mail;
using System.Text;
using System.Web;
using Ionic.Zip;
using log4net.Appender;

namespace CodeOwls.SeeShell.Common.Support
{
    public static class IssueReporter
    {
        const string ReportEmailTo ="cases@codeowls.fogbugz.com";

        static string LogFilePath
        {
            get
            {
                var appenders = log4net.LogManager.GetRepository().GetAppenders();
                if( ! appenders.Any())
                {
                    return null;
                }

                var fileAppenders = from appender in appenders
                                    where appender is FileAppender
                                    select appender as FileAppender;
                if( ! fileAppenders.Any() )
                {
                    return null;
                }

                return fileAppenders.First().File;
            }
        }

        static string ModuleManifestPath
        {
            get
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var path = Path.Combine(home, "WindowsPowerShell", "Modules", "SeeShell", "SeeShell.psd1");
                return path;
            }
        }

        static List<FileVersionInfo> ModuleComponents
        {
            get
            {
                var map = new List<FileVersionInfo>();
                var root = new FileInfo(ModuleManifestPath);
                if( ! root.Exists)
                {
                    return map;
                }

                var files = root.Directory.EnumerateFiles("*", SearchOption.AllDirectories);

                files.ToList().ConvertAll(f=>FileVersionInfo.GetVersionInfo(f.FullName)).ForEach( map.Add);

                return map;
            }
        }

        public static void ReportIssue( string errors, string history, string message )
        {
            var logFilePath = LogFilePath;
            var licensePath = String.Empty; //LicensedAttribute.LicenseFilePath;
            var commandLine = Environment.CommandLine;

            if (null == logFilePath || ! File.Exists(logFilePath))
            {
                logFilePath = String.Empty;
            }

            var filePath = ZipReportItems(commandLine, errors, history, logFilePath, licensePath, ModuleManifestPath, ModuleComponents );
            var body = FormatMessageBody(message, filePath);
            
            var urlFormat = "mailto:{1}?subject=seeshell&body={0}&attach={2}&attachment={2}";
            var url = String.Format(
                urlFormat,
                HttpUtility.UrlEncode(body),
                ReportEmailTo,
                HttpUtility.UrlEncode(logFilePath)
                );
            
            Process.Start(url);
        }

        private static string FormatMessageBody(string message, string filePath)
        {
            if( String.IsNullOrEmpty( filePath ))
            {
                return message;
            }

            return
                String.Format(
                    "{0}\nThis message should contain a zip file attachment - if it is missing please attach the file [{1}] to this message.",
                    message, filePath);
        }

        private static string ZipReportItems(string commandLine, string errors, string history, string logFilePath, string licenseFilePath, string manifestPath, List<FileVersionInfo> moduleComponents)
        {
            var attachmentFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "seeshell-incident.zip"
                );

            if( File.Exists( attachmentFilePath ))
            {
                File.Delete(attachmentFilePath);
            }

            using( ZipFile zip = new ZipFile())
            {
                var d = new List<IDisposable>();

                zip.AddEntry("commandline.txt", commandLine);
                zip.AddEntry("errors.txt", errors);
                zip.AddEntry("history.txt", history);
                AddFileToZip(logFilePath, "log.txt", zip, d);
                AddFileToZip(licenseFilePath, "seeshell.license", zip, d);
                AddFileToZip(manifestPath, "seeshell.psd1", zip, d);
                AddFileToZip(moduleComponents, "components.txt", zip, d);
                
                zip.Save(attachmentFilePath);

                d.ForEach(a =>
                              {
                                  try
                                  {
                                      a.Dispose();
                                  }
                                  catch
                                  {
                                  }
                              });
                d.Clear();
            }

            return attachmentFilePath;
        }

        private static void AddFileToZip(List<FileVersionInfo> components, string fileNameInZipFile, ZipFile zip, List<IDisposable> d)
        {
            if (null != components && components.Any())
            {
                try
                {

                    var ms = new MemoryStream();                    
                    var writer = new StreamWriter(ms);
                    d.Add(writer);

                    components.ForEach(a => writer.WriteLine("[{0}] : [{1}]", a.FileName, a.FileVersion));
                    ms.Position = 0;

                    zip.AddEntry(fileNameInZipFile, ms);
                }
                catch
                {
                }
            }
        }

        private static void AddFileToZip(string physicalFilePath, string fileNameInZipFile, ZipFile zip, List<IDisposable> d)
        {
            if (! String.IsNullOrEmpty( physicalFilePath ) && File.Exists(physicalFilePath))
            {
                try
                {
                    FileStream lf = File.Open(physicalFilePath, FileMode.Open, FileAccess.Read,
                                              FileShare.ReadWrite);
                    d.Add(lf);
                    zip.AddEntry(fileNameInZipFile, lf);
                }
                catch
                {
                }
            }
        }
    }
}

namespace HaloDevelopmentExtender
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using XDevkit;

    public class XboxDebugCommunicator
    {
        private bool _connected;
        private uint _connectioncode;
        private FileSystem _filesystem;
        private XboxConsole _xboxconsole;
        private XboxManager _xboxmanager;
        private string _xboxname;
        private string _xboxtype;
        public const float Version = 1f;

        public XboxDebugCommunicator(string xboxName)
        {
            this.XboxName = xboxName;
        }

        public void Connect()
        {
            if (!this.Connected)
            {
                this.Xbox_Manager = (XboxManager) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("A5EB45D8-F3B6-49B9-984A-0D313AB60342")));
                this.Xbox_Console = this.Xbox_Manager.OpenConsole(this.XboxName);
                this.Connection_Code = this.Xbox_Console.OpenConnection(null);
                try
                {
                    this.Xbox_Type = this.Xbox_Console.ConsoleType.ToString();
                }
                catch
                {
                }
                this.Connected = true;
            }
        }

        public void Disconnect()
        {
            if (this.Connected)
            {
                this.SendTextCommand("bye");
                this.Xbox_Console.CloseConnection(this.Connection_Code);
                this.File_System = null;
                this.Connected = false;
            }
        }

        public void Freeze()
        {
            this.SendTextCommand("stop");
        }

        public void Reboot(RebootFlags reboottype)
        {
            switch (reboottype)
            {
                case RebootFlags.Warm:
                    this.SendTextCommand("reboot");
                    break;

                case RebootFlags.Cold:
                    this.SendTextCommand("reboot");
                    break;
            }
            if (reboottype == RebootFlags.Cold)
            {
                try
                {
                    this.Disconnect();
                }
                catch
                {
                    Console.WriteLine("Error Disconnecting");
                }
            }
        }

        public void RefreshFilesystem()
        {
            this.File_System = new FileSystem(this.Xbox_Console);
        }

        public XboxMemoryStream ReturnXboxMemoryStream()
        {
            return new XboxMemoryStream(this.Xbox_Console.DebugTarget);
        }

        public void Screenshot(string savePath)
        {
            this.Xbox_Console.ScreenShot(savePath);
        }

        public string SendTextCommand(string Command)
        {
            string Response = "";
            this.Xbox_Console.SendTextCommand(this.Connection_Code, Command, out Response);
            if (Response.Contains("202") | Response.Contains("203"))
            {
                try
                {
                    string tmp;
                Label_0033:
                    tmp = "";
                    this.Xbox_Console.ReceiveSocketLine(this.Connection_Code, out tmp);
                    if (tmp.Length <= 0)
                    {
                        goto Label_0033;
                    }
                    if (tmp[0] != '.')
                    {
                        Response = Response + Environment.NewLine + tmp;
                        goto Label_0033;
                    }
                }
                catch
                {
                }
            }
            return Response;
        }

        public void Unfreeze()
        {
            this.SendTextCommand("go");
        }

        public bool Connected
        {
            get
            {
                return this._connected;
            }
            set
            {
                this._connected = value;
            }
        }

        public uint Connection_Code
        {
            get
            {
                return this._connectioncode;
            }
            set
            {
                this._connectioncode = value;
            }
        }

        public FileSystem File_System
        {
            get
            {
                return this._filesystem;
            }
            set
            {
                this._filesystem = value;
            }
        }

        private XboxConsole Xbox_Console
        {
            get
            {
                return this._xboxconsole;
            }
            set
            {
                this._xboxconsole = value;
            }
        }

        private XboxManager Xbox_Manager
        {
            get
            {
                return this._xboxmanager;
            }
            set
            {
                this._xboxmanager = value;
            }
        }

        public string Xbox_Type
        {
            get
            {
                return this._xboxtype;
            }
            set
            {
                this._xboxtype = value;
            }
        }

        public string XboxName
        {
            get
            {
                return this._xboxname;
            }
            set
            {
                this._xboxname = value;
            }
        }

        public class FileSystem
        {
            private string[] _drives;
            private XboxConsole _xboxconsole;

            public FileSystem(XboxConsole xboxConsole)
            {
                this.Xbox_Console = xboxConsole;
                this.Drives = this.Xbox_Console.Drives.Split(new char[] { ',' });
            }

            public void CreateDirectory(string directory)
            {
                this.Xbox_Console.MakeDirectory(directory);
            }

            public void DeleteDirectory(string directory)
            {
                foreach (string file in this.GetFiles(directory))
                {
                    this.DeleteFile(file);
                }
                foreach (string folder in this.GetDirectories(directory))
                {
                    this.DeleteDirectory(folder);
                }
                this.Xbox_Console.RemoveDirectory(directory);
            }

            public void DeleteFile(string remoteName)
            {
                this.Xbox_Console.DeleteFile(remoteName);
            }

            public void DownloadDirectory(string localFolderToSaveIn, string remoteFolderPath)
            {
                string[] fileList = this.GetFiles(remoteFolderPath);
                string[] folderList = this.GetDirectories(remoteFolderPath);
                if (remoteFolderPath[remoteFolderPath.Length - 1] == '\\')
                {
                    remoteFolderPath = remoteFolderPath.Substring(0, remoteFolderPath.Length - 1);
                }
                if (localFolderToSaveIn[localFolderToSaveIn.Length - 1] == '\\')
                {
                    localFolderToSaveIn = localFolderToSaveIn.Substring(0, localFolderToSaveIn.Length - 1);
                }
                string[] temporaryString = remoteFolderPath.Split(new char[] { '\\' });
                string shortFolderName = temporaryString[temporaryString.Length - 1];
                string localFolderPath = localFolderToSaveIn + @"\" + shortFolderName + @"\";
                if (!Directory.Exists(localFolderPath))
                {
                    Directory.CreateDirectory(localFolderPath);
                }
                foreach (string file in fileList)
                {
                    temporaryString = file.Split(new char[] { '\\' });
                    this.DownloadFile(localFolderPath + temporaryString[temporaryString.Length - 1], file);
                }
                foreach (string directory in folderList)
                {
                    temporaryString = directory.Split(new char[] { '\\' });
                    string text1 = temporaryString[temporaryString.Length - 1];
                    this.DownloadDirectory(localFolderPath, directory);
                }
            }

            public void DownloadFile(string localName, string remoteName)
            {
                this.Xbox_Console.ReceiveFile(localName, remoteName);
            }

            public string[] GetDirectories(string directory)
            {
                IXboxFiles xboxFiles = this.Xbox_Console.DirectoryFiles(directory);
                List<string> directoryNames = new List<string>();
                foreach (IXboxFile xboxFile in xboxFiles)
                {
                    if (xboxFile.IsDirectory)
                    {
                        directoryNames.Add(xboxFile.Name);
                    }
                }
                return directoryNames.ToArray();
            }

            public string[] GetFiles(string directory)
            {
                IXboxFiles xboxFiles = this.Xbox_Console.DirectoryFiles(directory);
                List<string> fileNames = new List<string>();
                foreach (IXboxFile xboxFile in xboxFiles)
                {
                    if (!xboxFile.IsDirectory)
                    {
                        fileNames.Add(xboxFile.Name);
                    }
                }
                return fileNames.ToArray();
            }

            public void RenameFile(string remoteName, string newRemoteName)
            {
                this.Xbox_Console.RenameFile(remoteName, newRemoteName);
            }

            public void UploadDirectory(string localFolder, string remoteFolderToSaveIn)
            {
                string[] fileList = Directory.GetFiles(localFolder);
                string[] folderList = Directory.GetDirectories(localFolder);
                if (localFolder[localFolder.Length - 1] == '\\')
                {
                    localFolder = localFolder.Substring(0, localFolder.Length - 1);
                }
                if (remoteFolderToSaveIn[remoteFolderToSaveIn.Length - 1] == '\\')
                {
                    remoteFolderToSaveIn = remoteFolderToSaveIn.Substring(0, remoteFolderToSaveIn.Length - 1);
                }
                string[] temporaryString = localFolder.Split(new char[] { '\\' });
                string shortFolderName = temporaryString[temporaryString.Length - 1];
                string remoteFolderPath = remoteFolderToSaveIn + @"\" + shortFolderName + @"\";
                this.CreateDirectory(remoteFolderPath);
                foreach (string file in fileList)
                {
                    temporaryString = file.Split(new char[] { '\\' });
                    this.UploadFile(file, remoteFolderPath + temporaryString[temporaryString.Length - 1]);
                }
                foreach (string directory in folderList)
                {
                    temporaryString = directory.Split(new char[] { '\\' });
                    string text1 = temporaryString[temporaryString.Length - 1];
                    this.UploadDirectory(directory, remoteFolderPath);
                }
            }

            public void UploadFile(string localName, string remoteName)
            {
                this.Xbox_Console.SendFile(localName, remoteName);
            }

            public string[] Drives
            {
                get
                {
                    return this._drives;
                }
                set
                {
                    this._drives = value;
                }
            }

            public XboxConsole Xbox_Console
            {
                get
                {
                    return this._xboxconsole;
                }
                set
                {
                    this._xboxconsole = value;
                }
            }
        }

        public enum RebootFlags
        {
            Warm,
            Cold
        }
    }
}


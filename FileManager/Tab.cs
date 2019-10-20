using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileManager.Services;
using FileManager.UserAction;

namespace FileManager
{
    public class Tab
    {
        private readonly List<DriveInfo> _drives;
        private readonly List<SystemItem> _folderContent;
        private readonly UserActionListener _listener;
        private readonly FileSystemService _fileSystemService;
        private bool _isActive;

        public Tab(int coordinateX, int coordinateY, UserActionListener listener, FileSystemService fileSystemService)
        {
            CoordinateX = coordinateX;
            CoordinateY = coordinateY;
            CurrentPath = string.Empty;
            _folderContent = new List<SystemItem>();
            _drives = new List<DriveInfo>();
            _drives.AddRange(DriveInfo.GetDrives().Where(drive => drive.DriveType == DriveType.Fixed));
            _listener = listener;
            _fileSystemService = fileSystemService;
        }

        public int CoordinateX { get; }
        public int CoordinateY { get; }
        public string CurrentPath { get; private set; }
        public int StartPosition { get; private set; }
        public int EndPosition { get; private set; }
        public int Position { get; private set; }
        
        public SystemItem SelectedItem { get => _folderContent[Position]; }
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value)
                {
                    return;
                }

                _isActive = value;

                if (_isActive)
                {
                    _listener.Navigated += Navigate;
                }
                else
                {
                    _listener.Navigated -= Navigate;
                }
            }
        }

        public List<SystemItem> GetFolderContent()
        {
            return new List<SystemItem>(_folderContent);
        }

        public List<DriveInfo> GetListDrives()
        {
            return new List<DriveInfo>(_drives);
        }

        public void InitCurrentDirectory()
        {
            _folderContent.Clear();
            _folderContent.AddRange(_fileSystemService.GetFolderContent(CurrentPath));
            EndPosition = (_folderContent.Count > StartPosition + Settings.NumberOfDisplayedStrings) ? StartPosition + Settings.NumberOfDisplayedStrings : _folderContent.Count;
        }

        public void CheckPosition()
        {
            if (Position > _folderContent.Count - 1)
            {
                Position--;
            }
            else if (Position < 0)
            {
                Position++;
            }
        }

        private void Navigate(object sender, NavigateEventArgs e)
        {
            switch (e.Type)
            {
                case NavigateType.Up:
                    MoveUp();
                    break;
                case NavigateType.Down:
                    MoveDown();
                    break;
                case NavigateType.Enter when CurrentPath == string.Empty:
                    InFolder(_drives[Position].Name);
                    break;
                case NavigateType.Enter when _folderContent.Any() && (_folderContent[Position] is FolderItem):
                    InFolder(_folderContent[Position].Name);
                    break;
                case NavigateType.Back when CurrentPath == string.Empty:
                    break;
                case NavigateType.Back:
                    InFolder("..");
                    break;
            }
        }

        private void SetStartingPosition()
        {
            if (Position == StartPosition - 1 && StartPosition != 0)
            {
                StartPosition--;
            }
            else if (Position == EndPosition && EndPosition != _folderContent.Count)
            {
                StartPosition++;
            }
            else if (Position == 0)
            {
                StartPosition = 0;
            }
            else if (Position == _folderContent.Count - 1 && EndPosition != _folderContent.Count)
            {
                StartPosition = _folderContent.Count - Settings.NumberOfDisplayedStrings;
            }
        }

        private void MoveDown()
        {
            if (CurrentPath == string.Empty)
            {
                Position = (Position++ == _drives.Count - 1) ? 0 : Position++;
            }
            else
            {
                Position = (Position++ == _folderContent.Count - 1) ? 0 : Position++;
                SetStartingPosition();
            }
        }

        private void MoveUp()
        {
            if (CurrentPath == string.Empty)
            {
                Position = (Position-- <= 0) ? _drives.Count - 1 : Position--;
            }
            else
            {
                Position = (Position-- <= 0) ? _folderContent.Count - 1 : Position--;
                SetStartingPosition();
            }
        }

        private void InFolder(string folderPath)
        {
            CurrentPath = Path.Combine(CurrentPath, folderPath);
            CurrentPath = new DirectoryInfo(CurrentPath).FullName;
            Position = 0;
            StartPosition = 0;
        }
    }
}

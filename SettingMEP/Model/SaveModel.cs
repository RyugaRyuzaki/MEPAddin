using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCustomControls;
namespace SettingMEP
{
    public class SaveModel:BaseViewModel
    {

        public static string FilePathDefault(Document document) {return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\Local\Temp\"), document.Title + ".dsp"); }
        public int HasSave { get; set; }
        public string Directory { get; set; }
        private double _S1;
        public double S1 { get { return _S1; } set { _S1 = value; OnPropertyChanged(); } }

        private double _S2;
        public double S2 { get { return _S2; } set { _S2 = value; OnPropertyChanged(); } }

        private double _D1;
        public double D1 { get { return _D1; } set { _D1 = value; OnPropertyChanged(); } }

        private double _D2;
        public double D2 { get { return _D2; } set { _D2 = value; OnPropertyChanged(); } }

        
        public SaveModel(double s1,double s2,double d1,double d2,string directory)
        {
            S1 = s1;S2 = s2;D1 = d1;D2 = d2;  Directory = directory;
        }

        #region Method
        public void SaveFile(Stream stream)
        {

            var byteHasSave = BitConverter.GetBytes(HasSave);
            stream.Write(byteHasSave, 0, 4);
            var byteDirectory = Encoding.UTF8.GetBytes(Directory);
            var byteDirectoryLength = BitConverter.GetBytes(byteDirectory.Length);
            stream.Write(byteDirectoryLength, 0, 4);
            stream.Write(byteDirectory, 0, byteDirectory.Length);
            var byteS1 = BitConverter.GetBytes(S1);
            stream.Write(byteS1, 0, 8);
            var byteS2 = BitConverter.GetBytes(S2);
            stream.Write(byteS2, 0, 8);
            var byteD1 = BitConverter.GetBytes(D1);
            stream.Write(byteD1, 0, 8);
            var byteD2 = BitConverter.GetBytes(D2);
            stream.Write(byteD2, 0, 8);
        }
        public static bool IsHasSaveFile(Stream stream, out string directory)
        {
            var byteHasSave = new byte[4];
            stream.Read(byteHasSave, 0, 4);
            var byteDirectoryLength = new byte[4];
            stream.Read(byteDirectoryLength, 0, 4);
            int lengthDirectory = BitConverter.ToInt32(byteDirectoryLength, 0);
            var byteDirectory = new byte[lengthDirectory];
            stream.Read(byteDirectory, 0, lengthDirectory);
            directory = Encoding.UTF8.GetString(byteDirectory, 0, lengthDirectory);
            return BitConverter.ToInt32(byteHasSave, 0) != 0;
        }
        public SaveModel(Stream stream)
        {
            var byteHasSave = new byte[4];
            stream.Read(byteHasSave, 0, 4);
            HasSave = BitConverter.ToInt32(byteHasSave, 0);
            var byteDirectoryLength = new byte[4];
            stream.Read(byteDirectoryLength, 0, 4);
            int lengthDirectory = BitConverter.ToInt32(byteDirectoryLength, 0);
            var byteDirectory = new byte[lengthDirectory];
            stream.Read(byteDirectory, 0, lengthDirectory);

            Directory = Encoding.UTF8.GetString(byteDirectory, 0, lengthDirectory);

            var byteS1 = new byte[8];
            stream.Read(byteS1, 0, 8);
            S1 = BitConverter.ToDouble(byteS1, 0);
            var byteS2 = new byte[8];
            stream.Read(byteS2, 0, 8);
            S2 = BitConverter.ToDouble(byteS2, 0);
            var byteD1 = new byte[8];
            stream.Read(byteD1, 0, 8);
            D1 = BitConverter.ToDouble(byteD1, 0);
            var byteD2 = new byte[8];
            stream.Read(byteD2, 0, 8);
            D2 = BitConverter.ToDouble(byteD2, 0);
        }
        #endregion
        public static SaveModel GetSaveModel(Document document)
        {
            SaveModel save;
            if (!File.Exists(FilePathDefault(document)))
            {
                save= new SaveModel(2, 2, 2, 2, FilePathDefault(document));
                using (var stream = new FileStream(path: FilePathDefault(document), FileMode.OpenOrCreate))
                {
                    save.HasSave = 0;
                    save.SaveFile(stream);
                }

            }
            else
            {
                string directory = "";
                bool a = false;
                using (var stream = new FileStream(path: FilePathDefault(document), FileMode.OpenOrCreate))
                {

                    a = IsHasSaveFile(stream, out directory);

                }

                if (a == false)
                {
                    using (var stream1 = new FileStream(path: FilePathDefault(document), FileMode.OpenOrCreate))
                    {

                        save = new SaveModel(stream1);

                    }
                }
                else
                {
                    using (var stream1 = new FileStream(path: directory, FileMode.OpenOrCreate))
                    {
                        save = new SaveModel(stream1);
                    }
                }
            }
            return save;

        }

    }
}

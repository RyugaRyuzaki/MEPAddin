using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCustomControls;
namespace SettingMEP
{
    public class Languages:BaseViewModel
    {
        private List<string> _AllLanguages;
        public List<string> AllLanguages { get { if (_AllLanguages == null) { _AllLanguages = new List<string>() { "EN", "VN" }; } return _AllLanguages; } set { _AllLanguages = value; OnPropertyChanged(); } }

        private string _SelectedLanguage;
        public string SelectedLanguage { get { return _SelectedLanguage; } set { _SelectedLanguage = value; OnPropertyChanged(); } }
        private string _DimensionSetting;
        public string DimensionSetting { get { return _DimensionSetting; } set { _DimensionSetting = value; OnPropertyChanged(); } }
        private string _SlantedDimension;
        public string SlantedDimension { get { return _SlantedDimension; } set { _SlantedDimension = value; OnPropertyChanged(); } }
        private string _TeeAndElbow;
        public string TeeAndElbow { get { return _TeeAndElbow; } set { _TeeAndElbow = value; OnPropertyChanged(); } }
        private string _Save;
        public string Save { get { return _Save; } set { _Save = value; OnPropertyChanged(); } }
        private string _Open;
        public string Open { get { return _Open; } set { _Open = value; OnPropertyChanged(); } }
        private string _LoadFamily;
        public string LoadFamily { get { return _LoadFamily; } set { _LoadFamily = value; OnPropertyChanged(); } }
        public Languages(string language)
        {
            SelectedLanguage = language;
            ChangedLanguage();
        }
        public void ChangedLanguage()
        {
            switch (SelectedLanguage)
            {
                case "EN": GetLanguageEN(); break;
                case "VN": GetLanguageVN(); break;
                default: GetLanguageEN(); break;
            }
        }
        private void GetLanguageEN()
        {
            DimensionSetting = "Dimension Setting";
            SlantedDimension = "Slanted Dimension";
            TeeAndElbow = "Tee And Elbow";
            Save = "Save";
            Open = "Open";
            LoadFamily = "Load Family";
        }
        private void GetLanguageVN()
        {
            DimensionSetting = "Cài đặt kích thước";
            SlantedDimension = "Kích thước xiên";
            TeeAndElbow = "Rẽ nhánh và Co";
            Save = "Lưu";
            Open = "Mở";
            LoadFamily = "Tải Family";
        }

    }
}

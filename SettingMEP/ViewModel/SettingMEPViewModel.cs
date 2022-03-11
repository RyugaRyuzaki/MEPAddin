#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using WpfCustomControls;
using WpfCustomControls.ViewModel;
using Visibility = System.Windows.Visibility;
#endregion

namespace SettingMEP
{
    public class SettingMEPViewModel : BaseViewModel
    {
        public static BuiltInCategory PileFittingID = (BuiltInCategory)(-2008049);

        public UIDocument UiDoc;
        public Document Doc;


        private TaskBarViewModel _TaskBarViewModel;
        public TaskBarViewModel TaskBarViewModel { get { return _TaskBarViewModel; } set { _TaskBarViewModel = value; OnPropertyChanged(); } }

        private SaveModel _SaveModel;
        public SaveModel SaveModel { get { return _SaveModel; } set { _SaveModel = value; OnPropertyChanged(); } }


        private Visibility _SlantedVisibility;
        public Visibility SlantedVisibility { get { return _SlantedVisibility; } set { _SlantedVisibility = value; OnPropertyChanged(); } }
        private Visibility _TeeElbowVisibility;
        public Visibility TeeElbowVisibility { get { return _TeeElbowVisibility; } set { _TeeElbowVisibility = value; OnPropertyChanged(); } }
        private Visibility _LoadFamilyVisibility;
        public Visibility LoadFamilyVisibility { get { return _LoadFamilyVisibility; } set { _LoadFamilyVisibility = value; OnPropertyChanged(); } }

        private int _SelectedMenu;
        public int SelectedMenu
        {
            get { return _SelectedMenu; }
            set
            {
                _SelectedMenu = value; OnPropertyChanged();
                SlantedVisibility = (SelectedMenu == 0) ? Visibility.Visible : Visibility.Collapsed;
                TeeElbowVisibility = (SelectedMenu == 1) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Languages _Languages;
        public Languages Languages { get { return _Languages; } set { _Languages = value; OnPropertyChanged(); } }
        #region Command
        public ICommand LoadWindowCommand { get; set; }

        public ICommand CloseWindowCommand { get; set; }
        public ICommand SelectionLanguageChangedCommand { get; set; }
        public ICommand SelectionMenuCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand LoadFamilyCommand { get; set; }
        #endregion

        public SettingMEPViewModel(UIDocument uiDoc, Document doc)
        {
            UiDoc = uiDoc;
            Doc = doc;
            Languages = new Languages("EN");
            TaskBarViewModel = new TaskBarViewModel();

            SaveModel = SaveModel.GetSaveModel(Doc);

            LoadWindowCommand = new RelayCommand<SettingMEPWindow>((p) => { return true; }, (p) =>
           {
               SlantedVisibility = (SelectedMenu == 0) ? Visibility.Visible : Visibility.Collapsed;
               TeeElbowVisibility = (SelectedMenu == 1) ? Visibility.Visible : Visibility.Collapsed;
               LoadFamilyVisibility = (CoditionLoadFamily()) ? Visibility.Visible : Visibility.Collapsed;
               Draw(p);
           });
            SelectionLanguageChangedCommand = new RelayCommand<SettingMEPWindow>((p) => { return true; }, (p) =>
            {
                Languages.ChangedLanguage();
            });
            SelectionMenuCommand = new RelayCommand<SettingMEPWindow>((p) => { return true; }, (p) =>
            {
                Draw(p);
            });

            CloseWindowCommand = new RelayCommand<SettingMEPWindow>((p) => { return true; }, (p) =>
            {
                if ((SaveModel.HasSave == 0) && (System.Windows.Forms.MessageBox.Show("Do you want to save ?", "Save", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK))
                {
                    SaveFileAction();
                }
                else
                {
                    using (var stream = new FileStream(path: SaveModel.FilePathDefault(Doc), FileMode.OpenOrCreate))
                    {

                        SaveModel.SaveFile(stream);
                    }
                }
               
                
                p.DialogResult = true;

            });
            #region Command

            OpenCommand = new RelayCommand<SettingMEPWindow>((p) => { return true; }, (p) =>
            {

                OpenFileAction();

            });
            SaveCommand = new RelayCommand<SettingMEPWindow>((p) => { return true; }, (p) =>
            {
                SaveFileAction();

            });
            LoadFamilyCommand = new RelayCommand<SettingMEPWindow>((p) => { return true; }, (p) =>
            {
                p.DialogResult = true;

            });
            #endregion


        }


        private void OpenFileAction()
        {

            OpenFileDialog openFileDialog = new OpenFileDialog
            {

                Title = "Browse Text Files",
                DefaultExt = "dsp",
                Filter = ".dsp files (*.dsp)|*.dsp",
                RestoreDirectory = true,
            };

            if (openFileDialog.ShowDialog() == true)
            {
                using (var stream = new FileStream(path: openFileDialog.FileName, FileMode.OpenOrCreate))
                {
                    SaveModel = new SaveModel(stream);
                }
            }

        }

        private void SaveFileAction()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {

                Title = "Browse Text Files",
                DefaultExt = "dsp",
                Filter = ".dsp files (*.dsp)|*.dsp",
                FilterIndex = 2,
                RestoreDirectory = true,
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveModel.Directory = saveFileDialog.FileName;
                using (var stream = new FileStream(path: SaveModel.Directory, FileMode.OpenOrCreate))
                {
                    SaveModel.HasSave = 1;
                    SaveModel.SaveFile(stream);
                }

            }
        }

        private bool CoditionLoadFamily()
        {
            List<Family> list = new FilteredElementCollector(Doc).OfClass(typeof(Family)).Cast<Family>().Where(x => x.FamilyCategory.Name.Equals(Category.GetCategory(Doc, PileFittingID).Name)).ToList();
            list = list.Where(x => HasAllFamily(x)).ToList();
            return list.Count == 0;
        }

        private bool HasAllFamily(Family family)
        {
            bool a = family.Name.Equals("a");
            bool b1 = family.Name.Equals("C_Nipple");
            bool b2 = family.Name.Equals("Copper_Transistion_Ruby");
            bool b3 = family.Name.Equals("DSC_Copper_Elbow_Ruby");
            bool b4 = family.Name.Equals("DSC_Copper_Socket_Ruby");
            bool b5 = family.Name.Equals("DSC_Copper_Tee_Ruby");
            bool b6 = family.Name.Equals("DSC_PPR_Elbow_TienPhong");
            bool b7 = family.Name.Equals("DSC_PPR_Endcap_TienPhong");
            return (b1 || b2 || b3 || b4 || b5 || b6 || b7);
        }
        private void Draw(SettingMEPWindow p)
        {
            p.MainCanvas.Children.Clear();
            switch (SelectedMenu)
            {
                case 0: DrawMainCanvas.DrawSlanted(p.MainCanvas); break;
                case 1: DrawMainCanvas.DrawTeeElbow(p.MainCanvas); break;
                default: DrawMainCanvas.DrawSlanted(p.MainCanvas); break;
            }
        }
    }
}

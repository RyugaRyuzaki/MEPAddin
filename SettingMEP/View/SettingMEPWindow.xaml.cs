using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SettingMEP
{
    public partial class SettingMEPWindow
    {
        private SettingMEPViewModel _viewModel;

        public SettingMEPWindow(SettingMEPViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            this.DataContext = viewModel;
        }



    }
}

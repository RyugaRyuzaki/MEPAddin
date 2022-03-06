using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MEP_Addin
{
    public partial class APIWindow
    {
        private RAPI00ViewModel _viewModel;

        public APIWindow(RAPI00ViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            this.DataContext = viewModel;
        }



    }
}

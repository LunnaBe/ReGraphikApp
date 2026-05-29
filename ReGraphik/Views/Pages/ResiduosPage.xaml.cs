using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ReGraphik.ViewModels;

namespace ReGraphik.Views.Pages
{
    public partial class ResiduosPage : Page
    {
        public ResiduosPage()
        {
            InitializeComponent();
            DataContext = new ResiduoViewModel();
        }
    }
}
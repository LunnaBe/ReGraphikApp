using System;
using System.Windows;
using System.Windows.Controls;
using ReGraphik.ViewModels;
using ApiRestReGraphik.Models;

namespace ReGraphik.Views.Pages
{
    public partial class MapaPage : Page
    {
        private readonly MapaViewModel _viewModel;

        public MapaPage()
        {
            InitializeComponent();
            _viewModel = new MapaViewModel();
            DataContext = _viewModel;

            // Vincula os controles da interface à ViewModel
            _viewModel.MapaBrowser = this.MapaBrowser;
            _viewModel.ListaPontos = this.ListaPontos;
            _viewModel.EstadoVazio = this.EstadoVazio;
            _viewModel.MapaPlaceholder = this.MapaPlaceholder;
            _viewModel.EstadoCarregando = this.EstadoCarregando;
        }

        // Evento para lidar com a seleção de um ponto na lista
        private void ListaPontos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaPontos.SelectedItem is PontosColeta pontoSelecionado)
            {
                _viewModel.FocarNoPonto(pontoSelecionado);
            }
        }

        // Evento para notificar a ViewModel quando o mapa for carregado
        private void MapaBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            _viewModel.NotificarMapaCarregado();
        }
    }
}
using System;
using System.Windows.Input;

namespace ReGraphik.Commands
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object?> _executar;
        private readonly Func<object?, bool>? _podeExecutar;

        // Construtor para métodos COM parâmetro: new RelayCommand(async _ => await Metodo())
        public RelayCommand(Action<object?> executar, Func<object?, bool>? podeExecutar = null)
        {
            _executar = executar;
            _podeExecutar = podeExecutar;
        }

        // Construtor para métodos SEM parâmetro: new RelayCommand(Metodo)
        public RelayCommand(Action executar, Func<bool>? podeExecutar = null)
        {
            _executar = _ => executar();
            _podeExecutar = podeExecutar == null ? null : _ => podeExecutar();
        }

        public bool CanExecute(object? parameter) => _podeExecutar == null || _podeExecutar(parameter);

        public void Execute(object? parameter) => _executar(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
using System;
using System.Windows.Input;

namespace ModelView.Command
{
    /// Implementación reutilizable de ICommand para enlazar acciones del ViewModel con la Vista.
    /// - _execute: acción a ejecutar cuando el comando se invoca.
    /// - _canExecute: función opcional que determina si el comando puede ejecutarse.
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute; // acción que se ejecutará
        private readonly Predicate<object> _canExecute; // condición opcional para habilitar/deshabilitar

        // Constructor cuando siempre se puede ejecutar
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        // Constructor principal: recibes la acción a ejecutar y opcionalmente la condición de ejecución.
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute; // puede ser null
        }

        // Devuelve true/false para habilitar o deshabilitar el control enlazado al comando.
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        // Lógica real que se ejecuta cuando se dispara el comando (por ejemplo, al pulsar un botón)
        public void Execute(object parameter) => _execute(parameter);

        // Eventos que WPF escucha para reevaluar el estado del comando (habilitado/deshabilitado).
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

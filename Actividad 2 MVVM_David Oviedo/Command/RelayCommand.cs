using System;
using System.Windows.Input;

namespace Actividad_2_MVVM_David_Oviedo.Command
{
    /// Implementación reutilizable de ICommand para enlazar acciones del ViewModel con la Vista.
    /// - _execute: acción a ejecutar cuando el comando se invoca.
    /// - _canExecute: función opcional que determina si el comando puede ejecutarse.
    public class RelayCommand : ICommand
    {
        // Acción que se ejecutará cuando el comando se invoque.
        private readonly Action<object> _execute;
        // Función opcional que determina si el comando está habilitado.
        private readonly Predicate<object> _canExecute;

        // Constructor sencillo: siempre se puede ejecutar.
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        // Constructor completo: recibe acción y condición de habilitado.
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Devuelve true/false para habilitar o deshabilitar el control enlazado al comando.
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        // Lógica real que se ejecuta cuando se dispara el comando (por ejemplo, al pulsar un botón).
        public void Execute(object parameter) => _execute(parameter);

        // Eventos que WPF escucha para reevaluar el estado del comando (habilitado/deshabilitado).
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

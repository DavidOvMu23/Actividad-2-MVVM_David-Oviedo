using System;
using System.Windows.Input;

namespace ModelView.Command
{
    /// <summary>
    /// Comando reutilizable para enganchar acciones del ViewModel a la vista.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Constructor cuando siempre se puede ejecutar.
        /// </summary>
        /// <param name="execute">Acción a ejecutar.</param>
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Constructor principal con la acción y la condición opcional.
        /// </summary>
        /// <param name="execute">Acción a ejecutar.</param>
        /// <param name="canExecute">Condición opcional para habilitar/deshabilitar.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Devuelve si el comando puede ejecutarse.
        /// </summary>
        /// <param name="parameter">Parámetro del comando.</param>
        /// <returns><c>true</c> si se puede ejecutar; en caso contrario, <c>false</c>.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Ejecuta la acción del comando.
        /// </summary>
        /// <param name="parameter">Parámetro del comando.</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Evento para que WPF reevalúe si el comando puede ejecutarse.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

using ModelView.Command;
using Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ModelView
{
    /// <summary>
    /// ViewModel que conecta la ventana de socios con los datos y la lógica.
    /// </summary>
    public class SociosViewModel : INotifyPropertyChanged
    {
        private readonly Repositorio _repositorio; // acceso a la BD (CRUD)
        private ObservableCollection<Socio> _listaSocios; // colección que muestra el DataGrid
        private Socio _socioEnFormulario; // objeto enlazado a los TextBox/CheckBox
        private Socio _socioSeleccionadoGrid; // fila seleccionada en el DataGrid

        /// <summary>
        /// Lista de socios para el DataGrid.
        /// </summary>
        public ObservableCollection<Socio> ListaSocios
        {
            get { return _listaSocios; }
            set { _listaSocios = value; OnPropertyChanged(nameof(ListaSocios)); }
        }

        /// <summary>
        /// Socio enlazado al formulario.
        /// </summary>
        public Socio SocioEnFormulario
        {
            get { return _socioEnFormulario; }
            set { _socioEnFormulario = value; OnPropertyChanged(nameof(SocioEnFormulario)); }
        }

        /// <summary>
        /// Socio seleccionado en el DataGrid.
        /// </summary>
        public Socio SocioSeleccionadoGrid
        {
            get { return _socioSeleccionadoGrid; }
            set
            {
                _socioSeleccionadoGrid = value;
                OnPropertyChanged(nameof(SocioSeleccionadoGrid));
                CommandManager.InvalidateRequerySuggested(); // refresca CanExecute de los comandos
            }
        }

        /// <summary>
        /// Comando para guardar.
        /// </summary>
        public ICommand GuardarCommand { get; private set; }
        /// <summary>
        /// Comando para crear un socio nuevo.
        /// </summary>
        public ICommand NuevoCommand { get; private set; }
        /// <summary>
        /// Comando para eliminar.
        /// </summary>
        public ICommand EliminarCommand { get; private set; }
        /// <summary>
        /// Comando para editar.
        /// </summary>
        public ICommand EditarCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged; // me lo ha hecho el chat

        /// <summary>
        /// Constructor del ViewModel de socios.
        /// </summary>
        public SociosViewModel()
        {
            _repositorio = new Repositorio();
            ListaSocios = new ObservableCollection<Socio>(_repositorio.SeleccionarSocios());
            SocioEnFormulario = CrearNuevoSocio(); // inicia el formulario limpio

            GuardarCommand = new RelayCommand(GuardarAccion);
            NuevoCommand = new RelayCommand(NuevoAccion);
            EliminarCommand = new RelayCommand(EliminarAccion, PuedeEditarEliminar);
            EditarCommand = new RelayCommand(EditarAccion, PuedeEditarEliminar);
        }

        private bool PuedeEditarEliminar(object parameter)
        {
            // Solo habilita Editar/Eliminar si hay algo seleccionado en el DataGrid
            return SocioSeleccionadoGrid != null;
        }

        private Socio CrearNuevoSocio()
        {
            return new Socio { Activo = true }; // valores por defecto
        }

        private void Nuevo()
        {
            SocioEnFormulario = CrearNuevoSocio(); // limpia el formulario
        }

        private void Editar()
        {
            if (SocioSeleccionadoGrid == null) return;
            // pasa la fila seleccionada al formulario para editarla
            SocioEnFormulario = SocioSeleccionadoGrid;
        }

        private void Guardar()
        {
            if (SocioEnFormulario == null) return;

            // Validación simple de campos obligatorios
            if (string.IsNullOrWhiteSpace(SocioEnFormulario.Nombre) || string.IsNullOrWhiteSpace(SocioEnFormulario.Email))
            {
                MessageBox.Show("Rellena nombre y email", "Aviso");
                return;
            }

            // Validación del formato de email
            if (!_repositorio.EsEmailValido(SocioEnFormulario.Email))
            {
                MessageBox.Show("Formato de email no válido", "Aviso");
                return;
            }

            var esNuevo = SocioEnFormulario.Id == 0;

            if (esNuevo)
            {
                _repositorio.GuardarSocio(SocioEnFormulario);
                MessageBox.Show("Socio creado", "Aviso");
            }
            else
            {
                _repositorio.ActualizarSocio(SocioEnFormulario);
                MessageBox.Show("Socio actualizado", "Aviso");
            }

            // Refrescamos el grid y limpiamos selección/formulario
            ListaSocios = new ObservableCollection<Socio>(_repositorio.SeleccionarSocios());
            SocioSeleccionadoGrid = null;
            SocioEnFormulario = CrearNuevoSocio();
        }

        private void Eliminar()
        {
            // Usamos el socio en formulario si tiene Id; si no, el seleccionado en el grid
            var socioObjetivo = SocioEnFormulario != null && SocioEnFormulario.Id != 0
                ? SocioEnFormulario
                : SocioSeleccionadoGrid;

            if (socioObjetivo == null || socioObjetivo.Id == 0)
            {
                MessageBox.Show("Selecciona un socio para eliminar", "Aviso");
                return;
            }

            var eliminado = _repositorio.EliminarSocio(socioObjetivo);
            if (!eliminado)
            {
                MessageBox.Show("No se puede eliminar porque tiene reservas.", "Aviso");
                return;
            }

            ListaSocios = new ObservableCollection<Socio>(_repositorio.SeleccionarSocios());
            SocioSeleccionadoGrid = null;
            SocioEnFormulario = CrearNuevoSocio();
            MessageBox.Show("Socio eliminado", "Aviso");
        }

        // Métodos adaptadores para RelayCommand
        private void GuardarAccion(object parameter)
        {
            Guardar();
        }

        private void NuevoAccion(object parameter)
        {
            Nuevo();
        }

        private void EliminarAccion(object parameter)
        {
            Eliminar();
        }

        private void EditarAccion(object parameter)
        {
            Editar();
        }

        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}

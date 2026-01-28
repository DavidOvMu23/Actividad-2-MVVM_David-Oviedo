using ModelView.Command;
using Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ModelView
{
    // ViewModel que conecta la ventana de actividades con los datos y lógica.
    public class ActividadesViewModel : INotifyPropertyChanged
    {
        private readonly Repositorio _repositorio; // acceso a la BD
        private ObservableCollection<Actividad> _listaActividades; // datos para el DataGrid
        private Actividad _actividadEnFormulario; // objeto enlazado al formulario
        private Actividad _actividadSeleccionadaGrid; // fila seleccionada en el DataGrid

        // Lista de actividades para el DataGrid
        public ObservableCollection<Actividad> ListaActividades
        {
            get { return _listaActividades; }
            set { _listaActividades = value; OnPropertyChanged(nameof(ListaActividades)); }
        }

        // Objeto enlazado al formulario de edición/creación
        public Actividad ActividadEnFormulario
        {
            get { return _actividadEnFormulario; }
            set { _actividadEnFormulario = value; OnPropertyChanged(nameof(ActividadEnFormulario)); }
        }

        // Fila seleccionada en el DataGrid
        public Actividad ActividadSeleccionadaGrid
        {
            get { return _actividadSeleccionadaGrid; }
            set
            {
                _actividadSeleccionadaGrid = value;
                OnPropertyChanged(nameof(ActividadSeleccionadaGrid));
                CommandManager.InvalidateRequerySuggested(); // refresca CanExecute de los comandos
            }
        }

        // Comandos usados en la vista
        public ICommand GuardarCommand { get; private set; }
        public ICommand NuevoCommand { get; private set; }
        public ICommand EliminarCommand { get; private set; }
        public ICommand EditarCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged; // notificación de cambios a la vista (me lo ha hecho el chat)

        // Constructor para inicializar datos y comandos
        public ActividadesViewModel()
        {
            _repositorio = new Repositorio();
            ListaActividades = new ObservableCollection<Actividad>(_repositorio.SeleccionarActividades());
            ActividadEnFormulario = CrearNuevaActividad();

            // Relacionamos botones con acciones y condiciones de habilitado
            GuardarCommand = new RelayCommand(GuardarAccion);
            NuevoCommand = new RelayCommand(NuevoAccion);
            EliminarCommand = new RelayCommand(EliminarAccion, PuedeEditarEliminar);
            EditarCommand = new RelayCommand(EditarAccion, PuedeEditarEliminar);
        }

        // Crea una nueva actividad con valores por defecto
        private Actividad CrearNuevaActividad()
        {
            return new Actividad { AforoMaximo = 1 }; // valor por defecto
        }

        // Acción para el comando Nuevo
        private void Nuevo()
        {
            ActividadEnFormulario = CrearNuevaActividad(); // limpia el formulario
        }

        // Acción para el comando Editar
        private void Editar()
        {
            if (ActividadSeleccionadaGrid == null) return;
            // pasa la selección del grid al formulario
            ActividadEnFormulario = ActividadSeleccionadaGrid;
        }

        // Acción para el comando Guardar
        private void Guardar()
        {
            if (ActividadEnFormulario == null) return;

            // Validar que tiene nombre y aforo válido
            if (string.IsNullOrWhiteSpace(ActividadEnFormulario.Nombre) || ActividadEnFormulario.AforoMaximo <= 0)
            {
                MessageBox.Show("Rellena nombre y un aforo mayor que 0", "Aviso");
                return;
            }

            var esNuevo = ActividadEnFormulario.Id == 0;

            // si el socio es nuevo, lo crea  si no, lo actualiza
            if (esNuevo)
            {
                _repositorio.GuardarActividad(ActividadEnFormulario);
                MessageBox.Show("Actividad creada", "Aviso");
            }
            else
            {
                _repositorio.ActualizarActividad(ActividadEnFormulario);
                MessageBox.Show("Actividad actualizada", "Aviso");
            }

            // Refrescamos grid y limpiamos selección/formulario
            ListaActividades = new ObservableCollection<Actividad>(_repositorio.SeleccionarActividades());
            ActividadSeleccionadaGrid = null;
            ActividadEnFormulario = CrearNuevaActividad();
        }

        private bool PuedeEditarEliminar(object parameter)
        {
            // Solo habilita Editar/Eliminar si hay una fila seleccionada
            return ActividadSeleccionadaGrid != null;
        }

        private void Eliminar()
        {
            // Preferimos la del formulario si tiene Id; si no, la seleccionada
            var actividadObjetivo = ActividadEnFormulario != null && ActividadEnFormulario.Id != 0
                ? ActividadEnFormulario
                : ActividadSeleccionadaGrid;

            // Validar que hay una actividad seleccionada
            if (actividadObjetivo == null || actividadObjetivo.Id == 0)
            {
                MessageBox.Show("Selecciona una actividad para eliminar", "Aviso");
                return;
            }

            // Intentar eliminar la actividad
            var eliminado = _repositorio.EliminarActividad(actividadObjetivo);
            if (!eliminado)
            {
                MessageBox.Show("No se puede eliminar porque tiene reservas.", "Aviso");
                return;
            }

            ListaActividades = new ObservableCollection<Actividad>(_repositorio.SeleccionarActividades());
            ActividadSeleccionadaGrid = null;
            ActividadEnFormulario = CrearNuevaActividad();
            MessageBox.Show("Actividad eliminada", "Aviso");
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

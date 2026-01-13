using Actividad_2_MVVM_David_Oviedo.Model;
using Actividad_2_MVVM_David_Oviedo.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Actividad_2_MVVM_David_Oviedo.ViewModel
{
    // ViewModel que conecta la vista con los datos y la lógica
    public class ActividadesViewModel : INotifyPropertyChanged
    {
        // Puerta de entrada a la base de datos usando el repositorio.
        private readonly Repositorio _repositorio;

        // Colección para mostrar la información en el DataGrid. Si cambia, la vista se actualiza sola.
        private ObservableCollection<Actividad> _actividades;

        // Para seleccionar una actividad en el DataGrid
        private Actividad _actividadSeleccionada;

        // Actividad que se está editando en el formulario (puede ser nueva o una existente).
        private Actividad _actividadFormulario;

        // Lista de actividades para el DataGrid.
        public ObservableCollection<Actividad> Actividades
        {
            get => _actividades;
            set
            {
                _actividades = value;
                OnPropertyChanged(nameof(Actividades));
            }
        }

        // Referencia a la fila seleccionada en el DataGrid para editar o borrar
        public Actividad ActividadSeleccionada
        {
            get => _actividadSeleccionada;
            set
            {
                _actividadSeleccionada = value;
                OnPropertyChanged(nameof(ActividadSeleccionada));
            }
        }

        // Objeto que alimenta el formulario (los TextBox).
        public Actividad ActividadFormulario
        {
            get => _actividadFormulario;
            set
            {
                _actividadFormulario = value;
                OnPropertyChanged(nameof(ActividadFormulario));
            }
        }

        // Comandos para enlazar a los botones del XAML. RelayCommand ejecuta el método que pasamos.
        public ICommand GuardarActividadCommand { get; }
        public ICommand NuevaActividadCommand { get; }
        public ICommand EliminarActividadCommand { get; }
        public ICommand EditarActividadCommand { get; }

        // Evento que notifica a la vista que una propiedad cambió.
        public event PropertyChangedEventHandler PropertyChanged;

        // Constructor que inicializa el ViewModel.
        public ActividadesViewModel()
        {
            _repositorio = new Repositorio();
            // Cargar datos iniciales para el DataGrid.
            Actividades = new ObservableCollection<Actividad>(_repositorio.SeleccionarActividades());
            // Preparar el formulario
            ActividadFormulario = CrearNuevaActividad();

            // Crear comandos que la vista usará. Cada botón dispara el método indicado.
            GuardarActividadCommand = new RelayCommand(_ => GuardarActividad());
            NuevaActividadCommand = new RelayCommand(_ => NuevaActividad());
            EliminarActividadCommand = new RelayCommand(EliminarActividad, _ => PuedeEliminar());
            EditarActividadCommand = new RelayCommand(_ => EditarActividad(), _ => PuedeEditar());
        }

        // Devuelve un objeto nuevo listo para usar en el formulario.
        private Actividad CrearNuevaActividad()
        {
            return new Actividad { AforoMaximo = 1 };
        }

        // Limpia el formulario y deselecciona el DataGrid.
        private void NuevaActividad()
        {
            ActividadSeleccionada = null;
            ActividadFormulario = CrearNuevaActividad();
        }

        // Copia la actividad seleccionada al formulario para editarla
        private void EditarActividad()
        {
            if (ActividadSeleccionada == null)
                return;

            ActividadFormulario = ActividadSeleccionada;
        }

        // Valida y guarda: si la actividad es nueva la inserta; si existe, actualiza.
        private void GuardarActividad()
        {
            // Obtener la actividad del formulario.
            var actividad = ActividadFormulario;
            if (actividad == null)
                return;

            // Validación básica: nombre obligatorio.
            if (string.IsNullOrWhiteSpace(actividad.Nombre))
            {
                MessageBox.Show("Debe introducir el nombre de la actividad.", "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.None);
                return;
            }

            // Validación de aforo mínimo 1.
            if (actividad.AforoMaximo <= 0)
            {
                actividad.AforoMaximo = 1;
                OnPropertyChanged(nameof(ActividadFormulario));
                MessageBox.Show("El aforo máximo debe ser al menos 1.", "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.None);
                return;
            }

            // Si Id==0 es nueva: se añade al contexto y a la lista observable.
            if (actividad.Id == 0)
            {
                _repositorio.GuardarActividad(actividad);
                Actividades.Add(actividad);
                MessageBox.Show("Actividad creada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.None);
                ActividadFormulario = CrearNuevaActividad();
            }
            else
            {
                // Si ya existe: se guardan cambios y se limpia el formulario.
                _repositorio.ActualizarActividad();
                MessageBox.Show("Actividad editada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.None);
                ActividadFormulario = CrearNuevaActividad();
                ActividadSeleccionada = null;
            }
        }

        // Controla si el botón Eliminar debe estar habilitado.
        private bool PuedeEliminar()
        {
            return ActividadSeleccionada != null && ActividadSeleccionada.Id != 0;
        }

        // Controla si el botón Editar debe estar habilitado.
        private bool PuedeEditar()
        {
            return ActividadSeleccionada != null;
        }

        // Elimina la actividad seleccionada (si tiene Id) y resetea el formulario.
        private void EliminarActividad(object parameter)
        {
            var actividad = parameter as Actividad ?? ActividadSeleccionada;
            if (actividad == null || actividad.Id == 0)
                return;

            _repositorio.EliminarActividad(actividad);
            Actividades.Remove(actividad);
            MessageBox.Show("Actividad eliminada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.None);
            ActividadSeleccionada = null;
            ActividadFormulario = CrearNuevaActividad();
        }

        // Notifica a la vista que una propiedad cambió para refrescar los bindings.
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using Actividad_2_MVVM_David_Oviedo.Model;
using Actividad_2_MVVM_David_Oviedo.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Actividad_2_MVVM_David_Oviedo.ViewModel
{
    public class ActividadesViewModel : INotifyPropertyChanged
    {
        private readonly Repositorio _repositorio;
        private ObservableCollection<Actividad> _actividades;
        private Actividad _actividadSeleccionada;
        private Actividad _actividadFormulario;

        public ObservableCollection<Actividad> Actividades
        {
            get => _actividades;
            set
            {
                _actividades = value;
                OnPropertyChanged(nameof(Actividades));
            }
        }

        public Actividad ActividadSeleccionada
        {
            get => _actividadSeleccionada;
            set
            {
                _actividadSeleccionada = value;
                OnPropertyChanged(nameof(ActividadSeleccionada));
            }
        }

        public Actividad ActividadFormulario
        {
            get => _actividadFormulario;
            set
            {
                _actividadFormulario = value;
                OnPropertyChanged(nameof(ActividadFormulario));
            }
        }

        public ICommand GuardarActividadCommand { get; }
        public ICommand NuevaActividadCommand { get; }
        public ICommand EliminarActividadCommand { get; }
        public ICommand EditarActividadCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ActividadesViewModel()
        {
            _repositorio = new Repositorio();
            Actividades = new ObservableCollection<Actividad>(_repositorio.SeleccionarActividades());
            ActividadFormulario = CrearNuevaActividad();

            GuardarActividadCommand = new RelayCommand(_ => GuardarActividad());
            NuevaActividadCommand = new RelayCommand(_ => NuevaActividad());
            EliminarActividadCommand = new RelayCommand(EliminarActividad, _ => PuedeEliminar());
            EditarActividadCommand = new RelayCommand(_ => EditarActividad(), _ => PuedeEditar());
        }

        private Actividad CrearNuevaActividad()
        {
            return new Actividad { AforoMaximo = 1 };
        }

        private void NuevaActividad()
        {
            ActividadSeleccionada = null;
            ActividadFormulario = CrearNuevaActividad();
        }

        private void EditarActividad()
        {
            if (ActividadSeleccionada == null)
                return;

            ActividadFormulario = ActividadSeleccionada;
        }

        private void GuardarActividad()
        {
            var actividad = ActividadFormulario;
            if (actividad == null)
                return;

            if (string.IsNullOrWhiteSpace(actividad.Nombre))
            {
                MessageBox.Show("Debe introducir el nombre de la actividad.", "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.None);
                return;
            }

            if (actividad.AforoMaximo <= 0)
            {
                actividad.AforoMaximo = 1;
                OnPropertyChanged(nameof(ActividadFormulario));
                MessageBox.Show("El aforo máximo debe ser al menos 1.", "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.None);
                return;
            }

            if (actividad.Id == 0)
            {
                _repositorio.GuardarActividad(actividad);
                Actividades.Add(actividad);
                MessageBox.Show("Actividad creada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.None);
                ActividadFormulario = CrearNuevaActividad();
            }
            else
            {
                _repositorio.ActualizarActividad();
                MessageBox.Show("Actividad editada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.None);
                ActividadFormulario = CrearNuevaActividad();
                ActividadSeleccionada = null;
            }
        }

        private bool PuedeEliminar()
        {
            return ActividadSeleccionada != null && ActividadSeleccionada.Id != 0;
        }

        private bool PuedeEditar()
        {
            return ActividadSeleccionada != null;
        }

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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

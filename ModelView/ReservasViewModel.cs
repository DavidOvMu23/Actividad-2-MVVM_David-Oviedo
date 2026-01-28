using ModelView.Command;
using Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ModelView
{
    // ViewModel que conecta la vista de reservas con la lógica y datos.
    public class ReservasViewModel : INotifyPropertyChanged
    {
        private readonly Repositorio _repositorio; // acceso a la BD
        private ObservableCollection<Reserva> _listaReservas; // datos para el DataGrid
        private ObservableCollection<Socio> _listaSocios; // opciones del combo socio
        private ObservableCollection<Actividad> _listaActividades; // opciones del combo actividad
        private Reserva _reservaEnFormulario; // modelo que enlaza con los campos del formulario
        private Reserva _reservaSeleccionadaGrid; // fila seleccionada en el DataGrid

        public ObservableCollection<Reserva> ListaReservas
        {
            get { return _listaReservas; }
            set { _listaReservas = value; OnPropertyChanged(nameof(ListaReservas)); }
        }

        public ObservableCollection<Socio> ListaSocios
        {
            get { return _listaSocios; }
            set { _listaSocios = value; OnPropertyChanged(nameof(ListaSocios)); }
        }

        public ObservableCollection<Actividad> ListaActividades
        {
            get { return _listaActividades; }
            set { _listaActividades = value; OnPropertyChanged(nameof(ListaActividades)); }
        }

        // Enlace con los TextBox/ComboBox/DatePicker
        public Reserva ReservaEnFormulario
        {
            get { return _reservaEnFormulario; }
            set { _reservaEnFormulario = value; OnPropertyChanged(nameof(ReservaEnFormulario)); }
        }

        // Fila seleccionada en el grid; al cambiar refrescamos comandos (habilita/deshabilita botones)
        public Reserva ReservaSeleccionadaGrid
        {
            get { return _reservaSeleccionadaGrid; }
            set
            {
                _reservaSeleccionadaGrid = value;
                OnPropertyChanged(nameof(ReservaSeleccionadaGrid));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Comandos que la vista llama (botones) usando RelayCommand
        public ICommand GuardarCommand { get; private set; }
        public ICommand NuevoCommand { get; private set; }
        public ICommand EliminarCommand { get; private set; }
        public ICommand EditarCommand { get; private set; }
        public ICommand AbrirSociosCommand { get; private set; }
        public ICommand AbrirActividadesCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ReservasViewModel()
        {
            _repositorio = new Repositorio();
            ListaReservas = new ObservableCollection<Reserva>(_repositorio.SeleccionarReservas());
            ListaSocios = new ObservableCollection<Socio>(_repositorio.SeleccionarSocios());
            ListaActividades = new ObservableCollection<Actividad>(_repositorio.SeleccionarActividades());
            ReservaEnFormulario = CrearNuevaReserva(); // inicializamos formulario vacío

            // Vinculamos cada botón con una acción y, en algunos, con la condición de habilitado
            GuardarCommand = new RelayCommand(GuardarAccion);
            NuevoCommand = new RelayCommand(NuevoAccion);
            EliminarCommand = new RelayCommand(EliminarAccion, PuedeEditarEliminar);
            EditarCommand = new RelayCommand(EditarAccion, PuedeEditarEliminar);
        }

        private Reserva CrearNuevaReserva()
        {
            return new Reserva { Fecha = DateTime.Today }; // valor por defecto en fecha
        }

        private void Nuevo()
        {
            ReservaEnFormulario = CrearNuevaReserva(); // limpia el formulario
        }

        private void Editar()
        {
            if (ReservaSeleccionadaGrid == null) return;
            // Copiamos la selección del grid al formulario para editarla
            ReservaEnFormulario = ReservaSeleccionadaGrid;
        }

        private void Guardar()
        {
            if (ReservaEnFormulario == null) return;

            // Validaciones básicas de formulario
            if (ReservaEnFormulario.SocioId <= 0 || ReservaEnFormulario.ActividadId <= 0 || ReservaEnFormulario.Fecha == default(DateTime))
            {
                MessageBox.Show("Selecciona socio, actividad y fecha", "Aviso");
                return;
            }

            var esNuevo = ReservaEnFormulario.Id == 0;
            bool ok;

            // Comprobamos aforo y duplicados con ayuda del repositorio
            if (!_repositorio.HayPlazasActividad(ReservaEnFormulario.ActividadId, esNuevo ? (int?)null : ReservaEnFormulario.Id))
            {
                MessageBox.Show("No hay plazas disponibles en esta actividad.", "Aviso");
                return;
            }

            if (_repositorio.ExisteReservaDuplicada(ReservaEnFormulario.SocioId, ReservaEnFormulario.ActividadId, ReservaEnFormulario.Fecha, esNuevo ? (int?)null : ReservaEnFormulario.Id))
            {
                MessageBox.Show("Ese socio ya tiene reserva en esa actividad y fecha.", "Aviso");
                return;
            }

            // Guardamos o actualizamos según corresponda
            if (esNuevo)
            {
                ok = _repositorio.GuardarReserva(ReservaEnFormulario);
                if (!ok)
                {
                    MessageBox.Show("No se pudo guardar la reserva.", "Aviso");
                    return;
                }
                MessageBox.Show("Reserva creada", "Aviso");
            }
            else
            {
                ok = _repositorio.ActualizarReserva(ReservaEnFormulario);
                if (!ok)
                {
                    MessageBox.Show("No se pudo actualizar la reserva.", "Aviso");
                    return;
                }
                MessageBox.Show("Reserva actualizada", "Aviso");
            }

            // Tras guardar: refrescamos datos y limpiamos selección/formulario
            RecargarListas();
            ReservaSeleccionadaGrid = null;
            ReservaEnFormulario = CrearNuevaReserva();
        }

        private bool PuedeEditarEliminar(object parameter)
        {
            // Solo permitimos editar/eliminar si hay algo seleccionado en el grid
            return ReservaSeleccionadaGrid != null;
        }

        private void Eliminar()
        {
            // Usamos el seleccionado en el formulario si tiene Id, si no el del grid
            var reservaObjetivo = ReservaEnFormulario != null && ReservaEnFormulario.Id != 0
                ? ReservaEnFormulario
                : ReservaSeleccionadaGrid;

            if (reservaObjetivo == null || reservaObjetivo.Id == 0)
            {
                MessageBox.Show("Selecciona una reserva para eliminar", "Aviso");
                return;
            }

            var eliminado = _repositorio.EliminarReserva(reservaObjetivo);
            if (!eliminado)
            {
                MessageBox.Show("No se pudo eliminar la reserva", "Aviso");
                return;
            }

            RecargarListas();
            ReservaSeleccionadaGrid = null;
            ReservaEnFormulario = CrearNuevaReserva();
            MessageBox.Show("Reserva eliminada", "Aviso");
        }


        private void RecargarListas()
        {
            // Vuelve a leer datos de la BD para que el grid y combos estén actualizados
            ListaReservas = new ObservableCollection<Reserva>(_repositorio.SeleccionarReservas());
            ListaSocios = new ObservableCollection<Socio>(_repositorio.SeleccionarSocios());
            ListaActividades = new ObservableCollection<Actividad>(_repositorio.SeleccionarActividades());
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

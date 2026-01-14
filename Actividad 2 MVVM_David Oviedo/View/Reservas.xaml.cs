using Actividad_2_MVVM_David_Oviedo.ViewModel;
using System.Windows;

namespace Actividad_2_MVVM_David_Oviedo.View
{
    /// <summary>
    /// Lógica de interacción para Reservas.xaml
    /// </summary>
    public partial class Reservas : Window
    {
        public Reservas()
        {
            InitializeComponent();
            // Asignamos el ViewModel como DataContext para que el XAML pueda enlazar sus propiedades y comandos.
            DataContext = new ReservasViewModel();
        }
    }
}

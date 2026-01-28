using ModelView;
using System.Windows;

namespace View
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

        private void button_socios_Click(object sender, RoutedEventArgs e)
        {
            var ventanaSocios = new Socios();
            ventanaSocios.ShowDialog();
        }

        private void button_Actividades_Click(object sender, RoutedEventArgs e)
        {
            var ventanaActividades = new Actividades();
            ventanaActividades.ShowDialog();
        }
    }
}

using ModelView;
using System.Windows;

namespace View
{
    /// <summary>
    /// Lógica de interacción para Actividades.xaml
    /// </summary>
    public partial class Actividades : Window
    {
        public Actividades()
        {
            InitializeComponent();
            // Asignamos el ViewModel como DataContext para que el XAML pueda enlazar sus propiedades y comandos.
            DataContext = new ActividadesViewModel();
        }
    }
}

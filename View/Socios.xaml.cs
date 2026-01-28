using ModelView;
using System.Windows;

namespace View
{
    /// <summary>
    /// Lógica de interacción para Socios.xaml
    /// </summary>
    public partial class Socios : Window
    {
        public Socios()
        {
            InitializeComponent();
            // Asignamos el ViewModel como DataContext para que el XAML pueda enlazar sus propiedades y comandos.
            DataContext = new SociosViewModel();
        }
    }
}

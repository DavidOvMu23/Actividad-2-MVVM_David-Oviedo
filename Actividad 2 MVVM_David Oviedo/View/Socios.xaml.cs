using Actividad_2_MVVM_David_Oviedo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Actividad_2_MVVM_David_Oviedo.View
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

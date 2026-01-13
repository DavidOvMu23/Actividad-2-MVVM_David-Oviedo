using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actividad_2_MVVM_David_Oviedo.Model
{
    public class Repositorio
    {
        private readonly CentroDeportivoEntities _context = new CentroDeportivoEntities();

        // MÉTODOS PARA ACTIVIDADES
        public List<Actividad> SeleccionarActividades()
        {
            return _context.Actividad.ToList();
        }
        
        public void GuardarActividad(Actividad actividad)
        {
            _context.Actividad.Add(actividad);
            _context.SaveChanges();
        }

        public Actividad BuscarActividad(int id)
        {
            return _context.Actividad.Find(id);
        }

        public void ActualizarActividad()
        {
            _context.SaveChanges();
        }

        public void EliminarActividad(Actividad actividad)
        {
            _context.Actividad.Remove(actividad);
            _context.SaveChanges();
        }

        // MÉTODOS PARA SOCIOS
        public List<Socio> SeleccionarSocios()
        {
            return _context.Socio.ToList();
        }

        public void GuardarSocio(Socio socio)
        {
            _context.Socio.Add(socio);
            _context.SaveChanges();
        }

        public Socio BuscarSocio(int id)
        {
            return _context.Socio.Find(id);
        }

        public void ActualizarSocio()
        {
            _context.SaveChanges();
        }

        public void EliminarSocio(Socio socio)
        {
            _context.Socio.Remove(socio);
            _context.SaveChanges();
        }

        // MÉTODOS PARA RESERVAS
        public List<Reserva> SeleccionarReservas()
        {
            return _context.Reserva.ToList();
        }

        public List<Reserva> SeleccionarReservasPorSocio(int idSocio)
        {
            return _context.Reserva.Where(r => r.Id == idSocio).ToList();
        }

        public List<Reserva> SeleccionarReservasPorActividad(int idActividad)
        {
            return _context.Reserva.Where(r => r.Id == idActividad).ToList();
        }

        public void GuardarReserva(Reserva reserva)
        {
            _context.Reserva.Add(reserva);
            _context.SaveChanges();
        }

        public Reserva BuscarReserva(int id)
        {
            return _context.Reserva.Find(id);
        }

        public void EliminarReserva(Reserva reserva)
        {
            _context.Reserva.Remove(reserva);
            _context.SaveChanges();
        }
    }
}

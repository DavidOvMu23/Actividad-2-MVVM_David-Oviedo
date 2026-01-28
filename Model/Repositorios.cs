using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Model
{
    public class Repositorio
    {
        // es el objeto que abre la conexión con la base de datos.
        private readonly Model1 _context = new Model1();

        // Comprueba si aún quedan plazas en la actividad (aforo disponible).
        // Recorre todas las reservas para contar cuántas son de esa actividad
        // y opcionalmente excluye una reserva (cuando editamos) para no contarse a sí misma.
        public bool HayPlazasActividad(int actividadId, int? reservaIdExcluir = null)
        {
            var actividad = _context.Actividad.Find(actividadId);
            if (actividad == null) return false; // si no existe la actividad, devolvemos false

            var ocupados = 0;
            foreach (var r in _context.Reserva)
            {
                if (r.ActividadId == actividadId && (!reservaIdExcluir.HasValue || r.Id != reservaIdExcluir.Value))
                {
                    ocupados++; // sumamos una plaza ocupada
                }
            }

            // Hay plazas libres cuando ocupados es menor que el aforo máximo.
            return ocupados < actividad.AforoMaximo;
        }

        // Comprueba si ya existe una reserva para el mismo socio, actividad y fecha
        // Recorremos todas las reservas comparando socio, actividad y solo la parte de fecha
        public bool ExisteReservaDuplicada(int socioId, int actividadId, System.DateTime fecha, int? reservaIdExcluir = null)
        {
            foreach (var r in _context.Reserva)
            {
                var mismaFecha = DbFunctions.TruncateTime(r.Fecha) == DbFunctions.TruncateTime(fecha);
                if ((!reservaIdExcluir.HasValue || r.Id != reservaIdExcluir.Value)
                    && r.SocioId == socioId
                    && r.ActividadId == actividadId
                    && mismaFecha)
                {
                    return true; // encontramos otra reserva igual
                }
            }
            return false; // no se encontró duplicado
        }

        // ACTIVIDADES
        // Devuelve todas las actividades de la base de datos en una lista.
        public List<Actividad> SeleccionarActividades()
        {
            return _context.Actividad.ToList();
        }

        // Inserta una nueva actividad.
        public void GuardarActividad(Actividad actividad)
        {
            _context.Actividad.Add(actividad);
            _context.SaveChanges();
        }

        // Actualiza los datos de una actividad existente.
        public void ActualizarActividad(Actividad actividad)
        {
            var existente = _context.Actividad.Find(actividad.Id);
            if (existente == null) return;
            existente.Nombre = actividad.Nombre;
            existente.AforoMaximo = actividad.AforoMaximo;
            _context.SaveChanges();
        }

        // Intenta eliminar una actividad. Devuelve false si no existe o si tiene reservas asociadas.
        public bool EliminarActividad(Actividad actividad)
        {
            var existente = _context.Actividad.Find(actividad.Id);
            if (existente == null) return false;

            var tieneReservas = false;
            foreach (var r in _context.Reserva)
            {
                if (r.ActividadId == actividad.Id)
                {
                    tieneReservas = true; // hay reservas que la usan
                    break;
                }
            }
            if (tieneReservas) return false; // evitamos romper la relación

            _context.Actividad.Remove(existente);
            _context.SaveChanges();
            return true;
        }

        // SOCIOS
        // Devuelve todos los socios.
        public List<Socio> SeleccionarSocios()
        {
            return _context.Socio.ToList();
        }

        // Inserta un nuevo socio.
        public void GuardarSocio(Socio socio)
        {
            _context.Socio.Add(socio);
            _context.SaveChanges();
        }

        // Actualiza un socio existente.
        public void ActualizarSocio(Socio socio)
        {
            var existente = _context.Socio.Find(socio.Id);
            if (existente == null) return;
            existente.Nombre = socio.Nombre;
            existente.Email = socio.Email;
            existente.Activo = socio.Activo;
            _context.SaveChanges();
        }

        // Intenta eliminar un socio. Devuelve false si no existe o si tiene reservas.
        public bool EliminarSocio(Socio socio)
        {
            var existente = _context.Socio.Find(socio.Id);
            if (existente == null) return false;

            var tieneReservas = false;
            foreach (var r in _context.Reserva)
            {
                if (r.SocioId == socio.Id)
                {
                    tieneReservas = true; // no se puede borrar si tiene reservas
                    break;
                }
            }
            if (tieneReservas) return false;

            _context.Socio.Remove(existente);
            _context.SaveChanges();
            return true;
        }

        // RESERVAS
        // Devuelve las reservas e incluye los datos de socio y actividad para mostrarlos en la vista.
        public List<Reserva> SeleccionarReservas()
        {
                return _context.Reserva.
                                Include("Socio").
                                Include("Actividad").
                                ToList();

        }

        // Inserta una nueva reserva. Devuelve false si no hay aforo o si ya existe.
        public bool GuardarReserva(Reserva reserva)
        {
            if (!HayPlazasActividad(reserva.ActividadId)) return false; // aforo lleno
            if (ExisteReservaDuplicada(reserva.SocioId, reserva.ActividadId, reserva.Fecha)) return false; // ya reservó mismo día

            _context.Reserva.Add(reserva);
            _context.SaveChanges();
            return true;
        }

        // Actualiza una reserva existente. Devuelve false si falla alguna validación.
        public bool ActualizarReserva(Reserva reserva)
        {
            var existente = _context.Reserva.Find(reserva.Id);
            if (existente == null) return false;

            if (!HayPlazasActividad(reserva.ActividadId, reserva.Id)) return false; // aforo al editar
            if (ExisteReservaDuplicada(reserva.SocioId, reserva.ActividadId, reserva.Fecha, reserva.Id)) return false; // duplicado al editar

            existente.SocioId = reserva.SocioId;
            existente.ActividadId = reserva.ActividadId;
            existente.Fecha = reserva.Fecha;
            _context.SaveChanges();
            return true;
        }

        // Elimina una reserva existente. Devuelve false si no se encontró.
        public bool EliminarReserva(Reserva reserva)
        {
            var existente = _context.Reserva.Find(reserva.Id);
            if (existente == null) return false;
            _context.Reserva.Remove(existente);
            _context.SaveChanges();
            return true;
        }
    }
}

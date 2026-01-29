using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

namespace Model
{
    public class Repositorio
    {
        // es el objeto que abre la conexión con la base de datos.
        private Model1 _context;
        private Model1 Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new Model1();
                }
                return _context;
            }
        }

        // Comprueba si el formato de un email es válido usando una expresión regular.
        public bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // Comprueba si una fecha de reserva es válida (no en el pasado).
        // Método estático para poder probar esta validación sin crear el contexto de EF.
        public static bool EsFechaReservaValida(System.DateTime fecha)
        {
            return fecha.Date >= System.DateTime.Today;
        }

        // Comprueba si hay plazas disponibles dado el aforo máximo y ocupados actuales.
        // Método estático para poder probarlo sin inicializar EF.
        public static bool HayPlazasDisponibles(int aforoMaximo, int ocupados)
        {
            return ocupados < aforoMaximo;
        }

        // Determina si se puede guardar una reserva basándose en aforo, duplicado y fecha.
        // Versión estática que no accede a la base de datos: recibe los valores necesarios como parámetros.
        public static bool PuedeGuardarReserva(int aforoMaximo, int ocupados, System.DateTime fecha, bool esDuplicado)
        {
            if (!EsFechaReservaValida(fecha)) return false;
            if (!HayPlazasDisponibles(aforoMaximo, ocupados)) return false;
            if (esDuplicado) return false;
            return true;
        }

        // Comprueba si aún quedan plazas en la actividad (aforo disponible).
        // Recorre todas las reservas para contar cuántas son de esa actividad
        // y opcionalmente excluye una reserva (cuando editamos) para no contarse a sí misma.
        public bool HayPlazasActividad(int actividadId, int? reservaIdExcluir = null)
        {
            var actividad = Context.Actividad.Find(actividadId);
            if (actividad == null) return false; // si no existe la actividad, devolvemos false

            var ocupados = 0;
            foreach (var r in Context.Reserva)
            {
                if (r.ActividadId == actividadId && (!reservaIdExcluir.HasValue || r.Id != reservaIdExcluir.Value))
                {
                    ocupados++;
                }
            }

            // Hay plazas libres cuando ocupados es menor que el aforo máximo.
            return ocupados < actividad.AforoMaximo;
        }

        // Comprueba si ya existe una reserva para el mismo socio, actividad y fecha
        // Recorremos todas las reservas comparando socio, actividad y solo la parte de fecha
        public bool ExisteReservaDuplicada(int socioId, int actividadId, System.DateTime fecha, int? reservaIdExcluir = null)
        {
            var start = fecha.Date;
            var end = start.AddDays(1);
            foreach (var r in _context.Reserva)
            {
                if ((!reservaIdExcluir.HasValue || r.Id != reservaIdExcluir.Value)
                    && r.SocioId == socioId
                    && r.ActividadId == actividadId
                    && r.Fecha >= start && r.Fecha < end)
                {
                    return true;
                }
            }
            return false;
        }

        // ACTIVIDADES
        // Devuelve todas las actividades de la base de datos en una lista.
        public List<Actividad> SeleccionarActividades()
        {
            return Context.Actividad.ToList();
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
            var existente = Context.Actividad.Find(actividad.Id);
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
            foreach (var r in Context.Reserva)
            {
                if (r.ActividadId == actividad.Id)
                {
                    tieneReservas = true; // hay reservas que la usan
                    break;
                }
            }
            if (tieneReservas) return false; // evitamos romper la relación

            Context.Actividad.Remove(existente);
            Context.SaveChanges();
            return true;
        }

        // SOCIOS
        // Devuelve todos los socios.
        public List<Socio> SeleccionarSocios()
        {
            return Context.Socio.ToList();
        }

        // Inserta un nuevo socio.
        public void GuardarSocio(Socio socio)
        {
            Context.Socio.Add(socio);
            Context.SaveChanges();
        }

        // Actualiza un socio existente.
        public void ActualizarSocio(Socio socio)
        {
            var existente = Context.Socio.Find(socio.Id);
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
            foreach (var r in Context.Reserva)
            {
                if (r.SocioId == socio.Id)
                {
                    tieneReservas = true; // no se puede borrar si tiene reservas
                    break;
                }
            }
            if (tieneReservas) return false;

            Context.Socio.Remove(existente);
            Context.SaveChanges();
            return true;
        }

        // RESERVAS
        // Devuelve las reservas e incluye los datos de socio y actividad para mostrarlos en la vista.
        public List<Reserva> SeleccionarReservas()
        {
                return Context.Reserva.
                                Include("Socio").
                                Include("Actividad").
                                ToList();

        }

        // Inserta una nueva reserva. Devuelve false si no hay aforo o si ya existe.
        public bool GuardarReserva(Reserva reserva)
        {
            // No se permiten reservas en fechas anteriores al día actual
            if (reserva.Fecha.Date < System.DateTime.Today) return false;
            if (!HayPlazasActividad(reserva.ActividadId)) return false; // aforo lleno
            if (ExisteReservaDuplicada(reserva.SocioId, reserva.ActividadId, reserva.Fecha)) return false; // ya reservó mismo día

            Context.Reserva.Add(reserva);
            Context.SaveChanges();
            return true;
        }

        // Actualiza una reserva existente. Devuelve false si falla alguna validación.
        public bool ActualizarReserva(Reserva reserva)
        {
            var existente = Context.Reserva.Find(reserva.Id);
            if (existente == null) return false;

            // No se permiten reservas en fechas anteriores al día actual
            if (reserva.Fecha.Date < System.DateTime.Today) return false;

            if (!HayPlazasActividad(reserva.ActividadId, reserva.Id)) return false; // aforo al editar
            if (ExisteReservaDuplicada(reserva.SocioId, reserva.ActividadId, reserva.Fecha, reserva.Id)) return false; // duplicado al editar

            existente.SocioId = reserva.SocioId;
            existente.ActividadId = reserva.ActividadId;
            existente.Fecha = reserva.Fecha;
            Context.SaveChanges();
            return true;
        }

        // Elimina una reserva existente. Devuelve false si no se encontró.
        public bool EliminarReserva(Reserva reserva)
        {
            var existente = Context.Reserva.Find(reserva.Id);
            if (existente == null) return false;
            Context.Reserva.Remove(existente);
            Context.SaveChanges();
            return true;
        }
    }
}

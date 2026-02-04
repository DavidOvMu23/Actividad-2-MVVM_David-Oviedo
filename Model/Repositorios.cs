using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

namespace Model
{
    /// <summary>
    /// Repositorio con acceso a datos y validaciones de actividades, socios y reservas.
    /// </summary>
    public class Repositorio
    {
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

        /// <summary>
        /// Comprueba si el email tiene un formato válido usando una expresión regular.
        /// </summary>
        /// <param name="email">Email a validar.</param>
        /// <returns><c>true</c> si el formato es válido; en caso contrario, <c>false</c>.</returns>
        public bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        /// <summary>
        /// Comprueba si una fecha de reserva es válida (no en el pasado).
        /// </summary>
        /// <param name="fecha">Fecha a validar.</param>
        /// <returns><c>true</c> si la fecha es válida; en caso contrario, <c>false</c>.</returns>
        public static bool EsFechaReservaValida(System.DateTime fecha)
        {
            return fecha.Date >= System.DateTime.Today;
        }

        /// <summary>
        /// Comprueba si quedan plazas según el aforo y los ocupados.
        /// </summary>
        /// <param name="aforoMaximo">Aforo máximo.</param>
        /// <param name="ocupados">Número de plazas ocupadas.</param>
        /// <returns><c>true</c> si hay plazas disponibles; en caso contrario, <c>false</c>.</returns>
        public static bool HayPlazasDisponibles(int aforoMaximo, int ocupados)
        {
            return ocupados < aforoMaximo;
        }

        /// <summary>
        /// Dice si se puede guardar una reserva según aforo, duplicados y fecha.
        /// </summary>
        /// <param name="aforoMaximo">Aforo máximo.</param>
        /// <param name="ocupados">Número de plazas ocupadas.</param>
        /// <param name="fecha">Fecha de la reserva.</param>
        /// <param name="esDuplicado">Indica si existe una reserva duplicada.</param>
        /// <returns><c>true</c> si se puede guardar; en caso contrario, <c>false</c>.</returns>
        public static bool PuedeGuardarReserva(int aforoMaximo, int ocupados, System.DateTime fecha, bool esDuplicado)
        {
            if (!EsFechaReservaValida(fecha)) return false;
            if (!HayPlazasDisponibles(aforoMaximo, ocupados)) return false;
            if (esDuplicado) return false;
            return true;
        }

        /// <summary>
        /// Comprueba si aún quedan plazas en la actividad.
        /// </summary>
        /// <param name="actividadId">Identificador de la actividad.</param>
        /// <param name="reservaIdExcluir">Reserva a excluir al contar ocupación.</param>
        /// <returns><c>true</c> si hay plazas disponibles; en caso contrario, <c>false</c>.</returns>
        public bool HayPlazasActividad(int actividadId, int? reservaIdExcluir = null)
        {
            var actividad = Context.Actividad.Find(actividadId);
            if (actividad == null) return false;

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

        /// <summary>
        /// Comprueba si ya hay una reserva para el mismo socio, actividad y fecha.
        /// </summary>
        /// <param name="socioId">Identificador del socio.</param>
        /// <param name="actividadId">Identificador de la actividad.</param>
        /// <param name="fecha">Fecha de la reserva.</param>
        /// <param name="reservaIdExcluir">Reserva a excluir en la comprobación.</param>
        /// <returns><c>true</c> si existe una reserva duplicada; en caso contrario, <c>false</c>.</returns>
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

        /// <summary>
        /// Devuelve todas las actividades de la base de datos.
        /// </summary>
        /// <returns>Lista de actividades.</returns>
        public List<Actividad> SeleccionarActividades()
        {
            return Context.Actividad.ToList();
        }

        /// <summary>
        /// Guarda una actividad nueva.
        /// </summary>
        /// <param name="actividad">Actividad a guardar.</param>
        public void GuardarActividad(Actividad actividad)
        {
            _context.Actividad.Add(actividad);
            _context.SaveChanges();
        }

        /// <summary>
        /// Actualiza los datos de una actividad existente.
        /// </summary>
        /// <param name="actividad">Actividad con los datos actualizados.</param>
        public void ActualizarActividad(Actividad actividad)
        {
            var existente = Context.Actividad.Find(actividad.Id);
            if (existente == null) return;
            existente.Nombre = actividad.Nombre;
            existente.AforoMaximo = actividad.AforoMaximo;
            _context.SaveChanges();
        }

        /// <summary>
        /// Intenta eliminar una actividad.
        /// </summary>
        /// <param name="actividad">Actividad a eliminar.</param>
        /// <returns><c>true</c> si se eliminó; en caso contrario, <c>false</c>.</returns>
        public bool EliminarActividad(Actividad actividad)
        {
            var existente = _context.Actividad.Find(actividad.Id);
            if (existente == null) return false;

            var tieneReservas = false;
            foreach (var r in Context.Reserva)
            {
                if (r.ActividadId == actividad.Id)
                {
                    tieneReservas = true;
                    break;
                }
            }
            if (tieneReservas) return false;

            Context.Actividad.Remove(existente);
            Context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Devuelve todos los socios.
        /// </summary>
        /// <returns>Lista de socios.</returns>
        public List<Socio> SeleccionarSocios()
        {
            return Context.Socio.ToList();
        }

        /// <summary>
        /// Guarda un socio nuevo.
        /// </summary>
        /// <param name="socio">Socio a guardar.</param>
        public void GuardarSocio(Socio socio)
        {
            Context.Socio.Add(socio);
            Context.SaveChanges();
        }

        /// <summary>
        /// Actualiza un socio existente.
        /// </summary>
        /// <param name="socio">Socio con los datos actualizados.</param>
        public void ActualizarSocio(Socio socio)
        {
            var existente = Context.Socio.Find(socio.Id);
            if (existente == null) return;
            existente.Nombre = socio.Nombre;
            existente.Email = socio.Email;
            existente.Activo = socio.Activo;
            _context.SaveChanges();
        }

        /// <summary>
        /// Intenta eliminar un socio.
        /// </summary>
        /// <param name="socio">Socio a eliminar.</param>
        /// <returns><c>true</c> si se eliminó; en caso contrario, <c>false</c>.</returns>
        public bool EliminarSocio(Socio socio)
        {
            var existente = _context.Socio.Find(socio.Id);
            if (existente == null) return false;

            var tieneReservas = false;
            foreach (var r in Context.Reserva)
            {
                if (r.SocioId == socio.Id)
                {
                    tieneReservas = true;
                    break;
                }
            }
            if (tieneReservas) return false;

            Context.Socio.Remove(existente);
            Context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Devuelve las reservas e incluye los datos de socio y actividad.
        /// </summary>
        /// <returns>Lista de reservas.</returns>
        public List<Reserva> SeleccionarReservas()
        {
                return Context.Reserva.
                                Include("Socio").
                                Include("Actividad").
                                ToList();

        }

        /// <summary>
        /// Guarda una reserva nueva.
        /// </summary>
        /// <param name="reserva">Reserva a guardar.</param>
        /// <returns><c>true</c> si se guardó; en caso contrario, <c>false</c>.</returns>
        public bool GuardarReserva(Reserva reserva)
        {
            if (reserva.Fecha.Date < System.DateTime.Today) return false;
            if (!HayPlazasActividad(reserva.ActividadId)) return false;
            if (ExisteReservaDuplicada(reserva.SocioId, reserva.ActividadId, reserva.Fecha)) return false;

            Context.Reserva.Add(reserva);
            Context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Actualiza una reserva existente.
        /// </summary>
        /// <param name="reserva">Reserva con los datos actualizados.</param>
        /// <returns><c>true</c> si se actualizó; en caso contrario, <c>false</c>.</returns>
        public bool ActualizarReserva(Reserva reserva)
        {
            var existente = Context.Reserva.Find(reserva.Id);
            if (existente == null) return false;

            if (reserva.Fecha.Date < System.DateTime.Today) return false;

            if (!HayPlazasActividad(reserva.ActividadId, reserva.Id)) return false;
            if (ExisteReservaDuplicada(reserva.SocioId, reserva.ActividadId, reserva.Fecha, reserva.Id)) return false;

            existente.SocioId = reserva.SocioId;
            existente.ActividadId = reserva.ActividadId;
            existente.Fecha = reserva.Fecha;
            Context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Elimina una reserva existente.
        /// </summary>
        /// <param name="reserva">Reserva a eliminar.</param>
        /// <returns><c>true</c> si se eliminó; en caso contrario, <c>false</c>.</returns>
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

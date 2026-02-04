namespace Model
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Reserva de una actividad hecha por un socio.
    /// </summary>
    public partial class Reserva
    {
        /// <summary>
        /// Id de la reserva.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id del socio que hace la reserva.
        /// </summary>
        public int SocioId { get; set; }
        /// <summary>
        /// Id de la actividad reservada.
        /// </summary>
        public int ActividadId { get; set; }
        /// <summary>
        /// Fecha de la reserva.
        /// </summary>
        public System.DateTime Fecha { get; set; }
    
        /// <summary>
        /// Actividad asociada a la reserva.
        /// </summary>
        public virtual Actividad Actividad { get; set; }
        /// <summary>
        /// Socio asociado a la reserva.
        /// </summary>
        public virtual Socio Socio { get; set; }
    }
}

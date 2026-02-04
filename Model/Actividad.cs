namespace Model
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Actividad disponible para reservas.
    /// </summary>
    public partial class Actividad
    {
        /// <summary>
        /// Crea una nueva actividad.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Actividad()
        {
            this.Reserva = new HashSet<Reserva>();
        }
    
        /// <summary>
        /// Id de la actividad.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre de la actividad.
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Aforo máximo permitido.
        /// </summary>
        public int AforoMaximo { get; set; }
    
        /// <summary>
        /// Reservas asociadas a la actividad.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reserva> Reserva { get; set; }
    }
}

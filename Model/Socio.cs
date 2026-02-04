namespace Model
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Socio del sistema.
    /// </summary>
    public partial class Socio
    {
        /// <summary>
        /// Crea un socio nuevo.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Socio()
        {
            this.Reserva = new HashSet<Reserva>();
        }
    
        /// <summary>
        /// Id del socio.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre del socio.
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Email del socio.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Indica si el socio está activo.
        /// </summary>
        public bool Activo { get; set; }
    
        /// <summary>
        /// Reservas asociadas al socio.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reserva> Reserva { get; set; }
    }
}

using System.Data.Entity;

namespace Model
{
    /// <summary>
    /// Contexto de Entity Framework de la base de datos.
    /// </summary>
    public class Model1 : DbContext
    {
        /// <summary>
        /// Conjunto de actividades.
        /// </summary>
        public DbSet<Actividad> Actividad { get; set; }
        /// <summary>
        /// Conjunto de socios.
        /// </summary>
        public DbSet<Socio> Socio { get; set; }
        /// <summary>
        /// Conjunto de reservas.
        /// </summary>
        public DbSet<Reserva> Reserva { get; set; }
    }
}


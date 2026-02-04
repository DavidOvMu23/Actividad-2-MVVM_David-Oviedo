namespace Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    /// <summary>
    /// Contexto de Entity Framework del centro deportivo.
    /// </summary>
    public partial class CentroDeportivoEntities : DbContext
    {
        /// <summary>
        /// Inicializa una nueva instancia del contexto.
        /// </summary>
        public CentroDeportivoEntities()
            : base("name=CentroDeportivoEntities")
        {
        }
    
        /// <summary>
        /// Configura el modelo de Entity Framework.
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        /// <summary>
        /// Conjunto de actividades.
        /// </summary>
        public virtual DbSet<Actividad> Actividad { get; set; }
        /// <summary>
        /// Conjunto de reservas.
        /// </summary>
        public virtual DbSet<Reserva> Reserva { get; set; }
        /// <summary>
        /// Conjunto de socios.
        /// </summary>
        public virtual DbSet<Socio> Socio { get; set; }
    }
}

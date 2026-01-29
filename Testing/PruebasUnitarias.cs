namespace Testing
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;

    [TestClass]
    public sealed class PruebasUnitarias
    {
        [TestMethod]
        public void ValidacionEmail()
        {
            var repo = new Repositorio();
            var socio = new Socio();

            // Caso válido
            socio.Email = "usuario@dominio.com";
            Assert.IsTrue(repo.EsEmailValido(socio.Email));

            // Caso inválido (sin @)
            socio.Email = "usuario.com";
            Assert.IsFalse(repo.EsEmailValido(socio.Email));
        }

        [TestMethod]
        public void ValidacionFechaReserva()
        {
            // Probamos la validación de fechas sin inicializar EF
            var fechaAyer = System.DateTime.Today.AddDays(-1);
            Assert.IsFalse(Repositorio.EsFechaReservaValida(fechaAyer));

            var fechaHoy = System.DateTime.Today;
            Assert.IsTrue(Repositorio.EsFechaReservaValida(fechaHoy));
        }

        [TestMethod]
        public void ValidacionAforoMaximo()
        {
            // Probamos la lógica sin acceder a la base de datos usando Validaciones.PuedeGuardarReserva.
            var aforo = 2;
            var ocupadosAntes = 0;
            var fechaValida = System.DateTime.Today;
            var fechaPasada = System.DateTime.Today.AddDays(-1);

            // No está lleno, no duplicado y fecha válida -> se puede guardar
            Assert.IsTrue(Repositorio.PuedeGuardarReserva(aforo, ocupadosAntes, fechaValida, esDuplicado: false));

            // Aforo lleno -> no se puede guardar
            Assert.IsFalse(Repositorio.PuedeGuardarReserva(aforo, aforo, fechaValida, esDuplicado: false));

            // Duplicado -> no se puede guardar
            Assert.IsFalse(Repositorio.PuedeGuardarReserva(aforo, ocupadosAntes, fechaValida, esDuplicado: true));

            // Fecha en el pasado -> no se puede guardar
            Assert.IsFalse(Repositorio.PuedeGuardarReserva(aforo, ocupadosAntes, fechaPasada, esDuplicado: false));
        }
    }
}

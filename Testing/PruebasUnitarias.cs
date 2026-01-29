namespace Testing
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using System;
    using System.Collections.Generic;

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

            // Caso inválido
            socio.Email = "usuario.com";
            Assert.IsFalse(repo.EsEmailValido(socio.Email));
        }

        [TestMethod]
        public void ValidacionFechaReserva()
        {
            // Creamos una reserva con fecha en el pasado y comprobamos
            // que el intento de guardado es rechazado por la validación
            var repo = new Repositorio();
            var reservaPasada = new Reserva
            {
                SocioId = 1,
                ActividadId = 1,
                Fecha = DateTime.Today.AddDays(-1)
            };

            // GuardarReserva devuelve false cuando la fecha es anterior a hoy
            Assert.IsFalse(repo.GuardarReserva(reservaPasada));

            // Para completar la verificación, la función estática también debe
            // indicar que la fecha no es válida.
            Assert.IsFalse(Repositorio.EsFechaReservaValida(reservaPasada.Fecha));
        }

        [TestMethod]
        public void ValidacionAforoMaximo()
        {
            // Simulamos una Actividad con aforo 1 y reservamos dos veces.
            var actividad = new Actividad { Id = 10, Nombre = "Clase", AforoMaximo = 1 };

            // Lista en memoria que simula las reservas existentes para la actividad
            var reservas = new List<Reserva>();

            // Primera reserva. si no hay ocupados debe permitirse
            var reserva1 = new Reserva { Id = 1, SocioId = 1, ActividadId = actividad.Id, Fecha = DateTime.Today };
            int ocupadosAntes = 0;
            foreach (var r in reservas)
            {
                if (r.ActividadId == actividad.Id)
                {
                    ocupadosAntes++;
                }
            }
            Assert.IsTrue(Repositorio.HayPlazasDisponibles(actividad.AforoMaximo, ocupadosAntes));

            // Añadimos la primera reserva (simulando que se guardó)
            reservas.Add(reserva1);

            // Segunda reserva. ahora ocupados no debe permitirse
            var reserva2 = new Reserva { Id = 2, SocioId = 2, ActividadId = actividad.Id, Fecha = DateTime.Today };
            int ocupadosDespues = 0;
            foreach (var r in reservas)
            {
                if (r.ActividadId == actividad.Id)
                {
                    ocupadosDespues++;
                }
            }
            Assert.IsFalse(Repositorio.HayPlazasDisponibles(actividad.AforoMaximo, ocupadosDespues));
        }
    }
}

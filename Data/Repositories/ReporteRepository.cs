using Scm.Domain;
using Scm.Data.Repositories;
using System;

namespace Scm.Data{
    public class ReporteRepository : BaseRepository<ReporteMovimientos>
    {
        public ReporteRepository(ScmContext context) : base(context)
        {
        }

        // Aqui irian metodos adicionales para funciones extra .


    }
}
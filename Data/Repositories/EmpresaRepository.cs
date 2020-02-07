using Scm.Data.Repositories;
using Scm.Domain;
namespace Scm.Data
{
    public class EmpresaRepository : BaseRepository<Empresa>
    {
        public EmpresaRepository(ScmContext context) : base(context)
        {
        }
    }
}
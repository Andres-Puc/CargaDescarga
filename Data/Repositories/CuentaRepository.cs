using Scm.Domain;
using Scm.Data.Repositories;
using System;
using System.Collections.Generic;
using Scm.Controllers.Dtos;

namespace Scm.Data{
    public class CuentaRepository : BaseRepository<AppUser>
    {
        public CuentaRepository(ScmContext context) : base(context)
        {
            
        }

 internal object GetByID()
        {
            throw new NotImplementedException();
        }
        
    }
}
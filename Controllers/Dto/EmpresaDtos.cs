using System.ComponentModel.DataAnnotations;

namespace Scm.Controllers.Dtos
{
    public class EmpresaDtos
    {
        [Required]
        public string  Nombre { get; set; }
    }

    public class EmpresaResponseDto
    {
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; }
    }
}
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Scm.Controllers.Dtos;
using Scm.Domain;
using Scm.Infrastructure.Authentication;
using Scm.Infrastructure.ManagedResponses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Scm.Data;
using System.Collections.Generic;
using System;

namespace Scm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {   
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;
        private JwtSettings _jwtSettings;

        private CuentaRepository _cuentaRepository;
        private IMapper _mapper;

        private ScmContext _context;
          public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, IConfiguration configuration, JwtSettings jwtSettings, RoleManager<AppRole> roleManager, IMapper mapper,CuentaRepository cuentaRepository, ScmContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwtSettings = jwtSettings;
            _roleManager = roleManager;
            _mapper = mapper;
            _cuentaRepository = cuentaRepository;
            _context = context;
        }
        /// <summary>
        /// It create a new user on database
        /// </summary>
        /// <param name="model">New user model</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegisterUserRequestDto model) {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ManagedErrorResponse(ManagedErrorCode.Validation, "Hay errores de validación", ModelState));
            }

            var user = new AppUser { UserName = model.Email.Trim(), Email = model.Email.Trim()};
            var result = await _userManager.CreateAsync(user, model.Password.Trim());
            if (result.Succeeded)
            {
                
                return Ok(_mapper.Map<RegisterUserResponseDto>(user));
            }
            else
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return BadRequest(new ManagedErrorResponse(ManagedErrorCode.Validation, "Identity validation errors", errors));
            }
        }

        [HttpGet("Todos")]
        public IActionResult Get(){
            var Usuarios = _cuentaRepository.GetAll();
            var UsuariosResult = _mapper.Map<List<RegisterUserResponseDto>>(Usuarios);
            return Ok(UsuariosResult);
        }

        [HttpDelete("Eliminar")]
        public string Eliminar(string UserId){
                try{
                    _cuentaRepository.Delete(UserId);
                    _context.SaveChanges(); 

                }catch(Exception e){
                    Console.WriteLine(e);
                    return e.ToString();
                    
                }
            return "Se ha eliminado correctamente";
        }

        [HttpPut("Modificar")]
        public IActionResult Modificar(int idEmpleado, [FromBody] EmpleadoUpdateDto model){
             
        var Emp = _mapper.Map<Empleado>(model);
        _EmpleadoRepository.Update(Emp);
        _context.SaveChanges();
            var Dto = _mapper.Map<EmpleadoResponseDto>(Emp);
            
            return Ok(Dto);
        }

        /// <summary>
        /// Login method for get a token
        /// </summary>
        /// <param name="model">username/password dto</param>
        /// <returns></returns>
        [HttpPost("token")]
        [ProducesResponseType(200, Type = typeof(AuthenticatedUser))]
        [ProducesResponseType(400, Type = typeof(ManagedErrorResponse))]
        public async Task<IActionResult> Login(LoginDto model){
            SecurityManager mgr = new SecurityManager(_jwtSettings, _userManager, _roleManager);
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if (result.Succeeded)
            {   
                var appUser = _userManager.Users.SingleOrDefault(r => r.UserName.Trim() == model.UserName.Trim());
                var authUser = await mgr.BuildAuthenticatedUserObject(appUser);
                return Ok(authUser);
            }
            else
            {
                return BadRequest(new ManagedErrorResponse(ManagedErrorCode.Validation,"La combinación usuario/password no es correcta"));
            }  
        }
    }
}
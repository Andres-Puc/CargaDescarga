using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using CargaDescarga;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scm.Controllers.Dtos;
using Scm.Service;

namespace Scm.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    [ProducesResponseType(401, Type = typeof(string))]
    public class FacturaController : ControllerBase
    {
        private readonly ValeService _valeService;

        private IMapper _mapper;

        
        public FacturaController(ValeService ValeService, IMapper mapper)
        {
            _valeService = ValeService;
            _mapper = mapper;
        }

        private string CurrentUserId(ClaimsPrincipal claims){
                return claims.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet("{folio}")]        
        public IActionResult getById(string folio){
                
                var valeResult = _valeService.getByFolio(folio);
                if (valeResult.isSuccess){
                    var result = _mapper.Map<ValeDto>(valeResult.Result);
                    return Ok(result);
                }else{
                    return BadRequest(valeResult.Errors);
                }               
        }

        [HttpGet("filter/{bussinesName}")]        
        public IActionResult filterByBusinessName(string bussinesName){
                
                var valesResult = _valeService.getByBusinessName(bussinesName);
                if (valesResult.isSuccess){
                    var result = _mapper.Map<ValeDto>(valesResult.Result);
                    return Ok(result);
                }else{
                        return BadRequest(valesResult.Errors);
                }               
        }
        [HttpPost("filter/date")]        
        public IActionResult filterByDate(DateTime date){
                
                var valesResult = _valeService.getBetweenDate(date);
                if (valesResult.isSuccess){
                    var result = _mapper.Map<ValeDto>(valesResult.Result);
                    return Ok(result);
                }else{
                        return BadRequest(valesResult.Errors);
                }               
        }

        
    }
}
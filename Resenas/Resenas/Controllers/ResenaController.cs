using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Resenas.Middleware.Auth;
using Resenas.Model;
using Resenas.Model.Interfaces;
using Resenas.Model.Repositories;
using Resenas.Security.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace Resenas.Controllers
{
    [ApiController]
    [Route("resena")]
    public class ResenaController : ControllerBase
    {
        private readonly VerificarToken _verificarToken;
        private readonly IResenaRepository _resenaRepository;

        public ResenaController(IResenaRepository resenaRepository, VerificarToken verificarToken)
        {
            _resenaRepository = resenaRepository;
            _verificarToken = verificarToken;
        }

        private Resena MapDataToResena(JsonElement optData, User usuario)
        {
            return new Resena
            {
                orderID = optData.GetProperty("orderID").GetInt32(),
                userID = usuario.Id,
                articleId = optData.GetProperty("articleID").GetInt32(),
                valoration = optData.GetProperty("valoration").GetInt32(),
                content = optData.GetProperty("content").GetString(),
                created = DateTime.Now,
                updated = DateTime.Now,
                imageUrl = "notengo"
            };
        }

        private Resena MapDataToResenaDtos(ResenaRequestDto optData, User usuario)
        {
            return new Resena
            {
                orderID = optData.orderID,
                userID = usuario.Id,
                articleId = optData.articleID,
                valoration = optData.valoration,
                content = optData.content,
                created = DateTime.Now,
                updated = DateTime.Now,
                imageUrl = "notengo"
            };
        }

        private Resena MapDataToResenaConID(JsonElement optData, User usuario)
        {
            return new Resena
            {
                orderID = optData.GetProperty("orderID").GetInt32(),
                userID = usuario.Id,
                idMongo = ObjectId.Parse(optData.GetProperty("id").GetString()),
                articleId = optData.GetProperty("articleID").GetInt32(),
                valoration = optData.GetProperty("valoration").GetInt32(),
                content = optData.GetProperty("content").GetString(),
                created = DateTime.Now,
                updated = DateTime.Now,
                imageUrl = "notengo"
            };
        }

        [NonAction]
        public async Task<User> VerificarLogueo(string token)
        {
            return await _verificarToken.obtenerUsuario(token);
        }

        [HttpGet]
        [Route("obtenerResena")]
        [SwaggerOperation(
            Summary = "Obtiene una reseña por su ID",
            Description = "Obtiene una reseña basada en el ID proporcionado.",
            OperationId = "ObtenerResena"
        )]
        [SwaggerResponse(200, "La reseña se obtiene correctamente", typeof(ResenaDto))]
        [SwaggerResponse(400, "Solicitud inválida")]
        public async Task<IActionResult> ObtenerResena([FromQuery, Required(ErrorMessage = "El ID de la reseña es requerido")] string idResena)
        {
            var resena = _resenaRepository.GetResenaByID(ObjectId.Parse(idResena));
            if (resena == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "No se pudo encontrar la reseña",
                    result = ""
                });
            }
            var resenaDto = ResenaDto.MapToResenaDto(resena);
            return Ok(new
            {
                success = true,
                message = "Reseña encontrada",
                result = resenaDto
            });
        }

        [HttpGet]
        [Route("ObtenerResenasPorArticulo")]
        [SwaggerOperation(
            Summary = "Obtiene una lista de reseñas por su ID de artículo",
            Description = "Obtiene una lista de reseñas por su ID de artículo proporcionado.",
            OperationId = "ObtenerResenasPorArticulo"
        )]
        [SwaggerResponse(200, "La lista de reseñas se obtiene correctamente", typeof(ResenaDto))]
        [SwaggerResponse(400, "Solicitud inválida")]
        public async Task<IActionResult> ObtenerResenaDelArticulo([Required(ErrorMessage = "El ID del producto es requerido")] int idProducto)
        {
            var resenas = _resenaRepository.GetResenaByArticle(idProducto);
            if (resenas == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "No se pudieron encontrar reseñas para este producto",
                    result = ""
                });
            }
            var resenaDtoLista = resenas.Select(ResenaDto.MapToResenaDto).ToList();
            return Ok(resenaDtoLista);
        }

        [HttpGet]
        [Route("ObtenerResenasPorUsuario")]
        [SwaggerOperation(
            Summary = "Obtiene una lista de reseñas por su ID de usuario",
            Description = "Obtiene una lista de reseñas por su ID de usuario proporcionado.",
            OperationId = "ObtenerResenasPorUsuario"
        )]
        [SwaggerResponse(200, "La lista de reseñas se obtiene correctamente", typeof(ResenaDto))]
        [SwaggerResponse(400, "Solicitud inválida")]
        public async Task<IActionResult> ObtenerResenaPorUsuario([Required(ErrorMessage = "El ID del usuario es requerido")] string idUser)
        {
            var resenas = _resenaRepository.GetResenaByUser(idUser);
            if (resenas == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "No se pudieron encontrar reseñas para este usuario",
                    result = ""
                });
            }
            var resenaDtoLista = resenas.Select(ResenaDto.MapToResenaDto).ToList();
            return Ok(new
            {
                success = true,
                message = "Reseñas encontradas",
                result = resenaDtoLista
            });
        }

        [HttpPost]
        [Route("guardarResena")]
        [SwaggerOperation(
            Summary = "Guardar una nueva reseña",
            Description = "Guarda una nueva reseña en la base de datos.",
            OperationId = "GuardarResena"
        )]
        [SwaggerResponse(200, "Reseña guardada correctamente", typeof(ResenaDto))]
        [SwaggerResponse(400, "Solicitud inválida")]
        [SwaggerRequestExample(typeof(ResenaDto), typeof(ResenaExample))]
        public async Task<IActionResult> GuardarResena([FromBody] ResenaRequestDto resenaDto, [FromHeader(Name = "Authorization")] string authorization)
        {
            var token = authorization.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase) ? authorization.Substring(7) : authorization;
            var usuario = await VerificarLogueo(token);
            if (usuario == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al autenticarse"
                });
            }
            try
            {
                var resena = MapDataToResenaDtos(resenaDto, usuario);
                resena = _resenaRepository.InsertResena(resena);
                return Ok(new
                {
                    success = true,
                    message = "Reseña insertada correctamente",
                    result = ResenaDto.MapToResenaDto(resena)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al insertar la reseña",
                    error = ex.Message
                });
            }
        }

        [HttpPut]
        [Route("modificarResena")]
        [SwaggerOperation(
            Summary = "Modificar una reseña",
            Description = "Modificar una nueva reseña en la base de datos.",
            OperationId = "modificarResena"
        )]
        [SwaggerResponse(200, "Reseña modificada correctamente", typeof(ResenaDto))]
        [SwaggerResponse(400, "Solicitud inválida")]
        [SwaggerRequestExample(typeof(ResenaDto), typeof(ResenaExample))]
        public async Task<IActionResult> ModificarResena([FromBody] ResenaRequestDto resenaDto, [FromHeader(Name = "Authorization")] string authorization)
        {
            var token = authorization.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase) ? authorization.Substring(7) : authorization;
            var usuario = await VerificarLogueo(token);
            if (usuario == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al autenticarse"
                });
            }
            try
            {
                var resena = MapDataToResenaDtos(resenaDto, usuario);
                _resenaRepository.ModificarResena(resena);
                return Ok(new
                {
                    success = true,
                    message = "Reseña modificada correctamente",
                    result = ResenaDto.MapToResenaDto(resena)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al modificar la reseña",
                    error = ex.Message
                });
            }
        }

        [HttpDelete]
        [Route("eliminarResena")]
        public async Task<IActionResult> EliminarResena([FromQuery] string id, [FromHeader(Name = "Authorization")] string authorization)
        {
            var token = authorization.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase) ? authorization.Substring(7) : authorization;
            var usuario = await VerificarLogueo(token);
            if (usuario == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al autenticarse"
                });
            }
            try
            {
                var resena = _resenaRepository.GetResenaByID(ObjectId.Parse(id));
                if (resena == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Reseña no encontrada"
                    });
                }
                _resenaRepository.EliminarResena(ObjectId.Parse(id));
                return Ok(new
                {
                    success = true,
                    message = "Reseña eliminada correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al eliminar la reseña",
                    error = ex.Message
                });
            }
        }
    }
}

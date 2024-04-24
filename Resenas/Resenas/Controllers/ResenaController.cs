using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Resenas.Middleware.Auth;
using Resenas.Model;
using Resenas.Model.Interfaces;
using Resenas.Model.Repositories;
using Resenas.Security.Tokens;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Filters;

namespace Resenas.Controllers
{
    [ApiController]
    [Route("resena")]
    public class ResenaController : ControllerBase
    {
        //private ResenaRepository resenaRepository;
        private readonly VerificarToken _verificarToken;
        private readonly IResenaRepository _resenaRepository;

        public  ResenaController(IResenaRepository resenaRepository, VerificarToken verificarToken) {
            _resenaRepository = resenaRepository;
            _verificarToken = verificarToken;
        }
        

        private Resena MapDataToResena(JsonElement optData,User usuario)
        {
            Resena resena = new Resena();
            resena.orderID = optData.GetProperty("orderID").GetInt32();
            resena.userID = MongoDB.Bson.ObjectId.Parse(usuario.Id); // Esto se reemplazaría con el servicio de autenticación
            //resena.idMongo = ObjectId.Parse(optData.GetProperty("id").GetString());
            resena.articleId = optData.GetProperty("articleID").GetInt32();
            resena.valoration = optData.GetProperty("valoration").GetInt32();
            resena.content = optData.GetProperty("content").GetString();
            resena.created = DateTime.Now;
            resena.updated = DateTime.Now;
            resena.imageUrl = "notengo";

            return resena;
        }

        private Resena MapDataToResenaConID(JsonElement optData, User usuario)
        {
            Resena resena = new Resena();
            resena.orderID = optData.GetProperty("orderID").GetInt32();
            resena.userID = MongoDB.Bson.ObjectId.Parse(usuario.Id); // Esto se reemplazaría con el servicio de autenticación
            resena.idMongo = ObjectId.Parse(optData.GetProperty("id").GetString());
            resena.articleId = optData.GetProperty("articleID").GetInt32();
            resena.valoration = optData.GetProperty("valoration").GetInt32();
            resena.content = optData.GetProperty("content").GetString();
            resena.created = DateTime.Now;
            resena.updated = DateTime.Now;
            resena.imageUrl = "notengo";

            return resena;
        }

        private async Task<User> VerificarLogueo(JsonElement optData) {
            string tokenUsuario = optData.GetProperty("userID").GetString();
            if (tokenUsuario == null)
            {
                return null;
            }
            return await _verificarToken.obtenerUsuario(tokenUsuario);            
        }





        //[HttpGet]
        //[Route("obtenerResena")]
        //public async Task<dynamic> obtenerResena() {

        //    string idResena = HttpContext.Request.Query["idResena"].ToString();           

        //    if (string.IsNullOrEmpty(idResena))
        //    {
        //        return new
        //        {
        //            success = false,
        //            message = "Falta el ID del articulo",
        //            result = ""
        //        };
        //    }

        //    if (!ObjectId.TryParse(idResena, out ObjectId objectId))
        //    {
        //        return new
        //        {
        //            success = false,
        //            message = "Formato del ID de reseña inválido",
        //            result = ""
        //        };
        //    }

        //    var resena = _resenaRepository.GetResenaByID(objectId);

        //    if (resena == null)
        //    {
        //        return new
        //        {
        //            success = false,
        //            message = "No se pudo encontrar la reseña",
        //            result = ""
        //        };
        //    }

        //    return new
        //    {
        //        success = true,
        //        message = "Reseña encontrada",
        //        result = resena
        //    };
        //}

        [HttpGet]
        [Route("obtenerResena")]
        [SwaggerOperation(
            Summary = "Obtiene una reseña por su ID",
            Description = "Obtiene una reseña basada en el ID proporcionado.",
            OperationId = "ObtenerResena"
        )]
        [SwaggerResponse(200, "La reseña se obtiene correctamente", typeof(Resena))]
        [SwaggerResponse(400, "Solicitud inválida")]
        public async Task<IActionResult> obtenerResena(
            [Required(ErrorMessage = "El ID de la reseña es requerido")] string idResena)
        {
            if (!ObjectId.TryParse(idResena, out ObjectId objectId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Formato del ID de reseña inválido",
                    result = ""
                });
            }

            var resena = _resenaRepository.GetResenaByID(objectId);

            if (resena == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "No se pudo encontrar la reseña",
                    result = ""
                });
            }

            return Ok(new
            {
                success = true,
                message = "Reseña encontrada",
                result = resena
            });
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Obtiene una lista de reseñas por su ID de articulo",
            Description = "Obtiene una lista de reseñas por su ID de articulo proporcionado.",
            OperationId = "ObtenerResenasPorArticulo"
        )]
        [SwaggerResponse(200, "La lista de reseñas se obtiene correctamente", typeof(Resena))]
        [SwaggerResponse(400, "Solicitud inválida")]
        [Route("ObtenerResenasPorArticulo")]
        public async Task<dynamic> obtenerResenaDelArticulo(
              [Required(ErrorMessage = "El ID de la reseña es requerido")] int idProducto)
        {


            idProducto = int.Parse(HttpContext.Request.Query["idProducto"].ToString());
            if (idProducto == null)
            {
                return new
                {
                    success = false,
                    message = "Falta el ID del producto",
                    result = ""
                };
            }

       
            return _resenaRepository.GetResenaByArticle(idProducto);
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Obtiene una lista de reseñas por su ID de usuario",
            Description = "Obtiene una lista de reseñas por su ID de usuario proporcionado.",
            OperationId = "ObtenerResenasPorUsuario"
        )]
        [SwaggerResponse(200, "La lista de reseñas se obtiene correctamente", typeof(Resena))]
        [SwaggerResponse(400, "Solicitud inválida")]
        [Route("ObtenerResenasPorUsuario")]
        public dynamic obtenerResenaPorUsuario()
        {

            //var data = JsonConvert.DeserializeObject<dynamic>(optData.ToString());


            ObjectId idUser = ObjectId.Parse(HttpContext.Request.Query["idUser"]);
            if (idUser == null)
            {
                return new
                {
                    success = false,
                    message = "Falta el ID del usuario",
                    result = ""
                };
            }       
            return _resenaRepository.GetResenaByUser(idUser);
        }

        [HttpPost]
        [Route("guardarResena")]
        [SwaggerOperation(
       Summary = "Guardar una nueva reseña",
       Description = "Guarda una nueva reseña en la base de datos.",
       OperationId = "GuardarResena"
   )]
        [SwaggerResponse(200, "Reseña guardada correctamente", typeof(Resena))]
        [SwaggerResponse(400, "Solicitud inválida")]
        [SwaggerRequestExample(typeof(Resena), typeof(ResenaExample))]
        public async Task<IActionResult> GuardarResena([FromBody] JsonElement optData)
        {
            User usuario = await VerificarLogueo(optData);
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
                Resena resena = MapDataToResena(optData, usuario);

                _resenaRepository.InsertResena(resena);
                return Ok(new
                {
                    success = true,
                    message = "Pudiste insertar bien",
                    result = resena
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
        public async Task<dynamic> modificarResena([FromBody] JsonElement optData) {

            User usuario = await VerificarLogueo(optData);
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
                //ObjectId idResena = ObjectId.Parse(HttpContext.Request.Query["idResena"]);
                Resena resena = MapDataToResenaConID(optData, usuario);
                _resenaRepository.ModificarResena(resena);
                return Ok(new
                {
                    success = true,
                    message = "La reseña se modifico correctamente",
                    result = resena
                });
            }catch (Exception ex) {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al insertar la reseña",
                    error = ex.Message
                });
            }
        }


        [HttpDelete]
        [Route("eliminarResena")]

        public async Task<dynamic> eliminarResena([FromBody] JsonElement optData)
        {
            User usuario = await VerificarLogueo(optData);
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
                Resena resena = new Resena();
                resena.idMongo = ObjectId.Parse(optData.GetProperty("id").GetString());
                bool resultado = _resenaRepository.EliminarResena(resena.idMongo);
                if (resultado == true)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Pudo eliminar la reseña"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Error al eliminar la reseña"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al puntuar la reseña",
                    error = ex.Message
                });
            }
        }


        [HttpGet]
        [Route("pruebaAuth")]
        public async Task<User> pruebaAuth()
        {
            string tokenUsuario = HttpContext.Request.Query["id"];
            //VerificarToken verif = new VerificarToken("http://localhost:3000");
            return await _verificarToken.obtenerUsuario(tokenUsuario);
        }

        [HttpGet]
        [Route("testRedis")]
        public dynamic testRedis()
        {
            return Tokens.hola();
        }



        public class ResenaExample : IExamplesProvider<Resena>
        {
            public Resena GetExamples()
            {
                return new Resena
                {
                    orderID = 1,
                    valoration = 5,
                    content = "Ejemplo de contenido de reseña."
                };
            }
        }


    }


}

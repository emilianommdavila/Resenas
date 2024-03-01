using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resenas.Middleware.Auth;
using Resenas.Model;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace Resenas.Controllers
{
    [ApiController]
    [Route("resena")]
    public class ResenaController : ControllerBase
    {
        private ResenaRepository resenaRepository;
        public  ResenaController() {
            var mongoDbSettings = new MongoDbSettings
            {
                ConnectionString = "mongodb://localhost:27017/",
                DatabaseName = "resenas"                
            };
            this.resenaRepository = new ResenaRepository(mongoDbSettings);
        }
        

        private Resena MapDataToResena(JsonElement optData)
        {
            Resena resena = new Resena();
            resena.orderID = optData.GetProperty("orderID").GetInt32();
            resena.userID = optData.GetProperty("userID").GetInt32(); // Esto se reemplazaría con el servicio de autenticación
            //resena.idMongo = ObjectId.Parse(optData.GetProperty("id").GetString());
            resena.articleId = optData.GetProperty("articleID").GetInt32();
            resena.valoration = optData.GetProperty("valoration").GetInt32();
            resena.content = optData.GetProperty("content").GetString();
            resena.created = DateTime.Now;
            resena.updated = DateTime.Now;
            resena.imageUrl = "notengo";

            return resena;
        }

        private Resena MapDataToResenaConID(JsonElement optData)
        {
            Resena resena = new Resena();
            resena.orderID = optData.GetProperty("orderID").GetInt32();
            resena.userID = optData.GetProperty("userID").GetInt32(); // Esto se reemplazaría con el servicio de autenticación
            resena.idMongo = ObjectId.Parse(optData.GetProperty("id").GetString());
            resena.articleId = optData.GetProperty("articleID").GetInt32();
            resena.valoration = optData.GetProperty("valoration").GetInt32();
            resena.content = optData.GetProperty("content").GetString();
            resena.created = DateTime.Now;
            resena.updated = DateTime.Now;
            resena.imageUrl = "notengo";

            return resena;
        }


        [HttpGet]
        [Route("obtenerResena")]
        public dynamic obtenerResena() {

            string idResena = HttpContext.Request.Query["idResena"].ToString();
            if (string.IsNullOrEmpty(idResena))
            {
                return new
                {
                    success = false,
                    message = "Falta el ID del articulo",
                    result = ""
                };
            }

            if (!ObjectId.TryParse(idResena, out ObjectId objectId))
            {
                return new
                {
                    success = false,
                    message = "Formato del ID de reseña inválido",
                    result = ""
                };
            }

            var resena = resenaRepository.GetResenaByID(objectId);

            if (resena == null)
            {
                return new
                {
                    success = false,
                    message = "No se pudo encontrar la reseña",
                    result = ""
                };
            }

            return new
            {
                success = true,
                message = "Reseña encontrada",
                result = resena
            };
        }


        //[HttpGet]
        //[Route("obtenerResenaPuntajeResena")]
        //public dynamic obtenerPuntajeResena()
        //{

        //    string idResena = HttpContext.Request.Query["idReseña"].ToString();
        //    if (string.IsNullOrEmpty(idResena))
        //    {
        //        return new
        //        {
        //            success = false,
        //            message = "Falta el ID de la resena",
        //            result = ""
        //        };
        //    }

        //    if (!ObjectId.TryParse(idResena, out ObjectId objectId))
        //    {
        //        return new
        //        {
        //            success = false,
        //            message = "ID de reseña inválido",
        //            result = ""
        //        };
        //    }
  
        //    var resena = resenaRepository.GetResenaByID(objectId);

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
        [Route("obtenerResenasDeArticulo")]
        public dynamic obtenerResenaDelArticulo()
        {

            //var data = JsonConvert.DeserializeObject<dynamic>(optData.ToString());


            int idProducto = int.Parse(HttpContext.Request.Query["idProducto"].ToString());
            if (idProducto == null)
            {
                return new
                {
                    success = false,
                    message = "Falta el ID del producto",
                    result = ""
                };
            }

       
            return resenaRepository.GetResenaByArticle(idProducto);
        }

        [HttpGet]
        [Route("obtenerResenasDeUsuario")]
        public dynamic obtenerResenaPorUsuario()
        {

            //var data = JsonConvert.DeserializeObject<dynamic>(optData.ToString());


            int idUser = int.Parse(HttpContext.Request.Query["idUser"]);
            if (idUser == null)
            {
                return new
                {
                    success = false,
                    message = "Falta el ID del usuario",
                    result = ""
                };
            }       
            return resenaRepository.GetResenaByUser(idUser);
        }

        [HttpPost]
        [Route("guardarResena")]
        public IActionResult GuardarResena([FromBody] JsonElement optData)
        {
            try
            {
                Resena resena = MapDataToResena(optData);
       
                resenaRepository.InsertResena(resena);
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
        public dynamic modificarResena([FromBody] JsonElement optData) {

            try
            {
                //ObjectId idResena = ObjectId.Parse(HttpContext.Request.Query["idResena"]);
                Resena resena = MapDataToResenaConID(optData);
                resenaRepository.ModificarResena(resena);
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

        //[HttpPut]
        //[Route("puntuarResena")]
        //public dynamic puntuarResena([FromBody] JsonElement optData)
        //{
        //    //deberia recibir el id de la reseña y el puntaje de la misma
        //    try
        //    {
        //        Resena resena = MapDataToResena(optData);
        //        resenaRepository.ModificarResena(resena);
        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Pudo puntuar la reseña",
        //            result = resena
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            success = false,
        //            message = "Error al puntuar la reseña",
        //            error = ex.Message
        //        });
        //    }
        //}

        [HttpDelete]
        [Route("eliminarResena")]
        public dynamic eliminarResena()
        {
            try
            {
               
                ObjectId idResena = ObjectId.Parse(HttpContext.Request.Query["id"]);               
                bool resultado = resenaRepository.EliminarResena(idResena);
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
        public dynamic pruebaAuth()
        {
            VerificarToken verif = new VerificarToken("http://localhost:3000");
            return verif.obtenerUsuario("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0b2tlbklEIjoiNjVlMjJkZGQzNzhlYmFkNzRhYjYyOTA5IiwidXNlcklEIjoiNjVlMjJkZGMzNzhlYmFkNzRhYjYyOTA4In0.acg7GAA5X5tnr0Vd85T4QONp6CoKcwYwCIDA-ALOrWs");
        }
    }
}

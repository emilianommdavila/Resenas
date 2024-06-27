using System.Text.Json;

namespace Resenas.Model
{
    public class ResenaDto
    {
        public string idMongo { get; set; }
        public int id { get; set; }
        public string userID { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public int orderID { get; set; }
        public int articleId { get; set; }
        public int valoration { get; set; }
        public string? content { get; set; }
        public int puntuation { get; set; }

        public static ResenaDto MapToResenaDto(Resena resena)
        {
            return new ResenaDto
            {
                idMongo = resena.idMongo.ToString(),
                id = resena.id,
                userID = resena.userID,
                created = resena.created,
                updated = resena.updated,
                orderID = resena.orderID,
                articleId = resena.articleId,
                valoration = resena.valoration,
                content = resena.content,
                puntuation = resena.puntuation
            };
        }
    }

    public class ResenaRequestDto
    {
        public int orderID { get; set; }
        public int articleID { get; set; }
        public int valoration { get; set; }
        public string content { get; set; }
    }

    public class ResenaExample
    {
        public JsonElement GetExamples()
        {
            //var example = new
            //{
            //    orderID = 1,
            //    valoration = 5,
            //    content = "Ejemplo de contenido de reseña."
            //};
            //var jsonString = JsonSerializer.Serialize(example);
            //return JsonSerializer.Deserialize<JsonElement>(jsonString);
            var example = (new
            {
                success = true,
                message = "La reseña se modifico correctamente"
            });
            var jsonString = JsonSerializer.Serialize(example);
            //return jsonString;
            return JsonSerializer.Deserialize<JsonElement>(jsonString);
        }
    }
}

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
        public string? imageUrl { get; set; }
        public int punctuation { get; set; }
        public List<PuntiacionDeResena> idUsuariosConPuntuacionEnEstaResena { get; set; }

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
                imageUrl = resena.imageUrl,
                punctuation = resena.punctuation,
                idUsuariosConPuntuacionEnEstaResena = resena.idUsuariosConPuntuacionEnEstaResena
            };
        }
    }
}

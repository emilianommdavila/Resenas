using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Resenas.Model
{

    public interface IResenaRepository
    {
        List<Resena> GetResenaByArticle(int articleID);
        List<Resena> GetResenaByUser(int idUser);
        Resena GetResenaByID(ObjectId objectId);
        int GetPunctuationByID(ObjectId objectId);
        Resena InsertResena(Resena resena);
        Resena ModificarResena(Resena resena);
        bool EliminarResena(ObjectId idResena);
    }

    public class Resena
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId idMongo { get; set; }
        public int id { get; set; }// posible deshuso
        public int userID { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public int orderID { get; set; }
        public int articleId { get; set; }
        public int valoration { get; set; }
        public string? content { get; set; }
        public string? imageUrl { get; set; }
        public int punctuation { get; set; }
        public List<PuntiacionDeResena> idUsuariosConPuntuacionEnEstaResena { get; set; }

    }


    public class PuntiacionDeResena {
        public string idUsuario { get; set; }
        public int puntuacion { get; set; }
    
    }
}

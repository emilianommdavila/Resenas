using MongoDB.Bson;
using MongoDB.Driver;
using Resenas.Model.Interfaces;
using System;

namespace Resenas.Model.Repositories
{
    public class ResenaRepository : IResenaRepository
    {

        private readonly IMongoDatabase _database;


        public ResenaRepository(MongoDbSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);

            if (_database == null)
            {
                throw new InvalidOperationException("Database connection is null.");
            }
        }

        //Busca las resenas asociadas a un articulo
        public List<Resena> GetResenaByArticle(int articleID)
        {
            if (_database == null)
            {
                throw new InvalidOperationException("La BD es nula");
            }

            var collection = _database.GetCollection<Resena>("resenas");
            var filter = Builders<Resena>.Filter.Eq(r => r.articleId, articleID);
            List<Resena> resenasEncontrada = collection.Find(filter).ToList();

            return resenasEncontrada;
        }


        //Busca reseñas asociadas a un usuario
        //public List<Resena> GetResenaByUser(string idUser)
        //{
        //    ObjectId idUserMongo = ObjectId.Parse(idUser);
        //    var collection = _database.GetCollection<Resena>("resenas");
        //    var filter = Builders<Resena>.Filter.Eq(r => ObjectId.Parse(r.userID), idUserMongo);
        //    List<Resena> resenasEncontrada = collection.Find(filter).ToList();

        //    return resenasEncontrada;
        //}

        public List<Resena> GetResenaByUser(string idUser)
        {
            var collection = _database.GetCollection<Resena>("resenas");
            var filter = Builders<Resena>.Filter.Eq(r => r.userID, idUser);
            List<Resena> resenasEncontradas = collection.Find(filter).ToList();

            return resenasEncontradas;
        }

        public Resena GetResenaByID(ObjectId objectId)
        {
            ObjectId objectIdMongo = objectId;
            if (_database == null)
            {
                throw new InvalidOperationException("La BD es nula");
            }

            // var collection = _database.GetCollection<Resena>("resenas");
            //var projection = Builders<Resena>.Projection
            // .Exclude(r => r.idMongo);


            //var filter = Builders<Resena>.Filter.Eq(r => ObjectId.Parse(r.idMongo), objectIdMongo);
            //var resena = collection.Find(filter).Project<Resena>(projection).FirstOrDefault();


            var collection = _database.GetCollection<Resena>("resenas");
            var filter = Builders<Resena>.Filter.Eq(r => r.idMongo, objectIdMongo);
            var resena = collection.Find(filter).FirstOrDefault();
            return resena;


        }

        public int GetPunctuationByID(ObjectId objectId)
        {
            ObjectId objectIdMongo =objectId;
            if (_database == null)
            {
                throw new InvalidOperationException("La BD es nula");
            }

            var collection = _database.GetCollection<Resena>("resenas");
            var filter = Builders<Resena>.Filter.Eq(r => r.idMongo, objectIdMongo);
            var resena = collection.Find(filter).FirstOrDefault();

            return resena.puntuation;
        }

        public Resena InsertResena(Resena resena)
        {
            if (_database == null)
            {
                throw new InvalidOperationException("La BD es nula");
            }

            var collection = _database.GetCollection<Resena>("resenas");

            var resenaInsert = new Resena() { userID = resena.userID, created = DateTime.Now, updated = DateTime.Now, orderID = resena.orderID, articleId = resena.articleId, valoration = resena.valoration, content = resena.content, imageUrl = "notengo" };
            collection.InsertOne(resenaInsert);
            return resenaInsert;
        }

        public Resena ModificarResena(Resena resena)
        {
            if (_database == null)
            {
                throw new InvalidOperationException("La BD es nula");
            }

            var collection = _database.GetCollection<Resena>("resenas");

            var filter = Builders<Resena>.Filter.Eq(r => r.idMongo, resena.idMongo);

            // Obtén el documento existente
            var existingResena = collection.Find(filter).FirstOrDefault();

            if (existingResena == null)
            {
                throw new InvalidOperationException("La reseña no existe");
            }

            // Actualiza solo los campos necesarios
            existingResena.updated = DateTime.Now;
            existingResena.valoration = resena.valoration;
            existingResena.content = resena.content;
            existingResena.imageUrl = "notengo";

            // Reemplaza el documento existente
            var result = collection.ReplaceOne(filter, existingResena, new ReplaceOptions { IsUpsert = true });
            return existingResena;
        }



        public bool EliminarResena(ObjectId idResena)
        {
            ObjectId idResenaMongo = idResena;
            if (_database == null)
            {
                throw new InvalidOperationException("La BD es nula");
            }

            var collection = _database.GetCollection<Resena>("resenas");

            var filter = Builders<Resena>.Filter.Eq(r => r.idMongo, idResenaMongo);
            if (collection.Find(filter).FirstOrDefault() == null)
            {
                return false;
            }
            var result = collection.DeleteOne(filter);

            if (collection.Find(filter).FirstOrDefault() != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}

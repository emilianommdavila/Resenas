﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Resenas.Model
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
        public List<Resena> GetResenaByUser(int idUser)
        {          
            var collection = _database.GetCollection<Resena>("resenas");
            var filter = Builders<Resena>.Filter.Eq(r => r.userID, idUser);
            List<Resena> resenasEncontrada = collection.Find(filter).ToList<Resena>();

            return resenasEncontrada;
        }

        public Resena GetResenaByID(ObjectId objectId) {
            if (_database == null)
            {
                throw new InvalidOperationException("La BD es nula");
            }

            var collection = _database.GetCollection<Resena>("resenas");
            var projection = Builders<Resena>.Projection
               .Exclude(r => r.idMongo);


            var filter = Builders<Resena>.Filter.Eq(r => r.idMongo, objectId);
            var resena = collection.Find(filter).Project<Resena>(projection).FirstOrDefault();
            return resena;
        }

        public int GetPunctuationByID(ObjectId objectId)
        {

            if (_database == null)
            {
                throw new InvalidOperationException("La BD es nula");
            }

            var collection = _database.GetCollection<Resena>("resenas");
            var filter = Builders<Resena>.Filter.Eq(r => r.idMongo, objectId);
            var resena = collection.Find(filter).FirstOrDefault();

            return resena.punctuation;
        }

        public Resena InsertResena(Resena resena) {
            if (_database == null)
            {
                throw new InvalidOperationException("La BD es nula");
            }

            var collection = _database.GetCollection<Resena>("resenas");

            var resenaInsert = new Resena() { userID = resena.userID, created = DateTime.Now, updated = DateTime.Now, orderID = resena.orderID, articleId = resena.articleId, valoration = resena.valoration, content = resena.content, imageUrl = "notengo" };
            collection.InsertOne(resenaInsert);
            return resenaInsert;
        }

        public Resena ModificarResena(Resena resena) {

            if (_database == null)
            {
                throw new InvalidOperationException("La BD es nula");
            }

            var collection = _database.GetCollection<Resena>("resenas");

            var filter = Builders<Resena>.Filter.Eq(r => r.idMongo, resena.idMongo);
            var replacement = new Resena()
            {
                //userID = 2,
                //created = filter.created,
                idMongo = ObjectId.Parse(resena.idMongo.ToString()),
                updated = DateTime.Now,
                //orderID = resena.orderID,
                //articleId = resena.articleId,
                valoration = resena.valoration,
                content = resena.content,
                imageUrl = "notengo"
            };

            var result = collection.ReplaceOne(filter, replacement, new ReplaceOptions { IsUpsert = true });
            return replacement;
        }


        public bool EliminarResena(ObjectId idResena) {

            if (_database == null)
            {
                throw new InvalidOperationException("La BD es nula");
            }

            var collection = _database.GetCollection<Resena>("resenas");

            var filter = Builders<Resena>.Filter.Eq(r => r.idMongo, idResena);
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

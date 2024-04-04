using MongoDB.Bson;
using Resenas.Model;

namespace Resenas.Model.Interfaces
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
}

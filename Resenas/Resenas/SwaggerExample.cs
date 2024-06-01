using MongoDB.Bson;
using Resenas.Model;
using Swashbuckle.AspNetCore.Filters;

namespace Resenas
{
    public class SwaggerExample : IExamplesProvider<Resena>
    {
        public Resena GetExamples()
        {
            return new Resena()
            {
                idMongo = new ObjectId(),
                userID = "fasf",
                orderID = 21,
                articleId = 21,
                valoration = 5,
                content = "dasd"
            };
        }
    }
}


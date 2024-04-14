using LiteDB;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MicroRest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CollectionController : ControllerBase
    {
        private const string dbfile = "litedb.db";

        [HttpGet]
        [Route("{nome}")]
        public IActionResult ObterColecao(string nome)
        {
            using var db = new LiteDatabase(dbfile);
            var colecao = db.GetCollection(nome);
            var docs = colecao.Query().ToArray();
            var doc = new BsonArray(docs);
            var docJson = LiteDB.JsonSerializer.Serialize(doc);
            return Ok(JsonDocument.Parse(docJson));
        }

        [HttpGet]
        [Route("{nome}/{id}")]
        public IActionResult ObterPorId(string nome, string id)
        {
            using var db = new LiteDatabase(dbfile);
            var colecao = db.GetCollection(nome);
            var doc = colecao.FindById(new ObjectId(id));
            if (doc != null)
            {
                var docJson = LiteDB.JsonSerializer.Serialize(doc);
                return Ok(JsonDocument.Parse(docJson));
            }
            else
            {
                return Ok(null);
            }
        }

        [HttpPut]
        [Route("{nome}")]
        public IActionResult Inserir(string nome, [FromBody] JsonDocument json)
        {
            using var db = new LiteDatabase(dbfile);
            var colecao = db.GetCollection(nome);
            var valor = LiteDB.JsonSerializer.Deserialize(json.RootElement.GetRawText());
            var doc = new BsonDocument((IDictionary<string, BsonValue>)valor);
            colecao.Insert(doc);
            db.Commit();
            var docJson = LiteDB.JsonSerializer.Serialize(doc);
            return Ok(JsonDocument.Parse(docJson));
        }
    }
}

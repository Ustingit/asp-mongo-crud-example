using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace MongoProject1.Models
{
    public class ComputerContext
    {
        private IMongoDatabase db;
        private IGridFSBucket gridFs;

        public ComputerContext()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            var connection = new MongoUrlBuilder(connectionString);
            var client = new MongoClient(connectionString);
            db = client.GetDatabase(connection.DatabaseName);
            gridFs = new GridFSBucket(db);
        }

        public IMongoCollection<Computer> Computers => db.GetCollection<Computer>("Computers");

        public async Task<IEnumerable<Computer>> GetComputers(int? year, string name)
        {
            var builder = new FilterDefinitionBuilder<Computer>();
            var filter = builder.Empty; // фильтр для выборки всех документов

            if (!string.IsNullOrWhiteSpace(name))
            {
                filter = filter & builder.Regex("Name", new BsonRegularExpression(name));
            }

            if (year.HasValue)
            {
                filter = filter & builder.Eq("Year", year.Value);
            }

            return await Computers.Find(filter).ToListAsync();
        }

        public async Task<Computer> GetComputer(string id)
        {
            return await Computers.Find(new BsonDocument("_id", new BsonObjectId(id))).FirstOrDefaultAsync();
        }

        public async Task Create(Computer c)
        {
            await Computers.InsertOneAsync(c);
        }

        public async Task Update(Computer c)
        {
            await Computers.ReplaceOneAsync(new BsonDocument("_id", new BsonObjectId(c.Id)), c);
        }

        public async Task Remove(string id)
        {
            await Computers.DeleteOneAsync(new BsonDocument("_id", new BsonObjectId(id)));
        }

        public async Task<byte[]> GetImage(string id)
        {
            return await gridFs.DownloadAsBytesAsync(new ObjectId(id));
        }

        public async Task StoreImange(string id, Stream imageStream, string imageName)
        {
            var computer = await GetComputer(id);

            if (computer.HasImage)
            {
                // если ранее уже была прикреплена картинка, удаляем ее
                await gridFs.DeleteAsync(new ObjectId(computer.ImageId));
            }

            var imageId = await gridFs.UploadFromStreamAsync(imageName, imageStream);
            computer.ImageId = imageId.ToString();

            var filter = Builders<Computer>.Filter.Eq("_id", new ObjectId(computer.Id));
            var update = Builders<Computer>.Update.Set("ImageId", computer.ImageId);

            await Computers.UpdateOneAsync(filter, update);
        }
    }
}
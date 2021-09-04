using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Server;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoProject1.Models
{
    public class Computer
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name []
    }
}
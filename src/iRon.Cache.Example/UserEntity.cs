using iRon.Cache.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iRon.Cache.Example.Controllers
{
    [CacheDuration(Enums.Duration.LOW)]
    public class UserEntity
    {
        public ObjectId Id { get; set; }
        public Guid GlobalId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public DateTimeOffset OtherDate { get; set; }        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using iRon.Cache.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace iRon.Cache.Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        readonly ICacheRepository<UserEntity> cacheRepository;
        public ValuesController(ICacheRepository<UserEntity> cacheRepository)
        {
            this.cacheRepository = cacheRepository;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<UserEntity> Get()
        {
            var entity = cacheRepository.Get("user");
            if (entity == null)
            {
                var newUser = new UserEntity()
                {
                    Date = DateTime.Now,
                    GlobalId = Guid.NewGuid(),
                    Id = ObjectId.GenerateNewId(),
                    Name = "XXX",
                    OtherDate = DateTimeOffset.Now
                };
                cacheRepository.Set("user", newUser);
                return newUser;
            }
            else
            {
                return entity;
            }
        }

        [HttpGet("all")]
        public ActionResult<List<UserEntity>> GetAll()
        {
            var newList = new List<UserEntity>();
            var entity = cacheRepository.Gets("userlist");
            if (entity == null)
            {
                var newUser = new UserEntity()
                {
                    Date = DateTime.Now,
                    GlobalId = Guid.NewGuid(),
                    Id = ObjectId.GenerateNewId(),
                    Name = "XXX",
                    OtherDate = DateTimeOffset.Now
                };
                newList.Add(newUser);
                cacheRepository.Set("userlist", newUser);

                return newList;
            }
            else
            {
                return entity.ToList();
            }
        }
    }
}

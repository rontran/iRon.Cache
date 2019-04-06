using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult<UserEntity>> Get()
        {

            var keys =await  this.cacheRepository.GetKeysAsync("*user*");
            if (keys.Any())
            {
                this.cacheRepository.DeletePatternAsync("*user*");
            }
            var entity = await cacheRepository.GetAsync("user");
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
                cacheRepository.SetAsync("user", newUser);
                return newUser;
            }
            else
            {
                return entity;
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<UserEntity>>> GetAll()
        {
            var newList = new List<UserEntity>();
            var entity = await cacheRepository.GetsAsync("userlist");
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
                cacheRepository.SetAsync("userlist", newUser);

                return newList;
            }
            else
            {
                return entity.ToList();
            }
        }
    }
}

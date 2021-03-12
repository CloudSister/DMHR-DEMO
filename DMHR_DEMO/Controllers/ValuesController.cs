using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DMHR_DEMO.Controllers
{
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private DMHRDbContext _dMHRDbContext;

        /// <summary>
        /// 构造函数方式注入数据库上下文
        /// </summary>
        /// <param name="dMHRDbContext"></param>
        public ValuesController(DMHRDbContext dMHRDbContext)
        {
            _dMHRDbContext = dMHRDbContext;
        }

        // GET api/values
        [HttpGet]
        [Route("api/[controller]")]
        public ActionResult<IEnumerable<City>> Get()
        {
            List<City> cities = _dMHRDbContext.Cities.ToList();
            return cities;
        }

        [HttpGet]
        [Route("api/[controller]/addCity")]
        public string addCity()
        {
            City city = new City();
            city.CITY_ID = "HF";
            city.CITY_NAME = "合肥";
            city.REGION_ID = 2;
            _dMHRDbContext.Cities.Add(city);
            _dMHRDbContext.SaveChanges();
            return "数据插入成功！";
        }

    }
}

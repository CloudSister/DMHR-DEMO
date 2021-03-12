# DMHR-DEMO

#### 介绍

asp.net core 2.1 WebAPI 项目连接达梦数据库 DM8，访问 City 表查询所有城市列表数据及插入数据的示例 Demo。

#### 软件架构

ASP.NET Core WebAPI

EntityFrameCore

达梦 DM8 数据库

#### 安装教程

1、先下载达梦数据库，并找到安装包下 source\drivers\dotNet 目录，先记住这两个目录名 DmProvider 和 EFCore.Dm2.1;

2、打开 Visual Studio 2017，我们新建一个 ASP.NET Core WebAPI 项目，这里的.NET Core SDK 版本选择 2.1;

3、右键解决方案下的“依赖项”，然后选择“管理 NuGet 程序包（N）...”;

4、在打开的 NuGet 包管理器界面，点击右上角的“设置”图标，打开选项设置对话框，点击上方的+添加可用程序包源，名称分别为 DmProvider 和 EFCore.Dm2.1，各自对应的源为达梦安装包驱动的目录;

5、程序包源下拉框中选择 DmProvider，点击左侧出现的 DmProvider 包，然后右侧点击“安装”;

6、程序包源下拉框中选择 EFCore.Dm2.1，点击左侧出现的 Microsoft.EntityFrameworkCore.Dm 包，然后右侧点击“安装”;

7、接着使用 NuGet 包管理工具，安装如下支持包：

```
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.Design
```



8、在 appsettings.json 文件中添加达梦数据库连接字符串

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DmConnectionString": "Server=127.0.0.1;Port=5236;Database=DMHR;UID=SYSDBA;PWD=ATapesm@2020;"
  }
}
```


9、我们在解决方案下创建一个 City 类，里面包含数据库表对应的三个字段（特别注意：如果达梦数据库在安装时，默认区分大小写的话，对应的实体类的表名和字段名的大小写必须一致，否则运行时会报错）：

```
using System.ComponentModel.DataAnnotations;

namespace DMHR_DEMO
{
    public class City
    {
        /// <summary>
        /// 城市编码
        /// </summary>
        [Key]
        public string CITY_ID { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CITY_NAME { get; set; }
        /// <summary>
        /// 区域编码
        /// </summary>
        public int REGION_ID { get; set; }
    }
}

```


10、创建一个数据库上下文 DMHRDbContext 类

```
using Microsoft.EntityFrameworkCore;

namespace DMHR_DEMO
{
    public partial class DMHRDbContext : DbContext
    {
        public DMHRDbContext(DbContextOptions<DMHRDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //数据库模式名称,一定要添加，否则会报错
            modelBuilder.HasDefaultSchema("DMHR"); 

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("CITY");
                entity.Property(e => e.CITY_ID).HasMaxLength(3);
                entity.Property(e => e.CITY_NAME).HasMaxLength(40);
                entity.Property(e => e.REGION_ID).HasColumnType("int(10)");
            });
        }
    }
}

```


11、修改 Startup.cs 类中 ConfigureServices 方法，注册 DMHRDbContext 上下文服务

```
public void ConfigureServices(IServiceCollection services)
{
      //【达梦】EF Dm 配置
     services.AddDbContext<DMHRDbContext>(options =>
         options.UseDm(Configuration.GetConnectionString("DmConnectionString"))
     );
     services.AddEntityFrameworkDm();
     services.AddMvc().AddWebApiConventions().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
}
```


12、修改解决方案下的 Controllers 目录下的 ValuesController 控制器类，通过在构造函数注入数据库上下文类，并编写从 City 表获取所有数据的方法

```
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

```


#### 使用说明

运行程序，访问地址：

http://localhost:65026/api/values 获取所有城市信息

http://localhost:65026/api/addCity 添加一条城市合肥数据

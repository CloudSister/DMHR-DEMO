# DMHR-DEMO

#### 介绍

asp.net core 2.1 WebAPI项目连接达梦数据库DM8，访问City表查询所有城市列表数据及插入数据的示例Demo。

#### 软件架构

ASP.NET Core WebAPI

EntityFrameCore

达梦DM8数据库

#### 安装教程

1.先下载达梦数据库，并找到安装包下source\drivers\dotNet目录，先记住这两个目录名DmProvider和EFCore.Dm2.1;
 
2.打开Visual Studio 2017，我们新建一个ASP.NET Core WebAPI项目，这里的.NET Core SDK版本选择2.1;
 
3.右键解决方案下的“依赖项”，然后选择“管理NuGet程序包（N）...”;
 
4.在打开的NuGet包管理器界面，点击右上角的“设置”图标，打开选项设置对话框，点击上方的+添加可用程序包源，名称分别为DmProvider和EFCore.Dm2.1，各自对应的源为达梦安装包驱动的目录;
 
5.	程序包源下拉框中选择DmProvider，点击左侧出现的DmProvider包，然后右侧点击“安装”;
 
6.	程序包源下拉框中选择EFCore.Dm2.1，点击左侧出现的Microsoft.EntityFrameworkCore.Dm包，然后右侧点击“安装”;
 
7.接着使用NuGet包管理工具，安装如下支持包：
   Microsoft.EntityFrameworkCore
   Microsoft.EntityFrameworkCore.Tools
   Microsoft.EntityFrameworkCore.Design
 
8.在 appsettings.json文件中添加达梦数据库连接字符串

{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    //达梦DM数据库连接字符串
    "DmConnectionString": "Server=127.0.0.1;Port=5236;Database=DMHR;UID=SYSDBA;PWD=SYSDBA;"
  }
}

10.我们在解决方案下创建一个City类，里面包含数据库表对应的三个字段（特别注意：如果达梦数据库在安装时，默认区分大小写的话，对应的实体类的表名和字段名的大小写必须一致，否则运行时会报错）：

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

11.创建一个数据库上下文DMHRDbContext类

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMHR_DEMO
{
    public class DMHRDbContext : DbContext
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
 
11.修改Startup.cs类中ConfigureServices方法，注册DMHRDbContext上下文服务

public void ConfigureServices(IServiceCollection services)
{
     //【达梦】EF Dm 配置
     services.AddDbContext<DMHRDbContext>(options =>
         options.UseDm(Configuration.GetConnectionString("DmConnectionString"))
     );
     services.AddEntityFrameworkDm();
     services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
}
 
12.修改解决方案下的Controllers目录下的ValuesController控制器类，通过在构造函数注入数据库上下文类，并编写从City表获取所有数据的方法

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace DMHR_DEMO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DMHRDbContext _dMHRDbContext;

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
        public ActionResult<IEnumerable<City>> Get()
        {
            List<City> cities = _dMHRDbContext.Cities.ToList();
            return cities;
        }
    }
}


#### 使用说明

  运行程序，访问地址：

  http://localhost:65026/api/values 获取所有城市信息
 
  http://localhost:65026/api/addCity 添加一条城市合肥数据
  



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

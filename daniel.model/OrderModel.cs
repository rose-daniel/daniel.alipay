using System;
using System.Collections.Generic;
using System.Text;

namespace daniel.model
{
    public class OrderModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public int userId { get; set; }
        /// <summary>
        /// 产品价格编号
        /// </summary>
        public int priceId { get; set; }
        /// <summary>
        /// 产品种类
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string subClass { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mainVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string subVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string provider { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string specification { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int num { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public decimal totalPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime addTime { get; set; }
    }
}

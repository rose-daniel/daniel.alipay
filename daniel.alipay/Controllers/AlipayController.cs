using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alipay.EasySDK.Factory;
using Alipay.EasySDK.Payment.Page.Models;
using daniel.bll;
using daniel.model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace daniel.alipay.Controllers
{
    /// <summary>
    /// 阿里云支付
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AlipayController : ControllerBase
    {
        /// <summary>
        /// 将小数值按指定的小数位数截断
        /// </summary>
        /// <param name="d">要截断的小数</param>
        /// <param name="s">小数位数，s大于等于0，小于等于28</param>
        /// <returns></returns>
        private static decimal ToFixed(decimal d, int s)
        {
            decimal sp = Convert.ToDecimal(Math.Pow(10, s));

            if (d < 0)
                return Math.Truncate(d) + Math.Ceiling((d - Math.Truncate(d)) * sp) / sp;
            else
                return Math.Truncate(d) + Math.Floor((d - Math.Truncate(d)) * sp) / sp;
        }

        [HttpPost]
        public MessageModel<string> WebPay([FromBody] IdModel item)
        {
            if (Order.GetDetail(item.id, out OrderModel model))
            {
                string subject = $"{model.name}({model.mainVersion}.{model.subVersion})_{model.specification}";
                string out_trade_no = Guid.NewGuid().ToString("N");
                string total_amount = ToFixed(model.totalPrice, 2).ToString();

                if (Order.EditAlipay(item.id, out_trade_no, subject, total_amount))
                {
                    AlipayTradePagePayResponse response = Factory.Payment.Page().Pay(subject,
             out_trade_no, total_amount, "跳转到前台地址");
                    return new MessageModel<string>
                    {
                        success = true,
                        message = "",
                        result = response.Body
                    };
                }
            }

            return new MessageModel<string>
            {
                success = false,
                message = "参数错误",
                result = ""
            };
        }

        /// <summary>
        /// 异步通知接收接收返回内容
        /// </summary>
        [HttpPost("Notify")]
        [AllowAnonymous]
        public void Notify()
        {
            // 使用Dictionary保存参数
            Dictionary<string, string> resData = new Dictionary<string, string>();

            IFormCollection forms = Request.Form;
            string[] requestItem = forms.Keys.ToArray();

            for (int i = 0; i < requestItem.Length; i++)
            {
                resData.Add(requestItem[i], Request.Form[requestItem[i]]);
            }
            // 验证成功
            if (Factory.Payment.Common().VerifyNotify(resData) == true)
            {
                AliPayNotify.Add(resData);
                //string json = JsonConvert.SerializeObject(resData, Formatting.Indented);
                //Utils.WriteLog($"{DateTime.Now.ToString()}-验证成功：" + json);
            }
        }
    }
}

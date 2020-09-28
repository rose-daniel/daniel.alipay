using daniel.dal;
using daniel.model;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace daniel.bll
{
    public class AliPayNotify
    {
        public static bool Add(Dictionary<string, string> items)
        {
            AliPayiNotifyModel model = new AliPayiNotifyModel();
            PropertyInfo[] props = model.GetType().GetProperties();
            Parallel.ForEach(props, p =>
            {
                if (items.TryGetValue(p.Name, out string item))
                {
                    p.SetValue(model, item);
                }
            });

            //记录支付信息
            MySqlParameter[] permission = new MySqlParameter[]
            {
                new MySqlParameter("gmt_create",MySqlDbType.VarChar,45),
                new MySqlParameter("charset",MySqlDbType.VarChar,45),
                new MySqlParameter("gmt_payment",MySqlDbType.VarChar,45),
                new MySqlParameter("notify_time",MySqlDbType.VarChar,45),
                new MySqlParameter("subject",MySqlDbType.VarChar,45),
                new MySqlParameter("buyer_id",MySqlDbType.VarChar,45),
                new MySqlParameter("invoice_amount",MySqlDbType.VarChar,45),
                new MySqlParameter("version",MySqlDbType.VarChar,45),
                new MySqlParameter("notify_id",MySqlDbType.VarChar,45),
                new MySqlParameter("fund_bill_list",MySqlDbType.VarChar,45),
                new MySqlParameter("notify_type",MySqlDbType.VarChar,45),
                new MySqlParameter("out_trade_no",MySqlDbType.VarChar,45),
                new MySqlParameter("total_amount",MySqlDbType.VarChar,45),
                new MySqlParameter("trade_status",MySqlDbType.VarChar,45),
                new MySqlParameter("trade_no",MySqlDbType.VarChar,45),
                new MySqlParameter("auth_app_id",MySqlDbType.VarChar,45),
                new MySqlParameter("receipt_amount",MySqlDbType.VarChar,45),
                new MySqlParameter("point_amount",MySqlDbType.VarChar,45),
                new MySqlParameter("buyer_pay_amount",MySqlDbType.VarChar,45),
                new MySqlParameter("app_id",MySqlDbType.VarChar,45),
                new MySqlParameter("seller_id",MySqlDbType.VarChar,45)
            };
            permission[0].Value = model.gmt_create;
            permission[1].Value = model.charset;
            permission[2].Value = model.gmt_payment;
            permission[3].Value = model.notify_time;
            permission[4].Value = model.subject;
            permission[5].Value = model.buyer_id;
            permission[6].Value = model.invoice_amount;
            permission[7].Value = model.version;
            permission[8].Value = model.notify_id;
            permission[9].Value = model.fund_bill_list;
            permission[10].Value = model.notify_type;
            permission[11].Value = model.out_trade_no;
            permission[12].Value = model.total_amount;
            permission[13].Value = model.trade_status;
            permission[14].Value = model.trade_no;
            permission[15].Value = model.auth_app_id;
            permission[16].Value = model.receipt_amount;
            permission[17].Value = model.point_amount;
            permission[18].Value = model.buyer_pay_amount;
            permission[19].Value = model.app_id;
            permission[20].Value = model.seller_id;

            int row = DbHelper.ExecuteNonQuery("INSERT INTO `aliPayNotify` (`gmt_create`,`charset`,`gmt_payment`,`notify_time`,`subject`,`buyer_id`,`invoice_amount`,`version`,`notify_id`,`fund_bill_list`,`notify_type`,`out_trade_no`,`total_amount`,`trade_status`,`trade_no`,`auth_app_id`,`receipt_amount`,`point_amount`,`buyer_pay_amount`,`app_id`,`seller_id`) VALUES (@gmt_create,@charset,@gmt_payment,@notify_time,@subject,@buyer_id,@invoice_amount,@version,@notify_id,@fund_bill_list,@notify_type,@out_trade_no,@total_amount,@trade_status,@trade_no,@auth_app_id,@receipt_amount,@point_amount,@buyer_pay_amount,@app_id,@seller_id);", permission);

            if (row > 0)
            {
                if (model.trade_status == "TRADE_SUCCESS")
                {
                    //更新订单状态
                    Order.SetStatus(model.out_trade_no);
                }
            }

            return row > 0;

        }


    }
}

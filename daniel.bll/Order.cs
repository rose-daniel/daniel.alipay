using daniel.common.Helper;
using daniel.dal;
using daniel.model;
using MySql.Data.MySqlClient;
using System.Data;

namespace daniel.bll
{
    public class Order
    {
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool GetDetail(int id, out OrderModel model)
        {
            DataTable dt = DbHelper.ExecuteDataTable($"SELECT `id`,`userId`,`priceId`,`category`,`subClass`,`name`,`mainVersion`,`subVersion`,`provider`,`specification`,`price`,`num`,`totalPrice`,`status` FROM `order` where `id` = {id};");
            model = new OrderModel();
            if (dt != null && dt.Rows.Count > 0)
            {
                model.id = dt.Rows[0]["id"].ObjToInt();
                model.userId = dt.Rows[0]["userId"].ObjToInt();
                model.priceId = dt.Rows[0]["priceId"].ObjToInt();
                model.category = dt.Rows[0]["category"].ObjToString();
                model.subClass = dt.Rows[0]["subClass"].ObjToString();
                model.name = dt.Rows[0]["name"].ObjToString();
                model.mainVersion = dt.Rows[0]["mainVersion"].ObjToString();
                model.subVersion = dt.Rows[0]["subVersion"].ObjToString();
                model.provider = dt.Rows[0]["provider"].ObjToString();

                model.specification = dt.Rows[0]["specification"].ObjToString();
                model.price = dt.Rows[0]["price"].ObjToDecimal();
                model.num = dt.Rows[0]["num"].ObjToInt();
                model.totalPrice = dt.Rows[0]["totalPrice"].ObjToDecimal();
                model.status = dt.Rows[0]["status"].ObjToInt();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新支付宝付款信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="outTradeNo"></param>
        /// <param name="paySubject"></param>
        /// <param name="payTotalAmount"></param>
        /// <returns></returns>
        public static bool EditAlipay(int id, string outTradeNo, string paySubject, string payTotalAmount)
        {
            MySqlParameter[] permission = new MySqlParameter[]
            {
                new MySqlParameter("outTradeNo",MySqlDbType.VarChar,45),
                new MySqlParameter("paySubject",MySqlDbType.VarChar,45),
                new MySqlParameter("payTotalAmount",MySqlDbType.Decimal),
                new MySqlParameter("id",MySqlDbType.Int32)
            };
            permission[0].Value = outTradeNo;
            permission[1].Value = paySubject;
            permission[2].Value = payTotalAmount;
            permission[3].Value = id;
            int row = DbHelper.ExecuteNonQuery("UPDATE `order` SET `outTradeNo` = @outTradeNo,`paySubject` = @paySubject,`payTotalAmount` = @payTotalAmount WHERE `id` = @id;", permission);
            return row > 0;
        }

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Add(OrderModel model, out int id)
        {
            MySqlParameter[] permission = new MySqlParameter[]
            {
                new MySqlParameter("userId",MySqlDbType.Int32),
                new MySqlParameter("priceId",MySqlDbType.Int32),
                new MySqlParameter("category",MySqlDbType.VarChar,45),
                new MySqlParameter("subClass",MySqlDbType.VarChar,45),
                new MySqlParameter("name",MySqlDbType.VarChar,45),
                new MySqlParameter("mainVersion",MySqlDbType.VarChar,45),
                new MySqlParameter("subVersion",MySqlDbType.VarChar,45),
                new MySqlParameter("provider",MySqlDbType.VarChar,45),
                new MySqlParameter("specification",MySqlDbType.VarChar,45),
                new MySqlParameter("price",MySqlDbType.Decimal),
                new MySqlParameter("num",MySqlDbType.Int32),
                new MySqlParameter("totalPrice",MySqlDbType.Decimal)
            };
            permission[0].Value = model.userId;
            permission[1].Value = model.priceId;
            permission[2].Value = model.category;
            permission[3].Value = model.subClass;
            permission[4].Value = model.name;
            permission[5].Value = model.mainVersion;
            permission[6].Value = model.subVersion;
            permission[7].Value = model.provider;
            permission[8].Value = model.specification;
            permission[9].Value = model.price;
            permission[10].Value = model.num;
            permission[11].Value = model.totalPrice;

            object obj = DbHelper.ExecuteScalar("INSERT INTO `order` (`userId`,`priceId`,`category`,`subClass`,`name`,`mainVersion`,`subVersion`,`provider`,`specification`,`price`,`num`,`totalPrice`,`status`,`addTime`) VALUES (@userId,@priceId,@category,@subClass,@name,@mainVersion,@subVersion,@provider,@specification,@price,@num,@totalPrice,0,now());select LAST_INSERT_ID();", permission);
            id = obj.ObjToInt();
            return id > 0;
        }

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public static bool SetStatus(string outTradeNo)
        {
            MySqlParameter[] permission = new MySqlParameter[]
           {
                new MySqlParameter("outTradeNo",MySqlDbType.VarChar,45)
           };
            permission[0].Value = outTradeNo;
            int row = DbHelper.ExecuteNonQuery("UPDATE `order` SET `status` =1,`isEnable` = 1,`startTime` = now(),`endTime` = date_add(now(), interval 1 week) WHERE `id` > 0 and `status` = 0 and `outTradeNo` = @outTradeNo", permission);

            return row > 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace daniel.model
{
    public class AliPayNotifyModel
    {

        public int id { get; set; }
        public string gmt_create { get; set; }
        public string charset { get; set; }
        public string gmt_payment { get; set; }
        public string notify_time { get; set; }
        public string subject { get; set; }
        public string buyer_id { get; set; }
        public string invoice_amount { get; set; }
        public string version { get; set; }
        public string notify_id { get; set; }
        public string fund_bill_list { get; set; }
        public string notify_type { get; set; }
        public string out_trade_no { get; set; }
        public string total_amount { get; set; }
        public string trade_status { get; set; }
        public string trade_no { get; set; }
        public string auth_app_id { get; set; }
        public string receipt_amount { get; set; }
        public string point_amount { get; set; }
        public string buyer_pay_amount { get; set; }
        public string app_id { get; set; }
        public string seller_id { get; set; }


    }
}

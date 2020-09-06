using Alipay.EasySDK.Factory;
using Alipay.EasySDK.Kernel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;

namespace daniel.alipay.ServiceExtensions
{
    public static class AlipaySetup
    {
        public static void AddAlipaySetup(this IServiceCollection services, IWebHostEnvironment hostEnvironment)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // 1. 设置参数（全局只需设置一次）
            Factory.SetOptions(GetConfig(hostEnvironment));
        }

        /// <summary>
        /// 阿里云配置
        /// </summary>
        /// <returns></returns>
        private static Config GetConfig(IWebHostEnvironment _hostEnvironment)
        {

            Config obj = new Config()
            {
                Protocol = "https",
                GatewayHost = "openapi.alipay.com",
                SignType = "RSA2",
                //"<-- 请填写您的AppId，例如：2019091767145019 -->"
                AppId = "请填写您的AppId",

                // 为避免私钥随源码泄露，推荐从文件中读取私钥字符串而不是写入源码中
                // <-- 请填写您的应用私钥，例如：MIIEvQIBADANB ... ... -->
                MerchantPrivateKey = File.ReadAllText(Path.Combine(_hostEnvironment.WebRootPath, "assets", "alipay", "tjvoyage.com_PrivateKey.txt"), Encoding.UTF8),

                //"<-- 请填写您的应用公钥证书文件路径，例如：/foo/appCertPublicKey_2019051064521003.crt -->"
                MerchantCertPath = Path.Combine(_hostEnvironment.WebRootPath, "assets", "alipay", "appCertPublicKey_2021001183601583.crt"),
                //"<-- 请填写您的支付宝公钥证书文件路径，例如：/foo/alipayCertPublicKey_RSA2.crt -->",
                AlipayCertPath = Path.Combine(_hostEnvironment.WebRootPath, "assets", "alipay", "alipayCertPublicKey_RSA2.crt"),
                //"<-- 请填写您的支付宝根证书文件路径，例如：/foo/alipayRootCert.crt -->",
                AlipayRootCertPath = Path.Combine(_hostEnvironment.WebRootPath, "assets", "alipay", "alipayRootCert.crt"),

                // 如果采用非证书模式，则无需赋值上面的三个证书路径，改为赋值如下的支付宝公钥字符串即可
                // "<-- 请填写您的支付宝公钥，例如：MIIBIjANBg... -->"
                AlipayPublicKey = File.ReadAllText(Path.Combine(_hostEnvironment.WebRootPath, "assets", "alipay", "tjvoyage.com_PublicKey.txt"), Encoding.UTF8),

                //可设置异步通知接收服务地址（可选）
                NotifyUrl = "填写您的支付类接口异步通知接收服务地址"  //"<-- 请填写您的支付类接口异步通知接收服务地址，例如：https://www.test.com/callback -->",

                //可设置AES密钥，调用AES加解密相关接口时需要（可选）
                //EncryptKey = "<-- 请填写您的AES密钥，例如：aa4BtZ4tspm2wnXLb1ThQA== -->"
            };
            return obj;
        }
    }
}

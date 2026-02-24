using BG_Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace BG_Services
{
    public class HttpService<T> : BaseInstance<T> where T:
        class,new()
    {
        /// <summary> 获得登录信息 </summary>
        /// <param name="userName"></param>
        protected string Http(string Url,string RequestType = "POST")
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            string uri = Url;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(uri);
                // 设置请求的超时
                request.Timeout = 1000;
                request.Method = RequestType;
                request.ContentType = "application/x-www-form-urlencoded";
                Stream myResponseStream = request.GetRequestStream();
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK && (
                    (response.ContentType.Contains("application/json"))))
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(receiveStream, Encoding.UTF8);
                    string retString = myStreamReader.ReadToEnd();
                    return retString;
                }
                else
                {
                    // 未能接受到任何请求的回馈
                    throw new HttpException("Nothing Get");
                }
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("405") ||
                   ex.Message.Contains("404") ||
                   ex.Message.Contains("无法连接远程服务器"))
                {
                    throw new HttpException(ex.Message);
                }
                else if(ex.Message.Contains("超时"))
                {
                    throw new HttpException("连接超时");
                }
                throw ex;
            }
            finally
            {
                // 切记一定要释放资源 
                if (response != null)
                {
                    response.Close();
                }
                request = null;
                response = null;
            }
        }


        /// <summary> Post数据插入方法体 </summary>
        /// <param name="userName"></param>
        protected string Http(string Url, string PostData, Encoding dataEncode , string RequestType = "POST")
        {//
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            string uri = Url;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(PostData); //转化
                request = (HttpWebRequest)WebRequest.Create(uri);
                // 设置请求的超时
                request.Timeout = 2000;
                request.Method = RequestType;
                request.ContentType = "application/json";
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);//写入参数
                    reqStream.Close();
                }
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK && (
                    (response.ContentType.Contains("application/json") || response.ContentType.Contains("text/html;charset=UTF-8"))))
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(receiveStream, Encoding.UTF8);
                    string retString = myStreamReader.ReadToEnd();
                    return retString;
                }
                else
                {
                    // 未能接受到任何请求的回馈
                    throw new HttpException("Nothing Get");
                }
            }
            catch (Exception ex)
            {
                var type = ex.GetType();
                if (ex is WebException)
                {
                    throw ex;
                }
                if (ex.Message.Contains("405") ||
                   ex.Message.Contains("404"))
                {
                    throw new HttpException(ex.Message);
                }
                else if (ex.Message.Contains("超时"))
                {
                    throw new HttpException("连接超时");
                }
                throw ex;
                //return "{\"value\":123 ,\"resultStatus\": \"1\",\"msg\": \"登录超时\"}";
            }
            finally
            {
                // 切记一定要释放资源 
                if (response != null)
                {
                    response.Close();
                }
                request = null;
                response = null;
            }
        }
    }
}

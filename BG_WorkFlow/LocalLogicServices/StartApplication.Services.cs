using BGModel;
using CMW.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BG_WorkFlow
{
    public class StartUgrApplication
    {
        public static StartUgrApplication Services { get; private set; }
        XmlDocument xmlDoc = new XmlDocument();
        public VersionInfo versionInfo;
        string xmlPath = SystemDirectoryConfig.AppDir + "UpdateData.xml";
        static StartUgrApplication()
        {
            Services = new StartUgrApplication();
        }
        public void StartUgr()
        {
            VersionParamater versionParamater = new VersionParamater();
            versionInfo = getVersionInfo();
            versionParamater.ClientCode = versionInfo?.serverSoftware.AppCode;
            versionParamater.CurrentVersion = versionInfo?.serverSoftware.Version;
            versionParamater.UpdateFilePath = SystemDirectoryConfig.AppDir.TrimEnd('\\');
            versionParamater.UgrServerIp = ConfigServices.GetInstance().localConfigModel.UgrServer; //TODU这里要添加一个更新后台服务器的接口配置
            versionParamater.MainApplicationPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            versionParamater.Language = ConfigServices.GetInstance().localConfigModel.LANGUAGE;
            string PassParamater = CommonFunc.ObjectToJson(versionParamater).Replace("\"", "\\\"");
            ActionStartClientExcute(PassParamater);
        }
        private void ActionStartClientExcute(string ArgumentsJson)
        {
            var address = Path.Combine(SystemDirectoryConfig.AppDir, "Upd.exe");// @"D:\Code\ControlStation\UpdateUgr\OutPut\Upd.exe";
            if(File.Exists(address))
            {
                Process process = new Process();
                process.StartInfo.FileName = address;
                process.StartInfo.Arguments = ArgumentsJson;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
        }

        public VersionInfo getVersionInfo()
        {
            VersionInfo versionInfo = new VersionInfo();
            ServerSoftware serverSoftware = new ServerSoftware();
            List<Description> descriptions = new List<Description>();
            versionInfo.serverSoftware = serverSoftware;
            serverSoftware.AppCode = ReadXmlValue("Main/AppCode");
            serverSoftware.AppName = ReadXmlValue("Main/AppName");
            serverSoftware.Version = ReadXmlValue("Main/Version");
            serverSoftware.ReleaseTime = ReadXmlValue("Main/ReleaseTime");
            descriptions = ReadListDescriptions("Descriptions");
            versionInfo.Descriptions = descriptions;
            return versionInfo;
        }

        public string ReadXmlValue(string Node)
        {
            if (!File.Exists(xmlPath)) return null;
            xmlDoc.Load(xmlPath);
            //获取xml根节点
            XmlNode xmlRoot = xmlDoc.DocumentElement;
            //根据节点顺序逐步读取
            return xmlRoot.SelectSingleNode(Node).InnerText;
        }

        public List<Description> ReadListDescriptions(string Node)
        {
            List<Description> Descriptions = new List<Description>();
            if (!File.Exists(xmlPath)) return null;
            xmlDoc.Load(xmlPath);
            //获取xml根节点
            XmlNode xmlRoot = xmlDoc.DocumentElement;
            //根据节点顺序逐步读取
            XmlNode childernNode = xmlRoot.SelectSingleNode(Node);
            foreach (XmlNode item in childernNode.ChildNodes)
            {
                Description description = new Description() { UpdateContents = new List<UpdateContent>() };
                description.ReleaseTime = item.SelectSingleNode("ReleaseTime").InnerText;
                description.Version = item.SelectSingleNode("Version").InnerText;
                XmlNode ContentNode = item.SelectSingleNode("UpdateContents");
                foreach (XmlNode ContentNodeItem in ContentNode.ChildNodes)
                {
                    UpdateContent updateContent = new UpdateContent();
                    updateContent.UpdateDescription = ContentNodeItem.InnerText;
                    description.UpdateContents.Add(updateContent);
                }
                Descriptions.Add(description);
            }
            return Descriptions;
        }
    }
}

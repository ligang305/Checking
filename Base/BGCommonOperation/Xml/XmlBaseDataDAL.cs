using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;

namespace BGCommonOperation
{
    public class XmlBaseDataDAL : BaseInstance<XmlBaseDataDAL>
    {
        
        #region XML解析载入类

        XMLOperator xmlOper;

        /// <summary>设置项目对象节点</summary>
        /// <param name="item">项目对象</param>
           
        public void SetItemNodeValue<T>(T item,ref XmlNode node)
        {
            Type entityType = typeof(T);
            var propertys = entityType.GetProperties();    
            // 设置属性
            foreach (var property in propertys)
            {
                var Value = property.GetValue(item, null) == null ? string.Empty : property.GetValue(item, null).ToString();
                var AttributeName = Attribute.GetCustomAttribute(property, typeof(BaseAttribute)) as BaseAttribute;
                if (AttributeName != null)
                {
                    xmlOper.SetAttributeValue(node, AttributeName.Name.ToString(), Value);
                }
            }
        }

        /// <summary>转换xml节点到项目对象</summary>
        /// <param name="node">xml节点</param>
        /// <returns>项目对象</returns>
        public T LoadItemNodeValue<T>(XmlNode node) where T : class,new()
        {
            T item = new T();

            var propertys = typeof(T).GetProperties();
            // 设置属性
            foreach (var property in propertys)
            {
                var AttributeName = Attribute.GetCustomAttribute(property, typeof(BaseAttribute)) as BaseAttribute;
                if (AttributeName != null)
                {
                    var Value = xmlOper.GetAttributeValue(node, AttributeName.Name.ToString());
                    property.SetValue(item, Value, null);
                }
            }
            return item;
        }
        /// <summary>初始化并加载工程数据列表文件</summary>
        public void InitItemsFile<T>(string filePath, string NodeName,List<T> TList)
        {
            xmlOper = new XMLOperator();
            // 初始化加载文件
            if (!File.Exists(filePath))
            {
                xmlOper.InitXMLRoot(NodeName);
                xmlOper.SaveXMLFile(filePath);
            }
            else
            {
                try
                {
                    xmlOper.LoadXMLFile(filePath);
                }
                catch
                {
                    File.Delete(filePath);
                    InitItemsFile<T>(filePath, NodeName, TList);
                    return;
                }
                if (xmlOper.XMLDoc == null)
                {
                    xmlOper.InitXMLRoot(NodeName);
                    xmlOper.SaveXMLFile(filePath);
                }
            }
        }
        public bool SaveSingleData<T>(string filePath, T materialPlan, string ParentNodeName, string ChildrenNode)
        {
            try
            {
                var ParentNode = xmlOper.SelectNode(ParentNodeName)[0];
                // 新增项目节点
                XmlNode itemXmlNode = xmlOper.AppendNode(ParentNode, ChildrenNode);
                // 设置项目节点信息
                SetItemNodeValue<T>(materialPlan, ref itemXmlNode);
                // 保存工程xml数据
                xmlOper.SaveXMLFile(filePath);
                return true;
            }
            catch (Exception)
            {
                //TODO 这里需要待定
                return false;
            }
        }
        public bool SaveSingleDataByCondition<T>(string filePath, T tObject, string ParentNodeName,string AttartubtionName,string Value)
        {
            try
            {
                var ParentNode = xmlOper.SelectNode(ParentNodeName)[0];
                //InitItemsFile<T>(filePath, ParentNodeName, null);
                // 找到的节点
                XmlNode itemXmlNode = xmlOper.SelectChildNodeByAttrValue(ParentNode, AttartubtionName, Value);
                // 设置项目节点信息
                SetItemNodeValue<T>(tObject,ref itemXmlNode);
                // 保存工程xml数据
                xmlOper.SaveXMLFile(filePath);
                return true;
            }
            catch (Exception ex)
            {
                //TODO 这里需要待定
                return false;
            }
        }


        public IList<T> GetObjects<T>(string filePath, string NodeName) where T : class,new()
        {
            List<T> itemList = new List<T>();
            // 初始化文件
            InitItemsFile<T>(filePath, NodeName, null);
            // 项目列表节点
            XmlNode rootNode = xmlOper.XMLDoc.SelectSingleNode(NodeName);
            foreach (XmlNode cNode in rootNode.ChildNodes)
            {
                // 加载信息
                T item = new T();
                item = LoadItemNodeValue<T>(cNode);
                itemList.Add(item);
            }
            return itemList;
        }
        #endregion
    }
}

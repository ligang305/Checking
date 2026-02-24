using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using System;
///<summary>
///------------------------中广核贝谷科技有限公司--------------------------
/// Copyright (C) 2019-2020 
/// 文件名：XMLOperator.cs
/// 编写者：朱志武
/// 功能：公用方法类
/// 编写日期：2019-12-30
///--------------------------------------------------
///</summary>
namespace CMW.Common.Utilities
{
    /// <summary>XMLOperator 类提供对 XML 数据的读写</summary>
    public class XMLOperator
    {
        #region "初始化"

        // xml文件对象
        XmlDocument _xmlDoc;
        // 文件路径
        string _filePath = "";
        // xml操作对象
        CXMLDocManage _xmdMgr = new CXMLDocManage();

        /// <summary>构造函数</summary>
        public XMLOperator()
        {
            _xmlDoc = new XmlDocument();
        }

        /// <summary>构造函数</summary>
        /// <param name="xmlIn"></param>
        public XMLOperator(XmlDocument xmlIn)
        {
            _xmlDoc = xmlIn;
        }

        /// <summary>定义声明</summary>
        /// <param name="version">版本</param>
        /// <param name="enCoding">编码格式</param>
        /// <param name="standalone">是否写独立属性(string.empty、Null则不写)</param>
        public void CreateXmlDeclaration(string version, string enCoding, string standalone)
        {
            //定义声明
            XmlDeclaration xmlDec = _xmlDoc.CreateXmlDeclaration(version, enCoding, standalone);
            _xmlDoc.InsertBefore(xmlDec, _xmlDoc.DocumentElement);
        }


        /// <summary>初始化 XmlDocument 根节点</summary>
        /// <param name="rootName">根节点名称</param>
        public XmlNode InitXMLRoot(string rootName)
        {
            // 清空 XmlDocument
            _xmlDoc.RemoveAll();
            // 创建 XML 声明         
            CreateXmlDeclaration("1.0", "UTF-8", "yes");
            // 增加 XmlDocument 根节点
            return _xmlDoc.AppendChild(_xmlDoc.CreateElement(rootName));

        }

        /// <summary>获取根节点</summary>
        /// <returns>赖日昌 20111227</returns>
        public XmlNode GetRootNode()
        {
            if (_xmlDoc == null) return null;
            if (_xmlDoc.ChildNodes.Count <= 0) return null;

            return _xmlDoc.DocumentElement as XmlNode;
        }

        #endregion

        #region "节点状态"

        /// <summary>XML 文档是否为空</summary>
        public bool IsEmpty
        {
            get { return (_xmlDoc.DocumentElement == null); }
        }

        /// <summary>指定节点是否存在</summary>
        /// <param name="xmlPath">XML 节点路径</param>
        /// <returns></returns>
        public bool HaveNode(string xmlPath)
        {
            return (_xmlDoc.SelectSingleNode(xmlPath) != null);
        }

        /// <summary>获取子节点个数</summary>
        /// <param name="xmlPath">XML 节点路径</param>
        /// <returns></returns>
        public int GetNodeChildCount(string xmlPath)
        {
            XmlNode nd = _xmlDoc.SelectSingleNode(xmlPath);
            if (nd == null) return -1;

            return nd.ChildNodes.Count;
        }

        #endregion

        #region "获取节点属性值"

        /// <summary>根据节点查询，返回节点的值</summary>
        /// <param name="xmlPath">XML 节点路径</param>
        /// <returns></returns>
        public string GetNodeValue(string xmlPath)
        {
            XmlNode nd = _xmlDoc.SelectSingleNode(xmlPath);

            return GetNodeValue(nd).Trim();
        }

        /// <summary>获得节点的值</summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string GetNodeValue(XmlNode node)
        {
            if (node == null) return string.Empty;

            return CommonFunc.ParseStr(node.InnerText);
        }

        /// <summary>取得参数节点的值</summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public string GetParamNodeText(string paramName)
        {
            // 取得子节点的值
            return GetNodeChildText(_xmlDoc.DocumentElement, paramName).Trim();
        }

        /// <summary>取得子节点的值</summary>
        /// <param name="parentNode"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public string GetNodeChildText(XmlNode parentNode, string childName)
        {
            if (parentNode == null) return "";

            // 取得指定节点（父节点的绝对路径）
            XmlNode nd = parentNode.SelectSingleNode(childName);
            if (nd == null) return "";

            return nd.InnerText.Trim();
        }

        /// <summary>取得属性值</summary>
        /// <param name="node"></param>
        /// <param name="attName"></param>
        /// <returns></returns>
        public string GetAttributeValue(XmlNode ndIn, string attName)
        {
            if (ndIn == null) return "";

            XmlAttribute att = ndIn.Attributes[attName];
            if (att == null) return "";

            return att.Value.Trim();
        }

        /// <summary>属性查询，返回属性值</summary>
        /// <param name="xmlPath">XML 节点路径</param>
        /// <param name="attName">属性名称</param>
        /// <returns></returns>
        public string GetAttributeValue(string xmlPath, string attName)
        {
            XmlNode nd = _xmlDoc.SelectSingleNode(xmlPath);
            // 取得属性值
            return GetAttributeValue(nd, attName);
        }

        #endregion

        #region "新增节点"

        /// <summary>新增节点</summary>
        /// <param name="ndIn"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public XmlNode AppendNode(XmlNode ndIn, string nodeName)
        {
            if (ndIn == null) return null;

            XmlNode node = _xmlDoc.CreateElement(nodeName);

            ndIn.AppendChild(node);

            return node;
        }

        /// <summary>在指定路径插入节点</summary>
        /// <param name="ndIn">XML 节点</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="newText">节点内容</param>
        /// <returns></returns>
        public XmlNode AppendNode(XmlNode ndIn, string nodeName, string newText)
        {
            XmlNode node = AppendNode(ndIn, nodeName);

            if (node != null && !newText.Trim().Equals(string.Empty)) node.InnerText = newText;

            return node;
        }

        /// <summary> 在第一个位置插入最新对象</summary>
        /// <param name="ndIn"></param>
        /// <param name="nodeName"></param>
        /// <param name="newText"></param>
        /// <returns></returns>
        public XmlNode AppendFirstIndexNode(XmlNode ndIn, string nodeName, string newText)
        {
            if (ndIn == null) return null;

            XmlElement elem = _xmlDoc.CreateElement(nodeName);
            elem.InnerText = newText;

            if (ndIn.ChildNodes.Count > 0)
            {
                return ndIn.InsertBefore(elem, ndIn.ChildNodes[0]);
            }
            else
            {
                return ndIn.AppendChild(elem);
            }

        }

        /// <summary>在指定路径插入节点</summary>
        /// <param name="xmlPath">XML 节点路径</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="newText">节点内容</param>
        /// <returns></returns>
        public XmlNode AppendNode(string xmlPath, string nodeName, string newText)
        {
            XmlNode nd = _xmlDoc.SelectSingleNode(xmlPath);
            // 在指定路径插入节点
            return AppendNode(nd, nodeName, newText);
        }

        /// <summary>在指定路径插入多个节点</summary>
        /// <param name="ndIn">XML 节点</param>
        /// <param name="lstNodeName">子节点名称列表</param>
        /// <param name="lstNewText">子节点内容列表</param>
        /// <returns></returns>
        public bool AppendNode(XmlNode ndIn, List<string> lstNodeName, List<string> lstNewText)
        {
            if (ndIn == null) return false;
            // 循环子节点名称列表
            for (int i = 0; i < lstNodeName.Count; i++)
            {
                if (i >= lstNewText.Count) break;
                // 在指定路径插入节点
                AppendNode(ndIn, lstNodeName[i], lstNewText[i]);
            }
            return true;
        }

        /// <summary>在指定路径插入多个节点</summary>
        /// <param name="xmlPath">XML 节点路径</param>
        /// <param name="lstNodeName">子节点名称列表</param>
        /// <param name="lstNewText">子节点内容列表</param>
        /// <returns></returns>
        public bool AppendNode(string xmlPath, List<string> lstNodeName, List<string> lstNewText)
        {
            XmlNode nd = _xmlDoc.SelectSingleNode(xmlPath);
            // 在指定路径插入多个节点
            return AppendNode(nd, lstNodeName, lstNewText);
        }

        #endregion

        #region "属性管理"

        /// <summary>设置属性值</summary>
        /// <param name="ndIn">XML 节点</param>
        /// <param name="attName">属性名称</param>
        /// <param name="newValue">新的属性值</param>
        /// <returns></returns>
        public bool SetAttributeValue(XmlNode ndIn, string attName, string newValue)
        {
            XmlElement elem = ndIn as XmlElement;
            if (elem == null) return false;
            // 设置节点属性
            elem.SetAttribute(attName, newValue);

            return true;
        }

        /// <summary>设置属性值</summary>
        /// <param name="xmlPath">XML 节点路径</param>
        /// <param name="attName">属性名称</param>
        /// <param name="newValue">新的属性值</param>
        /// <returns></returns>
        public bool SetAttributeValue(string xmlPath, string attName, string newValue)
        {
            XmlNode nd = _xmlDoc.SelectSingleNode(xmlPath);
            // 设置属性值
            return SetAttributeValue(nd, attName, newValue);
        }

        /// <summary>设置多个属性值</summary>
        /// <param name="ndIn">XML 节点</param>
        /// <param name="lstAttName">属性名称列表</param>
        /// <param name="lstNewValue">属性值列表</param>
        /// <returns></returns>
        public bool SetAttributeValue(XmlNode ndIn, List<string> lstAttName, List<string> lstNewValue)
        {
            XmlElement elem = ndIn as XmlElement;
            if (elem == null) return false;

            for (int i = 0; i < lstAttName.Count; i++)
            {
                if (i >= lstNewValue.Count) break;
                // 设置节点属性
                elem.SetAttribute(lstAttName[i], lstNewValue[i]);
            }
            return true;
        }

        /// <summary>设置多个属性值</summary>
        /// <param name="xmlPath">XML 节点路径</param>
        /// <param name="lstAttName">属性名称列表</param>
        /// <param name="lstNewValue">属性值列表</param>
        /// <returns></returns>
        public bool SetAttributeValue(string xmlPath, List<string> lstAttName, List<string> lstNewValue)
        {
            XmlNode nd = _xmlDoc.SelectSingleNode(xmlPath);
            // 设置多个属性值
            return SetAttributeValue(nd, lstAttName, lstNewValue);
        }

        #endregion

        #region "节点管理"

        /// <summary>查找节点</summary>
        /// <param name="xPath">节点路径</param>
        /// <param name="attrName">属性名称</param>
        /// <param name="attrValue">属性值</param>
        /// <returns></returns>
        public XmlNode SelectNode(string xPath, string attrName, string attrValue)
        {
            if (_xmlDoc == null) return null;

            XmlNodeList lstNode = _xmlDoc.SelectNodes(xPath);
            if (lstNode == null || lstNode.Count <= 0) return null;
            // 默认反馈第一个节点
            if (attrName.Equals(string.Empty)) return lstNode[0];

            foreach (XmlNode node in lstNode)
            {
                if (attrValue.Equals(GetAttributeValue(node, attrName)))
                {
                    return node;
                }
            }

            return null;
        }

        /// <summary>查找节点</summary>
        /// <param name="xPath">节点路径</param>
        /// <param name="attrName">属性名称</param>
        /// <param name="attrValue">属性值</param>
        /// <returns></returns>
        public XmlNode SelectContainNode(string xPath, string attrName, string attrValue)
        {
            if (_xmlDoc == null) return null;

            XmlNodeList lstNode = _xmlDoc.SelectNodes(xPath);

            foreach (XmlNode node in lstNode)
            {
                if (GetAttributeValue(node, attrName).Contains(attrValue))
                {
                    return node;
                }
            }

            return null;
        }


        /// <summary> 查找节点 </summary>
        /// <param name="xPath">节点路径</param>
        /// <param name="attrName">属性名称</param>
        /// <param name="attrValue">属性值</param>
        /// <returns></returns>
        public List<XmlNode> SelectContainNodeByExpand(string xPath, string attrName, string attrValue, string expandStr)
        {
            if (_xmlDoc == null) return null;

            List<XmlNode> findNodeLst = new List<XmlNode>();
            XmlNodeList lstNode = _xmlDoc.SelectNodes(xPath);
            attrValue = attrValue + expandStr;
            foreach (XmlNode node in lstNode)
            {
                if (GetAttributeValue(node, attrName).Contains(attrValue))
                {
                    findNodeLst.Add(node);
                }
            }

            return findNodeLst;
        }

        /// <summary>查找节点集合 </summary>
        /// <param name="xPath">节点路径</param>
        /// <returns></returns>
        public XmlNodeList SelectNode(string xPath)
        {
            if (_xmlDoc == null) return null;

            return _xmlDoc.SelectNodes(xPath);
        }

        /// <summary>获取</summary>
        /// <param name="curNode"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static XmlNode SelectNode(XmlNode curNode, string nodeName)
        {
            if (curNode == null) return null;

            if (curNode.ChildNodes == null) return null;

            foreach (XmlNode tmpNode in curNode.ChildNodes)
            {
                if (tmpNode.Name.Trim().Equals(nodeName))
                {
                    return tmpNode;
                }
            }

            return null;
        }

        /// <summary>获取</summary>
        /// <param name="curNode"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public List<XmlNode> SelectNodeList(XmlNode curNode, string nodeName)
        {
            if (curNode == null) return null;

            if (curNode.ChildNodes == null) return null;

            List<XmlNode> xmlNodeList = new List<XmlNode>();

            foreach (XmlNode tmpNode in curNode.ChildNodes)
            {
                if (tmpNode.Name.Trim().Equals(nodeName))
                {
                    xmlNodeList.Add(tmpNode);
                }
            }

            return xmlNodeList;
        }

        /// <summary>根据文本信息获取xml节点</summary>
        /// <param name="node"></param>
        /// <param name="txtValue"></param>
        /// <returns></returns>
        public XmlNode SelectChildNodeByText(XmlNode node, string txtValue)
        {
            foreach (XmlNode cNode in node.ChildNodes)
            {
                if (cNode.Name.Equals(txtValue)) return cNode;
            }
            return null;
        }

        /// <summary>查找子集中对应名称及值的节点</summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public XmlNode SelectChildNodeByAttrValue(XmlNode node, string attrName, string attrValue)
        {
            foreach (XmlNode cNode in node.ChildNodes)
            {
                if (GetAttributeValue(cNode, attrName).Equals(attrValue)) return cNode;
            }
            return null;
        }

        /// <summary>统计子集中对应名称及值的节点</summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public int SelectCountByAttrValue(XmlNode node, string attrName, string attrValue)
        {
            int num = 0;
            foreach (XmlNode cNode in node.ChildNodes)
            {
                if (GetAttributeValue(cNode, attrName).Equals(attrValue))
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>查找子集中对应名称及值的节点</summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public XmlNode SelectChildNodeContainAttrValue(XmlNode node, string attrName, string attrValue)
        {
            foreach (XmlNode cNode in node.ChildNodes)
            {
                if (GetAttributeValue(cNode, attrName).Contains(attrValue)) return cNode;
            }
            return null;
        }

        /// <summary>在指定节点新增注释</summary>
        /// <param name="ndIn">XML 节点</param>
        /// <param name="newComment">节点注释</param>
        /// <returns></returns>
        public XmlNode AppendNodeComment(XmlNode ndIn, string newComment)
        {
            if (ndIn == null) return null;
            // 添加节点注释
            return ndIn.AppendChild(_xmlDoc.CreateComment(newComment));
        }

        /// <summary>在指定节点新增注释</summary>
        /// <param name="xmlPath">XML 节点路径</param>
        /// <param name="newComment">节点注释</param>
        /// <returns></returns>
        public XmlNode AppendNodeComment(string xmlPath, string newComment)
        {
            XmlNode nd = _xmlDoc.SelectSingleNode(xmlPath);
            // 在指定节点新增注释
            return AppendNodeComment(nd, newComment);
        }

        /// <summary>删除节点</summary>
        /// <param name="xmlPath">XML 节点路径</param>
        public void DeleteNode(string xmlPath)
        {
            XmlNode nd = _xmlDoc.SelectSingleNode(xmlPath);
            if (nd == null) return;

            // 是否具有父节点
            if (nd.ParentNode == null)
            {
                // 清空当前节点
                nd.RemoveAll();
            }
            else // 删除当前节点
            {
                nd.ParentNode.RemoveChild(nd);
            }
        }

        /// <summary>删除节点</summary>
        /// <param name="xmlPath">XML 节点路径</param>
        public void DeleteNode(string xmlPath, string attrName, string attrValue)
        {
            XmlNode nd = SelectNode(xmlPath, attrName, attrValue);
            if (nd == null) return;

            // 是否具有父节点
            if (nd.ParentNode == null)
            {
                // 清空当前节点
                nd.RemoveAll();
            }
            else // 删除当前节点
            {
                nd.ParentNode.RemoveChild(nd);
            }
        }

        /// <summary>删除节点</summary>
        /// <param name="pNode"></param>
        /// <param name="xmlPath"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        public void DeleteNode(XmlNode pNode, string xmlPath, string attrName, string attrValue)
        {
            XmlNodeList nodeLst = pNode.SelectNodes(xmlPath);
            int nIndex = -1;
            for (int i = 0; i < nodeLst.Count; i++)
            {
                if (GetAttributeValue(nodeLst[i], attrName).Trim().Equals(attrValue))
                {
                    nIndex = i;
                    break;
                }
            }
            if (nIndex != -1) pNode.RemoveChild(pNode.ChildNodes[nIndex]);
        }

        /// <summary>删除子集</summary>
        /// <param name="pNode"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public void DeleteChildNode(XmlNode pNode, string attrName, string attrValue)
        {
            XmlNodeList nodeLst = pNode.ChildNodes;
            int nIndex = -1;
            for (int i = 0; i < nodeLst.Count; i++)
            {
                if (GetAttributeValue(nodeLst[i], attrName).Trim().Equals(attrValue))
                {
                    nIndex = i;
                    break;
                }
            }
            if (nIndex != -1) pNode.RemoveChild(pNode.ChildNodes[nIndex]);
        }

        /// <summary>获取节点值</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueConfigByKey(string key)
        {
            try
            {
                XmlNode node = _xmlDoc.SelectSingleNode("//add[@key='" + key + "']");
                if (node != null) return GetAttributeValue(node, "value");

                return "";
            }
            catch
            {
                return "";
            }
        }
      
        /// <summary>更新节点值</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool SetValueConfigByKey(string key, string value)
        {
            try
            {
                XmlNode node = _xmlDoc.SelectSingleNode("//add[@key='" + key + "']");
                if (node == null) return false;
                //设置属性值
                SetAttributeValue(node, "value", value);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region "文件操作"

        /// <summary>加载 XML 文件</summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回加载是否成功</returns>
        public bool LoadXMLFile(string filePath)
        {
            // XML 文件是否存在
            if (!File.Exists(filePath)) return false;

            // 设置默认路径
            _filePath = filePath;
            // 清空 XmlDocument
            _xmlDoc.RemoveAll();

            try
            {
                _xmlDoc.Load(filePath);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }

        /// <summary>加载 XMD 文件</summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回加载是否成功</returns>
        public bool LoadXMDFile(string filePath)
        {
            // XMD 文件是否存在
            if (!File.Exists(filePath)) return false;

            // 设置默认路径
            _filePath = filePath;
            // 清空 XmlDocument
            _xmlDoc.RemoveAll();

            try
            {
                _xmdMgr.LoadXMDFile(filePath);
                _xmlDoc = _xmdMgr.XMLDoc;

                return true;
            }
            catch
            {
                return false;
            }
        }




        /// <summary>加载xml字符串</summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public bool LoadXMLStr(string xmlData)
        {
            // XML数据是否存在
            if (xmlData.Trim().Equals(string.Empty)) return false;

            // 清空 XmlDocument
            _xmlDoc.RemoveAll();

            try
            {
                _xmlDoc.LoadXml(xmlData);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>保存 XML 文件</summary>
        /// <returns>返回保存是否成功</returns>
        public bool SaveXMLFile()
        {
            return SaveXMLFile(_filePath);
        }

        /// <summary>保存 XMD 文件</summary>
        /// <returns>返回保存是否成功</returns>
        public bool SaveXMDFile()
        {
            return SaveXMDFile(_filePath);
        }

        /// <summary>保存 XML 文件</summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回保存是否成功</returns>
        public bool SaveXMLFile(string filePath)
        {
            if (filePath == "") return false;

            // XML 文档是否为空
            if (_xmlDoc.DocumentElement == null) return false;

            if (File.Exists(filePath))
            {
                FileInfo xmlFile = new FileInfo(filePath);
                // 解除文件只读属性
                if (xmlFile.IsReadOnly) xmlFile.IsReadOnly = false;
            }

            string directory = filePath.Substring(0, filePath.LastIndexOf("\\"));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _filePath = filePath;

            XMLDoc.Save(filePath);

            return true;
        }

        /// <summary>保存 XMD 文件</summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回保存是否成功</returns>
        public bool SaveXMDFile(string filePath)
        {
            if (filePath == "") return false;

            // XMD 文档是否为空
            if (_xmlDoc.DocumentElement == null) return false;

            if (File.Exists(filePath))
            {
                FileInfo xmlFile = new FileInfo(filePath);
                // 解除文件只读属性
                if (xmlFile.IsReadOnly) xmlFile.IsReadOnly = false;
            }

            _xmdMgr.XMLDoc = _xmlDoc;
            _xmdMgr.SaveXMDFile(filePath);

            return true;
        }

        /// <summary>通过 XML 文件创建 DataSet</summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public DataSet CreateDataSetByXmlFile(string filePath)
        {
            // XML 文件是否存在
            if (!File.Exists(filePath)) return null;

            try
            {
                DataSet ds = new DataSet();
                // 读取 XML 文件
                ds.ReadXml(filePath);

                return ds;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>XmlDocument 对象</summary>
        public XmlDocument XMLDoc
        {
            get { return _xmlDoc; }
            set { _xmlDoc = value; }
        }

        /// <summary>文件路径</summary>
        public string FilePath
        {
            get { return _filePath; }
        }

        #endregion

        /// <summary>
        /// 创建加载xml文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="rootNodeName">根节点名称</param>
        public void loadAndCreateXmlFile(string filePath, string rootNodeName)
        {
            // XML 文件是否存在
            if (!File.Exists(filePath))
            {
                //不存在目录，则创建目录
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }
                InitXMLRoot(rootNodeName);
                SaveXMLFile(filePath);
            }

            // 设置默认路径
            _filePath = filePath;
            // 清空 XmlDocument
            _xmlDoc.RemoveAll();

            try
            {
                _xmlDoc.Load(filePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
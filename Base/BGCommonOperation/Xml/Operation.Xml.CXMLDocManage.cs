//------------------------------------------
// 对 XMD 文件的操作
// Create by COM.J -- 2008.09.09
// Finished by COM.J -- 2008.09.10
//------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
namespace CMW.Common.Utilities
{
    public class CXMLDocManage
    {
        // xml文件对象
        XmlDocument m_xmlDoc;
        // 文件读取流对象
        CDocReadStream m_docRead;
        // 文件写入流对象
        CDocWriteStream m_docWrite;

        /// <summary>XMD 文件管理类 构造函数</summary>
        public CXMLDocManage()
        {
            m_xmlDoc = new XmlDocument();
        }

        ~CXMLDocManage()
        {
            m_xmlDoc.RemoveAll();
            m_xmlDoc = null;
        }

        /// <summary>加载 XML 文件</summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回加载是否成功</returns>
        public bool LoadXMLFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;
            // 清空 XmlDocument
            m_xmlDoc.RemoveAll();

            try
            {
                m_xmlDoc.Load(filePath);
                return true;
            }
            catch (Exception)
            {
                // 防止重复处理文件（递规死循环）
                if (filePath.EndsWith(".temp")) return false;

                CDocWriteXMLStream docWriteXML = new CDocWriteXMLStream(filePath);
                // 格式化 XML 标准内容
                docWriteXML.FormatXMLStandard();
                // 是否需要更新 XML 文件
                if (!docWriteXML.IsUpdate) return false;

                docWriteXML.Save(filePath + ".temp");
                return LoadXMLFile(filePath + ".temp");
            }
        }

        /// <summary>加载二进制 XMD 文件</summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回加载是否成功</returns>
        public bool LoadXMDFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;
            // 清空 XmlDocument
            m_xmlDoc.RemoveAll();

            m_docRead = new CDocReadStream(filePath);
            return XmdToMsXml();
        }

        /// <summary>保存 XML 文件</summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回保存是否成功</returns>
        public bool SaveXMLFile(string filePath)
        {
            if (m_xmlDoc == null) return false;
            if (m_xmlDoc.ChildNodes.Count <= 0) return false;

            m_xmlDoc.Save(filePath);
            return true;
        }

        /// <summary>保存 XMD 文件</summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回保存是否成功</returns>
        public bool SaveXMDFile(string filePath)
        {
            if (m_xmlDoc == null) return false;
            if (m_xmlDoc.ChildNodes.Count <= 0) return false;

            m_docWrite = new CDocWriteStream();

            if (!MsXmlToXmd()) return false;
            return m_docWrite.Save(filePath);
        }

        /// <summary>属性：MS 的 XML 结构</summary>
        public XmlDocument XMLDoc
        {
            get { return m_xmlDoc; }
            set { m_xmlDoc = value; }
        }

        /// <summary>把二进制 XMD 文件转换成 MS 的 XML 结构</summary>
        /// <returns></returns>
        bool XmdToMsXml()
        {
            try
            {
                string strNode, strText;
                XmlElement xmlElem;

                // 创建 XML 声明
                m_xmlDoc.CreateXmlDeclaration("1.0", "UTF-16", "yes");
                // 跳过文件头
                m_docRead.ReadString();

                // 读取根节点
                strNode = FormatNodeName(m_docRead.ReadString());
                strText = m_docRead.ReadString();
                xmlElem = m_xmlDoc.CreateElement("", strNode, "");
                // if (strText.Equals("") == false)
                // {
                //     xmlTxt = m_xmlDoc.CreateTextNode(strText);
                //     xmlElem.AppendChild(xmlTxt);
                // }

                // 创建根节点
                m_xmlDoc.AppendChild(xmlElem);

                return ParseXmdElement(ref xmlElem);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>解析二进制 XMD 数据节点</summary>
        /// <param name="outXmlElem"></param>
        /// <returns></returns>
        bool ParseXmdElement(ref XmlElement outXmlElem)
        {
            string strNode, strText;
            int i, iCount;
            XmlElement xmlElem;
            XmlText xmlTxt;
            XmlAttribute xmlAttr;
            bool bRe = true;

            // 读取属性
            iCount = m_docRead.ReadLong();
            for (i = 0; i < iCount; i++)
            {
                // 读取属性
                try
                {
                    strNode = FormatNodeName(m_docRead.ReadString());
                    strText = m_docRead.ReadString();

                    // 
                    if (strNode.Trim().Length == 0|| strNode .Trim().Equals("#text"))
                    {
                        continue;
                    }

                    // 
                    // try
                    // {
                    xmlAttr = m_xmlDoc.CreateAttribute(strNode);
                    if (strText.Equals("") == false)
                    {
                        xmlTxt = m_xmlDoc.CreateTextNode(strText);
                        // }
                        // catch (XmlException)
                        // {
                        //     continue;
                        // }

                        // 创建属性
                        xmlAttr.AppendChild(xmlTxt);
                    }
                    outXmlElem.Attributes.Append(xmlAttr);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            // 读取子节点
            iCount = m_docRead.ReadLong();
            for (i = 0; i < iCount; i++)
            {
                // 读取子节点
                strNode = FormatNodeName(m_docRead.ReadString());
                strText = m_docRead.ReadString();

                // 林翔修改于2009.03.23，由于广西配置的标准_其他费用表没有这个退出就存在报错
                if (strNode.Trim().Length == 0 || strNode.Trim().Equals("#text"))
                {
                    continue;
                }

                xmlElem = m_xmlDoc.CreateElement(strNode);
                if (strText.Equals("") == false)
                {
                    xmlTxt = m_xmlDoc.CreateTextNode(strText);

                    // 创建子节点
                    xmlElem.AppendChild(xmlTxt);
                }
                outXmlElem.AppendChild(xmlElem);

                bRe = (bRe && ParseXmdElement(ref xmlElem));
            }

            return bRe;
        }

        // 把 MS 的 XML 结构转换成二进制 XMD 文件 
        bool MsXmlToXmd()
        {
            // 写入文件头
            m_docWrite.WriteString("<XMD 1.0>");

            // 写入根节点
            m_docWrite.WriteString(m_xmlDoc.DocumentElement.Name);
            m_docWrite.WriteString(m_xmlDoc.DocumentElement.InnerText.Trim());

            return WriteXmdElement(m_xmlDoc.DocumentElement);
        }

        // 写入二进制 XMD 数据节点
        bool WriteXmdElement(XmlNode xmlIn)
        {
            int i, iCount;
            XmlNode xml;
            bool bRe = true;
            if (xmlIn.Attributes != null)
            {
                // 写入属性长度
                iCount = xmlIn.Attributes.Count;
                m_docWrite.WriteLong(iCount);

                for (i = 0; i < iCount; i++)
                {
                    xml = xmlIn.Attributes[i];
                    // 写入属性
                    m_docWrite.WriteString(xml.Name);
                    m_docWrite.WriteString(xml.InnerText.Trim());
                }
            }

            // 写入子节点长度
            iCount = xmlIn.ChildNodes.Count;
            m_docWrite.WriteLong(iCount);

            for (i = 0; i < iCount; i++)
            {
                xml = xmlIn.ChildNodes[i];
                // 写入子节点
                m_docWrite.WriteString(xml.Name);
                m_docWrite.WriteString(xml.InnerText.Trim());

                bRe = (bRe && WriteXmdElement(xml));
            }

            return bRe;
        }

        // 格式化 XML 节点名称
        string FormatNodeName(string sIn)
        {
            string strRe = sIn.Trim().TrimStart("(".ToCharArray()).TrimEnd(
                ")".ToCharArray()).Replace("(", ".").Replace(")", ".");
            strRe = strRe.TrimStart("[".ToCharArray()).TrimEnd(
                "]".ToCharArray()).Replace("[", ".").Replace("]", ".");
            strRe = strRe.TrimStart("（".ToCharArray()).TrimEnd(
                "）".ToCharArray()).Replace("（", ".").Replace("）", ".");
            strRe = strRe.Replace("%", ".");
            strRe = strRe.Replace("、", ".");

            // 林翔于2009.04.02日修改，由于在第一个字符存在"."会报错，所以过滤到第一个字符为"."的
            // 林翔于2009.04.02日修改，由于在第一个字符存在数据会报错，所以增加了固定的前缀
            // if (strRe.Trim().Length > 0)
            // {
            //     if ((strRe.Substring(0, 1) == "0") ||
            //        (strRe.Substring(0, 1) == "1") ||
            //        (strRe.Substring(0, 1) == "2") ||
            //        (strRe.Substring(0, 1) == "3") ||
            //        (strRe.Substring(0, 1) == "4") ||
            //        (strRe.Substring(0, 1) == "5") ||
            //        (strRe.Substring(0, 1) == "6") ||
            //        (strRe.Substring(0, 1) == "7") ||
            //        (strRe.Substring(0, 1) == "8") ||
            //        (strRe.Substring(0, 1) == "9") ||
            //        (strRe.Substring(0, 1) == "."))
            //     {
            //         strRe = "Failed" + strRe;
            //     }
            // }

            return strRe;
        }
    }

    //========================================================================

    class CDocReadStream
    {
        byte[] m_fs;

        int m_iIndex;

        /// <summary>XMD 文件流管理类 构造函数</summary>
        /// <param name="filePath">文件路径</param>
        public CDocReadStream(string filePath)
        {
            if (File.Exists(filePath)) m_fs = File.ReadAllBytes(filePath);
            m_iIndex = 0;
        }

        // 读取字符串
        public string ReadString()
        {
            if (m_fs == null) return "";

            CStreamDefine.StringStreamHead head = new CStreamDefine.StringStreamHead();
            ReadStreamHead(ref head);

            if (((m_iIndex + head.nByteLength) >= m_fs.Length) ||
                (head.nByteLength <= 0)) return "";

            // 读字符串
            string strRe = "";
            if (head.bUnicode == 0)
            {
                strRe = Encoding.ASCII.GetString(m_fs, m_iIndex, head.nByteLength);
            }
            else
            {
                strRe = Encoding.Unicode.GetString(m_fs, m_iIndex, head.nByteLength);
            }
            strRe = strRe.TrimEnd(("\0").ToCharArray());

            m_iIndex += head.nByteLength;
            return strRe;
        }

        // 读取内容结构标识
        void ReadStreamHead(ref CStreamDefine.StringStreamHead outHead)
        {
            if (m_fs == null) return;
            if ((m_iIndex + CStreamDefine.StringStreamHeadLength) >= m_fs.Length) return;

            outHead.bUnicode = Convert.ToUInt16((m_fs[m_iIndex + 1] * CStreamDefine.CARRY_BYTE) +
                m_fs[m_iIndex]);
            outHead.nByteLength = Convert.ToUInt16((m_fs[m_iIndex + 3] * CStreamDefine.CARRY_BYTE) +
                m_fs[m_iIndex + 2]);

            m_iIndex += CStreamDefine.StringStreamHeadLength;
        }

        // 读取整型
        public int ReadLong()
        {
            if (m_fs == null) return -1;
            if ((m_iIndex + CStreamDefine.INT32_LENGTH) >= m_fs.Length) return -1;

            int iRe = (m_fs[m_iIndex + 3] * CStreamDefine.CARRY_BYTE * CStreamDefine.CARRY_BYTE * CStreamDefine.CARRY_BYTE) +
                (m_fs[m_iIndex + 2] * CStreamDefine.CARRY_BYTE * CStreamDefine.CARRY_BYTE) +
                (m_fs[m_iIndex + 1] * CStreamDefine.CARRY_BYTE) + m_fs[m_iIndex];

            m_iIndex += CStreamDefine.INT32_LENGTH;
            return iRe;
        }
    }

    //========================================================================

    /// <summary>文件写入流</summary>
    class CDocWriteStream
    {
        protected byte[] m_fs = new byte[CStreamDefine.MEMORY_BYTE_LENGTH];

        int m_iIndex;

        CByteMemoryManage m_btManage;

        /// <summary>XMD 文件流管理类 构造函数</summary>
        public CDocWriteStream()
        {
            m_iIndex = 0;
            m_btManage = new CByteMemoryManage();
        }

        /// <summary>保存 XMD 文件</summary>
        /// <param name="filePath">文件路径</param>
        public bool LoadFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;
            m_fs = File.ReadAllBytes(filePath);

            m_iIndex = m_fs.Length;
            return true;
        }

        /// <summary>保存 XMD 文件</summary>
        /// <param name="filePath">文件路径</param>
        public bool Save(string filePath)
        {
            if (m_btManage.ApplyByteMemory(ref m_fs, m_iIndex) < 0) return false;

            File.WriteAllBytes(filePath, m_fs);
            return true;
        }

        /// <summary>写入字符串</summary>
        /// <param name="sIn"></param>
        /// <returns></returns>
        public bool WriteString(string sIn)
        {
            string str = "";
            if (sIn != null) str = sIn;

            byte[] bt = Encoding.Unicode.GetBytes(str);
            int iCount = bt.Length;

            CStreamDefine.StringStreamHead head = new CStreamDefine.StringStreamHead();
            head.bUnicode = 1;
            head.nByteLength = Convert.ToUInt16(iCount + 2);

            // 写入内容结构标识
            if (!WriteStreamHead(head)) return false;
            // 动态分配数据流内存
            if (m_btManage.AllocByteMemory(ref m_fs,
                (m_iIndex + head.nByteLength)) < 0) return false;

            if (iCount > 0)
            {
                bt.CopyTo(m_fs, m_iIndex);
                m_iIndex += iCount;
            }

            // 写入字符串结束符"\0"
            m_fs[m_iIndex] = 0;
            m_iIndex++;
            m_fs[m_iIndex] = 0;
            m_iIndex++;

            return true;
        }

        /// <summary>写入内容结构标识</summary>
        /// <param name="headIn"></param>
        /// <returns></returns>
        bool WriteStreamHead(CStreamDefine.StringStreamHead headIn)
        {
            // 动态分配数据流内存
            if (m_btManage.AllocByteMemory(ref m_fs,
                (m_iIndex + CStreamDefine.StringStreamHeadLength)) < 0) return false;

            int iByte, iValue = headIn.bUnicode, intIndex = m_iIndex;
            // 分配 bUnicode
            while (iValue > 0)
            {
                iByte = iValue % CStreamDefine.CARRY_BYTE;

                m_fs[intIndex] = Convert.ToByte(iByte);
                intIndex++;

                iValue = (iValue - iByte) / CStreamDefine.CARRY_BYTE;
            }

            iValue = headIn.nByteLength;
            intIndex = m_iIndex + CStreamDefine.INT16_LENGTH;
            // 分配 nByteLength
            while (iValue > 0)
            {
                iByte = iValue % CStreamDefine.CARRY_BYTE;

                m_fs[intIndex] = Convert.ToByte(iByte);
                intIndex++;

                iValue = (iValue - iByte) / CStreamDefine.CARRY_BYTE;
            }

            m_iIndex += CStreamDefine.StringStreamHeadLength;
            return true;
        }

        /// <summary>写入整型</summary>
        /// <param name="vIn"></param>
        /// <returns></returns>
        public bool WriteLong(int vIn)
        {
            // 动态分配数据流内存
            if (m_btManage.AllocByteMemory(ref m_fs,
                (m_iIndex + CStreamDefine.INT32_LENGTH)) < 0) return false;

            int iByte, iValue = vIn, intIndex = m_iIndex;

            while (iValue > 0)
            {
                iByte = iValue % CStreamDefine.CARRY_BYTE;

                m_fs[intIndex] = Convert.ToByte(iByte);
                intIndex++;

                iValue = (iValue - iByte) / CStreamDefine.CARRY_BYTE;
            }

            m_iIndex += CStreamDefine.INT32_LENGTH;
            return true;
        }
    }

    //========================================================================

    /// <summary>Xml文件写入流</summary>
    class CDocWriteXMLStream : CDocWriteStream
    {
        bool m_bUpdate;

        /// <summary>XML 文件流管理类 构造函数</summary>
        /// <param name="filePath">文件路径</param>
        public CDocWriteXMLStream(string filePath)
        {
            if (File.Exists(filePath)) LoadFile(filePath);
        }

        /// <summary>属性：是否需要更新 XML 文件</summary>
        public bool IsUpdate
        {
            get { return m_bUpdate; }
        }

        /// <summary>格式化 XML 标准内容</summary>
        public void FormatXMLStandard()
        {
            m_bUpdate = false;
            // 检查 XML 文件流是否使用 Unicode 编码
            if (CheckXMLUnicode()) FormatXMLStandardUnicode();
            else FormatXMLStandardByte();
        }

        /// <summary>检查 XML 文件流是否使用 Unicode 编码</summary>
        /// <returns></returns>
        bool CheckXMLUnicode()
        {
            int i, iLen = m_fs.Length;
            if (iLen < 100) return false;

            int iTemp = 0;
            for (i = 0; i < 100; i++)
            {
                if (m_fs[i] == 0) iTemp++;
            }

            if (iTemp > 10) return true;
            else return false;
        }

        /// <summary>格式化 XML 标准内容</summary>
        void FormatXMLStandardUnicode()
        {
            byte btOpr = 0;
            int i, iLen = m_fs.Length;
            int iTemp;
            bool bUpdatePrintInfo = true;

            for (i = 0; i < iLen; i += 2)
            {
                if (btOpr == 0)
                {
                    if ((m_fs[i] == 60) && (m_fs[i + 1] == 0)) btOpr = m_fs[i]; // "<"
                }
                else if (btOpr == 60)
                {
                    if (!((m_fs[i] == 47) && (m_fs[i + 1] == 0))) // != "/"
                    {
                        if ((bUpdatePrintInfo) && ((i + 17) < iLen))
                        {
                            // 容错 -- 处理非法的 "PrintInfo" 内容
                            if ((m_fs[i] == 80) && (m_fs[i + 1] == 0) && (m_fs[i + 2] == 114)
                                && (m_fs[i + 3] == 0) && (m_fs[i + 4] == 105) && (m_fs[i + 5] == 0)
                                && (m_fs[i + 6] == 110) && (m_fs[i + 7] == 0) && (m_fs[i + 8] == 116)
                                && (m_fs[i + 9] == 0) && (m_fs[i + 10] == 73) && (m_fs[i + 11] == 0)
                                && (m_fs[i + 12] == 110) && (m_fs[i + 13] == 0) && (m_fs[i + 14] == 102)
                                && (m_fs[i + 15] == 0) && (m_fs[i + 16] == 111) && (m_fs[i + 17] == 0))
                            {
                                i += 18;
                                while (i < iLen)
                                {
                                    if (((m_fs[i] == 62) || (m_fs[i] == 47)) && (m_fs[i + 1] == 0)) break; // ">" || "/"
                                    // 循环清空可能的异常数据
                                    m_fs[i] = 32; m_fs[i + 1] = 0; i += 2;
                                }
                                // 标记处理 "PrintInfo" 完成
                                bUpdatePrintInfo = false;
                            }
                        }
                        // 格式化节点内容
                        FormatNodeContentUnicode(ref i, ref btOpr);
                    }
                }
                else if (((m_fs[i] == 62) || (m_fs[i] == 47)) && (m_fs[i + 1] == 0)) // ">" || "/"
                {
                    btOpr = 0;
                }
                else if (btOpr == 32) // " "
                {
                    if ((m_fs[i] == 61) && (m_fs[i + 1] == 0)) // "="
                    {
                        btOpr = m_fs[i];
                    }
                    else if (!((m_fs[i] == 32) && (m_fs[i + 1] == 0))) // != " "
                    {
                        // 格式化节点内容
                        FormatNodeContentUnicode(ref i, ref btOpr);
                    }
                }
                else if (btOpr == 61) // "="
                {
                    iTemp = 0;
                    while (i < iLen)
                    {
                        if ((m_fs[i] == 34) && (m_fs[i + 1] == 0)) iTemp++; // "\""
                        // 跳过两个引号范围
                        if (iTemp > 1) break;
                        i += 2;
                    }
                    btOpr = 32; // " "
                }
            }
        }

        /// <summary>格式化节点内容</summary>
        /// <param name="iStart"></param>
        /// <param name="btIn"></param>
        void FormatNodeContentUnicode(ref int iStart, ref byte btIn)
        {
            int i = iStart, iLen = m_fs.Length;
            byte btOpr = btIn;

            while (i < iLen)
            {
                if (((m_fs[i] == 62) || (m_fs[i] == 47)) && (m_fs[i + 1] == 0)) // ">" || "/"
                {
                    btIn = 0; break;
                }
                else if (((m_fs[i] == 32) || (m_fs[i] == 61)) && (m_fs[i + 1] == 0)) // " " || "="
                {
                    btIn = m_fs[i]; break;
                }
                else if (i == iStart)
                {
                    if ((m_fs[i] >= 48) && (m_fs[i] <= 57) && (m_fs[i + 1] == 0)) // >= "0" && <= "9"
                    {
                        m_fs[i] = 90; // "Z"
                    }
                }
                else
                {
                    // 格式化字节内容
                    FormatXMLNodeByte(ref i, true);
                }

                i += 2;
            }

            iStart = i;
        }

        /// <summary>格式化 XML 标准内容</summary>
        void FormatXMLStandardByte()
        {
            byte btOpr = 0;
            int i, iLen = m_fs.Length;
            int iTemp;
            bool bUpdatePrintInfo = true;

            for (i = 0; i < iLen; i++)
            {
                if (btOpr == 0)
                {
                    if (m_fs[i] == 60) btOpr = m_fs[i]; // "<"
                }
                else if (btOpr == 60) // "<"
                {
                    if (m_fs[i] != 47) // != "/"
                    {
                        if ((bUpdatePrintInfo) && ((i + 8) < iLen))
                        {
                            // 容错 -- 处理非法的 "PrintInfo" 内容
                            if ((m_fs[i] == 80) && (m_fs[i + 1] == 114) && (m_fs[i + 2] == 105)
                                && (m_fs[i + 3] == 110) && (m_fs[i + 4] == 116) && (m_fs[i + 5] == 73)
                                && (m_fs[i + 6] == 110) && (m_fs[i + 7] == 102) && (m_fs[i + 8] == 111))
                            {
                                i += 9;
                                while (i < iLen)
                                {
                                    if ((m_fs[i] == 62) || (m_fs[i] == 47)) break; // ">" || "/"
                                    // 循环清空可能的异常数据
                                    m_fs[i] = 32; i++;
                                }
                                // 标记处理 "PrintInfo" 完成
                                bUpdatePrintInfo = false;
                            }
                        }
                        // 格式化节点内容
                        FormatNodeContentByte(ref i, ref btOpr);
                    }
                }
                else if ((m_fs[i] == 62) || (m_fs[i] == 47)) // ">" || "/"
                {
                    btOpr = 0;
                }
                else if (btOpr == 32) // " "
                {
                    if (m_fs[i] == 61) // "="
                    {
                        btOpr = m_fs[i];
                    }
                    else if (m_fs[i] != 32) // != " "
                    {
                        // 格式化节点内容
                        FormatNodeContentByte(ref i, ref btOpr);
                    }
                }
                else if (btOpr == 61) // "="
                {
                    iTemp = 0;
                    while (i < iLen)
                    {
                        if (m_fs[i] == 34) iTemp++; // "\""
                        // 跳过两个引号范围
                        if (iTemp > 1) break;
                        i++;
                    }
                    btOpr = 32; // " "
                }
            }
        }

        /// <summary>格式化节点内容</summary>
        /// <param name="iStart"></param>
        /// <param name="btIn"></param>
        void FormatNodeContentByte(ref int iStart, ref byte btIn)
        {
            int i = iStart, iLen = m_fs.Length;
            byte btOpr = btIn;

            while (i < iLen)
            {
                if ((m_fs[i] == 62) || (m_fs[i] == 47)) // ">" || "/"
                {
                    btIn = 0; break;
                }
                else if ((m_fs[i] == 32) || (m_fs[i] == 61)) // " " || "="
                {
                    btIn = m_fs[i]; break;
                }
                else if (i == iStart)
                {
                    if ((m_fs[i] >= 48) && (m_fs[i] <= 57)) // >= "0" && <= "9"
                    {
                        m_fs[i] = 90; // "Z"
                    }
                }
                else
                {
                    // 格式化字节内容
                    FormatXMLNodeByte(ref i, false);
                }

                i++;
            }

            iStart = i;
        }

        /// <summary>格式化字节内容</summary>
        /// <param name="iIndex"></param>
        /// <param name="bUnicode"></param>
        void FormatXMLNodeByte(ref int iIndex, bool bUnicode)
        {
            if (bUnicode)
            {
                if (((m_fs[iIndex] == 58) || (m_fs[iIndex] == 40) || (m_fs[iIndex] == 41)
                    || (m_fs[iIndex] == 91) || (m_fs[iIndex] == 93) || (m_fs[iIndex] == 37))
                    && (m_fs[iIndex + 1] == 0)) // ":" || "(" || ")" || "[" || "]" || "%"
                {
                    m_fs[iIndex] = 46; // "."
                    m_bUpdate = true; return;
                }
            }
            else if ((m_fs[iIndex] == 58) || (m_fs[iIndex] == 40) || (m_fs[iIndex] == 41)
                    || (m_fs[iIndex] == 91) || (m_fs[iIndex] == 93) || (m_fs[iIndex] == 37))
            {
                m_fs[iIndex] = 46; // "."
                m_bUpdate = true; return;
            }

            // 是否中文字符
            if (m_fs[iIndex + 1] == 0) return;

            if ((iIndex + 1) < m_fs.Length)
            {
                bool bl = false;
                byte[] bt = Encoding.Unicode.GetBytes("（");
                if ((m_fs[iIndex] == bt[0]) && (m_fs[iIndex + 1] == bt[1])) bl = true;

                if (!bl)
                {
                    bt = Encoding.Unicode.GetBytes("）");
                    if ((m_fs[iIndex] == bt[0]) && (m_fs[iIndex + 1] == bt[1])) bl = true;
                }
                if (!bl)
                {
                    bt = Encoding.Unicode.GetBytes("、");
                    if ((m_fs[iIndex] == bt[0]) && (m_fs[iIndex + 1] == bt[1])) bl = true;
                }

                if (bl)
                {
                    m_fs[iIndex] = 46; m_fs[iIndex + 1] = 46; // "."
                    m_bUpdate = true;

                    if (!bUnicode) iIndex++;
                }
            }
        }
    }

    //========================================================================

    class CByteMemoryManage
    {
        byte[] m_fsCopy = new byte[CStreamDefine.MEMORY_BYTE_LENGTH];

        // 动态分配数据流内存
        public int AllocByteMemory(ref byte[] outByte, int newLen)
        {
            int iRe = -1, iLen = outByte.Length;

            if (iLen < newLen)
            {
                if (m_fsCopy.Length < iLen) m_fsCopy = new byte[iLen];
                if (m_fsCopy != null)
                {
                    outByte.CopyTo(m_fsCopy, 0);
                    // 每次扩展固定空间内存，防止频繁开内存操作
                    iLen += CStreamDefine.MEMORY_BYTE_LENGTH;

                    outByte = new byte[iLen];
                    if (outByte != null)
                    {
                        m_fsCopy.CopyTo(outByte, 0);
                        iRe = iLen;
                    }
                }
            }
            else
            {
                iRe = iLen;
            }

            return iRe;
        }

        // 应用数据流内存 -- 清除冗余的内存
        public int ApplyByteMemory(ref byte[] outByte, int newLen)
        {
            if (outByte == null) return -1;
            if (newLen <= 0) return -1;

            int iLen = outByte.Length;
            if (newLen > iLen) return -1;
            // 已经应用
            if (newLen == iLen) return newLen;

            int i, iRe = -1;
            if (m_fsCopy.Length < iLen) m_fsCopy = new byte[iLen];

            if (m_fsCopy != null)
            {
                outByte.CopyTo(m_fsCopy, 0);
                outByte = new byte[newLen];

                if (outByte != null)
                {
                    for (i = 0; i < newLen; i++)
                    {
                        outByte[i] = m_fsCopy[i];
                    }
                    iRe = newLen;
                }
            }

            return iRe;
        }
    }

    //========================================================================

    static class CStreamDefine
    {
        public const int MEMORY_BYTE_LENGTH = 256 * 1024;
        public const int CARRY_BYTE = 256;

        public const int INT32_LENGTH = 4;
        public const int INT16_LENGTH = 2;

        public struct StringStreamHead
        {
            // 是否是双字节字符串（TRUE=1, FALSE=0）
            public ushort bUnicode;
            // 字符串长度（包括结束符0）
            public ushort nByteLength; 
        };
        public const int StringStreamHeadLength = 4;

        public const int DATASET_VERSION = 3;
    }
}

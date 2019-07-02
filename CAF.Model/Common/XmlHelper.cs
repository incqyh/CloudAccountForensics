using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CAF.Model.Common
{
    /// <summary>
    /// 该类将当前程序中的数据导出到xml文件中保存
    /// </summary>
    public class XmlHelper
    {
        static public void SaveContact(List<Contact> contacts)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", "");
            xmldoc.AppendChild(xmldecl);
            //InfoTable
            XmlElement xmlele = xmldoc.CreateElement("InfoTable");
            xmlele.SetAttribute("ID", "2");
            xmldoc.AppendChild(xmlele);
            //Node
            XmlNode root = xmldoc.SelectSingleNode("InfoTable");
            xmlele = xmldoc.CreateElement("Node");
            xmlele.SetAttribute("vType", "string");
            xmlele.InnerText = "/基础信息/通讯录";
            root.AppendChild(xmlele);
            //TCol
            xmlele = xmldoc.CreateElement("TCol");
            //Prop
            XmlElement xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "integer");
            xmlsub.InnerText = "删除";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.SetAttribute("FieldTag", "2");
            xmlsub.InnerText = "照片";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "联系人";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "昵称";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "电话";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "职位";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "组别";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "邮箱";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "网络";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "地址";
            xmlele.AppendChild(xmlsub);
			xmlsub = xmldoc.CreateElement("Prop");
			xmlsub.SetAttribute("vType", "text");
			xmlsub.InnerText = "公司";
			xmlele.AppendChild(xmlsub);
			xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "备注";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "私人信息";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "DateTime");
            xmlsub.InnerText = "创建时间";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "DateTime");
            xmlsub.InnerText = "最后编辑时间";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "位置";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "integer");
            xmlsub.InnerText = "快速拨号";
            xmlele.AppendChild(xmlsub);
            //xmlsub = xmldoc.CreateElement("Prop");
            //xmlsub.SetAttribute("vType", "integer");
            //xmlsub.InnerText = "联系人ID";
            //xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "其他";
            xmlele.AppendChild(xmlsub);
            root.AppendChild(xmlele);
            for (int i = 0; i < contacts.Count; i++)
            {
                //Tln   Tln
                xmlele = xmldoc.CreateElement("Tln");
                //lV     lV
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                xmlsub.InnerText = "0";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.SetAttribute("FieldTag", "2");
                //xmlsub.InnerText = "照片";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = contacts[i].Name;
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "昵称";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                foreach (var j in contacts[i].PhoneNumber)
                    xmlsub.InnerText += j.Key + ":" + j.Value + " ";
				xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = contacts[i].Title; 
				xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                xmlsub.InnerText = string.Join(",", contacts[i].Group);
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "组别";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                foreach (var j in contacts[i].Email)
                    xmlsub.InnerText += j.Key + ":" + j.Value + " ";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "网络";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                foreach (var j in contacts[i].Address)
                    xmlsub.InnerText += j.Key + ":" + j.Value + " ";
                xmlele.AppendChild(xmlsub);
				xmlsub = xmldoc.CreateElement("Prop");
				//xmlsub.SetAttribute("vType", "text");
				xmlsub.InnerText = contacts[i].Company;
				xmlele.AppendChild(xmlsub);
				xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "备注";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "私人信息";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "Datetime");
                //xmlsub.InnerText = "创建时间";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "Datetime");
                //xmlsub.InnerText = "最后编辑时间";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "位置";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                xmlsub.InnerText = "0";
                xmlele.AppendChild(xmlsub);
                //xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                //xmlsub.InnerText = "联系人ID";
                //xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "其他";
                xmlele.AppendChild(xmlsub);
                root.AppendChild(xmlele);
            }
            xmldoc.Save(string.Format(@"{0}\Contact.xml", Setting.XmlFolder));
        }

        static public void SaveMessage(List<Message> messages)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", "");
            xmldoc.AppendChild(xmldecl);
            //InfoTable
            XmlElement xmlele = xmldoc.CreateElement("InfoTable");
            xmlele.SetAttribute("ID", "3");
            xmldoc.AppendChild(xmlele);
            //Node
            XmlNode root = xmldoc.SelectSingleNode("InfoTable");
            xmlele = xmldoc.CreateElement("Node");
            xmlele.SetAttribute("vType", "string");
            xmlele.InnerText = "/基础信息/短信";
            root.AppendChild(xmlele);
            //TCol
            xmlele = xmldoc.CreateElement("TCol");
            //Prop
            XmlElement xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "integer");
            xmlsub.InnerText = "删除";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "integer");
            xmlsub.InnerText = "已读";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "类型";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "文件夹";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "对方";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "对方号码";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "本机号码";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "内容";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "DateTime");
            xmlsub.InnerText = "时间";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.SetAttribute("FieldTag", "4");
            xmlsub.InnerText = "附件";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.SetAttribute("FieldTag", "5");
            xmlsub.InnerText = "附件拷贝";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "DateTime");
            xmlsub.InnerText = "服务中心标签时间";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "位置";
            xmlele.AppendChild(xmlsub);
            root.AppendChild(xmlele);

            for (int i = 0; i < messages.Count; i++)
            {
                //Tln
                xmlele = xmldoc.CreateElement("Tln");
                //lV
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                xmlsub.InnerText = "0";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                xmlsub.InnerText = "";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = "短信";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                if (messages[i].Direction == "接收")
                    xmlsub.InnerText = "收件箱";
                else
                    xmlsub.InnerText = "发件箱";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "对方";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = messages[i].PhoneNumber;
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "本机号码";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = messages[i].Content;
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "DateTime");
                xmlsub.InnerText = messages[i].MessageTime.ToString("G");
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.SetAttribute("FieldTag", "4");
                //xmlsub.InnerText = "附件";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.SetAttribute("FieldTag", "5");
                //xmlsub.InnerText = "附件拷贝";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "DateTime");
                //xmlsub.InnerText = "服务中心标签时间";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "位置";
                xmlele.AppendChild(xmlsub);
                root.AppendChild(xmlele);
            }
            xmldoc.Save(string.Format(@"{0}\Message.xml", Setting.XmlFolder));
        }

        static public void SavePicture(List<Picture> pictures)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", "");
            xmldoc.AppendChild(xmldecl);
            //InfoTable
            XmlElement xmlele = xmldoc.CreateElement("InfoTable");
            xmlele.SetAttribute("ID", "13");
            xmldoc.AppendChild(xmlele);
            //Node
            XmlNode root = xmldoc.SelectSingleNode("InfoTable");
            xmlele = xmldoc.CreateElement("Node");
            xmlele.SetAttribute("vType", "string");
            xmlele.InnerText = "/基础信息/照片和录像";
            root.AppendChild(xmlele);
            //TCol
            xmlele = xmldoc.CreateElement("TCol");
            //Prop
            XmlElement xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "integer");
            xmlsub.InnerText = "删除";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "照片名称";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "拍摄时间";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "经度";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "纬度";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "高度";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.SetAttribute("FieldTag", "2");
            xmlsub.InnerText = "路径";
            xmlele.AppendChild(xmlsub);
            root.AppendChild(xmlele);


            for (int i = 0; i < pictures.Count; i++)
            {
                //Tln 
                xmlele = xmldoc.CreateElement("Tln");
                //lV
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                xmlsub.InnerText = "0";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = pictures[i].Name;
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = pictures[i].Time.ToString();
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "经度";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "纬度";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "高度";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.SetAttribute("FieldTag", "2");
                xmlsub.InnerText = pictures[i].LocalUrl;
                xmlele.AppendChild(xmlsub);
                root.AppendChild(xmlele);
            }
            xmldoc.Save(string.Format(@"{0}\Picture.xml", Setting.XmlFolder));
        }

        static public void SaveCallRecord(List<CallRecord> callrecords)
        {

            XmlDocument xmldoc = new XmlDocument();
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", "");
            xmldoc.AppendChild(xmldecl);
            //InfoTable
            XmlElement xmlele = xmldoc.CreateElement("InfoTable");
            xmlele.SetAttribute("ID", "4");
            xmldoc.AppendChild(xmlele);
            //Node
            XmlNode root = xmldoc.SelectSingleNode("InfoTable");
            xmlele = xmldoc.CreateElement("Node");
            xmlele.SetAttribute("vType", "string");
            xmlele.InnerText = "/基础信息/通话记录";
            root.AppendChild(xmlele);
            //TCol
            xmlele = xmldoc.CreateElement("TCol");
            //Prop
            XmlElement xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "integer");
            xmlsub.InnerText = "删除";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "方向";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "类型";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "对方";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "对方号码";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "内容";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "描述";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "DateTime");
            xmlsub.InnerText = "时长";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "DateTime");
            xmlsub.InnerText = "时间戳";
            xmlele.AppendChild(xmlsub);
            //xmlsub = xmldoc.CreateElement("Prop");
            //xmlsub.SetAttribute("vType", "integer");
            //xmlsub.InnerText = "联系人ID";
            //xmlele.AppendChild(xmlsub);
            root.AppendChild(xmlele);

            for (int i = 0; i < callrecords.Count; i++)
            {
                //Tln
                xmlele = xmldoc.CreateElement("Tln");
                //lV
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                xmlsub.InnerText = "0";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = callrecords[i].Direction;
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "对方";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = callrecords[i].PhoneNumber;
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "内容";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                //xmlsub.InnerText = "描述";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "DateTime");
                xmlsub.InnerText = callrecords[i].LastTime.ToString();
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "DateTime");
                xmlsub.InnerText = callrecords[i].PhoneTime.ToString("G");
                xmlele.AppendChild(xmlsub);
                //xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                //xmlsub.InnerText = "联系人ID";
                //xmlele.AppendChild(xmlsub);
                root.AppendChild(xmlele);
            }
            xmldoc.Save(string.Format(@"{0}\CallRecord.xml", Setting.XmlFolder));
        }

        static public void SaveRecord(List<Record> records)
        {

            XmlDocument xmldoc = new XmlDocument();
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", "");
            xmldoc.AppendChild(xmldecl);
            //InfoTable
            XmlElement xmlele = xmldoc.CreateElement("InfoTable");
            xmlele.SetAttribute("ID", "14");
            xmldoc.AppendChild(xmlele);
            //Node
            XmlNode root = xmldoc.SelectSingleNode("InfoTable");
            xmlele = xmldoc.CreateElement("Node");
            xmlele.SetAttribute("vType", "string");
            xmlele.InnerText = "/基础信息/录音";
            root.AppendChild(xmlele);
            //TCol
            xmlele = xmldoc.CreateElement("TCol");
            //Prop
            XmlElement xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "integer");
            xmlsub.InnerText = "删除";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "名称";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "DateTime");
            xmlsub.InnerText = "创建时间";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "DateTime");
            xmlsub.InnerText = "修改时间";
            xmlele.AppendChild(xmlsub);
			xmlsub = xmldoc.CreateElement("Prop");
			xmlsub.SetAttribute("vType", "text");
			xmlsub.InnerText = "存储路径";
			xmlele.AppendChild(xmlsub);
			//xmlsub = xmldoc.CreateElement("Prop");
			//xmlsub.SetAttribute("vType", "integer");
			//xmlsub.InnerText = "ID";
			//xmlele.AppendChild(xmlsub);
			root.AppendChild(xmlele);

            for (int i = 0; i < records.Count; i++)
            {
                //Tln
                xmlele = xmldoc.CreateElement("Tln");
                //lV
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                xmlsub.InnerText = "0";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = records[i].Name;
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "DateTime");
                xmlsub.InnerText = records[i].CreateTime.ToString("G");
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "DateTime");
                xmlsub.InnerText = records[i].ModifyTime.ToString("G");
                xmlele.AppendChild(xmlsub);
				xmlsub = xmldoc.CreateElement("lV");
				//xmlsub.SetAttribute("vType", "text");
				xmlsub.InnerText = records[i].LocalUrl;
				xmlele.AppendChild(xmlsub);
				//xmlsub = xmldoc.CreateElement("lV");
				//xmlsub.SetAttribute("vType", "integer");
				//xmlsub.InnerText = records[i].Id;
				//xmlele.AppendChild(xmlsub);
				root.AppendChild(xmlele);
            }
            xmldoc.Save(string.Format(@"{0}\Record.xml", Setting.XmlFolder));
        }

        static public void SaveNote(List<Note> notes)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", "");
            xmldoc.AppendChild(xmldecl);
            //InfoTable
            XmlElement xmlele = xmldoc.CreateElement("InfoTable");
            xmlele.SetAttribute("ID", "14");
            xmldoc.AppendChild(xmlele);
            //Node
            XmlNode root = xmldoc.SelectSingleNode("InfoTable");
            xmlele = xmldoc.CreateElement("Node");
            xmlele.SetAttribute("vType", "string");
            xmlele.InnerText = "/基础信息/备忘录";
            root.AppendChild(xmlele);
            //TCol
            xmlele = xmldoc.CreateElement("TCol");
            //Prop
            XmlElement xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "integer");
            xmlsub.InnerText = "删除";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "内容";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "DateTime");
            xmlsub.InnerText = "创建时间";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "DateTime");
            xmlsub.InnerText = "修改时间";
            xmlele.AppendChild(xmlsub);
            //xmlsub = xmldoc.CreateElement("Prop");
            //xmlsub.SetAttribute("vType", "integer");
            //xmlsub.InnerText = "ID";
            //xmlele.AppendChild(xmlsub);
            root.AppendChild(xmlele);

            for (int i = 0; i < notes.Count; i++)
            {
                //Tln
                xmlele = xmldoc.CreateElement("Tln");
                //lV
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                xmlsub.InnerText = "0";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = notes[i].Snippet;
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "DateTime");
                xmlsub.InnerText = notes[i].CreateTime.ToString("G");
                if (Setting.Provider == ServiceProvider.HuaWei)
                {
                    xmlsub.InnerText = "";
                }
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "DateTime");
                xmlsub.InnerText = notes[i].ModifyTime.ToString("G");
                xmlele.AppendChild(xmlsub);
                //xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                //xmlsub.InnerText = notes[i].Id;
                //xmlele.AppendChild(xmlsub);
                root.AppendChild(xmlele);
            }
            xmldoc.Save(string.Format(@"{0}\Note.xml", Setting.XmlFolder));
        }

        static public void SaveGps(List<Gps> gpses)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", "");
            xmldoc.AppendChild(xmldecl);
            //InfoTable
            XmlElement xmlele = xmldoc.CreateElement("InfoTable");
            xmlele.SetAttribute("ID", "8");
            xmldoc.AppendChild(xmlele);
            //Node
            XmlNode root = xmldoc.SelectSingleNode("InfoTable");
            xmlele = xmldoc.CreateElement("Node");
            xmlele.SetAttribute("vType", "string");
            xmlele.InnerText = "/基础信息/地理位置";
            root.AppendChild(xmlele);
            //TCol
            xmlele = xmldoc.CreateElement("TCol");
            //Prop
            XmlElement xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "integer");
            xmlsub.InnerText = "删除";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "经度";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "纬度";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "text");
            xmlsub.InnerText = "精度";
            xmlele.AppendChild(xmlsub);
            xmlsub = xmldoc.CreateElement("Prop");
            xmlsub.SetAttribute("vType", "DateTime");
            xmlsub.InnerText = "时间";
            xmlele.AppendChild(xmlsub);
            //xmlsub = xmldoc.CreateElement("Prop");
            //xmlsub.SetAttribute("vType", "integer");
            //xmlsub.InnerText = "ID";
            //xmlele.AppendChild(xmlsub);
            root.AppendChild(xmlele);

            for (int i = 0; i < gpses.Count; i++)
            {
                //Tln
                xmlele = xmldoc.CreateElement("Tln");
                //lV
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                xmlsub.InnerText = "0";
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = gpses[i].Longitude.ToString();
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = gpses[i].Latitude.ToString();
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "text");
                xmlsub.InnerText = gpses[i].Accuracy.ToString();
                xmlele.AppendChild(xmlsub);
                xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "DateTime");
                xmlsub.InnerText = gpses[i].Time.ToString("G");
                xmlele.AppendChild(xmlsub);
                //xmlsub = xmldoc.CreateElement("lV");
                //xmlsub.SetAttribute("vType", "integer");
                //xmlsub.InnerText = "ID";
                //xmlele.AppendChild(xmlsub);
                root.AppendChild(xmlele);
            }
            xmldoc.Save(string.Format(@"{0}\Gps.xml", Setting.XmlFolder));
        }
    }
}

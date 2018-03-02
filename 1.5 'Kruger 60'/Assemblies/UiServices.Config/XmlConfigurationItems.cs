﻿// Copyright (C) 2014-2017, GitHub/Codeplex user AlphaCentaury
// 
// All rights reserved, except those granted by the governing license of this software.
// See 'license.txt' file in the project root for complete license information.
// 
// http://movistartv.alphacentaury.org/ https://github.com/AlphaCentaury

using IpTviewr.Common.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace IpTviewr.UiServices.Configuration
{
    [Serializable]
    public class XmlConfigurationItems
    {
        public static XmlElement GetXmlElement(Guid id, IConfigurationItem item)
        {
            XmlDocument xDoc = new XmlDocument();
            XPathNavigator xNavigator = xDoc.CreateNavigator();
            using (XmlWriter writer = xNavigator.AppendChild())
            {
                XmlSerializer ser = new XmlSerializer(item.GetType());
                ser.Serialize(writer, (object)item);
            } // using

            var xAttr = xDoc.CreateAttribute("configurationId");
            xAttr.Value = id.ToString();
            xDoc.DocumentElement.Attributes.Append(xAttr);

            return xDoc.DocumentElement;
        } // GetXmlElement

        [XmlArray("Registry")]
        [XmlArrayItem("Type")]
        public List<string> Registry
        {
            get;
            set;
        } // Registry

        [XmlAnyElement]
        public List<XmlElement> XmlData
        {
            get;
            set;
        } // XmlData
    } // class XmlConfigurationItems
} // namespace
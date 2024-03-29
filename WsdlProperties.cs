﻿namespace WebServiceStudio
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WsdlProperties
    {
        private bool bypassCertificateValidation = false;
        private string customCodeDomProvider;
        private string domain;
        private Language language;
        private string password;
        private Protocol protocol;
        private string proxy;
        private string proxyBaseType;
        private string proxyDomain;
        private string proxyPassword;
        private string proxyUserName;
        private string userName;
        private bool useClientCert;

        [XmlAttribute]
        public bool UseClientCert
        {
            get { return useClientCert; }
            set { useClientCert = value; }
        }

        public string[] GetProxyBaseTypeList()
        {
            return Configuration.MasterConfig.GetProxyBaseTypes();
        }

        public override string ToString()
        {
            return "";
        }

        [XmlAttribute]
        public bool BypassCertificateValidation
        {
            get
            {
                return this.bypassCertificateValidation;
            }
            set
            {
                this.bypassCertificateValidation = value;
            }
        }
        
        [RefreshProperties(RefreshProperties.All), XmlAttribute]
        public string CustomCodeDomProvider
        {
            get
            {
                return ((this.language == Language.Custom) ? this.customCodeDomProvider : "");
            }
            set
            {
                this.customCodeDomProvider = value;
                if ((value != null) && (value.Length > 0))
                {
                    this.language = Language.Custom;
                }
            }
        }

        [XmlAttribute]
        public string Domain
        {
            get
            {
                return this.domain;
            }
            set
            {
                this.domain = value;
            }
        }

        [XmlAttribute, RefreshProperties(RefreshProperties.All)]
        public Language Language
        {
            get
            {
                return this.language;
            }
            set
            {
                this.language = value;
            }
        }

        [XmlAttribute]
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }

        [XmlAttribute]
        public Protocol Protocol
        {
            get
            {
                return this.protocol;
            }
            set
            {
                this.protocol = value;
            }
        }

        [TypeConverter(typeof(ListStandardValues)), XmlAttribute]
        public string ProxyBaseType
        {
            get
            {
                return this.proxyBaseType;
            }
            set
            {
                this.proxyBaseType = value;
            }
        }

        [XmlAttribute]
        public string ProxyDomain
        {
            get
            {
                return this.proxyDomain;
            }
            set
            {
                this.proxyDomain = value;
            }
        }

        [XmlAttribute]
        public string ProxyPassword
        {
            get
            {
                return this.proxyPassword;
            }
            set
            {
                this.proxyPassword = value;
            }
        }

        [XmlAttribute]
        public string ProxyServer
        {
            get
            {
                return this.proxy;
            }
            set
            {
                this.proxy = value;
            }
        }

        [XmlAttribute]
        public string ProxyUserName
        {
            get
            {
                return this.proxyUserName;
            }
            set
            {
                this.proxyUserName = value;
            }
        }

        [XmlAttribute]
        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
            }
        }
    }
}


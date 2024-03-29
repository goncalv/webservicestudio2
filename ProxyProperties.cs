﻿using System;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;
using System.Web.Services.Protocols;
using WebServiceStudio.Dialogs;

namespace WebServiceStudio
{
    internal class ProxyProperties
    {
        private IAdditionalProperties additionalProperties;
        private bool allowAutoRedirect;
        private ServerProperties httpProxy;
        private bool preAuthenticate;
        private static Hashtable proxyTypeHandlers;
        private ServerProperties server;
        private int timeout;
        private static string typeNotFoundMessage = "ProxyPropertiesType {0} specified in WebServiceStudio.exe.options is not found";
        private bool useCookieContainer;
        private static string NTLM = "NTLM";
        private static string BASIC = "Basic";

        public ProxyProperties()
        {
        }

        public ProxyProperties(HttpWebClientProtocol proxy)
        {
            this.Timeout = proxy.Timeout;
            this.AllowAutoRedirect = proxy.AllowAutoRedirect;
            this.PreAuthenticate = proxy.PreAuthenticate;
            if (proxy.CookieContainer == null)
            {
                this.UseCookieContainer = true;
            }
            this.Server = new ServerProperties();
            this.Server.Url = proxy.Url;
            this.SetCredentialValues(proxy.Credentials, new Uri(this.Server.Url), out this.Server.UseDefaultCredentials, out this.Server.UserNameForBasicAuth, out this.Server.PasswordForBasicAuth);
            WebProxy proxy2 = proxy.Proxy as WebProxy;
            if (proxy2 != null)
            {
                this.HttpProxy = new ServerProperties();
                this.HttpProxy.Url = proxy2.Address.ToString();
                this.SetCredentialValues(proxy2.Credentials, new Uri(this.HttpProxy.Url), out this.HttpProxy.UseDefaultCredentials, out this.HttpProxy.UserNameForBasicAuth, out this.HttpProxy.PasswordForBasicAuth);
            }
            this.InitAdditionalProperties(proxy);
        }

        private void InitAdditionalProperties(HttpWebClientProtocol proxy)
        {
            if (proxyTypeHandlers == null)
            {
                proxyTypeHandlers = new Hashtable();
                CustomHandler[] proxyProperties = Configuration.MasterConfig.ProxyProperties;
                if ((proxyProperties != null) && (proxyProperties.Length > 0))
                {
                    foreach (CustomHandler handler in proxyProperties)
                    {
                        string typeName = handler.TypeName;
                        string str2 = handler.Handler;
                        if (((typeName != null) && (typeName.Length != 0)) && ((str2 != null) && (str2.Length != 0)))
                        {
                            Type key = Type.GetType(typeName);
                            if (key == null)
                            {
                                NewMainForm.ShowMessage(this, MessageType.Warning, string.Format(typeNotFoundMessage, typeName));
                            }
                            else
                            {
                                Type type = Type.GetType(str2);
                                if (type == null)
                                {
                                    NewMainForm.ShowMessage(this, MessageType.Warning, string.Format(typeNotFoundMessage, str2));
                                }
                                else
                                {
                                    proxyTypeHandlers.Add(key, type);
                                }
                            }
                        }
                    }
                }
            }
            for (Type type3 = proxy.GetType(); type3 != typeof(object); type3 = type3.BaseType)
            {
                Type type4 = proxyTypeHandlers[type3] as Type;
                if (type4 != null)
                {
                    this.AdditionalProperties = (IAdditionalProperties)Activator.CreateInstance(type4, new object[] { proxy });
                    break;
                }
            }
        }

        private ICredentials ReadCredentials(ICredentials credentials, Uri uri, bool useDefaultCredentials, string userName, string password)
        {
            CredentialCache cache = credentials as CredentialCache;

            if (cache == null)
            {
                cache = new CredentialCache();
            }
            if (useDefaultCredentials)
            {
                if (cache.GetCredential(uri, NTLM) == null)
                {
                    cache.Add(uri, NTLM, (NetworkCredential)CredentialCache.DefaultCredentials);
                }
            }
            else
            {
                if (cache.GetCredential(uri, NTLM) != null)
                {
                    cache.Remove(uri, NTLM);
                }
            }
            if ((((userName != null) && (userName.Length > 0)) || ((password != null) && (password.Length > 0))))
            {
                if (cache.GetCredential(uri, BASIC) == null)
                {
                    NetworkCredential cred = new NetworkCredential(userName, password);
                    cache.Add(uri, BASIC, cred);
                }
                else
                {
                    cache.GetCredential(uri, BASIC).UserName = userName;
                    cache.GetCredential(uri, BASIC).Password = password;
                }
            }
            return cache;
        }

        private void SetCredentialValues(ICredentials credentials, Uri uri, out bool useDefaultCredentials, out string userName, out string password)
        {
            useDefaultCredentials = false;
            userName = string.Empty;
            password = string.Empty;
            if (((credentials == null) || (credentials is CredentialCache)) && (credentials != null))
            {
                NetworkCredential credential = null;
                CredentialCache cache = credentials as CredentialCache;
                if (cache != null)
                {
                    if (CredentialCache.DefaultCredentials == cache.GetCredential(uri, NTLM))
                    {
                        useDefaultCredentials = true;
                    }
                    credential = cache.GetCredential(uri, BASIC);
                }
                else if (credentials == CredentialCache.DefaultCredentials)
                {
                    useDefaultCredentials = true;
                }
                else
                {
                    credential = credentials as NetworkCredential;
                }
                if (credential != null)
                {
                    userName = credential.UserName;
                    password = credential.Password;
                }
            }
        }

        public void UpdateProxy(HttpWebClientProtocol proxy)
        {
            proxy.Timeout = this.Timeout;
            proxy.AllowAutoRedirect = this.AllowAutoRedirect;
            proxy.PreAuthenticate = this.PreAuthenticate;
            if (this.UseCookieContainer)
            {
                if (proxy.CookieContainer == null)
                {
                    proxy.CookieContainer = new CookieContainer();
                }
            }
            else
            {
                proxy.CookieContainer = null;
            }
            proxy.Url = this.Server.Url;
            proxy.Credentials = this.ReadCredentials(proxy.Credentials, new Uri(this.Server.Url), this.Server.UseDefaultCredentials, this.Server.UserNameForBasicAuth, this.Server.PasswordForBasicAuth);
            if (((this.HttpProxy != null) && (this.HttpProxy.Url != null)) && (this.HttpProxy.Url.Length > 0))
            {
                Uri uri = new Uri(this.HttpProxy.Url);
                if (proxy.Proxy == null)
                {
                    proxy.Proxy = new WebProxy();
                }
                WebProxy proxy2 = proxy.Proxy as WebProxy;
                proxy2.Address = uri;
                proxy2.Credentials = this.ReadCredentials(proxy2.Credentials, uri, this.Server.UseDefaultCredentials, this.Server.UserNameForBasicAuth, this.Server.PasswordForBasicAuth);
            }
            if (this.additionalProperties != null)
            {
                this.additionalProperties.UpdateProxy(proxy);
            }
        }

        public IAdditionalProperties AdditionalProperties
        {
            get
            {
                return this.additionalProperties;
            }
            set
            {
                this.additionalProperties = value;
            }
        }

        public bool AllowAutoRedirect
        {
            get
            {
                return this.allowAutoRedirect;
            }
            set
            {
                this.allowAutoRedirect = value;
            }
        }

        public ServerProperties HttpProxy
        {
            get
            {
                return this.httpProxy;
            }
            set
            {
                this.httpProxy = value;
            }
        }

        public bool PreAuthenticate
        {
            get
            {
                return this.preAuthenticate;
            }
            set
            {
                this.preAuthenticate = value;
            }
        }

        public ServerProperties Server
        {
            get
            {
                return this.server;
            }
            set
            {
                this.server = value;
            }
        }

        public int Timeout
        {
            get
            {
                return this.timeout;
            }
            set
            {
                this.timeout = value;
            }
        }

        public bool UseCookieContainer
        {
            get
            {
                return this.useCookieContainer;
            }
            set
            {
                this.useCookieContainer = value;
            }
        }

        public class ServerProperties
        {
            public string PasswordForBasicAuth;
            public string Url;
            public bool UseDefaultCredentials;
            public string UserNameForBasicAuth;
        }
    }
}

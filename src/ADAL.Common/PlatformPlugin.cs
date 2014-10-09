//----------------------------------------------------------------------
// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
// Apache License 2.0
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
    internal static class PlatformPluginSwitch
    {
        public static bool DynamicallyLinkAssembly { get; set; }

        static PlatformPluginSwitch()
        {
            DynamicallyLinkAssembly = true;
        }
    }

    internal static class PlatformPlugin
    {
        static PlatformPlugin()
        {
            HttpWebRequestFactory = new HttpWebRequestFactory();
            RequestCreationHelper = new RequestCreationHelper();

            if (PlatformPluginSwitch.DynamicallyLinkAssembly)
            {
                InitializeByAssemblyDynamicLinking();
            }
        }

        public static IWebUIFactory WebUIFactory { get; set; }
        public static IHttpWebRequestFactory HttpWebRequestFactory { get; set; }
        public static IRequestCreationHelper RequestCreationHelper { get; set; }
        public static LoggerBase Logger { get; set; }
        public static PlatformInformationBase PlatformInformation { get; set; }
        public static ICryptographyHelper CryptographyHelper { get; set; }

        public static void InitializeByAssemblyDynamicLinking()
        {
            try
            {
                Assembly assembly = LoadPlatformSpecificAssembly();
                const string Namespace = "Microsoft.IdentityModel.Clients.ActiveDirectory.";
                InjectDependecies(
                    (IWebUIFactory)Activator.CreateInstance(assembly.GetType(Namespace + "WebUIFactory")),
                    (LoggerBase)Activator.CreateInstance(assembly.GetType(Namespace + "Logger")),
                    (PlatformInformationBase)Activator.CreateInstance(assembly.GetType(Namespace + "PlatformInformation")),
                    (ICryptographyHelper)Activator.CreateInstance(assembly.GetType(Namespace + "CryptographyHelper"))
                );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void InjectDependecies(IWebUIFactory webUIFactory, LoggerBase logger, PlatformInformationBase platformInformation, ICryptographyHelper cryptographyHelper)
        {
            WebUIFactory = webUIFactory;
            Logger = logger;
            PlatformInformation = platformInformation;
            CryptographyHelper = cryptographyHelper;            
        }

        private static Assembly LoadPlatformSpecificAssembly()
        {
            // For security reasons, it is important to have PublicKeyToken mentioned referencing the assembly.
            const string PlatformSpecificAssemblyNameTemplate = "Microsoft.IdentityModel.Clients.ActiveDirectory.Platform, Version={0}, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

            string platformSpecificAssemblyName = string.Format(PlatformSpecificAssemblyNameTemplate, AdalIdHelper.GetAdalVersion());

            try
            {
                return Assembly.Load(new AssemblyName(platformSpecificAssemblyName));
            }
            catch (FileNotFoundException ex)
            {
                throw new AdalException(AdalError.AssemblyNotFound, string.Format(CultureInfo.InvariantCulture, AdalErrorMessage.AssemblyNotFoundTemplate, platformSpecificAssemblyName), ex);
            }
            catch (Exception ex) // FileLoadException is missing from PCL
            {
                throw new AdalException(AdalError.AssemblyLoadFailed, string.Format(CultureInfo.InvariantCulture, AdalErrorMessage.AssemblyLoadFailedTemplate, platformSpecificAssemblyName), ex);
            }
        }

        private static void ThrowAssemlyLoadFailedException(string webAuthenticationDialogAssemblyName, Exception innerException)
        {
        }
    }
}
﻿//----------------------------------------------------------------------
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

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
    internal class CryptographyHelper : ICryptographyHelper
    {
        public string CreateSha256Hash(string input)
        {
            IBuffer inputBuffer = CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf8);

            var hasher = HashAlgorithmProvider.OpenAlgorithm("SHA256");
            IBuffer hashed = hasher.HashData(inputBuffer);

            return CryptographicBuffer.EncodeToBase64String(hashed);
        }


        public byte[] SignWithCertificate(string message, byte[] rawData, string password)
        {
            throw new NotImplementedException();            
        }

        public string GetX509CertificateThumbprint(ClientAssertionCertificate credential)
        {
            throw new NotImplementedException();
        }

    }
}

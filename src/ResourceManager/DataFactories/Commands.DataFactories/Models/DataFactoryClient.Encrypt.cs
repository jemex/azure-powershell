﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Security;
using Microsoft.Azure.Management.DataFactories;
using Microsoft.DataTransfer.Gateway.Encryption;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.DataFactories
{
    public partial class DataFactoryClient
    {
        public virtual string OnPremisesEncryptString(SecureString value, string resourceGroupName, string dataFactoryName, string gatewayName, PSCredential credential, string type)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            LinkedServiceType linkedServiceType = type == null ? LinkedServiceType.OnPremisesSqlLinkedService : (LinkedServiceType) Enum.Parse(typeof(LinkedServiceType), type, true);

            var response = DataPipelineManagementClient.Gateways.RetrieveConnectionInfo(resourceGroupName, dataFactoryName, gatewayName);
            var gatewayEncryptionInfos = new[]
                {
                    new GatewayEncryptionInfo
                        {
                            ServiceToken = response.ConnectionInfo.ServiceToken,
                            IdentityCertThumbprint = response.ConnectionInfo.IdentityCertThumbprint,
                            HostServiceUri = response.ConnectionInfo.HostServiceUri,
                            InstanceVersionString = response.ConnectionInfo.Version 
                        }
                };

            string userName = credential != null ? credential.UserName : null;
            SecureString password = credential != null ? credential.Password : null;
            UserInputConnectionString connectionString = new UserInputConnectionString(value, userName, password, linkedServiceType);
            return GatewayEncryptionClient.Encrypt(connectionString, gatewayEncryptionInfos);
        }
    }
}

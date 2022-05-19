// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request
{
    public class BankApiResponseFile
    {
        public BankApiResponseFile(string responseFile, bool useResponseFileAsApiResponse)
        {
            ResponseFile = responseFile;
            UseResponseFileAsApiResponse = useResponseFileAsApiResponse;
        }

        /// <summary>
        ///     Allows to use existing bank registration instead of creating new one. Response
        ///     should be of type <see cref="ClientRegistrationModelsPublic.OBClientRegistration1" /> but is supplied as JSON text
        ///     file
        ///     so can be de-serialised and validated in same way as response from bank.
        ///     Settings in <see cref="BankRegistrationPostCustomBehaviour" /> will be used.
        ///     This argument is intended for use in testing scenarios where you want to test
        ///     creation of a new registration but cannot re-POST one to bank for whatever reason.
        /// </summary>
        public string ResponseFile { get; }

        public bool UseResponseFileAsApiResponse { get; }
    }
}

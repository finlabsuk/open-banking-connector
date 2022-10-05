// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> The OBError1. </summary>
    public partial class OBError1
    {
        /// <summary> Initializes a new instance of OBError1. </summary>
        /// <param name="errorCode"> Low level textual error code, e.g., UK.OBIE.Field.Missing. </param>
        /// <param name="message">
        /// A description of the error that occurred. e.g., &apos;A mandatory field isn&apos;t supplied&apos; or &apos;RequestedExecutionDateTime must be in future&apos;
        /// OBIE doesn&apos;t standardise this field
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="errorCode"/> or <paramref name="message"/> is null. </exception>
        public OBError1(string errorCode, string message)
        {
            if (errorCode == null)
            {
                throw new ArgumentNullException(nameof(errorCode));
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            ErrorCode = errorCode;
            Message = message;
        }

        /// <summary> Initializes a new instance of OBError1. </summary>
        /// <param name="errorCode"> Low level textual error code, e.g., UK.OBIE.Field.Missing. </param>
        /// <param name="message">
        /// A description of the error that occurred. e.g., &apos;A mandatory field isn&apos;t supplied&apos; or &apos;RequestedExecutionDateTime must be in future&apos;
        /// OBIE doesn&apos;t standardise this field
        /// </param>
        /// <param name="path"> Recommended but optional reference to the JSON Path of the field with error, e.g., Data.Initiation.InstructedAmount.Currency. </param>
        /// <param name="url"> URL to help remediate the problem, or provide more information, or to API Reference, or help etc. </param>
        public OBError1(string errorCode, string message, string path, string url)
        {
            ErrorCode = errorCode;
            Message = message;
            Path = path;
            Url = url;
        }

        /// <summary> Low level textual error code, e.g., UK.OBIE.Field.Missing. </summary>
        public string ErrorCode { get; }
        /// <summary>
        /// A description of the error that occurred. e.g., &apos;A mandatory field isn&apos;t supplied&apos; or &apos;RequestedExecutionDateTime must be in future&apos;
        /// OBIE doesn&apos;t standardise this field
        /// </summary>
        public string Message { get; }
        /// <summary> Recommended but optional reference to the JSON Path of the field with error, e.g., Data.Initiation.InstructedAmount.Currency. </summary>
        public string Path { get; }
        /// <summary> URL to help remediate the problem, or provide more information, or to API Reference, or help etc. </summary>
        public string Url { get; }
    }
}

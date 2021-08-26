// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.UkDcrApi.V3p1.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class RegistrationError
    {
        /// <summary>
        /// Initializes a new instance of the RegistrationError class.
        /// </summary>
        public RegistrationError()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the RegistrationError class.
        /// </summary>
        /// <param name="error">Possible values include:
        /// 'invalid_redirect_uri', 'invalid_client_metadata',
        /// 'invalid_software_statement',
        /// 'unapproved_software_statement'</param>
        public RegistrationError(RegistrationErrorerrorEnum error, string errorDescription = default(string))
        {
            Error = error;
            ErrorDescription = errorDescription;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'invalid_redirect_uri',
        /// 'invalid_client_metadata', 'invalid_software_statement',
        /// 'unapproved_software_statement'
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public RegistrationErrorerrorEnum Error { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "error_description")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (ErrorDescription != null)
            {
                if (ErrorDescription.Length > 500)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "ErrorDescription", 500);
                }
                if (ErrorDescription.Length < 1)
                {
                    throw new ValidationException(ValidationRules.MinLength, "ErrorDescription", 1);
                }
            }
        }
    }
}

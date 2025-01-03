// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Links relevant to the payload
    /// </summary>
    public partial class Links
    {
        /// <summary>
        /// Initializes a new instance of the Links class.
        /// </summary>
        public Links()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Links class.
        /// </summary>
        public Links(Uri self, Uri first = default(Uri), Uri prev = default(Uri), Uri next = default(Uri), Uri last = default(Uri))
        {
            Self = self;
            First = first;
            Prev = prev;
            Next = next;
            Last = last;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Self")]
        public Uri Self { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "First")]
        public Uri First { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Prev")]
        public Uri Prev { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Next")]
        public Uri Next { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Last")]
        public Uri Last { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Self == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Self");
            }
        }
    }
}

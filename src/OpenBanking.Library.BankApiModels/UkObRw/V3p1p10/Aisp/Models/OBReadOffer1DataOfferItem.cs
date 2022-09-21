// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> The OBReadOffer1DataOfferItem. </summary>
    public partial class OBReadOffer1DataOfferItem
    {
        /// <summary> Initializes a new instance of OBReadOffer1DataOfferItem. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="accountId"/> is null. </exception>
        internal OBReadOffer1DataOfferItem(string accountId)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            AccountId = accountId;
        }

        /// <summary> Initializes a new instance of OBReadOffer1DataOfferItem. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="offerId"> A unique and immutable identifier used to identify the offer resource. This identifier has no meaning to the account owner. </param>
        /// <param name="offerType"> Offer type, in a coded form. </param>
        /// <param name="description"> Further details of the offer. </param>
        /// <param name="startDateTime">
        /// Date and time at which the offer starts.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="endDateTime">
        /// Date and time at which the offer ends.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="rate"> Rate associated with the offer type. </param>
        /// <param name="value"> Value associated with the offer type. </param>
        /// <param name="term"> Further details of the term of the offer. </param>
        /// <param name="url"> URL (Uniform Resource Locator) where documentation on the offer can be found. </param>
        /// <param name="amount"> Amount of money associated with the offer type. </param>
        /// <param name="fee"> Fee associated with the offer type. </param>
        internal OBReadOffer1DataOfferItem(string accountId, string offerId, OBReadOffer1DataOfferTypeEnum? offerType, string description, DateTimeOffset? startDateTime, DateTimeOffset? endDateTime, string rate, int? value, string term, string url, OBReadOffer1DataOfferItemAmount amount, OBReadOffer1DataOfferItemFee fee)
        {
            AccountId = accountId;
            OfferId = offerId;
            OfferType = offerType;
            Description = description;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Rate = rate;
            Value = value;
            Term = term;
            URL = url;
            Amount = amount;
            Fee = fee;
        }

        /// <summary> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </summary>
        public string AccountId { get; }
        /// <summary> A unique and immutable identifier used to identify the offer resource. This identifier has no meaning to the account owner. </summary>
        public string OfferId { get; }
        /// <summary> Offer type, in a coded form. </summary>
        public OBReadOffer1DataOfferTypeEnum? OfferType { get; }
        /// <summary> Further details of the offer. </summary>
        public string Description { get; }
        /// <summary>
        /// Date and time at which the offer starts.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? StartDateTime { get; }
        /// <summary>
        /// Date and time at which the offer ends.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? EndDateTime { get; }
        /// <summary> Rate associated with the offer type. </summary>
        public string Rate { get; }
        /// <summary> Value associated with the offer type. </summary>
        public int? Value { get; }
        /// <summary> Further details of the term of the offer. </summary>
        public string Term { get; }
        /// <summary> URL (Uniform Resource Locator) where documentation on the offer can be found. </summary>
        public string URL { get; }
        /// <summary> Amount of money associated with the offer type. </summary>
        public OBReadOffer1DataOfferItemAmount Amount { get; }
        /// <summary> Fee associated with the offer type. </summary>
        public OBReadOffer1DataOfferItemFee Fee { get; }
    }
}

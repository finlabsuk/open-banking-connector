// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions2
    {
        private BankProfile GetTsb()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.Tsb);
            return new BankProfile(
                BankProfileEnum.Tsb,
                "https://apis.tsb.co.uk/apis/sandbox/open-banking/v3.1", // from https://apis.developer.tsb.co.uk/?WT.mc_id=&WT.srch=1&keyword=&matchtype=&adid=&cmp=&agrp=&eact=&ch=ppc&sch=perfmax&pf=mor&co=acq&gclid=Cj0KCQiAubmPBhCyARIsAJWNpiMGIo64EcPICBrMmxT3nK1J6hx_fNkHXW3FP-zxqQJzTe8Aig7xQNMaAnPNEALw_wcB&gclsrc=aw.ds
                bankProfileHiddenProperties.GetRequiredFinancialId(),
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiVersion(),
                    BaseUrl =
                        "https://apis.tsb.co.uk/apis/sandbox/open-banking/v3.1/pisp" //from https://apis.developer.tsb.co.uk/?WT.mc_id=&WT.srch=1&keyword=&matchtype=&adid=&cmp=&agrp=&eact=&ch=ppc&sch=perfmax&pf=mor&co=acq&gclid=Cj0KCQiAubmPBhCyARIsAJWNpiMGIo64EcPICBrMmxT3nK1J6hx_fNkHXW3FP-zxqQJzTe8Aig7xQNMaAnPNEALw_wcB&gclsrc=aw.ds
                },
                null,
                false);
        }
    }
}

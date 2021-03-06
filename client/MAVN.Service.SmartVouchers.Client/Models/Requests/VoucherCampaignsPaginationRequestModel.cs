﻿using JetBrains.Annotations;

namespace MAVN.Service.SmartVouchers.Client.Models.Requests
{
    /// <summary>
    /// Request model to get paginated response for existing voucher campaigns.
    /// </summary>
    [PublicAPI]
    public class VoucherCampaignsPaginationRequestModel : BasePaginationRequestModel
    {
        /// <summary>Represents search field by campaign's name</summary>
        public string CampaignName { get; set; }

        /// <summary>Only active campaigns flag</summary>
        public bool OnlyActive { get; set; }
    }
}

﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace MAVN.Service.SmartVouchers.Client.Models.Responses
{
    /// <summary>
    /// Paginated response model for existing voucher campaigns.
    /// </summary>
    [PublicAPI]
    public class PaginatedVouchersListResponseModel : BasePaginationResponseModel
    {
        /// <summary>
        /// List of smart vouchers
        /// </summary>
        public IReadOnlyList<VoucherResponseModel> Vouchers { get; set; }
    }
}

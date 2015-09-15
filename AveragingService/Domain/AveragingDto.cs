using System;
using System.Collections.Generic;

namespace AveragingService.Domain
{
    public class AveragingDto
    {
        public Guid AccountId { get; set; }

        /// <summary>از تاریخ</summary>
        public long FromDate { get; set; }

        /// <summary>تا تاریخ</summary>
        public long ToDate { get; set; }

        /// <summary>مدت معدلگیری به روز</summary>
        public long Duration { get; set; }

        /// <summary>شناسه استراتژی</summary>
        public short PrivilegeId { get; set; }

        /// <summary>ضریب کاهش</summary>
        public decimal ReductionRatio { get; set; }

        /// <summary>حداکثر مانده مجاز در روز</summary>
        public decimal MaxBalancePerDay { get; set; }

        //موجودی از سال قبل تا امسال
        public decimal UntilDateBalance { get; set; }

        //گردش بدهکار از سال قبل تا امسال
        public decimal UntilDateDebtor { get; set; }

        //گردش بستانکار از سال قبل تا امسال
        public decimal UntilDateDeposit { get; set; }

        public List<AccountActivityDto> AccountActivityDtos { get; set; }
    }
}

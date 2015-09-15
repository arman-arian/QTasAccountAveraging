using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AveragingService.Domain
{
    /// <summary>نوع عملیات : بدهکار یا بستانکار یا هر دو</summary>
    public enum TransactionType : short
    {
        /// <summary>بدهکار = 1</summary>
        [Description("بدهکار")]
        Debit = 1,

        /// <summary>-بستانکار = 1</summary>
        [Description("بستانکار")]
        Credit = -1,
    }
}

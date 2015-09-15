using System;

namespace AveragingService.Domain
{
    public class AccountActivityDto
    {
        public Guid Id { get; set; }

        public short YearOf { get; set; }

        public long RegDate { get; set; }

        public int RegTime { get; set; }

        public decimal Amount { get; set; }

        public short TransactionType { get; set; }
    }
}

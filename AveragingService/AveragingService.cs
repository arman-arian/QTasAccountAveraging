using System;
using System.Collections.Generic;
using System.Linq;
using AveragingService.AveragingModule;
using AveragingService.Domain;

namespace AveragingService
{
    public class AveragingService
    {
        readonly DbAveragingTestEntities ctx = new DbAveragingTestEntities();

        public AverageResult CalcAverage(long fromDate, long toDate, int duration, int generalNo, int indexNo, long accountNo, short privilegeId, decimal MaxBalancePerDay = decimal.MaxValue)
        {
            var account = ctx.Accounts.FirstOrDefault(a => a.GeneralNo == generalNo && a.IndexNo == indexNo && a.AccountNo == accountNo);
            if(account == null)
                throw new Exception("No Account!");

            var lastYearAccount = GetLastYearAccount(account.Id, fromDate);

            var sumOfTurnOverFromDate = GetAccountActivitiesGroupSumTurnOver(account.Id, fromDate, lastYearAccount.Balance, lastYearAccount.Debtor, lastYearAccount.Creditor);




            var accountActivities = GetAccountActivities(account.Id, fromDate, toDate);

            var averageDto = CreateAverageDto(fromDate, toDate, duration, account.Id, privilegeId, MaxBalancePerDay,
                sumOfTurnOverFromDate.Balance, sumOfTurnOverFromDate.SumOfDebitAmount,
                sumOfTurnOverFromDate.SumOfCreditAmount, accountActivities);

            var averagingStrategy = CreateAveragingStrategy(averageDto.PrivilegeId, averageDto.FromDate, averageDto.ToDate);
            var averagingResult = averagingStrategy.CalcAverage(averageDto);
            return averagingResult;

        }

        /// <summary>بازیابی جمع مبالغ بدهکار و بستانکار از تاریخ تا تاریخ</summary>
        /// <param name="accountId">شناسه حساب</param>
        /// <param name="fromDate">از تاریخ</param>
        /// <param name="toDate">تا تاریخ</param>
        /// <returns></returns>
        private List<ActivitiesGroup> GroupAccountActivitiesByTransactionType(Guid accountId, long fromDate, long toDate)
        {
            var groupActivities = (
                from activity in ctx.AccountActivities.AsNoTracking().Where(a => a.AccountId == accountId)
                where
                    activity.RegDate >= fromDate &&
                    activity.RegDate < toDate
                group activity by activity.TransactionType
                    into activities
                    select new ActivitiesGroup
                    {
                        TransactionType = activities.Key,
                        SumBalance = activities.Sum(d => d.Amount) * (int)activities.Key
                    }).ToList();
            return groupActivities;
        }

        private ActivitiesGroupSumTurnOver GetAccountActivitiesGroupSumTurnOver(Guid accountId, long fromDate, long toDate)
        {
            var groupActivities = GroupAccountActivitiesByTransactionType(accountId, fromDate, toDate);

            var turnover = new ActivitiesGroupSumTurnOver
            {
                //موجودی امسال
                Balance = groupActivities.Sum(d => d.SumBalance),

                //گردش بدهکار امسال
                SumOfDebitAmount = groupActivities
                    .Where(d => d.TransactionType == 1 /*TransactionType.Debit*/)
                    .Select(ga => (decimal?) ga.SumBalance).FirstOrDefault() ?? Decimal.Zero,

                //گردش بستانکار امسال
                SumOfCreditAmount = groupActivities
                    .Where(d => d.TransactionType == -1 /*TransactionType.Credit*/)
                    .Select(ga => (decimal?) ga.SumBalance).FirstOrDefault() ?? Decimal.Zero
            };

            return turnover;
        }

        private ActivitiesGroupSumTurnOver GetAccountActivitiesGroupSumTurnOver(Guid accountId, long fromDate, decimal lastYearBalance, decimal lastYearDebtor, decimal lastYearCreditor)
        {
            var firstOfThisYear = PDateClass.GetFirstDateOfYear(fromDate);

            var turnover = GetAccountActivitiesGroupSumTurnOver(accountId, firstOfThisYear, fromDate);

            var activitiesGroupSumTurnOver = new ActivitiesGroupSumTurnOver
            {
                //موجودی از سال قبل تا امسال
                Balance = lastYearBalance + turnover.Balance,

                //گردش بدهکاراز سال قبل تا امسال
                SumOfDebitAmount = lastYearDebtor + turnover.SumOfDebitAmount,

                //گردش بستانکاراز سال قبل تا امسال
                SumOfCreditAmount = lastYearCreditor + turnover.SumOfCreditAmount
            };

            return activitiesGroupSumTurnOver;
        }

        private YearAccount GetYearAccount(Guid accountId, long Year)
        {
            var yearAccount = ctx.YearAccounts.FirstOrDefault(p => p.YearOf == Year && p.AccountId == accountId);
            if (yearAccount != null) return yearAccount;
            var defaultYearAccount = new YearAccount {Balance = 0, Creditor = 0, Debtor = 0, AccountId = accountId};
            return defaultYearAccount;
        }

        private YearAccount GetLastYearAccount(Guid accountId, long fromDate)
        {
            var lastYear = PDateClass.GetLastYear(fromDate);
            var yearAccount = GetYearAccount(accountId, lastYear);
            return yearAccount;
        }

        private Dictionary<long, int> GetHolidays(long fromDate, long toDate)
        {
            var regDates =
                ctx.DayIndexAccounts.AsNoTracking()
                    .Where(a => a.RegDate >= fromDate && a.RegDate <= toDate)
                    .Select(a => a.RegDate).Distinct().ToList();

            var holidays = new Dictionary<Int64, int>();
            for (var count = 0; count < regDates.Count() - 1; count = count + 1)
            {
                if (PDateClass.SubtractTwoDate(regDates[count], regDates[count + 1]) != 1)
                    holidays.Add(regDates[count], PDateClass.SubtractTwoDate(regDates[count], regDates[count + 1]));
            }
            return holidays;
        }

        private AveragingStrategy CreateAveragingStrategy(short privilegeId, long fromDate, long toDate)
        {
            switch (privilegeId)
            {
                // معدلگیری بصورت نرمال
                case 1:
                    return new AveragingWithLastBalancePerDayStrategy();

                // معدلگیری بر اساس کمترین موجودی
                case 2:
                    return new AveragingWithMinBalancePerDayStrategy();

                // معدلگیری بر اساس کمترین موجودی و با احتساب تعطیلات
                case 3:
                    var holidays = GetHolidays(fromDate, toDate);
                    return new AveragingWithMinBalancePerDayByHolidaysStrategy(holidays);

                // معدلگیری بصورت نرمال
                default:
                    return new AveragingWithLastBalancePerDayStrategy();
            }
        }

        private static AveragingDto CreateAverageDto(long fromDate, long toDate, int duration, Guid accountId, short privilegeId, decimal MaxBalancePerDay, decimal balance, decimal SumOfDebitAmount, decimal SumOfCreditAmount, List<AccountActivityDto> accountActivityDtos)
        {
            return new AveragingDto
            {
                UntilDateBalance = balance,
                UntilDateDebtor = SumOfDebitAmount,
                UntilDateDeposit = SumOfCreditAmount,
                AccountId = accountId,
                FromDate = fromDate,
                ToDate = toDate,
                MaxBalancePerDay = MaxBalancePerDay,
                PrivilegeId = privilegeId,
                AccountActivityDtos = accountActivityDtos,
                Duration = duration,
                ReductionRatio = 0.4m
            };
        }

        private List<AccountActivityDto> GetAccountActivities(Guid accountId, long fromDate, long toDate)
        {
            return ctx.AccountActivities.AsNoTracking()
                    .Where(a => a.AccountId == accountId && a.RegDate >= fromDate && a.RegDate <= toDate)
                    .Select(a => new AccountActivityDto
                    {
                        Id = a.Id,
                        Amount = a.Amount,
                        RegDate = a.RegDate,
                        RegTime = a.RegTime,
                        TransactionType = a.TransactionType,
                        YearOf = a.YearOf
                    }).ToList();
        }
    }

    public class ActivitiesDayGroup
    {
        public long Date { get; set; }

        public decimal DayBalance { get; set; }

        public decimal DayDebtor { get; set; }

        public decimal DayCreditor { get; set; }
    }

    public class ActivitiesGroup
    {
        public short TransactionType { get; set; }

        public decimal SumBalance { get; set; }
    }

    public class ActivitiesGroupSumTurnOver
    {
        public decimal Balance { get; set; }

        public decimal SumOfCreditAmount { get; set; }

        public decimal SumOfDebitAmount { get; set; }
    }

    public class AverageResult
    {
        public AverageResult()
        {
            Privilege = 0;
        }

        public decimal AverageAmount { get; set; }

        public decimal Privilege { get; set; }

        public decimal Balance { get; set; }

        public decimal MinBalance { get; set; }

        public decimal SumOfDebit { get; set; }

        public decimal SumOfCredit { get; set; }
    }
}

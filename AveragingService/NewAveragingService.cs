using System;
using System.Collections.Generic;
using System.Linq;
using AveragingService.AveragingModule;
using AveragingService.Domain;

namespace AveragingService
{
    public class NewAveragingService
    {
        readonly DbAveragingTestEntities ctx = new DbAveragingTestEntities();

        public AverageResult CalcAverage(long fromDate, long toDate, int duration, int generalNo, int indexNo, long accountNo, short privilegeId, decimal maxBalancePerDay = decimal.MaxValue)
        {
            var account = ctx.Accounts.FirstOrDefault(a => a.GeneralNo == generalNo && a.IndexNo == indexNo && a.AccountNo == accountNo);
            if (account == null)
                throw new Exception("No Account!");

            var today = PDateClass.GetNowDate();

            var balance = account.Balance - SumAccountActivitiesAmount(account.Id, fromDate, today);

            var accountActivities = GetAccountActivities(account.Id, fromDate, toDate);

            var averageDto = CreateAverageDto(fromDate, toDate, duration, account.Id, privilegeId, maxBalancePerDay,
                balance, 0, 0, accountActivities);

            var averagingStrategy = CreateAveragingStrategy(averageDto.PrivilegeId, averageDto.FromDate, averageDto.ToDate);
            var averagingResult = averagingStrategy.CalcAverage(averageDto);
            return averagingResult;
        }

        private decimal SumAccountActivitiesAmount(Guid accountId, long fromDate, long toDate)
        {
            return ctx.AccountActivities.AsNoTracking()
                .Where(a => a.AccountId == accountId & a.RegDate >= fromDate && a.RegDate < toDate)
                .Sum(a => a.Amount*a.TransactionType);
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

        private static AveragingDto CreateAverageDto(long fromDate, long toDate, int duration, Guid accountId, short privilegeId, decimal maxBalancePerDay, decimal balance, decimal sumOfDebitAmount, decimal sumOfCreditAmount, List<AccountActivityDto> accountActivityDtos)
        {
            return new AveragingDto
            {
                UntilDateBalance = balance,
                UntilDateDebtor = sumOfDebitAmount,
                UntilDateDeposit = sumOfCreditAmount,
                AccountId = accountId,
                FromDate = fromDate,
                ToDate = toDate,
                MaxBalancePerDay = maxBalancePerDay,
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
}

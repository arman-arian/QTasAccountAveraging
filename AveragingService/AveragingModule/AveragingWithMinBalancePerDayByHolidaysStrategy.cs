using System.Collections.Generic;
using System.Linq;
using AveragingService.Domain;

namespace AveragingService.AveragingModule
{
    public class AveragingWithMinBalancePerDayByHolidaysStrategy : AveragingWithMinBalancePerDayStrategy
    {
        private Dictionary<long, int> Holidays;

        public AveragingWithMinBalancePerDayByHolidaysStrategy(Dictionary<long, int> holidays)
        {
            this.Holidays = holidays;
        }

        ~AveragingWithMinBalancePerDayByHolidaysStrategy()
        {
            Holidays = null;
        }

        /// <summary>محاسبه معدل</summary>
        protected override AverageResult CalcAverage(List<AccountActivityDto> accountActivityDtos, long fromDate, long toDate, long duration, decimal balance,
            decimal sumOfDebit, decimal sumOfCredit, decimal maxBalancePerDay, decimal reductionRatio)
        {
            var activitiesDayGroups = GroupActivitiesByDate(accountActivityDtos);

            var privilege = 0m;
            var minBalance = balance;
            var dayMinBalance = balance;
            var theDate = fromDate;
            var holidaysCount = 0;

            if (minBalance == 0)
                minBalance = activitiesDayGroups.Select(db => db.DayBalance).FirstOrDefault();

            // به ازای هر روز دارای عملیات
            foreach (var dayBalance in activitiesDayGroups)// با هر روز دارای عملیات
            {
                privilege = CalcPrivilege(balance, dayMinBalance, privilege, maxBalancePerDay, dayBalance.Date, theDate, reductionRatio, holidaysCount);

                sumOfDebit = Sum(sumOfDebit, dayBalance.DayDebtor);

                sumOfCredit = Sum(sumOfCredit, dayBalance.DayCreditor);

                dayMinBalance = Sum(balance, dayBalance.DayBalance);

                minBalance = CalcMinBalance(minBalance, dayMinBalance);

                theDate = dayBalance.Date;

                var groupActivities = GroupActivitiesOfDayByTime(accountActivityDtos, theDate);
                dayMinBalance = GetMinDayBalance(theDate, balance, dayMinBalance, groupActivities);

                balance = Sum(balance, dayBalance.DayBalance);

                holidaysCount = CalcHolidaysCount(balance, dayMinBalance, dayBalance.Date);
            }

            //اگر در پیمایش عملیات یک حساب تاریخ آخرین عملیات با تاریخ پایان دوره معدل گیری منطبق نبود
            if (theDate < /*=*/ toDate)
            {
                privilege = CalcPrivilege(balance, dayMinBalance, privilege, maxBalancePerDay, toDate, theDate, reductionRatio, holidaysCount);
            }

            //محاسبه معدل
            var averageAmount = CalcAverageAmount(privilege, duration);

            return CreateResult(balance, minBalance, privilege, sumOfCredit, sumOfDebit, averageAmount);
        }

        private int CalcHolidaysCount(decimal balance, decimal dayMinBalance, long date)
        {
            return balance == dayMinBalance ? 1 : GetHolydayCount(date, Holidays);
        }

        /// <summary>بازیابی تعداد روزهای تعطیل</summary>
        private static int GetHolydayCount(long theDate, IReadOnlyDictionary<long, int> holidays)
        {
            var holidayCount = 1;
            if (holidays.ContainsKey(theDate)) holidays.TryGetValue(theDate, out holidayCount);
            return holidayCount;
        }
    }
}

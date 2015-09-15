using System;
using System.Collections.Generic;
using System.Linq;
using AveragingService.Domain;

namespace AveragingService.AveragingModule
{
    public class AveragingWithMinBalancePerDayStrategy : AveragingStrategy
    {
        /// <summary>محاسبه معدل</summary>
        protected override AverageResult CalcAverage(List<AccountActivityDto> accountActivityDtos, long fromDate, long toDate, long duration, decimal balance,
            decimal sumOfDebit, decimal sumOfCredit, decimal maxBalancePerDay, decimal reductionRatio)
        {
            var activitiesDayGroups = GroupActivitiesByDate(accountActivityDtos);

            var privilege = 0m;
            var minBalance = balance;
            var dayMinBalance = balance;
            var theDate = fromDate;
            const int holidaysCount = 1;

            if (minBalance == 0)
                minBalance = activitiesDayGroups.Select(db => db.DayBalance).FirstOrDefault();

            // به ازای هر روز دارای عملیات
            foreach (var dayBalance in activitiesDayGroups)
            {
                sumOfDebit = Sum(sumOfDebit, dayBalance.DayDebtor);

                sumOfCredit = Sum(sumOfCredit, dayBalance.DayCreditor);



                privilege = CalcPrivilege(balance, dayMinBalance, privilege, maxBalancePerDay, dayBalance.Date, theDate, reductionRatio, holidaysCount);

                dayMinBalance = Sum(balance, dayBalance.DayBalance);

                minBalance = CalcMinBalance(minBalance, dayMinBalance);

                theDate = dayBalance.Date;

                var groupActivities = GroupActivitiesOfDayByTime(accountActivityDtos, theDate);
                dayMinBalance = GetMinDayBalance(theDate, balance, dayMinBalance, groupActivities);

                balance = Sum(balance, dayBalance.DayBalance);
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
        protected static decimal CalcPrivilege(decimal balance, decimal dayMinBalance, decimal privilege, decimal maxBalancePerDay, long fromDate, long toDate, decimal reductionRatio, int holidaysCount)
        {
            privilege = CalcPrivilegeWithMin(privilege, dayMinBalance, holidaysCount, fromDate, toDate, maxBalancePerDay);

            privilege = CalcWithoutMin(balance, privilege, maxBalancePerDay, fromDate, toDate, reductionRatio, holidaysCount);

            return privilege;
        }

        private static decimal CalcPrivilegeWithMin(decimal privilege, decimal dayMinBalance, int holidaysCount, long fromDate, long toDate, decimal maxBalancePerDay)
        {
            if (maxBalancePerDay > 0 && maxBalancePerDay < (Math.Abs(dayMinBalance)))
            {
                privilege = CalcPrivilegeWithReductionWithMin(dayMinBalance, privilege, fromDate, toDate, maxBalancePerDay, 0.4m, holidaysCount);
            }
            else
            {
                privilege = CalcPrivilegeNormalWithMin(privilege, dayMinBalance, holidaysCount);
            }

            return privilege;
        }

        private static decimal CalcPrivilegeWithReductionWithMin(decimal balance, decimal privilege, long fromDate, long toDate, decimal maxBalancePerDay, decimal reductionRatio, int holidaysCount)
        {
            checked
            {
                privilege = CalcPrivilegeNormalWithMin(privilege, maxBalancePerDay, holidaysCount);

                var exessAmount = Math.Abs(balance) - maxBalancePerDay;

                return privilege + CalcPrivilage(exessAmount, holidaysCount) * reductionRatio;
            }
        }

        private static decimal CalcPrivilegeNormalWithMin(decimal privilege, decimal balance, int days)
        {
            checked
            {
                return privilege + CalcPrivilage(balance, days);
            }
        }

        private static decimal CalcWithoutMin(decimal balance, decimal privilege, decimal maxBalancePerDay, long fromDate, long toDate, decimal reductionRatio, int holidaysCount)
        {
            if (maxBalancePerDay > 0 && maxBalancePerDay < Math.Abs(balance))
            {
                privilege = CalcPrivilegeWithReduction(privilege, balance, fromDate, toDate, maxBalancePerDay, reductionRatio, holidaysCount);
            }
            else
            {
                privilege = CalcPrivilegeNormal(privilege, balance, holidaysCount, fromDate, toDate);
            }

            return privilege;
        }

        private static decimal CalcPrivilegeWithReduction(decimal privilege, decimal balance, long fromDate, long toDate, decimal maxBalancePerDay, decimal reductionRatio, int holidaysCount)
        {
            checked
            {
                privilege = CalcPrivilegeNormalWithMin(privilege, maxBalancePerDay, holidaysCount);

                var exessAmount = Math.Abs(balance) - maxBalancePerDay;

                return privilege + CalcPrivilage(exessAmount, fromDate, toDate, holidaysCount) * reductionRatio;
            }
        }

        private static decimal CalcPrivilegeNormal(decimal privilege, decimal balance, int holidaysCount, long fromDate, long toDate)
        {
            checked
            {
                return privilege + CalcPrivilage(balance, fromDate, toDate, holidaysCount);
            }
        }

        protected static decimal GetMinDayBalance(long theDate, decimal untilDateBalance, decimal dayMinActivity, IEnumerable<ActivitiesTimeGroup> activitiesTimeGroups)
        {
            checked
            {
                var stepBalance = untilDateBalance;
                foreach (var dayActivity in activitiesTimeGroups)
                {
                    stepBalance = stepBalance + dayActivity.Balance;
                    if (Math.Abs(dayMinActivity) > Math.Abs(stepBalance)) dayMinActivity = stepBalance;
                }
                return dayMinActivity;
            }
        }

        protected static IEnumerable<ActivitiesTimeGroup> GroupActivitiesOfDayByTime(IEnumerable<AccountActivityDto> accountActivityDtos, long theDate)
        {
            var activities = accountActivityDtos.Where(a => a.RegDate == theDate)
                .GroupBy(a => a.RegTime)
                .Select(n =>
                    new ActivitiesTimeGroup
                    {
                        Time = n.Key,
                        Balance = n.Sum(b => (decimal?)b.Amount * (decimal?)b.TransactionType) ?? Decimal.Zero
                    })
                .OrderBy(a => a.Time)
                .ToList();
            return activities;
        }
    }

    public class AccountAveDto
    {
        /// <summary> شناسه حساب مورد معدل </summary>
        public Guid AccountId { get; set; }

        /// <summary> تاریخ افتتاح حساب </summary>
        public Int64 BeginDate { get; set; }

        /// <summary>موجودی</summary>
        public decimal Balance { get; set; }

        /// <summary>موجودی</summary>
        public decimal MinBalance { get; set; }

        /// <summary>مبلغ بدهکار</summary>
        public decimal Debtor { get; set; }

        /// <summary>معدل</summary>
        public decimal Privilege { get; set; }

        /// <summary>تاریخ ابتدا ی معدلگیری در صورتیکه تاریخ وارد شده توسط کاربر از تاریخ اولین عملیات کوچکتر باشد</summary>
        public long FromDate { get; set; }

        /// <summary>پیغام</summary>
        public int? ReturnMessage { get; set; }
    }

    public class ActivitiesTimeGroup
    {
        public int Time { get; set; }

        public decimal Balance { get; set; }
    }
}

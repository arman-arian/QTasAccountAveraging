using System;
using System.Collections.Generic;
using System.Linq;
using AveragingService.Domain;

namespace AveragingService.AveragingModule
{
    public class AveragingWithLastBalancePerDayStrategy : AveragingStrategy
    {
        /// <summary>محاسبه معدل</summary>
        protected override AverageResult CalcAverage(List<AccountActivityDto> accountActivityDtos, long fromDate, long toDate, long duration, decimal balance, decimal sumOfDebit, decimal sumOfCredit, decimal maxBalancePerDay, decimal reductionRatio)
        {
            //گروه بندی سند بر اساس تاریخ
            var activitiesDayGroups = GroupActivitiesByDate(accountActivityDtos);

            var privilege = 0m;
            var minBalance = balance;
            var theDate = fromDate;

            if (minBalance == 0)
                minBalance = activitiesDayGroups.Select(db => db.DayBalance).FirstOrDefault();

            // به ازای هر روز دارای عملیات
            foreach (var dayBalance in activitiesDayGroups)
            {
                //محاسبه گردش بدهکار
                sumOfDebit = Sum(sumOfDebit, dayBalance.DayDebtor);

                //محاسبه گردش بستانکار
                sumOfCredit = Sum(sumOfCredit, dayBalance.DayCreditor);

                //امتیاز تا آن روز
                privilege = CalcPrivilege(balance, privilege, maxBalancePerDay, dayBalance.Date, theDate, reductionRatio);

                //مانده تا آن روز
                balance = Sum(balance, dayBalance.DayBalance);

                //محاسبه کمترین مانده تا آن روز
                minBalance = CalcMinBalance(minBalance, balance);

                //جلو بردن تاریخ ابتدا در هر محاسبه
                theDate = dayBalance.Date;
            }

            //اگر در پیمایش عملیات یک حساب تاریخ آخرین عملیات با تاریخ پایان دوره معدل گیری منطبق نبود
            if (theDate </*=*/ toDate)
            {
                privilege = CalcPrivilege(balance, privilege, maxBalancePerDay, toDate, theDate, reductionRatio);
            }

            //محاسبه معدل
            var averageAmount = CalcAverageAmount(privilege, duration);

            //ایجاد نتیجه
            return CreateResult(balance, minBalance, privilege, sumOfCredit, sumOfDebit, averageAmount);
        }
    }
}
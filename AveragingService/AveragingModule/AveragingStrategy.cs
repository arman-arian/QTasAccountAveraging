using System;
using System.Collections.Generic;
using System.Linq;
using AveragingService.Domain;

namespace AveragingService.AveragingModule
{
    public abstract class AveragingStrategy
    {
        protected AveragingStrategy() { }

        protected AveragingStrategy(string name)
        {
            Name = name;
        }

        /// <summary>شناسه استراتژی</summary>
        public int Id { get; set; }

        /// <summary>نام استراتژی</summary>
        public string Name { get; set; }

        /// <summary>محاسبه امتیاز</summary>
        /// <returns></returns>
        /// <summary>محاسبه معدل</summary>
        public virtual AverageResult CalcAverage(AveragingDto averagingDto)
        {
            if (averagingDto.AccountActivityDtos.Count != 0)
            {
                return CalcAverage(averagingDto.AccountActivityDtos, averagingDto.FromDate, averagingDto.ToDate, averagingDto.Duration,
                    averagingDto.UntilDateBalance, averagingDto.UntilDateDebtor, averagingDto.UntilDateDeposit,
                    averagingDto.MaxBalancePerDay, averagingDto.ReductionRatio);
            }

            return CalcAverageWithoutTransaction(averagingDto.FromDate, averagingDto.ToDate, averagingDto.Duration,
                averagingDto.UntilDateBalance, averagingDto.MaxBalancePerDay, averagingDto.UntilDateDebtor,
                averagingDto.UntilDateDeposit, averagingDto.ReductionRatio);
        }

        protected abstract AverageResult CalcAverage(List<AccountActivityDto> accountActivityDtos, long fromDate,
            long toDate, long duration, decimal balance, decimal sumOfDebit, decimal sumOfCredit,
            decimal maxBalancePerDay, decimal reductionRatio);

        /// <summary>اگر در یک بازه بخواهیم معدل گیری کنیم و در این بازه حساب هیچ عملیات نداشته</summary>
        protected AverageResult CalcAverageWithoutTransaction(long fromDate, long toDate, long duration, decimal balance, decimal sumOfDebit, decimal sumOfCredit, decimal maxBalancePerDay, decimal reductionRatio)
        {
            //امتیاز اولیه
            var privilege = 0m;

            //محاسبه امتیاز
            privilege = CalcPrivilege(balance, privilege, maxBalancePerDay, fromDate, toDate, reductionRatio);

            //محاسبه معدل
            var averageAmount = CalcAverageAmount(privilege, duration);

            //ایجاد نتیجه
            return CreateResult(balance, balance, privilege, sumOfCredit, sumOfDebit, averageAmount);
        }

        /// <summary>محاسبه امتیاز</summary>
        protected static decimal CalcPrivilege(decimal balance, decimal privilege, decimal maxBalancePerDay, long fromDate, long toDate, decimal reductionRatio)
        {
            // جدا کردن نحوه محاسبه سقف مجاز و بیشتر از آن
            if ((maxBalancePerDay > 0) && (maxBalancePerDay < (Math.Abs(balance))))
            {
                privilege = CalcPrivilegeWithReduction(balance, privilege, fromDate, toDate, maxBalancePerDay, reductionRatio);
            }
            else
            {
                privilege = CalcPrivilegeNormal(balance, privilege, fromDate, toDate);
            }

            return privilege;
        }

        /// <summary>محاسبه امتیاز به صورت معمول</summary>
        protected static decimal CalcPrivilegeNormal(decimal balance, decimal privilege, long fromDate, long toDate)
        {
            checked
            {
                return privilege + CalcPrivilage(balance, fromDate, toDate);
            }
        }

        /// <summary>محاسبه امتیاز با F6</summary>
        protected static decimal CalcPrivilegeWithReduction(decimal balance, decimal privilege, long fromDate, long toDate, decimal maxBalancePerDay, decimal reductionRatio)
        {
            checked
            {
                privilege = CalcPrivilegeNormal(maxBalancePerDay, privilege, fromDate, toDate);

                // مقدار بیش از سقف مجاز در دو پنجم ضرب می شود
                var excessAmount = Math.Abs(balance) - maxBalancePerDay;

                return privilege + CalcPrivilage(excessAmount, fromDate, toDate) * reductionRatio;
            }
        }

        /// <summary>گروه بندی تراکنش های یک تاریخ</summary>
        /// <param name="accountActivityDtos"></param>
        /// <returns></returns>
        protected static List<ActivitiesDayGroup> GroupActivitiesByDate(IEnumerable<AccountActivityDto> accountActivityDtos)
        {
            var activities = accountActivityDtos
                .GroupBy(a => a.RegDate)
                .Select(a => new ActivitiesDayGroup
                {
                    Date = a.Key,
                    DayBalance = a.Sum(b => (decimal?)b.Amount * b.TransactionType) ?? Decimal.Zero,
                    DayDebtor = a.Where(b => b.TransactionType == (short)TransactionType.Debit).Sum(b => (decimal?)b.Amount) ?? Decimal.Zero,
                    DayCreditor = a.Where(b => b.TransactionType == (short)TransactionType.Credit).Sum(b => (decimal?)b.Amount) ?? Decimal.Zero
                })
                .OrderBy(a => a.Date)
                .ToList();
            return activities;
        }

        /// <summary>ایجاد نتیجه نهایی</summary>
        protected static AverageResult CreateResult(decimal balance, decimal minBalance, decimal privilege, decimal sumOfCredit, decimal sumOfDebit, decimal averageAmount)
        {
            return new AverageResult
            {
                Balance = balance,
                MinBalance = minBalance,
                Privilege = privilege,
                SumOfCredit = sumOfCredit,
                SumOfDebit = sumOfDebit,
                AverageAmount = averageAmount
            };
        }

        /// <summary>محاسبه مبلغ معدل</summary>
        protected static decimal CalcAverageAmount(decimal privilege, long duration)
        {
            checked
            {
                return Math.Floor(privilege / duration);
            }
        }

        /// <summary>محاسبه کمترین مانده</summary>
        protected static decimal CalcMinBalance(decimal minBalance, decimal balance)
        {
            checked
            {
                return Math.Abs(minBalance) > Math.Abs(balance) ? balance : minBalance;
            }
        }

        /// <summary>عملیات جمع</summary>
        protected static decimal Sum(decimal A, decimal B)
        {
            checked
            {
                return A + B;
            }
        }

        /// <summary>محاسبه امتیاز</summary>
        protected static decimal CalcPrivilage(decimal balance, int days)
        {
            checked
            {
                return (Math.Abs(balance)) * days;
            }
        }

        /// <summary>محاسبه امتیاز</summary>
        protected static decimal CalcPrivilage(decimal balance, long fromDate, long toDate)
        {
            checked
            {
                var days = PDateClass.SubtractTwoDate(fromDate, toDate);
                return CalcPrivilage(balance, days);
            }
        }

        /// <summary>محاسبه امتیاز</summary>
        protected static decimal CalcPrivilage(decimal balance, long fromDate, long toDate, int difference)
        {
            checked
            {
                var days = PDateClass.SubtractTwoDate(fromDate, toDate) - difference;
                return CalcPrivilage(balance, days);
            }
        }
    }
}

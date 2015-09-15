using System;
using System.Globalization;

namespace AveragingService
{
    public class PDateClass
    {
        /// <summary>(بازیابی تاریخ چند روز آینده (نسبت به تاریخ ابتدایی</summary>
        /// <param name="date">تاریخ ابتدایی</param>
        /// <param name="days">تعداد روز موردنظر</param>
        /// <returns>تاریخ چند روز آینده</returns>
        public static long GetNextDaysDate(long date, int days)
        {
            var gregorianDate = ShamsiToGregorian(date); // تاریخ میلادی
            gregorianDate = gregorianDate.AddDays(days);
            return Gregorian2Shamsi(gregorianDate);
        }

        public static long MaxValue
        {
            get { return 99999999; }
        }

        public static long MinValue
        {
            get { return 10000101; }
        }

        public static short ThisYear
        {
            get
            {
                // تاریخ روز
                var today = GetNowDate();

                // جداسازی سال تاریخ روز
                var year = today.ToString().Substring(0, 4);
                return Int16.Parse(year);
            }
        }

        /// <summary>تاریخ دیروز</summary>
        public static long Yesterday
        {
            get
            {
                var today = GetNowDate();
                return GetPreviousDays(today, 1);
            }
        }

        /// <summary>بازیابی سال یک تاریخ خاص</summary>
        /// <param name="date">تاریخ موردنظر</param>
        /// <returns>سال تاریخ موردنظر</returns>
        public static long GetYear(long date)
        {
            // مثال: اگر تاریخ ورودی 13940105 باشد، خروجی این متد 1394 می باشد
            return (date / 10000);
        }

        /// <summary>بازیابی سال گذشته یک تاریخ خاص</summary>
        /// <param name="date">تاریخ موردنظر</param>
        /// <returns>سال گذشته تاریخ موردنظر</returns>
        public static long GetLastYear(long date)
        {
            // مثال: اگر تاریخ ورودی 13940105 باشد، خروجی این متد 1393 می باشد
            var thisYear = (date / 10000);
            return thisYear - 1;
        }

        /// <summary>بازیابی تاریخ ابتدای تاریخ موردنظر</summary>
        /// <param name="date">تاریخ موردنظر</param>
        /// <returns>تاریخ ابتدای تاریخ موردنظر</returns>
        public static long GetFirstDateOfYear(long date)
        {
            // مثال: اگر تاریخ ورودی 13931005 باشد، خروجی این متد 13930101 می باشد
            var thisYear = GetYear(date);
            return thisYear * 10000 + 101; //تاریخ ابتدا 13930101
        }

        /// <summary>(بازیابی تاریخ چند روز گذشته (نسبت به تاریخ ابتدایی</summary>
        /// <param name="date">تاریخ ابتدایی</param>
        /// <param name="days">تعداد روز موردنظر</param>
        /// <returns>تاریخ چند روز گذشته</returns>
        public static long GetPreviousDays(long date, int days)
        {
            var gregorianDate = ShamsiToGregorian(date); // تاریخ میلادی
            gregorianDate = gregorianDate.AddDays(days*-1);
            return Gregorian2Shamsi(gregorianDate);
        }

        /// <summary>(بازیابی تاریخ چند ماه گذشته (نسبت به تاریخ ابتدایی</summary>
        /// <param name="date">تاریخ ابتدایی</param>
        /// <param name="months">تعداد ماه موردنظر</param>
        /// <returns>تاریخ چند ماه گذشته</returns>
        public static long GetPreviousMonths(long date, int months)
        {
            var gregorianDate = ShamsiToGregorian(date); // تاریخ میلادی
            gregorianDate = gregorianDate.AddMonths(months * -1);
            return Gregorian2Shamsi(gregorianDate);
        }

        /// <summary>(بازیابی تاریخ چند سال گذشته (نسبت به تاریخ ابتدایی</summary>
        /// <param name="date">تاریخ ابتدایی</param>
        /// <param name="years">تعداد سال موردنظر</param>
        /// <returns>تاریخ چند سال گذشته</returns>
        public static long GetPreviousYears(long date, int years)
        {
            var gregorianDate = ShamsiToGregorian(date); // تاریخ میلادی
            gregorianDate = gregorianDate.AddYears(years * -1);
            return Gregorian2Shamsi(gregorianDate);
        }

        /// <summary>(بازیابی تاریخ چند روز آینده (نسبت به تاریخ ابتدایی</summary>
        /// <param name="date">تاریخ ابتدایی</param>
        /// <param name="days">تعداد روز موردنظر</param>
        /// <returns>تاریخ چند روز آینده</returns>
        public static long GetNextDays(long date, int days)
        {
            var gregorianDate = ShamsiToGregorian(date); // تاریخ میلادی
            gregorianDate = gregorianDate.AddDays(days);
            return Gregorian2Shamsi(gregorianDate);
        }

        /// <summary>(بازیابی تاریخ چند ماه آینده (نسبت به تاریخ ابتدایی</summary>
        /// <param name="date">تاریخ ابتدایی</param>
        /// <param name="months">تعداد ماه موردنظر</param>
        /// <returns>تاریخ چند ماه آینده</returns>
        public static long GetNextMonths(long date, int months)
        {
            var gregorianDate = ShamsiToGregorian(date); // تاریخ میلادی
            gregorianDate = gregorianDate.AddMonths(months);
            return Gregorian2Shamsi(gregorianDate);
        }

        /// <summary>(بازیابی تاریخ چند سال آینده (نسبت به تاریخ ابتدایی</summary>
        /// <param name="date">تاریخ ابتدایی</param>
        /// <param name="years">تعداد سال موردنظر</param>
        /// <returns>تاریخ چند سال آینده</returns>
        public static long GetNextYears(long date, int years)
        {
            var gregorianDate = ShamsiToGregorian(date); // تاریخ میلادی
            gregorianDate = gregorianDate.AddYears(years);
            return Gregorian2Shamsi(gregorianDate);
        }

        /// <summary>تبدیل تاریخ شمسی به میلادی</summary>
        /// <param name="shamsiDate">تاریخ شمسی</param>
        /// <returns>تاریخ میلادی</returns>
        public static DateTime ShamsiToGregorian(long shamsiDate)
        {
            var pc = new PersianCalendar();
            var year = Convert.ToInt32(shamsiDate.ToString(CultureInfo.InvariantCulture).Substring(0, 4));
            var month = Convert.ToInt32(shamsiDate.ToString(CultureInfo.InvariantCulture).Substring(4, 2));
            var day = Convert.ToInt32(shamsiDate.ToString(CultureInfo.InvariantCulture).Substring(6, 2));
            return pc.ToDateTime(year, month, day, 1, 1, 1, 1, 1);
        }

        /// <summary>تبدیل تاریخ میلادی به شمسی</summary>
        /// <param name="gregorianDate">تاریخ میلادی</param>
        /// <returns>تاریخ شمسی</returns>
        public static long Gregorian2Shamsi(DateTime gregorianDate)
        {
            var pc = new PersianCalendar();
            var y = Convert.ToString(pc.GetYear(gregorianDate));
            var m = Convert.ToString(pc.GetMonth(gregorianDate));
            if (m.Length == 1) m = "0" + m;
            var d = Convert.ToString(pc.GetDayOfMonth(gregorianDate));
            if (d.Length == 1) d = "0" + d;
            return Convert.ToInt64(y + m + d);
        }

        /// <summary>بررسی معتبر بودن تاریخ شمسی</summary>
        /// <param name="shamsiDate">تاریخ شمسی</param>
        /// <returns>برمی گرداند True درصورت معتبر بودن، مقدار</returns>
        public static bool IsValidShamsiDate(long shamsiDate)
        {
            //TODO: پیاده سازی شود
          //  throw new NotImplementedException();
            return true;
        }

        /// <summary>بررسی معتبر بودن تاریخ میلادی</summary>
        /// <param name="gregorianDate">تاریخ میلادی</param>
        /// <returns>برمی گرداند True درصورت معتبر بودن، مقدار</returns>
        public static bool IsValidGregorianDate(DateTime gregorianDate)
        {
            //TODO: پیاده سازی شود
            //throw new NotImplementedException();
            return true;
        }

        /// <summary>Long به String تبدیل فرمت تاریخ شمسی از</summary>
        /// <param name="shamsiDate">String تاریخ شمسی با فرمت</param>
        /// <returns>Long تاریخ شمسی با فرمت</returns>
        public static long ConvertDateFormatToLong(string shamsiDate)
        {
            //TODO: پیاده سازی شود
            throw new NotImplementedException();
      
        }

        /// <summary>String به Long تبدیل فرمت تاریخ شمسی از</summary>
        /// <param name="shamsiDate">Long تاریخ شمسی با فرمت</param>
        /// <returns>String تاریخ شمسی با فرمت</returns>
        public static string ConvertDateFormatToString(long shamsiDate)
        {
            //TODO: پیاده سازی شود
            throw new NotImplementedException();
        }

        /// <summary>بازیابی تعداد روزهای یک ماه خاص</summary>
        /// <param name="year">سال موردنظر - جهت بررسی کبیسه بودن یا نبودن سال</param>
        /// <param name="month">شماره ماه موردنظر</param>
        /// <returns>تعداد روزهای ماه موردنظر</returns>
        public static int GetDaysOfMonth(int year, int month)
        {
            if (month >= 1 && month <= 6)
            {
                return 31;
            }

            if (month >= 7 && month <= 11)
            {
                return 30;
            }

            if (month == 12)
            {
                var persianCalendar = new PersianCalendar();

                // بررسی کبیسه بودن یا نبودن سال موردنظر
                var isLeapYear = persianCalendar.IsLeapYear(year);

                // اگر سال موردنظر کبیسه باشد
                if (isLeapYear)
                {
                    return 30;
                }

                return 29;
            }

            return -1;
        }

        /// <summary>محاسبه اختلاف روزهای یک بازه تاریخی</summary>
        /// <param name="fromDate">تاریخ ابتدایی</param>
        /// <param name="toDate">تاریخ انتهایی</param>
        /// <returns>اختلاف روز</returns>
        public static int SubtractTwoDate(long fromDate, long toDate)
        {
            return Math.Abs((ShamsiToGregorian(toDate) - ShamsiToGregorian(fromDate)).Days);
        }

        /// <summary>محاسبه سن شخص</summary>
        /// <param name="birthDate">تاریخ تولد شخص</param>
        /// <returns>سن شخص</returns>
        public static int CalcAgeOfPerson(long birthDate)
        {
            string strOwnerBirth = Convert.ToString(birthDate);
            //سال تولد
            int yearBirth = Convert.ToInt32(strOwnerBirth.Substring(0, 4));
            //ماه تولد
            int monthBirth = Convert.ToInt32(strOwnerBirth.Substring(4, 2));
            //روز تولد
            int dayBirth = Convert.ToInt32(strOwnerBirth.Substring(6, 2));

            //تاریخ امروز
            string[] shamsiToday = Miladi2Shomsi(DateTime.Now.ToShortDateString()).Split('/');

            //سال امروز
            int yearToday = Convert.ToInt32(shamsiToday[0]);
            //ماه امروز
            int monthToday = Convert.ToInt32(shamsiToday[1]);
            //روز امروز
            int dayToday = Convert.ToInt32(shamsiToday[2]);

            int age;
            //اگر سال تولد با امسال برابر باشد-اختلاف این ماه با ماه تولد کوچکتر از صفر باشد- اختلاف امروز با روز تولد کوچکتر از صفر باشد =>سن یکسال کوچکتر ار اختلاف سال تولد می شود
            if (yearToday - yearBirth == 0 && monthToday - monthBirth < 0 || dayToday - dayBirth < 0)
                age = yearToday - yearBirth - 1;
            else
                age = yearToday - yearBirth;
            return age;
        }











        public static string GetStrNowTime()
        {
            var md = DateTime.Now;
            var h = md.Hour.ToString(CultureInfo.InvariantCulture);
            if (h.Length == 1) h = "0" + h;

            var m = md.Minute.ToString(CultureInfo.InvariantCulture);
            if (m.Length == 1) m = "0" + m;
            var time = h + m;

            var s = md.Minute.ToString(CultureInfo.InvariantCulture);
            if (s.Length == 1) s = "0" + s;
            time = time + s;

            var hh = time.Substring(0, 2);
            var mm = time.Substring(2, 2);
            var ss = time.Substring(4, 2);

            return string.Format("{0}:{1}:{2}", hh, mm, ss);
        }

        public static int ConvertMonthToDay(int month)
        {
            return month * 30;
        }

        public static int FiveYearsAgo(long date)
        {
            var milayDate = ShamsiToGregorian(date).ToShortDateString(); // تاریخ میلادی
            return SubtractDays(milayDate, 1825); //روز1825 ==> 5سال
        }

        public static int SixMonthAgo(long date)
        {
            var milayDate = ShamsiToGregorian(date).ToShortDateString(); // تاریخ میلادی
            return SubtractDays(milayDate, 180); //روز180 ==> 6 ماه
        }

        //بیشترین مقداری که به تاریخ اولین قسط می توان اضافه کرد یک ماه و بیست روز بعد تاریخ واریز می باشد
        public static int GetMaxFirstPaymentDate(long shamsiDate)
        {
            var miladiLoanDate = ShamsiToGregorian(shamsiDate).ToShortDateString();
            return AddDays(miladiLoanDate, 50);
        }

        public static string Miladi2Shomsi(string miladi) //8/8/2014
        {
            var pc = new PersianCalendar();
            DateTime md;
            if (!DateTime.TryParse(miladi, out md)) md = DateTime.Now;
            var y = Convert.ToString(pc.GetYear(md));
            var m = Convert.ToString(pc.GetMonth(md));
            if (m.Length == 1) m = "0" + m;
            var sh = y + "/" + m;
            var d = Convert.ToString(pc.GetDayOfMonth(md));
            if (d.Length == 1) d = "0" + d;
            sh = sh + "/" + d;
            return sh;
        }

        public static long Miladi2ShomsiLong(string miladi)
        {
            var pc = new PersianCalendar();
            DateTime md;
            if (!DateTime.TryParse(miladi, out md)) md = DateTime.Now;
            var y = Convert.ToString(pc.GetYear(md));
            var m = Convert.ToString(pc.GetMonth(md));
            if (m.Length == 1) m = "0" + m;
            var d = Convert.ToString(pc.GetDayOfMonth(md));
            if (d.Length == 1) d = "0" + d;
            return Convert.ToInt64(y + m + d);
        }

        public static string Miladi2Shomsi(DateTime miladi)
        {
            var pc = new PersianCalendar();
            var y = Convert.ToString(pc.GetYear(miladi));
            var m = Convert.ToString(pc.GetMonth(miladi));
            if (m.Length == 1) m = "0" + m;
            var sh = y + "/" + m;
            var d = Convert.ToString(pc.GetDayOfMonth(miladi));
            if (d.Length == 1) d = "0" + d;
            sh = sh + "/" + d;
            return sh;
        }

        public static long ToLongDate(string date)
        {
            Int64 d;
            return Int64.TryParse(date, out d) ? d : 0;
        }

        public static long GetNowDate()
        {
            var pc = new PersianCalendar();
            var md = DateTime.Now;
            var y = Convert.ToString(pc.GetYear(md));
            var m = Convert.ToString(pc.GetMonth(md));
            if (m.Length == 1) m = "0" + m;
            var sh = y + m;
            var d = Convert.ToString(pc.GetDayOfMonth(md));
            if (d.Length == 1) d = "0" + d;
            sh = sh + d;

            return Int64.Parse(sh);
        }

        public static int GetNowTime()
        {
            var md = DateTime.Now;
            var h = md.Hour.ToString(CultureInfo.InvariantCulture);
            if (h.Length == 1) h = "0" + h;

            var m = md.Minute.ToString(CultureInfo.InvariantCulture);
            if (m.Length == 1) m = "0" + m;
            var sh = h + m;

            var s = md.Minute.ToString(CultureInfo.InvariantCulture);
            if (s.Length == 1) s = "0" + s;
            sh = sh + s;

            return Int32.Parse(sh);
        }

        public static int Miladi2ShomsiInt(string miladi)
        {
            int i;
            string sh = Miladi2Shomsi(miladi);
            sh = sh.Replace("/", "");
            int.TryParse(sh, out i);
            return i;
        }

        public static string Shamsi2Miladi(string shamsi)
        {
            var pc = new PersianCalendar();
            var sd = shamsi.Split('/');
            var md = pc.ToDateTime(Convert.ToInt32(sd[0]), Convert.ToInt32(sd[1]), Convert.ToInt32(sd[2]), 1, 1, 1, 1, 1);
            var sh = md.ToShortDateString();
            return sh;
        }

        public static string ShamsitoMiladi(string shamsi)
        {
            var pc = new PersianCalendar();
            var y = shamsi.Substring(0, 4);
            var m = shamsi.Substring(4, 2);
            var d = shamsi.Substring(6, 2);
            var date = pc.ToDateTime(Convert.ToInt32(y), Convert.ToInt32(m), Convert.ToInt32(d), 1, 1, 1, 1, 1)
                            .ToString(CultureInfo.InvariantCulture);
            return date;
        }





        public static string AddShamsiYears(string shamsi, int years)
        {
            DateTime md;
            var sh = Shamsi2Miladi(shamsi);
            DateTime.TryParse(sh, out md);
            md = md.AddYears(years);
            sh = md.ToShortDateString();
            sh = Miladi2Shomsi(sh);
            return sh;
        }

        public static string AddShamsiMonths(string shamsi, int months)
        {
            DateTime md;
            var sh = ShamsitoMiladi(shamsi);
            DateTime.TryParse(sh, out md);
            md = md.AddMonths(months);
            sh = md.ToShortDateString();
            sh = Miladi2Shomsi(sh);
            return sh;
        }

        public static string AddShamsiDays(string shamsi, int days)
        {
            DateTime md;
            var sh = Shamsi2Miladi(shamsi);
            DateTime.TryParse(sh, out md);
            md = md.AddDays(days);
            sh = md.ToShortDateString();
            sh = Miladi2Shomsi(sh);
            return sh;
        }

        public static long AddShamsiDays(long shamsi, int days)
        {
            var md = ShamsiToGregorian(shamsi);
            md = md.AddDays(days);
            return Miladi2ShomsiLong(md.ToShortDateString());
        }

        public static int SubtractDays(string miladi, int days)
        {
            DateTime dtMiladi;
            if (!DateTime.TryParse(miladi, out dtMiladi)) return -1;

            dtMiladi = dtMiladi.AddDays(days * -1);
            return Miladi2ShomsiInt(dtMiladi.ToShortDateString());
        }

        public static int AddDays(string miladi, int days)
        {
            DateTime dtMiladi;
            DateTime.TryParse(miladi, out dtMiladi);
            dtMiladi = dtMiladi.AddDays(days * 1);
            return Miladi2ShomsiInt(dtMiladi.ToShortDateString());
        }

        public static int ShamsiCompare(string d1, string d2)
        {
            DateTime dd1, dd2;
            string sh = Shamsi2Miladi(d1);
            DateTime.TryParse(sh, out dd1);
            sh = Shamsi2Miladi(d2);
            DateTime.TryParse(sh, out dd2);
            sh = (dd1 - dd2).ToString();
            string[] r = sh.Split('.');
            int i;
            if (!int.TryParse(r[0], out i)) i = 0;
            return i;
        }

        public static bool ValidDate(string strDate)
        {
            DateTime md;
            return DateTime.TryParse(strDate, out md);
        }

        public static string MakeDate(string strDate)
        {
            var st = string.Format("{0:####/##/##}", Convert.ToInt32(RefineDate(strDate)));
            return ValidDate(st) ? st : strDate;
            //MessageBox.Show("تاريخ وارد شده اشتباه است");
        }

        public static string RefineDate(string strDate)
        {
            return strDate.Replace("/", "");
        }

        /// <summary>محاسبه تفاضل تاریخ روز پرداخت و تاریخ سررسید قسط برای محاسبه تاخیری و تعجیلی</summary>
        /// <param name="fromDate">تاریخ روز پرداخت قسط</param>
        /// <param name="toDate">تاریخ سررسید پرداخت قسط</param>
        /// <returns>مقدار تفاضل دو تاریخ</returns>
        public static int TwoDaySubtractForSpeedyDelay(long fromDate, long toDate)
        {
            return (ShamsiToGregorian(toDate) - ShamsiToGregorian(fromDate)).Days;
        }

    }

}

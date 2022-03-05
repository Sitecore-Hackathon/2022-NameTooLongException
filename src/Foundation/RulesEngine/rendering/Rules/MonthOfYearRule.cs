using System;

namespace Mvp.Foundation.RulesEngine.Rules
{
    public class MonthOfYearRule : Rule
    {
        public string Month { get; set; }

        public override bool Execute()
        {
            var monthOfYear = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month).ToLower();
            return Month.ToLower().Equals(monthOfYear);
        }
    }
}

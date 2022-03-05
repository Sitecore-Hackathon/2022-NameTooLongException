using System;
using System.Linq;

namespace Mvp.Foundation.RulesEngine.Rules
{
    public class DayOfWeekRule : Rule
    {
        public string[] ListOfDays { get; set; } = Array.Empty<string>();

        public override bool Execute()
        {
            var currentDay = DateTime.Now.DayOfWeek.ToString();
            return ListOfDays.Contains(currentDay);
        }
    }
}

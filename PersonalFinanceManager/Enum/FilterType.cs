using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceManager.Enum
{
    public enum FilterType
    {
        All,
        Today,
        Yesterday,
        ThisWeek,
        LastWeek,
        ThisMonth,
        LastMonth,
        ThisYear,
        LastYear,
        CustomRange
    }

    public enum SortType
    {
        DateNewest,
        DateOldest,
        AmountHigh,
        AmountLow,
        Category,
        Account
    }
}

﻿namespace Mvp.Foundation.RulesEngine.Utils
{
    public static class StringExtensions
    {
		public static string Mid(this string text, int start)
		{
			if (start >= text.Length || start < 0)
			{
				return string.Empty;
			}
			return text.Substring(start);
		}

		public static string Left(this string text, int length)
		{
			if (length <= 0)
			{
				return string.Empty;
			}
			if (text.Length <= length)
			{
				return text;
			}
			return text.Substring(0, length);
		}
	}
}

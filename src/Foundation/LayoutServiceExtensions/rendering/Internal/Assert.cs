using System;
using System.Diagnostics;

namespace Mvp.Foundation.LayoutServiceExtensions.Internal
{
	internal static class Assert
	{
		[DebuggerStepThrough]
		internal static T ArgumentNotNull<T>(T argument, string name)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(name);
			}
			return argument;
		}

		[DebuggerStepThrough]
		internal static string ArgumentNotNullOrWhitespace(string argument, string name)
		{
			if (string.IsNullOrWhiteSpace(argument))
			{
				throw new ArgumentNullException(name);
			}
			return argument;
		}

		[DebuggerStepThrough]
		internal static T NotNull<T>(T value, string? message = null)
		{
			if (value == null)
			{
				throw new NullReferenceException(message);
			}
			return value;
		}
	}
}

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Sitecore.LayoutService.Client;

namespace Mvp.Foundation.LayoutServiceExtensions.Internal
{
	[GeneratedCode("GenerateResourceClass.ps1", "0.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[ExcludeFromCodeCoverage]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("Sitecore.LayoutService.Client.Resources", typeof(Resources).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture { get; set; }

		internal static string Exception_AbstractRegistrationsMustProvideFactory => GetString("Exception_AbstractRegistrationsMustProvideFactory");

		internal static string Exception_HandlerNameIsNull => GetString("Exception_HandlerNameIsNull");

		internal static string HttpStatusCode_KeyName => GetString("HttpStatusCode_KeyName");

		internal Resources()
		{
		}

		internal static string Exception_HandlerRegistryKeyNotFound(object handlerName)
		{
			return string.Format(Culture, GetString("Exception_HandlerRegistryKeyNotFound"), handlerName);
		}

		internal static string Exception_RegisterTypesOfService(object serviceType)
		{
			return string.Format(Culture, GetString("Exception_RegisterTypesOfService"), serviceType);
		}

		private static string GetString(string key)
		{
			return ResourceManager.GetString(key, Culture);
		}
	}
}

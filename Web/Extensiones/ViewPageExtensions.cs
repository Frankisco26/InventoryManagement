using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

// ReSharper disable CheckNamespace

namespace Microsoft.AspNetCore.Mvc
{
	public static class ViewPageExtensions
	{
		private const string ScriptblockBuilder = "ScriptBlockBuilder";

		/// <summary>
		/// Determina si la petición actual es AJAX.
		/// </summary>
		public static bool IsAjax(this RazorPage webPage)
		{
			ArgumentNullException.ThrowIfNull(webPage);

			return webPage.Context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}

		/// <summary>
		/// Acumula un bloque de script de un parcial para escribirlo al final del layout.
		/// </summary>
		public static HtmlString ScriptBlock(this RazorPage webPage, Func<dynamic, HelperResult> template)
		{
			return Block(webPage, template, ScriptblockBuilder);
		}

		public static HtmlString WriteScriptBlocks(this RazorPage webPage)
		{
			var blockBuilder = webPage.Context.Items[ScriptblockBuilder] as StringBuilder ?? new StringBuilder();

			return new HtmlString(blockBuilder.ToString());
		}

		private static HtmlString Block(RazorPage webPage, Func<dynamic, HelperResult> template, string block)
		{
			var writer = new StringWriter();
			template(null!).WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);

			if (webPage.IsAjax())
				return new HtmlString(writer.ToString());

			var blockBuilder = webPage.Context.Items[block] as StringBuilder ?? new StringBuilder();
			blockBuilder.Append(writer);
			webPage.Context.Items[block] = blockBuilder;

			return new HtmlString(string.Empty);
		}
	}
}

namespace Microsoft.AspNetCore.Mvc.Rendering
{
	public static class ViewContextExtensions
	{
		public static bool IsPost(this ViewContext viewContext)
		{
			return viewContext.HttpContext.Request.Method == "POST";
		}
	}
}

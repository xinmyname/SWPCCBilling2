using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Nancy;
using Nancy.Helpers;
using Nancy.Responses;

namespace SWPCCBilling2.Infrastructure
{
	public class EmbeddedStaticContentConventionBuilder
	{
		private static readonly ConcurrentDictionary<string, Func<Response>> ResponseFactoryCache;
		private static readonly Regex PathReplaceRegex = new Regex(@"[/\\]", RegexOptions.Compiled);

		static EmbeddedStaticContentConventionBuilder()
		{
			ResponseFactoryCache = new ConcurrentDictionary<string, Func<Response>>();
		}

		/// <summary>
		/// Adds a directory-based convention for embedded static convention.
		/// </summary>
		/// <param name="requestedPath">The path that should be matched with the request.</param>
		/// <param name="contentPath">The path to where the content is stored in your application, relative to the root. If this is <see langword="null" /> then it will be the same as <paramref name="requestedPath"/>.</param>
		/// <param name="allowedExtensions">A list of extensions that is valid for the conventions. If not supplied, all extensions are valid.</param>
		/// <returns>A <see cref="EmbeddedFileResponse"/> instance for the requested embedded static contents if it was found, otherwise <see langword="null"/>.</returns>
		public static Func<NancyContext, string, Response> AddDirectory(string requestedPath, Assembly assembly, string contentPath = null, params string[] allowedExtensions)
		{
			return (ctx, root) =>
			{
				var path =
					HttpUtility.UrlDecode(ctx.Request.Path);

				var fileName =
					Path.GetFileName(path);

				if (string.IsNullOrEmpty(fileName))
				{
					return null;
				}

				var pathWithoutFilename =
					GetPathWithoutFilename(fileName, path);

				if (!requestedPath.StartsWith("/"))
				{
					requestedPath = string.Concat("/", requestedPath);
				}

				if (!pathWithoutFilename.StartsWith(requestedPath, StringComparison.OrdinalIgnoreCase))
				{
					ctx.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[EmbeddedStaticContentConventionBuilder] The requested resource '", path, "' does not match convention mapped to '", requestedPath, "'")));
					return null;
				}

				contentPath =
					GetContentPath(requestedPath, contentPath);

				var responseFactory =
					ResponseFactoryCache.GetOrAdd(path, BuildContentDelegate(ctx, requestedPath, contentPath, assembly, allowedExtensions));

				return responseFactory.Invoke();
			};
		}

		private static Func<string, Func<Response>> BuildContentDelegate(NancyContext context, string requestedPath, string contentPath, Assembly assembly, string[] allowedExtensions)
		{
			return requestPath =>
			{
				string resPath = "SWPCCBilling2" + requestPath
					.Replace("_", "-")
					.Replace("/", ".");

				string resName = Path.GetFileName(requestPath);

				resPath = resPath.Substring(0, resPath.Length - (resName.Length + 1));

				return () => new EmbeddedFileResponse(assembly, resPath, resName);
			};
		}

		private static string GetEncodedPath(string path)
		{
			return PathReplaceRegex.Replace(path.TrimStart(new[] { '/' }), Path.DirectorySeparatorChar.ToString());
		}

		private static string GetSafeRequestPath(string requestPath, string requestedPath, string contentPath)
		{
			var actualContentPath =
				(contentPath.Equals("/") ? string.Empty : contentPath);

			if (requestedPath.Equals("/"))
			{
				return string.Concat(actualContentPath, requestPath);
			}

			var expression =
				new Regex(Regex.Escape(requestedPath), RegexOptions.IgnoreCase);

			return expression.Replace(requestPath, actualContentPath, 1);
		}

		private static string GetContentPath(string requestedPath, string contentPath)
		{
			contentPath =
				contentPath ?? requestedPath;

			if (!contentPath.StartsWith("/"))
			{
				contentPath = string.Concat("/", contentPath);
			}

			return contentPath;
		}

		private static string GetPathWithoutFilename(string fileName, string path)
		{
			var pathWithoutFileName =
				path.Replace(fileName, string.Empty);

			return (pathWithoutFileName.Equals("/")) ?
				pathWithoutFileName :
				pathWithoutFileName.TrimEnd(new[] { '/' });
		}

		/// <summary>
		/// Returns whether the given filename is contained within the content folder
		/// </summary>
		/// <param name="contentRootPath">Content root path</param>
		/// <param name="fileName">Filename requested</param>
		/// <returns>True if contained within the content root, false otherwise</returns>
		private static bool IsWithinContentFolder(string contentRootPath, string fileName)
		{
			return fileName.StartsWith(contentRootPath, StringComparison.Ordinal);
		}
	}
}
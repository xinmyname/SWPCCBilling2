using System;
using System.Net.Sockets;
using System.Net;

namespace SWPCCBilling2
{
	public class UrlFactory
	{
		private static UrlFactory _defaultUrlFactory;
		private static int _port;

		public static UrlFactory DefaultUrlFactory
		{
			get 
			{
				if (_defaultUrlFactory == null)
					_defaultUrlFactory = new UrlFactory();

				return _defaultUrlFactory;
			}
		}

		static UrlFactory()
		{
			var listener = new TcpListener(IPAddress.Any, 0);
			listener.Start();
			_port = ((IPEndPoint)listener.LocalEndpoint).Port;
			listener.Stop();
		}

		public string BaseUrl { get; private set; }

		public UrlFactory()
		{
			BaseUrl = String.Format("http://localhost:{0}", _port);
		}

		public Uri BaseUri
		{
			get { return new Uri(BaseUrl); }
		}

		public string UrlForPath(string path)
		{
			return String.Format("{0}/{1}", BaseUrl, path);
		}
	}
}


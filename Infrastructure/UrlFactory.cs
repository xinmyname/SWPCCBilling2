using System;
using System.Net.Sockets;
using System.Net;

namespace SWPCCBilling2
{
	public class UrlFactory
	{
		private static UrlFactory _defaultUrlFactory;
		private static int _port;
		private static IPAddress _address;

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
			_address = ((IPEndPoint)listener.LocalEndpoint).Address;
			listener.Stop();
		}

		public string BaseUrl { get; private set; }

		public UrlFactory()
		{
			BaseUrl = String.Format("http://{0}:{1}", _address, _port);
		}

		public Uri BaseUri
		{
			get { return new Uri(BaseUrl); }
		}

		public string UrlForPath(string path)
		{
			return String.Format("{0}/{1}", BaseUrl, path);
		}

		public string UrlForPath(string format, params object[] args)
		{
			return String.Format("{0}/{1}", BaseUrl, 
				String.Format(format, args));
		}
	}
}


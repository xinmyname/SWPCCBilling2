using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace SWPCCBilling2.Models
{
	public class MICR
	{
		public string RoutingNumber { get; private set; }
		public string AccountNumber { get; private set; }
		public string CheckNumber { get; private set; }
		public bool IsValid { get; private set; }
		public string SHA256 { get; private set; }

		public MICR(string text)
		{
			try
			{
				bool routeFirst = text[0] == 'T';

				text = text.Replace(" ", "");
				text = text.Replace('T', ',');
				text = text.Replace('U', ',');
				text = text.Trim(',');
				text = text.Replace(",,", ",");

				string[] segments = text.Split(',');

				if (routeFirst)
				{
					RoutingNumber = segments[0];
					AccountNumber = segments[1];
					CheckNumber = segments[2].TrimStart('0');
				}
				else
				{
					CheckNumber = segments[0].TrimStart('0');
					RoutingNumber = segments[1];
					AccountNumber = segments[2];
				}

				string routeAccount = RoutingNumber + AccountNumber;
				var crypt = new SHA256Managed();
				byte[] sha256Bytes = crypt.ComputeHash(Encoding.ASCII.GetBytes(routeAccount));
				var sha256Text = new StringBuilder();

				foreach(byte b in sha256Bytes)
					sha256Text.AppendFormat("{0:x2}", b);

				SHA256 = sha256Text.ToString();

				IsValid = true;
			}
			catch (Exception)
			{
				IsValid = false;
			}
		}

		public override string ToString()
		{
			return string.Format("R{0} A{1} #{2}", RoutingNumber, AccountNumber, CheckNumber);
		}
	}
}

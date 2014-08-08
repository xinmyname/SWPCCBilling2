using System;
using System.Text;

namespace SWPCCBilling2.Infrastructure
{

	public static class ConsoleEx
	{
		public static ConsoleKeyInfo TranslateKey()
		{
			ConsoleKeyInfo keyInfo = Console.ReadKey(true);

			if (keyInfo.Key == 0 && keyInfo.KeyChar == '~' && keyInfo.Modifiers == ConsoleModifiers.Alt)
			{
				keyInfo = Console.ReadKey(true);
				if (keyInfo.Key == ConsoleKey.D3 && keyInfo.KeyChar == '3' && keyInfo.Modifiers == 0)
				{
					keyInfo = Console.ReadKey(true);
					if (keyInfo.Key == 0 && keyInfo.KeyChar == '~' && keyInfo.Modifiers == 0)
					{
						return new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false);
					}
				}
			}

			return keyInfo;
		}
	}
	
}

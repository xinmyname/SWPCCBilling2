using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SWPCCBilling2.Infrastructure;
using System.Text;

namespace SWPCCBilling2.Infrastructure
{
	public interface ICompleteText
	{
		string Trunk { get; set; }
		void Preload();
		string Next();
	}
	
}

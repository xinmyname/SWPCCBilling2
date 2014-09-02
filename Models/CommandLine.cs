using System;
using System.Collections.Generic;
using SWPCCBilling2.Infrastructure;
using System.Text;

namespace SWPCCBilling2.Models
{
	public class CommandLine
	{
		public ActionInfo ActionInfo { get; private set; }
		public object[] Parameters { get; private set; }
		public IList<string> Errors { get; private set; }
		public bool Quit { get; set; }

		public CommandLine(ActionInfo actionInfo, object[] parameters, IList<string> errors)
		{
			ActionInfo = actionInfo;
			Parameters = parameters;
			Errors = errors;
			Quit = false;
		}

		public bool HasErrors
		{
			get { return Errors.Count > 0; }
		}
	}
	
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace SWPCCBilling2
{
	public interface ICompleteText
	{
		string Trunk { get; set; }
		string Next();
	}

	public class ActionCompletion : ICompleteText
	{
		public string _trunk;
		public IList<string> _allActions;
		public IList<string> _filteredActions;
		public int _curIndex;

		public ActionCompletion()
		{
			_allActions = new []
			{
				"set-database",
				"set-email-server",
				"set-email-port",
				"set-email-secure",
				"set-email-from",
				"import-families",
				"show-family",
				"remove-family",
				"import-micr",
				"import-fees",
				"show-fee",
				"remove-fee",
				"import-discounts",
				"show-discount",
				"remove-discount",
				"debit",
				"credit-fee",
				"credit-payment",
				"scan-payment",
				"deposit-payment",
				"show-payment",
				"open-invoice",
				"close-invoice",
				"send-invoice",
				"show-invoice",
				"rebuild-invoice",
				"remove-invoice",
				"report-month",
				"report-unpaid",
				"report-payments"
			};

			_filteredActions = _allActions;
			_curIndex = 0;
		}

		public string Trunk
		{
			get { return _trunk; }
			set 
			{
				_trunk = value;

				_filteredActions = _allActions
					.Where(s => s.StartsWith(_trunk, StringComparison.InvariantCultureIgnoreCase))
					.ToList();

				_curIndex = 0;
			}
		}

		public string Next()
		{
			string next = null;

			if (_filteredActions.Count > 0) 
			{
				next = _filteredActions[_curIndex];

				_curIndex++;

				if (_curIndex == _filteredActions.Count)
					_curIndex = 0;
			}

			return next;
		}
	}
}


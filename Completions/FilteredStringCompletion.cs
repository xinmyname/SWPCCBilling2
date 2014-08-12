using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SWPCCBilling2.Infrastructure;
using System.Text;

namespace SWPCCBilling2.Completions
{

	public class FilteredStringCompletion : ICompleteText
	{
		private string _trunk;
		private IList<string> _allStrings;
		private IList<string> _filteredStrings;
		private int _curIndex;

		public FilteredStringCompletion()
		{
			_allStrings = new List<string>();
			_filteredStrings = _allStrings;
			_curIndex = 0;
		}

		protected IList<string> AllStrings
		{
			get { return _allStrings; }
			set
			{  
				_allStrings = new List<string>(value);
				_filteredStrings = _allStrings;
				_curIndex = 0;
			}
		}

		public string Trunk
		{
			get { return _trunk.ToString(); }
			set
			{ 
				_trunk = value; 
				Refilter();
			}
		}

		public void Preload()
		{
		}

		public string Next()
		{
			string next = null;

			if (_filteredStrings.Count > 0) 
			{
				next = _filteredStrings[_curIndex];

				_curIndex++;

				if (_curIndex == _filteredStrings.Count)
					_curIndex = 0;
			}

			return next;
		}

		private void Refilter()
		{
			_filteredStrings = _allStrings
				.Where(s => s.StartsWith(_trunk.ToString(), StringComparison.InvariantCultureIgnoreCase))
				.ToList();

			_curIndex = 0;
		}
	}
}

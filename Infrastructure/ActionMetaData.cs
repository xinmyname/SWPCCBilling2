using System;
using System.Reflection;
using System.Collections.Generic;
using SWPCCBilling2.Infrastructure;
using System.Linq;

namespace SWPCCBilling2
{
	public class ActionParam
	{
		public string Name { get; set; }
		public Type ParamType { get; set; }
		public Type CompletionType { get; set; }
	}

	public class ActionInfo
	{
		public string Name { get; set; }
		public Type ControllerType { get; set; }
		public MethodInfo ActionMethod { get; set; }
		public IList<ActionParam> Parameters { get; set; }
	}

	public class ActionMetaData
	{
		private readonly IDictionary<string, ActionInfo> _allActions;
		private static ActionMetaData _defaultActionMetaData;

		public static ActionMetaData DefaultActionMetaData
		{
			get 
			{
				if (_defaultActionMetaData == null)
					_defaultActionMetaData = new ActionMetaData();

				return _defaultActionMetaData;
			}
		}

		private ActionMetaData()
		{
			_allActions = new Dictionary<string, ActionInfo>();

			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (!type.IsSubclassOf(typeof(Controller)))
					continue;

				foreach (MethodInfo methodInfo in type.GetMethods())
				{
					ActionAttribute actionAttr = methodInfo.GetCustomAttributes(false)
						.Where(attr => attr.GetType() == typeof(ActionAttribute))
						.Cast<ActionAttribute>()
						.SingleOrDefault();

					if (actionAttr != null)
						_allActions[actionAttr.Name] = BuildActionInfo(type, methodInfo, actionAttr);
				}
			}
		}

		public ActionInfo GetAction(string name)
		{
			if (!_allActions.ContainsKey(name))
				return null;

			return _allActions[name];
		}

		public IEnumerable<ActionInfo> GetAllActions()
		{
			return _allActions.Values;
		}

		private ActionInfo BuildActionInfo(Type controllerType, MethodInfo methodInfo, ActionAttribute actionAttr)
		{
			return new ActionInfo 
			{
				Name = actionAttr.Name,
				ControllerType = controllerType,
				ActionMethod = methodInfo,
				Parameters = BuildActionParameters(methodInfo).ToList()
			};
		}

		private IEnumerable<ActionParam> BuildActionParameters(MethodInfo methodInfo)
		{
			foreach (ParameterInfo paramInfo in methodInfo.GetParameters())
			{
				Type completionType = typeof(NoCompletion);

				CompleteWithAttribute completeWithAttr = paramInfo.GetCustomAttributes(false)
					.Where(attr => attr.GetType() == typeof(CompleteWithAttribute))
					.Cast<CompleteWithAttribute>()
					.SingleOrDefault();

				if (completeWithAttr != null)
					completionType = completeWithAttr.Type;

				yield return new ActionParam
				{
					Name = paramInfo.Name,
					ParamType = paramInfo.ParameterType,
					CompletionType = completionType
				};
			}
		}
	}
}


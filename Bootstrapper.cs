﻿using System;
using System.Collections.Generic;
using SWPCCBilling2.Infrastructure;
using System.Text;
using SWPCCBilling2.Models;
using System.Reflection;
using System.Linq;
using Nancy.Hosting.Self;
using Nancy.Conventions;
using Nancy.ViewEngines;
using Nancy.Bootstrapper;
using Nancy;

namespace SWPCCBilling2
{
	class Bootstrapper : DefaultNancyBootstrapper
	{
		public static void Main(string[] args)
		{
			try
			{
				new Bootstrapper().Run(args);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		private readonly UrlFactory _urlFactory;

		public Bootstrapper()
		{
			_urlFactory = UrlFactory.DefaultUrlFactory;
		}

		public void Run(string[] args)
		{
			NancyHost host = null;

			var cmdLineFactory = new CommandLineFactory(ActionMetaData.DefaultActionMetaData);

			string combinedArgs = null;

			if (args.Length > 0)
				combinedArgs = args.Aggregate((cur, next) => cur + " " + next);
			else
			{
				host = new NancyHost(this, _urlFactory.BaseUri);
				host.Start();

				Console.WriteLine("Listening on {0}", _urlFactory.BaseUrl);
			}

			foreach (CommandLine cmdLine in cmdLineFactory.Acquire(combinedArgs))
			{
				if (cmdLine.HasErrors)
				{
					foreach (string error in cmdLine.Errors)
						Console.WriteLine("ERROR: {0}", error);
				}
				else if (cmdLine.ActionInfo != null)
				{
					ActionInfo actionInfo = cmdLine.ActionInfo;
					object controller = Activator.CreateInstance(actionInfo.ControllerType);

					try
					{
						actionInfo.ActionMethod.Invoke(controller, cmdLine.Parameters);
					}
					catch (TargetInvocationException ex)
					{
						Console.WriteLine("ERROR: {0}", ex.InnerException.Message);
					}
					catch (Exception ex)
					{
						Console.WriteLine("ERROR: {0}", ex.Message);
					}

				}
			}

			if (host != null)
				host.Stop();

			Console.WriteLine("Done!");
		}

		protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
		{
			base.ConfigureApplicationContainer(container);

			// More stuff here...
		}

		protected override void ConfigureConventions(NancyConventions conventions)
		{
			base.ConfigureConventions(conventions);

			conventions.StaticContentsConventions.Add(
				EmbeddedStaticContentConventionBuilder.AddDirectory(
					"/vendor",
					GetType().Assembly,
					"vendor"));

			conventions.StaticContentsConventions.Add(
				EmbeddedStaticContentConventionBuilder.AddDirectory(
					"/css",
					GetType().Assembly,
					"css"));

			ResourceViewLocationProvider
				.RootNamespaces
				.Add(GetType().Assembly, "SWPCCBilling2.Views");
		}

		protected override NancyInternalConfiguration InternalConfiguration
		{
			get { return NancyInternalConfiguration.WithOverrides(OnConfigurationBuilder); }
		}

		void OnConfigurationBuilder(NancyInternalConfiguration x)
		{
			x.ViewLocationProvider = typeof(ResourceViewLocationProvider);
		}
	}
}

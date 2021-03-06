﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util;
using Akka.Visualize.Clients;

namespace Akka.Visualize
{
	/// <summary>
	/// The extension class registered with an Akka.NET <see cref="ActorSystem"/>
	/// </summary>
	public class ActorVisualizeExtension : ExtensionIdProvider<ActorVisualize>
	{
		protected static ConcurrentSet<string> ActorSystemsWithLogger = new ConcurrentSet<string>();
		protected static object LoggerLock = new object();

		public override ActorVisualize CreateExtension(ExtendedActorSystem system)
		{
			try
			{
				//ActorSystem does not have a monitor logger yet
				if (!ActorSystemsWithLogger.Contains(system.Name))
				{
					lock (LoggerLock)
					{
						if (ActorSystemsWithLogger.TryAdd(system.Name))
						{
							system.ActorOf<ActorVisualizeLogger>(ActorVisualizeLogger.Name);
						}
					}
				}

			}
			catch { }
			return new ActorVisualize(system);
		}

		#region Static methods

		public static void InstallVisualizer(ActorSystem system, IActorVisualizeClient client)
		{
			system
				.WithExtension<ActorVisualize, ActorVisualizeExtension>()
				.RegisterMonitor(client);
		}

		///// <summary>
		///// Register a new <see cref="AbstractActorVisualizeingClient"/> instance to use when monitoring Actor operations.
		///// </summary>
		///// <returns>true if the monitor was succeessfully registered, false otherwise.</returns>
		//public static bool RegisterMonitor(ActorSystem system, AbstractActorVisualizeingClient client)
		//{
		//	return Monitors(system).RegisterMonitor(client);
		//}

		///// <summary>
		///// Deregister an existing <see cref="AbstractActorVisualizeingClient"/> instance so it no longer reports metrics to existing Actors.
		///// </summary>
		///// <returns>true if the monitor was succeessfully deregistered, false otherwise.</returns>
		//public static bool DeregisterMonitor(ActorSystem system, AbstractActorVisualizeingClient client)
		//{
		//	return Monitors(system).DeregisterMonitor(client);
		//}

		///// <summary>
		///// Terminates all existing monitors. You can add new ones after this call has been made.
		///// </summary>
		//public static void TerminateMonitors(ActorSystem system)
		//{
		//	Monitors(system).TerminateMonitors();
		//}

		#endregion
	}
}

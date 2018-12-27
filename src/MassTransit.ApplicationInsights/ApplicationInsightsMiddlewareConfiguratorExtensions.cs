﻿// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.ApplicationInsights
{
	using GreenPipes;
	using Microsoft.ApplicationInsights;
	using Microsoft.ApplicationInsights.DataContracts;
	using Microsoft.ApplicationInsights.Extensibility;
	using System;

	public static class ApplicationInsightsDefaultConfiguration
	{
		public const string DefaultTelemetryHeaderRootKey = "TelemetryRootId";
		public const string DefaultTelemetryHeaderParentKey = "TelemetryParentId";
	}

	public static class ApplicationInsightsMiddlewareConfiguratorExtensions
	{
		/// <summary>
		/// Add support for ApplicationInsights to the pipeline, which will be used to track all consumer message reception.
		/// </summary>
		public static void UseConsumeApplicationInsightsTelemetry<T>(this IPipeConfigurator<T> configurator, TelemetryClient telemetryClient
			, Action<IOperationHolder<RequestTelemetry>, T> configureOperation = null
			, string telemetryHeaderRootKey = ApplicationInsightsDefaultConfiguration.DefaultTelemetryHeaderRootKey
			, string telemetryHeaderParentKey = ApplicationInsightsDefaultConfiguration.DefaultTelemetryHeaderParentKey)
			where T : class, ConsumeContext
		{
			configurator.AddPipeSpecification(new ApplicationInsightsConsumeSpecification<T>(telemetryClient, configureOperation, telemetryHeaderRootKey, telemetryHeaderParentKey));
		}

		/// <summary>
		/// Add support for ApplicationInsight to track all send message on the bus.
		/// </summary>
		public static void UseSendApplicationInsightsTelemetry(this ISendPipeConfigurator configurator, TelemetryClient telemetryClient
			, string telemetryHeaderRootKey = ApplicationInsightsDefaultConfiguration.DefaultTelemetryHeaderRootKey
			, string telemetryHeaderParentKey = ApplicationInsightsDefaultConfiguration.DefaultTelemetryHeaderParentKey)
		{
			configurator.AddPipeSpecification(new ApplicationInsightsSendSpecification<SendContext>(telemetryClient, telemetryHeaderRootKey, telemetryHeaderParentKey));
		}

		/// <summary>
		/// Add support for ApplicationInsight to track all published message on the bus.
		/// </summary>
		public static void UsePublishApplicationInsightsTelemetry(this IPublishPipeConfigurator configurator, TelemetryClient telemetryClient
			, string telemetryHeaderRootKey = ApplicationInsightsDefaultConfiguration.DefaultTelemetryHeaderRootKey
			, string telemetryHeaderParentKey = ApplicationInsightsDefaultConfiguration.DefaultTelemetryHeaderParentKey)
		{
			configurator.AddPipeSpecification(new ApplicationInsightsPublishSpecification<PublishContext>(telemetryClient, telemetryHeaderRootKey, telemetryHeaderParentKey));
		}

		/// <summary>
		/// Add support for ApplicationInsights to the pipeline, which will be used to track all message publication.
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="telemetryClient">Telemetry client</param>
		/// <param name="configureOperation">Add additional information to operation</param>
		/// <typeparam name="T"></typeparam>
		public static void UseApplicationInsightsOnPublish<T>(this IPipeConfigurator<T> configurator,
			TelemetryClient telemetryClient,
			Action<IOperationHolder<DependencyTelemetry>, T> configureOperation = null)
			where T : class, PublishContext
		{

			configurator.AddPipeSpecification(new ApplicationInsightsPublishSpecification<T>(telemetryClient, configureOperation));
		}

		/// <summary>
		/// Add support for ApplicationInsights to the pipeline, which will be used to track all message publication.
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="telemetryClient">Telemetry client</param>
		/// <param name="configureOperation">Add additional information to operation</param>
		public static void UseApplicationInsightsOnPublish(this IPublishPipelineConfigurator configurator,
			TelemetryClient telemetryClient,
			Action<IOperationHolder<DependencyTelemetry>, PublishContext> configureOperation = null)
		{
			configurator.ConfigurePublish(pipeConfigurator =>
			{
				var specification = new ApplicationInsightsPublishSpecification<PublishContext>(telemetryClient, configureOperation);
				pipeConfigurator.AddPipeSpecification(specification);
			});
		}
	}
}
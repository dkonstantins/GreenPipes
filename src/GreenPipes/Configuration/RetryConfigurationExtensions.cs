﻿// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace GreenPipes
{
    using System;
    using System.Linq;
    using Configurators;
    using Policies;
    using Policies.ExceptionFilters;
    using Specifications;


    public static class RetryConfigurationExtensions
    {
        static readonly IExceptionFilter _all = new AllExceptionFilter();

        public static void UseRetry<T>(this IPipeConfigurator<T> configurator, Action<IRetryConfigurator> configure)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new RetryPipeSpecification<T>();

            configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Create an immediate retry policy with the specified number of retries, with no
        /// delay between attempts.
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IRetryConfigurator None(this IRetryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new NoRetryPolicy());

            return configurator;
        }

        /// <summary>
        /// Create an immediate retry policy with the specified number of retries, with no
        /// delay between attempts.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryLimit">The number of retries to attempt</param>
        /// <returns></returns>
        public static IRetryConfigurator Immediate(this IRetryConfigurator configurator, int retryLimit)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new ImmediateRetryPolicy(filter, retryLimit));

            return configurator;
        }

        /// <summary>
        /// Create an interval retry policy with the specified intervals. The retry count equals
        /// the number of intervals provided
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="intervals">The intervals before each subsequent retry attempt</param>
        /// <returns></returns>
        public static IRetryConfigurator Intervals(this IRetryConfigurator configurator, params TimeSpan[] intervals)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new IntervalRetryPolicy(filter, intervals));

            return configurator;
        }

        /// <summary>
        /// Create an interval retry policy with the specified intervals. The retry count equals
        /// the number of intervals provided
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="intervals">The intervals before each subsequent retry attempt</param>
        /// <returns></returns>
        public static IRetryConfigurator Intervals(this IRetryConfigurator configurator, params int[] intervals)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new IntervalRetryPolicy(filter, intervals));

            return configurator;
        }

        /// <summary>
        /// Create an interval retry policy with the specified number of retries at a fixed interval
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryCount">The number of retry attempts</param>
        /// <param name="interval">The interval between each retry attempt</param>
        /// <returns></returns>
        public static IRetryConfigurator Interval(this IRetryConfigurator configurator, int retryCount, TimeSpan interval)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new IntervalRetryPolicy(filter, Enumerable.Repeat(interval, retryCount).ToArray()));

            return configurator;
        }

        /// <summary>
        /// Create an interval retry policy with the specified number of retries at a fixed interval
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryCount">The number of retry attempts</param>
        /// <param name="interval">The interval between each retry attempt</param>
        /// <returns></returns>
        public static IRetryConfigurator Interval(this IRetryConfigurator configurator, int retryCount, int interval)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new IntervalRetryPolicy(filter, Enumerable.Repeat(interval, retryCount).ToArray()));

            return configurator;
        }

        /// <summary>
        /// Create an exponential retry policy with the specified number of retries at exponential
        /// intervals
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryLimit"></param>
        /// <param name="minInterval"></param>
        /// <param name="maxInterval"></param>
        /// <param name="intervalDelta"></param>
        /// <returns></returns>
        public static IRetryConfigurator Exponential(this IRetryConfigurator configurator, int retryLimit, TimeSpan minInterval, TimeSpan maxInterval,
            TimeSpan intervalDelta)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new ExponentialRetryPolicy(filter, retryLimit, minInterval, maxInterval, intervalDelta));

            return configurator;
        }

        /// <summary>
        /// Create an incremental retry policy with the specified number of retry attempts with an incrementing
        /// interval between retries
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryLimit">The number of retry attempts</param>
        /// <param name="initialInterval">The initial retry interval</param>
        /// <param name="intervalIncrement">The interval to add to the retry interval with each subsequent retry</param>
        /// <returns></returns>
        public static IRetryConfigurator Incremental(this IRetryConfigurator configurator, int retryLimit, TimeSpan initialInterval, TimeSpan intervalIncrement)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetRetryPolicy(filter => new IncrementalRetryPolicy(filter, retryLimit, initialInterval, intervalIncrement));

            return configurator;
        }

        /// <summary>
        /// Retry all exceptions except for the exception types specified
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="exceptionTypes"></param>
        /// <returns></returns>
        public static IRetryConfigurator Except(this IRetryConfigurator configurator, params Type[] exceptionTypes)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetExceptionFilter(new ExceptExceptionFilter(exceptionTypes));

            return configurator;
        }

        /// <summary>
        /// Retry all exceptions except for the exception types specified
        /// </summary>
        /// <returns></returns>
        public static IRetryConfigurator Except<T1>(this IRetryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetExceptionFilter(new ExceptExceptionFilter(typeof(T1)));

            return configurator;
        }

        /// <summary>
        /// Retry all exceptions except for the exception types specified
        /// </summary>
        /// <returns></returns>
        public static IRetryConfigurator Except<T1, T2>(this IRetryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetExceptionFilter(new ExceptExceptionFilter(typeof(T1), typeof(T2)));

            return configurator;
        }

        /// <summary>
        /// Retry all exceptions except for the exception types specified
        /// </summary>
        /// <returns></returns>
        public static IRetryConfigurator Except<T1, T2, T3>(this IRetryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetExceptionFilter(new ExceptExceptionFilter(typeof(T1), typeof(T2), typeof(T3)));

            return configurator;
        }

        /// <summary>
        /// Retry only the exception types specified
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="exceptionTypes"></param>
        /// <returns></returns>
        public static IRetryConfigurator Selected(this IRetryConfigurator configurator, params Type[] exceptionTypes)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetExceptionFilter(new SelectedExceptionFilter(exceptionTypes));

            return configurator;
        }

        /// <summary>
        /// Retry only the exception types specified
        /// </summary>
        /// <returns></returns>
        public static IRetryConfigurator Selected<T1>(this IRetryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetExceptionFilter(new SelectedExceptionFilter(typeof(T1)));

            return configurator;
        }

        /// <summary>
        /// Retry only the exception types specified
        /// </summary>
        /// <returns></returns>
        public static IRetryConfigurator Selected<T1, T2>(this IRetryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetExceptionFilter(new SelectedExceptionFilter(typeof(T1), typeof(T2)));

            return configurator;
        }

        /// <summary>
        /// Retry only the exception types specified
        /// </summary>
        /// <returns></returns>
        public static IRetryConfigurator Selected<T1, T2, T3>(this IRetryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetExceptionFilter(new SelectedExceptionFilter(typeof(T1), typeof(T2), typeof(T3)));

            return configurator;
        }

        /// <summary>
        /// Retry all exceptions
        /// </summary>
        /// <returns></returns>
        public static IRetryConfigurator All(this IRetryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetExceptionFilter(_all);

            return configurator;
        }

        /// <summary>
        /// Filter an exception type
        /// </summary>
        /// <typeparam name="T">The exception type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="filter">The filter expression</param>
        /// <returns>True if the exception should be retried, otherwise false</returns>
        public static IRetryConfigurator Filter<T>(this IRetryConfigurator configurator, Func<T, bool> filter)
            where T : Exception
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.SetExceptionFilter(new FilterExceptionFilter<T>(filter));

            return configurator;
        }
    }
}
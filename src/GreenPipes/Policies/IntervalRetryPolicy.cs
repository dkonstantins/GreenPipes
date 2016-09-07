// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace GreenPipes.Policies
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Util;


    public class IntervalRetryPolicy :
        RetryPolicy
    {
        readonly IExceptionFilter _filter;

        public IntervalRetryPolicy(IExceptionFilter filter, params TimeSpan[] intervals)
        {
            if (intervals == null)
                throw new ArgumentNullException(nameof(intervals));
            if (intervals.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(intervals), "At least one interval must be specified");

            _filter = filter;

            Intervals = intervals;
        }

        public TimeSpan[] Intervals { get; }

        public IntervalRetryPolicy(IExceptionFilter filter, params int[] intervals)
        {
            if (intervals == null)
                throw new ArgumentNullException(nameof(intervals));
            if (intervals.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(intervals), "At least one interval must be specified");

            _filter = filter;
            Intervals = intervals.Select(x => TimeSpan.FromMilliseconds(x)).ToArray();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                Policy = "Interval",
                Limit = Intervals.Length,
                Intervals = Intervals
            });

            _filter.Probe(context);
        }

        public Task<bool> CanRetry(Exception exception, out RetryContext retryContext)
        {
            retryContext = new IntervalRetryContext(this, exception, 0);

            return Matches(exception) ? TaskUtil.True : TaskUtil.False;
        }

        public bool Matches(Exception exception)
        {
            return _filter.Match(exception);
        }

        public override string ToString()
        {
            return $"Interval (limit {Intervals.Length}, intervals {string.Join(";", Intervals.Take(5).Select(x => x.ToString()))})";
        }
    }
}
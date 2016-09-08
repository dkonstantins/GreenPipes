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
namespace GreenPipes.Filters
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class FilterObservable :
        Connectable<IFilterObserver>,
        IFilterObserver
    {
        public Task PreSend<T>(T context)
            where T : class, PipeContext
        {
            return ForEachAsync(x => x.PreSend(context));
        }

        public Task PostSend<T>(T context)
            where T : class, PipeContext
        {
            return ForEachAsync(x => x.PostSend(context));
        }

        public Task SendFault<T>(T context, Exception exception)
            where T : class, PipeContext
        {
            return ForEachAsync(x => x.SendFault(context, exception));
        }
    }


    public class FilterObservable<T> :
        Connectable<IFilterObserver<T>>,
        IFilterObserver<T>
        where T : class, PipeContext
    {
        public Task PreSend(T context)
        {
            return ForEachAsync(x => x.PreSend(context));
        }

        public Task PostSend(T context)
        {
            return ForEachAsync(x => x.PostSend(context));
        }

        public Task SendFault(T context, Exception exception)
        {
            return ForEachAsync(x => x.SendFault(context, exception));
        }

        public ConnectHandle Connect(IFilterObserver observer)
        {
            return base.Connect(new ObservableAdapter<T>(observer));
        }
    }
}
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
    public interface IDispatchObserverConnector<in TKey>
    {
        /// <summary>
        /// Connect an observer to the filter and/or pipe
        /// </summary>
        /// <param name="key"></param>
        /// <param name="observer"></param>
        /// <returns></returns>
        ConnectHandle ConnectObserver<T>(TKey key, IFilterObserver<T> observer)
            where T : class, PipeContext;
    }


    public interface IObserverConnector
    {
        /// <summary>
        /// Connect an observer to the filter and/or pipe
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        ConnectHandle ConnectObserver<T>(IFilterObserver<T> observer)
            where T : class, PipeContext;
    }


    public interface IObserverConnector<T>
        where T : class, PipeContext
    {
        /// <summary>
        /// Connect an observer to the filter and/or pipe
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        ConnectHandle ConnectObserver(IFilterObserver<T> observer);
    }
}
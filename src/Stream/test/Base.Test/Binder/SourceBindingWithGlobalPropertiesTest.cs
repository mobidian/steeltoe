﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Steeltoe.Common.Lifecycle;
using Steeltoe.Stream.Config;
using System.Threading.Tasks;
using Xunit;

namespace Steeltoe.Stream.Binder
{
    public class SourceBindingWithGlobalPropertiesTest : AbstractTest
    {
        [Fact]
        public async Task TestGlobalPropertiesSet()
        {
            var searchDirectories = GetSearchDirectories("MockBinder");
            var provider = CreateStreamsContainerWithDefaultBindings(
                searchDirectories,
                "spring.cloud.stream.default.contentType=application/json",
                "spring.cloud.stream.bindings.output.destination=ticktock",
                "spring.cloud.stream.default.producer.requiredGroups:0=someGroup",
                "spring.cloud.stream.default.producer.partitionCount=1",
                "spring.cloud.stream.bindings.output.producer.headerMode=none",
                "spring.cloud.stream.bindings.output.producer.partitionCount=4",
                "spring.cloud.stream.defaultBinder=mock")
                        .BuildServiceProvider();

            await provider.GetRequiredService<ILifecycleProcessor>().OnRefresh(); // Only starts Autostart

            var factory = provider.GetService<IBinderFactory>();
            Assert.NotNull(factory);
            var binder = factory.GetBinder(null);
            Assert.NotNull(binder);

            var bindingServiceProperties = provider.GetService<IOptions<BindingServiceOptions>>();
            Assert.NotNull(bindingServiceProperties.Value);
            var bindingProperties = bindingServiceProperties.Value.GetBindingOptions("output");
            Assert.NotNull(bindingProperties);
            Assert.Equal("application/json", bindingProperties.ContentType.ToString());
            Assert.Equal("ticktock", bindingProperties.Destination);
            Assert.NotNull(bindingProperties.Producer);
            Assert.Single(bindingProperties.Producer.RequiredGroups);
            Assert.Contains("someGroup", bindingProperties.Producer.RequiredGroups);
            Assert.Equal(4, bindingProperties.Producer.PartitionCount);
            Assert.Equal(HeaderMode.None, bindingProperties.Producer.HeaderMode);
        }
    }
}

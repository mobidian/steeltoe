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

using Microsoft.Extensions.Configuration;
using OpenTelemetry.Trace;
using Steeltoe.Management.OpenTelemetry.Trace;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Steeltoe.Management.Tracing.Test
{
    public class OpenCensusTracingTest
    {
        [Fact]
        public void InitializedWithDefaults()
        {
            var builder = new ConfigurationBuilder();
            var opts = new TracingOptions(null, builder.Build());

            var tracing = new OpenTelemetryTracing(opts);
            Assert.NotNull(tracing.Tracer);
        }

        [Fact]
        public void BindsConfigurationCorrectly()
        {
            var appsettings = new Dictionary<string, string>()
            {
                ["management:tracing:name"] = "foobar",
                ["management:tracing:ingressIgnorePattern"] = "pattern",
                ["management:tracing:egressIgnorePattern"] = "pattern",
                ["management:tracing:maxNumberOfAttributes"] = "100",
                ["management:tracing:maxNumberOfAnnotations"] = "100",
                ["management:tracing:maxNumberOfMessageEvents"] = "100",
                ["management:tracing:maxNumberOfLinks"] = "100",
                ["management:tracing:alwaysSample"] = "true",
                ["management:tracing:neverSample"] = "true",
                ["management:tracing:useShortTraceIds"] = "true",
            };

            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(appsettings);
            var opts = new TracingOptions(null, builder.Build());

            var tracing = new OpenTelemetryTracing(opts);

            Assert.NotNull(tracing.Tracer);

            appsettings = new Dictionary<string, string>()
            {
                ["management:tracing:name"] = "foobar",
                ["management:tracing:ingressIgnorePattern"] = "pattern",
                ["management:tracing:egressIgnorePattern"] = "pattern",
                ["management:tracing:maxNumberOfAttributes"] = "100",
                ["management:tracing:maxNumberOfAnnotations"] = "100",
                ["management:tracing:maxNumberOfMessageEvents"] = "100",
                ["management:tracing:maxNumberOfLinks"] = "100",
                ["management:tracing:alwaysSample"] = "true",
                ["management:tracing:useShortTraceIds"] = "true",
            };

            builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(appsettings);
            opts = new TracingOptions(null, builder.Build());

            tracing = new OpenTelemetryTracing(opts, null);
            Assert.NotNull(tracing.Tracer);
        }

        private class TestSampler : Sampler
        {
            public override string Description => throw new System.NotImplementedException();

            public override Decision ShouldSample(in SpanContext parentContext, in ActivityTraceId traceId, in ActivitySpanId spanId, string name, SpanKind spanKind, IDictionary<string, object> attributes, IEnumerable<Link> links)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}

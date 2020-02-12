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
using Steeltoe.Management.OpenTelemetry.Trace.Propagation;
using Steeltoe.Management.OpenTelemetryTracingBase.Test;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Steeltoe.Management.Tracing.Observer.Test
{
    public class HttpClientDesktopObserverTest : AbstractObserverTest
    {
        [Fact]
        public void ProcessEvent_IgnoresNulls()
        {
            var opts = GetOptions();
            var tracing = new OpenTelemetryTracing(opts, null);
            var obs = new HttpClientDesktopObserver(opts, tracing);
            obs.ProcessEvent(null, null);
        }

        [Fact]
        public void ProcessEvent_IgnoresMissingHttpRequestMessage()
        {
            var opts = GetOptions();
            var tracing = new OpenTelemetryTracing(opts, null);
            var obs = new HttpClientDesktopObserver(opts, tracing);
            obs.ProcessEvent(string.Empty, new object());
        }

        [Fact]
        public void ProcessEvent_IgnoresUnknownEvent()
        {
            var opts = GetOptions();
            var tracing = new OpenTelemetryTracing(opts, null);
            var obs = new HttpClientDesktopObserver(opts, tracing);
            obs.ProcessEvent(string.Empty, new { Request = GetHttpRequestMessage() });
        }

        [Fact]
        public void ShouldIgnore_ReturnsExpected()
        {
            var opts = GetOptions();
            var tracing = new OpenTelemetryTracing(opts, null);
            var obs = new HttpClientDesktopObserver(opts, tracing);

            Assert.True(obs.ShouldIgnoreRequest("/api/v2/spans"));
            Assert.True(obs.ShouldIgnoreRequest("/v2/apps/foobar/permissions"));
            Assert.True(obs.ShouldIgnoreRequest("/v2/apps/barfoo/permissions"));
            Assert.False(obs.ShouldIgnoreRequest("/api/test"));
            Assert.False(obs.ShouldIgnoreRequest("/v2/apps"));
        }

        [Fact]
        public void ProcessEvent_Stop_NoRespose()
        {
            var opts = GetOptions();
            var tracing = new OpenTelemetryTracing(opts, null);
            var obs = new HttpClientDesktopObserver(opts, tracing);
            var request = GetHttpRequestMessage();
            obs.ProcessEvent(HttpClientDesktopObserver.STOP_EVENT, new { Request = request });
            var span = GetCurrentSpan(tracing.Tracer);
            Assert.Null(span);
            Assert.False(obs.Pending.TryGetValue(request, out var context));
        }

        [Fact]
        public void ProcessEvent_StopEx_NothingStarted()
        {
            var opts = GetOptions();
            var tracing = new OpenTelemetryTracing(opts, null);
            var obs = new HttpClientDesktopObserver(opts, tracing);
            var request = GetHttpRequestMessage();
            obs.ProcessEvent(HttpClientDesktopObserver.STOPEX_EVENT, new { Request = request, StatusCode = HttpStatusCode.OK, Headers = new WebHeaderCollection() });
            var span = GetCurrentSpan(tracing.Tracer);
            Assert.Null(span);
            Assert.False(obs.Pending.TryGetValue(request, out var context));
        }

        [Fact]
        public void ProcessEvent_StopEx_PreviousStarted()
        {
            var opts = GetOptions();
            var tracing = new OpenTelemetryTracing(opts, null);
            var obs = new HttpClientDesktopObserver(opts, tracing);
            var request = GetHttpRequestMessage();
            obs.ProcessEvent(HttpClientDesktopObserver.START_EVENT, new { Request = request });

            var span = GetCurrentSpan(tracing.Tracer);
            Assert.NotNull(span);
            Assert.True(obs.Pending.TryGetValue(request, out var pendingSpan));
            Assert.NotNull(pendingSpan);
            Assert.Equal(span, pendingSpan);
            //Assert.NotNull(spanContext.ActiveScope);
            var spanInfo = SpanSdkHelper.GetSpanData(span);
            Assert.Equal("httpclient:/", spanInfo.SpanData.Name);

            var respHeaders = new WebHeaderCollection();
            respHeaders.Add("TEST", "Header");

            obs.ProcessEvent(HttpClientDesktopObserver.STOPEX_EVENT, new { Request = request, StatusCode = HttpStatusCode.OK, Headers = respHeaders });
            spanInfo = SpanSdkHelper.GetSpanData(span);

            Assert.True(spanInfo.HasEnded);
            Assert.False(obs.Pending.TryGetValue(request, out var pendingSpan2));

            var spanData = spanInfo.SpanData;
            var attributes = spanData.Attributes.ToDictionary(kv => kv.Key, kv => kv.Value);
            //Todo: Check Internal vs Client
            Assert.Equal(SpanKind.Internal, spanData.Kind);

            Assert.Equal("http://localhost:5555/", attributes[SpanAttributeConstants.HttpUrlKey]);
            Assert.Equal(HttpMethod.Get.ToString(), attributes[SpanAttributeConstants.HttpMethodKey]);
            Assert.Equal("localhost:5555", attributes[SpanAttributeConstants.HttpHostKey]);
            Assert.Equal("/", attributes[SpanAttributeConstants.HttpPathKey]);
            Assert.Equal("Header", attributes["http.request.TEST"]);
            Assert.Equal("Header", attributes["http.response.TEST"]);
            Assert.Equal((long)HttpStatusCode.OK, attributes[SpanAttributeConstants.HttpStatusCodeKey]);
        }

        [Fact]
        public void ProcessEvent_Start()
        {
            var opts = GetOptions();
            var tracing = new OpenTelemetryTracing(opts, null);
            var obs = new HttpClientDesktopObserver(opts, tracing);
            var request = GetHttpRequestMessage();
            obs.ProcessEvent(HttpClientDesktopObserver.START_EVENT, new { Request = request });

            var span = GetCurrentSpan(tracing.Tracer);
            Assert.NotNull(span);
            Assert.True(obs.Pending.TryGetValue(request, out var pendingSpan));
            Assert.NotNull(pendingSpan);
            Assert.Equal(span, pendingSpan);
            //Assert.NotNull(spanContext.ActiveScope);

            var spanInfo = SpanSdkHelper.GetSpanData(span);
            Assert.Equal("httpclient:/", spanInfo.SpanData.Name);

            Assert.NotNull(request.Headers.Get(B3Constants.XB3TraceId));
            Assert.NotNull(request.Headers.Get(B3Constants.XB3SpanId));
            Assert.Null(request.Headers.Get(B3Constants.XB3ParentSpanId));

            var spanId = request.Headers.Get(B3Constants.XB3SpanId);
            Assert.Equal(span.Context.SpanId.ToHexString(), spanId);

            var traceId = request.Headers.Get(B3Constants.XB3TraceId);
            var expected = GetTraceId(opts, span.Context);
            Assert.Equal(expected, traceId);

            //if (span.Context.TraceOptions.IsSampled)
            //{
            //    Assert.NotNull(request.Headers.Get(B3Constants.XB3Sampled));
            //}

            Assert.False(spanInfo.HasEnded);

            var spanData = spanInfo.SpanData;
            var attributes = spanData.Attributes.ToDictionary(kv => kv.Key, kv => kv.Value);
            Assert.Equal(SpanKind.Internal, spanData.Kind);
            Assert.Equal("http://localhost:5555/", attributes[SpanAttributeConstants.HttpUrlKey]);
            Assert.Equal(HttpMethod.Get.ToString(), attributes[SpanAttributeConstants.HttpMethodKey]);
            Assert.Equal("localhost:5555", attributes[SpanAttributeConstants.HttpHostKey]);
            Assert.Equal("/", attributes[SpanAttributeConstants.HttpPathKey]);
            Assert.Equal("Header", attributes["http.request.TEST"]);
        }

        [Fact]
        public void InjectTraceContext()
        {
            var opts = GetOptions();
            var tracing = new OpenTelemetryTracing(opts, null);
            var obs = new HttpClientDesktopObserver(opts, tracing);
            var request = GetHttpRequestMessage();

            //tracing.Tracer.SpanBuilder("MySpan").StartScopedSpan(out ISpan span);
            tracing.Tracer.StartActiveSpan("MySpan", out var span);

            obs.InjectTraceContext(request, null);

            Assert.NotNull(request.Headers.Get(B3Constants.XB3TraceId));
            Assert.NotNull(request.Headers.Get(B3Constants.XB3SpanId));
            Assert.Null(request.Headers.Get(B3Constants.XB3ParentSpanId));

            var spanId = request.Headers.Get(B3Constants.XB3SpanId);
            var spanInfo = SpanSdkHelper.GetSpanData(span);
       
            Assert.Equal(spanInfo.SpanData.Context.SpanId.ToHexString(), spanId);

            var traceId = request.Headers.Get(B3Constants.XB3TraceId);
            var expected = GetTraceId(opts, span.Context);
            Assert.Equal(expected, traceId);

            //TODO: Check for all IsSampled
            //if (span.Context.TraceOptions.IsSampled)
            //{
            //    Assert.NotNull(request.Headers.Get(B3Constants.XB3Sampled));
            //}
        }

        private HttpWebRequest GetHttpRequestMessage()
        {
            var m = WebRequest.CreateHttp("http://localhost:5555/");
            m.Method = HttpMethod.Get.Method;
            m.Headers.Add("TEST", "Header");
            return m;
        }

        private TracingOptions GetOptions()
        {
            var appsettings = new Dictionary<string, string>()
            {
                ["management:tracing:name"] = "foobar",
                ["management:tracing:alwaysSample"] = "true",
                ["management:tracing:useShortTraceIds"] = "true",
            };
            return GetOptions(appsettings);
        }

        private TracingOptions GetOptions(Dictionary<string, string> settings)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(settings);
            TracingOptions opts = new TracingOptions(null, builder.Build());
            return opts;
        }

        private string GetTraceId(TracingOptions options, SpanContext context)
        {
            var traceId = context.TraceId.ToHexString();
            if (traceId.Length > 16 && options.UseShortTraceIds)
            {
                traceId = traceId.Substring(traceId.Length - 16, 16);
            }

            return traceId;
        }
    }
}

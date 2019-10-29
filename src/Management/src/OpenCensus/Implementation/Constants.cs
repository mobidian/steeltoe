﻿// <copyright file="Resource.cs" company="OpenCensus Authors">
// Copyright 2018, OpenCensus Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of theLicense at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace OpenCensus.Implementation
{
    /// <summary>
    /// Constants enforced by OpenCensus specifications.
    /// </summary>
    internal class Constants
    {
        /// <summary>
        /// Maximum length of the resource type name.
        /// </summary>
        public const int MaxResourceTypeNameLength = 255;

        /// <summary>
        /// Special resource type name that is assigned if nothing else is detected.
        /// </summary>
        public const string GlobalResourceType = "Global";

        /// <summary>
        /// OpenCensus Resource Type Environment Variable Name.
        /// </summary>
        public const string ResourceTypeEnvironmentVariable = "OC_RESOURCE_TYPE";

        /// <summary>
        /// OpenCensus Resource Labels Environment Variable Name.
        /// </summary>
        public const string ResourceLabelsEnvironmentVariable = "OC_RESOURCE_LABELS";
    }
}

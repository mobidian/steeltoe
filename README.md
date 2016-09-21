# ASP.NET Core External Security Providers

This repo contains [ASP.NET Core External Security Providers](https://github.com/aspnet/Security) which simplify the process of connecting to Security services on CloudFoundry.  

Windows Master (Stable): [![AppVeyor Master](https://ci.appveyor.com/api/projects/status/w27a5c4x64pd7jyq?svg=true)](https://ci.appveyor.com/project/steeltoe/security)

Windows Dev (Less Stable): [![AppVeyor Dev](https://ci.appveyor.com/api/projects/status/w27a5c4x64pd7jyq/branch/dev?svg=true)](https://ci.appveyor.com/project/steeltoe/security/branch/dev)

Linux/OS X Master (Stable): [![Travis Master](https://travis-ci.org/SteelToeOSS/Security.svg?branch=master)](https://travis-ci.org/SteelToeOSS/Security)

Linux/OS X Dev (Less Stable):  [![Travis Dev](https://travis-ci.org/SteelToeOSS/Security.svg?branch=dev)](https://travis-ci.org/SteelToeOSS/Security)

# .NET Runtime & Framework Support
Like ASP.NET Core, the providers are intended to support both .NET 4.5.1+ and .NET Core (CoreCLR/CoreFX) runtimes. 

The providers are built and unit tested on Windows, Linux and OSX.

The providers and samples have been tested on .NET Core 1.0.0 Preview 2 SDK, .NET 4.5.1, and on ASP.NET Core 1.0.0.

# Usage
See the Readme for each provider for more details on how to make use of it in an application.

# Nuget Feeds
All new provider development is done on the dev branch. More stable versions of the providers can be found on the master branch. The latest prebuilt packages from each branch can be found on one of two MyGet feeds. Released version can be found on nuget.org.

[Development feed (Less Stable)](https://www.myget.org/gallery/steeltoedev) - https://www.myget.org/gallery/steeltoedev

[Master feed (Stable)](https://www.myget.org/gallery/steeltoemaster) - https://www.myget.org/gallery/steeltoemaster

[Release or Release Candidate feed](https://www.nuget.org/) - https://www.nuget.org/. 

# Building Packages & Running Tests - Windows
To build the packages on windows:

1. git clone ...
2. cd `<clone directory>`
3. Install .NET Core SDK
4. dotnet restore --configfile nuget.config src
5. cd src\ `<project>` (e.g. cd src\Steeltoe.Security.Authentication.CloudFoundry)
6. dotnet pack --configuration `<Release or Debug>` 

The resulting artifacts can be found in the bin folder under the corresponding project. (e.g. src\Steeltoe.Security.Authentication.CloudFoundry\bin)

To run the unit tests:

1. git clone ...
2. cd `<clone directory>`
3. Install .NET Core SDK 
4. dotnet restore --configfile nuget.config test
5. cd test\ `<test project>` (e.g. cd test\Steeltoe.Security.Authentication.CloudFoundry.Test)
6. dotnet test

# Building Packages & Running Tests - Linux/OSX
To build the packages on Linux/OSX: 

1. git clone ...
2. cd `<clone directory>`
3. Install .NET Core SDK
4. dotnet restore --configfile nuget.config src
5. cd src/ `<project>` (e.g.. cd src/Steeltoe.Security.Authentication.CloudFoundry)
6. dotnet pack --configuration `<Release or Debug>`

The resulting artifacts can be found in the bin folder under the corresponding project. (e.g. src/Steeltoe.Security.Authentication.CloudFoundry/bin

To run the unit tests: 

1. git clone ...
2. cd `<clone directory>`
3. Install .NET Core SDK 
4. dotnet restore --configfile nuget.config test
5. cd test\ `<test project>` (e.g. cd test/Steeltoe.Security.Authentication.CloudFoundry.Test)
6. dotnet test --framework netcoreapp1.0

# Sample Applications
See the [Samples](https://github.com/SteelToeOSS/Samples) repo for examples of how to use these packages.

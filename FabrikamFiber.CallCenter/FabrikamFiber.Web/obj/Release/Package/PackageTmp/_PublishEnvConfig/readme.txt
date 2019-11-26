This folder contains project files that set the properties used when publishing the website.
There is a separate settings file for each environment (staging, test etc).

The main project (Tailspin.Web.csproj) will import the settings for environment if the $(TargetEnvPropsFile) property is set

  e.g. msbuild Tailspin.Web.csproj /p:TargetEnvPropsFile=_PublishEnvConfig\staging.proj /p:DeployOnBuild=true

would import the settings for the staging environment and publish the website on build.

NOTE: the site won't be published unless $(DeployOnBuild) is set to true.

You can either pass the settings explicitly on the command line, or set up build definitions that
set them appropriately (by setting the "MSBuild Arguments" property in the Advanced tab of the build definition.



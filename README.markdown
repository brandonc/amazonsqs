# amazonsqs

A higher-level interface to Amazon SQS for .NET. It features an object queue that serializes objects to JSON internally. Depends on the AWS SDK for .NET (included)

## Installation

Add the following keys to your <appSettings> config section.

   <add key="AWSAccessKey" value=""/>
   <add key="AWSSecretKey" value=""/>

If you want to run the tests, make a copy of App.config.sample and name it App.config. Add your AWS credentials.

Also included is a simple WPF app for observing queues and messages.
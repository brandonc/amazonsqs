# Amazon SQS .NET Object Interface

A higher-level interface to Amazon SQS for .NET. It features an object queue that serializes objects to JSON internally. Also included is a simple WPF app for observing queues and messages. Depends on the AWS SDK for .NET (included). 

## Installation

Add the following keys to your &lt;appSettings&gt; config section.

    <add key="AWSAccessKey" value=""/>
    <add key="AWSSecretKey" value=""/>

Then you can create an instance of `AmazonSqs.ObjectQueue`:

    AmazonSqs.ObjectQueue oq = new AmazonSqs.ObjectQueue(
        ConfigurationManager.AppSettings["AWSAccessKey"],
        ConfigurationManager.AppSettings["AWSSecretKey"],
        "MyQueue"
    );

    oq.Enqueue(new Tuple<string, string>("Hello", "World"));

*If you want to run the tests, make a copy of App.config.sample and name it App.config. Add your AWS credentials.*
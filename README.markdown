# Amazon SQS .NET Object Interface

A higher-level interface to Amazon SQS for .NET. It features an object queue that serializes objects to JSON internally. Also included is a simple WPF app for observing queues and messages. Depends on the AWS SDK for .NET (included). 

## Installation

Add the following keys to your &lt;appSettings&gt; config section.

    <add key="AWSAccessKey" value=""/>
    <add key="AWSSecretKey" value=""/>


## Example Usage

Create an instance of `AmazonSqs.ObjectQueue`:

    AmazonSqs.ObjectQueue queue = new AmazonSqs.ObjectQueue(
        ConfigurationManager.AppSettings["AWSAccessKey"],
        ConfigurationManager.AppSettings["AWSSecretKey"],
        "MyQueue"
    );

    queue.Enqueue<List<string>>(new List<string>(new string[] { "Hello", "World", "Bro" }));

    // Meanwhile in another place and at another time...

    var mylist = queue.DequeueOne<List<string>>();

    // or if you don't want to automatically delete message...

    ObjectMessage<List<string>> msg = queue.Peek<List<string>>();
    queue.DeleteMessage(msg.ReceiptHandle);

*If you want to run the tests, make a copy of App.config.sample and name it App.config. Add your AWS credentials.*
# Amazon SQS .NET Object Interface

[![Build status](https://ci.appveyor.com/api/projects/status/76nk0rtv7sfti0vn?svg=true)](https://ci.appveyor.com/project/AvantPrime/amazonsqs)

A higher-level interface to Amazon SQS for .NET. It features an object queue that serializes objects to JSON internally. Also included is a simple WPF app for observing queues and messages. Depends on the AWS SDK for .NET (included). 

## Example Usage

Create an instance of `AmazonSqs.ObjectQueue`:

    AmazonSqs.ObjectQueue queue = new AmazonSqs.ObjectQueue(
        ConfigurationManager.AppSettings["AWSAccessKey"],   // Or your AWS access key
        ConfigurationManager.AppSettings["AWSSecretKey"],   // Or your AWS secret key
        "MyQueue"                                           // Any queue name
    );

    queue.Enqueue<List<string>>(new List<string>(new string[] { "Hello", "World", "Bro" }));

Serialized objects must be less than 256K in length. Unfortunately, there's no way of knowing if a type is serializable unless you can successfully serialize and deserialize it. The only thing the library does is ensure your type is a reference type with a default constructor.

    // Meanwhile in another place and at another time...

    var mylist = queue.DequeueOne<List<string>>();  // Deletes message from queue!

    // If you don't want to automatically delete the message you can peek it.

    ObjectMessage<List<string>> msg = queue.Peek<List<string>>();
    queue.DeleteMessage(msg.ReceiptHandle);

*If you want to run the tests, make a copy of App.config.sample and name it App.config. Add your AWS credentials to the example appSettings.

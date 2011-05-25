using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AmazonSqs {
    public class ObjectQueue {
        private static readonly Lazy<JavaScriptSerializer> serializer = new Lazy<JavaScriptSerializer>();

        private const int MAX_MESSAGE_SIZE = 262144; // 256K

        private readonly AmazonSQS client;
        private readonly string queueUrl;

        private bool? queueExists = null;

        public ObjectQueue(string awsAccessKey, string awsSecretKey, string queueName) {
            this.client = AWSClientFactory.CreateAmazonSQSClient(
                awsAccessKey,
                awsSecretKey
            );

            EnsureQueueExists();

            var cqr = new CreateQueueRequest();
            cqr.QueueName = queueName;

            var response = this.client.CreateQueue(cqr);
            if (response.IsSetCreateQueueResult()) {
                this.queueUrl = response.CreateQueueResult.QueueUrl;
            } else {
                throw new QueueException("Queue could not be created.");
            }
        }

        private JavaScriptSerializer Serializer {
            get {
                if (!serializer.IsValueCreated) {
                    serializer.Value.MaxJsonLength = MAX_MESSAGE_SIZE;
                }
                return serializer.Value;
            }
        }

        private void EnsureQueueExists() {
            if (queueExists.HasValue && queueExists.Value) {
                return;
            } else if (queueExists.HasValue && !queueExists.Value) {
                throw new QueueException("Queue is not available or could not be created.");
            }

            var lqr = new ListQueuesRequest();
            var queues = client.ListQueues(lqr);

            if (queues.IsSetListQueuesResult()) {
                foreach (string queue in queues.ListQueuesResult.QueueUrl) {
                    if (queue == this.queueUrl) {
                        queueExists = true;

                        return;
                    }
                }
            }

            queueExists = false;
        }

        public void DeleteMessage(string receiptHandle) {
            var dmr = new DeleteMessageRequest();
            dmr.QueueUrl = queueUrl;
            dmr.ReceiptHandle = receiptHandle;

            this.client.DeleteMessage(dmr);
        }

        public void Enqueue<T>(T submission) where T : new() {
            try {
                SendMessageRequest req = new SendMessageRequest();
                req.QueueUrl = this.queueUrl;

                req.MessageBody = this.Serializer.Serialize(submission);

                client.SendMessage(req);
            } catch (AmazonSQSException ex) {
                throw new QueueException(
                    "Could not queue request.",
                    ex
                );
            } catch (InvalidOperationException ex) {
                throw new QueueException(
                    "The maximum size of the object graph must not exceed " + Serializer.MaxJsonLength + " bytes.",
                    ex
                );
            } catch (ArgumentException ex) {
                throw new QueueException(
                    "The maximum object depth (" + Serializer.RecursionLimit + ") was reached.",
                    ex
                );
            }
        }

        public ObjectMessage<T> Peek<T>() where T : new() {
            return Next<T>(false);

        }

        public T DequeueOne<T>() where T : new() {
            return Next<T>(true).Object;
        }

        private ObjectMessage<T> Next<T>(bool delete) where T : new() {
            var rmr = new ReceiveMessageRequest();
            rmr.QueueUrl = queueUrl;
            rmr.AttributeName.Add("SentTimestamp");
            rmr.AttributeName.Add("ApproximateReceiveCount");
            rmr.AttributeName.Add("ApproximateFirstReceiveTimestamp");

            var response = this.client.ReceiveMessage(rmr);

            if (response.IsSetReceiveMessageResult()) {
                var result = response.ReceiveMessageResult;

                if (result.IsSetMessage() && result.Message.Count > 0) {
                    ObjectMessage<T> value = new ObjectMessage<T>();
                    Message m = result.Message[0];
                    DateTime epochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    value.Object = this.Serializer.Deserialize<T>(m.Body);
                    value.ReceiptHandle = m.ReceiptHandle;

                    if (m.IsSetAttribute()) {
                        foreach (Amazon.SQS.Model.Attribute att in m.Attribute) {
                            switch (att.Name) {
                                case "SentTimestamp":
                                    value.Sent = epochDate.AddMilliseconds(double.Parse(att.Value));
                                    break;
                                case "ApproximateReceiveCount":
                                    value.ApproximateReceiveCount = Int32.Parse(att.Value);
                                    break;
                                case "ApproximateFirstReceiveTimestamp":
                                    value.FirstReceived = epochDate.AddMilliseconds(double.Parse(att.Value));
                                    break;
                            }
                        }
                    }

                    if(delete)
                        DeleteMessage(m.ReceiptHandle);

                    return value;
                }
            }

            return default(ObjectMessage<T>);
        }

        public List<T> Dequeue<T>(int maxMessages = 1) where T : new() {
            if (maxMessages < 1 || maxMessages > 10) {
                throw new ArgumentOutOfRangeException("maxMessages", "maxMessages must be between 1 and 10.");
            }

            var rmr = new ReceiveMessageRequest();
            rmr.QueueUrl = queueUrl;
            rmr.MaxNumberOfMessages = maxMessages;

            List<T> retval = new List<T>();

            var response = this.client.ReceiveMessage(rmr);
            if (response.IsSetReceiveMessageResult()) {
                var result = response.ReceiveMessageResult;

                if (result.IsSetMessage()) {
                    retval.Capacity = result.Message.Count;
                    foreach (Message m in result.Message) {
                        T value = this.Serializer.Deserialize<T>(m.Body);
                        DeleteMessage(m.ReceiptHandle);
                        retval.Add(value);
                    }
                }
            }

            return retval;
        }
    }
}

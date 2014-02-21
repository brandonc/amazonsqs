using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Collections.ObjectModel;

namespace AmazonSqs.Status.Components {
    class QueueAdmin {
        private readonly IAmazonSQS client;

        public ObservableCollection<QueueDescription> ListQueues() {
            var lqr = new ListQueuesRequest();
            var response = this.client.ListQueues(lqr);

            var result = new ObservableCollection<QueueDescription>();

            foreach (string url in response.QueueUrls) {
                string name = url.Substring(url.LastIndexOf('/') + 1);
                result.Add(new QueueDescription() {
                    Name = name,
                    Url = url
                });
            }

            return result;
        }

        public void DeleteQueue(string url) {
            var req = new DeleteQueueRequest() {
                QueueUrl = url
            };

            client.DeleteQueue(req);
        }

        public void DeleteMessage(string queueUrl, string receiptHandle) {
            var req = new DeleteMessageRequest() {
                QueueUrl = queueUrl,
                ReceiptHandle = receiptHandle
            };

            client.DeleteMessage(req);
        }

        public void PopulateQueueAttributes(QueueDescription queue) {
            var req = new GetQueueAttributesRequest() {
                QueueUrl = queue.Url
            };

            req.AttributeNames.Add("ApproximateNumberOfMessages");
            req.AttributeNames.Add("MessageRetentionPeriod");

            var response = client.GetQueueAttributes(req);
            if (response.Attributes != null && response.Attributes.Any()) {
                foreach (KeyValuePair<string, string> att in response.Attributes) {
                    switch (att.Key) {
                        case "MessageRetentionPeriod":
                            queue.MessageRetentionPeriod = TimeSpan.FromSeconds(Double.Parse(att.Value));
                            break;
                        case "ApproximateNumberOfMessages":
                            queue.ApproximateNumberOfMessages = Int32.Parse(att.Value);
                            break;
                    }
                }
            }
        }

        public List<QueueMessage> ListTop10Messages(string queueUrl) {
            var req = new ReceiveMessageRequest() {
                MaxNumberOfMessages = 10,
                QueueUrl = queueUrl
            };

            req.AttributeNames.Add("SentTimestamp");
            req.AttributeNames.Add("ApproximateReceiveCount");
            req.AttributeNames.Add("ApproximateFirstReceiveTimestamp");

            var result = new List<QueueMessage>(10);
            var response = client.ReceiveMessage(req);
            if (response.Messages != null && response.Messages.Any()) {
                DateTime epochDate = new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc);
                foreach (Message msg in response.Messages) {
                    var qm = new QueueMessage {
                        Body = msg.Body,
                        ReceiptHandle = msg.ReceiptHandle
                    };

                    if (msg.Attributes != null && msg.Attributes.Any()) {
                        foreach (KeyValuePair<string, string> att in msg.Attributes) {
                            switch (att.Key) {
                                case "SentTimestamp":
                                    qm.Sent = epochDate.AddMilliseconds(double.Parse(att.Value));
                                    break;
                                case "ApproximateReceiveCount":
                                    qm.ApproximateReceiveCount = Int32.Parse(att.Value);
                                    break;
                                case "ApproximateFirstReceiveTimestamp":
                                    qm.FirstReceived = epochDate.AddMilliseconds(double.Parse(att.Value));
                                    break;
                            }
                        }
                    }

                    result.Add(qm);
                }
            }

            return result;
        }

        public QueueAdmin(string awsAccessKey, string awsSecretKey) {
            this.client = AWSClientFactory.CreateAmazonSQSClient(
                awsAccessKey, awsSecretKey
            );
        }
    }
}

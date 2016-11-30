using System;

namespace AmazonSqs {
    public class ObjectMessage<T> {
        public T Object { get; set; }
        public string ReceiptHandle { get; set; }
        public DateTime Sent { get; set; }
        public int ApproximateReceiveCount { get; set; }
        public DateTime FirstReceived { get; set; }
    }
}

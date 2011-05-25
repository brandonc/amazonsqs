using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonSqs.Status.Components {
    class QueueMessage {
        public string ReceiptHandle { get; set; }
        public string Body { get; set; }
        public DateTime Sent { get; set; }
        public int ApproximateReceiveCount { get; set; }
        public DateTime FirstReceived { get; set; }
    }
}

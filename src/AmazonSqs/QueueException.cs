using System;

namespace AmazonSqs {
    public class QueueException : ApplicationException {
        public QueueException(string message)
            : base(message) { }

        public QueueException(string message, Exception ex)
            : base(message, ex) { }
    }
}

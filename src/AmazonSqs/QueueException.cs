using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonSqs {
    public class QueueException : ApplicationException {
        public QueueException(string message)
            : base(message) { }

        public QueueException(string message, Exception ex)
            : base(message, ex) { }
    }
}

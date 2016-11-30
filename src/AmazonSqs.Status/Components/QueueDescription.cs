﻿using System;

namespace AmazonSqs.Status.Components {
    class QueueDescription {
        public string Name { get; set; }
        public string Url { get; set; }
        public int? ApproximateNumberOfMessages { get; set; }
        public TimeSpan? MessageRetentionPeriod { get; set; }
    }
}

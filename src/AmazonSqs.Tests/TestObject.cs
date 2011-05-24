using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonSqs.Tests {
    public class TestObject {
        public int ID { get; set; }
        public string Name { get; set; }

        public TestObject NestedObject { get; set; }
        public string IgnoredProperty { get { return "Hello, Ignored Property!"; } }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace AmazonSqs.Tests {
    public class TestObject {
        public int ID { get; set; }
        public string Name { get; set; }

        public TestObject NestedObject { get; set; }

        [ScriptIgnore]
        public string IgnoredProperty { get; set; }
    }
}

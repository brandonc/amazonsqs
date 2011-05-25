using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Threading;
using System.Diagnostics;

namespace AmazonSqs.Tests {
    [TestClass]
    public class ObjectQueueTests {
        private static ObjectQueue queue;

        [ClassInitialize]
        public static void OpenQueue(TestContext testContext) {
            queue = new ObjectQueue(
                ConfigurationManager.AppSettings["AWSAccessKey"],
                ConfigurationManager.AppSettings["AWSSecretKey"],
                "AmazonSqs-ObjectQueue-UnitTests"
            );
        }

        [TestMethod]
        public void CanQueueOneObject() {
            queue.Enqueue(new TestObject() {
                ID = 1,
                Name = "Object 1",
                IgnoredProperty = "Not available!"
            });
        }

        [TestMethod]
        public void CanEnqueueGenericList() {
            queue.Enqueue(new List<string>(new string[] { "hello", "world", "how", "are", "you" }));
        }

        [TestMethod]
        public void CanQueueTwoObjects() {
            queue.Enqueue(new TestObject() {
                ID = 2,
                Name = "Object 2"
            });

            queue.Enqueue(new TestObject() {
                ID = 3,
                Name = "Object 3",
                NestedObject = new TestObject() {
                    ID = 31,
                    Name = "Object 3_1"
                }
            });
        }

        [TestMethod]
        [ExpectedException(typeof(QueueException))]
        public void CircularReferenceThrowsQueueException() {
            TestObject o1 = new TestObject() {
                ID = 4,
                Name = "c1"
            };
            TestObject o2 = new TestObject() {
                ID = 5,
                Name = "c2"
            };

            o1.NestedObject = o2;
            o2.NestedObject = o1;

            queue.Enqueue(o1);
        }

        [TestMethod]
        [ExpectedException(typeof(QueueException))]
        public void MessageOver256KThrowsQueueException() {
            string bigstring = new string('0', 263168);

            TestObject toobig = new TestObject() {
                ID = 6,
                Name = bigstring
            };

            queue.Enqueue(toobig);
        }
    }
}

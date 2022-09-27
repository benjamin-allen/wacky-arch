using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WackyArch.Components;
using WackyArch.Utilities;

namespace Test.Components
{
    [TestClass]
    public class StackTest
    {
        private Stack stack;

        [TestInitialize]
        public void TestInitialize()
        {
            stack = new Stack();
        }

        [TestMethod]
        public void Push()
        {
            stack.StackInterface.Write(12);
            stack.Cycle();
            Assert.AreEqual(12, stack.Words[0].Value);
            Assert.AreEqual(1, stack.SP);
        }

        [TestMethod]
        public void Pop()
        {
            stack.StackInterface.Write(15);
            stack.Cycle();
            var read = stack.StackInterface.Read(out bool didRead);
            stack.Cycle();
            Assert.AreEqual(15, read.Value);
            Assert.AreEqual(0, stack.SP);
            Assert.AreEqual(15, stack.Words[0].Value);
        }

        [TestMethod]
        public void MultipleCyclesBetweenPushAndPop()
        {
            stack.StackInterface.Write(100);
            stack.Cycle();
            stack.Cycle();
            stack.Cycle();
            stack.Cycle();
            var read = stack.StackInterface.Read(out bool didRead);
            stack.Cycle();
            Assert.AreEqual(100, read.Value);
            Assert.AreEqual(0, stack.SP);
            Assert.AreEqual(100, stack.Words[0].Value);
        }

        [TestMethod]
        public void MultipleWritesThenReads()
        {
            stack.StackInterface.Write(100);
            stack.Cycle();
            stack.StackInterface.Write(200);
            stack.Cycle();
            Assert.AreEqual(2, stack.SP);
            var read1 = stack.StackInterface.Read(out bool didRead1);
            stack.Cycle();
            Assert.AreEqual(200, read1.Value);
            Assert.AreEqual(1, stack.SP);
            var read2 = stack.StackInterface.Read(out bool didRead2);
            stack.Cycle();
            Assert.AreEqual(100, read2.Value);
            Assert.AreEqual(0, stack.SP);
            Assert.AreEqual(100, stack.Words[0].Value);
            Assert.AreEqual(200, stack.Words[1].Value);
        }

        [TestMethod]
        public void StressTest()
        {
            Random r = new Random();
            // Do 14 writes, waiting for a random period of time in between, then do 14 reads
            List<int> values = new List<int>();
            for (int i = 0; i < 14; i++)
            {
                var val = r.Next(Word.Min, Word.Max);
                values.Add(val);
                stack.StackInterface.Write(val);
                for ( int j = 0; j < r.Next(100); j++)
                {
                    stack.Cycle();
                }

                Assert.AreEqual(val, stack.Words[i].Value);
                Assert.AreEqual(i + 1, stack.SP);
            }
            for (int i = 0; i < 14; i++)
            {
                var val = stack.StackInterface.Read(out bool didRead);
                Assert.AreEqual(values[14 - i - 1], val.Value);
                for (int j = 0; j < r.Next(100); j++)
                {
                    stack.Cycle();
                }

                Assert.AreEqual(val.Value, stack.Words[14 - i - 1].Value);
                Assert.AreEqual(14 - i - 1, stack.SP);
            }
        }
    }
}

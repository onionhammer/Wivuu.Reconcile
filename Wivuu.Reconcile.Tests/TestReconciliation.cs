using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wivuu.Reconcile.Tests
{
    [TestClass]
    public class TestReconciliation
    {
        [TestMethod]
        public void TestZipAddOnly()
        {
            int[] sourceItems  = { 2, 4, 5 };
            var destItems      = new List<string> { "1", "2", "3" };
            string[] expected  = { "1", "2", "3", "4", "5" };

            destItems
                .Reconcile(sourceItems, (src, dest) => src.ToString() == dest)
                .WithItemNotInDestination(src => destItems.Add(src.ToString()))
                .Done();

            foreach (var i in expected)
                Assert.IsTrue(destItems.Contains(i));
        }

        [TestMethod]
        public void TestZipAddDelete()
        {
            int[] sourceItems  = { 2, 4, 5 };
            var destItems      = new List<string> { "1", "2", "3" };
            string[] expected  = { "2", "4", "5" };

            destItems
                .Reconcile(sourceItems, (src, dest) => src.ToString() == dest)
                .WithItemNotInDestination(src => destItems.Add(src.ToString()))
                .WithItemNotInSource(dest => destItems.Remove(dest))
                .Done();

            foreach (var i in expected)
                Assert.IsTrue(destItems.Contains(i));
        }

        [TestMethod]
        public void TestZipDelete()
        {
            int[] sourceItems = { 2, 4, 5 };
            var destItems = new List<string> { "1", "2", "3" };
            string[] expected = { "2" };

            destItems
                .Reconcile(sourceItems, (src, dest) => src.ToString() == dest)
                .WithItemNotInSource(dest => destItems.Remove(dest))
                .Done();

            foreach (var i in expected)
                Assert.IsTrue(destItems.Contains(i));
        }
    }
}

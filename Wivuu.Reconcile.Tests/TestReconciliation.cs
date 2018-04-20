using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wivuu.Reconcile.Tests
{
    [TestClass]
    public class TestReconciliation
    {
        [TestMethod]
        public void TestZipAddOnly()
        {
            int[] sourceItems = { 2, 4, 5 };
            var destItems     = new List<string> { "1", "2", "3" };
            string[] expected = { "1", "2", "3", "4", "5" };

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
            int[] sourceItems = { 2, 4, 5 };
            var destItems     = new List<string> { "1", "2", "3" };
            string[] expected = { "2", "4", "5" };

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
            var destItems     = new List<string> { "1", "2", "3" };
            string[] expected = { "2" };

            destItems
                .Reconcile(sourceItems, (src, dest) => src.ToString() == dest)
                .WithItemNotInSource(dest => destItems.Remove(dest))
                .Done();

            foreach (var i in expected)
                Assert.IsTrue(destItems.Contains(i));
        }

        class TestItem
        {
            public string Id { get; }

            public string Email { get; set; }

            public TestItem(string id, string email)
            {
                this.Id = id;
                this.Email = email;
            }
        }

        [TestMethod]
        public void TestZipUpdate()
        {
            var userInput = new[] { new TestItem("1", "test.user@email.com") };
            var database = new[] 
            {
                new TestItem("1", "test.user@emailz.com"),
                new TestItem("2", "second.user@email.com"),
            };

            var expected = new[]
            {
                new TestItem("1", "test.user@email.com"),
                new TestItem("2", "second.user@email.com"),
            };

            database
                .Reconcile(userInput, (src, dest) => src.Id == dest.Id)
                .WithMatching((src, dest) => dest.Email = src.Email)
                .Done();

            foreach (var i in expected)
                Assert.IsTrue(database.Any(d => d.Email == i.Email));
        }

        [TestMethod]
        public void TestZipAddDeleteUpdate()
        {
            var userInput = new[] 
            {
                new TestItem("1", "test.user@email.com"),
                new TestItem("3", "third.user@email.com"),
            };

            var database = new List<TestItem>
            {
                new TestItem("1", "test.user@emailz.com"),
                new TestItem("2", "second.user@email.com"),
            };

            var expected = new[]
            {
                new TestItem("1", "test.user@email.com"),
                new TestItem("3", "third.user@email.com")
            };

            database
                .Reconcile(userInput, (src, dest) => src.Id == dest.Id)
                .WithMatching((src, dest) => dest.Email = src.Email)
                .WithItemNotInSource(dest => database.Remove(dest))
                .WithItemNotInDestination(source => database.Add(source))
                .Done();

            foreach (var i in expected)
                Assert.IsTrue(database.Any(d => d.Email == i.Email));
        }
    }
}

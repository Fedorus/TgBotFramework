using System;
using System.Collections.Generic;
using NUnit.Framework;
using TgBotFramework.StageManaging;

namespace TgBotFramework.Tests
{
    [TestFixture]
    public class StageManagerTest
    {
        private StageManager _sm;
        public StageManagerTest()
        {
            var sortedDictionary = new SortedDictionary<string, Type>()
            {
                {"authorization", typeof(object)},
                {"addPartner", typeof(object)},
                {"addCustomer", typeof(object)},
            };
            _sm = new StageManager(sortedDictionary);
        }

        [Theory]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("default")]
        [TestCase("authorization")]
        [TestCase("authorization_1")]
        [TestCase("addPartner")]
        [TestCase("addPartner_0")]
        [TestCase("addCustomer")]
        [TestCase("addCustomer_0")]
        [TestCase("")]
        public void Should_be_true(string param)
        {
            Assert.True(_sm.Check(param));
        }
        
        [TestCase("add")]
        public void Should_be_false(string param)
        {
            Assert.False(_sm.Check(param));
        }
    }
}
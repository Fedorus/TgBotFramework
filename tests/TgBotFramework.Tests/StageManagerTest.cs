using System;
using System.Collections.Generic;
using NUnit.Framework;
using TgBotFramework.StageManaging;

namespace TgBotFramework.Tests
{
    [TestFixture]
    public class StageManagerTest
    {
        [Test]
        public void ShouldPass()
        {
            var sortedDictionary = new SortedDictionary<string, Type>()
            {
                {"authorization", typeof(object)},
                {"addPartner", typeof(object)},
                {"addCustomer", typeof(object)},
            };
            
            StageManager sm = new StageManager(sortedDictionary);
            
            Assert.True(sm.Check(""));
            Assert.True(sm.Check(null));
            Assert.True(sm.Check("default"));
            
            Assert.True(sm.Check("authorization"));
            Assert.True(sm.Check("authorization_1"));
            Assert.True(sm.Check("addPartner"));
            Assert.True(sm.Check("addPartner_0"));
            Assert.True(sm.Check("addCustomer"));
            Assert.True(sm.Check("addCustomer_0"));
            
            Assert.False(sm.Check("add"));
        }
    }
}
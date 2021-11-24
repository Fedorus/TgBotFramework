using System;
using System.Collections.Generic;
using NUnit.Framework;
using TgBotFramework.StageManaging;

namespace TgBotFramework.Tests
{
    public class UserStageManagerTest
    {
        private UserStageManager _userStateManager;

        [SetUp]
        public void Setup()
        {
            var sortedDictionary = new SortedDictionary<string, Type>()
            {
                {"authorization", typeof(object)},
                {"addPartner", typeof(object)},
                {"addCustomer", typeof(object)},
            };
            _userStateManager = new UserStageManager(new StageManager(sortedDictionary));
        }

        [TestCase("")]
        [TestCase("default")]
        [TestCase("addPartner")]
        [TestCase("addPartner_some_id")]
        [TestCase("authorization")]
        [TestCase(null)]
        public void ShouldPass(string param)
        {
            _userStateManager.Stage = param;
        }

        [TestCase("Not passed")]
        [TestCase("add")]
        public void ShouldNotPass(string param)
        {
            Assert.Catch(() => { _userStateManager.Stage = param; });
        }

    }
}
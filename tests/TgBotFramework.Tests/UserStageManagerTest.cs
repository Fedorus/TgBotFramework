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

        [Test]
        public void ShouldPass()
        {
            _userStateManager.Stage = "";
            _userStateManager.Stage = null;
            _userStateManager.Stage = "default";
            _userStateManager.Stage = "addPartner";
            _userStateManager.Stage = "addPartner_some_id";
            _userStateManager.Stage = "authorization";
            Assert.Pass();  
        }

        [Test]
        public void ShouldNotPass()
        {
            Assert.Catch(() => { _userStateManager.Stage = "Not passed"; });
            Assert.Catch(() => { _userStateManager.Stage = "add"; });
        }

    }
}
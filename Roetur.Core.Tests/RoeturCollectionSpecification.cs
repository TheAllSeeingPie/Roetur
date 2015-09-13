﻿using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Roetur.Core.Tests
{
    [TestClass]
    public class RoeturCollectionSpecification
    {
        [TestInitialize]
        public void Initialise()
        {
            Roetur.Routes.Clear();
        }

        [TestMethod]
        public void Simple_routes_are_added_to_collection()
        {
            Roetur.AddRoet("/", context => Task.Factory.StartNew(()=> {}));
            Assert.AreEqual(1, Roetur.Routes.Count);
        }

        [TestMethod]
        public void Complex_routes_are_added_to_collection()
        {
            Roetur.AddRoet("/awdawdad/:id", context => Task.Factory.StartNew(id => { }, context.Param<int>(":id")));
            Assert.AreEqual(1, Roetur.Routes.Count);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AssetPackage;
using System.Net;
using EvaluationAssetNameSpace;
using AssetManagerPackage;

namespace UnitTestEvaluation
{
    [TestClass]
    public class TestCaseEA
    {
        #region HelperMethods

        /// <summary>
        /// Logging functionality for the Tests
        /// </summary>
        /// <param name="msg"> Message to be logged </param>
        public void log(String msg, Severity severity = Severity.Information)
        {
            ILog logger = (ILog)AssetManager.Instance.Bridge;
            logger.Log(severity, "[EA Test]" + msg);
        }

        /// <summary>
        /// Method returning the Asset
        /// </summary>
        /// <returns> The Asset</returns>
        public EvaluationAsset getEA()
        {
            return (EvaluationAsset)AssetManager.Instance.findAssetByClass("EvaluationAsset");
        }


        #endregion HelperMethods
        #region TestMethods

        /// <summary>
        /// Initialize all required objects
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            if (AssetManager.Instance.findAssetsByClass("EvaluationAsset").Count == 0)
            {
#warning change bridge implementation (in UnitTestEvaluation/Bridge.cs) for testing (IDataStoragePath and ILog - logging behaviour)
                //Adding the bridge
                AssetManager.Instance.Bridge = new Bridge();

                //creating the asset
                EvaluationAsset cia = new EvaluationAsset();
            }

        }


        /// <summary>
        /// Test number one - sendig data to the asset
        /// </summary>
        [TestMethod]
        public void performTest1()
        {
            log("Calling test 1 - Evaluation Asset");

            EvaluationAssetSettings eas = new EvaluationAssetSettings();
            eas.GameId = "watercooler";
            eas.GameVersion = "2";
            eas.PlayerId = "player123";

            this.getEA().Settings = eas;

            try
            {
                getEA().sensorData("gameactivity", "event=messagetoplayer&tool=chat)");
                getEA().sensorData("gameactivity", "event=messagetoplayer&tool=chat&goalorientation=neutral");
            }
            catch
            {
                Assert.Fail();
            }
            log("Tests Evaluation Asset - test 1 - done!");
        }

        #endregion TestMethods
    }
}

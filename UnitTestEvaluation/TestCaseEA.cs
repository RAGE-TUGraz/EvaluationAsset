/*
  Copyright 2016 TUGraz, http://www.tugraz.at/
  
  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  This project has received funding from the European Union’s Horizon
  2020 research and innovation programme under grant agreement No 644187.
  You may obtain a copy of the License at
  
      http://www.apache.org/licenses/LICENSE-2.0
  
  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
  
  This software has been created in the context of the EU-funded RAGE project.
  Realising and Applied Gaming Eco-System (RAGE), Grant agreement No 644187, 
  http://rageproject.eu/

  Development was done by Cognitive Science Section (CSS) 
  at Knowledge Technologies Institute (KTI)at Graz University of Technology (TUGraz).
  http://kti.tugraz.at/css/

  Created by: Matthias Maurer, TUGraz <mmaurer@tugraz.at>
*/
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
#warning the ILog interface is expected on the AssetManager
            AssetManager.Instance.Log(severity, "[EA Test]: {0}", msg);
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

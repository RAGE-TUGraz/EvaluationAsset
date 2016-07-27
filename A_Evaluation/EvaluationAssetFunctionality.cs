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
using AssetManagerPackage;
using AssetPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvaluationAssetNameSpace
{
    internal class EvaluationAssetHandler
    {

        #region Fields

        /// <summary>
        /// Instance of the EvaluationAsset
        /// </summary>
        internal EvaluationAsset evaluationAsset = null;

        /// <summary>
        /// Instance of the class EvaluationAssetHandler - Singelton pattern
        /// </summary>
        private static EvaluationAssetHandler instance;

        #endregion Fields
        #region Constructors

            /// <summary>
            /// private EvaluationAssetHandler-ctor for Singelton-pattern 
            /// </summary>
        private EvaluationAssetHandler() { }

        #endregion Constructors
        #region Properties

        /// <summary>
        /// Getter for Instance of the EvaluationAssetHandler - Singelton pattern
        /// </summary>
        public static EvaluationAssetHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EvaluationAssetHandler();
                }
                return instance;
            }
        }

        #endregion Properties
        #region Methods

        /// <summary>
        /// Method returning an instance of the EvaluationAssetHandler.
        /// </summary>
        /// <returns> Instance of the EvaluationAssetHandler </returns>
        internal EvaluationAsset getEA()
        {
            if (evaluationAsset == null)
                evaluationAsset = (EvaluationAsset)AssetManager.Instance.findAssetByClass("EvaluationAsset");
            return (evaluationAsset);
        }

        /// <summary>
        /// Method for sending data to the evaluation server
        /// </summary>
        /// <param name="gameId"> Game identifier </param>
        /// <param name="playerId">Player Identifier </param>
        /// <param name="gameEvent"> Type of event </param>
        /// <param name="parameter"> Event information </param>
        /// <param name="gameversion"> version of the game </param>
        internal void sensorData(String gameId, String gameversion, String playerId, String gameEvent, String parameter)
        {
            if (!isReceivedDataValid(gameId, playerId, gameEvent, parameter))
            {
                loggingEA("Received data(" + gameId + "/" + playerId + "/" + gameEvent + "/" + parameter + ") not valid, input ignored!");
                return;
            }
            else
                loggingEA("Reiceveid sensor data ("+gameId+"/"+playerId+"/"+gameEvent+"/"+parameter+").");

            String xmlString = buildXMLString(gameId, gameversion, playerId, gameEvent, parameter);
            loggingEA("Created xml string: \"" +xmlString+"\"." );

            postData(xmlString);
        }

        /// <summary>
        /// Method for converting input data into a xml string for the evaluation service
        /// </summary>
        /// <param name="gameId"> Game identifier </param>
        /// <param name="playerId">Player Identifier </param>
        /// <param name="gameEvent"> Type of event </param>
        /// <param name="parameter"> Event information </param>
        /// <param name="gameversion"> version of the game </param>
        /// <returns> A XML string representation of the data </returns>
        internal String buildXMLString(String gameId, String gameversion, String playerId, String gameEvent, String parameter)
        {
            String xml = "<sensordata>";

            String dateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            xml += "<context project = \"rage\" application = \""+gameId+ "\" version=\""+gameversion+"\" date = \""+dateTime+"\"/>";
            xml += "<user id = \""+playerId+"\" group = \"\" ref= \"\"/>";
            xml += "<predicate tag = \""+gameEvent+"\"/>";

            String[] parameterPairs = parameter.Split('&');

            xml += "<parameter ";
            foreach(String parameterPair in parameterPairs)
            {
                String[] currentParameterPair = parameterPair.Split('=');
                xml += currentParameterPair[0] + "=\""+ currentParameterPair[1] + "\" ";
            }
            xml += "/>";

            xml += "</sensordata>";
            return (xml);
        }

        /// <summary>
        /// Method for performing POSR request for sending evaluation data.
        /// </summary>
        /// <param name="body"> data to be send to the evaluation service. </param>
        internal void postData(String body)
        {

            IWebServiceRequest iwr = (IWebServiceRequest)AssetManager.Instance.Bridge;
            if (iwr != null)
            {
                loggingEA("performing POST request with evaluation data.");
                Uri uri = new Uri(getEA().getEASettings().PostUrl);
                Dictionary<string, string> headers = new Dictionary<string, string>();
                RequestResponse rr = new RequestResponse();
                RequestSetttings rs = new RequestSetttings();
                rs.method = "POST";
                rs.uri = uri;
                rs.requestHeaders = headers;
                rs.body = body;
                iwr.WebServiceRequest(rs, out rr);
            }
            else
            {
                loggingEA("IWebServiceRequest bridge absent for performing POST request for sending evaluation data.", Severity.Error);
                throw new Exception("EXCEPTION: IWebServiceRequest bridge absent for performing POST request for sending evaluation data.");
            }
        }

        /// <summary>
        /// Method for checking input data format
        /// </summary>
        /// <param name="gameId"> Game identifier </param>
        /// <param name="playerId">Player Identifier </param>
        /// <param name="gameEvent"> Type of event </param>
        /// <param name="parameter"> Event information </param>
        /// <returns> True, if the received data is valid, false otherwise </returns>
        internal Boolean isReceivedDataValid(String gameId, String playerId, String gameEvent, String parameter)
        {
            string[] parameterPairs = parameter.Split('&');
            List<string> keys = new List<string>();

            foreach(string pair in parameterPairs)
                keys.Add(pair.Split('=')[0]);

            switch (gameEvent)
            {
                case "gameusage":
                    if (keys.Count == 1 && keys.Contains("event"))
                        return (true);
                    break;
                case "userprofile":
                    if (keys.Count == 1 && keys.Contains("event"))
                        return (true);
                    break;
                case "gameactivity":
                    if (keys.Count == 3 && keys.Contains("event") && keys.Contains("goalorientation") && keys.Contains("tool"))
                        return (true);
                    break;
                case "gamification":
                    if (keys.Count == 1 && keys.Contains("event"))
                        return (true);
                    break;
                case "gameflow":
                    if (keys.Count == 3 && keys.Contains("type") && keys.Contains("id") && keys.Contains("completed"))
                        return (true);
                    break;
                case "support":
                    if (keys.Count == 1 && keys.Contains("event"))
                        return (true);
                    break;
                case "assetactivity":
                    if (keys.Count == 2 && keys.Contains("asset") && keys.Contains("done"))
                        return (true);
                    break;
            }

            return (false);
        }

        #endregion Methods
        #region Testmethods

        /// <summary>
        /// Method for logging (Diagnostics).
        /// </summary>
        /// 
        /// <param name="msg"> Message to be logged. </param>
        internal void loggingEA(String msg, Severity severity = Severity.Information)
        {
            getEA().Log(severity, "[EA]: " + msg);
        }

        /*
        /// <summary>
        /// Method calling all Tests of this Class.
        /// </summary>
        internal void performAllTests()
        {
            loggingEA("****************************************************************");
            loggingEA("Calling all tests (Evaluation Asset):");
            performTest1();
            loggingEA("Tests Evaluation Asset - done!");
            loggingEA("****************************************************************");
        }

        /// <summary>
        /// Test number one - sendig data to the asset
        /// </summary>
        internal void performTest1()
        {
            loggingEA("Calling test 1 - Evaluation Asset");

            EvaluationAssetSettings  eas = new EvaluationAssetSettings();
            eas.GameId = "watercooler";
            eas.GameVersion = "2";
            eas.PlayerId = "player123";

            this.getEA().Settings = eas;

            getEA().sensorData("gameactivity", "event=messagetoplayer&tool=chat)");
            getEA().sensorData("gameactivity", "event=messagetoplayer&tool=chat&goalorientation=neutral");
            loggingEA("Tests Evaluation Asset - test 1 - done!");
        }
        */

        #endregion Testmethods
    }

    /*
    /// <summary>
    /// Implementation of the WebServiceResponse-Interface for handling web requests.
    /// </summary>
    public class WebServiceResponse : IWebServiceResponse
    {
        /// <summary>
        /// Describes behaviour in case the web request failed.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        public void Error(string url, string msg)
        {
            EvaluationAssetHandler.Instance.loggingEA("Web Request for sending evaluation data to " + url + " failed! " + msg, Severity.Error);
            throw new Exception("EXCEPTION: Web Request for sending evaluation data to " + url + " failed! " + msg);
        }

        /// <summary>
        /// Describes behaviour in case the web requests succeeds
        /// </summary>
        /// <param name="url"></param>
        /// <param name="code"></param>
        /// <param name="headers"></param>
        /// <param name="body"></param>
        public void Success(string url, int code, Dictionary<string, string> headers, string body)
        {
            EvaluationAssetHandler.Instance.loggingEA("WebClient request successful!");
            //EvaluationAssetHandler.Instance.loggingEA(DomainModelHandler.Instance.getDMFromXmlString(body));
        }
    }

    */
}

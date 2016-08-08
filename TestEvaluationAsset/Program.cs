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

#undef ASYNC

using AssetManagerPackage;
using AssetPackage;
using EvaluationAssetNameSpace;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace TestEvaluationAsset
{
    class Program
    {
        static void Main(string[] args)
        {


            AssetManager am = AssetManager.Instance;
            am.Bridge = new Bridge();
            

            EvaluationAsset ea = new EvaluationAsset();


            TestEvaluationAsset tea = new TestEvaluationAsset();
            tea.performAllTests();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();


        }
    }

    class TestEvaluationAsset
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
        /// Method calling all Tests of this Class.
        /// </summary>
        internal void performAllTests()
        {
            log("****************************************************************");
            log("Calling all tests (Evaluation Asset):");
            performTest1();
            log("Tests Evaluation Asset - done!");
            log("****************************************************************");
        }

        /// <summary>
        /// Test number one - sendig data to the asset
        /// </summary>
        internal void performTest1()
        {
            log("Calling test 1 - Evaluation Asset");

            EvaluationAssetSettings eas = new EvaluationAssetSettings();
            eas.GameId = "watercooler";
            eas.GameVersion = "2";
            eas.PlayerId = "player123";

            this.getEA().Settings = eas;

            getEA().sensorData("gameactivity", "event=messagetoplayer&tool=chat)");
            getEA().sensorData("gameactivity", "event=messagetoplayer&tool=chat&goalorientation=neutral");
            log("Tests Evaluation Asset - test 1 - done!");
        }
        
        #endregion TestMethods
    }

    public class Bridge : IBridge, ILog, IDataStorage, IWebServiceRequest /*IWebServiceRequestAsync*/
    {
        string IDataStoragePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); //@"C:\Users\mmaurer\Desktop\rageCsFiles\";
        

        #region IDataStorage

        public bool Delete(string fileId)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string fileId)
        {
#warning Change DataStorage-path if needed in Program.cs, Class Bridge, Variable IDataStoragePath
            string filePath = IDataStoragePath + fileId;
            return (File.Exists(filePath));
        }

        public string[] Files()
        {
            throw new NotImplementedException();
        }

        public string Load(string fileId)
        {
#warning Change Loading-path if needed in Program.cs, Class Bridge, Variable IDataStoragePath
            string filePath = IDataStoragePath + fileId;
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    return (line);
                }
            }
            catch (Exception e)
            {
                Log(Severity.Error, e.Message);
                Log(Severity.Error, "Error by loading the DM! - Maybe you need to change the path: \"" + IDataStoragePath + "\"");
            }

            return (null);
        }

        public void Save(string fileId, string fileData)
        {
#warning Change Saving-path if needed in Program.cs, Class Bridge, Variable IDataStoragePath
            string filePath = IDataStoragePath + fileId;
            using (StreamWriter file = new StreamWriter(filePath))
            {
                file.Write(fileData);
            }
        }

        #endregion IDataStorage

        #region ILog

        public void Log(Severity severity, string msg)
        {
            Console.WriteLine("BRIDGE:  " + msg);
        }

        #endregion ILog

        #region IWebServiceRequestAsync Members
        /*

 #if ASYNC

         /// <summary>
         /// Web service request.
         /// </summary>
         ///
         /// <param name="method">           The method. </param>
         /// <param name="uri">              URI of the document. </param>
         /// <param name="headers">          The headers. </param>
         /// <param name="body">             The body. </param>
         /// <param name="notifyOnResponse"> The response. </param>
         public async void WebServiceRequest(
             string method,
             Uri uri,
             Dictionary<string, string> headers,
             string body,
             IWebServiceResponse notifyOnResponse)
 #else
         /// <summary>
         /// Web service request.
         /// </summary>
         ///
         /// <param name="method">           The method. </param>
         /// <param name="uri">              URI of the document. </param>
         /// <param name="headers">          The headers. </param>
         /// <param name="body">             The body. </param>
         /// <param name="notifyOnResponse"> The response. </param>
         public void WebServiceRequest(
             string method,
             Uri uri,
             Dictionary<string, string> headers,
             string body,
             IWebServiceResponse notifyOnResponse)
 #endif
         {
             try
             {
                 //! Create might throw a silent System.IOException on .NET 3.5 (sync).
                 // 
                 HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                 request.Method = method;

                 // TODO Cookies
                 // 
                 // Both Accept and Content-Type are not allowed as Headers in a HttpWebRequest.
                 // They need to be assigned to a matching property.
                 // 
                 if (headers.ContainsKey("Accept"))
                 {
                     request.Accept = headers["Accept"];
                 }

                 if (!String.IsNullOrEmpty(body))
                 {
                     byte[] data = Encoding.UTF8.GetBytes(body);

                     if (headers.ContainsKey("Content-Type"))
                     {
                         request.ContentType = headers["Content-Type"];
                     }

                     foreach (KeyValuePair<string, string> kvp in headers)
                     {
                        if (kvp.Key.Equals("Accept") || kvp.Key.Equals("Content-Type"))
                        {
                            continue;
                        }
                        request.Headers.Add(kvp.Key, kvp.Value);
                     }

                     request.ContentLength = data.Length;

                     // See https://msdn.microsoft.com/en-us/library/system.net.servicepoint.expect100continue(v=vs.110).aspx
                     // A2 currently does not support this 100-Continue response for POST requets.
                     request.ServicePoint.Expect100Continue = false;

 #if ASYNC
                     Stream stream = await request.GetRequestStreamAsync();
                     await stream.WriteAsync(data, 0, data.Length);
                     stream.Close();
 #else
                     Stream stream = request.GetRequestStream();
                     stream.Write(data, 0, data.Length);
                     stream.Close();
 #endif
                 }
                 else
                 {
                     foreach (KeyValuePair<string, string> kvp in headers)
                     {
                        if (kvp.Key.Equals("Accept") || kvp.Key.Equals("Content-Type"))
                        {
                            continue;
                        }
                         request.Headers.Add(kvp.Key, kvp.Value);
                     }
                 }

 #if ASYNC
                 WebResponse response = await request.GetResponseAsync();
 #else
                 WebResponse response = request.GetResponse();
 #endif
                 if (notifyOnResponse != null)
                 {
                     Dictionary<string, string> responseHeaders = new Dictionary<string, string>();

                     if (response.Headers.HasKeys())
                     {
                         foreach (string key in response.Headers.AllKeys)
                         {
                            responseHeaders.Add(key, response.Headers.Get(key));
                         }
                     }

                     int responseCode = (int)(response as HttpWebResponse).StatusCode;
                     string responseBody = String.Empty;

                     using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                     {
 #if ASYNC
                         responseBody = await reader.ReadToEndAsync();
 #else
                         responseBody = reader.ReadToEnd();
 #endif
                     }

                     notifyOnResponse.Success(uri.ToString(), responseCode, responseHeaders, responseBody);
                 }
             }
             catch (Exception e)
             {
                 if (notifyOnResponse != null)
                 {
                     notifyOnResponse.Error(uri.ToString(), e.Message);
                 }
                 else
                 {
                     Log(Severity.Error, String.Format("{0} - {1}", e.GetType().Name, e.Message));
                 }
             }
         }
 */
        #endregion IWebServiceRequestAsync Members

        #region IWebServiceRequest Members

        // See http://stackoverflow.com/questions/12224602/a-method-for-making-http-requests-on-unity-ios
        // for persistence.
        // See http://18and5.blogspot.com.es/2014/05/mono-unity3d-c-https-httpwebrequest.html

#if ASYNC
        public void WebServiceRequest(RequestSetttings requestSettings, out RequestResponse requestReponse)
        {
            // Wrap the actual method in a Task. Neccesary because we cannot:
            // 1) Make this method async (out is not allowed) 
            // 2) Return a Task<RequestResponse> as it breaks the interface (only void does not break it).
            //
            Task<RequestResponse> taskName = Task.Factory.StartNew<RequestResponse>(() =>
            {
                return WebServiceRequestAsync(requestSettings).Result;
            });

            requestReponse = taskName.Result;
        }

        /// <summary>
        /// Web service request.
        /// </summary>
        ///
        /// <param name="requestSettings"> Options for controlling the operation. </param>
        ///
        /// <returns>
        /// A RequestResponse.
        /// </returns>
        private async Task<RequestResponse> WebServiceRequestAsync(RequestSetttings requestSettings)
#else
        /// <summary>
        /// Web service request.
        /// </summary>
        ///
        /// <param name="requestSettings">  Options for controlling the operation. </param>
        /// <param name="requestResponse"> The request response. </param>
        public void WebServiceRequest(RequestSetttings requestSettings, out RequestResponse requestResponse)
        {
            requestResponse = WebServiceRequest(requestSettings);
        }

        /// <summary>
        /// Web service request.
        /// </summary>
        ///
        /// <param name="requestSettings">Options for controlling the operation. </param>
        ///
        /// <returns>
        /// A RequestResponse.
        /// </returns>
        private RequestResponse WebServiceRequest(RequestSetttings requestSettings)
#endif
        {
            RequestResponse result = new RequestResponse(requestSettings);

            try
            {
                //! Might throw a silent System.IOException on .NET 3.5 (sync).
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestSettings.uri);

                request.Method = requestSettings.method;

                // Both Accept and Content-Type are not allowed as Headers in a HttpWebRequest.
                // They need to be assigned to a matching property.

                if (requestSettings.requestHeaders.ContainsKey("Accept"))
                {
                    request.Accept = requestSettings.requestHeaders["Accept"];
                }

                if (!String.IsNullOrEmpty(requestSettings.body))
                {
                    byte[] data = Encoding.UTF8.GetBytes(requestSettings.body);

                    if (requestSettings.requestHeaders.ContainsKey("Content-Type"))
                    {
                        request.ContentType = requestSettings.requestHeaders["Content-Type"];
                    }

                    foreach (KeyValuePair<string, string> kvp in requestSettings.requestHeaders)
                    {
                        if (kvp.Key.Equals("Accept") || kvp.Key.Equals("Content-Type"))
                        {
                            continue;
                        }
                        request.Headers.Add(kvp.Key, kvp.Value);
                    }

                    request.ContentLength = data.Length;

                    // See https://msdn.microsoft.com/en-us/library/system.net.servicepoint.expect100continue(v=vs.110).aspx
                    // A2 currently does not support this 100-Continue response for POST requets.
                    request.ServicePoint.Expect100Continue = false;

#if ASYNC
                    Stream stream = await request.GetRequestStreamAsync();
                    await stream.WriteAsync(data, 0, data.Length);
                    stream.Close();
#else
                    Stream stream = request.GetRequestStream();
                    stream.Write(data, 0, data.Length);
                    stream.Close();
#endif
                }
                else
                {
                    foreach (KeyValuePair<string, string> kvp in requestSettings.requestHeaders)
                    {
                        if (kvp.Key.Equals("Accept") || kvp.Key.Equals("Content-Type"))
                        {
                            continue;
                        }
                        request.Headers.Add(kvp.Key, kvp.Value);
                    }
                }

#if ASYNC
                WebResponse response = await request.GetResponseAsync();
#else
                WebResponse response = request.GetResponse();
#endif
                if (response.Headers.HasKeys())
                {
                    foreach (string key in response.Headers.AllKeys)
                    {
                        result.responseHeaders.Add(key, response.Headers.Get(key));
                    }
                }

                result.responseCode = (int)(response as HttpWebResponse).StatusCode;

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
#if ASYNC
                    result.body = await reader.ReadToEndAsync();
#else
                    result.body = reader.ReadToEnd();
#endif
                }
            }
            catch (Exception e)
            {
                result.responsMessage = e.Message;

                Log(Severity.Error, String.Format("{0} - {1}", e.GetType().Name, e.Message));
            }

            return result;
        }

        #endregion IWebServiceRequest Members

        //#region IWebServiceRequestAsync

        //public void WebServiceRequest(string method, Uri uri, Dictionary<string, string> headers, string body, IWebServiceResponse response)
        //{
        //    string url = uri.AbsoluteUri;

        //    if (string.Equals(method, "get", StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        try
        //        {
        //            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //            HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
        //            Stream resStream = webResponse.GetResponseStream();

        //            Dictionary<string, string> responseHeader = new Dictionary<string, string>();
        //            foreach (string key in webResponse.Headers.AllKeys)
        //                responseHeader.Add(key, webResponse.Headers[key]);

        //            StreamReader reader = new StreamReader(resStream);
        //            string dm = reader.ReadToEnd();

        //            response.Success(url, (int)webResponse.StatusCode, responseHeader, dm);
        //        }
        //        catch (Exception e)
        //        {
        //            response.Error(url, e.Message);
        //        }
        //    }
        //    else if(string.Equals(method, "post", StringComparison.CurrentCultureIgnoreCase))
        //    { //http://stackoverflow.com/questions/4015324/http-request-with-post
        //        try
        //        {
        //            var request = (HttpWebRequest)WebRequest.Create(uri);
        //            var data = Encoding.ASCII.GetBytes(body);

        //            request.Method = "POST";
        //            request.ContentType = "text/plain";
        //            request.ContentLength = data.Length;

        //            using (var stream = request.GetRequestStream())
        //            {
        //                stream.Write(data, 0, data.Length);
        //            }

        //            var responsePost = (HttpWebResponse)request.GetResponse();

        //            var responseString = new StreamReader(responsePost.GetResponseStream()).ReadToEnd();

        //            Dictionary<string, string> responseHeader = new Dictionary<string, string>();
        //            foreach (string key in responsePost.Headers.AllKeys)
        //                responseHeader.Add(key, responsePost.Headers[key]);

        //            response.Success(url, (int)responsePost.StatusCode, responseHeader, responseString);
        //        }
        //        catch (Exception e)
        //        {
        //            response.Error(url, e.Message);
        //        }
        //    }
        //    else
        //    {
        //        response.Error(url, "Requested method " + method + " not implemented!");
        //    }
        //}

        //public void WebServiceRequest(RequestSetttings requestSettings, out RequestResponse requestResponse)
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion IWebServiceRequestAsync
    }

}

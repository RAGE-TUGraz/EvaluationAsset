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
using AssetPackage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace UnitTestEvaluation
{
    public class Bridge : IBridge, /*IVirtualProperties,*/ ILog, IDataStorage, IWebServiceRequest/*, ISerializer*/
    {
        string IDataStoragePath = "./";

        #region IDataStorage

        public bool Delete(string fileId)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string fileId)
        {
            string filePath = IDataStoragePath + fileId;
            return (File.Exists(filePath));
        }

        public string[] Files()
        {
            throw new NotImplementedException();
        }

        public string Load(string fileId)
        {
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
    }
}

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
namespace EvaluationAssetNameSpace
{
    using AssetPackage;
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// An asset settings.
    /// 
    /// BaseSettings contains the (de-)serialization methods.
    /// </summary>
    public class EvaluationAssetSettings : BaseSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the A_Evaluation.AssetSettings class.
        /// </summary>
        public EvaluationAssetSettings()
            : base()
        {
            // Set Default values here.
            TestProperty = "Hello Default World";
            TestList = new String[] { "Red", "Green", "Blue" };
            TestPrivate = true;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the test property.
        /// </summary>
        ///
        /// <value>
        /// The test property.
        /// </value>
        [XmlElement()]
        public String TestProperty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the string[].
        /// </summary>
        ///
        /// <value>
        /// .
        /// </value>
        [XmlArray()]
        [XmlArrayItem("ListItem")]
        public String[] TestList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the test read only.
        /// </summary>
        ///
        /// <value>
        /// true if test read only, false if not.
        /// </value>
        public Boolean TestReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the test private.
        /// </summary>
        ///
        /// <value>
        /// true if test private only, false if not.
        /// </value>
        public Boolean TestPrivate
        {
            get;
            private set;
        }

        #endregion Properties
    }
}

// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Configuration;
using System.Configuration;
using CONFIG = System.Configuration.Configuration;
using System.IO;
using System.Reflection;

namespace Ukadc.Diagnostics.Tests
{
    [TestClass]
    public class ConfigFileTest
    {
        [TestMethod]
        public void TestCanFindConfigFile()
        {
            UkadcDiagnosticsSection section = UkadcDiagnosticsSection.ReadConfigSection();
            Assert.IsNotNull(section, "The UkadcDiagnosticsSection could not be loaded - is the config file available?");
        }

        /// <summary>
        /// Test which will check that if the config file is missing, or if the ukadc.diagnostics section is missing, we'll get
        /// a ConfigurationErrorsException thrown
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestInvalidConfig()
        {
            // Get the configuration file location in a way that works with MSTest and TestDriven.NET's host
            string originalFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            string dir = Path.GetDirectoryName(originalFile);
            string tempFile = Path.Combine(dir, "temp.config");

            // And attempt to rename the file
            if (File.Exists(originalFile))
            {
                // Now try to get the config section...
                try
                {
                    // Rename the config file so it's unavailable to the test
                    File.Move(originalFile, tempFile);

                    ConfigurationManager.RefreshSection("ukadc.diagnostics");

                    UkadcDiagnosticsSection section = UkadcDiagnosticsSection.ReadConfigSection();
                }
                finally
                {
                    // Rename it back
                    File.Move(tempFile, originalFile);
                    
                    // Ensure that the next test will read the right stuff...
                    ConfigurationManager.RefreshSection("ukadc.diagnostics");
                }
            }
        }
    }
}

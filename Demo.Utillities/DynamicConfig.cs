using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;


using System.IO;
using System.Reflection;
using System.Dynamic;

namespace Demo.Utilities
{


/**
 *      A object holding configuration, dynamically populated from json.
 *      It has an associated embeded default config file (embeded resource).
 *      If a config file is specified using this file, all parameters defined in this file will 
 *      overwrite those in the default file. 
 *      
 *      However, if the config file misses some paramters defined in the default file; the default parameters will apply.
 * */
    
    public class DynamicConfig 
    {

        


        public static dynamic GetConfig(String jsonFile, String DefaultJsonStrConfig) { 
            String jsonStr = String.Empty;
            dynamic _config;

            try
            {
                /**
                 * First, Try to fetch config from supplied config file
                 */
                jsonStr = File.ReadAllText(jsonFile);
            }
            catch (Exception e)
            {
                /**
                 * If that fails, Try to fetch config from embeded default config file
                 * To add a txt file to resources, right click on project --> properties --> resources and drag the file in.
                 */
                jsonStr = DefaultJsonStrConfig;
                
            }
            finally
            {
                _config = JsonConvert.DeserializeObject(jsonStr);   
            }
            return _config;
        }


       

         
    }
}

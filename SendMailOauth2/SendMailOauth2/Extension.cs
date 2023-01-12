using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SendMailOauth2.Extension
{
    static class Extension
    {
        /// <summary>
        /// Convert a json string to an object type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns>object type T</returns>
        public static T FromJson<T>(this string jsonString)
        {
            T obj = JsonConvert.DeserializeObject<T>(jsonString);
            return obj;
        }
        
        /// <summary>
        /// Convert an object to a json String
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns>object type T</returns>
        public static string ToJson(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
        
        /// <summary>
        ///     A FileInfo extension method that renames.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="newName">Name of the new.</param>
        /// ###
        /// <returns>.</returns>
        public static void Rename(this FileInfo @this, string newName)
        {
            string filePath = Path.Combine(@this.Directory.FullName, newName);
            @this.MoveTo(filePath);
        }

    }
}


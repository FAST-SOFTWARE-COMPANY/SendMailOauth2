using Newtonsoft.Json;

namespace ConsoleApp2
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
    }
}

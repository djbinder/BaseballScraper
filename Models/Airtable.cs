// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using Newtonsoft.Json.Linq;

// namespace BaseballScraper.Models
// {
//     public class Airtable
//     {
//         private string accountId;
//         private string apiKey;
//         private const string AIRTABLE_API_URL = "https://api.airtable.com/v0/{0}/{1}?api_key={2}";

//         public event Action<object, AirtableEventArgs> OnHttpResponse = delegate {};

//         public event Action<object, AirtableEventArgs> OnBeforeAssignValues = delegate {};

//         public event Action<object, AirtableEventArgs> OnTypeConversionFailure = delegate {};

//         private List<AirtableColumnConvertor> convertors;


//         public Airtable(string accountId, string apiKey)
//         {
//             this.accountId = accountId;
//             this.apiKey    = apiKey;
//             convertors     = new List<AirtableColumnConvertor>();
//         }

//         public void AddConvertor(string columnName, string propertyName, Func<string, object> function)
//         {
//             // Remove one, if it already exists
//             convertors.Remove(convertors.FirstOrDefault(c => c.IsMatch(columnName,propertyName)));

//             // Add this one
//             convertors.Add(new AirtableColumnConvertor() { ColumnName = columnName, PropertyName = propertyName, Function = function });
//         }

//         public List<T> GetObjects<T>(string tableName)
//         {
//             var json = new WebClient().DownloadString(string.Format(AIRTABLE_API_URL, accountId, tableName, apiKey));

//             // Call the event handler, and reset the json in the event it's changed...
//             var httpResponseEventArgs = new AirtableEventArgs() { HttpResponse = json };
//             OnHttpResponse(this, httpResponseEventArgs);
//             json = httpResponseEventArgs.HttpResponse;

//             var records = new List<T>();

//             // Loop all the "records"
//             foreach (var jsonRecord in JObject.Parse(json)["records"])
//             {
//                 // We'll "pre-stage" all property assignments here so we can hand them off to an event handler before final assignment
//                 var thisRecord = new Dictionary<string, object>();

//                 // We need the ID in a couple places below...
//                 var airTableId = jsonRecord["id"].ToString();

//                 // Set the Airtable ID on the property marked with AirtableId
//                 var idProp = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttributes<AirtableIdAttribute>(true).Any());

//                 if (idProp != null)
//                 {
//                     if (idProp.PropertyType != typeof(System.String))
//                     {
//                         throw new InvalidCastException("Property containing the AirtableId attribute must be of type System.String");
//                     }
//                     thisRecord.Add(idProp.Name, airTableId);
//                 }

//                 // Loop through all the properties of the object which have the AirtableColumn attribute
//                 foreach (var prop in typeof(T).GetProperties().Where(p => p.GetCustomAttributes<AirtableColumnAttribute>(true).Any()))
//                 {
//                     // Get the value from the JSON
//                     var columnName = prop.GetCustomAttribute<AirtableColumnAttribute>(true).ColumnName;
//                     var field      = (JProperty)jsonRecord["fields"].FirstOrDefault(f => ((JProperty)f).Name == columnName);

//                     // Do we have an Airtable column?
//                     if (field == null)
//                     {
//                         continue;
//                     }

//                     // This is simply the string present in the JSON. We still have to try to convert it to the right type.
//                     var rawFieldValue = (string)field.Value;

//                     // Load a custom convertor, if we have one
//                     var convertor = convertors.FirstOrDefault(c => c.IsMatch(columnName, prop.Name));

//                     // Attempt to convert it to the required type
//                     object value;
//                     try
//                     {
//                         if (convertor != null)
//                         {
//                             // Use the custom convertor
//                             value = convertor.Convert(rawFieldValue);
//                         }
//                         else
//                         {
//                             // Just try to switch types
//                             value = Convert.ChangeType(rawFieldValue, prop.PropertyType);
//                         }
//                     }
//                     catch (Exception e)
//                     {
//                         // Throw the type conversion failure event to allow the calling code to fix the problem
//                         var typeConversionFailureEventArgs = new AirtableEventArgs()
//                         {
//                             PropertyName = prop.Name,
//                             FieldName    = field.Name,
//                             FieldValue   = rawFieldValue,
//                             PropertyType = prop.PropertyType
//                         };
//                         OnTypeConversionFailure(this, typeConversionFailureEventArgs);
//                         value = typeConversionFailureEventArgs.FieldValue;

//                         if (typeConversionFailureEventArgs.Cancel)
//                         {
//                             continue;
//                         }
//                     }

//                     thisRecord.Add(prop.Name, value);
//                 }

//                 // The dictionary can be modified in this event
//                 var beforeAssignEventArgs = new AirtableEventArgs() { Values = thisRecord };
//                 OnBeforeAssignValues(this, beforeAssignEventArgs);

//                 // If the event handler wants to cancel, then abort
//                 if (beforeAssignEventArgs.Cancel)
//                 {
//                     continue;
//                 }

//                 // Create the object, assign all the properties, and add to the collection
//                 var obj = (T)Activator.CreateInstance(typeof(T));
//                 foreach (var assignment in thisRecord)
//                 {
//                     try
//                     {
//                         typeof(T).GetProperty(assignment.Key).SetValue(obj, assignment.Value, null);
//                     }
//                     catch (Exception e)
//                     {
//                         var truncatedValue = assignment.Value.ToString().Substring(0, assignment.Value.ToString().Length > 50 ? 50 : assignment.Value.ToString().Length);
//                         throw new Exception($"Failed to assign property value. Property name: \"{assignment.Key}\"; Value \"{truncatedValue}\"; Exception: \"{e.Message}\"");
//                     }
//                 }
//                 records.Add(obj);
//             }
//             return records;
//         }
//     }

//     public class AirtableEventArgs
//     {
//         public string HttpResponse { get; set; }
//         public Dictionary<string, object> Values { get; set; }
//         public bool Cancel { get; set; }
//         public string PropertyName { get; set; }
//         public string FieldName { get; set; }
//         public object FieldValue { get; set; }
//         public Type PropertyType { get; set; }
//     }


//     [AttributeUsage(AttributeTargets.Property)]
//     public class AirtableIdAttribute: Attribute { }


//     [AttributeUsage(AttributeTargets.Property)]
//     public class AirtableColumnAttribute: Attribute
//     {
//         public string ColumnName { get; private set; }

//         public AirtableColumnAttribute(string columnName)
//         {
//             ColumnName = columnName;
//         }
//     }


//     public class AirtableColumnConvertor
//     {
//         public string ColumnName { get; set; }
//         public string PropertyName { get; set; }
//         public Func<string, object> Function { get; set; }

//         public object Convert(string value)
//         {
//             return Function(value);
//         }

//         public bool IsMatch(string columnName, string propertyName)
//         {
//             return (ColumnName == columnName || columnName == "*") && (PropertyName == propertyName || propertyName == "*");
//         }
//     }
// }
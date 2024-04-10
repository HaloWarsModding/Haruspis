//-----------------------------------------------------------------------------
// File: DynamicProperties.cs
// Description: Contains the DynamicProperties class responsible for managing dynamic properties.
//    This class provides functionality to add, retrieve, remove, and check for the existence of dynamic properties.
//-----------------------------------------------------------------------------

namespace Ethereal.Core.Utils
{
    public interface IDynamicProperties
    {
        /// <summary>
        /// Adds a property with the specified name and value.
        /// </summary>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="value">The value of the property to add.</param>
        void AddProperty(string propertyName, object value);

        /// <summary>
        /// Tries to retrieve the value of the property with the specified name.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified name, if the name is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the property with the specified name was found; otherwise, false.</returns>
        bool TryGetProperty(string propertyName, out object? value);

        /// <summary>
        /// Removes the property with the specified name.
        /// </summary>
        /// <param name="propertyName">The name of the property to remove.</param>
        void RemoveProperty(string propertyName);

        /// <summary>
        /// Checks if a property with the specified name exists.
        /// </summary>
        /// <param name="propertyName">The name of the property to check.</param>
        /// <returns>true if a property with the specified name exists; otherwise, false.</returns>
        bool ContainsProperty(string propertyName);
    }

    public class DynamicProperties : IDynamicProperties
    {
        private static readonly Dictionary<string, object> dynamicProperties = [];

        public void AddProperty(string propertyName, object value)
        {
            dynamicProperties[propertyName] = value;
        }

        public bool TryGetProperty(string propertyName, out object? value)
        {
            return dynamicProperties.TryGetValue(propertyName, out value);
        }

        public void RemoveProperty(string propertyName)
        {
            _ = dynamicProperties.Remove(propertyName);
        }

        public bool ContainsProperty(string propertyName)
        {
            return dynamicProperties.ContainsKey(propertyName);
        }
    }
}
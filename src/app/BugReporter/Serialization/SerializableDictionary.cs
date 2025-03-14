﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializableDictionary.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BugReporter.Serialization
{
    [Serializable]
    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable where TKey : notnull where TValue : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey,TValue}"/> class.
        /// This is the default constructor provided for XML serializer.
        /// </summary>
        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            ArgumentNullException.ThrowIfNull(dictionary);

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            /*if (reader.IsEmptyElement)
            {
                return;
            }*/
            XmlReader inner = reader.ReadSubtree();

            XElement xElement = XElement.Load(inner);
            if (xElement.HasElements)
            {
                foreach (XElement element in xElement.Elements())
                {
                    Add((TKey)Convert.ChangeType(element.Name.ToString(), typeof(TKey)), (TValue)Convert.ChangeType(element.Value, typeof(TValue)));
                }
            }

            inner.Close();

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (TKey key in Keys)
            {
                writer.WriteStartElement(key.ToString().Replace(" ", ""));

                // Check to see if we can actually serialize element
#pragma warning disable SYSLIB0050 // 'Type.IsSerializable' is obsolete: 'Formatter-based serialization is obsolete and should not be used.'
                if (this[key].GetType().IsSerializable)
#pragma warning restore SYSLIB0050
                {
                    // if it's Serializable doesn't mean serialization will succeed (IE. GUID and SQLError types)
                    try
                    {
                        writer.WriteValue(this[key]);
                    }
                    catch (Exception)
                    {
                        // we're not Throwing anything here, otherwise evil thing will happen
                        writer.WriteValue(this[key].ToString());
                    }
                }
                else
                {
                    // If Type has custom implementation of ToString() we'll get something useful here
                    // Otherwise we'll get Type string. (Still better than crashing).
                    writer.WriteValue(this[key].ToString());
                }

                writer.WriteEndElement();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace Vixark {



    /// <summary>
    /// Clase auxiliar que permite serializar a JSON diccionarios con claves enumeraciones.
    /// </summary>
    class FábricaConvertidorDiccionarioClaveEnumeración : JsonConverterFactory { // Tomado de https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to#support-dictionary-with-non-string-key.


        #region Métodos y Funciones

        public override bool CanConvert(Type typeToConvert) {

            if (!typeToConvert.IsGenericType) return false;
            if (typeToConvert.GetGenericTypeDefinition() != typeof(Dictionary<,>)) return false;
            return typeToConvert.GetGenericArguments()[0].IsEnum;

        } // CanConvert>


        public override JsonConverter? CreateConverter(Type type, JsonSerializerOptions options) {

            var tipoClave = type.GetGenericArguments()[0];
            var tipoValor = type.GetGenericArguments()[1];
            return (JsonConverter?)Activator.CreateInstance(typeof(ConvertidorDiccionarioClaveEnumeración<,>)
                .MakeGenericType(new Type[] { tipoClave, tipoValor }), BindingFlags.Instance | BindingFlags.Public, binder: null, 
                args: new object[] { options }, culture: null);

        } // CreateConverter>


        #endregion Métodos y Funciones>



        private class ConvertidorDiccionarioClaveEnumeración<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>> where TKey : struct, Enum {


            #region Propiedades y Campos

            private readonly JsonConverter<TValue> _valueConverter;

            #pragma warning disable IDE0044 // Agregar modificador de solo lectura. Se omite esta advertencia porque si necesita ser escrito en el constructor.
            private Type _keyType;

            private Type _valueType;
            #pragma warning restore IDE0044

            #endregion Propiedades y Campos>


            #region Constructores


            public ConvertidorDiccionarioClaveEnumeración(JsonSerializerOptions options) {
                
                _valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue)); // For performance, use the existing converter if available.
                _keyType = typeof(TKey); // Cache the key and value types.
                _valueType = typeof(TValue);

            } // ConvertidorDiccionarioClaveEnumInterno>


            #endregion Constructores>


            #region Métodos y Funciones


            public override Dictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {

                if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

                var dictionary = new Dictionary<TKey, TValue>();

                while (reader.Read()) {

                    if (reader.TokenType == JsonTokenType.EndObject) return dictionary;   
                    if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException(); // Get the key.
                    var propertyName = reader.GetString();            
                    if (!Enum.TryParse(propertyName, ignoreCase: false, out TKey clave) && !Enum.TryParse(propertyName, ignoreCase: true, out clave)) // For performance, parse with ignoreCase:false first.
                        throw new JsonException($"Unable to convert \"{propertyName}\" to Enum \"{_keyType}\"."); 

                    TValue valor; // Get the value.
                    if (_valueConverter != null) {
                        reader.Read();
                        valor = _valueConverter.Read(ref reader, _valueType, options);
                    } else {
                        valor = JsonSerializer.Deserialize<TValue>(ref reader, options);
                    }

                    dictionary.Add(clave, valor); // Add to dictionary.

                }

                throw new JsonException();

            } // Read>


            public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue> dictionary, JsonSerializerOptions options) {

                writer.WriteStartObject();
                foreach (KeyValuePair<TKey, TValue> kvp in dictionary) {

                    writer.WritePropertyName(kvp.Key.ToString());
                    if (_valueConverter != null) {
                        _valueConverter.Write(writer, kvp.Value, options);
                    } else {
                        JsonSerializer.Serialize(writer, kvp.Value, options);
                    }

                }

                writer.WriteEndObject();

            } // Write>


            #endregion Métodos y Funciones>


        } // ConvertidorDiccionarioClaveEnumeración>



    } // FábricaConvertidorDiccionarioClaveEnumeración>



} // Vixark>

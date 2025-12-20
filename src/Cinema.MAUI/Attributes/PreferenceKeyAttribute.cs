using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Cinema.MAUI.Attributes;

/// <summary>
/// Associates a strongly-typed preference key with a model property.
/// </summary>
/// <remarks>
/// <para>
/// Storing app preferences via string keys can lead to duplication, typos, and drift between
/// model properties and their corresponding storage keys. <see cref="PreferenceKeyAttribute"/>
/// co-locates the key metadata with the property itself, improving discoverability and reducing
/// magic strings scattered across the codebase.
/// </para>
/// <para>
/// Intended usage:
/// <list type="bullet">
///   <item>
///     Apply to public properties on simple data models (e.g., authentication tokens, user profile).
///     A helper/service (e.g., a <c>PreferenceHelper</c>) can reflect over these properties and
///     read/write values in <c>Preferences</c> using the declared keys.
///   </item>
///   <item>
///     Keep keys namespaced and stable (e.g., <c>cinema.auth.access_token</c>) to avoid collisions
///     and make migration/versioning manageable.
///   </item>
///   <item>
///     Prefer using primitive types (string, int, bool) for direct storage. For complex types,
///     serialize/deserialze to/from JSON in the helper as needed.
///   </item>
/// </list>
/// </para>
/// <para>
/// Benefits:
/// <list type="bullet">
///   <item>Eliminates scattered magic strings.</item>
///   <item>Centralizes key intent with the owning property.</item>
///   <item>Enables generic save/load routines via reflection.</item>
/// </list>
/// </para>
/// <para>
/// Trade-offs:
/// <list type="bullet">
///   <item>Uses reflection in helper code (acceptable for small models).</item>
///   <item>Refactoring keys across the app still requires careful coordination.</item>
/// </list>
/// </para>
/// <example>
/// Example model usage:
/// <code>
/// using Cinema.MAUI.Attributes;
/// 
/// internal class Token
/// {
///     [PreferenceKey("cinema.auth.access_token")]
///     public required string AccessToken { get; set; }
/// 
///     [PreferenceKey("cinema.auth.user_id")]
///     public required int UserId { get; set; }
/// }
/// </code>
/// Example helper usage:
/// <code>
/// // Save all attributed properties:
/// PreferenceHelper.Save(token);
/// 
/// // Load values into a new instance:
/// var saved = PreferenceHelper.Load&lt;Token&gt;();
/// </code>
/// </example>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
internal sealed class PreferenceKeyAttribute : Attribute
{
    /// <summary>
    /// The unique, namespaced key used to store and retrieve the property's value in Preferences.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="PreferenceKeyAttribute"/>.
    /// </summary>
    /// <param name="key">A stable, descriptive key (e.g., <c>cinema.auth.access_token</c>).</param>
    public PreferenceKeyAttribute(string key) => Key = key;
}

/// <summary>
/// Helper methods to persist and restore objects to/from MAUI <c>Preferences</c>,
/// based on <see cref="PreferenceKeyAttribute"/> decorations on model properties.
/// </summary>
/// <remarks>
/// <para>
/// <b>How it works:</b> The helper reflects over public instance properties of a given type
/// and looks for <see cref="PreferenceKeyAttribute"/>. For properties with the attribute,
/// values are written to or read from <c>Preferences</c> using the declared key.
/// Primitive types (string, int, bool, double, float, long, DateTime) are stored directly;
/// other types are serialized/deserialized as JSON.
/// </para>
/// <para>
/// <b>Use cases:</b> Persisting authentication tokens, simple user settings, and other
/// lightweight app state without scattering string keys throughout the codebase.
/// </para>
/// <para>
/// <b>Notes:</b> Reflection overhead is minimal for small models and infrequent calls.
/// Ensure keys are stable and namespaced to avoid collisions across the app.
/// </para>
/// </remarks>
internal static class PreferenceHelper
{
    /// <summary>
    /// Saves all public instance properties of <typeparamref name="T"/> that are decorated
    /// with <see cref="PreferenceKeyAttribute"/> into <c>Preferences</c>.
    /// </summary>
    /// <typeparam name="T">The model type to persist.</typeparam>
    /// <param name="instance">The model instance to read values from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is null.</exception>
    /// <remarks>
    /// - Primitive types are stored using native <c>Preferences.Set</c> overloads.
    /// - <see cref="DateTime"/> values are stored as UTC ticks for portability.
    /// - Non-primitive values are JSON-serialized.
    /// - <c>null</c> values remove the corresponding key.
    /// </remarks>
    public static void Save<T>(T instance)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        foreach (PropertyInfo p in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            PreferenceKeyAttribute? attr = p.GetCustomAttribute<PreferenceKeyAttribute>();
            if (attr is null || !p.CanRead)
            {
                continue;
            }

            object? value = p.GetValue(instance);
            WriteValue(attr.Key, value, p.PropertyType);
        }
    }

    /// <summary>
    /// Loads values from <c>Preferences</c> into a new instance of <typeparamref name="T"/>,
    /// using keys declared via <see cref="PreferenceKeyAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The model type to restore.</typeparam>
    /// <returns>A new instance populated with any available values from <c>Preferences</c>.</returns>
    /// <remarks>
    /// - Only properties with <see cref="PreferenceKeyAttribute"/> are considered.
    /// - If a key is missing, the property remains at its default value.
    /// - Non-primitive values are deserialized from JSON if present.
    /// </remarks>
    public static T Load<T>() where T : new()
    {
        var result = new T();
        foreach (PropertyInfo p in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            PreferenceKeyAttribute? attr = p.GetCustomAttribute<PreferenceKeyAttribute>();
            if (attr is null || !p.CanWrite)
            {
                continue;
            }

            object? value = ReadValue(attr.Key, p.PropertyType);
            if (value is not null)
            {
                p.SetValue(result, value);
            }
        }

        return result;
    }

    /// <summary>
    /// Removes all preference entries associated with properties of <typeparamref name="T"/>
    /// that are decorated with <see cref="PreferenceKeyAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose attributed keys should be removed.</typeparam>
    public static void RemoveAll<T>()
    {
        foreach (PropertyInfo p in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            PreferenceKeyAttribute? attr = p.GetCustomAttribute<PreferenceKeyAttribute>();
            if (attr is null)
            {
                continue;
            }

            Preferences.Remove(attr.Key);
        }
    }

    /// <summary>
    /// Attempts to load <typeparamref name="T"/> and indicates whether any attributed keys
    /// were found in <c>Preferences</c>.
    /// </summary>
    /// <typeparam name="T">The model type to restore.</typeparam>
    /// <param name="result">On success, the restored instance; otherwise a new default instance.</param>
    /// <returns><c>true</c> if at least one attributed key exists; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// Use this to check if persisted data is present without inspecting individual keys.
    /// </remarks>
    public static bool TryLoad<T>(out T result) where T : new()
    {
        result = Load<T>();
        foreach (PropertyInfo p in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            PreferenceKeyAttribute? attr = p.GetCustomAttribute<PreferenceKeyAttribute>();
            if (attr is null)
            {
                continue;
            }

            if (Preferences.ContainsKey(attr.Key))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Writes a single value to <c>Preferences</c> using the most appropriate storage format.
    /// </summary>
    /// <param name="key">The preference key.</param>
    /// <param name="value">The value to store, or <c>null</c> to remove the key.</param>
    /// <param name="type">The property's declared type.</param>
    /// <remarks>
    /// Primitive types are stored via native overloads; complex types are JSON-serialized.
    /// <see cref="DateTime"/> is stored as UTC ticks.
    /// </remarks>
    private static void WriteValue(string key, object? value, Type type)
    {
        if (value is null)
        {
            Preferences.Remove(key);
            return;
        }

        if (type == typeof(string))
            Preferences.Set(key, (string)value);
        else if (type == typeof(int))
            Preferences.Set(key, (int)value);
        else if (type == typeof(bool))
            Preferences.Set(key, (bool)value);
        else if (type == typeof(double))
            Preferences.Set(key, (double)value);
        else if (type == typeof(float))
            Preferences.Set(key, (float)value);
        else if (type == typeof(long))
            Preferences.Set(key, (long)value);
        else if (type == typeof(DateTime))
            Preferences.Set(key, ((DateTime)value).ToUniversalTime().Ticks);
        else
            Preferences.Set(key, JsonSerializer.Serialize(value));
    }

    /// <summary>
    /// Reads a single value from <c>Preferences</c>, converting it to the requested type.
    /// </summary>
    /// <param name="key">The preference key.</param>
    /// <param name="type">The property's declared type.</param>
    /// <returns>The value if present; otherwise <c>null</c>.</returns>
    /// <remarks>
    /// Primitive types are read via native overloads; complex types are JSON-deserialized.
    /// <see cref="DateTime"/> is reconstructed from UTC ticks if available.
    /// </remarks>
    private static object? ReadValue(string key, Type type)
    {
        if (!Preferences.ContainsKey(key)) return null;

        if (type == typeof(string))
            return Preferences.Get(key, string.Empty);
        if (type == typeof(int))
            return Preferences.Get(key, 0);
        if (type == typeof(bool))
            return Preferences.Get(key, false);
        if (type == typeof(double))
            return Preferences.Get(key, 0d);
        if (type == typeof(float))
            return Preferences.Get(key, 0f);
        if (type == typeof(long))
            return Preferences.Get(key, 0L);
        if (type == typeof(DateTime))
        {
            var ticks = Preferences.Get(key, 0L);
            return ticks == 0L ? DateTime.MinValue : new DateTime(ticks, DateTimeKind.Utc);
        }

        var json = Preferences.Get(key, string.Empty);
        return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize(json, type);
    }
}

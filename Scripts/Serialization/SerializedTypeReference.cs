using System;

using UnityEngine;

/// <summary>
/// Reference to a class <see cref="System.Type"/> with support for Unity serialization.
/// </summary>
[Serializable]
public sealed class SerializedTypeReference : ISerializationCallbackReceiver
{
    public static string GetClassReference(Type type)
    {
        return type != null
            ? type.FullName + ", " + type.Assembly.GetName().Name
            : "";
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="SerializedTypeReference"/> class.
    /// </summary>
    public SerializedTypeReference()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializedTypeReference"/> class.
    /// </summary>
    /// <param name="assemblyQualifiedClassName">Assembly qualified class name.</param>
    public SerializedTypeReference(string assemblyQualifiedClassName)
    {
        Type = !string.IsNullOrEmpty(assemblyQualifiedClassName)
            ? Type.GetType(assemblyQualifiedClassName)
            : null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializedTypeReference"/> class.
    /// </summary>
    /// <param name="type">Class type.</param>
    /// <exception cref="System.ArgumentException">
    /// If <paramref name="type"/> is not a class type.
    /// </exception>
    public SerializedTypeReference(Type type)
    {
        Type = type;
    }


    #region ISerializationCallbackReceiver Members

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (!string.IsNullOrEmpty(classReference))
        {
            type = Type.GetType(classReference);

            if (type == null)
            {
                Debug.LogWarning($"'{classReference}' was referenced but class type was not found.");
            }
        }
        else
        {
            type = null;
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    { }

    #endregion


    [SerializeField]
    private string classReference;

    private Type type;

    /// <summary>
    /// Gets or sets type of class reference.
    /// </summary>
    /// <exception cref="System.ArgumentException">
    /// If <paramref name="value"/> is not a class type.
    /// </exception>
    public Type Type
    {
        get { return type; }
        set
        {
            if (value != null && !value.IsClass)
            {
                throw new ArgumentException($"'{value.FullName}' is not a class type.", nameof(value));
            }

            type = value;
            classReference = GetClassReference(value);
        }
    }

    public static implicit operator string(SerializedTypeReference typeReference) => typeReference.classReference;

    public static implicit operator Type(SerializedTypeReference typeReference) => typeReference.Type;

    public static implicit operator SerializedTypeReference(Type type) => new SerializedTypeReference(type);

    public override string ToString() => Type != null ? Type.FullName : $"(None)";
}
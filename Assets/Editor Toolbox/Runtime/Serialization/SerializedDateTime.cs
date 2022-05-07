using System;

namespace UnityEngine
{
    /// <summary>
    /// Reference to a class <see cref="System.DateTime"/> with support for Unity serialization.
    /// </summary>
    [Serializable]
    public sealed class SerializedDateTime : ISerializationCallbackReceiver
    {
        [SerializeField]
        private long ticks;

        private DateTime dateTime;


        public SerializedDateTime() : this(0)
        { }

        public SerializedDateTime(long ticks) : this(new DateTime(ticks))
        { }

        public SerializedDateTime(DateTime dateTime)
        {
            DateTime = dateTime;
        }


        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            dateTime = new DateTime(ticks);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        { }


        public DateTime DateTime
        {
            get => dateTime;
            set
            {
                dateTime = value;
                ticks = dateTime.Ticks;
            }
        }


        public static implicit operator DateTime(SerializedDateTime sdt)
        {
            return sdt.DateTime;
        }

        public static implicit operator SerializedDateTime(DateTime dt)
        {
            return new SerializedDateTime
            {
                ticks = dt.Ticks
            };
        }
    }
}
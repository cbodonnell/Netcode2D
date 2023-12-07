using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Runtime.Serialization;

public class HandleStates
{
    public class InputState
    {
        public int tick;
        public Vector2 moveInput;
    }

    public class TransformStateRW : INetworkSerializable
    {
        public int tick;
        public Vector3 position;
        public bool isMoving; // check to ensure other player is moving before we update

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out tick);
                reader.ReadValueSafe(out position);
                reader.ReadValueSafe(out isMoving);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(tick);
                writer.WriteValueSafe(position);
                writer.WriteValueSafe(isMoving);
            }
        }
    }
}

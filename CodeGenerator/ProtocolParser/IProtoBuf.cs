
namespace SilentOrbit.ProtocolBuffers
{
    public interface IProtoBuf
    {
        /// <summary>
        /// Deserialize byte array into the instance.  
        /// (a new empty instance of the desired class must be constructed beforehand)
        /// </summary>
        IProtoBuf ToObject(byte[] buffer);

        /// <summary>
        /// Serializes this instance into a byte array
        /// </summary>
        byte[] ToBytes();
    }


}
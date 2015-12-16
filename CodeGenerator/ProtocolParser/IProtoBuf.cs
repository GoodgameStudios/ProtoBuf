
namespace SilentOrbit.ProtocolBuffers
{
    public interface IProtoBuf
    {
        IProtoBuf Deserialize(byte[] buffer);
        byte[] Serialize();
    }


}
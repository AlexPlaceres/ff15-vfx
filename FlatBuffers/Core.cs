using System.Text;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

namespace ReblackVfx.FlatBuffers;

public class FlatBuffer : IDisposable
{
    private MemoryOwner<byte> owner_;
    public int cursor_;

    public FlatBuffer(MemoryOwner<byte> buffer)
        : this(buffer, 0) { }

    public FlatBuffer(MemoryOwner<byte> buffer, int origin)
    {
        owner_ = buffer;
        cursor_ = origin;
    }

    public Span<byte> Data => owner_.Span;
    public int Length => owner_.Length;
    public int Position
    {
        get { return cursor_; }
        set { cursor_ = value; }
    }

    public void Reset()
    {
        cursor_ = 0;
    }

    public sbyte ReadS8(int offset)
    {
        sbyte result = owner_.Memory.Span.Cast<byte, sbyte>()[offset];
        return result;
    }

    public byte ReadU8(int offset)
    {
        byte result = owner_.Memory.Span.Cast<byte, byte>()[offset];
        return result;
    }

    public short ReadS16(int offset)
    {
        short result = owner_.Memory.Span.Slice(offset).Cast<byte, short>()[0];
        return result;
    }

    public ushort ReadU16(int offset)
    {
        ushort result = owner_.Memory.Span.Slice(offset).Cast<byte, ushort>()[0];
        return result;
    }

    public int ReadS32(int offset)
    {
        int result = owner_.Memory.Span.Slice(offset).Cast<byte, int>()[0];
        return result;
    }

    public uint ReadU32(int offset)
    {
        uint result = owner_.Memory.Span.Slice(offset).Cast<byte, uint>()[0];
        return result;
    }

    public long ReadS64(int offset)
    {
        long result = owner_.Memory.Span.Slice(offset).Cast<byte, long>()[0];
        return result;
    }

    public ulong ReadU64(int offset)
    {
        ulong result = owner_.Memory.Span.Slice(offset).Cast<byte, ulong>()[0];
        return result;
    }

    public float ReadF32(int offset)
    {
        float result = owner_.Memory.Span.Slice(offset).Cast<byte, float>()[0];
        return result;
    }

    public string ReadString(int offset)
    {
        int size = ReadS32(offset);
        var result = owner_.Span.Slice(offset + 4, size);
        return Encoding.UTF8.GetString(result);
    }

    public string ReadString(int offset, int size)
    {
        var result = owner_.Span.Slice(offset, size);
        return Encoding.UTF8.GetString(result);
    }

    private void AssertOffsetAndLength(int offset, int length)
    {
        if (offset < 0 || offset > owner_.Length - length)
            throw new ArgumentOutOfRangeException();
    }

    public void PutS8(int offset, sbyte value)
    {
        AssertOffsetAndLength(offset, sizeof(byte));
        owner_.Span[offset] = (byte)value;
    }

    public void PutU8(int offset, byte value)
    {
        AssertOffsetAndLength(offset, sizeof(byte));
        owner_.Span[offset] = value;
    }

    public void PutU8(int offset, byte value, int count)
    {
        AssertOffsetAndLength(offset, sizeof(byte) * count);
        owner_.Span.Slice(offset, sizeof(byte) * count).Fill(value);
    }

    public void PutS16(int offset, short value)
    {
        AssertOffsetAndLength(offset, sizeof(short));
        owner_.Span.Slice(offset).Cast<byte, short>()[0] = value;
    }

    public void PutU16(int offset, ushort value)
    {
        AssertOffsetAndLength(offset, sizeof(ushort));
        owner_.Span.Slice(offset).Cast<byte, ushort>()[0] = value;
    }

    public void PutS32(int offset, int value)
    {
        AssertOffsetAndLength(offset, sizeof(int));
        owner_.Span.Slice(offset).Cast<byte, int>()[0] = value;
    }

    public void PutU32(int offset, uint value)
    {
        AssertOffsetAndLength(offset, sizeof(uint));
        owner_.Span.Slice(offset).Cast<byte, uint>()[0] = value;
    }

    public void PutS64(int offset, long value)
    {
        AssertOffsetAndLength(offset, sizeof(long));
        owner_.Span.Slice(offset).Cast<byte, long>()[0] = value;
    }

    public void PutU64(int offset, ulong value)
    {
        AssertOffsetAndLength(offset, sizeof(ulong));
        owner_.Span.Slice(offset).Cast<byte, ulong>()[0] = value;
    }

    public void PutFloat(int offset, float value)
    {
        AssertOffsetAndLength(offset, sizeof(float));
        owner_.Span.Slice(offset).Cast<byte, float>()[0] = value;
    }

    public void PutDouble(int offset, double value)
    {
        AssertOffsetAndLength(offset, sizeof(double));
        owner_.Span.Slice(offset).Cast<byte, double>()[0] = value;
    }

    public void Dispose()
    {
        owner_.Dispose();
    }
}

public interface IFlatBufferObj
{
    public void init(int origin, FlatBuffer buffer);
}

public struct FlatBufferTable
{
    public int origin_;
    public FlatBuffer buffer_;

    public int __offset(int offset)
    {
        int vtablePos = origin_ - buffer_.ReadS32(origin_);
        return buffer_.ReadS16(vtablePos + offset);
    }

    public int __relative(int offset)
    {
        return offset + buffer_.ReadS32(offset);
    }

    public int __vector_length(int offset)
    {
        offset += origin_;
        offset += buffer_.ReadS32(offset);
        return buffer_.ReadS32(offset);
    }

    public int __vector(int offset)
    {
        offset += origin_;
        return offset + buffer_.ReadS32(offset) + sizeof(int);
    }

    public string __string(int offset)
    {
        offset += buffer_.ReadS32(offset);
        var length = buffer_.ReadS32(offset);
        return Encoding.UTF8.GetString(buffer_.Data.Slice(offset + 4, length));
    }

    public byte[] __byteArray(int offset)
    {
        offset += buffer_.ReadS32(offset);
        var length = buffer_.ReadS32(offset);
        return buffer_.Data.Slice(offset + 4, length).ToArray();
    }
}

public struct FlatBufferStruct
{
    public int origin_;
    public FlatBuffer buffer_;
}

namespace ReblackVfx.FlatBuffers;

public struct LmVector4 : IFlatBufferObj
{
    public FlatBufferStruct s_;

    public void init(int origin, FlatBuffer buffer)
    {
        s_.origin_ = origin;
        s_.buffer_ = buffer;
    }

    public LmVector4 assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    public float X
    {
        get { return s_.buffer_.ReadF32(s_.origin_ + sizeof(float) * 0); }
    }
    public float Y
    {
        get { return s_.buffer_.ReadF32(s_.origin_ + sizeof(float) * 1); }
    }
    public float Z
    {
        get { return s_.buffer_.ReadF32(s_.origin_ + sizeof(float) * 2); }
    }
    public float W
    {
        get { return s_.buffer_.ReadF32(s_.origin_ + sizeof(float) * 3); }
    }
}

public struct LmVector3 : IFlatBufferObj
{
    public FlatBufferStruct s_;

    public void init(int origin, FlatBuffer buffer)
    {
        s_.origin_ = origin;
        s_.buffer_ = buffer;
    }

    public LmVector3 assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    public float X
    {
        get { return s_.buffer_.ReadF32(s_.origin_ + sizeof(float) * 0); }
    }
    public float Y
    {
        get { return s_.buffer_.ReadF32(s_.origin_ + sizeof(float) * 1); }
    }
    public float Z
    {
        get { return s_.buffer_.ReadF32(s_.origin_ + sizeof(float) * 2); }
    }
}

public struct LmVector2 : IFlatBufferObj
{
    public FlatBufferStruct s_;

    public void init(int origin, FlatBuffer buffer)
    {
        s_.origin_ = origin;
        s_.buffer_ = buffer;
    }

    public LmVector2 assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    public float X
    {
        get { return s_.buffer_.ReadF32(s_.origin_ + sizeof(float) * 0); }
    }
    public float Y
    {
        get { return s_.buffer_.ReadF32(s_.origin_ + sizeof(float) * 1); }
    }
}

public struct Matrix44 : IFlatBufferObj
{
    public FlatBufferStruct s_;

    public void init(int origin, FlatBuffer buffer)
    {
        s_.origin_ = origin;
        s_.buffer_ = buffer;
    }

    public Matrix44 assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    public LmVector4 Row0
    {
        get { return (new LmVector4()).assign(s_.origin_ + 0, s_.buffer_); }
    }
    public LmVector4 Row1
    {
        get { return (new LmVector4()).assign(s_.origin_ + 16, s_.buffer_); }
    }
    public LmVector4 Row2
    {
        get { return (new LmVector4()).assign(s_.origin_ + 32, s_.buffer_); }
    }
    public LmVector4 Row3
    {
        get { return (new LmVector4()).assign(s_.origin_ + 48, s_.buffer_); }
    }
}

public struct GraphPropertyValueTable : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }

    public GraphPropertyValueTable assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    public bool Boolean(int i)
    {
        int offset = t_.__offset(4);
        return offset != 0 ? 0 != t_.buffer_.ReadU8(t_.__vector(offset) + i) : false;
    }

    public int Int32(int i)
    {
        int offset = t_.__offset(10);
        return offset != 0 ? t_.buffer_.ReadS32(t_.__vector(offset) + i * sizeof(int)) : (int)0;
    }

    public float Float32(int i)
    {
        int offset = t_.__offset(22);
        return offset != 0 ? t_.buffer_.ReadF32(t_.__vector(offset) + i * sizeof(int)) : (int)0;
    }

    public byte[]? ByteArray(int i)
    {
        int offset = t_.__offset(26);
        return offset != 0 ? t_.__byteArray(t_.__vector(offset) + i * 4) : null;
    }

    public LmVector2? LmVector2(int i)
    {
        int offset = t_.__offset(30);
        return offset != 0
            ? (LmVector2?)(new LmVector2()).assign(t_.__vector(offset) + i * 16, t_.buffer_)
            : null;
    }

    public LmVector3? LmVector3(int i)
    {
        int offset = t_.__offset(32);
        return offset != 0
            ? (LmVector3?)(new LmVector3()).assign(t_.__vector(offset) + i * 16, t_.buffer_)
            : null;
    }

    public LmVector4? LmVector4(int i)
    {
        int offset = t_.__offset(34);
        return offset != 0
            ? (LmVector4?)(new LmVector4()).assign(t_.__vector(offset) + i * 16, t_.buffer_)
            : null;
    }

    public Matrix44? Matrix44(int i)
    {
        int offset = t_.__offset(42);
        return offset != 0
            ? (Matrix44?)(new Matrix44()).assign(t_.__vector(offset) + i * 64, t_.buffer_)
            : null;
    }

    public Int32VectorTable? Int32Vector(int i)
    {
        int offset = t_.__offset(54);
        return offset != 0
            ? (Int32VectorTable?)
                (new Int32VectorTable()).assign(
                    t_.__relative(t_.__vector(offset) + i * 4),
                    t_.buffer_
                )
            : null;
    }

    public int BooleanLength
    {
        get
        {
            int offset = t_.__offset(4);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }

    // public int Field01Length { get { int offset = t_.__offset(6); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field02Length { get { int offset = t_.__offset(8); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    public int Int32Length
    {
        get
        {
            int offset = t_.__offset(10);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }

    // public int Field04Length { get { int offset = t_.__offset(12); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field05Length { get { int offset = t_.__offset(14); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field06Length { get { int offset = t_.__offset(16); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field07Length { get { int offset = t_.__offset(18); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field08Length { get { int offset = t_.__offset(20); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    public int Float32Length
    {
        get
        {
            int offset = t_.__offset(22);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }

    // public int Field10Length { get { int offset = t_.__offset(24); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    public int ByteArrayLength
    {
        get
        {
            int offset = t_.__offset(26);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }

    // public int Field12Length { get { int offset = t_.__offset(28); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    public int Vector2Length
    {
        get
        {
            int offset = t_.__offset(30);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }
    public int Vector3Length
    {
        get
        {
            int offset = t_.__offset(32);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }
    public int Vector4Length
    {
        get
        {
            int offset = t_.__offset(34);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }

    // public int Field16Length { get { int offset = t_.__offset(36); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field17Length { get { int offset = t_.__offset(38); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field18Length { get { int offset = t_.__offset(40); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    public int Matrix44Length
    {
        get
        {
            int offset = t_.__offset(42);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }

    // public int Field20Length { get { int offset = t_.__offset(44); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field21Length { get { int offset = t_.__offset(46); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field22Length { get { int offset = t_.__offset(48); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field23Length { get { int offset = t_.__offset(50); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field24Length { get { int offset = t_.__offset(52); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    public int Int32VectorLength
    {
        get
        {
            int offset = t_.__offset(54);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }

    // public int Field26Length { get { int offset = t_.__offset(56); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field27Length { get { int offset = t_.__offset(58); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field28Length { get { int offset = t_.__offset(60); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field29Length { get { int offset = t_.__offset(62); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field30Length { get { int offset = t_.__offset(64); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field31Length { get { int offset = t_.__offset(66); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field32Length { get { int offset = t_.__offset(68); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field33Length { get { int offset = t_.__offset(70); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field34Length { get { int offset = t_.__offset(72); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field35Length { get { int offset = t_.__offset(74); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field36Length { get { int offset = t_.__offset(76); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field37Length { get { int offset = t_.__offset(78); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field38Length { get { int offset = t_.__offset(80); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field39Length { get { int offset = t_.__offset(82); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field40Length { get { int offset = t_.__offset(84); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field41Length { get { int offset = t_.__offset(86); return offset != 0 ? t_.__vector_length(offset) : 0; } }

    public static VectorOffset CreateBoolean_Vector(FlatBufferBuilder builder, bool[] data)
    {
        builder.StartVector(1, data.Length, 1);
        for (int i = data.Length - 1; i >= 0; i--)
            builder.AddBool(data[i]);
        return builder.EndVector();
    }

    public static VectorOffset CreateInt32_Vector(FlatBufferBuilder builder, int[] data)
    {
        builder.StartVector(4, data.Length, 4);
        for (int i = data.Length - 1; i >= 0; i--)
            builder.AddInt(data[i]);
        return builder.EndVector();
    }
}

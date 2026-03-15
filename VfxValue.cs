namespace ReblackVfx;

public struct GraphPropertyValueTable : IFlatBufferObj
{
    public FlatBufferTable t_;
    public void init(int origin, FlatBuffer buffer) { t_.origin_ = origin; t_.buffer_ = buffer; }
    public GraphPropertyValueTable assign(int origin, FlatBuffer buffer) { init(origin, buffer); return this; }

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

    public int BooleanLength { get { int offset = t_.__offset(4); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field01Length { get { int offset = t_.__offset(6); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field02Length { get { int offset = t_.__offset(8); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    public int Int32Length { get { int offset = t_.__offset(10); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field04Length { get { int offset = t_.__offset(12); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field05Length { get { int offset = t_.__offset(14); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field06Length { get { int offset = t_.__offset(16); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field07Length { get { int offset = t_.__offset(18); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field08Length { get { int offset = t_.__offset(20); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    public int Float32Length { get { int offset = t_.__offset(22); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field10Length { get { int offset = t_.__offset(24); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    public int StringLength { get { int offset = t_.__offset(26); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field12Length { get { int offset = t_.__offset(28); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field13Length { get { int offset = t_.__offset(30); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field14Length { get { int offset = t_.__offset(32); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field15Length { get { int offset = t_.__offset(34); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field16Length { get { int offset = t_.__offset(36); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field17Length { get { int offset = t_.__offset(38); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field18Length { get { int offset = t_.__offset(40); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    public int Matrix44Length { get { int offset = t_.__offset(42); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field20Length { get { int offset = t_.__offset(44); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field21Length { get { int offset = t_.__offset(46); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field22Length { get { int offset = t_.__offset(48); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field23Length { get { int offset = t_.__offset(50); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field24Length { get { int offset = t_.__offset(52); return offset != 0 ? t_.__vector_length(offset) : 0; } }
    // public int Field25Length { get { int offset = t_.__offset(54); return offset != 0 ? t_.__vector_length(offset) : 0; } }
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
}

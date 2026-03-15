using System.Numerics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Text;

namespace ReblackVfx;

public enum VfxValueType
{
    Bool = 0x1,
    Int32 = 0x2,
    Float32 = 0x3,
    Vector2 = 0x4,
    Vector3 = 0x5,
    Vector4 = 0x6,
    Matrix44 = 0x7,
    String = 0x8,
    Pointer = 0x9,
    Int32Array = 0xA,
    FCurve = 0xB,
    Float32Array = 0xC,
    Vector4Array = 0xF,
}

public abstract class VfxPropertyBase : IXmlSerializable
{
    public uint MemberId;
    public string? Name;
    public uint Hash;

    public abstract XmlSchema? GetSchema();
    public abstract void ReadXml(XmlReader reader);
    public abstract void WriteXml(XmlWriter writer);

    public static VfxPropertyBase Invalid(VfxPropertyDataTable prop, GraphPropertyValueTable val)
    {
        throw new NotImplementedException();
    }
}

public class VfxBoolProperty : VfxPropertyBase
{
    private static readonly VfxValueType Type = VfxValueType.Bool;
    public bool Value;

    public override XmlSchema? GetSchema()
    {
        throw new NotImplementedException();
    }

    public override void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement($"Property_{MemberId:X}");

        writer.WriteStartAttribute("Type");
        writer.WriteString("Bool");
        writer.WriteEndAttribute();

        writer.WriteStartAttribute("Hash");
        writer.WriteValue(Hash);
        writer.WriteEndAttribute();

        writer.WriteValue(Value);

        writer.WriteEndElement();
    }

    public static VfxPropertyBase FromFlat(VfxPropertyDataTable prop, GraphPropertyValueTable val)
    {
        int vIndex = prop.PropertyValue.ValueIndex;

        return new VfxBoolProperty
        {
            MemberId = (uint)prop.Identity.MemberId,
            Hash = prop.Identity.Name,
            Value = val.Boolean(vIndex)
        };
    }
}

public class VfxFloat32Property : VfxPropertyBase
{
    private static readonly VfxValueType Type = VfxValueType.Float32;
    public float Value;

    public override XmlSchema? GetSchema()
    {
        throw new NotImplementedException();
    }

    public override void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement($"Property_{MemberId:X}");

        writer.WriteStartAttribute("Type");
        writer.WriteString("Float");
        writer.WriteEndAttribute();

        writer.WriteStartAttribute("Hash");
        writer.WriteValue(Hash);
        writer.WriteEndAttribute();

        writer.WriteValue(Value);

        writer.WriteEndElement();
    }

    public static VfxPropertyBase FromFlat(VfxPropertyDataTable prop, GraphPropertyValueTable val)
    {
        int vIndex = prop.PropertyValue.ValueIndex;

        return new VfxFloat32Property
        {
            MemberId = (uint)prop.Identity.MemberId,
            Hash = prop.Identity.Name,
            Value = val.Float32(vIndex)
        };
    }
}

public class VfxInt32Property : VfxPropertyBase
{
    private static readonly VfxValueType Type = VfxValueType.Int32;
    public int Value;

    public override XmlSchema? GetSchema()
    {
        throw new NotImplementedException();
    }

    public override void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement($"Property_{MemberId:X}");

        writer.WriteStartAttribute("Type");
        writer.WriteString("Int");
        writer.WriteEndAttribute();

        writer.WriteStartAttribute("Hash");
        writer.WriteValue(Hash);
        writer.WriteEndAttribute();

        writer.WriteValue(Value);

        writer.WriteEndElement();
    }

    public static VfxPropertyBase FromFlat(VfxPropertyDataTable prop, GraphPropertyValueTable val)
    {
        int vIndex = prop.PropertyValue.ValueIndex;

        return new VfxInt32Property
        {
            MemberId = (uint)prop.Identity.MemberId,
            Hash = prop.Identity.Name,
            Value = val.Int32(vIndex)
        };
    }
}

public class VfxVector2Property : VfxPropertyBase
{
    private static readonly VfxValueType Type = VfxValueType.Vector2;
    public Vector2 Value;

    public override XmlSchema? GetSchema()
    {
        throw new NotImplementedException();
    }

    public override void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement($"Property_{MemberId:X}");

        writer.WriteStartAttribute("Type");
        writer.WriteString("Float2");
        writer.WriteEndAttribute();

        writer.WriteStartAttribute("Hash");
        writer.WriteValue(Hash);
        writer.WriteEndAttribute();

        writer.WriteString($"{Value.X},{Value.Y}");

        writer.WriteEndElement();
    }

    public static VfxPropertyBase FromFlat(VfxPropertyDataTable prop, GraphPropertyValueTable val)
    {
        int vIndex = prop.PropertyValue.ValueIndex;
        // TODO: Actually read Vector2

        return new VfxVector2Property
        {
            MemberId = (uint)prop.Identity.MemberId,
            Hash = prop.Identity.Name,
            Value = Vector2.Zero
        };
    }
}

public class VfxVector3Property : VfxPropertyBase
{
    private static readonly VfxValueType Type = VfxValueType.Vector3;
    public Vector3 Value;

    public override XmlSchema? GetSchema()
    {
        throw new NotImplementedException();
    }

    public override void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement($"Property_{MemberId:X}");

        writer.WriteStartAttribute("Type");
        writer.WriteString("Float3");
        writer.WriteEndAttribute();

        writer.WriteStartAttribute("Hash");
        writer.WriteValue(Hash);
        writer.WriteEndAttribute();

        writer.WriteString($"{Value.X},{Value.Y},{Value.Z}");

        writer.WriteEndElement();
    }

    public static VfxPropertyBase FromFlat(VfxPropertyDataTable prop, GraphPropertyValueTable val)
    {
        int vIndex = prop.PropertyValue.ValueIndex;
        // TODO: Actually read Vector3

        return new VfxVector3Property
        {
            MemberId = (uint)prop.Identity.MemberId,
            Hash = prop.Identity.Name,
            Value = Vector3.Zero
        };
    }
}

public class VfxVector4Property : VfxPropertyBase
{
    private static readonly VfxValueType Type = VfxValueType.Vector4;
    public Vector4 Value;

    public override XmlSchema? GetSchema()
    {
        throw new NotImplementedException();
    }

    public override void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement($"Property_{MemberId:X}");

        writer.WriteStartAttribute("Type");
        writer.WriteString("Float4");
        writer.WriteEndAttribute();

        writer.WriteStartAttribute("Hash");
        writer.WriteValue(Hash);
        writer.WriteEndAttribute();

        writer.WriteString($"{Value.X},{Value.Y},{Value.Z},{Value.W}");

        writer.WriteEndElement();
    }

    public static VfxPropertyBase FromFlat(VfxPropertyDataTable prop, GraphPropertyValueTable val)
    {
        int vIndex = prop.PropertyValue.ValueIndex;
        // TODO: Actually read Vector4

        return new VfxVector4Property
        {
            MemberId = (uint)prop.Identity.MemberId,
            Hash = prop.Identity.Name,
            Value = Vector4.Zero
        };
    }
}

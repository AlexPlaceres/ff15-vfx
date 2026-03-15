using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ReblackVfx;

public class VFXGraphTrayData : IXmlSerializable
{
    public XmlSchema? GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }
}

public class VFXSignalLinkData : IXmlSerializable
{
    public uint SrcNode;
    public uint SrcPort;
    public uint DstNode;
    public uint DstPort;

    public XmlSchema? GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }
}

public class VFXPropertyLinkData : IXmlSerializable
{
    public uint SrcNode;

    public uint SrcPort;

    public uint DstNode;

    public uint DstPort;

    public XmlSchema? GetSchema()
    {
        return (null);
    }

    public void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Link");

        writer.WriteStartAttribute("SrcNode");
        writer.WriteValue(SrcNode);
        writer.WriteEndAttribute();

        writer.WriteStartAttribute("SrcPort");
        writer.WriteValue(SrcPort);
        writer.WriteEndAttribute();

        writer.WriteStartAttribute("DstNode");
        writer.WriteValue(DstNode);
        writer.WriteEndAttribute();

        writer.WriteStartAttribute("DstPort");
        writer.WriteValue(DstPort);
        writer.WriteEndAttribute();

        writer.WriteEndElement();
    }
}

public class VfxNodeData : IXmlSerializable
{
    public uint NodeId;

    public uint NodeTypeId;

    public List<VfxPropertyBase> Properties = [];

    public XmlSchema? GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement($"Node_{NodeTypeId:X4}");

        writer.WriteStartAttribute("Id");
        writer.WriteValue(NodeId);
        writer.WriteEndAttribute();

        writer.WriteStartAttribute("TypeId");
        writer.WriteValue(NodeTypeId);
        writer.WriteEndAttribute();

        foreach (var prop in Properties)
        {
            prop.WriteXml(writer);
        }

        writer.WriteEndElement();
    }
}

public class VfxGraphData : IXmlSerializable
{
    public List<VfxNodeData> Nodes = [];
    public List<VFXGraphTrayData> Trays = [];
    public List<VFXSignalLinkData> SignalLinks = [];
    public List<VFXPropertyLinkData> PropertyLinks = [];

    public XmlSchema? GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Nodes");
        foreach (var node in Nodes)
        {
            node.WriteXml(writer);
        }
        writer.WriteEndElement();
    }
}

using System.Xml;
using System.Xml.Linq;
using CommunityToolkit.HighPerformance.Buffers;
using ReblackVfx;
using ReblackVfx.FlatBuffers;

public struct VfxGraphTable : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }

    public VfxGraphTable assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    // NOTE: Version info isn't really necessary but maybe later

    public VfxGraphContainerTable GraphContainer
    {
        get
        {
            int offset = t_.__offset(6);
            return new VfxGraphContainerTable().assign(
                t_.__relative(t_.origin_ + offset),
                t_.buffer_
            );
        }
    }

    public GraphPropertyValueTable ValueData
    {
        get
        {
            int offset = t_.__offset(8);
            return new GraphPropertyValueTable().assign(
                t_.__relative(t_.origin_ + offset),
                t_.buffer_
            );
        }
    }
}

public struct VfxTimeline : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }

    public VfxTimeline assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }
}

public struct VfxGraphRoot
{
    public FlatBufferTable t_;
    public VfxGraphTable Graph
    {
        get
        {
            int offset = t_.__offset(4);
            int origin = t_.__relative(t_.origin_ + offset);
            return new VfxGraphTable().assign(origin, t_.buffer_);
        }
    }
    public VfxTimelineDataTable? Timeline
    {
        get
        {
            int offset = t_.__offset(6);
            int origin = t_.__relative(t_.origin_ + offset);
            return new VfxTimelineDataTable().assign(origin, t_.buffer_);
        }
    }

    public static VfxGraphRoot GetRoot(FlatBuffer buffer, int origin)
    {
        var result = new VfxGraphRoot();

        // Initialize
        result.t_.buffer_ = buffer;
        result.t_.origin_ = origin;
        return result;
    }
}

internal static class Program
{
    private static readonly string FILEPATH = @"/home/cherry/Downloads/sg_titan_warp_01.vfx";

    public static void ToXml()
    {
        long size;
        MemoryOwner<byte> buffer;
        using (var stream = new FileStream(FILEPATH, FileMode.Open))
        {
            size = stream.Length;
            buffer = MemoryOwner<byte>.Allocate((int)size);
            stream.ReadExactly(buffer.Span);
        }

        FlatBuffer fb = new FlatBuffer(buffer);
        int origin = fb.ReadS32(16);

        var root = VfxGraphRoot.GetRoot(fb, 16 + origin);

        var graph = root.Graph;
        var container = graph.GraphContainer;
        var count = container.NodesLength;

        var valueData = graph.ValueData;
        var timelineData = root.Timeline;

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.OmitXmlDeclaration = true;
        settings.Async = false;
        using (
            var stream = new FileStream(
                "/home/cherry/Programming/lm-vfx/toxml/mockup.xml",
                FileMode.OpenOrCreate
            )
        )
        {
            using XmlWriter writer = XmlWriter.Create(stream, settings);

            writer.WriteStartElement("vfxGraph");

            writer.WriteStartAttribute("name");
            writer.WriteValue($"{Path.GetFileNameWithoutExtension(FILEPATH)}");
            writer.WriteEndAttribute();

            writer.WriteStartElement("components");
            writer.WriteStartElement("nodes");
            for (var i = 0; i < count; i++)
            {
                VfxNodeDataTable node = container.Nodes(i);
                var id = node.Identity;
                writer.WriteStartElement("node");

                writer.WriteStartAttribute("instanceId");
                writer.WriteValue(id.InstanceId);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("type");
                writer.WriteValue(
                    NodeMetadata.NodeType.ContainsKey((int)id.DesignId)
                        ? NodeMetadata.NodeType[(int)id.DesignId]
                        : $"VfxNode_{id.DesignId:X}"
                );
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("typeId");
                writer.WriteValue(id.DesignId);
                writer.WriteEndAttribute();

                // properties
                var propCount = node.PropertiesLength;
                for (var j = 0; j < propCount; j++)
                {
                    VfxPropertyDataTable prop = node.Properties(j);
                    var pId = prop.Identity;
                    var pVal = prop.PropertyValue;
                    writer.WriteStartElement("property");

                    writer.WriteStartAttribute("memberId");
                    writer.WriteValue(pId.MemberId);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("name");
                    writer.WriteValue(pId.Name);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("valueType");
                    writer.WriteValue(prop.ValueType);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("valueRef");
                    writer.WriteValue(pVal.ValueRef);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("valueIndex");
                    writer.WriteValue(pVal.ValueIndex);
                    writer.WriteEndAttribute();

                    writer.WriteEndElement(); // </property>
                }

                var inCount = node.InPortsLength;
                for (var j = 0; j < inCount; j++)
                {
                    if (node.InPorts(j) is VfxSignalDataTable t)
                    {
                        var identity = t.Identity!.Value;
                        writer.WriteStartElement("port");
                        writer.WriteStartAttribute("memberId");
                        writer.WriteValue(identity.MemberId);
                        writer.WriteEndAttribute();

                        writer.WriteStartAttribute("name");
                        writer.WriteValue(identity.Name);
                        writer.WriteEndAttribute();

                        writer.WriteStartAttribute("argumentTypes");
                        writer.WriteValue(t.ArgumentTypes);
                        writer.WriteEndAttribute();

                        writer.WriteEndElement(); // </port>
                    }
                    else
                    {
                        throw new InvalidDataException();
                    }
                }

                var outCount = node.OutPortsLength;
                for (var j = 0; j < outCount; j++)
                {
                    if (node.OutPorts(j) is VfxSignalDataTable t)
                    {
                        var identity = t.Identity!.Value;
                        writer.WriteStartElement("port");
                        writer.WriteStartAttribute("memberId");
                        writer.WriteValue(identity.MemberId);
                        writer.WriteEndAttribute();

                        writer.WriteStartAttribute("name");
                        writer.WriteValue(identity.Name);
                        writer.WriteEndAttribute();

                        writer.WriteStartAttribute("argumentTypes");
                        writer.WriteValue(t.ArgumentTypes);
                        writer.WriteEndAttribute();

                        writer.WriteEndElement(); // </port>
                    }
                    else
                    {
                        throw new InvalidDataException();
                    }
                }

                writer.WriteEndElement(); // </node>
            }
            writer.WriteEndElement(); // </nodes>

            var trayCount = container.TraysLength;
            writer.WriteStartElement("trays");
            for (var i = 0; i < trayCount; i++)
            {
                writer.WriteStartElement("tray");
                if (container.Trays(i) is VfxTrayDataTable t)
                {
                    var identity = t.Identity!.Value;

                    writer.WriteStartAttribute("instanceId");
                    writer.WriteValue(identity.InstanceId);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("type");
                    writer.WriteValue(
                        NodeMetadata.NodeType.ContainsKey((int)identity.DesignId)
                            ? NodeMetadata.NodeType[(int)identity.DesignId]
                            : $"VfxNode_{identity.DesignId:X}"
                    );
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("typeId");
                    writer.WriteValue(identity.DesignId);
                    writer.WriteEndAttribute();

                    writer.WriteStartElement("nodes");
                    var nodeCount = t.ChildrenLength;
                    for (var j = 0; j < nodeCount; j++)
                    {
                        writer.WriteStartElement("node");

                        writer.WriteStartAttribute("instanceId");
                        writer.WriteValue(t.Children(j));
                        writer.WriteEndAttribute();

                        writer.WriteEndElement(); // </node>
                    }
                    writer.WriteEndElement(); // </nodes>
                }
                else
                    throw new InvalidDataException();
                writer.WriteEndElement(); // </tray>
            }
            writer.WriteEndElement(); // </trays>

            writer.WriteStartElement("signalLinks");
            var signalLinkCount = container.SignalLinksLength;
            for (var i = 0; i < signalLinkCount; i++)
            {
                writer.WriteStartElement("link");
                if (container.SignalLinks(i) is VfxLinkDataTable t)
                {
                    writer.WriteStartAttribute("instanceid");
                    writer.WriteValue(t.InstanceId);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("sourceNodeId");
                    writer.WriteValue(t.SourceNodeId);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("sourceMemberId");
                    writer.WriteValue(t.SourceMemberId);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("destinationNodeId");
                    writer.WriteValue(t.DestinationNodeId);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("destinationMemberId");
                    writer.WriteValue(t.DestinationMemberId);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("linkType");
                    writer.WriteValue(t.LinkType);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("unknown");
                    writer.WriteValue(t.Unknown);
                    writer.WriteEndAttribute();
                }
                else
                    throw new InvalidDataException();
                writer.WriteEndElement(); // </link>
            }
            writer.WriteEndElement(); // </signalLinks>

            writer.WriteStartElement("propertyLinks");
            var propLinkCount = container.PropertyLinksLength;
            for (var i = 0; i < propLinkCount; i++)
            {
                writer.WriteStartElement("link");
                if (container.PropertyLinks(i) is VfxLinkDataTable t)
                {
                    writer.WriteStartAttribute("instanceid");
                    writer.WriteValue(t.InstanceId);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("sourceNodeId");
                    writer.WriteValue(t.SourceNodeId);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("sourceMemberId");
                    writer.WriteValue(t.SourceMemberId);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("destinationNodeId");
                    writer.WriteValue(t.DestinationNodeId);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("destinationMemberId");
                    writer.WriteValue(t.DestinationMemberId);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("linkType");
                    writer.WriteValue(t.LinkType);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("unknown");
                    writer.WriteValue(t.Unknown);
                    writer.WriteEndAttribute();
                }
                else
                    throw new InvalidDataException();
                writer.WriteEndElement(); // </link>
            }
            writer.WriteEndElement(); // </propertyLinks>

            writer.WriteEndElement(); // </components>

            // VECTOR DATA
            writer.WriteStartElement("graphVectorData");

            writer.WriteStartElement("booleans");
            var boolCount = valueData.BooleanLength;
            for (var i = 0; i < boolCount; i++)
            {
                writer.WriteStartElement("boolean");

                writer.WriteStartAttribute("index");
                writer.WriteValue(i);
                writer.WriteEndAttribute();

                writer.WriteValue(valueData.Boolean(i));

                writer.WriteEndElement(); // </boolean>
            }
            writer.WriteEndElement(); // </booleans>

            writer.WriteStartElement("ints");
            var intCount = valueData.Int32Length;
            for (var i = 0; i < intCount; i++)
            {
                writer.WriteStartElement("int");

                writer.WriteStartAttribute("index");
                writer.WriteValue(i);
                writer.WriteEndAttribute();

                writer.WriteValue(valueData.Int32(i));

                writer.WriteEndElement(); // </int>
            }
            writer.WriteEndElement(); // </ints>

            writer.WriteStartElement("floats");
            var floatCount = valueData.Float32Length;
            for (var i = 0; i < floatCount; i++)
            {
                writer.WriteStartElement("float");

                writer.WriteStartAttribute("index");
                writer.WriteValue(i);
                writer.WriteEndAttribute();

                writer.WriteValue(valueData.Float32(i));

                writer.WriteEndElement(); // </float>
            }
            writer.WriteEndElement(); // </floats>

            writer.WriteStartElement("strings");
            var strCount = valueData.StringLength;
            for (var i = 0; i < strCount; i++)
            {
                writer.WriteStartElement("string");

                writer.WriteStartAttribute("index");
                writer.WriteValue(i);
                writer.WriteEndAttribute();

                writer.WriteValue(valueData.String(i));

                writer.WriteEndElement(); // </string>
            }
            writer.WriteEndElement(); // </strings>

            writer.WriteStartElement("float2s");
            var float2Count = valueData.Vector2Length;
            for (var i = 0; i < float2Count; i++)
            {
                writer.WriteStartElement("float2");

                writer.WriteStartAttribute("index");
                writer.WriteValue(i);
                writer.WriteEndAttribute();

                if (valueData.LmVector2(i) is LmVector2 m)
                {
                    writer.WriteValue($"{m.X},{m.Y}");
                }
                else
                    throw new InvalidDataException();

                writer.WriteEndElement(); // </float2>
            }
            writer.WriteEndElement(); // </float2s>

            writer.WriteStartElement("float3s");
            var float3Count = valueData.Vector3Length;
            for (var i = 0; i < float3Count; i++)
            {
                writer.WriteStartElement("float3");

                writer.WriteStartAttribute("index");
                writer.WriteValue(i);
                writer.WriteEndAttribute();

                if (valueData.LmVector3(i) is LmVector3 m)
                {
                    writer.WriteValue($"{m.X},{m.Y},{m.Z}");
                }
                else
                    throw new InvalidDataException();

                writer.WriteEndElement(); // </float3>
            }
            writer.WriteEndElement(); // </float3s>

            writer.WriteStartElement("float4s");
            var float4Count = valueData.Vector4Length;
            for (var i = 0; i < float4Count; i++)
            {
                writer.WriteStartElement("float4");

                writer.WriteStartAttribute("index");
                writer.WriteValue(i);
                writer.WriteEndAttribute();

                if (valueData.LmVector4(i) is LmVector4 m)
                {
                    writer.WriteValue($"{m.X},{m.Y},{m.Z},{m.W}");
                }
                else
                    throw new InvalidDataException();

                writer.WriteEndElement(); // </float4>
            }
            writer.WriteEndElement(); // </float4s>

            writer.WriteStartElement("matrices");
            var matrixCount = valueData.Matrix44Length;
            for (var i = 0; i < matrixCount; i++)
            {
                writer.WriteStartElement("matrix");

                writer.WriteStartAttribute("index");
                writer.WriteValue(i);
                writer.WriteEndAttribute();

                if (valueData.Matrix44(i) is Matrix44 m)
                {
                    writer.WriteStartElement("row0");
                    writer.WriteValue($"{m.Row0.X},{m.Row0.Y},{m.Row0.Z},{m.Row0.W}");
                    writer.WriteEndElement();

                    writer.WriteStartElement("row1");
                    writer.WriteValue($"{m.Row1.X},{m.Row1.Y},{m.Row1.Z},{m.Row1.W}");
                    writer.WriteEndElement();

                    writer.WriteStartElement("row2");
                    writer.WriteValue($"{m.Row2.X},{m.Row2.Y},{m.Row2.Z},{m.Row2.W}");
                    writer.WriteEndElement();

                    writer.WriteStartElement("row3");
                    writer.WriteValue($"{m.Row3.X},{m.Row3.Y},{m.Row3.Z},{m.Row3.W}");
                    writer.WriteEndElement();
                }
                else
                    throw new InvalidDataException();

                writer.WriteEndElement(); // </matrix>
            }
            writer.WriteEndElement(); // </matrices>

            writer.WriteStartElement("int32Vectors");
            var int32VecCount = valueData.Int32VectorLength;
            for (var i = 0; i < int32VecCount; i++)
            {
                writer.WriteStartElement("int32Vector");

                writer.WriteStartAttribute("index");
                writer.WriteValue(i);
                writer.WriteEndAttribute();

                if (valueData.Int32Vector(i) is Int32VectorTable t)
                {
                    int vCount = t.ValuesLength;
                    for (var j = 0; j < vCount; j++)
                    {
                        writer.WriteStartElement("int32");
                        writer.WriteValue(t.Values(j));
                        writer.WriteEndElement(); // </int32>
                    }
                }
                else
                    throw new InvalidDataException();

                writer.WriteEndElement(); // </int32Vector>
            }
            writer.WriteEndElement(); // </int32Vectors>

            writer.WriteEndElement(); // </graphVectorData>
            writer.WriteEndElement(); // </vfxGraph>
        }
    }

    public static void ToBinary(string filePath)
    {
        XElement root = XElement.Load(filePath);
        if ((string?)root.Attribute("name") is string s)
        {
            Console.WriteLine($"Attempting to serialize VFX Graph {s}...");
        }
    }

    public static void Main(string[] args)
    {
        ToBinary("/home/cherry/Programming/lm-vfx/toxml/mockup.xml");
    }
}

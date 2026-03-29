using System.Xml.Linq;
using Google.FlatBuffers;
using ReblackVfx.FlatBuffer.BaseType;
using ReblackVfx.FlatBuffer.Graph;
using ReblackVfx.FlatBuffer.Math;
using ReblackVfx.FlatBuffer.VfxGraph;

namespace ReblackVfx;

public static class VfxConverter
{
    private static VectorOffset serializeByteData(FlatBufferBuilder builder, XElement xml)
    {
        var elements = xml.Elements();
        sbyte[] values = new sbyte[elements.Count()];
        int i = 0;
        foreach (var element in elements)
        {
            if (elements is not XElement e)
                throw new InvalidDataException();
            values[i++] = sbyte.Parse(e.Value);
        }
        return GraphBaseTypeData.CreateByteDataVector(builder, values);
    }

    private static VectorOffset serializeBoolData(FlatBufferBuilder builder, XElement xml)
    {
        var elements = xml.Elements();
        bool[] values = new bool[elements.Count()];
        int i = 0;
        foreach (var element in elements)
        {
            if (elements is not XElement e)
                throw new InvalidDataException();
            values[i++] = bool.Parse(e.Value);
        }
        return GraphBaseTypeData.CreateBoolDataVector(builder, values);
    }

    private static VectorOffset serializeInt16Data(FlatBufferBuilder builder, XElement xml)
    {
        var elements = xml.Elements();
        short[] values = new short[elements.Count()];
        int i = 0;
        foreach (var element in elements)
        {
            if (elements is not XElement e)
                throw new InvalidDataException();
            values[i++] = short.Parse(e.Value);
        }
        return GraphBaseTypeData.CreateInt16DataVector(builder, values);
    }

    private static VectorOffset serializeInt32Data(FlatBufferBuilder builder, XElement xml)
    {
        var elements = xml.Elements();
        int[] values = new int[elements.Count()];
        int i = 0;
        foreach (var element in elements)
        {
            if (elements is not XElement e)
                throw new InvalidDataException();
            values[i++] = int.Parse(e.Value);
        }
        return GraphBaseTypeData.CreateInt32DataVector(builder, values);
    }

    private static VectorOffset serializeInt64Data(FlatBufferBuilder builder, XElement xml)
    {
        var elements = xml.Elements();
        long[] values = new long[elements.Count()];
        long i = 0;
        foreach (var element in elements)
        {
            if (elements is not XElement e)
                throw new InvalidDataException();
            values[i++] = long.Parse(e.Value);
        }
        return GraphBaseTypeData.CreateInt64DataVector(builder, values);
    }

    private static VectorOffset serializeUbyteData(FlatBufferBuilder builder, XElement xml)
    {
        var elements = xml.Elements();
        byte[] values = new byte[elements.Count()];
        int i = 0;
        foreach (var element in elements)
        {
            if (elements is not XElement e)
                throw new InvalidDataException();
            values[i++] = byte.Parse(e.Value);
        }
        return GraphBaseTypeData.CreateUbyteDataVector(builder, values);
    }

    private static VectorOffset serializeUint16Data(FlatBufferBuilder builder, XElement xml)
    {
        var elements = xml.Elements();
        ushort[] values = new ushort[elements.Count()];
        int i = 0;
        foreach (var element in elements)
        {
            if (elements is not XElement e)
                throw new InvalidDataException();
            values[i++] = ushort.Parse(e.Value);
        }
        return GraphBaseTypeData.CreateUint16DataVector(builder, values);
    }

    private static VectorOffset serializeUint32Data(FlatBufferBuilder builder, XElement xml)
    {
        var elements = xml.Elements();
        uint[] values = new uint[elements.Count()];
        int i = 0;
        foreach (var element in elements)
        {
            if (elements is not XElement e)
                throw new InvalidDataException();
            values[i++] = uint.Parse(e.Value);
        }
        return GraphBaseTypeData.CreateUint32DataVector(builder, values);
    }

    private static VectorOffset serializeUint64Data(FlatBufferBuilder builder, XElement xml)
    {
        var elements = xml.Elements();
        ulong[] values = new ulong[elements.Count()];
        int i = 0;
        foreach (var element in elements)
        {
            if (elements is not XElement e)
                throw new InvalidDataException();
            values[i++] = ulong.Parse(e.Value);
        }
        return GraphBaseTypeData.CreateUint64DataVector(builder, values);
    }

    private static T[] parsePrimitive<T>(XElement xml)
        where T : IParsable<T>
    {
        var elements = xml.Elements();
        T[] values = new T[elements.Count()];
        int i = 0;
        foreach (var element in elements)
        {
            if (elements is not XElement e)
                throw new InvalidDataException();
            values[i++] = T.Parse(e.Value, null);
        }
        return values;
    }

    public static Offset<GraphBaseTypeData> SerializeBaseTypeData(
        FlatBufferBuilder builder,
        XElement xml
    )
    {
        return GraphBaseTypeData.CreateGraphBaseTypeData(
            builder,
            xml.Element("byteData") is XElement byteData
                ? GraphBaseTypeData.CreateByteDataVector(builder, parsePrimitive<sbyte>(byteData))
                : new VectorOffset(0),
            xml.Element("boolData") is XElement boolData
                ? GraphBaseTypeData.CreateBoolDataVector(builder, parsePrimitive<bool>(boolData))
                : new VectorOffset(0),
            xml.Element("int16Data") is XElement int16Data
                ? GraphBaseTypeData.CreateInt16DataVector(builder, parsePrimitive<short>(int16Data))
                : new VectorOffset(0),
            xml.Element("int32Data") is XElement int32Data
                ? GraphBaseTypeData.CreateInt32DataVector(builder, parsePrimitive<int>(int32Data))
                : new VectorOffset(0),
            xml.Element("int64Data") is XElement int64Data
                ? GraphBaseTypeData.CreateInt64DataVector(builder, parsePrimitive<long>(int64Data))
                : new VectorOffset(0),
            xml.Element("ubyteData") is XElement ubyteData
                ? GraphBaseTypeData.CreateUbyteDataVector(builder, parsePrimitive<byte>(ubyteData))
                : new VectorOffset(0),
            xml.Element("uint16Data") is XElement uInt16Data
                ? GraphBaseTypeData.CreateUint16DataVector(
                    builder,
                    parsePrimitive<ushort>(uInt16Data)
                )
                : new VectorOffset(0),
            xml.Element("uint32Data") is XElement uInt32Data
                ? GraphBaseTypeData.CreateUint32DataVector(
                    builder,
                    parsePrimitive<uint>(uInt32Data)
                )
                : new VectorOffset(0),
            xml.Element("uint64Data") is XElement uInt64Data
                ? GraphBaseTypeData.CreateUint64DataVector(
                    builder,
                    parsePrimitive<ulong>(uInt64Data)
                )
                : new VectorOffset(0),
            xml.Element("floatData") is XElement floatData
                ? GraphBaseTypeData.CreateFloatDataVector(builder, parsePrimitive<float>(floatData))
                : new VectorOffset(0),
            xml.Element("doubleData") is XElement doubleData
                ? GraphBaseTypeData.CreateDoubleDataVector(
                    builder,
                    parsePrimitive<double>(doubleData)
                )
                : new VectorOffset(0),
            xml.Element("stringData") is XElement stringData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("enumData") is XElement enumData
                ? GraphBaseTypeData.CreateEnumDataVector(builder, parsePrimitive<int>(enumData))
                : new VectorOffset(0),
            xml.Element("vector2Data") is XElement vector2Data
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("vector3Data") is XElement vector3Data
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("vector4Data") is XElement vector4Data
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("colorData") is XElement colorData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("matrix33Data") is XElement matrix33Data
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("matrix34Data") is XElement matrix34Data
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("matrix44Data") is XElement matrix44Data
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("quaternionData") is XElement quaternionData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("eulerData") is XElement eulerData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("byteArrayData") is XElement byteArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("boolArrayData") is XElement boolArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("int16ArrayData") is XElement int16ArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("int32ArrayData") is XElement int32ArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("int64ArrayData") is XElement int64ArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("uint16ArrayData") is XElement uint16ArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("uint32ArrayData") is XElement uint32ArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("uint64ArrayData") is XElement uint64ArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("floatArrayData") is XElement floatArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("doubleArrayData") is XElement doubleArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("stringArrayData") is XElement stringArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("vector2ArrayData") is XElement vector2ArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("vector3ArrayData") is XElement vector3ArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("vector4ArrayData") is XElement vector4ArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("colorArrayData") is XElement colorArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("matrix33ArrayData") is XElement matrix33ArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("matrix34ArrayData") is XElement matrix34ArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("matrix44ArrayData") is XElement matrix44ArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0),
            xml.Element("eulerArrayData") is XElement eulerArrayData
                ? new VectorOffset(0)
                : new VectorOffset(0)
        );
    }

    private static XElement graphContainerToXml(GraphContainer data)
    {
        XElement[] nodesXml = new XElement[data.NodesLength];
        XElement[] traysXml = new XElement[data.TraysLength];
        XElement[] signalLinksXml = new XElement[data.SignalLinksLength];
        XElement[] propertyLinksXml = new XElement[data.PropertyLinksLength];

        // Nodes
        for (int i = 0; i < nodesXml.Length; i++)
        {
            if (
                data.Nodes(i) is not GraphNodeData graphNodeData
                || graphNodeData.Identity is not GraphNodeDataBase identity
            )
                throw new InvalidDataException();

            XElement[] properties = new XElement[graphNodeData.PropertiesLength];
            XElement[] inPorts = new XElement[graphNodeData.InPortsLength];
            XElement[] outPorts = new XElement[graphNodeData.OutPortsLength];

            string? name = null;
            if (!NodeMetadata.NodeType.TryGetValue((int)identity.TypeId, out name))
                name = $"Node_{identity.TypeId:X}";

            for (int j = 0; j < properties.Length; j++)
            {
                if (
                    graphNodeData.Properties(j) is not GraphNodeProperty p
                    || p.Identity is not GraphNodeMemberBase propId
                    || p.ValueReference is not Value propVal
                )
                    throw new InvalidDataException();
                properties[j] = new XElement(
                    "property",
                    [
                        new XAttribute("memberId", propId.MemberId),
                        new XAttribute("nameHash", propId.NameHash),
                        new XAttribute("valueTable", p.ValueTable),
                        new XAttribute(
                            "baseType",
                            NodeMetadata.VfxBaseTypeNames[propVal.ValueType]
                        ),
                        new XAttribute("valueIndex", propVal.ValueIndex),
                    ]
                );
            }
            for (int j = 0; j < inPorts.Length; j++)
            {
                if (graphNodeData.InPorts(j) is not GraphNodePort port)
                    throw new InvalidDataException();

                if (port.Identity is not GraphNodeMemberBase portId)
                    throw new InvalidDataException();

                inPorts[j] = new XElement(
                    "port",
                    [
                        new XAttribute("memberId", portId.MemberId),
                        new XAttribute("nameHash", portId.NameHash),
                        new XAttribute("argumentTypes", port.ArgumentTypes),
                    ]
                );
            }
            for (int j = 0; j < outPorts.Length; j++)
            {
                if (graphNodeData.OutPorts(j) is not GraphNodePort port)
                    throw new InvalidDataException();

                if (port.Identity is not GraphNodeMemberBase portId)
                    throw new InvalidDataException();

                outPorts[j] = new XElement(
                    "port",
                    [
                        new XAttribute("memberId", portId.MemberId),
                        new XAttribute("nameHash", portId.NameHash),
                        new XAttribute("argumentTypes", port.ArgumentTypes),
                    ]
                );
            }

            nodesXml[i] = new XElement(
                "node",
                [
                    new XAttribute("instanceId", identity.InstanceId),
                    new XAttribute("type", name),
                    new XAttribute("typeId", identity.TypeId),
                    new XElement("properties", properties),
                    new XElement("inPorts", inPorts),
                    new XElement("outPorts", outPorts),
                ]
            );
        }

        for (int i = 0; i < traysXml.Length; i++)
        {
            if (data.Trays(i) is not GraphNodeTray tray)
                throw new InvalidDataException();
            string? name = null;
            if (!NodeMetadata.NodeType.TryGetValue((int)tray.TypeId, out name))
                name = $"Node_{tray.TypeId:X}";

            XElement[] references = new XElement[tray.ReferencesLength];
            for (int j = 0; j < references.Length; j++)
                references[j] = new XElement(
                    "reference",
                    new XAttribute("instanceId", tray.References(j))
                );
            traysXml[i] = new XElement(
                "tray",
                [
                    new XAttribute("instanceId", tray.InstanceId),
                    new XAttribute("type", name),
                    new XAttribute("typeId", tray.TypeId),
                    new XAttribute("references", references),
                ]
            );
        }

        for (int i = 0; i < signalLinksXml.Length; i++)
        {
            if (data.SignalLinks(i) is not GraphLinkData link)
                throw new InvalidDataException();
            signalLinksXml[i] = new XElement(
                "link",
                [
                    new XAttribute("instanceId", link.InstanceId),
                    new XAttribute("sourceNodeId", link.SourceNodeId),
                    new XAttribute("sourceMemberId", link.SourceMemberId),
                    new XAttribute("destinationNodeId", link.DestinationNodeId),
                    new XAttribute("destinationMemberId", link.DestinationMemberId),
                    new XAttribute("linkType", link.LinkType),
                    new XAttribute("unknown", link.Unknown),
                ]
            );
        }

        for (int i = 0; i < propertyLinksXml.Length; i++)
        {
            if (data.PropertyLinks(i) is not GraphLinkData link)
                throw new InvalidDataException();
            propertyLinksXml[i] = new XElement(
                "link",
                [
                    new XAttribute("instanceId", link.InstanceId),
                    new XAttribute("sourceNodeId", link.SourceNodeId),
                    new XAttribute("sourceMemberId", link.SourceMemberId),
                    new XAttribute("destinationNodeId", link.DestinationNodeId),
                    new XAttribute("destinationMemberId", link.DestinationMemberId),
                    new XAttribute("linkType", link.LinkType),
                    new XAttribute("unknown", link.Unknown),
                ]
            );
        }

        return new XElement(
            "graphContainer",
            [
                new XElement("nodes", nodesXml),
                new XElement("trays", traysXml),
                new XElement("signalLinks", signalLinksXml),
                new XElement("propertyLinks", propertyLinksXml),
            ]
        );
    }

    private static XElement baseTypeDataToXml(GraphBaseTypeData data)
    {
        XElement[] byteDataXml = new XElement[data.ByteDataLength];
        XElement[] boolDataXml = new XElement[data.BoolDataLength];
        XElement[] int16DataXml = new XElement[data.Int16DataLength];
        XElement[] int32DataXml = new XElement[data.Int32DataLength];
        XElement[] int64DataXml = new XElement[data.Int64DataLength];
        XElement[] ubyteDataXml = new XElement[data.UbyteDataLength];
        XElement[] uInt16DataXml = new XElement[data.Uint16DataLength];
        XElement[] uInt32DataXml = new XElement[data.Uint32DataLength];
        XElement[] uInt64DataXml = new XElement[data.Uint64DataLength];
        XElement[] floatDataXml = new XElement[data.FloatDataLength];
        XElement[] doubleDataXml = new XElement[data.DoubleDataLength];
        XElement[] stringDataXml = new XElement[data.StringDataLength];
        XElement[] enumDataXml = new XElement[data.EnumDataLength];
        XElement[] vector2DataXml = new XElement[data.Vector2DataLength];
        XElement[] vector3DataXml = new XElement[data.Vector3DataLength];
        XElement[] vector4DataXml = new XElement[data.Vector4DataLength];
        XElement[] colorDataXml = new XElement[data.ColorDataLength];
        XElement[] matrix33DataXml = new XElement[data.Matrix33DataLength];
        XElement[] matrix34DataXml = new XElement[data.Matrix34DataLength];
        XElement[] matrix44DataXml = new XElement[data.Matrix44DataLength];
        XElement[] quaternionDataXml = new XElement[data.QuaternionDataLength];
        XElement[] eulerDataXml = new XElement[data.EulerDataLength];
        XElement[] int32ArrayDataXml = new XElement[data.Int32ArrayDataLength];
        XElement[] floatArrayDataXml = new XElement[data.FloatArrayDataLength];

        for (int i = 0; i < byteDataXml.Length; i++)
            byteDataXml[i] = new XElement("value", [new XAttribute("index", i), data.ByteData(i)]);

        for (int i = 0; i < boolDataXml.Length; i++)
            boolDataXml[i] = new XElement("value", [new XAttribute("index", i), data.BoolData(i)]);
        for (int i = 0; i < int16DataXml.Length; i++)
            int16DataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.Int16Data(i)]
            );
        for (int i = 0; i < int32DataXml.Length; i++)
            int32DataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.Int32Data(i)]
            );
        for (int i = 0; i < int64DataXml.Length; i++)
            int64DataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.Int64Data(i)]
            );
        for (int i = 0; i < ubyteDataXml.Length; i++)
            ubyteDataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.UbyteData(i)]
            );
        for (int i = 0; i < uInt16DataXml.Length; i++)
            uInt16DataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.Uint16Data(i)]
            );
        for (int i = 0; i < uInt32DataXml.Length; i++)
            uInt32DataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.Uint32Data(i)]
            );
        for (int i = 0; i < uInt64DataXml.Length; i++)
            uInt64DataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.Uint64Data(i)]
            );
        for (int i = 0; i < floatDataXml.Length; i++)
            floatDataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.FloatData(i)]
            );
        for (int i = 0; i < doubleDataXml.Length; i++)
            doubleDataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.DoubleData(i)]
            );
        for (int i = 0; i < stringDataXml.Length; i++)
            stringDataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.StringData(i)]
            );
        for (int i = 0; i < enumDataXml.Length; i++)
            enumDataXml[i] = new XElement("value", [new XAttribute("index", i), data.EnumData(i)]);

        for (int i = 0; i < vector2DataXml.Length; i++)
        {
            if (data.Vector2Data(i) is not Vector4 v)
                throw new InvalidDataException();

            vector2DataXml[i] = new XElement("value", [new XAttribute("index", i), $"{v.X},{v.Y}"]);
        }

        for (int i = 0; i < vector3DataXml.Length; i++)
        {
            if (data.Vector3Data(i) is not Vector4 v)
                throw new InvalidDataException();

            vector3DataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), $"{v.X},{v.Y},{v.Z}"]
            );
        }

        for (int i = 0; i < vector4DataXml.Length; i++)
        {
            if (data.Vector4Data(i) is not Vector4 v)
                throw new InvalidDataException();

            vector4DataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), $"{v.X},{v.Y},{v.Z},{v.W}"]
            );
        }

        for (int i = 0; i < colorDataXml.Length; i++)
        {
            if (data.ColorData(i) is not Vector4 c)
                throw new InvalidDataException();
            colorDataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), $"{c.X},{c.Y},{c.Z},{c.W}"]
            );
        }

        for (int i = 0; i < matrix33DataXml.Length; i++)
        {
            if (data.Matrix33Data(i) is not Matrix33 m)
                throw new InvalidDataException();
            matrix33DataXml[i] = new XElement(
                "value",
                [
                    new XAttribute("index", i),
                    new XElement("row0", $"{m.Row0.X},{m.Row0.Y},{m.Row0.Z}"),
                    new XElement("row1", $"{m.Row1.X},{m.Row1.Y},{m.Row1.Z}"),
                    new XElement("row2", $"{m.Row2.X},{m.Row2.Y},{m.Row2.Z}"),
                ]
            );
        }

        for (int i = 0; i < matrix34DataXml.Length; i++)
        {
            if (data.Matrix34Data(i) is not Matrix34 m)
                throw new InvalidDataException();
            matrix34DataXml[i] = new XElement(
                "value",
                [
                    new XAttribute("index", i),
                    new XElement("row0", $"{m.Row0.X},{m.Row0.Y},{m.Row0.Z},{m.Row0.W}"),
                    new XElement("row1", $"{m.Row1.X},{m.Row1.Y},{m.Row1.Z},{m.Row1.W}"),
                    new XElement("row2", $"{m.Row2.X},{m.Row2.Y},{m.Row2.Z},{m.Row2.W}"),
                ]
            );
        }

        for (int i = 0; i < matrix44DataXml.Length; i++)
        {
            if (data.Matrix44Data(i) is not Matrix44 m)
                throw new InvalidDataException();
            matrix44DataXml[i] = new XElement(
                "value",
                [
                    new XAttribute("index", i),
                    new XElement("row0", $"{m.Row0.X},{m.Row0.Y},{m.Row0.Z},{m.Row0.W}"),
                    new XElement("row1", $"{m.Row1.X},{m.Row1.Y},{m.Row1.Z},{m.Row1.W}"),
                    new XElement("row2", $"{m.Row2.X},{m.Row2.Y},{m.Row2.Z},{m.Row2.W}"),
                    new XElement("row3", $"{m.Row3.X},{m.Row3.Y},{m.Row3.Z},{m.Row3.W}"),
                ]
            );
        }

        for (int i = 0; i < quaternionDataXml.Length; i++)
        {
            if (data.QuaternionData(i) is not Quaternion q)
                throw new InvalidDataException();
            quaternionDataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), $"{q.X},{q.Y},{q.Z},{q.W}"]
            );
        }

        for (int i = 0; i < eulerDataXml.Length; i++)
        {
            if (data.EulerData(i) is not Euler e)
                throw new InvalidDataException();
            eulerDataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), new XAttribute("order", e.Order), $"{e.X},{e.Y},{e.Z}"]
            );
        }

        for (int i = 0; i < int32ArrayDataXml.Length; i++)
        {
            if (data.Int32ArrayData(i) is not Int32ArrayTable t)
                throw new InvalidDataException();

            XElement[] values = new XElement[t.DataLength];
            for (int j = 0; j < values.Length; j++)
                values[j] = new XElement("value", t.Data(j));
            int32ArrayDataXml[i] = new XElement("int32Array", [new XAttribute("index", i), values]);
        }

        for (int i = 0; i < floatArrayDataXml.Length; i++)
        {
            if (data.FloatArrayData(i) is not FloatArrayTable t)
                throw new InvalidDataException();

            XElement[] values = new XElement[t.DataLength];
            for (int j = 0; j < values.Length; j++)
                values[j] = new XElement("value", t.Data(j));
            floatArrayDataXml[i] = new XElement("floatArray", [new XAttribute("index", i), values]);
        }

        // Bool = 1,
        // Int32 = 2,
        // Float32 = 3,
        // Vector2 = 4,
        // Vector3 = 5,
        // Vector4 = 6,
        // Matrix44 = 7,
        // String = 8,
        // Pointer = 9,
        // Int32Array = 10,
        // FCurve = 11,
        // FloatArray = 12,
        // Vector4Array = 15,
        // VectorData = 16
        return new XElement(
            "baseTypeData",
            [
                new XElement("byteData", [byteDataXml]),
                // new XElement("boolData", [boolDataXml]),
                // new XElement("int16Data", [int16DataXml]),
                new XElement("int32Data", [int32DataXml]),
                // new XElement("int64Data", [int64DataXml]),
                // new XElement("ubyteData", [ubyteDataXml]),
                // new XElement("uint16Data", [uInt16DataXml]),
                // new XElement("uint32Data", [uInt32DataXml]),
                // new XElement("uint64Data", [uInt64DataXml]),
                new XElement("floatData", [floatDataXml]),
                // new XElement("doubleData", [doubleDataXml]),
                new XElement("stringData", [stringDataXml]),
                new XElement("enumData", [enumDataXml]),
                new XElement("vector2Data", [vector2DataXml]),
                new XElement("vector3Data", [vector3DataXml]),
                new XElement("vector4Data", [vector4DataXml]),
                // new XElement("colorData", [colorDataXml]),
                // new XElement("matrix33Data", [matrix33DataXml]),
                // new XElement("matrix34Data", [matrix34DataXml]),
                new XElement("matrix44Data", [matrix44DataXml]),
                // new XElement("quaternionData", [quaternionDataXml]),
                // new XElement("eulerData", [eulerDataXml]),
                // new XElement("byteData"),
                // new XElement("byteData"),
                // new XElement("byteData"),
                new XElement("int32ArrayData", int32ArrayDataXml),
                // new XElement("byteData"),
                // new XElement("byteData"),
                // new XElement("byteData"),
                // new XElement("byteData"),
                new XElement("floatArrayData", floatArrayDataXml),
                // new XElement("byteData"),
                // new XElement("byteData"),
                // new XElement("byteData"),
                // new XElement("byteData"),
                // new XElement("byteData"),
                // new XElement("byteData"),
                // new XElement("byteData"),
                // new XElement("byteData"),
                // new XElement("byteData"),
                // new XElement("byteData"),
                // new XElement("byteData"),
            ]
        );
    }

    private static XElement vfxTypeDataToXml(VfxTypeData data)
    {
        if (data.BoxDataLength > 0)
            throw new NotImplementedException(
                "Vfx Box serialization not implemented. Bother me about it"
            );
        if (data.SphereDataLength > 0)
            throw new NotImplementedException(
                "Vfx Sphere serialization not implemented. Bother me about it"
            );
        if (data.PointerDataLength > 0)
            throw new NotImplementedException(
                "Vfx Pointer serialization not implemented. Bother me about it"
            );
        if (data.VectorDataLength > 0)
            throw new NotImplementedException(
                "Vfx Vector serialization not implemented. Bother me about it"
            );
        if (data.OffsetDataLength > 0)
            throw new NotImplementedException(
                "Vfx Offset serialization not implemented. Bother me about it"
            );
        if (data.OffsetArrayDataLength > 0)
            throw new NotImplementedException(
                "Vfx Offset Array serialization not implemented. Bother me about it"
            );

        XElement[] fCurvesXml = new XElement[data.FCurveDataLength];
        for (int i = 0; i < fCurvesXml.Length; i++)
        {
            if (data.FCurveData(i) is not VfxFCurveData curveTable)
                throw new InvalidDataException();

            XElement[] anchorPoints = new XElement[curveTable.AnchorPointsLength];
            for (int j = 0; j < anchorPoints.Length; j++)
            {
                if (curveTable.AnchorPoints(j) is not FCurveAnchorPoint anchorPoint)
                    throw new InvalidDataException();

                anchorPoints[j] = new XElement(
                    "anchorPoint",
                    [
                        new XAttribute("time", $"{anchorPoint.Time}"),
                        $"{anchorPoint.X},{anchorPoint.Y},{anchorPoint.Z}",
                    ]
                );
            }

            fCurvesXml[i] = new XElement(
                "fCurve",
                [
                    new XAttribute("loopIn", curveTable.LoopIn),
                    new XAttribute("loopOut", curveTable.LoopOut),
                    new XAttribute("loopType", curveTable.CurveLoopType),
                    new XElement("anchorPoints", anchorPoints),
                ]
            );
        }
        return new XElement("vfxTypeData", [new XElement("fCurves", fCurvesXml)]);
    }

    public static XElement ToXml(VfxGraphBinary data, string name)
    {
        if (data.GraphBinary is not GraphBinary graphBinary)
            throw new InvalidDataException();

        if (graphBinary.GraphContainer is not GraphContainer graphContainer)
            throw new InvalidDataException();

        XElement graphContainerXml = graphContainerToXml(graphContainer);

        if (graphBinary.BaseTypeData is not GraphBaseTypeData baseTypeData)
            throw new InvalidDataException();
        XElement baseTypeDataXml = baseTypeDataToXml(baseTypeData);

        if (data.VfxTypeData is not VfxTypeData vfxTypeData)
            throw new InvalidDataException();

        XElement vfxTypeDataXml = vfxTypeDataToXml(vfxTypeData);

        return new XElement(
            "vfxGraph",
            [
                new XAttribute("name", name),
                new XElement("graph", [graphContainerXml, baseTypeDataXml]),
                vfxTypeDataXml,
            ]
        );
    }
}

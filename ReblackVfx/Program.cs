using System.CommandLine;
using System.Xml.Linq;
using Google.FlatBuffers;
using ReblackVfx;
using ReblackVfx.FlatBuffer.Graph;
using ReblackVfx.FlatBuffer.VfxGraph;

internal static class Program
{
    public static void ToBinary(XElement root, FlatBufferBuilder builder)
    {
        if ((string?)root.Attribute("name") is string s)
        {
            Console.WriteLine($"Attempting to serialize VFX Graph {s}...");
        }

        // INFO: Output is built backwards
        // INFO: Step 1: Version info struct
        // INFO: Step 2: Graph Header Table
        GraphHeader.StartGraphHeader(builder);
        GraphHeader.AddVersion(builder, GraphVersionInfo.CreateGraphVersionInfo(builder, 1, 2, 3));
        var headerOffset = GraphHeader.EndGraphHeader(builder);

        // INFO: Step 3: Base Type Data
        if (root.Element("baseTypeData") is not XElement baseTypeDataXml)
            throw new InvalidDataException();

        // INFO: Step 4: Vfx Type Data

        // INFO: Step 5: Base Type Table
        var baseTypeDataOffset = VfxConverter.SerializeBaseTypeData(builder, baseTypeDataXml);
        // INFO: Step 6: Vfx Type Table
        var vfxTypeDataOffset = VfxTypeData.CreateVfxTypeData(
            builder,
            new VectorOffset(0), // Box
            new VectorOffset(0), // Sphere
            new VectorOffset(0), // Vector
            new VectorOffset(0), // Pointer
            new VectorOffset(0), // Offset
            new VectorOffset(0), // Offset Array
            new VectorOffset(0) // FCurve
        );

        // INFO: Step 7: Nodes
        // INFO: Step 8: Trays
        // INFO: Step 9: Property links
        // INFO: Step 10: Signal links

        // INFO: Step 11: Graph Container Table

        var graphContainerOffset = GraphContainer.CreateGraphContainer(
            builder,
            new VectorOffset(0), // Nodes
            new VectorOffset(0), // Trays
            new VectorOffset(0), // Signal Links
            new VectorOffset(0) // Property Links
        );

        // INFO: Step 12: Graph Binary Table
        GraphBinary.StartGraphBinary(builder);
        GraphBinary.AddBaseTypeData(builder, baseTypeDataOffset);
        GraphBinary.AddGraphContainer(builder, graphContainerOffset);
        GraphBinary.AddGraphHeader(builder, headerOffset);
        var graphBinaryOffset = GraphBinary.EndGraphBinary(builder);

        // INFO: Step 13: Root Table
        VfxGraphBinary.StartVfxGraphBinary(builder);
        VfxGraphBinary.AddVfxTypeData(builder, vfxTypeDataOffset);
        VfxGraphBinary.AddGraphBinary(builder, graphBinaryOffset);
        var rootOffset = VfxGraphBinary.EndVfxGraphBinary(builder);
    }

    public static int Main(string[] args)
    {
        Option<FileInfo> inFileOption = new("--in", "-i")
        {
            Description = "Input file path",
            Required = true,
            Recursive = true,
        };
        Option<FileInfo> outFileOption = new("--out", "-o")
        {
            Description = "Output file path",
            Required = false,
            Recursive = true,
        };

        RootCommand rootCommand = new("Final Fantasy XV VFX Graph Converter");
        rootCommand.Options.Add(inFileOption);
        rootCommand.Options.Add(outFileOption);

        Command vfxToXmlCommand = new("vfx-xml", "Convert .vfx to XML");
        Command xmlToVfxCommand = new("xml-vfx", "Convert XML to .vfx");

        rootCommand.Subcommands.Add(vfxToXmlCommand);
        rootCommand.Subcommands.Add(xmlToVfxCommand);

        vfxToXmlCommand.SetAction(parseResult =>
        {
            var input = parseResult.GetValue(inFileOption);
            if (input is not FileInfo file)
                throw new Exception();
            Console.WriteLine($"Converting {input.Name} to .xml...");

            string name;
            if (parseResult.GetValue(outFileOption) is FileInfo output)
                name = output.FullName;
            else
                name = $"{Path.GetFileNameWithoutExtension(input.Name)}.xml";

            byte[] data;
            using (var stream = new FileStream(input.FullName, FileMode.Open))
            {
                data = new byte[stream.Length];
                stream.ReadExactly(data);
            }
            ByteBuffer buffer = new ByteBuffer(data, 16);
            VfxGraphBinary vfxData = VfxGraphBinary.GetRootAsVfxGraphBinary(buffer);

            XElement root = VfxConverter.ToXml(vfxData, Path.GetFileNameWithoutExtension(name));

            root.Save(name);
        });

        xmlToVfxCommand.SetAction(parseResult =>
        {
            var input = parseResult.GetValue(inFileOption);
            if (input is not FileInfo file)
                throw new Exception();
            Console.WriteLine($"Converting {input.Name} to .vfx...");
            string name;
            if (parseResult.GetValue(outFileOption) is FileInfo output)
                name = output.FullName;
            else
                name = $"{Path.GetFileNameWithoutExtension(input.Name)}.xml";
            FlatBufferBuilder builder = new FlatBufferBuilder(1024);
            XElement root = XElement.Load(input.FullName);
            ToBinary(root, builder);

            using (var stream = new FileStream(name, FileMode.Create))
            {
                byte[] array = builder.SizedByteArray();
                stream.Write(array, 0, array.Length);
            }
        });

        rootCommand.Parse(args).Invoke();
        return 0;
    }
}

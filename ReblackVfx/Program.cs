using System.CommandLine;
using System.Globalization;
using System.Xml.Linq;
using Google.FlatBuffers;
using ReblackVfx;
using ReblackVfx.FlatBuffer.VfxGraph;

public static class Program
{
    public static void Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("us-EN");
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
        Command testCommand = new("test", "Run flatbuffer check on all .vfx files");

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
                name = $"{Path.GetFileNameWithoutExtension(input.Name)}.vfx";
            FlatBufferBuilder builder = new FlatBufferBuilder(1024);
            XElement root = XElement.Load(input.FullName);
            VfxConverter.CreateVfxBinary(builder, root);

            using (var stream = new FileStream(name, FileMode.Create))
            {
                byte[] array = builder.SizedByteArray();
                stream.Write(array, 0, array.Length);
            }
        });

        rootCommand.Parse(args).Invoke();
    }
}

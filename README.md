# Final Fantasy XV VFX Tool
A simple converter to assist with modding Final Fantasy XV VFX (.vfx) files 

> [!NOTE]
> The version of Flatbuffers used in this repository is modified to disable
vtable trimming and append the 16 byte VFX header.

## Usage

Get the latest version in
[**Releases**](https://github.com/AlexPlaceres/ff15-vfx/releases).


### Convert VFX Flatbuffer to XML
```
ReblackVfx vfx-xml -i <path_to_vfx_file> [-o <path_to_output_file>]
```

### Generate VFX Flatbuffer from XML
```
ReblackVfx xml-vfx -i <path_to_xml_file> [-o <path_to_output_file>]
```

> [!WARNING]
> At the moment, VFX-specific types other than curves are unsupported.
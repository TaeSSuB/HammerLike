# ProPixelizer Subtarget for ShaderGraph

## What is the ProPixelizer Subtarget?

ShaderGraph is a graph-based shader editor, which allows you to create shaders for Unity by assembling graphs from nodes.
Since version 1.0 of ProPixelizer, I've used ShaderGraph to define the look and feel of the ProPixelizer shader, including
cel-shading, lighting, and dither patterns. The intention behind this has always been to make ProPixelizer as easily modifiable
as possible, so that people can tweak it to get whatever look they like.

However, ShaderGraph has a fairly big limitation - it is incapable of adding Custom Passes. This has been an often requested
feature, see for example this forum post:
https://forum.unity.com/threads/how-to-add-a-pass-tag-to-a-shadergraph.865594/

A custom pass is required to generate ID-based outline information in ProPixelizer. In previous versions of ProPixelizer, I
added this pass to the ShaderGraph shader by creating a second shader, PixelizedWithOutline, and used UsePass to combine
the passes from the ShaderGraph shader with the ProPixelizer pass. It works, but is messy, and made editing the graphs a nuisance.

Starting from v1.9 of ProPixelizer, I have added a way for advanced users to directly create all required passes in a single ShaderGraph shader.
This works by creating a 'SubTarget' for shadergraph. Sadly, **the ShaderGraph API is mostly sealed and private**, so
a (very minor!) modification is required before we can use it. I am supporting this feature for 2022 LTS, and until a better solution is available.
In an ideal world, Unity would open the API up; it rather defeats the purpose of having the API flexible and 'scriptable' to then hide it.

## Installation (Automated)

Open `Windows > ProPixelizer > Advanced Options`. Follow the instructions in the `ShaderGraph SubTarget` group, and click `Enable` to embed and install the packages.
(You may need to click Enable a few times to tick every box - Unity Package Manager will ocassionally interrupt and stop the execution.)

In `Project Settings -> Player -> Other Settings -> Scripting Define Symbols`, add a define for `PROPIXELIZER_SHADERGRAPH`.

## Installation (Manual)

1. You must first 'embed' the Universal Render Pipeline and ShaderGraph packages, so that we can expose their innards.
   Instructions are found here:
   https://docs.unity3d.com/Manual/upm-embed.html
   'Embedding' is Unity slang for copying the Packages from the package cache (often `Library/Packages`) to a folder called `/Packages` in your project directory.
   The easiest way to locate the packages in the cache is to right click them in the project explorer and click 'Show in Explorer'. 

   (Note that if you want to update these packages in the future, you will need to replace the folders in /Packages.)

2. Add ProPixelizer to your project (if you haven't already). Make sure you are on the `CustomSG` branch.

3. Add the following line to the AssemblyInfo.cs files at these locations:
   (i) Project Folder/Packages/com.unity.render-pipelines.universal@version/Editor/AssemblyInfo.cs
   (ii) Project Folder/Packages/com.unity.render-pipelines.universal@version/Runtime/AssemblyInfo.cs
   (iii) Project Folder/Packages/com.unity.shadergraph@version/Editor/AssemblyInfo.cs

   The line to add is:
       `[assembly: InternalsVisibleTo("ProPixelizer")]`

4. In `Project Settings -> Player -> Other Settings -> Scripting Define Symbols`, add a define for `PROPIXELIZER_SHADERGRAPH`.
   (Note that these definitions are per-platform, so you need to add them for each platform).

## Using the SubTarget (Example)

1. Copy the ProPixelizer/ShaderGraph/ProPixelizerBase ShaderGraph, and rename the file ProPixelizerUber and the shader name (in the ShaderGraph blackboard) to ProPixelizerUber.

2. In the Graph Inspector, under the Universal foldout, click where it says 'Material - Lit' and change from 'Lit' to 'ProPixelizer'.

3. Change the Custom Editor GUI (in the ShaderGraph blackboard) from `ProPixelizer.ProPixelizerBaseGUI` to `ProPixelizer.PixelizedWithOutlineShaderGUI`.

4. (optional) On the blackboard, under the text 'ProPixelizerUber' where it says 'ProPixelizer/Hidden' in feint grey text, change 'ProPixelizer/Hidden' to 'ProPixelizer'.

Enjoy! You can now use ProPixelizerBase in your materials, and modify it to your heart's content, and all the passes required will be added.
# UnityShaderAnalyzer
Editor tool to compile unity's shader's into GLSL, split them into vertex and fragment methods for each variant, and run the Mali Offline Compiler to analyze the complexity of each method.

# Usage
This is a work in progress. The functionality of analyzing individual shader variants is there, but the editor tools are lacking, so its not very helpful yet. I'm planning on writing a custom editor for this when I get some more time. 

In the meantime, you can use this by building the project. That will generate a ShaderViriants-(timestamp) asset in the root of the project, containing a list of all of the shader variants processed at build time, including for other platforms and hardware tiers. There are a couple of buttons to trigger Mali Offline Compiler to generate the analysis of the shader complexity either as JSON or text format. It will process for 15+ seconds, and after that you can browse through the list of shader variants and browse the generated analyses. 

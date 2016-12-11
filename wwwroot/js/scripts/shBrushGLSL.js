/**
 * SyntaxHighlighter
 * http://alexgorbatchev.com/SyntaxHighlighter
 *
 * SyntaxHighlighter is donationware. If you are using it, please donate.
 * http://alexgorbatchev.com/SyntaxHighlighter/donate.html
 *
 * @version
 * 3.0.83 (July 02 2010)
 * 
 * @copyright
 * Copyright (C) 2004-2010 Alex Gorbatchev.
 *
 * @license
 * Dual licensed under the MIT and GPL licenses.
 */
;(function()
{
	// CommonJS
	typeof(require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;

	function Brush()
	{
		// Copyright 2014 Tigran G. Saluev
	
		var datatypes =	'int uint bool float double vec2 vec3 vec4 mat2 mat3 mat4 dmat2 dmat3 dmat4' +
                                                'mat2x3 mat2x4 mat3x2 mat3x4 mat4x2 mat4x3 '+
                                                'dmat2x3 dmat2x4 dmat3x2 dmat3x4 dmat4x2 dmat4x3 '+
                                                'sampler1D sampler2D sampler3D samplerCube sampler2DRect ' +
                                                'sampler1DArray sampler2DArray samplerCubeArray samplerBuffer ' +
                                                'sampler2DMS sampler2DMSArray sampler1DShadow sampler2DShadow ' +
                                                'samplerCubeShadow sampler2DRectShadow sampler1DArrayShadow ' +
                                                'sampler2DArrayShadow samplerCubeArrayShadow ' +
                                                'ivec2 ivec3 ivec4 uvec2 uvec3 uvec4 bvec2 bvec3 bvec4 ' +
                                                'gl_DepthRangeParameters gl_LightSourceParameters ';
                
		var keywords =	'break case const continue else enum if for inline return  ' +
						'sizeof struct switch true false typedef union void while ' +
                                                'uniform varying in out layout atomic location binding';
                
                var builtins =  'gl_Vertex gl_Position gl_Color gl_Material ' +
                                                'gl_MultiTexCoord gl_TexCoord gl_Normal gl_FragColor gl_FragDepth ' +
                                                'gl_LightSource gl_VertexID gl_InstanceID gl_PointSize gl_ClipDistance ' +
                                                'gl_PatchVerticesIn gl_PrimitiveID gl_InvocationID gl_in gl_TessLevelOuter ' +
                                                'gl_TessLevelInner gl_out gl_TessCoord gl_PrimitiveIDIn gl_Layer gl_ViewportIndex ' +
                                                'gl_FragCoord gl_FrontFacing gl_PointCoord gl_SampleID gl_SamplePosition ' +
                                                'gl_SampleMaskIn gl_DepthRange gl_SampleMask gl_NumWorkGroups gl_WorkGroupID ' +
                                                'gl_LocalInvocationID gl_GlobalInvocationID gl_LocalInvocationIndex gl_WorkGroupSize ';
		
		var functions =	'radians degrees sin cos tan asin acos atan pow exp log exp2 log2 sqrt inversesqrt ' +
                                                'abs sign floor ceil fract mod min max clamp mix step smoothstep ' +
                                                'length distance dot cross normalize faceforward reflect refract ' +
                                                'matrixCompMult lessThan lessThanEqual greaterThan greaterThanEqual ' +
                                                'equal notEqual any all not texture1D texture2D texture3D textureCube ' +
                                                'texture textureSize textureOffset textureProj textureLod textureGrad ' +
                                                'textureProjOffset textureProjLod textureProjLodOffset textureProjGrad ' +
                                                'textureProjGradOffset textureLodOffset textureGradOffset texelFetch ' +
                                                'texelFetchOffset EmitVertex EndPrimitive ';

		this.regexList = [
			{ regex: SyntaxHighlighter.regexLib.singleLineCComments,	css: 'comments' },			// one line comments
			{ regex: SyntaxHighlighter.regexLib.multiLineCComments,		css: 'comments' },			// multiline comments
			{ regex: SyntaxHighlighter.regexLib.doubleQuotedString,		css: 'string' },			// strings
			{ regex: SyntaxHighlighter.regexLib.singleQuotedString,		css: 'string' },			// strings
			{ regex: /^ *#.*/gm,										css: 'preprocessor' },
			{ regex: /\d+(\.\d+)?/gm,									css: 'topcomment' },
			{ regex: new RegExp(this.getKeywords(datatypes), 'gm'),		css: 'color1 bold' },
			{ regex: new RegExp(this.getKeywords(functions), 'gm'),		css: 'functions bold' },
                        { regex: new RegExp(this.getKeywords(builtins), 'gm'),          css: 'builtins bold' },
			{ regex: new RegExp(this.getKeywords(keywords), 'gm'),		css: 'keyword bold' }
			];
	};

	Brush.prototype	= new SyntaxHighlighter.Highlighter();
	Brush.aliases	= ['glsl'];

	SyntaxHighlighter.brushes.GLSL = Brush;

	// CommonJS
	typeof(exports) != 'undefined' ? exports.Brush = Brush : null;
})();

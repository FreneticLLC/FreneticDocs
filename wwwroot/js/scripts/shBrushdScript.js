// MCMONKEY - All
;(function()
{
	// CommonJS
	typeof(require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;

	function Brush()
	{
		var keywords =	'if else';

		function fixComments(match, regexInfo)
		{
			var css = (match[0].indexOf("///") == 0)
				? 'color1'
				: 'comments'
				;
			
			return [new SyntaxHighlighter.Match(match[0], match.index, css)];
		}

		this.regexList = [
			{ regex: SyntaxHighlighter.regexLib.doubleQuotedString,		css: 'string2' },
			{ regex: SyntaxHighlighter.regexLib.singleQuotedString,		css: 'string2' },
			{ regex: /^.*#\s*-.*/gm,									css: 'topcomment' },
			{ regex: /\&lt\;(.*)\&gt\;/gm,								css: 'variable3' },
			{ regex: /\%.*\%/gm,										css: 'variable' },
			{ regex: /\[.*\]/gm,										css: 'variable2' },
			{ regex: /^[^\-\#]*:.*$/gm,									css: 'section' },
			{ regex: /^.*#\s*[\|\+\=].*/gm,								css: 'speccomment' },
			{ regex: /^.*#.*/gm,										css: 'comments' },
			{ regex: /^\s*-\s[^\s]+/gm,									css: 'italic' },
			{ regex: new RegExp(this.getKeywords(keywords), 'gm'),		css: 'italic' }
			];
		
		this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
	};

	Brush.prototype	= new SyntaxHighlighter.Highlighter();
	Brush.aliases	= ['ds', 'dscript'];

	SyntaxHighlighter.brushes.dScript = Brush;

	// CommonJS
	typeof(exports) != 'undefined' ? exports.Brush = Brush : null;
})();


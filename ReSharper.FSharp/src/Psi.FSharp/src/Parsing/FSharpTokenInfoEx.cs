using JetBrains.ReSharper.Psi.Parsing;
using Microsoft.FSharp.Compiler.SourceCodeServices;

namespace JetBrains.ReSharper.Psi.FSharp.Parsing
{
  public class FSharpTokenInfoEx
  {
    public static TokenNodeType GetTokenType(FSharpTokenInfo token)
    {
      if (token.ColorClass == FSharpTokenColorKind.InactiveCode) return FSharpTokenType.DEAD_CODE;
      if (token.ColorClass == FSharpTokenColorKind.PreprocessorKeyword) return FSharpTokenType.KEYWORD;

      switch (token.CharClass)
      {
        case FSharpTokenCharKind.Keyword:
          return FSharpTokenType.KEYWORD;

        case FSharpTokenCharKind.Identifier:
          return FSharpTokenType.IDENTIFIER;

        case FSharpTokenCharKind.String:
          return FSharpTokenType.STRING;

        case FSharpTokenCharKind.Literal:
          return FSharpTokenType.LITERAL;

        case FSharpTokenCharKind.Operator:
          return FSharpTokenType.OPERATOR;

        case FSharpTokenCharKind.LineComment:
        case FSharpTokenCharKind.Comment:
          return FSharpTokenType.COMMENT;

        case FSharpTokenCharKind.WhiteSpace:
          return FSharpTokenType.WHITESPACE;

        case FSharpTokenCharKind.Delimiter:
          if (token.Tag == FSharpTokenTag.RPAREN)
            return FSharpTokenType.RPAREN;
          if (token.Tag == FSharpTokenTag.LPAREN)
            return FSharpTokenType.LPAREN;
          if (token.Tag == FSharpTokenTag.DOT)
            return FSharpTokenType.DOT;
          return FSharpTokenType.TEXT;

        default:
          return FSharpTokenType.TEXT;
      }
    }
  }
}
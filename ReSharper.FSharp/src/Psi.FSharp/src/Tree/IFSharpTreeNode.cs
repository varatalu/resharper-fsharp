﻿using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Psi.FSharp.Tree
{
  public interface IFSharpTreeNode : ITreeNode
  {
    void Accept (TreeNodeVisitor visitor);
    void Accept<TContext> (TreeNodeVisitor<TContext> visitor, TContext context);
    TReturn Accept<TContext, TReturn> (TreeNodeVisitor<TContext, TReturn> visitor, TContext context);
  }
}
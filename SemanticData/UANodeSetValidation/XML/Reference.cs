﻿//___________________________________________________________________________________
//
//  Copyright (C) 2021, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using System;
using UAOOI.SemanticData.UANodeSetValidation.DataSerialization;

namespace UAOOI.SemanticData.UANodeSetValidation.XML
{
  public partial class Reference : IEquatable<Reference>
  {
    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
    public bool Equals(Reference other)
    {
      return this.ToString().CompareTo(other.ToString()) == 0;
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString()
    {
      return $"{this.IsForward}, {this.ReferenceType}, {this.Value}";
    }

    internal void RecalculateNodeIds(Func<string, NodeId> importNodeId)
    {
      ReferenceTypeNodeid = importNodeId(ReferenceType);
      ValueNodeId = importNodeId(Value);
    }

    internal NodeId ReferenceTypeNodeid { get; private set; }
    internal NodeId ValueNodeId { get; private set; }
  }
}
﻿//__________________________________________________________________________________________________
//
//  Copyright (C) 2022, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GitHub: https://github.com/mpostol/OPC-UA-OOI/discussions
//__________________________________________________________________________________________________

namespace UAOOI.SemanticData.AddressSpace.Abstractions
{
  /// <summary>
  /// Interface IUAType - instances implementing this interface supports type definition factoring.
  /// </summary>
  public interface IUAType : IUANode
  {
    /// <summary>
    /// Sets a value indicating whether this instance is abstract.
    /// A boolean Attribute with the following values:
    ///   TRUE it is an abstract DataType.
    ///   FALSE it is not an abstract DataType.
    /// </summary>
    /// <remarks>Default Value is false</remarks>
    /// <value><c>true</c> if this instance is abstract; otherwise, <c>false</c>.</value>
    bool IsAbstract { get; set; }
  }
}
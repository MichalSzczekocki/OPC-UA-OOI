﻿//___________________________________________________________________________________
//
//  Copyright (C) 2021, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using UAOOI.SemanticData.InformationModelFactory;

namespace UAOOI.SemanticData.UANodeSetValidation.ModelFactoryTestingFixture
{
  /// <summary>
  /// TODO Class ViewInstanceFactoryBase.
  /// </summary>
  internal class ViewInstanceFactoryBase : InstanceFactoryBase, IViewInstanceFactory
  {
    /// <summary>
    /// Sets a value indicating whether the View NodeClass supports events.
    /// </summary>
    /// <value><c>null</c> if supports events contains no value, <c>true</c> if the view supports events; otherwise, <c>false</c>. Default value is <c>false</c></value>
    public bool? SupportsEvents
    {
      set { }
    }

    /// <summary>
    /// Sets a value indicating whether the Address Space represented by the View NodeClass contains no loops.
    /// </summary>
    /// <value><c>true</c> if the partial Address Space contains no loops; otherwise, <c>false</c>. Default value is <c>false</c></value>
    public bool ContainsNoLoops
    {
      set { }
    }
  }
}
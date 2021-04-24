﻿//__________________________________________________________________________________________________
//
//  Copyright (C) 2021, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GitHub: https://github.com/mpostol/OPC-UA-OOI/discussions
//__________________________________________________________________________________________________

using System;
using System.Xml;
using System.Xml.Serialization;

namespace UAOOI.DataDiscovery.DiscoveryServices.Models
{
  /// <summary>
  /// Class DomainModel - domain description holder.
  /// </summary>
  /// <remarks>
  /// Domain is a collection of data over which an owner has control. It may be used to describe:
  /// * a collection of package addresses used to push the message to the receiver.
  /// * a collection of data used to provide data semantic unique identifier and support subscription to receive copies of the data as the message payload based on the data semantics.
  /// </remarks>
  public partial class DomainModel
  {
    #region API

    /// <summary>
    /// Gets or sets the URI of the domain.
    /// </summary>
    /// <value>The URI.</value>
    [XmlIgnore]
    public Uri DomainModelUri
    {
      get => new Uri(DomainModelUriString);
      set => DomainModelUriString = value.ToString();
    }

    /// <summary>
    /// Gets or sets the unique name of the domain.
    /// </summary>
    /// <value>The name of the unique.</value>
    [XmlIgnore]
    public Guid DomainModelGuid
    {
      get => XmlConvert.ToGuid(DomainModelGuidString);
      set => DomainModelGuidString = XmlConvert.ToString(value);
    }

    /// <summary>
    /// Gets or sets the universal discovery service locator - this URL (REST call is assigned by the resolver).
    /// </summary>
    /// <value>The universal discovery service locator.</value>
    [XmlIgnore]
    public string UniversalDiscoveryServiceLocator { get; set; }

    #endregion API
  }
}
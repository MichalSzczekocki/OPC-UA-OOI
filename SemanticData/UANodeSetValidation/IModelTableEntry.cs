﻿//___________________________________________________________________________________
//
//  Copyright (C) 2021, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using System;
using UAOOI.SemanticData.UANodeSetValidation.XML;

namespace UAOOI.SemanticData.UANodeSetValidation
{
  /// <summary>
  /// Interface IModelTableEntry - instance of this interface is defined in the <see cref="UANodeSet"/>
  /// along with any dependencies these models have.
  /// </summary>
  public interface IModelTableEntry
  {
    /// <summary>
    /// Gets or sets the access restrictions. The default <c>AccessRestrictions</c> that apply to all <c>Nodes</c> in the model.
    /// </summary>
    /// <value>The access restrictions.</value>
    byte AccessRestrictions { get; }

    /// <summary>
    /// Gets or sets the model URI. The URI for the model. This URI should be one of the entries in the <see cref="NamespaceTable"/> table.
    /// </summary>
    /// <value>The model URI.</value>
    string ModelUri { get; }

    /// <summary>
    /// Gets or sets the publication date. When the model was published. This value is used for comparisons if the model is defined in multiple UANodeSet files.
    /// </summary>
    /// <value>The publication date.</value>
    DateTime? PublicationDate { get; }

    /// <summary>
    /// Gets or sets the required model. A list of dependencies for the model. If the model requires a minimum version the PublicationDate shall be specified.
    /// Tools which attempt to resolve these dependencies may accept any PublicationDate after this date.
    /// </summary>
    /// <value>The required model.</value>
    IModelTableEntry[] RequiredModel { get; }

    /// <summary>
    /// Gets or sets the role permissions. The list of default RolePermissions for all Nodes in the model.
    /// </summary>
    /// <value>The role permissions.</value>
    IRolePermission[] RolePermissions { get; }

    /// <summary>
    /// Gets or sets the version. The version of the model defined in the UANodeSet. This is a human readable string and not intended for programmatic comparisons.
    /// </summary>
    /// <value>The version.</value>
    string Version { get; }
  }
}
﻿//__________________________________________________________________________________________________
//
//  Copyright (C) 2022, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GitHub: https://github.com/mpostol/OPC-UA-OOI/discussions
//__________________________________________________________________________________________________

using System.Xml;
using UAOOI.SemanticData.UANodeSetValidation.DataSerialization;

namespace UAOOI.SemanticData.UANodeSetValidation.UANodeSetDSL
{
  internal interface IUAVariable : IUAInstance
  {
    /// <summary>
    /// Sets the access level. The AccessLevel attribute indicates the accessibility of the Value of a Variable node not taking user access rights into account and
    /// applies only to a UAVariable element. The AccessLevel attribute is used to indicate how the Value of a Variable node can be accessed (read/write) and
    /// if it contains current and/or historic data. The AccessLevel does not take any user access rights into account, i.e. although the Variable is writable this
    /// may be restricted to a certain user / user group.
    /// </summary>
    /// <value>The access level.</value>
    uint? AccessLevel
    {
      set; get;
    }

    /// <summary>
    /// Sets the default value. The value of the Variable node that the server assigns while instantiating the node. Its data type is defined by the DataType field.
    /// </summary>
    /// <value>The default value.</value>
    XmlElement /*Default*/ Value
    {
      set; get;
    }

    /// <summary>
    /// Sets a value indicating whether this <see cref="IUAVariable"/> is historizing. The Historizing attribute indicates whether the server is actively
    /// collecting data for the history of the Variable node. This differs from the AccessLevel which identifies if the Variable has any historical data. A value of <c>true</c>
    /// indicates that the server is actively collecting data. A value of <c>false</c> indicates that  the server is not actively collecting data. Default value is <c>false</c>.
    /// </summary>
    /// <value><c>true</c> if historizing; otherwise, <c>false</c>.</value>
    bool Historizing
    {
      set; get;
    }

    /// <summary>
    /// Sets the minimum sampling interval. The MinimumSamplingInterval attribute indicates how “current” the Value of the Variable NodeClass will be kept.
    /// It specifies (in milliseconds) how fast the server can reasonably sample the value for changes. The accuracy of this value (the ability of the server to attain
    /// “best case” performance) can be greatly affected by the system load and other factors. A MinimumSamplingInterval of 0 indicates that the server is to monitor the
    /// item continuously. A MinimumSamplingInterval of -1 means indeterminate value.
    /// </summary>
    /// <value>The minimum sampling interval.</value>
    double MinimumSamplingInterval
    {
      set; get;
    }

    /// <summary>
    /// Sets the user access level. The UserAccessLevel attribute is used to indicate how the Value attribute of a Variable NodeClass can be accessed (read/write)
    /// and if it contains current or historic data taking user access rights into account. It applies only to a UAVariable element. If the OPC UA Server does not
    /// have the ability to get any user access rights related information from the underlying system it should use the same bit mask as used in the AccessLevel attribute.
    /// The UserAccessLevel attribute can restrict the accessibility indicated by the AccessLevel, but not exceed it.
    /// </summary>
    /// <value>The user access level.</value>
    byte? UserAccessLevel
    {
      set; get;
    }

    string ArrayDimensions { get; set; }
    string DataType { get; set; }
    int ValueRank { get; set; }
    bool Translation { get; }
    NodeId DataTypeNodeId { get; }
  }
}
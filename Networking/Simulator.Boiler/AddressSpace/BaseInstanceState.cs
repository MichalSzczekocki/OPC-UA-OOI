﻿//___________________________________________________________________________________
//
//  Copyright (C) 2021, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using System;
using System.Collections.Generic;
using UAOOI.SemanticData.UANodeSetValidation;
using UAOOI.SemanticData.UANodeSetValidation.DataSerialization;

namespace UAOOI.Networking.Simulator.Boiler.AddressSpace
{
  /// <summary>
  /// Used to receive notifications when a non-value attribute is read or written.
  /// </summary>
  public delegate void NodeStateChangedHandler(ISystemContext context, NodeState node, NodeStateChangeMasks changes);

  /// <summary>
  /// Class BaseInstanceState.
  /// </summary>
  /// <seealso cref="UAOOI.Networking.Simulator.Boiler.AddressSpace.NodeState" />
  public class BaseInstanceState : NodeState
  {
    /// <summary>
    /// Initializes the instance with its default attribute values.
    /// </summary>
    protected BaseInstanceState(NodeState parent, NodeClass nodeClass, QualifiedName browseName) : base(nodeClass, browseName)
    {
      if (parent == null)
        return;
      Parent = (BaseInstanceState)parent;
      Parent.AddChild(this);
    }

    [Obsolete()]
    public BaseInstanceState(NodeState parent) : base(parent) { }

    /// <summary>
    /// The parent node.
    /// </summary>
    public BaseInstanceState Parent { get; internal set; }

    /// <summary>
    /// Returns the id of the default type definition node for the instance.
    /// </summary>
    /// <param name="namespaceUris">The namespace uris.</param>
    /// <returns></returns>
    protected virtual NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
    {
      return null;
    }

    /// <summary>
    /// Finds the child with the specified browse path.
    /// </summary>
    /// <param name="context">The context to use.</param>
    /// <param name="browsePath">The browse path.</param>
    /// <param name="index">The current position in the browse path.</param>
    /// <returns>The target if found. Null otherwise.</returns>
    public virtual BaseInstanceState FindChild(ISystemContext context, IList<QualifiedName> browsePath, int index)
    {
      if (index < 0 || index >= int.MaxValue)
        throw new ArgumentOutOfRangeException("index");
      BaseInstanceState instance = FindChild(context, browsePath[index], false, null);
      if (instance != null)
      {
        if (browsePath.Count == index + 1)
          return instance;
        return instance.FindChild(context, browsePath, index + 1);
      }
      return null;
    }

    /// <summary>
    /// Finds the child with the specified browse name.
    /// </summary>
    /// <param name="context">The context for the system being accessed.</param>
    /// <param name="browseName">The browse name of the children to add.</param>
    /// <param name="createOrReplace">if set to <c>true</c> and the child could exist then the child is created.</param>
    /// <param name="replacement">The replacement to use if createOrReplace is true.</param>
    /// <returns>The child.</returns>
    protected virtual BaseInstanceState FindChild(ISystemContext context, QualifiedName browseName, bool createOrReplace, BaseInstanceState replacement)
    {
      if (QualifiedName.IsNull(browseName))
        return null;
      for (int ii = 0; ii < m_children.Count; ii++)
      {
        BaseInstanceState child = m_children[ii];
        if (browseName == child.BrowseName)
        {
          if (createOrReplace && replacement != null)
            m_children[ii] = child = replacement;
          return child;
        }
      }
      if (createOrReplace)
      {
        if (replacement != null)
          AddChild(replacement);
      }
      return null;
    }

    /// <summary>
    /// Populates a list with the children that belong to the node.
    /// </summary>
    /// <param name="context">The context for the system being accessed.</param>
    /// <param name="children">The list of children to populate.</param>
    /// <exception cref="System.NotImplementedException">This method is added to avoid compiler errors only</exception>
    [Obsolete("This method is added to avoid compiler errors only and will cause NotImplementedException")]
    public virtual void GetChildren(ISystemContext context, IList<BaseInstanceState> children)
    {
      throw new NotImplementedException("This method is added to avoid compiler errors only");
    }

    /// <summary>
    /// Populates a list with the children that belong to the node.
    /// </summary>
    /// <param name="children">The list of children to populate.</param>
    public void GetChildren(IList<BaseInstanceState> children)
    {
      for (int ii = 0; ii < m_children.Count; ii++)
        children.Add(m_children[ii]);
    }

    /// <summary>
    /// Clears the change masks.
    /// </summary>
    /// <param name="context">The context that describes how access the system containing the data..</param>
    /// <param name="includeChildren">if set to <c>true</c> clear masks recursively for all children..</param>
    public void ClearChangeMasks(ISystemContext context, bool includeChildren)
    {
      if (includeChildren)
      {
        List<BaseInstanceState> children = new List<BaseInstanceState>();
        GetChildren(children);
        for (int ii = 0; ii < children.Count; ii++)
          children[ii].ClearChangeMasks(context, true);
      }
      if (ChangeMasks == NodeStateChangeMasks.None)
        return;
      OnStateChanged?.Invoke(context, this, ChangeMasks);
      ChangeMasks = NodeStateChangeMasks.None;
    }

    /// <summary>
    /// Called when ClearChangeMasks is called and the ChangeMask is not None.
    /// </summary>
    public event NodeStateChangedHandler OnStateChanged;

    /// <summary>
    /// Adds a child to the node.
    /// </summary>
    internal void AddChild(BaseInstanceState child)
    {
      m_children.Add(child);
      ChangeMasks |= NodeStateChangeMasks.Children;
    }

    internal void RegisterVariable(IReadOnlyList<BaseInstanceState> hasComponentPath, Action<BaseInstanceState, string[]> register)
    {
      List<BaseInstanceState> _hasComponentPathAndMe = new List<BaseInstanceState>(hasComponentPath)
      {
        this
      };
      CallRegister(_hasComponentPathAndMe, register);
      List<BaseInstanceState> _myComponents = new List<BaseInstanceState>();
      GetChildren(_myComponents);
      for (int ii = 0; ii < _myComponents.Count; ii++)
      {
        if (_myComponents[ii] == this)
          throw new ArgumentOutOfRangeException("Redundant browse path");
        _myComponents[ii].RegisterVariable(_hasComponentPathAndMe, register);
      }
    }

    protected virtual void CallRegister(List<BaseInstanceState> hasComponentPathAndMe, Action<BaseInstanceState, string[]> register)
    {
    }

    private List<BaseInstanceState> m_children = new List<BaseInstanceState>();
  }
}
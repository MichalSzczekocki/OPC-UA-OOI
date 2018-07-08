﻿using System;
using System.Collections.Generic;
using UAOOI.SemanticData.UANodeSetValidation.DataSerialization;
using UAOOI.SemanticData.UANodeSetValidation.Utilities;

namespace UAOOI.Networking.Simulator.Boiler.AddressSpace
{
  public class BaseInstanceState : NodeState
  {

    /// <summary>
    /// Initializes the instance with its default attribute values.
    /// </summary>
    protected BaseInstanceState(NodeClass nodeClass, NodeState parent) : base(nodeClass)
    {
      Parent = parent;
    }
    /// <summary>
    /// The parent node.
    /// </summary>
    public NodeState Parent { get; internal set; }
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
    /// Adds a child to the node. 
    /// </summary>
    public void AddChild(BaseInstanceState child)
    {
      throw new NotImplementedException(nameof(AddChild));
      //if (!Object.ReferenceEquals(child.Parent, this))
      //{
      //  child.Parent = this;

      //  if (NodeId.IsNull(child.ReferenceTypeId))
      //  {
      //    child.ReferenceTypeId = ReferenceTypeIds.HasComponent;
      //  }
      //}

      //if (m_children == null)
      //{
      //  m_children = new List<BaseInstanceState>();
      //}

      //m_children.Add(child);
      //ChangeMasks |= NodeStateChangeMasks.Children;
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
      if (index < 0 || index >= Int32.MaxValue)
        throw new ArgumentOutOfRangeException("index");

      BaseInstanceState instance = FindChild(context, browsePath[index], false, null);

      if (instance != null)
      {
        if (browsePath.Count == index + 1)
        {
          return instance;
        }

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
      {
        return null;
      }

      if (m_children != null)
      {
        for (int ii = 0; ii < m_children.Count; ii++)
        {
          BaseInstanceState child = m_children[ii];

          if (browseName == child.BrowseName)
          {
            if (createOrReplace && replacement != null)
            {
              m_children[ii] = child = replacement;
            }

            return child;
          }
        }
      }

      if (createOrReplace)
      {
        if (replacement != null)
        {
          AddChild(replacement);
        }
      }

      return null;
    }
    /// <summary>
    /// What has changed in the node since <see cref="ClearChangeMasks"/> was last called.
    /// </summary>
    /// <value>The change masks that indicates what has changed in a node.</value>
    private List<BaseInstanceState> m_children = null;

  }
}
﻿//___________________________________________________________________________________
//
//  Copyright (C) 2019, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using UAOOI.SemanticData.BuildingErrorsHandling;
using UAOOI.SemanticData.InformationModelFactory;
using UAOOI.SemanticData.InformationModelFactory.UAConstants;
using UAOOI.SemanticData.UANodeSetValidation.DataSerialization;
using UAOOI.SemanticData.UANodeSetValidation.UAInformationModel;
using UAOOI.SemanticData.UANodeSetValidation.XML;

namespace UAOOI.SemanticData.UANodeSetValidation
{
  /// <summary>
  /// Class Validator - contains static methods used to validate and export a collection of nodes - part of the Address Space.
  /// </summary>
  internal class Validator : IValidator
  {
    public Validator(IAddressSpaceBuildContext addressSpace, IBuildErrorsHandling traceBuildErrorsHandling)
    {
      AS = addressSpace;
      Log = traceBuildErrorsHandling;

    }

    private readonly IAddressSpaceBuildContext AS;
    public readonly IBuildErrorsHandling Log;

    #region internal API

    /// <summary>
    /// Validates <paramref name="nodeContext" /> and exports it using an object of <see cref="IModelFactory" />  type.
    /// </summary>
    /// <param name="nodeContext">The node context to be validated and exported.</param>
    /// <param name="exportFactory">A model export factory.</param>
    /// <param name="parentReference">The reference to parent node.</param>
    /// <exception cref="ApplicationException">In {nameof(ValidateExportNode)}</exception>
    public void ValidateExportNode(IUANodeBase nodeContext, INodeContainer exportFactory, UAReferenceContext parentReference)
    {
      Debug.Assert(nodeContext != null, "Validator.ValidateExportNode the argument nodeContext is null.");
      //TODO Handle HasComponent ReferenceType errors. #42
      if (Object.ReferenceEquals(nodeContext.UANode, null))
      {
        string _msg = string.Format("The node {0} is undefined", nodeContext.NodeIdContext);
        BuildError _be = null;
        if (parentReference == null || parentReference.ReferenceKind == ReferenceKindEnum.HasProperty)
          _be = BuildError.UndefinedHasPropertyTarget;
        else
          _be = BuildError.UndefinedHasComponentTarget;
        TraceMessage _traceMessage = TraceMessage.BuildErrorTraceMessage(_be, _msg);
        Log.TraceEvent(_traceMessage);
        CreateModelDesignStub(exportFactory);
      }
      else
      {
        switch (nodeContext.UANode.NodeClassEnum)
        {
          case NodeClassEnum.UADataType:
            CreateNode<IDataTypeFactory, UADataType>(exportFactory.AddNodeFactory<IDataTypeFactory>, nodeContext, (x, y) => Update(x, y), UpdateType);
            break;

          case NodeClassEnum.UAMethod:
            CreateNode<IMethodInstanceFactory, UAMethod>(exportFactory.AddNodeFactory<IMethodInstanceFactory>, nodeContext, (x, y) => Update(x, y, parentReference), UpdateInstance);
            break;

          case NodeClassEnum.UAObject:
            CreateNode<IObjectInstanceFactory, UAObject>(exportFactory.AddNodeFactory<IObjectInstanceFactory>, nodeContext, (x, y) => Update(x, y), UpdateInstance);
            break;

          case NodeClassEnum.UAObjectType:
            CreateNode<IObjectTypeFactory, UAObjectType>(exportFactory.AddNodeFactory<IObjectTypeFactory>, nodeContext, Update, UpdateType);
            break;

          case NodeClassEnum.UAReferenceType:
            CreateNode<IReferenceTypeFactory, UAReferenceType>(exportFactory.AddNodeFactory<IReferenceTypeFactory>, nodeContext, (x, y) => Update(x, y), UpdateType);
            break;

          case NodeClassEnum.UAVariable:
            if (parentReference.ReferenceKind == ReferenceKindEnum.HasProperty)
              CreateNode<IPropertyInstanceFactory, UAVariable>(exportFactory.AddNodeFactory<IPropertyInstanceFactory>, nodeContext, (x, y) => Update(x, y, nodeContext, parentReference), UpdateInstance);
            else
              CreateNode<IVariableInstanceFactory, UAVariable>(exportFactory.AddNodeFactory<IVariableInstanceFactory>, nodeContext, (x, y) => Update(x, y, nodeContext, parentReference), UpdateInstance);
            break;

          case NodeClassEnum.UAVariableType:
            CreateNode<IVariableTypeFactory, UAVariableType>(exportFactory.AddNodeFactory<IVariableTypeFactory>, nodeContext, (x, y) => Update(x, y), UpdateType);
            break;

          case NodeClassEnum.UAView:
            CreateNode<IViewInstanceFactory, UAView>(exportFactory.AddNodeFactory<IViewInstanceFactory>, nodeContext, (x, y) => Update(x, y), UpdateInstance);
            break;

          case NodeClassEnum.Unknown:
            throw new ApplicationException($"In {nameof(ValidateExportNode)} unexpected NodeClass value");
        }
      }
    }

    #endregion internal API

    #region private

    private static ApplicationException InstanceDeclarationNotSupported(NodeClassEnum nodeClass)
    {
      return new ApplicationException($"{nodeClass} doesn't support instance declarations");
    }

    private void Update(IObjectInstanceFactory nodeDesign, UAObject nodeSet)
    {
      nodeDesign.SupportsEvents = nodeSet.EventNotifier.GetSupportsEvents(Log.TraceEvent);
    }

    private void Update(IPropertyInstanceFactory propertyInstance, UAVariable nodeSet, IUANodeBase nodeContext, UAReferenceContext parentReference)
    {
      try
      {
        Update(propertyInstance, nodeSet);
        propertyInstance.ReferenceType = parentReference == null ? null : parentReference.GetReferenceTypeName();
        if (!nodeContext.IsProperty)
          Log.TraceEvent(TraceMessage.BuildErrorTraceMessage(BuildError.WrongReference2Property, $"Creating Property {nodeContext.BrowseName}- wrong reference type {parentReference.ReferenceKind.ToString()}"));
      }
      catch (Exception _ex)
      {
        Log.TraceEvent(TraceMessage.BuildErrorTraceMessage(BuildError.WrongReference2Property, string.Format("Cannot resolve the reference for Property because of error {0} at: {1}.", _ex, _ex.StackTrace)));
      }
    }

    private void Update(IVariableInstanceFactory variableInstance, UAVariable nodeSet, IUANodeBase nodeContext, UAReferenceContext parentReference)
    {
      try
      {
        Update(variableInstance, nodeSet);
        variableInstance.ReferenceType = parentReference == null ? null : parentReference.GetReferenceTypeName();
        if (nodeContext.IsProperty)
          Log.TraceEvent(TraceMessage.BuildErrorTraceMessage(BuildError.WrongReference2Variable, string.Format("Creating Variable - wrong reference type {0}", parentReference.ReferenceKind.ToString())));
      }
      catch (Exception _ex)
      {
        Log.TraceEvent(TraceMessage.BuildErrorTraceMessage(BuildError.WrongReference2Property, string.Format("Cannot resolve the reference for Variable because of error {0} at: {1}.", _ex, _ex.StackTrace)));
      }
    }

    private void Update(IVariableInstanceFactory nodeDesign, UAVariable nodeSet)
    {
      nodeDesign.AccessLevel = nodeSet.AccessLevel.GetAccessLevel(Log.TraceEvent);
      nodeDesign.ArrayDimensions = nodeSet.ArrayDimensions.ExportString(string.Empty);
      nodeDesign.DataType = string.IsNullOrEmpty(nodeSet.DataType) ? null : AS.ExportBrowseName(NodeId.Parse(nodeSet.DataType), DataTypes.Number);//TODO add test case must be DataType, must not be abstract
      nodeDesign.DefaultValue = nodeSet.Value; //TODO add test case must be of type defined by DataType
      nodeDesign.Historizing = nodeSet.Historizing.Export(false);
      nodeDesign.MinimumSamplingInterval = nodeSet.MinimumSamplingInterval.Export(0D);
      nodeDesign.ValueRank = nodeSet.ValueRank.GetValueRank(Log.TraceEvent);
      if (nodeSet.Translation != null)
        Log.TraceEvent(TraceMessage.BuildErrorTraceMessage(BuildError.NotSupportedFeature, "- the Translation element for the UAVariable"));
    }

    private void Update(IVariableTypeFactory nodeDesign, UAVariableType nodeSet)
    {
      nodeDesign.ArrayDimensions = nodeSet.ArrayDimensions.ExportString(string.Empty);
      nodeDesign.DataType = AS.ExportBrowseName(NodeId.Parse(nodeSet.DataType), DataTypes.Number);
      nodeDesign.DefaultValue = nodeSet.Value;
      nodeDesign.ValueRank = nodeSet.ValueRank.GetValueRank(Log.TraceEvent);
    }

    private void Update(IMethodInstanceFactory nodeDesign, UAMethod nodeSet, UAReferenceContext parentReference)
    {
      if (nodeSet.ArgumentDescription != null)
        foreach (UAMethodArgument _argument in nodeSet.ArgumentDescription)
        {
          if (_argument.Description == null)
            continue;
          foreach (XML.LocalizedText _description in _argument.Description)
            nodeDesign.AddArgumentDescription(_argument.Name, _description.Locale, _description.Value);
        }
      nodeDesign.Executable = !nodeSet.Executable ? nodeSet.Executable : new Nullable<bool>();
      nodeDesign.UserExecutable = !nodeSet.UserExecutable ? nodeSet.UserExecutable : new Nullable<bool>();
      nodeDesign.MethodDeclarationId = nodeSet.MethodDeclarationId;
      nodeDesign.ReleaseStatus = nodeSet.ReleaseStatus.ConvertToReleaseStatus();
      nodeDesign.AddInputArguments(x => GetParameters(x));
      nodeDesign.AddOutputArguments(x => GetParameters(x));
    }

    private void Update(IViewInstanceFactory nodeDesign, UAView nodeSet)
    {
      nodeDesign.ContainsNoLoops = nodeSet.ContainsNoLoops;//TODO add test case against the loops in the model.
      nodeDesign.SupportsEvents = nodeSet.EventNotifier.GetSupportsEvents(Log.TraceEvent);
    }

    private void Update(IDataTypeFactory nodeDesign, UADataType nodeSet)
    {
      nodeSet.Definition.GetParameters(nodeDesign.NewDefinition(), AS, Log.TraceEvent);
      nodeDesign.DataTypePurpose = nodeSet.Purpose.ConvertToDataTypePurpose();
      if (nodeSet.Purpose != XML.DataTypePurpose.Normal)
        Log.TraceEvent(TraceMessage.DiagnosticTraceMessage($"DataTypePurpose value {nodeSet.Purpose } is not supported by the tool"));
    }

    private void Update(IReferenceTypeFactory nodeDesign, UAReferenceType nodeSet)
    {
      nodeSet.InverseName.ExportLocalizedTextArray(nodeDesign.AddInverseName);
      nodeDesign.Symmetric = nodeSet.Symmetric;
      if (nodeSet.Symmetric && (nodeSet.InverseName != null && nodeSet.InverseName.Where(x => !string.IsNullOrEmpty(x.Value)).Any()))
      {
        XML.LocalizedText _notEmpty = nodeSet.InverseName.Where(x => !string.IsNullOrEmpty(x.Value)).First();
        Log.TraceEvent(TraceMessage.BuildErrorTraceMessage(BuildError.WrongInverseName, string.Format("If ReferenceType {0} is symmetric the InverseName {1}:{2} shall be omitted.", nodeSet.NodeIdentifier(), _notEmpty.Locale, _notEmpty.Value)));
      }
      else if (!nodeSet.Symmetric && !nodeSet.IsAbstract && (nodeSet.InverseName == null || !nodeSet.InverseName.Where(x => !string.IsNullOrEmpty(x.Value)).Any()))
        Log.TraceEvent(TraceMessage.BuildErrorTraceMessage(BuildError.WrongInverseName, string.Format("If ReferenceType {0} is not symmetric and not abstract the InverseName shall be specified.", nodeSet.NodeIdentifier())));
    }

    private void Update(IObjectTypeFactory nodeDesign, UAObjectType nodeSet)
    {
    }

    private void CreateNode<FactoryType, NodeSetType>
      (
        Func<FactoryType> createNode,
        IUANodeBase nodeContext,
        Action<FactoryType, NodeSetType> updateNode,
        Action<FactoryType, NodeSetType, IUANodeBase> updateBase
      )
      where FactoryType : INodeFactory
      where NodeSetType : UANode
    {
      FactoryType _nodeFactory = createNode();
      nodeContext.CalculateNodeReferences(_nodeFactory, this);
      NodeSetType _nodeSet = (NodeSetType)nodeContext.UANode;
      XmlQualifiedName _browseName = nodeContext.ExportNodeBrowseName();
      string _symbolicName;
      if (string.IsNullOrEmpty(_nodeSet.SymbolicName))
        _symbolicName = _browseName.Name.ValidateIdentifier(Log.TraceEvent); //TODO IsValidLanguageIndependentIdentifier is not supported by the .NET standard #340
      else
        _symbolicName = _nodeSet.SymbolicName.ValidateIdentifier(Log.TraceEvent); //TODO IsValidLanguageIndependentIdentifier is not supported by the .NET standard #340
      _nodeFactory.BrowseName = _browseName.Name.ExportString(_symbolicName);
      _nodeSet.Description.ExportLocalizedTextArray(_nodeFactory.AddDescription);
      _nodeSet.DisplayName.Truncate(512, Log.TraceEvent).ExportLocalizedTextArray(_nodeFactory.AddDisplayName);
      _nodeFactory.SymbolicName = new XmlQualifiedName(_symbolicName, _browseName.Namespace);
      Action<uint, string> _doReport = (x, y) =>
      {
        Log.TraceEvent(TraceMessage.BuildErrorTraceMessage(BuildError.WrongWriteMaskValue, string.Format("The current value is {0:x} of the node type {1}.", x, y)));
      };
      _nodeFactory.WriteAccess = _nodeSet is UAVariable ? _nodeSet.WriteMask.Validate(0x200000, x => _doReport(x, _nodeSet.GetType().Name)) : _nodeSet.WriteMask.Validate(0x400000, x => _doReport(x, _nodeSet.GetType().Name));
      _nodeFactory.AccessRestrictions = ConvertToAccessRestrictions(_nodeSet.AccessRestrictions, _nodeSet.GetType().Name);
      _nodeFactory.Category = _nodeSet.Category;
      if (_nodeSet.RolePermissions != null)
        Log.TraceEvent(TraceMessage.DiagnosticTraceMessage("RolePermissions is not supported. You must fix it manually."));
      if (!string.IsNullOrEmpty(_nodeSet.Documentation))
        Log.TraceEvent(TraceMessage.DiagnosticTraceMessage("Documentation is not supported. You must fix it manually."));
      updateBase(_nodeFactory, _nodeSet, nodeContext);
      updateNode(_nodeFactory, _nodeSet);
    }

    private AccessRestrictions ConvertToAccessRestrictions(byte accessRestrictions, string typeName)
    {
      if (accessRestrictions > 7)
      {
        Log.TraceEvent(TraceMessage.BuildErrorTraceMessage(BuildError.WrongAccessLevel, $"The current value is {accessRestrictions} of the node type {typeName}. Assigned max value"));
        return AccessRestrictions.EncryptionRequired & AccessRestrictions.SessionRequired & AccessRestrictions.SigningRequired;
      }
      return (AccessRestrictions)accessRestrictions;
    }

    private void UpdateType(ITypeFactory nodeDesign, UAType nodeSet, IUANodeBase nodeContext)
    {
      nodeDesign.BaseType = nodeContext.ExportBaseTypeBrowseName(true);
      nodeDesign.IsAbstract = nodeSet.IsAbstract;
    }

    private static void UpdateInstance(IInstanceFactory nodeDesign, UAInstance nodeSet, IUANodeBase nodeContext)
    {
      if (nodeContext.ModelingRule.HasValue)
        nodeDesign.ModelingRule = nodeContext.ModelingRule.Value;
      nodeDesign.TypeDefinition = nodeContext.ExportBaseTypeBrowseName(false);
      //nodeSet.ParentNodeId - The NodeId of the Node that is the parent of the Node within the information model. This field is used to indicate
      //that a tight coupling exists between the Node and its parent (e.g. when the parent is deleted the child is deleted
      //as well). This information does not appear in the AddressSpace and is intended for use by design tools.
    }

    private static void CreateModelDesignStub(INodeContainer factory)
    {
      BuildError _err = BuildError.DanglingReferenceTarget;
      IPropertyInstanceFactory _pr = factory.AddNodeFactory<IPropertyInstanceFactory>();
      _pr.SymbolicName = new XmlQualifiedName(string.Format("{0}{1}", _err.Focus.ToString(), m_ErrorNumber++), "http://commsvr.com/OOIUA/SemanticData/UANodeSetValidation");
      _pr.AddDescription("en-en", _err.Descriptor);
      _pr.AddDisplayName("en-en", string.Format("ERROR{0}", m_ErrorNumber));
    }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    /// <param name="arguments">The <see cref="XmlElement"/> encapsulates the arguments.</param>
    /// <returns>Parameter[].</returns>
    private Parameter[] GetParameters(XmlElement arguments)
    {
      List<Parameter> _parameters = new List<Parameter>();
      foreach (DataSerialization.Argument _item in arguments.GetParameters())
        _parameters.Add(AS.ExportArgument(_item));
      return _parameters.ToArray();
    }

    private static int m_ErrorNumber = 0;

    #endregion private
  }
}
﻿//___________________________________________________________________________________
//
//  Copyright (C) 2021, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using UAOOI.SemanticData.UANodeSetValidation.XML;

namespace UAOOI.SemanticData.UANodeSetValidation.UnitTest
{
  [TestClass]
  [DeploymentItem(@"XMLModels\", @"XMLModels\")]
  public class ResourcesUnitTest
  {
    [TestMethod]
    [TestCategory("Code")]
    public void OpcUaNodeSet2TestMethod()
    {
      FileInfo _testDataFileInfo = new FileInfo(@"XMLModels\CorrectModels\ReferenceTest\ReferenceTest.NodeSet2.xml");
      Assert.IsTrue(_testDataFileInfo.Exists);
      UANodeSet _model = UANodeSet.ReadModelFile(_testDataFileInfo);
      Assert.IsNotNull(_model);
    }

    [TestMethod]
    [TestCategory("Code")]
    public void ReadUADefinedTypesTestMethod()
    {
      UANodeSet _standard = UANodeSet.ReadUADefinedTypes();
      Assert.IsNotNull(_standard);
      Assert.IsNull(_standard.NamespaceUris);
    }
  }
}
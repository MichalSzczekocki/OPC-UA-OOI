﻿//___________________________________________________________________________________
//
//  Copyright (C) 2021, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using System;
using System.IO;
using UAOOI.SemanticData.BuildingErrorsHandling;
using UAOOI.SemanticData.UAModelDesignExport.XML;
using UAOOI.SemanticData.UANodeSetValidation;

namespace UAOOI.SemanticData.UAModelDesignExport.Instrumentation
{

  /// <summary>
  /// Class AddressSpaceContextService - Entry point to create Information Model using the <see name="AddressSpaceContextService.CreateInstance"/> to populate it.
  /// </summary>
  /// <remarks>It may be used to export selected target namespace .</remarks>
  public static class AddressSpaceContextService
  {

    #region public static
    /// <summary>
    /// Creates new instance of the <see cref="ModelDesign.ModelDesign" />.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="traceEvent">The trace event.</param>
    /// <returns>An object of <see cref="ModelDesign.ModelDesign"/>.</returns>
    /// <exception cref="System.IO.FileNotFoundException">The imported file does not exist</exception>
    public static ModelDesign CreateInstance(FileInfo filePath, string URI, Action<TraceMessage> traceEvent)
    {
      if (!filePath.Exists)
        throw new FileNotFoundException("The imported file does not exist", filePath.FullName);
      traceEvent(TraceMessage.DiagnosticTraceMessage("Entering AddressSpaceContextService.CreateInstance"));
      BuildErrorsHandling.Log.TraceEventAction += traceEvent;
      IAddressSpaceContext _as = AddressSpaceFactory.AddressSpace;
      ModelFactory _factory = new ModelFactory(traceEvent);
      _as.InformationModelFactory = _factory;
      _as.ImportUANodeSet(filePath);
      _as.ValidateAndExportModel(new Uri(URI));
      return _factory.Export();
    }
    #endregion

  }

}

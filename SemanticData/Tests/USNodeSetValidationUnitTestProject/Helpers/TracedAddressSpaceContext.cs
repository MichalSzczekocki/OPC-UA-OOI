﻿//__________________________________________________________________________________________________
//
//  Copyright (C) 2021, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GitHub: https://github.com/mpostol/OPC-UA-OOI/discussions
//__________________________________________________________________________________________________

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UAOOI.SemanticData.BuildingErrorsHandling;
using UAOOI.SemanticData.UANodeSetValidation.Diagnostic;

namespace UAOOI.SemanticData.UANodeSetValidation.Helpers
{
  internal class TracedAddressSpaceContext : IBuildErrorsHandling
  {
    public TracedAddressSpaceContext()
    {
      AddressSpaceContext = new AddressSpaceContext(this);
    }

    public void TestConsistency(int diagnosticCounter, int errorsCounter)
    {
      //TODO Enhance/Improve the Program logging and tracing infrastructure. #590
      //Assert.AreEqual<int>(diagnosticCounter, Errors);
      Assert.AreEqual<int>(errorsCounter, TraceList.Count);
    }

    internal AddressSpaceContext AddressSpaceContext = null;
    internal readonly List<TraceMessage> TraceList = new List<TraceMessage>();

    internal void Clear()
    {
      Errors = 0;
      TraceList.Clear();
    }

    #region IBuildErrorsHandling

    public int Errors { get; set; } = 0;

    public void TraceData(TraceEventType eventType, int id, object data)
    {
      string message = $"TraceData eventType = {eventType}, id = {id}, {data}";
      Console.WriteLine(message);
      if (eventType == TraceEventType.Critical || eventType == TraceEventType.Error)
        throw new ApplicationException(message);
    }

    public void WriteTraceMessage(TraceMessage traceMessage)
    {
      Console.WriteLine(traceMessage.ToString());
      Errors++;
      if (traceMessage.BuildError.Focus != Focus.Diagnostic)
        TraceList.Add(traceMessage);
    }

    #endregion IBuildErrorsHandling
  }
}
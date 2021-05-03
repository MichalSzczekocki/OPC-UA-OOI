﻿//___________________________________________________________________________________
//
//  Copyright (C) 2019, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using CommandLine;
using System;
using System.Collections.Generic;

namespace UAOOI.SemanticData.AddressSpacePrototyping.CommandLineSyntax
{
  internal static class Extensions
  {
    internal static void Parse<T>(this string[] args, Action<T> RunCommand, Action<IEnumerable<Error>> dump)
    {
      using (Parser parser = Parser.Default)
      {
        ParserResult<T> parserResult = parser.ParseArguments<T>(args).WithParsed<T>((T opts) => { RunCommand(opts); }).WithNotParsed<T>(dump);
      }
    }
  }
}
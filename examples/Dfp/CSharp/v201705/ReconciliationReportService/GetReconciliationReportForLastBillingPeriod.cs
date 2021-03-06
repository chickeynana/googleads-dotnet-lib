// Copyright 2016, Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using Google.Api.Ads.Dfp.Lib;
using Google.Api.Ads.Dfp.Util.v201705;
using Google.Api.Ads.Dfp.v201705;
using System;

namespace Google.Api.Ads.Dfp.Examples.CSharp.v201705 {
  /// <summary>
  /// This example gets the previous billing period's reconciliation report.
  /// </summary>
  public class GetReconciliationReportForLastBillingPeriod : SampleBase {
    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This example gets the previous billing period's reconciliation report.";
      }
    }

    /// <summary>
    /// Main method, to run this code example as a standalone application.
    /// </summary>
    public static void Main() {
      GetReconciliationReportForLastBillingPeriod codeExample =
          new GetReconciliationReportForLastBillingPeriod();
      Console.WriteLine(codeExample.Description);
      try {
        codeExample.Run(new DfpUser());
      } catch (Exception e) {
        Console.WriteLine("Failed to get reconciliation reports. Exception says \"{0}\"",
            e.Message);
      }
    }

    /// <summary>
    /// Run the code example.
    /// </summary>
    public void Run(DfpUser dfpUser) {
      using (ReconciliationReportService reconciliationReportService =
          (ReconciliationReportService) dfpUser
              .GetService(DfpService.v201705.ReconciliationReportService)) {

        // Create a statement to select reconciliation reports.
        int pageSize = StatementBuilder.SUGGESTED_PAGE_LIMIT;
        StatementBuilder statementBuilder = new StatementBuilder()
            .Where("startDate = :startDate")
            .OrderBy("id ASC")
            .Limit(pageSize)
            .AddValue("startDate", DateTimeUtilities.FromDateTime(
                new System.DateTime(System.DateTime.Today.Year,
                    System.DateTime.Today.Month - 1, 1), "America/New_York").date
            );

        // Retrieve a small amount of reconciliation reports at a time, paging through until all
        // reconciliation reports have been retrieved.
        int totalResultSetSize = 0;
        do {
          ReconciliationReportPage page =
              reconciliationReportService.getReconciliationReportsByStatement(
                  statementBuilder.ToStatement());

          // Print out some information for each reconciliation report.
          if (page.results != null) {
            totalResultSetSize = page.totalResultSetSize;
            int i = page.startIndex;
            foreach (ReconciliationReport reconciliationReport in page.results) {
              String startDateString = new System.DateTime(
                  day: reconciliationReport.startDate.day,
                  month: reconciliationReport.startDate.month,
                  year: reconciliationReport.startDate.year
              ).ToString("d");
              Console.WriteLine(
                  "{0}) Reconciliation report with ID {1} and start date \"{2}\" was found.",
                  i++,
                  reconciliationReport.id,
                  startDateString
              );
            }
          }

          statementBuilder.IncreaseOffsetBy(pageSize);
        } while (statementBuilder.GetOffset() < totalResultSetSize);

        Console.WriteLine("Number of results found: {0}", totalResultSetSize);
      }
    }
  }
}

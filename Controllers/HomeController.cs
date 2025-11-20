using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using part_1.Models;
using Prometheus;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using BenchmarkDotNet.Reports;
namespace part_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            auto_create_check check = new auto_create_check();
            check.InitializeSystem();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }





        //poe

        //report
        public IActionResult report()
        {
            LecturerClaimViewModel load = new LecturerClaimViewModel();
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.id = HttpContext.Session.GetString("id");



            return View(load);
        }
        //GenerateCompanyReportPDF

        [HttpPost]
        public IActionResult GenerateCompanyReportPDF([FromBody] List<int> claimIDs)
        {
            var model = new LecturerClaimViewModel();
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // Filter selected claims
            var selectedClaims = new List<ClaimItem>();
            for (int i = 0; i < model.ClaimIDs.Count; i++)
            {
                if (claimIDs.Contains(model.ClaimIDs[i]))
                {
                    selectedClaims.Add(new ClaimItem
                    {
                        ClaimID = model.ClaimIDs[i],
                        ModuleName = model.ModuleNames[i],
                        NumberOfSessions = model.NumberOfSessions[i],
                        NumberOfHours = model.NumberOfHours[i],
                        Rate = model.Rates[i],
                        TotalAmount = model.TotalAmounts[i],
                        FacultyName = model.FacultyNames[i],
                        LecturerName = model.FullNames[i],
                        SupportingDocument = model.SupportingDocuments[i],
                        CreatingDate = model.CreatingDates[i]
                    });
                }
            }

            if (!selectedClaims.Any())
                return BadRequest("No claims selected");

            // Group claims by lecturer
            var lecturerGroups = selectedClaims
                .GroupBy(c => c.LecturerName)
                .Select(g => new
                {
                    Lecturer = g.Key,
                    Faculty = g.First().FacultyName,
                    TotalClaims = g.Count(),
                    TotalHours = g.Sum(x => x.NumberOfHours),
                    TotalAmount = g.Sum(x => x.TotalAmount)
                })
                .ToList();

            // Company totals
            var totalLecturers = lecturerGroups.Count;
            var totalClaimsOverall = selectedClaims.Count;
            var totalHoursOverall = selectedClaims.Sum(c => c.NumberOfHours);
            var totalAmountOverall = selectedClaims.Sum(c => c.TotalAmount);
            var averageClaim = totalClaimsOverall > 0 ? totalAmountOverall / totalClaimsOverall : 0;

            // Generate PDF
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

                    // Header
                    page.Header().Element(ComposeHeader);

                    // Content
                    page.Content().Element(ComposeContent);

                    // Footer
                    page.Footer().Element(ComposeFooter);

                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Company_Payroll_Report_{DateTime.Now:yyyyMMdd_HHmm}.pdf");

            void ComposeHeader(IContainer container)
            {
                container.Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("COMPANY PAYROLL REPORT")
                            .FontSize(20)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        column.Item().Text("Human Resources Department")
                            .FontSize(12)
                            .FontColor(Colors.Grey.Darken1);

                        column.Item().Text($"Reporting Period: {DateTime.Now:MMMM yyyy}")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Medium);
                    });

                    row.ConstantItem(120).AlignRight().Column(column =>
                    {
                        column.Item().Text("Rosebank College")
                            .FontSize(11)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3)
                            .AlignRight();

                        column.Item().Text("Finance & HR Division")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Medium)
                            .AlignRight();

                        column.Item().Text($"Generated: {DateTime.Now:dd MMM yyyy}")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Medium)
                            .AlignRight();
                    });
                });
            }

            void ComposeContent(IContainer container)
            {
                container.Column(column =>
                {
                    column.Spacing(20);

                    // Executive Summary Cards
                    column.Item().Element(ComposeSummaryCards);

                    // Lecturer Breakdown Table
                    column.Item().Element(ComposeLecturerTable);

                    // Detailed Financial Summary
                    column.Item().Element(ComposeFinancialSummary);
                });
            }

            void ComposeSummaryCards(IContainer container)
            {
                container.Grid(grid =>
                {
                    grid.Columns(4);
                    grid.Spacing(10);

                    // Total Lecturers Card
                    grid.Item().Background(Colors.Blue.Lighten5).Padding(15).CornerRadius(8).Column(card =>
                    {
                        card.Item().Text("TOTAL LECTURERS").Bold().FontSize(11).FontColor(Colors.Blue.Darken3);
                        card.Item().Text(totalLecturers.ToString()).FontSize(18).Bold().FontColor(Colors.Blue.Darken3);
                        card.Item().Text("Active Faculty").FontSize(9).FontColor(Colors.Grey.Medium);
                    });

                    // Total Claims Card
                    grid.Item().Background(Colors.Green.Lighten5).Padding(15).CornerRadius(8).Column(card =>
                    {
                        card.Item().Text("TOTAL CLAIMS").Bold().FontSize(11).FontColor(Colors.Green.Darken3);
                        card.Item().Text(totalClaimsOverall.ToString()).FontSize(18).Bold().FontColor(Colors.Green.Darken3);
                        card.Item().Text("Processed").FontSize(9).FontColor(Colors.Grey.Medium);
                    });

                    // Total Hours Card
                    grid.Item().Background(Colors.Orange.Lighten5).Padding(15).CornerRadius(8).Column(card =>
                    {
                        card.Item().Text("TOTAL HOURS").Bold().FontSize(11).FontColor(Colors.Orange.Darken3);
                        card.Item().Text(totalHoursOverall.ToString()).FontSize(18).Bold().FontColor(Colors.Orange.Darken3);
                        card.Item().Text("Hours Worked").FontSize(9).FontColor(Colors.Grey.Medium);
                    });

                    // Total Amount Card
                    grid.Item().Background(Colors.Red.Lighten5).Padding(15).CornerRadius(8).Column(card =>
                    {
                        card.Item().Text("TOTAL PAYROLL").Bold().FontSize(11).FontColor(Colors.Red.Darken3);
                        card.Item().Text(totalAmountOverall.ToString("C0", CultureInfo.CreateSpecificCulture("en-ZA"))).FontSize(18).Bold().FontColor(Colors.Red.Darken3);
                        card.Item().Text("Gross Amount").FontSize(9).FontColor(Colors.Grey.Medium);
                    });
                });
            }

            void ComposeLecturerTable(IContainer container)
            {
                container.Column(col =>
                {
                    col.Item().Text("LECTURER BREAKDOWN").Bold().FontSize(14).FontColor(Colors.Blue.Darken3);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Lecturer
                            columns.RelativeColumn(2); // Faculty
                            columns.ConstantColumn(60); // Claims
                            columns.ConstantColumn(60); // Hours
                            columns.ConstantColumn(80); // Amount
                        });

                        // Table Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Darken3).Padding(10).Text("LECTURER NAME").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(10).Text("FACULTY").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(10).Text("CLAIMS").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(10).Text("HOURS").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(10).Text("AMOUNT").FontColor(Colors.White).Bold().FontSize(9);
                        });

                        // Table Rows
                        for (int i = 0; i < lecturerGroups.Count; i++)
                        {
                            var lecturer = lecturerGroups[i];
                            var backgroundColor = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                            table.Cell().Background(backgroundColor).Padding(8).Text(lecturer.Lecturer).FontSize(9);
                            table.Cell().Background(backgroundColor).Padding(8).Text(lecturer.Faculty).FontSize(9);
                            table.Cell().Background(backgroundColor).Padding(8).Text(lecturer.TotalClaims.ToString()).FontSize(9).AlignRight();
                            table.Cell().Background(backgroundColor).Padding(8).Text(lecturer.TotalHours.ToString()).FontSize(9).AlignRight();
                            table.Cell().Background(backgroundColor).Padding(8).Text(lecturer.TotalAmount.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"))).FontSize(9).AlignRight().SemiBold();
                        }
                    });
                });
            }

            void ComposeFinancialSummary(IContainer container)
            {
                container.Background(Colors.Grey.Lighten4).Padding(20).CornerRadius(10).Column(summary =>
                {
                    summary.Spacing(10);

                    summary.Item().Text("FINANCIAL SUMMARY").Bold().FontSize(16).FontColor(Colors.Blue.Darken3);

                    summary.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Row(detail =>
                            {
                                detail.RelativeItem().Text("Total Lecturers:");
                                detail.ConstantItem(100).Text(totalLecturers.ToString()).AlignRight().SemiBold();
                            });
                            col.Item().Row(detail =>
                            {
                                detail.RelativeItem().Text("Total Claims Processed:");
                                detail.ConstantItem(100).Text(totalClaimsOverall.ToString()).AlignRight().SemiBold();
                            });
                            col.Item().Row(detail =>
                            {
                                detail.RelativeItem().Text("Total Hours Worked:");
                                detail.ConstantItem(100).Text(totalHoursOverall.ToString()).AlignRight().SemiBold();
                            });
                        });

                        row.ConstantItem(20); // Spacer

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Row(detail =>
                            {
                                detail.RelativeItem().Text("Average per Claim:");
                                detail.ConstantItem(120).Text(averageClaim.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"))).AlignRight().SemiBold();
                            });
                            col.Item().Row(detail =>
                            {
                                detail.RelativeItem().Text("Average per Lecturer:");
                                detail.ConstantItem(120).Text((totalAmountOverall / totalLecturers).ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"))).AlignRight().SemiBold();
                            });
                            col.Item().Row(detail =>
                            {
                                detail.RelativeItem().Text("Average Hourly Rate:");
                                detail.ConstantItem(120).Text((totalHoursOverall > 0 ? totalAmountOverall / totalHoursOverall : 0).ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"))).AlignRight().SemiBold();
                            });
                        });
                    });

                    summary.Item().PaddingTop(10).BorderTop(1).BorderColor(Colors.Grey.Medium).Row(totalRow =>
                    {
                        totalRow.RelativeItem().Text("TOTAL PAYROLL AMOUNT:").Bold().FontSize(12);
                        totalRow.ConstantItem(150).Text(totalAmountOverall.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"))).Bold().FontSize(12).AlignRight().FontColor(Colors.Green.Darken2);
                    });

                    summary.Item().PaddingTop(15).Background(Colors.Blue.Lighten5).Padding(10).CornerRadius(5)
                        .Text("This report summarizes all payroll claims processed for the current period.")
                        .FontSize(9)
                        .Italic()
                        .FontColor(Colors.Blue.Darken2);
                });
            }

            void ComposeFooter(IContainer container)
            {
                container.BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(10).Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(footerColumn =>
                        {
                            footerColumn.Item().Text("Rosebank College Finance & HR Department").Bold().FontSize(9);
                            footerColumn.Item().Text("123 Rosebank College Avenue • finance@RosebankCollege.ac.za").FontSize(8).FontColor(Colors.Grey.Medium);
                        });

                        row.ConstantItem(120).AlignRight().Column(footerColumn =>
                        {
                            footerColumn.Item().Text($"Page 1 of 1").FontSize(8).FontColor(Colors.Grey.Medium);
                            footerColumn.Item().Text($"Report ID: CR{DateTime.Now:yyyyMMdd}").FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });

                    column.Item().PaddingTop(5).Text("CONFIDENTIAL - For authorized personnel only")
                        .FontSize(8)
                        .Bold()
                        .FontColor(Colors.Red.Medium)
                        .AlignCenter();
                });
            }
        }







        [HttpPost]
        public IActionResult GeneratePayrollPDF([FromBody] List<int> claimIDs)
        {
            var model = new LecturerClaimViewModel();
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // Filter selected claims
            var selectedClaims = new List<ClaimItem>();
            for (int i = 0; i < model.ClaimIDs.Count; i++)
            {
                if (claimIDs.Contains(model.ClaimIDs[i]))
                {
                    selectedClaims.Add(new ClaimItem
                    {
                        ClaimID = model.ClaimIDs[i],
                        ModuleName = model.ModuleNames[i],
                        NumberOfSessions = model.NumberOfSessions[i],
                        NumberOfHours = model.NumberOfHours[i],
                        Rate = model.Rates[i],
                        TotalAmount = model.TotalAmounts[i],
                        FacultyName = model.FacultyNames[i],
                        SupportingDocument = model.SupportingDocuments[i],
                        CreatingDate = model.CreatingDates[i]
                    });
                }
            }

            if (!selectedClaims.Any())
                return BadRequest("No claims selected");

            var lecturerIndex = model.ClaimIDs.IndexOf(claimIDs.First());
            var lecturerName = model.FullNames[lecturerIndex];
            var email = model.Emails[lecturerIndex];
            var faculty = model.FacultyNames[lecturerIndex];

            // PDF generation
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

                    // Modern header
                    page.Header().Element(ComposeHeader);

                    // Main content
                    page.Content().Element(c => ComposeContent(c, lecturerName, email, faculty, selectedClaims));

                    // Professional footer
                    page.Footer().Element(ComposeFooter);

                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Payroll_Report_{lecturerName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf");

            void ComposeHeader(IContainer container)
            {
                container.Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("PAYROLL REPORT")
                            .FontSize(20)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        column.Item().Text($"For: {lecturerName}")
                            .FontSize(12)
                            .SemiBold()
                            .FontColor(Colors.Grey.Darken1);

                        column.Item().Text($"Period: {DateTime.Now:MMMM yyyy}")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Medium);
                    });

                    row.ConstantItem(100).AlignRight().Column(column =>
                    {
                        column.Item().Text("HR DEPARTMENT")
                            .FontSize(9)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3)
                            .AlignRight();

                        column.Item().Text("Rosebank College")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Medium)
                            .AlignRight();

                        column.Item().Text($"Report Date: {DateTime.Now:dd/MM/yyyy}")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Medium)
                            .AlignRight();
                    });
                });
            }

            void ComposeContent(IContainer container, string name, string email, string faculty, List<ClaimItem> claims)
            {
                container.Column(column =>
                {
                    column.Spacing(15);

                    // Lecturer Information
                    column.Item().Background(Colors.Grey.Lighten3).Padding(12).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("LECTURER INFORMATION").Bold().FontSize(11).FontColor(Colors.Blue.Darken3);
                            col.Item().Text($"Name: {name}").FontSize(9);
                            col.Item().Text($"Email: {email}").FontSize(9);
                            col.Item().Text($"Faculty: {faculty}").FontSize(9);
                        });

                        row.ConstantItem(150).Column(col =>
                        {
                            col.Item().Text("PAYMENT SUMMARY").Bold().FontSize(11).FontColor(Colors.Blue.Darken3);
                            col.Item().Text($"Total Claims: {claims.Count}").FontSize(9);
                            col.Item().Text($"Total Hours: {claims.Sum(c => c.NumberOfHours)}").FontSize(9);
                            col.Item().Text($"Gross Amount: {claims.Sum(c => c.TotalAmount):C}").FontSize(9).SemiBold();
                        });
                    });

                    // Claims Table
                    column.Item().Element(ComposeClaimsTable);

                    // Final Summary
                    column.Item().Element(ComposeFinalSummary);
                });

                void ComposeClaimsTable(IContainer container)
                {
                    container.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // ID
                            columns.RelativeColumn(2);   // Module
                            columns.ConstantColumn(50);  // Sessions
                            columns.ConstantColumn(50);  // Hours
                            columns.ConstantColumn(60);  // Rate
                            columns.ConstantColumn(70);  // Total
                        });

                        // Table Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Darken3).Padding(8).Text("ID").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(8).Text("MODULE NAME").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(8).Text("SESSIONS").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(8).Text("HOURS").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(8).Text("RATE").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(8).Text("AMOUNT").FontColor(Colors.White).Bold().FontSize(9);
                        });

                        // Table Rows
                        for (int i = 0; i < claims.Count; i++)
                        {
                            var claim = claims[i];
                            var backgroundColor = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                            table.Cell().Background(backgroundColor).Padding(6).Text(claim.ClaimID.ToString()).FontSize(8);
                            table.Cell().Background(backgroundColor).Padding(6).Text(claim.ModuleName).FontSize(8);
                            table.Cell().Background(backgroundColor).Padding(6).Text(claim.NumberOfSessions.ToString()).FontSize(8).AlignRight();
                            table.Cell().Background(backgroundColor).Padding(6).Text(claim.NumberOfHours.ToString()).FontSize(8).AlignRight();
                            table.Cell().Background(backgroundColor).Padding(6).Text(claim.Rate.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"))).FontSize(8).AlignRight();
                            table.Cell().Background(backgroundColor).Padding(6).Text(claim.TotalAmount.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"))).FontSize(8).AlignRight().SemiBold();
                        }
                    });
                }

                void ComposeFinalSummary(IContainer container)
                {
                    var totalAmount = claims.Sum(c => c.TotalAmount);
                    var totalHours = claims.Sum(c => c.NumberOfHours);
                    var averageRate = totalHours > 0 ? totalAmount / totalHours : 0;

                    container.Background(Colors.Blue.Lighten5).Padding(15).Column(summaryColumn =>
                    {
                        summaryColumn.Spacing(8);

                        summaryColumn.Item().Text("PAYROLL SUMMARY").Bold().FontSize(12).FontColor(Colors.Blue.Darken3);

                        summaryColumn.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Total Claims Processed:");
                            row.ConstantItem(100).Text(claims.Count.ToString()).AlignRight().SemiBold();
                        });

                        summaryColumn.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Total Hours Worked:");
                            row.ConstantItem(100).Text(totalHours.ToString()).AlignRight().SemiBold();
                        });

                        summaryColumn.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Average Hourly Rate:");
                            row.ConstantItem(100).Text(averageRate.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"))).AlignRight().SemiBold();
                        });

                        summaryColumn.Item().BorderTop(1).BorderColor(Colors.Blue.Medium).PaddingTop(5).Row(row =>
                        {
                            row.RelativeItem().Text("TOTAL GROSS PAYMENT:").Bold().FontSize(11);
                            row.ConstantItem(100).Text(totalAmount.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"))).Bold().FontSize(11).AlignRight().FontColor(Colors.Green.Darken2);
                        });

                        summaryColumn.Item().PaddingTop(10).Text("Payment will be processed within 7-10 working days.")
                            .FontSize(8)
                            .Italic()
                            .FontColor(Colors.Grey.Medium);
                    });
                }
            }

            void ComposeFooter(IContainer container)
            {
                container.BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(10).Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(footerColumn =>
                        {
                            footerColumn.Item().Text("Human Resources Department").Bold().FontSize(8);
                            footerColumn.Item().Text("Rosebank College • Contact: hr@College.ac.za").FontSize(7).FontColor(Colors.Grey.Medium);
                        });

                        row.ConstantItem(120).AlignRight().Column(footerColumn =>
                        {
                            footerColumn.Item().Text($"Page 1 of 1").FontSize(7).FontColor(Colors.Grey.Medium);
                            footerColumn.Item().Text($"Generated: {DateTime.Now:dd MMM yyyy HH:mm}").FontSize(7).FontColor(Colors.Grey.Medium);
                        });
                    });

                    column.Item().PaddingTop(5).Text("Confidential Payroll Document - For Internal Use Only")
                        .FontSize(7)
                        .Italic()
                        .FontColor(Colors.Red.Medium)
                        .AlignCenter();
                });
            }
        }


        //invoices
        public IActionResult Invoices()
        {
            LecturerClaimViewModel load = new LecturerClaimViewModel();
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.id = HttpContext.Session.GetString("id");



            return View(load);
        }





        [HttpPost]
        public IActionResult GenerateInvoice([FromBody] List<int> claimIDs)
        {
            // Set QuestPDF license
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var model = new LecturerClaimViewModel();

            var selectedClaims = new List<ClaimItem>();
            for (int i = 0; i < model.ClaimIDs.Count; i++)
            {
                if (claimIDs.Contains(model.ClaimIDs[i]))
                {
                    selectedClaims.Add(new ClaimItem
                    {
                        ClaimID = model.ClaimIDs[i],
                        ModuleName = model.ModuleNames[i],
                        NumberOfSessions = model.NumberOfSessions[i],
                        NumberOfHours = model.NumberOfHours[i],
                        Rate = model.Rates[i],
                        TotalAmount = model.TotalAmounts[i],
                        FacultyName = model.FacultyNames[i],
                        SupportingDocument = model.SupportingDocuments[i],
                        CreatingDate = model.CreatingDates[i]
                    });
                }
            }

            if (!selectedClaims.Any())
                return BadRequest("No claims selected");

            var lecturerIndex = model.ClaimIDs.IndexOf(claimIDs.First());
            var lecturerName = model.FullNames[lecturerIndex];
            var email = model.Emails[lecturerIndex];
            var faculty = model.FacultyNames[lecturerIndex];

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

                    page.Header().Element(ComposeHeader);

                    page.Content().Element(c => ComposeContent(c, lecturerName, email, faculty, selectedClaims));

                    page.Footer().Element(ComposeFooter);

                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Payment_Invoice_{lecturerName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmm}.pdf");

            void ComposeHeader(IContainer container)
            {
                container.Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("PAYMENT INVOICE")
                            .FontSize(24)
                            .Bold()
                            .FontColor(Colors.Green.Darken3);

                        column.Item().Text($"INV#{DateTime.Now:yyyyMMdd}-{selectedClaims.First().ClaimID}")
                            .FontSize(12)
                            .FontColor(Colors.Grey.Medium);

                        column.Item().Height(10);

                        column.Item().Text("Lecturer Payment Statement")
                            .FontSize(14)
                            .SemiBold()
                            .FontColor(Colors.Grey.Darken2);
                    });

                    row.ConstantItem(150).AlignRight().Column(column =>
                    {


                        column.Item().Text("Rosebank College")
                            .FontSize(9)
                            .Bold()
                            .FontColor(Colors.Grey.Darken2)
                            .AlignCenter();

                        column.Item().Text("Higher Education Institution")
                            .FontSize(7)
                            .FontColor(Colors.Grey.Medium)
                            .AlignCenter();
                    });
                });
            }

            void ComposeContent(IContainer container, string name, string email, string faculty, List<ClaimItem> claims)
            {
                container.Column(column =>
                {
                    column.Spacing(20);

                    // Payment To Section (Lecturer)
                    column.Item().Background(Colors.Green.Lighten4).Padding(15).Column(infoColumn =>
                    {
                        infoColumn.Spacing(8);

                        infoColumn.Item().Text("PAYMENT TO").Bold().FontSize(12).FontColor(Colors.Green.Darken3);

                        infoColumn.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text(name).Bold().FontSize(12);
                                col.Item().Text(email).FontColor(Colors.Grey.Darken1);
                                col.Item().Text(faculty).FontColor(Colors.Grey.Darken1);
                                col.Item().Text($"Employee ID: {model.ClaimIDs[0]}").FontSize(9).FontColor(Colors.Grey.Medium);
                            });

                            row.ConstantItem(180).Column(col =>
                            {
                                col.Item().Text("Invoice Date:").SemiBold();
                                col.Item().Text(DateTime.Now.ToString("dd MMMM yyyy"));

                                col.Item().PaddingTop(5).Text("Payment Due:").SemiBold();
                                col.Item().Text(DateTime.Now.AddDays(14).ToString("dd MMMM yyyy")).FontColor(Colors.Green.Darken2);

                                col.Item().PaddingTop(5).Text("Payment Method:").SemiBold();
                                col.Item().Text("Electronic Transfer").FontColor(Colors.Grey.Darken1);
                            });
                        });
                    });

                    column.Item().Element(ComposeClaimsTable);

                    column.Item().Element(ComposePaymentSummary);

                    column.Item().Element(ComposePaymentInstructions);
                });

                void ComposeClaimsTable(IContainer container)
                {
                    container.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(60);  // Claim ID
                            columns.RelativeColumn(2);   // Module
                            columns.ConstantColumn(70);  // Sessions
                            columns.ConstantColumn(70);  // Hours
                            columns.ConstantColumn(80);  // Rate
                            columns.ConstantColumn(90);  // Total
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Green.Darken3).Padding(8).Text("CLAIM ID").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Green.Darken3).Padding(8).Text("MODULE").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Green.Darken3).Padding(8).Text("SESSIONS").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Green.Darken3).Padding(8).Text("HOURS").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Green.Darken3).Padding(8).Text("RATE").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Green.Darken3).Padding(8).Text("AMOUNT").FontColor(Colors.White).Bold();

                            header.Cell().ColumnSpan(6).PaddingTop(5);
                        });

                        for (int i = 0; i < claims.Count; i++)
                        {
                            var claim = claims[i];
                            var backgroundColor = i % 2 == 0 ? Colors.White : Colors.Green.Lighten5;

                            table.Cell().Background(backgroundColor).Padding(8).Text($"CL{claim.ClaimID}").FontSize(9);
                            table.Cell().Background(backgroundColor).Padding(8).Text(claim.ModuleName);
                            table.Cell().Background(backgroundColor).Padding(8).Text(claim.NumberOfSessions.ToString()).AlignRight();
                            table.Cell().Background(backgroundColor).Padding(8).Text($"{claim.NumberOfHours} hrs").AlignRight();
                            table.Cell().Background(backgroundColor).Padding(8).Text($"R {claim.Rate:F2}/hr").AlignRight();
                            table.Cell().Background(backgroundColor).Padding(8).Text($"R {claim.TotalAmount:F2}").AlignRight().SemiBold();

                            if (i < claims.Count - 1)
                            {
                                table.Cell().ColumnSpan(6).PaddingTop(2);
                            }
                        }
                    });
                }

                void ComposePaymentSummary(IContainer container)
                {
                    var totalAmount = claims.Sum(c => c.TotalAmount);
                    var tax = totalAmount * 0.25m;
                    var netPayment = totalAmount - tax;

                    container.Row(row =>
                    {
                        row.RelativeItem();

                        row.ConstantItem(220).Background(Colors.Green.Lighten4).Padding(15).Column(summaryColumn =>
                        {
                            summaryColumn.Spacing(8);

                            summaryColumn.Item().Text("PAYMENT SUMMARY").Bold().FontSize(12).FontColor(Colors.Green.Darken3);

                            summaryColumn.Item().Row(summaryRow =>
                            {
                                summaryRow.RelativeItem().Text("Gross Amount:");
                                summaryRow.ConstantItem(80).Text($"R {totalAmount:F2}").AlignRight();
                            });

                            summaryColumn.Item().Row(summaryRow =>
                            {
                                summaryRow.RelativeItem().Text("Tax Deduction (25%):");
                                summaryRow.ConstantItem(80).Text($"R {tax:F2}").AlignRight().FontColor(Colors.Red.Medium);
                            });

                            summaryColumn.Item().PaddingTop(8).BorderTop(1).BorderColor(Colors.Green.Medium).Row(summaryRow =>
                            {
                                summaryRow.RelativeItem().Text("NET PAYMENT:").Bold().FontSize(12);
                                summaryRow.ConstantItem(80).Text($"R {netPayment:F2}").Bold().FontSize(12).AlignRight().FontColor(Colors.Green.Darken3);
                            });

                            summaryColumn.Item().PaddingTop(5).Background(Colors.Green.Lighten5).Padding(5).Text("Approved for Payment")
                                .FontSize(9)
                                .Bold()
                                .FontColor(Colors.Green.Darken2)
                                .AlignCenter();
                        });
                    });
                }

                void ComposePaymentInstructions(IContainer container)
                {
                    container.Background(Colors.Grey.Lighten4).Padding(15).Column(termsColumn =>
                    {
                        termsColumn.Spacing(5);

                        termsColumn.Item().Text("PAYMENT INFORMATION").Bold().FontSize(10).FontColor(Colors.Green.Darken3);

                        termsColumn.Item().Text("• Payment will be processed within 14 working days")
                            .FontSize(8);

                        termsColumn.Item().Text("• Paid via electronic transfer to your registered bank account")
                            .FontSize(8);

                        termsColumn.Item().Text("• Tax certificate will be issued at financial year end")
                            .FontSize(8);

                        termsColumn.Item().Text("• Contact payroll@College.ac.za for payment queries")
                            .FontSize(8);

                        termsColumn.Item().Text("• Please ensure your banking details are up to date in the system")
                            .FontSize(8);
                    });
                }
            }

            void ComposeFooter(IContainer container)
            {
                container.BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(10).Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(footerColumn =>
                        {
                            footerColumn.Item().Text("College Finance Department").Bold().FontSize(9);
                            footerColumn.Item().Text("123 College Avenue, Academic City 2001").FontSize(8);
                            footerColumn.Item().Text("Tel: +27 11 123 4567 | Email: finance@College.ac.za").FontSize(8);
                        });

                        row.ConstantItem(150).AlignRight().Column(footerColumn =>
                        {
                            footerColumn.Item().Text($"Invoice: INV#{DateTime.Now:yyyyMMdd}-{selectedClaims.First().ClaimID}").FontSize(8).FontColor(Colors.Grey.Medium);
                            footerColumn.Item().Text($"Generated: {DateTime.Now:dd MMM yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });

                    column.Item().PaddingTop(5).Text("Thank you for your valuable contribution to our academic community!")
                        .FontSize(9)
                        .Italic()
                        .FontColor(Colors.Green.Medium)
                        .AlignCenter();
                });
            }
        }




        //end of pdf
























    }
}

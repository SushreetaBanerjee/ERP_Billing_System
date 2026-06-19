using ERPBilling.Models;
using ERPBilling.Service.Interface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ERPBilling.Service
{
    public class InvoicePdfService:IInvoicePdfService
    {
        // ── Company letterhead placeholders — replace with real values ──────
        // ── Company letterhead placeholders — replace with real values ──────
        private const string CompanyName = "Your Company Pvt. Ltd.";
        private const string CompanyAddress = "123 Business Park, Sector 5, Kolkata, West Bengal — 700091";
        private const string CompanyGstin = "19AAAAA0000A1Z5";
        private const string CompanyEmail = "billing@yourcompany.com";

        public byte[] GenerateInvoicePdf(Invoice invoice)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(36);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Calibri));

                    // ── HEADER ─────────────────────────────────────────────
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text(CompanyName).FontSize(18).Bold();
                                c.Item().Text(CompanyAddress).FontSize(9).FontColor(Colors.Grey.Darken1);
                                c.Item().Text($"GSTIN: {CompanyGstin}").FontSize(9).FontColor(Colors.Grey.Darken1);
                                c.Item().Text(CompanyEmail).FontSize(9).FontColor(Colors.Grey.Darken1);
                            });

                            row.ConstantItem(160).Column(c =>
                            {
                                c.Item().AlignRight().Text("TAX INVOICE").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
                                c.Item().AlignRight().Text(invoice.InvoiceNumber).FontSize(12).Bold();
                                c.Item().AlignRight().Text(StatusLabel(invoice.Status))
                                    .FontSize(10).Bold().FontColor(StatusColor(invoice.Status));
                            });
                        });

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    // ── CONTENT ────────────────────────────────────────────
                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        // Billed To + Invoice Meta
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("BILL TO").FontSize(9).Bold().FontColor(Colors.Grey.Darken2);
                                c.Item().PaddingTop(2).Text(invoice.Customer?.Name ?? "").FontSize(12).Bold();
                                c.Item().Text(invoice.Customer?.Address ?? "");
                                c.Item().Text(invoice.Customer?.Email ?? "");
                                c.Item().Text(invoice.Customer?.Phone ?? "");
                                if (!string.IsNullOrEmpty(invoice.Customer?.GSTNumber))
                                    c.Item().Text($"GSTIN: {invoice.Customer.GSTNumber}").Bold();
                            });

                            row.ConstantItem(180).Column(c =>
                            {
                                MetaRow(c, "Invoice Date", invoice.InvoiceDate.ToString("dd MMM yyyy"));
                                MetaRow(c, "Due Date", invoice.DueDate.ToString("dd MMM yyyy"));
                                MetaRow(c, "Status", invoice.Status.ToString());
                            });
                        });

                        col.Item().PaddingTop(20);

                        // ── LINE ITEMS TABLE ──────────────────────────────
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);   // Product
                                columns.RelativeColumn(1);   // Qty
                                columns.RelativeColumn(1.4f);// Unit Price
                                columns.RelativeColumn(1.4f);// Tax
                                columns.RelativeColumn(1.6f);// Line Total
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Product / Service");
                                header.Cell().Element(HeaderCell).AlignCenter().Text("Qty");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Unit Price");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Tax");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Line Total");

                                static IContainer HeaderCell(IContainer c) => c
                                    .DefaultTextStyle(x => x.Bold().FontColor(Colors.White))
                                    .Background(Colors.Blue.Darken2)
                                    .Padding(6);
                            });

                            // Data rows
                            bool alt = false;
                            foreach (var item in invoice.InvoiceItems)
                            {
                                var bg = alt ? Colors.Grey.Lighten4 : Colors.White;
                                alt = !alt;

                                table.Cell().Background(bg).Padding(6).Text(item.Product?.Name ?? "");
                                table.Cell().Background(bg).Padding(6).AlignCenter().Text(item.Quantity.ToString());
                                table.Cell().Background(bg).Padding(6).AlignRight().Text($"₹{item.UnitPrice:N2}");
                                table.Cell().Background(bg).Padding(6).AlignRight().Text($"₹{item.TaxAmount:N2}");
                                table.Cell().Background(bg).Padding(6).AlignRight().Text($"₹{item.LineTotal:N2}").Bold();
                            }
                        });

                        // ── TOTALS ─────────────────────────────────────────
                        decimal subtotal = invoice.TotalAmount - invoice.InvoiceItems.Sum(i => i.TaxAmount);
                        decimal totalTax = invoice.InvoiceItems.Sum(i => i.TaxAmount);
                        decimal cgst = Math.Round(totalTax / 2, 2);   // assumes intra-state split
                        decimal sgst = totalTax - cgst;
                        decimal amountPaid = invoice.Payments.Sum(p => p.AmountPaid);
                        decimal balanceDue = invoice.TotalAmount - amountPaid;

                        col.Item().PaddingTop(10).AlignRight().Width(260).Column(c =>
                        {
                            TotalRow(c, "Subtotal", subtotal, false);
                            TotalRow(c, "CGST", cgst, false);
                            TotalRow(c, "SGST", sgst, false);
                            c.Item().PaddingVertical(4).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                            TotalRow(c, "Grand Total", invoice.TotalAmount, true);
                            c.Item().PaddingTop(6);
                            TotalRow(c, "Amount Paid", amountPaid, false, Colors.Green.Darken1);
                            TotalRow(c, "Balance Due", balanceDue, true, balanceDue > 0 ? Colors.Red.Darken1 : Colors.Green.Darken1);
                        });

                        // ── NOTES ──────────────────────────────────────────
                        col.Item().PaddingTop(30).Text("Thank you for your business.")
                            .FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                        col.Item().Text("This is a system-generated invoice and does not require a physical signature.")
                            .FontSize(8).FontColor(Colors.Grey.Medium);
                    });

                    // ── FOOTER ─────────────────────────────────────────────
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Generated on ").FontSize(8).FontColor(Colors.Grey.Medium);
                        text.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            });

            return document.GeneratePdf();
        }

        // ── Helpers ──────────────────────────────────────────────────────

        private static void MetaRow(ColumnDescriptor col, string label, string value)
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Text(label).FontColor(Colors.Grey.Darken1);
                row.RelativeItem().AlignRight().Text(value).Bold();
            });
        }

        private static void TotalRow(ColumnDescriptor col, string label, decimal amount, bool emphasize, string? color = null)
        {
            col.Item().Row(row =>
            {
                var labelText = row.RelativeItem().Text(label)
                    .FontSize(emphasize ? 12 : 10)
                    .FontColor(color ?? (emphasize ? Colors.Black : Colors.Grey.Darken2));
                if (emphasize) labelText.Bold();

                var amountText = row.RelativeItem().AlignRight().Text($"₹{amount:N2}")
                    .FontSize(emphasize ? 12 : 10)
                    .FontColor(color ?? (emphasize ? Colors.Black : Colors.Grey.Darken2));
                if (emphasize) amountText.Bold();
            });
        }

        private static string StatusLabel(InvoiceStatus status) => status switch
        {
            InvoiceStatus.Paid => "PAID",
            InvoiceStatus.Unpaid => "UNPAID",
            InvoiceStatus.Partial => "PARTIALLY PAID",
            _ => status.ToString().ToUpper()
        };

        private static string StatusColor(InvoiceStatus status) => status switch
        {
            InvoiceStatus.Paid => Colors.Green.Darken2,
            InvoiceStatus.Unpaid => Colors.Red.Darken2,
            InvoiceStatus.Partial => Colors.Orange.Darken2,
            _ => Colors.Grey.Darken2
        };
    }
}

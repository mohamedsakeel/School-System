using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.AppCore.DTOs;
using SMS.AppCore.Interfaces;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Tables;
using System.Security.Claims;

namespace SMS.Web.Controllers
{
    [Authorize]
    public class OperationsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IEnterMarksRepository _enterMarksRepo;
        private readonly IStudentReportRepository _reportRepository;

        public OperationsController(IMapper mapper,
                                    IEnterMarksRepository enterMarksRepo,
                                    IStudentReportRepository studentReportRepo)
        {
            _mapper = mapper;
            _enterMarksRepo = enterMarksRepo;
            _reportRepository = studentReportRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ExamMarks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ClassSubjectMarksViewModel VM = new ClassSubjectMarksViewModel();

            var classesSubjects = await _enterMarksRepo.GetClassesSubjectsForTeacher(userId);
            VM.ClassSubjectMarkss = _mapper.Map<IEnumerable<ClassSubjectMarksDTO>>(classesSubjects);

            return View(VM);
        }

        public async Task<IActionResult> InitiateExam([FromBody] ExamInitiationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var examInititation = _mapper.Map<ExamInitiationDTO>(dto);
            examInititation.IsActive = true;
            examInititation.InitiatedBy = User.Identity.Name;
            examInititation.InitiatedDate = DateTime.Now;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EnterMarks(int classId, int CompPercentage)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if teacher is assigned to this class
            var isTeacherAssigned = await _enterMarksRepo.IsTeacherAssigned(userId);

            if (!isTeacherAssigned)
            {
                return Forbid(); // Prevent unauthorized access
            }

            var viewModel = await _enterMarksRepo.GetMarksEntryTableAsync(userId, classId);

            ViewBag.CompPercentage = CompPercentage;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMarks(SaveMarksDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool IsSaved = await _enterMarksRepo.SaveMarks(model);

            if (!IsSaved)
            {
                return BadRequest();
            }

            return RedirectToAction("EnterMarks", new { classId = model.ClassId });

        }

        //Generate Report
        [HttpGet("operations/student/{studentId}/class/{classId}")]
        public async Task<IActionResult> GenerateStudentReport(int studentId, int classId)
        {
            var studentReport = await _reportRepository.GetStudentReport(studentId, classId);
            if (studentReport == null) return NotFound("Student not found");

            var classReports = await _reportRepository.GetClassStudentReports(classId);

            // Assign ranks
            for (int i = 0; i < classReports.Count; i++)
            {
                classReports[i].Rank = i + 1;
            }
            studentReport.Rank = classReports.FirstOrDefault(s => s.StudentId == studentId)?.Rank ?? 0;

            var pdfBytes = GenerateStudentReportPDF(studentReport);
            return File(pdfBytes, "application/pdf", $"{studentReport.FullName}_Report.pdf");
        }

        [HttpGet("operations/class/{classId}")]
        public async Task<IActionResult> GenerateClassMasterReport(int classId)
        {
            var studentReports = await _reportRepository.GetClassStudentReports(classId);

            // Assign ranks
            for (int i = 0; i < studentReports.Count; i++)
            {
                studentReports[i].Rank = i + 1;
            }

            var pdfBytes = GenerateClassMasterSheetPDF(studentReports);
            return File(pdfBytes, "application/pdf", "Class_Master_Report.pdf");
        }

        /// <Private>
        /// Generate Report
        /// 
        private byte[] GenerateStudentReportPDF(StudentReportDTO studentReport)
        {
            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;

                // Define fonts
                PdfFont headingFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
                PdfFont subHeadingFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
                PdfFont bodyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
                PdfFont tableHeaderFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);

                // Starting Y position
                float y = 30;

                // School Name
                graphics.DrawString("XYZ School", headingFont, PdfBrushes.Black, new PointF(200, y));
                y += 30;

                // Report Title
                graphics.DrawString("1st Term Exam 2025 Report", subHeadingFont, PdfBrushes.Black, new PointF(180, y));
                y += 40;

                // Student Details
                graphics.DrawString($"Student Name: {studentReport.FullName}", bodyFont, PdfBrushes.Black, new PointF(50, y));
                y += 20;
                graphics.DrawString($"Grade: {studentReport.Marks}", bodyFont, PdfBrushes.Black, new PointF(50, y));
                y += 20;
                graphics.DrawString($"Admission No: {studentReport.StudentId}", bodyFont, PdfBrushes.Black, new PointF(50, y));
                y += 30;

                // Create a Table for Marks
                PdfLightTable marksTable = new PdfLightTable();
                marksTable.Style.ShowHeader = true;
                marksTable.Style.HeaderStyle.Font = tableHeaderFont;
                marksTable.Style.DefaultStyle.Font = bodyFont;
                marksTable.Columns.Add(new PdfColumn("Subject"));
                marksTable.Columns.Add(new PdfColumn("Marks"));

                // Add Marks Data to Table
                foreach (var subject in studentReport.Marks)
                {
                    marksTable.Rows.Add(new string[] { subject.Key, subject.Value?.ToString() ?? "N/A" });
                }

                // Draw the Table on the PDF
                marksTable.Draw(page, new PointF(50, y));
                y += (studentReport.Marks.Count * 20) + 40;

                // Total Marks, Average, and Rank
                graphics.DrawString($"Total Marks: {studentReport.TotalMarks}", bodyFont, PdfBrushes.Black, new PointF(50, y));
                y += 20;
                graphics.DrawString($"Average Marks: {studentReport.AverageMarks:F2}", bodyFont, PdfBrushes.Black, new PointF(50, y));
                y += 20;
                graphics.DrawString($"Rank: {studentReport.Rank}", bodyFont, PdfBrushes.Black, new PointF(50, y));

                // Save PDF to Memory Stream
                using (MemoryStream stream = new MemoryStream())
                {
                    document.Save(stream);
                    return stream.ToArray();
                }
            }
        }


        private byte[] GenerateClassMasterSheetPDF(List<StudentReportDTO> studentReports)
        {
            using (PdfDocument document = new PdfDocument())
            {
                // Set page to landscape and reduce margins for more space
                document.PageSettings.Orientation = PdfPageOrientation.Landscape;
                document.PageSettings.Margins.All = 20;

                // Create the first page
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;

                // Fonts (reduced sizes for better fit)
                PdfFont headingFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
                PdfFont subHeadingFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
                PdfFont tableHeaderFont = new PdfStandardFont(PdfFontFamily.Helvetica, 9, PdfFontStyle.Bold);
                PdfFont tableFont = new PdfStandardFont(PdfFontFamily.Helvetica, 8);

                float y = 20; // Starting Y position

                // School Name
                graphics.DrawString("XYZ School", headingFont, PdfBrushes.Black, new PointF(20, y));
                y += 25;

                // Report Title
                graphics.DrawString("1st Term Exam 2025 - Class Master Sheet", subHeadingFont, PdfBrushes.Black, new PointF(20, y));
                y += 20;

                // Subject headers
                var subjectNames = studentReports.SelectMany(s => s.Marks.Keys).Distinct().ToList();
                int totalColumns = subjectNames.Count + 3; // Student Name + Total + Rank

                // Dynamically adjust column width based on the number of subjects
                float columnWidth = Math.Max(50, (page.GetClientSize().Width - 40) / totalColumns);

                float x = 20;
                graphics.DrawString("Student Name", tableHeaderFont, PdfBrushes.Black, new PointF(x, y));
                x += columnWidth * 2; // Give more space for names

                foreach (var subject in subjectNames)
                {
                    graphics.DrawString(subject, tableHeaderFont, PdfBrushes.Black, new PointF(x, y));
                    x += columnWidth;
                }

                graphics.DrawString("Total", tableHeaderFont, PdfBrushes.Black, new PointF(x, y));
                x += columnWidth;
                graphics.DrawString("Rank", tableHeaderFont, PdfBrushes.Black, new PointF(x, y));

                y += 15;
                graphics.DrawLine(PdfPens.Black, new PointF(20, y), new PointF(x + columnWidth, y));
                y += 10;

                // Page constraints
                float maxY = page.GetClientSize().Height - 30;
                float rowHeight = 15;

                foreach (var student in studentReports)
                {
                    if (y + rowHeight > maxY)
                    {
                        // Create new page if needed
                        page = document.Pages.Add();
                        graphics = page.Graphics;
                        y = 20;

                        // Redraw headers
                        x = 20;
                        graphics.DrawString("Student Name", tableHeaderFont, PdfBrushes.Black, new PointF(x, y));
                        x += columnWidth * 2;

                        foreach (var subject in subjectNames)
                        {
                            graphics.DrawString(subject, tableHeaderFont, PdfBrushes.Black, new PointF(x, y));
                            x += columnWidth;
                        }

                        graphics.DrawString("Total", tableHeaderFont, PdfBrushes.Black, new PointF(x, y));
                        x += columnWidth;
                        graphics.DrawString("Rank", tableHeaderFont, PdfBrushes.Black, new PointF(x, y));

                        y += 15;
                        graphics.DrawLine(PdfPens.Black, new PointF(20, y), new PointF(x + columnWidth, y));
                        y += 10;
                    }

                    // Draw student details
                    x = 20;
                    graphics.DrawString(student.FullName, tableFont, PdfBrushes.Black, new PointF(x, y));
                    x += columnWidth * 2;

                    float totalMarks = 0;
                    foreach (var subject in subjectNames)
                    {
                        var mark = student.Marks.ContainsKey(subject) ? student.Marks[subject] : (float?)null;
                        totalMarks += mark ?? 0;
                        graphics.DrawString(mark?.ToString() ?? "N/A", tableFont, PdfBrushes.Black, new PointF(x, y));
                        x += columnWidth;
                    }

                    graphics.DrawString(totalMarks.ToString(), tableFont, PdfBrushes.Black, new PointF(x, y));
                    x += columnWidth;
                    graphics.DrawString(student.Rank.ToString(), tableFont, PdfBrushes.Black, new PointF(x, y));

                    y += rowHeight;
                }

                // Return PDF as byte array
                using (MemoryStream stream = new MemoryStream())
                {
                    document.Save(stream);
                    return stream.ToArray();
                }
            }
        }






    }
}

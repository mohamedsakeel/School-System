using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.AppCore.DTOs;
using SMS.AppCore.Interfaces;
using SMS.AppCore.Repositories;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
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

                // Fonts
                PdfFont headingFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
                PdfFont subHeadingFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
                PdfFont bodyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

                // Starting Y position
                float y = 20;

                // School Name
                graphics.DrawString("XYZ School", headingFont, PdfBrushes.Black, new PointF(20, y));
                y += 30;

                // Report Title
                graphics.DrawString("1st Term Exam 2025 Report", subHeadingFont, PdfBrushes.Black, new PointF(20, y));
                y += 30;

                // Student Name
                graphics.DrawString($"Student Name: {studentReport.FullName}", bodyFont, PdfBrushes.Black, new PointF(20, y));
                y += 20;

                // Marks for each subject
                foreach (var subject in studentReport.Marks)
                {
                    graphics.DrawString($"{subject.Key}: {subject.Value?.ToString() ?? "N/A"}", bodyFont, PdfBrushes.Black, new PointF(20, y));
                    y += 20;
                }

                // Total Marks, Average, and Rank
                y += 10;
                graphics.DrawString($"Total Marks: {studentReport.TotalMarks}", bodyFont, PdfBrushes.Black, new PointF(20, y));
                y += 20;
                graphics.DrawString($"Average Marks: {studentReport.AverageMarks:F2}", bodyFont, PdfBrushes.Black, new PointF(20, y));
                y += 20;
                graphics.DrawString($"Rank: {studentReport.Rank}", bodyFont, PdfBrushes.Black, new PointF(20, y));

                // Return the PDF as byte array
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
                // Set page orientation to landscape and apply margins
                document.PageSettings.Orientation = PdfPageOrientation.Landscape;
                document.PageSettings.Margins.All = 50;

                // Create the page and graphics object
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;

                // Fonts
                PdfFont headingFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
                PdfFont subHeadingFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
                PdfFont tableHeaderFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
                PdfFont tableFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);

                // Starting Y position
                float y = 20;

                // School Name
                graphics.DrawString("XYZ School", headingFont, PdfBrushes.Black, new PointF(20, y));
                y += 30;

                // Report Title
                graphics.DrawString("1st Term Exam 2025 - Class Master Sheet", subHeadingFont, PdfBrushes.Black, new PointF(20, y));
                y += 30;

                // Table Header
                graphics.DrawString("Student Name", tableHeaderFont, PdfBrushes.Black, new PointF(20, y));
                float columnStartX = 200;

                // Column for each subject
                var subjectNames = studentReports.SelectMany(s => s.Marks.Keys).Distinct().ToList();
                foreach (var subject in subjectNames)
                {
                    graphics.DrawString(subject, tableHeaderFont, PdfBrushes.Black, new PointF(columnStartX, y));
                    columnStartX += 100;  // Adjust width for each subject column
                }

                // Total and Rank columns
                graphics.DrawString("Total", tableHeaderFont, PdfBrushes.Black, new PointF(columnStartX, y));
                columnStartX += 100;
                graphics.DrawString("Rank", tableHeaderFont, PdfBrushes.Black, new PointF(columnStartX, y));
                y += 20;

                // Draw a horizontal line after the header
                graphics.DrawLine(PdfPens.Black, new PointF(20, y), new PointF(columnStartX + 100, y));
                y += 10;

                // Adjust the maximum Y position on the page
                float maxY = page.GetClientSize().Height - 50; // Leave some space at the bottom for margin
                float lineHeight = 20; // Height of each row

                // Loop through each student and display their data in a table format
                foreach (var student in studentReports)
                {
                    // Check if we need to move to the next page
                    if (y + lineHeight > maxY)
                    {
                        // Create a new page in landscape if the current page is full
                        page = document.Pages.Add();
                        graphics = page.Graphics;
                        y = 20; // Reset the Y position for the new page

                        // Redraw the table header (for new page)
                        graphics.DrawString("Student Name", tableHeaderFont, PdfBrushes.Black, new PointF(20, y));
                        columnStartX = 200;

                        foreach (var subject in subjectNames)
                        {
                            graphics.DrawString(subject, tableHeaderFont, PdfBrushes.Black, new PointF(columnStartX, y));
                            columnStartX += 100;
                        }

                        graphics.DrawString("Total", tableHeaderFont, PdfBrushes.Black, new PointF(columnStartX, y));
                        columnStartX += 100;
                        graphics.DrawString("Rank", tableHeaderFont, PdfBrushes.Black, new PointF(columnStartX, y));
                        y += 20;
                        graphics.DrawLine(PdfPens.Black, new PointF(20, y), new PointF(columnStartX + 100, y));
                        y += 10;
                    }

                    float x = 20;
                    graphics.DrawString(student.FullName, tableFont, PdfBrushes.Black, new PointF(x, y));
                    x += 180;

                    // Display marks for each subject
                    float totalMarks = 0;
                    foreach (var subject in subjectNames)
                    {
                        var mark = student.Marks.ContainsKey(subject) ? student.Marks[subject] : (float?)null;
                        totalMarks += mark ?? 0;
                        graphics.DrawString(mark?.ToString() ?? "N/A", tableFont, PdfBrushes.Black, new PointF(x, y));
                        x += 100;
                    }

                    // Total and Rank columns
                    graphics.DrawString(totalMarks.ToString(), tableFont, PdfBrushes.Black, new PointF(x, y));
                    x += 100;
                    graphics.DrawString(student.Rank.ToString(), tableFont, PdfBrushes.Black, new PointF(x, y));
                    y += lineHeight;
                }

                // Return the PDF as byte array
                using (MemoryStream stream = new MemoryStream())
                {
                    document.Save(stream);
                    return stream.ToArray();
                }
            }
        }





    }
}

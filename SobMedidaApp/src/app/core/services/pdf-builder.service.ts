import { Injectable } from '@angular/core';
import jsPDF from 'jspdf';
import { GeneratedResume, ResumeContent } from '../models/profile.model';

@Injectable({
    providedIn: 'root'
})
export class PdfBuilderService {

    generate(resume: GeneratedResume): void {
        let content: ResumeContent;

        try {
            content = JSON.parse(resume.generatedContent);
        } catch {
            alert('Não foi possível gerar o PDF. O conteúdo do currículo está em formato inválido.');
            return;
        }

        const doc = new jsPDF({ unit: 'mm', format: 'a4' });
        const pageWidth = doc.internal.pageSize.getWidth();
        const pageHeight = doc.internal.pageSize.getHeight();
        const marginLeft = 20;
        const marginRight = 20;
        const contentWidth = pageWidth - marginLeft - marginRight;
        let y = 0;

        const checkPageBreak = (space: number) => {
            if (y + space > pageHeight - 15) {
                doc.addPage();
                y = 20;
            }
        };

        const drawSectionHeader = (title: string) => {
            checkPageBreak(12);
            y += 5;
            doc.setFont('helvetica', 'bold');
            doc.setFontSize(11);
            doc.setTextColor(0, 0, 0);
            doc.text(title.toUpperCase(), marginLeft, y);
            y += 3;
            doc.setDrawColor(0, 0, 0);
            doc.setLineWidth(0.4);
            doc.line(marginLeft, y, pageWidth - marginRight, y);
            y += 5;
        };

        const drawText = (text: string, indent = 0, bold = false, size = 10) => {
            doc.setFont('helvetica', bold ? 'bold' : 'normal');
            doc.setFontSize(size);
            doc.setTextColor(50, 50, 50);
            const lines = doc.splitTextToSize(text, contentWidth - indent);
            lines.forEach((line: string) => {
                checkPageBreak(6);
                doc.text(line, marginLeft + indent, y);
                y += 5.5;
            });
        };

        const drawBullet = (text: string) => {
            checkPageBreak(6);
            doc.setFont('helvetica', 'normal');
            doc.setFontSize(10);
            doc.setTextColor(50, 50, 50);
            doc.text('•', marginLeft + 2, y);
            const lines = doc.splitTextToSize(text, contentWidth - 8);
            lines.forEach((line: string) => {
                checkPageBreak(6);
                doc.text(line, marginLeft + 8, y);
                y += 5.5;
            });
        };

        // Header — Name
        y = 22;
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(20);
        doc.setTextColor(0, 0, 0);
        doc.text(content.personalInfo.fullName, pageWidth / 2, y, { align: 'center' });
        y += 7;

        // Contact line
        const contactParts = [
            content.personalInfo.email,
            content.personalInfo.phone,
            content.personalInfo.location
        ].filter(Boolean);

        doc.setFont('helvetica', 'normal');
        doc.setFontSize(9);
        doc.setTextColor(80, 80, 80);
        doc.text(contactParts.join(' | '), pageWidth / 2, y, { align: 'center' });
        y += 5;

        // Links line
        const linkParts = [
            content.personalInfo.linkedInUrl,
            content.personalInfo.gitHubUrl
        ].filter(Boolean);

        if (linkParts.length > 0) {
            doc.text(linkParts.join(' | '), pageWidth / 2, y, { align: 'center' });
            y += 5;
        }

        // Divider
        doc.setDrawColor(0, 0, 0);
        doc.setLineWidth(0.8);
        doc.line(marginLeft, y, pageWidth - marginRight, y);
        y += 6;

        // Summary
        if (content.summary) {
            drawSectionHeader('Resumo Profissional');
            drawText(content.summary);
            y += 2;
        }

        // Experience
        if (content.experience?.length > 0) {
            drawSectionHeader('Experiência Profissional');
            content.experience.forEach(exp => {
                checkPageBreak(10);
                drawText(`${exp.jobTitle} — ${exp.company}`, 0, true);
                drawText(`${exp.location} | ${exp.period}`, 0, false, 9);
                y += 1;
                drawText(exp.description, 4);
                y += 3;
            });
        }

        // Education
        if (content.education?.length > 0) {
            drawSectionHeader('Formação Acadêmica');
            content.education.forEach(edu => {
                checkPageBreak(10);
                drawText(edu.degree, 0, true);
                drawText(`${edu.institution} | ${edu.location} | ${edu.period}`, 0, false, 9);
                y += 3;
            });
        }

        // Skills
        if (content.skills?.length > 0) {
            drawSectionHeader('Habilidades');
            content.skills.forEach(group => {
                checkPageBreak(7);
                drawText(`${group.category}: ${group.items}`);
            });
            y += 2;
        }

        // Projects
        if (content.projects?.length > 0) {
            drawSectionHeader('Projetos');
            content.projects.forEach(proj => {
                checkPageBreak(10);
                drawText(`${proj.name} | ${proj.technologies}`, 0, true);
                drawText(proj.description, 4);
                y += 3;
            });
        }

        // Certifications
        if (content.certifications?.length > 0) {
            drawSectionHeader('Certificações');
            content.certifications.forEach(cert => {
                checkPageBreak(7);
                drawBullet(`${cert.name} — ${cert.issuingOrganization} (${cert.date})`);
            });
        }

        const filename = `curriculo-${resume.jobTitle.replace(/\s+/g, '-').toLowerCase()}.pdf`;
        doc.save(filename);
    }
}
import { Injectable } from '@angular/core';
import { GeneratedResume, ResumeContent } from '../models/profile.model';

@Injectable({
    providedIn: 'root'
})
export class ResumeParserService {
    parse(resume: GeneratedResume): GeneratedResume {
        try {
            const parsed: ResumeContent = JSON.parse(resume.generatedContent);
            return { ...resume, parsedContent: parsed };
        } catch {
            return resume;
        }
    }
}
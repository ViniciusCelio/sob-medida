import { PersonalInfo, Experience, Education, Skill, Project, Certification } from './profile.model';

export interface ImportedProfile {
  personalInfo: PersonalInfo | null;
  experiences: Experience[];
  educations: Education[];
  skills: Skill[];
  projects: Project[];
  certifications: Certification[];
}

export interface ImportSummary {
  extractedData: ImportedProfile;
  newExperiences: number;
  updatedExperiences: number;
  newEducations: number;
  newSkills: number;
  newProjects: number;
  newCertifications: number;
  personalInfoUpdated: boolean;
}
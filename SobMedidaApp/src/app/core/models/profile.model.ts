export interface Address {
  street: string;
  number: string;
  neighborhood: string;
  city: string;
  state: string;
  zipCode: string;
}

export interface PersonalInfo {
  id?: number;
  fullName: string;
  email: string;
  phone: string;
  address: Address;
  linkedInUrl: string;
  gitHubUrl: string;
  summary: string;
}

export interface Experience {
  id?: number;
  jobTitle: string;
  company: string;
  location: string;
  startDate: string | null;
  endDate: string | null;
  isCurrentJob: boolean;
  description: string;
}

export interface Education {
  id?: number;
  degree: string;
  fieldOfStudy: string;
  institution: string;
  location: string;
  startDate: string | null;
  endDate: string | null;
  isCurrentlyStudying: boolean;
  description: string;
}

export interface Skill {
  id?: number;
  name: string;
  category: string;
  level: string;
}

export interface Project {
  id?: number;
  name: string;
  description: string;
  technologies: string;
  projectUrl: string;
  gitHubUrl: string;
  startDate: string | null;
  endDate: string | null;
}

export interface Certification {
  id?: number;
  name: string;
  issuingOrganization: string;
  issueDate: string | null;
  expirationDate: string | null;
  doesNotExpire: boolean;
  credentialUrl: string;
}

export interface GenerateResumeRequest {
  jobTitle: string;
  jobDescription: string;
}

export interface GeneratedResume {
  id: number;
  jobTitle: string;
  jobDescription: string;
  generatedContent: string;
  createdAt: string;
}

export interface ResumePersonalInfo {
  fullName: string;
  email: string;
  phone: string;
  location: string;
  linkedInUrl: string;
  gitHubUrl: string;
}

export interface ResumeExperience {
  jobTitle: string;
  company: string;
  location: string;
  period: string;
  description: string;
}

export interface ResumeEducation {
  degree: string;
  institution: string;
  location: string;
  period: string;
}

export interface ResumeSkillGroup {
  category: string;
  items: string;
}

export interface ResumeProject {
  name: string;
  technologies: string;
  description: string;
}

export interface ResumeCertification {
  name: string;
  issuingOrganization: string;
  date: string;
}

export interface ResumeContent {
  personalInfo: ResumePersonalInfo;
  summary: string;
  experience: ResumeExperience[];
  education: ResumeEducation[];
  skills: ResumeSkillGroup[];
  projects: ResumeProject[];
  certifications: ResumeCertification[];
}

export interface GeneratedResume {
  id: number;
  jobTitle: string;
  jobDescription: string;
  generatedContent: string;
  parsedContent?: ResumeContent;
  createdAt: string;
}
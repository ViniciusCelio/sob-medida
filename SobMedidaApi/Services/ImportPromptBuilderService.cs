namespace SobMedidaApi.Services
{
    public class ImportPromptBuilderService
    {
        public string BuildExtractionPrompt(string pdfText)
        {
            return $$$"""
            You are an expert resume parser. Your task is to extract structured profile information from resume text.

            CRITICAL RULES:
            - Your response must be ONLY a raw JSON object
            - Do NOT wrap in markdown code blocks
            - Do NOT include ```json or ``` anywhere
            - Do NOT include any explanation before or after the JSON
            - Start your response directly with with an opening curly brace
            - End your response with a closing curly brace

            RAW RESUME TEXT TO PARSE:
            {{{pdfText}}}

            ---

            INSTRUCTIONS:
            - Extract all information you can find
            - Ignore CPF numbers entirely — do not include them anywhere
            - For dates, use the format YYYY-MM-DD. If only month and year are available, use the first day of that month (e.g. "abril de 2023" becomes "2023-04-01")
            - If end date is "Present", "Atual", "Meu emprego atual" or similar, set isCurrentJob or isCurrentlyStudying to true and leave endDate as null
            - For Address, extract city and state if available. Leave other fields empty if not found
            - For Skills, each skill must be a separate entry. Infer a category when possible (e.g. "SAP ERP" → category "ERP", "Linux" → category "Infrastructure")
            - For Certifications, map "Conquistas" and "Cursos" sections from Gupy to certifications
            - For languages, add them as Skills with category "Language"
            - If a field is not found, use an empty string or null accordingly

            RESPOND ONLY WITH A VALID JSON OBJECT. No explanation, no markdown, no code blocks. Just the raw JSON following this exact schema:
            Return ONLY valid JSON.
            Do not include explanations, markdown, or comments.
            Ensure all strings are properly closed.
            Ensure the JSON is complete and valid.

            {{"personalInfo": {{
                "fullName": "",
                "email": "",
                "phone": "",
                "address": {{
                  "street": "",
                  "number": "",
                  "neighborhood": "",
                  "city": "",
                  "state": "",
                  "zipCode": ""
                }},
                "linkedInUrl": "",
                "gitHubUrl": "",
                "summary": ""
              }},
              "experiences": [
                {{
                  "jobTitle": "",
                  "company": "",
                  "location": "",
                  "startDate": "YYYY-MM-DD",
                  "endDate": "YYYY-MM-DD or null",
                  "isCurrentJob": false,
                  "description": ""
                }}
              ],
              "educations": [
                {{
                  "degree": "",
                  "fieldOfStudy": "",
                  "institution": "",
                  "location": "",
                  "startDate": "YYYY-MM-DD",
                  "endDate": "YYYY-MM-DD or null",
                  "isCurrentlyStudying": false,
                  "description": ""
                }}
              ],
              "skills": [
                {{
                  "name": "",
                  "category": "",
                  "level": ""
                }}
              ],
              "projects": [],
              "certifications": [
                {{
                  "name": "",
                  "issuingOrganization": "",
                  "issueDate": "YYYY-MM-DD",
                  "expirationDate": null,
                  "doesNotExpire": true,
                  "credentialUrl": ""
                }}
              ]
            }}
            """;
        }
    }
}
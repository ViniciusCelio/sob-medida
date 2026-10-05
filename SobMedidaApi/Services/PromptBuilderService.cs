using Microsoft.EntityFrameworkCore;
using SobMedidaApi.Data;
using SobMedidaApi.Models;

namespace SobMedidaApi.Services
{
  public class PromptBuilderService
  {
    private readonly AppDbContext _context;

    public PromptBuilderService(AppDbContext context)
    {
      _context = context;
    }

    public async Task<string> BuildPrompt(string userId, string jobTitle, ExtractedJobInfo jobInfo)
    {
      var profileBlock = await BuildProfileBlock(userId);

      return $$"""
        Você é um especialista em recrutamento e seleção com amplo conhecimento em sistemas ATS (Applicant Tracking Systems) e redação de currículos profissionais para o mercado brasileiro.

        Sua tarefa é gerar um currículo personalizado para um candidato que está se candidatando à seguinte vaga:

        TÍTULO DA VAGA: {{jobTitle}}

        REQUISITOS OBRIGATÓRIOS DA VAGA:
        {{string.Join(", ", jobInfo.RequiredSkills)}}

        REQUISITOS DESEJÁVEIS DA VAGA:
        {{string.Join(", ", jobInfo.DesiredSkills)}}

        RESPONSABILIDADES DO CARGO:
        {{string.Join("\n- ", jobInfo.Responsibilities)}}

        PALAVRAS-CHAVE PARA ATS:
        {{string.Join(", ", jobInfo.AtsKeywords)}}

        NÍVEL DE EXPERIÊNCIA EXIGIDO: {{jobInfo.ExperienceLevel}}
        FORMAÇÃO EXIGIDA: {{jobInfo.EducationRequired}}

        ---

        PERFIL DO CANDIDATO (esta é a ÚNICA fonte de informação — jamais invente dados que não estejam presentes aqui):
        {{profileBlock}}

        ---

        INSTRUÇÕES — siga estas etapas antes de escrever:

        Etapa 1 - ANALISE a descrição da vaga e identifique as competências exigidas, responsabilidades principais e palavras-chave relevantes para ATS.
        Etapa 2 - MAPEIE o perfil do candidato em relação aos requisitos da vaga, identificando as experiências e habilidades mais relevantes para esta oportunidade.
        Etapa 3 - REDIJA o currículo utilizando APENAS as informações presentes no perfil do candidato. Nunca fabrique dados, experiências ou habilidades inexistentes.

        REGRAS CRÍTICAS DE SAÍDA:
        - Responda APENAS com um objeto JSON puro, sem nenhum texto adicional
        - NÃO utilize blocos de markdown
        - NÃO utilize crases ou cercas de código
        - Inicie a resposta diretamente com uma chave de abertura
        - Finalize a resposta com uma chave de fechamento
        - Todo o conteúdo textual do JSON deve estar em português brasileiro
        - Não utilize nenhuma palavra em inglês no conteúdo do currículo gerado

        Utilize exatamente este esquema JSON:

        {
          "personalInfo": {
            "fullName": "",
            "email": "",
            "phone": "",
            "location": "",
            "linkedInUrl": "",
            "gitHubUrl": ""
          },
          "summary": "",
          "experience": [
            {
              "jobTitle": "",
              "company": "",
              "location": "",
              "period": "",
              "description": ""
            }
          ],
          "education": [
            {
              "degree": "",
              "institution": "",
              "location": "",
              "period": ""
            }
          ],
          "skills": [
            {
              "category": "",
              "items": ""
            }
          ],
          "projects": [
            {
              "name": "",
              "technologies": "",
              "description": ""
            }
          ],
          "certifications": [
            {
              "name": "",
              "issuingOrganization": "",
              "date": ""
            }
          ]
        }
        """;
    }
    private string Truncate(string text, int maxLength = 300)
    {
      if (string.IsNullOrWhiteSpace(text)) return string.Empty;
      return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
    }

    public string BuildJobExtractionPrompt(string jobDescription)
    {
      return $$"""
        Você é um especialista em recrutamento e seleção com amplo conhecimento no mercado brasileiro de tecnologia.

        Sua tarefa é analisar a descrição bruta de uma vaga de emprego e extrair APENAS as informações relevantes para a elaboração de um currículo profissional.

        DESCRIÇÃO BRUTA DA VAGA:
        {{jobDescription}}

        ---

        INSTRUÇÕES:
        - Ignore completamente informações sobre salário, benefícios, cultura da empresa, missão, valores, localização do escritório e outras informações institucionais
        - Foque exclusivamente no que o candidato precisa ter ou fazer
        - Se uma informação não estiver presente na descrição, deixe o campo como uma lista vazia ou string vazia
        - Responda APENAS com um objeto JSON puro, sem markdown, sem crases, sem explicações
        - Inicie a resposta diretamente com uma chave de abertura

        Use exatamente este esquema JSON:

        {
          "jobTitle": "",
          "requiredSkills": ["", ""],
          "desiredSkills": ["", ""],
          "responsibilities": ["", ""],
          "atsKeywords": ["", ""],
          "experienceLevel": "",
          "educationRequired": ""
        }

        Onde:
        - jobTitle: título do cargo conforme descrito na vaga
        - requiredSkills: competências e habilidades obrigatórias mencionadas explicitamente
        - desiredSkills: competências mencionadas como diferenciais ou desejáveis
        - responsibilities: principais responsabilidades e atividades do cargo
        - atsKeywords: palavras-chave técnicas e termos relevantes para sistemas ATS presentes na vaga
        - experienceLevel: nível de experiência exigido (Júnior, Pleno, Sênior ou Não especificado)
        - educationRequired: formação acadêmica exigida ou Não especificado
        """;
    }

    public string BuildValidationPrompt(string profileBlock, string generatedResumeJson)
    {
      return $$"""
    Você é um auditor especializado em verificação de currículos profissionais.

    Sua tarefa é verificar se um currículo gerado por inteligência artificial é fiel ao perfil original do candidato, identificando apenas casos reais de invenção de dados (alucinação) — e não meras reformulações de texto.

    PERFIL ORIGINAL DO CANDIDATO:
    {{profileBlock}}

    ---

    CURRÍCULO GERADO (em formato JSON):
    {{generatedResumeJson}}

    ---

    DISTINÇÃO FUNDAMENTAL — leia com atenção antes de aplicar as regras:

    O currículo gerado é uma redação profissional do perfil original, não uma cópia literal. É esperado e desejável que o texto seja reformulado, resumido, reorganizado ou enriquecido estilisticamente. Isso NÃO é alucinação. Alucinação é apenas a introdução de um FATO que não existe no perfil original (uma empresa, cargo, curso, habilidade, projeto ou certificação que o candidato nunca informou).

    NÃO são violações (considere IsValid mesmo com essas diferenças):
    - Sinônimos ou termos equivalentes (ex.: perfil diz "atendimento ao cliente" e currículo diz "suporte a clientes")
    - Agrupamento ou reclassificação de habilidades em categorias diferentes das do perfil (ex.: perfil lista "Python" solto e currículo agrupa em "Linguagens de Programação: Python")
    - Reformulação de descrições de experiência/projetos com linguagem mais formal ou orientada a resultados, desde que não insira fatos novos (números, ferramentas, responsabilidades) que não estejam implícitos no perfil
    - Mudança de formato de datas (ex.: "mar 2021" vs "março de 2021" vs "2021-03")
    - Resumo ou compressão de um texto longo em uma versão mais curta, desde que preserve o sentido
    - Pequenas variações ortográficas ou de acentuação

    SÃO violações reais (marcar isValid como false e listar):
    1. CARGOS E EMPRESAS INEXISTENTES: menção a cargo ou empresa que não consta, de forma alguma, no perfil original.
    2. HABILIDADES INEXISTENTES: habilidade técnica ou ferramenta específica que não foi informada em nenhum lugar do perfil.
    3. FORMAÇÃO INEXISTENTE: instituição de ensino, curso ou grau que não conste no perfil original.
    4. PROJETOS INEXISTENTES: projeto que não foi informado no perfil.
    5. CERTIFICAÇÕES INEXISTENTES: certificação que não foi informada no perfil.
    6. NOME DIVERGENTE: nome do candidato claramente diferente do informado no perfil (não conte variações menores de formatação, como abreviação de sobrenome do meio).
    7. DATAS CONTRADITÓRIAS: datas de início/fim que contradizem factualmente o perfil (não conte apenas mudança de formato).
    8. MÉTRICAS OU RESULTADOS INVENTADOS: números, percentuais ou resultados quantitativos que não estejam no perfil original.

    INSTRUÇÕES:
    - Compare o SIGNIFICADO de cada campo do currículo gerado com o perfil original, não o texto literal
    - Só marque uma violação quando houver um fato concreto no currículo que não tenha base alguma no perfil — reformulação, síntese ou embelezamento estilístico não conta
    - Na dúvida entre "é uma reformulação aceitável" e "é uma invenção", prefira considerar como reformulação aceitável, a menos que o fato novo seja claramente verificável e ausente do perfil
    - Responda APENAS com um objeto JSON puro, sem markdown, sem crases, sem explicações
    - Inicie a resposta diretamente com uma chave de abertura

    Use exatamente este esquema JSON:

    {
      "isValid": true,
      "violations": []
    }

    Onde:
    - isValid: true se nenhuma violação real foi encontrada, false caso contrário
    - violations: lista de strings descrevendo cada violação encontrada em português brasileiro, citando o fato inventado especificamente. Se não houver violações, retorne uma lista vazia.
    """;
    }

    public async Task<string> BuildProfileBlock(string userId)
    {
      var personalInfo = await _context.PersonalInfos
          .FirstOrDefaultAsync(p => p.UserId == userId);

      var experiences = await _context.Experiences
          .Where(e => e.UserId == userId)
          .OrderByDescending(e => e.StartDate)
          .ToListAsync();

      var educations = await _context.Educations
          .Where(e => e.UserId == userId)
          .OrderByDescending(e => e.StartDate)
          .ToListAsync();

      var skills = await _context.Skills
          .Where(s => s.UserId == userId)
          .ToListAsync();

      var projects = await _context.Projects
          .Where(p => p.UserId == userId)
          .OrderByDescending(p => p.StartDate)
          .ToListAsync();

      var certifications = await _context.Certifications
          .Where(c => c.UserId == userId)
          .OrderByDescending(c => c.IssueDate)
          .ToListAsync();

      var profileBlock = "";

      if (personalInfo != null)
      {
        var city = personalInfo.Address?.City ?? string.Empty;
        var state = personalInfo.Address?.State ?? string.Empty;
        var location = (city + (string.IsNullOrWhiteSpace(state) ? "" : $", {state}")).Trim();

        var contactParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(personalInfo.Email)) contactParts.Add(personalInfo.Email);
        if (!string.IsNullOrWhiteSpace(personalInfo.Phone)) contactParts.Add(personalInfo.Phone);
        if (!string.IsNullOrWhiteSpace(location)) contactParts.Add(location);

        var linkParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(personalInfo.LinkedInUrl)) linkParts.Add(personalInfo.LinkedInUrl);
        if (!string.IsNullOrWhiteSpace(personalInfo.GitHubUrl)) linkParts.Add(personalInfo.GitHubUrl);

        profileBlock += $"""
        === INFORMAÇÕES PESSOAIS ===
        Nome: {personalInfo.FullName}
        Contato: {string.Join(" | ", contactParts)}
        Links: {string.Join(" | ", linkParts)}
        Resumo: {personalInfo.Summary}

        """;
      }

      if (experiences.Any())
      {
        profileBlock += "=== EXPERIÊNCIAS PROFISSIONAIS ===\n";
        foreach (var exp in experiences)
        {
          var endDate = exp.IsCurrentJob ? "Atual" : exp.EndDate?.ToString("MMM yyyy");
          profileBlock += $"""
            - {exp.JobTitle} na {exp.Company} ({exp.Location})
              Período: {exp.StartDate:MMM yyyy} até {endDate}
              Descrição: {Truncate(exp.Description)}

            """;
        }
      }

      if (educations.Any())
      {
        profileBlock += "=== FORMAÇÃO ACADÊMICA ===\n";
        foreach (var edu in educations)
        {
          var endDate = edu.IsCurrentlyStudying ? "Atual" : edu.EndDate?.ToString("MMM yyyy");
          profileBlock += $"""
            - {edu.Degree} em {edu.FieldOfStudy} na {edu.Institution} ({edu.Location})
              Período: {edu.StartDate:MMM yyyy} até {endDate}

            """;
        }
      }

      if (skills.Any())
      {
        profileBlock += "=== HABILIDADES ===\n";
        var grouped = skills.GroupBy(s => s.Category);
        foreach (var group in grouped)
        {
          var skillList = string.Join(", ", group.Select(s => s.Name));
          profileBlock += $"- {group.Key}: {skillList}\n";
        }
        profileBlock += "\n";
      }

      if (projects.Any())
      {
        profileBlock += "=== PROJETOS ===\n";
        foreach (var proj in projects)
        {
          profileBlock += $"""
            - {proj.Name} | {proj.Technologies}
              Descrição: {Truncate(proj.Description)}

            """;
        }
      }

      if (certifications.Any())
      {
        profileBlock += "=== CERTIFICAÇÕES ===\n";
        foreach (var cert in certifications)
        {
          profileBlock += $"- {cert.Name} por {cert.IssuingOrganization} ({cert.IssueDate:MMM yyyy})\n";
        }
      }

      return profileBlock;
    }
  }
}
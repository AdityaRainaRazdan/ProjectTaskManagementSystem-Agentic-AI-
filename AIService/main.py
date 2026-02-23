from fastapi import FastAPI
from pydantic import BaseModel
from langchain_openai import AzureChatOpenAI
from langchain_core.prompts import ChatPromptTemplate
import os
import json

app = FastAPI()

# =========================
# ENV CONFIG (SET YOUR KEY)
# =========================
os.environ["OPENAI_API_KEY"] = "."
os.environ["AZURE_OPENAI_ENDPOINT"] = "https://-ai.openai.azure.com"
os.environ["AZURE_OPENAI_API_VERSION"] = "2024-02-15-preview"

# =========================
# LLM SETUP
# =========================
llm = AzureChatOpenAI(
    api_key=os.getenv("OPENAI_API_KEY"),
    azure_endpoint=os.getenv("AZURE_OPENAI_ENDPOINT"),
    api_version=os.getenv("AZURE_OPENAI_API_VERSION"),
    azure_deployment="gpt-4o-mini",
    temperature=0,
    response_format={"type": "json_object"}
)

# =========================
# PROMPT (FIXED FOR WHERE + FILTERS)
# =========================
prompt = ChatPromptTemplate.from_messages([
    ("system",
"""
You are an enterprise XAF CRUD command generator.

Return ONLY valid JSON.

Format:
{{
  "entity": "EntityName",
  "action": "create|read|update|delete",
  "fields": {{}},
  "filters": {{}},
  "logicalOperator": "AND|OR"
}}

Rules:
- Entity names must be singular PascalCase (Employee, Organization, Invoice)
- Convert plural to singular automatically
- Convert entity names to proper casing
- UPDATE ? put SET values in fields
- UPDATE/DELETE ? extract WHERE conditions into filters
- Never mix fields and filters
- If no WHERE provided for update/delete ? filters must be empty
- Never return explanation

Examples:

User: update employees set email=abc@gmail.com where name=John
Output:
{{
  "entity":"Employee",
  "action":"update",
  "fields":{{"email":"abc@gmail.com"}},
  "filters":{{"name":"John"}},
  "logicalOperator":"AND"
}}

User: delete employee where id=5
Output:
{{
  "entity":"Employee",
  "action":"delete",
  "fields":{{}},
  "filters":{{"id":5}},
  "logicalOperator":"AND"
}}

User: create employee name=John email=john@gmail.com
Output:
{{
  "entity":"Employee",
  "action":"create",
  "fields":{{"name":"John","email":"john@gmail.com"}},
  "filters":{{}},
  "logicalOperator":"AND"
}}
"""),
    ("user", "{text}")
])

chain = prompt | llm

# =========================
# REQUEST MODEL
# =========================
class UserInput(BaseModel):
    message: str


# =========================
# ENTITY NORMALIZATION
# Fix employee vs Employee vs employees
# =========================
def normalize_entity(name: str):
    if not name:
        return name

    name = name.strip().lower()

    # convert plural ? singular
    if name.endswith("s"):
        name = name[:-1]

    return name.capitalize()


# =========================
# RESPONSE VALIDATION
# =========================
def validate_response(data):
    return (
        isinstance(data, dict)
        and "entity" in data
        and data.get("action") in ["create", "read", "update", "delete"]
        and "fields" in data
        and "filters" in data
    )


# =========================
# PARSE ENDPOINT
# =========================
@app.post("/parse")
async def parse_command(input: UserInput):
    try:
        response = await chain.ainvoke({"text": input.message})

        parsed = json.loads(response.content)

        if not validate_response(parsed):
            return {"error": "Invalid AI structure"}

        # normalize entity name
        parsed["entity"] = normalize_entity(parsed["entity"])

        # ensure fields/filters exist
        parsed.setdefault("fields", {})
        parsed.setdefault("filters", {})
        parsed.setdefault("logicalOperator", "AND")

        return parsed

    except Exception as e:
        return {"error": str(e)}
"""from fastapi import FastAPI
from pydantic import BaseModel
from langchain_openai import AzureChatOpenAI
from langchain_core.prompts import ChatPromptTemplate
import os
import json

app = FastAPI()

#  IMPORTANT: Put your Azure OpenAI key here
os.environ["OPENAI_API_KEY"] = "YOUR_AZURE_OPENAI_KEY"

# If using Azure OpenAI, also set:
# os.environ["AZURE_OPENAI_ENDPOINT"] = "https://your-resource.openai.azure.com/"
# os.environ["AZURE_OPENAI_API_VERSION"] = "2024-02-15-preview"

llm = AzureChatOpenAI(
    api_key=".",
    azure_endpoint="https://.-ai.openai.azure.com",
    api_version="2024-02-15-preview",
    azure_deployment="gpt-4o-mini",
    temperature=0,
    response_format={"type": "json_object"}

)


class UserInput(BaseModel):
    message: str

@app.post("/parse")
async def parse_command(input: UserInput):

    prompt = ChatPromptTemplate.from_messages([
    ("system",
     "You are a CRUD command generator. "
     "Return ONLY valid JSON with structure: "
     "{{ \"entity\": string, \"action\": \"create|read|update|delete\", \"fields\": {{}} }}"),
    ("user", "{text}")
])


    chain = prompt | llm

    response = chain.invoke({"text": input.message})

    try:
        return json.loads(response.content)
    except:
        return {"error": "Invalid JSON from AI"}
        """"
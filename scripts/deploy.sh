
var app-id-from-previous-step=$1
var password-from-previous-step=$2
var id-or-bot-app-service-name=$3
var bot-app-service-name=$4
var name-of-app-service-plan=$5
var region="useast"
var resource-group-name="rg-azure-openai-func"

az deployment group create \
    --resource-group "${resource-group-name}" \
    --template-file "<path-to-template-with-preexisting-rg.json>" \
    --parameters appId="${app-id-from-previous-step}" appSecret="${password-from-previous-step}" botId="${id-or-bot-app-service-name}" newWebAppName="${bot-app-service-name}" newAppServicePlanName="${name-of-app-service-plan}" appServicePlanLocation="${region}" \
    --name "${bot-app-service-name}"

az bot prepare-deploy  \
     --lang Csharp \
     --code-dir "."  \
     --proj-file-path "OpenAIBotProject.csproj"

az webapp deployment source config-zip \
    --resource-group "${resource-group-name}"  \
    --name "${bot-app-service-name}" \
    --src ".deployment"
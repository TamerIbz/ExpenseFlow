# EXPENSE FLOW
##
user secrets

Setting connection to secret manager -> POSTGRES
$postgres_password = "[Pass]"~~~~
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=ExpenseFlow;Username=postgres;Password=$postgres_password"
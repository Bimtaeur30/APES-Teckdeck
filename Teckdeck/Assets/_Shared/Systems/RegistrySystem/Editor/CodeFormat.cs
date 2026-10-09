namespace _Shared.Systems.RegistrySystem.Editor
{
    public static class CodeFormat
    {
        public static string EnumFormat = 
            @"namespace {0}
{{
    public enum {1}
    {{
        {2}
    }}
}}
";
        
    }
}
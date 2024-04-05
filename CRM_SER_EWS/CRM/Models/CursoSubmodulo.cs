namespace CRM_EWS.Servicios
{
    public class CursoSubmodulo
    {
        public int idSubmodulo { get; set; }
        public int idModulo { get; set; }
        public string nombre { get; set; }
        public bool activo { get; set; }
        public CursoModulo? modulo { get; set; }
    }
}

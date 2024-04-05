namespace CRM_EWS.Servicios 
{
    public class CursoModulo
    {
        public int idModulo { get; set; }   
        public int idCurso { get; set; }
        public string nombre { get; set; }
        public bool activo { get; set; }
        public List<CursoSubmodulo>? Submodulos { get; set; }
        public Curso? curso { get; set; }
    }
}

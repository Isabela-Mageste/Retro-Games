namespace RetroGames.Repository;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(Exception causa)
        : base("Já existe um registro com o mesmo valor único.", causa)
    {
    }
}

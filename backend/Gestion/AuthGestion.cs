using EventosAPI.Data;
using EventosAPI.Models;
using EventosAPI.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EventosAPI.Gestion;

public class AuthGestion
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public AuthGestion(AppDbContext context) => _context = context;

    public async Task<(bool Exito, string Mensaje)> RegistrarUsuario(RegistroDTO request)
    {
    if (await _context.Usuarios.AnyAsync(u => u.Correo == request.Correo))
            return (false, "El correo electrónico ya está registrado.");

        var nuevoUsuario = new Usuario 
        { 
            Nombre = request.Nombre, 
            Correo = request.Correo, 
            Rol = "Usuario" 
        };
        
        nuevoUsuario.ContraseñaHash = _passwordHasher.HashPassword(nuevoUsuario, request.Password);

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();
        
        return (true, "Usuario registrado exitosamente.");
    }

    public async Task<(Usuario? Usuario, string Mensaje)> ValidarLogin(LoginDTO request)
    {
    var usuario = await _context.Usuarios.FirstOrDefaultAsync<Usuario>(u => u.Correo == request.Correo);        

        if (usuario == null)
            return (null, "El correo no está registrado.");

        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.ContraseñaHash, request.Password);
        
        if (resultado != PasswordVerificationResult.Success)
            return (null, "La contraseña es incorrecta.");

        return (usuario, "Login exitoso.");
    }
}
import { defineConfig } from 'vite';

// Em desenvolvimento, o Vite responde na porta 5173 e repassa tudo que começa com /api
// para a API (porta 5080). Para o navegador, front-end e API têm a mesma origem:
// não há CORS e o cookie SameSite=Strict funciona.
export default defineConfig({
  server: {
    port: 5173,
    strictPort: true,
    proxy: {
      '/api': { target: 'http://localhost:5080', changeOrigin: true },
    },
  },
});

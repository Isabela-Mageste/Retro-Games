import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// Em desenvolvimento, o Vite responde na porta 5173 e repassa tudo que começa com /api
// para a API (porta 5080). Para o navegador, front-end e API têm a mesma origem:
// não há CORS e o cookie SameSite=Strict funciona.
// O plugin react (do template do Vite) atualiza a tela ao salvar um componente sem
// recarregar a página nem perder o estado (Fast Refresh).
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    strictPort: true,
    proxy: {
      '/api': { target: 'http://localhost:5080', changeOrigin: true },
    },
  },
});

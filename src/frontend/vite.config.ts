import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@api': path.resolve(__dirname, 'src/api/index.ts'),
      '@components': path.resolve(__dirname, 'src/components/index.ts'),
      '@hooks': path.resolve(__dirname, 'src/hooks/index.ts'),
      '@pages': path.resolve(__dirname, 'src/pages/index.ts'),
      '@store': path.resolve(__dirname, 'src/store/index.ts'),
      '@types': path.resolve(__dirname, 'src/types/index.ts'),
    },
  },
});

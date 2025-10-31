import { defineConfig } from "vite";

// https://vitejs.dev/config/
export default defineConfig({
  server: {
    https: true,
    port: 4201,
    strictPort: true, // exit if port is in use
    hmr: {
      clientPort: 4201, // point vite websocket connection to vite directly, circumventing .net proxy
    },
  },
  optimizeDeps: {
    force: true,
  },
  build: {
    outDir: "../Server/wwwroot",
    emptyOutDir: true,
    rollupOptions: {
      output: {
        manualChunks: {
          "material-ui": ["@mui/material"],
        },
      },
    },
  }
});

import { defineConfig } from "vite";
// import mkcert from "vite-plugin-mkcert";
import react from "@vitejs/plugin-react-swc";
import fs from "fs";

// https://vitejs.dev/config/
export default defineConfig({
  build: {
    outDir: "../../backend/API/wwwroot",
    emptyOutDir: true,
  },
  server: {
    port: 3000,
    // https: true,
    https: {
      key: fs.readFileSync("./localhost+3-key.pem"),
      cert: fs.readFileSync("./localhost+3.pem"),
    },
  },
  plugins: [react()],
});

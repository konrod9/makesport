import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
  reactCompiler: true,
};

module.exports = {
  allowedDevOrigins: ["127.0.0.1", "0.0.0.0"],
};

export default nextConfig;

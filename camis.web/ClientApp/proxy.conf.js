const PROXY_CONFIG = [
  {
    context: [
      "/geoserver",
    ],
    target: "http://localhost:8080",  // Include /geoserver in target
    secure: false,
    changeOrigin: true,
    logLevel: "debug",
    pathRewrite: {
      "^/geoserver": ""  // Remove /geoserver from path since it's in target
    }
  }
]

module.exports = PROXY_CONFIG;

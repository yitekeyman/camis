
declare global {
    interface Window { app : any }
  }

export const baseUrl = window.app && window.app.env.API_URL;
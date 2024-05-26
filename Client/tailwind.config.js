/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  darkMode: 'selector',
  theme: {
    extend: {
      colors: {
        'sun-yellow': '#f7f700',
        'moon-blue': '#D9DCEB',
        'dark-bg': '#0A1124',
        'dark-fg': '#D9DCEB',
        'light-bg': '#F9FAFB',
      },
      boxShadow: {
        'sun-glow': '0px 0px 60px 20px #f7f700',
        'moon-glow': '0px 0px 60px 20px #D9DCEB',
      },
    },
  },
  plugins: [],
}

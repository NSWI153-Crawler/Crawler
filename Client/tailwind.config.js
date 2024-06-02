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
        'sun-yellow': '#F7F700',
        'moon-blue': '#D9DCEB',
        'dark-bg': '#0A1124',
        'dark-fg': '#D9DCEB',
        'light-bg': '#F9FAFB',
      },
      boxShadow: {
        'sun-glow': '0 0 60px 10px #f7f766',
        'moon-glow': '0 0 60px 20px #D9DCEB',
        'green-light': '0 0 10px 4px #00FF00, 0 0 3px 3px #00FF00',
        'red-light': '0 0 10px 4px #FF0000, 0 0 3px 3px #FF0000',
      },
    },
  },
  plugins: [],
}

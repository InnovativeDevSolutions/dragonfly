# ArmaDragonflyClient Documentation

This directory contains the Docus-powered documentation site for ArmaDragonflyClient.

> [!NOTE]
> This documentation is automatically generated from the `docs/` folder using the `generate-docs.ps1` script.

## ✨ Features

- 🎨 **Beautiful Design** - Clean, modern documentation theme
- 📱 **Responsive** - Mobile-first responsive design  
- 🌙 **Dark Mode** - Built-in dark/light mode support
- 🔍 **Search** - Full-text search functionality
- 📝 **Markdown Enhanced** - Extended markdown with custom components
- 🎨 **Customizable** - Easy theming and brand customization
- ⚡ **Fast** - Optimized for performance with Nuxt 4
- 🔧 **TypeScript** - Full TypeScript support

## 🚀 Quick Start

### Generate Documentation

From the dragonfly root directory:

```powershell
.\docus\generate-docs.ps1
```

### Development Server

```bash
cd docus
bun install
bun run dev
```

Your documentation site will be running at `http://localhost:3000`

### Build for Production

```bash
bun run build
```

## 📁 Project Structure

```
docus/
├── content/                      # Generated markdown content
│   ├── index.md                  # Homepage
│   ├── 1.getting-started/        # Installation & quick start
│   └── 2.api/                    # API Reference
│       ├── 1.core/               # Core functions
│       ├── 2.basic/              # Basic operations  
│       ├── 3.hash/               # Hash operations
│       └── 4.list/               # List operations
├── public/                      # Static assets
├── app.config.ts                # Site configuration
├── nuxt.config.ts               # Nuxt & GitHub Pages config
├── generate-docs.ps1            # Documentation generator script
└── package.json                 # Dependencies
```

## ⚡ Built with

This starter comes pre-configured with:

- [Nuxt 4](https://nuxt.com) - The web framework
- [Nuxt Content](https://content.nuxt.com/) - File-based CMS
- [Nuxt UI](https://ui.nuxt.com) - UI components
- [Nuxt Image](https://image.nuxt.com/) - Optimized images
- [Tailwind CSS 4](https://tailwindcss.com/) - Utility-first CSS
- [Docus Layer](https://www.npmjs.com/package/docus) - Documentation theme

## 📖 Documentation

For detailed documentation on customizing your Docus project, visit the [Docus Documentation](https://docus.dev)

## 🚀 GitHub Pages Deployment

This documentation is configured for automatic deployment to GitHub Pages.

### Setup

1. Enable GitHub Pages in repository settings (Settings → Pages)
2. Set source to "GitHub Actions"
3. Push changes to the `master` branch

The `.github/workflows/deploy-docs.yml` workflow will automatically:
- Build the documentation
- Deploy to `https://innovativedevsolutions.github.io/dragonfly/`

See `SETUP_GITHUB_PAGES.md` for detailed instructions.

## 📄 License

[MIT License](https://opensource.org/licenses/MIT) 
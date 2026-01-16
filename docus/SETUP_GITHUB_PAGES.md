# Setting up GitHub Pages for ArmaDragonflyClient Documentation

This guide will help you deploy the Docus documentation site to GitHub Pages.

## Prerequisites

- GitHub repository with admin access
- Generated documentation (run `generate-docs.ps1` first)

## Step 1: Enable GitHub Pages

1. Go to your repository on GitHub
2. Navigate to **Settings** → **Pages**
3. Under "Build and deployment":
   - Source: **GitHub Actions**

## Step 2: Create GitHub Actions Workflow

Create `.github/workflows/deploy-docs.yml` with the following content:

```yaml
name: Deploy Documentation

on:
  push:
    branches:
      - master
  workflow_dispatch:

permissions:
  contents: read
  pages: write
  id-token: write

concurrency:
  group: "pages"
  cancel-in-progress: false

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup Bun
        uses: oven-sh/setup-bun@v2
        with:
          bun-version: latest

      - name: Setup Pages
        uses: actions/configure-pages@v5

      - name: Install dependencies
        run: |
          cd docus
          bun install

      - name: Build documentation
        run: |
          cd docus
          bun run build

      - name: Upload artifact
        uses: actions/upload-pages-artifact@v3
        with:
          path: ./docus/.output/public

  deploy:
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}
    runs-on: ubuntu-latest
    needs: build
    steps:
      - name: Deploy to GitHub Pages
        id: deployment
        uses: actions/deploy-pages@v4
```

## Step 3: Generate Documentation

From the dragonfly root directory, run:

```powershell
.\docus\generate-docs.ps1
```

This will:
- Convert all documentation from `docs/` to Docus format
- Create the API reference structure
- Generate homepage and getting started guides

## Step 4: Test Locally

Before deploying, test the site locally:

```bash
cd docus
bun install
bun run dev
```

Visit `http://localhost:3000` to preview.

## Step 5: Commit and Push

```bash
git add .
git commit -m "Add Docus documentation site"
git push origin master
```

The GitHub Actions workflow will automatically:
1. Build the documentation
2. Deploy to GitHub Pages

## Step 6: Access Your Documentation

Once deployed, your documentation will be available at:

```
https://innovativedevsolutions.github.io/dragonfly/
```

## Updating Documentation

To update the documentation:

1. Edit files in the `docs/` folder
2. Run `.\docus\generate-docs.ps1` to regenerate
3. Commit and push changes

The site will automatically rebuild and deploy.

## Troubleshooting

### Build Fails

Check the Actions tab in GitHub for error logs. Common issues:
- Missing dependencies in `package.json`
- Syntax errors in markdown files
- Invalid frontmatter

### Page Not Found

Ensure:
- GitHub Pages is enabled in repository settings
- Workflow has proper permissions
- `baseURL` in `nuxt.config.ts` matches your repository name

### Styling Issues

If styles don't load:
- Verify `baseURL` configuration
- Check browser console for 404 errors
- Ensure `buildAssetsDir` is correctly set

## Configuration Files

Key files for GitHub Pages setup:

- `docus/nuxt.config.ts` - Nuxt configuration with GitHub Pages settings
- `docus/app.config.ts` - Site metadata and branding
- `.github/workflows/deploy-docs.yml` - GitHub Actions workflow

## Additional Resources

- [Docus Documentation](https://docus.dev)
- [Nuxt Static Site Generation](https://nuxt.com/docs/getting-started/deployment#static-hosting)
- [GitHub Pages Documentation](https://docs.github.com/en/pages)

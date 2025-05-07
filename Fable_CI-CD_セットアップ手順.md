# Fable CI/CD セットアップ手順

## Viteセットアップ

以下のコマンドを実行 

```bash
npm create vite@latest （Framework → Vanilla, Variant → javaScript を選択）
```

```bash
cd プロジェクト名
npm i
```
`vite.config.js` をルートディレクトリに作成

```js
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  base: "./",
})
```

## GitHub Actions 設定

Web 上の GitHub ページの `Settings → Pages` の `Build and deployment` の `Source` を `Github Actions` にする

`.github/workflows/deploy.yml` を以下のように作成する

```yml
name: build and deploy

on:
  # Runs on pushes targeting the default branch
  push:
    branches: ["main"]

  # Allows you to run this workflow manually from the Actions tab
  workflow_dispatch:

# Sets permissions of the GITHUB_TOKEN to allow deployment to GitHub Pages
permissions:
  contents: read
  pages: write
  id-token: write

concurrency:
  group: "pages"
  cancel-in-progress: false

jobs:
  # Single deploy job since we're just deploying
  deploy:
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4
      - name: Setup Pages
        uses: actions/configure-pages@v5
      - name: Setup Node
        uses: actions/setup-node@v4
        with:
          node-version: 22
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      - name: Restore .NET tools
        run: dotnet tool restore
      - name: build
        run: npm i && npm run build
      - name: Upload artifact
        uses: actions/upload-pages-artifact@v3
        with:
          # ビルド生成物はdistフォルダ
          path: './dist'
      - name: Deploy to GitHub Pages
        id: deployment
        uses: actions/deploy-pages@v4
```

## F# プロジェクト作成

F#プロジェクト用のフォルダを作成し、以下コマンドを実行

```bash
cd F#プロジェクト用フォルダ名
dotnet new console -lang F#
```

## Fable 導入

F# プロジェクトの `.fsproj` ファイルに Fable 関連パッケージを追記

```fsproj
  <ItemGroup>
    <PackageReference Include="Fable.Core" Version="4.5.0" />
    <PackageReference Include="Fable.Browser.Dom" Version="2.19.0" />
  </ItemGroup>
```

ルートディレクトリにある `package.json` の "scripts" 部分を以下のように修正

```json
"scripts": {
  "dev": "npm run fable && vite",
  "build": "npm run fable && vite build",
  "preview": "vite preview",
  "fable": "dotnet fable src-fsharp/src-fsharp.fsproj --outDir src/fable-output"
},
```

ルートディレクトリに `.config/dotnet-tools.json` を作成

```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "fable": {
      "version": "4.25.0",
      "commands": [
        "fable"
      ]
    }
  }
} 
```

下記コマンド実行

```bash
dotnet tool restore
```

Fable のビルド出力用ディレクトリ（src/fable-output）を作成後、下記コマンド実行

```bash
npm run fable
npm run build
```
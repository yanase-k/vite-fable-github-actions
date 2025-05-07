// For more information see https://aka.ms/fsharp-console-apps
module Program

open Browser.Dom

let element = document.getElementById "app"
element.innerText <- "Vite + Fable + GitHub Actions テスト"

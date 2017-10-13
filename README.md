# FreneticDocs

Welcome to the Frenetic Docs Engine, built in ASP.NET Core MVC!

If you're seeing this, you're probably horrendously lost.

### Notice

At current stage, FreneticDocs is Copyright (C) 2016-2017 FreneticLLC, All Rights Reserved.

Licensing is likely to change in the future.

### Project Status

Early development. Commands and tags pages are ready!

These docs are for legacy (comment structured) docs!

### Setup

- Create `config/docs.cfg` based on `sample_config.cfg` and configure it.
- Install .NET Core CLI SDK version 1.0.1 Preview 5 or higher! That's available at: https://github.com/dotnet/cli
- Install NPM (Node Package Manager) and Node.js.
- Install NPM package `gulp` and related (at a command line, `npm install gulp gulp-cli rimraf gulp-concat gulp-cssmin gulp-uglify gulp-rename`).
- Might have to install `gulp-cli` globally (`npm install -g gulp gulp-cli`).
- Run `development.bat` or `start.bat`.
- Connect to server address (or `localhost`) at port `8051`.
- That's all, folks!

### Other things to configure

- Create a `config/docs_homepage.html` to contain homepage HTML data.
- Create your own docs logo and favicon

### Editing

- Primary editing should be done in VS Code. Requires OmniSharp-VS Code 1.6.0-beta6 or higher.

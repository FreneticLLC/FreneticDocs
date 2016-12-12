# FreneticDocs

Welcome to the Frenetic Docs Engine, built in ASP.NET Core MVC!

If you're seeing this, you're probably horrendously lost.

### Notice

At current stage, FreneticDocs is Copyright (C) 2016 FreneticXYZ, All Rights Reserved.

Licensing is likely to change in the future.

### Project Status

Early development. Commands and tags pages are ready!

### Setup

- Create `config/docs.cfg` based on `sample_config.cfg` and configure it.
- Install .NET Core CLI.
- Install NPM (Node Package Manager) and Node.js.
- Install NPM package `gulp` and related (at a command line, `npm install gulp gulp-cli rimraf gulp-concat gulp-cssmin gulp-uglify gulp-rename`).
- Might have to install `gulp-cli` globally (`npm install -g gulp gulp-cli`).
- TEMPORARY: Might have to run `dotnet migrate` depending on environment! (Fair warning, Linux /will/ give issues).
- Run `dotnet restore`.
- Run `development.bat` or `start.bat`.
- Connect to server address (or `localhost`) at port `8051`.
- That's all, folks!

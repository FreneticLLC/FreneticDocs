# FreneticDocs

Welcome to the Frenetic Docs Engine, built in ASP.NET Core MVC!

### Notice

At current stage, FreneticDocs is Copyright (C) 2016-2018 Frenetic LLC, All Rights Reserved.

Licensing is likely to change in the future (probably MIT).

### Project Status

Midway through initial development - most pages are available (though not fully complete), Discord bot is working but needs more work.

These docs are for legacy (comment structured) docs!

### Setup

- Create `config/docs.cfg` based on `sample_config.cfg` and configure it.
- Install .NET Core CLI SDK version 2.0.0 or higher! That's available at: https://github.com/dotnet/cli
- Install `NPM` (Node Package Manager) and `Node.js`.
- Install NPM package `gulp` and related (at a command line, `npm install gulp gulp-cli rimraf gulp-concat gulp-cssmin gulp-uglify gulp-rename`).
- Might have to install `gulp-cli` globally (`npm install -g gulp gulp-cli`).
- If on Windows, run `development.bat` or `start.bat`. Otherwise, use `launch.sh`.
- Connect to server address (or `localhost`) at port `8051`.
- That's all, folks!

### Other things to configure

- Create a `config/docs_homepage.html` to contain homepage HTML data.
- Create your own docs logo and favicon.
- Create/edit `start.sh` to allow proper auto-restarting, and/or configure the executable to be a service.

### Editing

- Primary editing should be done in VS Code (With the OmniSharp extension... VSCode will automatically suggest installing it).

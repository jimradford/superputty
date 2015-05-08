# Introduction #

These are some guidelines that developers should be using when working on the SuperPuTTY application as developers

# Patches #

Please provide any patches in the issue tracker for possible inclusion into the core application, patches should be in most cases made against **trunk**

# Commit access #

We are happy to provide commit access to anyone who shows a willingness to further the development of SuperPuTTY and shows a history of submitting useful patches to the project.

# Subversion Source Tree Layout #

**trunk** is where current development happens and can be unstable or unusable but should _always_ compile.

**branches** is where experimental features are developed. Any developer with commit access should use a naming scheme to make clear what the branch is and who the developer is e.g. _jradford-lua_ might indicate developer jradford is experimenting with integrating lua into the application.

**tags** is where stable releases are located.

# Version Numbering #

We will do our best to follow the Semantic Versioning guidelines as outlined on this website http://semver.org/ for version numbering. API changes are also meant to convey backwards compatibility with configuration files.
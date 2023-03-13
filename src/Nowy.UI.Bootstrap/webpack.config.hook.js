const fs = require('fs');
const path = require('path');

console.log(path.resolve(__dirname, 'wwwroot/output'));

module.exports = {
  'hook': function () {

    for (let theme of ['lr', 'ts', 'nowy',]) {
      for (let framework of ['bootstrap5', 'bootstrap5-fluentui',]) {

        let path_bundle_scss = path.resolve(__dirname, `resources/bundles/bundle-${theme}-${framework}.scss`);
        let path_bundle_ts = path.resolve(__dirname, `resources/bundles/bundle-${theme}-${framework}.ts`);

        fs.writeFileSync(path_bundle_scss, `
        @import "../nowy/theme-${theme}/_index";
        @import "../nowy/common/telerik";
        @import "../nowy/framework-${framework}/_index";
        @import "../nowy/common/_index";
        `.split("\n").map((item) => item.trim()).join("\n"));

        fs.writeFileSync(path_bundle_ts, `
        import "../nowy/theme-${theme}/_index";
        import "../nowy/framework-${framework}/_index";
        import "../nowy/common/_index";
        `.split("\n").map((item) => item.trim()).join("\n"));
      }
    }

  }
};


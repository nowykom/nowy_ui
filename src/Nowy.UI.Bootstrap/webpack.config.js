const fs = require('fs');
const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const TsconfigPathsPlugin = require('tsconfig-paths-webpack-plugin');

console.log(path.resolve(__dirname, 'wwwroot/output'));

module.exports = function (env, {mode}) {
  const production = mode === 'production';

  let entry_points = {};
  entry_points[`init`] = [
    path.resolve(__dirname, `resources/bundles/init.scss`),
    path.resolve(__dirname, `resources/bundles/init.ts`),
  ];
  entry_points[`module-leaflet`] = [
    path.resolve(__dirname, `resources/bundles/module-leaflet.ts`),
  ];

  for (let theme of ['lr', 'ts', 'nowy',]) {
    for (let framework of ['bootstrap5', 'bootstrap5-fluentui',]) {

      let path_bundle_scss = path.resolve(__dirname, `resources/bundles/bundle-${theme}-${framework}.scss`);
      let path_bundle_ts = path.resolve(__dirname, `resources/bundles/bundle-${theme}-${framework}.ts`);

      fs.writeFileSync(path_bundle_scss, `
        @import "../nowy/theme-${theme}/_index";
        @import "../nowy/common/telerik";
        @import "../nowy/framework-${framework}/_index";
        @import "../nowy/common/_index";
        `.split("\n").map((item) => item.trim()).join("\n")
      );
      fs.writeFileSync(path_bundle_ts, `
        import "../nowy/theme-${theme}/_index";
        import "../nowy/framework-${framework}/_index";
        import "../nowy/common/_index";
        `.split("\n").map((item) => item.trim()).join("\n")
      );

      entry_points[`bundle-${theme}-${framework}`] = [
        path_bundle_scss,
        path_bundle_ts,
      ];
    }
  }

  return {

    node: {
      global: false,
      __filename: false,
      __dirname: false,
    },

    mode: production ? 'production' : 'development',
    devtool: production ? 'source-map' : 'inline-source-map',
    entry: entry_points,

    output: {
      path: path.resolve(__dirname, 'wwwroot/output'),
      filename: '[name].js',
      clean: true,
      module: true,
      library: {
        type: "module",
      },
    },

    experiments: {
      outputModule: true,
    },

    resolve: {
      extensions: ['.ts', '.js'],
      modules: ['resources', 'node_modules'],
      plugins: [new TsconfigPathsPlugin({
        configFile: path.resolve(__dirname, 'tsconfig.webpack.json'),
      })]
    },

    module: {
      rules: [
        {
          test: /\.s[ac]ss$/i,
          use: [
            // Creates `style` nodes from JS strings
            MiniCssExtractPlugin.loader, // production ? MiniCssExtractPlugin.loader : "style-loader",
            // Translates CSS into CommonJS
            {
              loader: 'css-loader',
              options: {
                url: true,
                sourceMap: true,
              }
            },
            {
              loader: 'resolve-url-loader',
              options: {
                sourceMap: true,
              }
            },
            // Compiles Sass to CSS
            {
              loader: 'sass-loader',
              options: {
                sourceMap: true, // <-- !!IMPORTANT!!
              }
            },
          ],
        },
        {
          test: /\.ts$/i,
          use: [
            {
              loader: 'ts-loader',
              options: {
                configFile: "tsconfig.webpack.json"
              }
            }
          ],
          exclude: /node_modules/
        }
      ]
    },

    plugins: [
      new MiniCssExtractPlugin(),
    ],

  }
};


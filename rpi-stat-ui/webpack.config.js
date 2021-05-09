const path = require("path");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");

module.exports = {
    entry: "./Scripts/index.ts",
    output: {
        path: path.resolve(__dirname, "wwwroot/js"),
        filename: "[name].js",
        library: "Stat"
    },
    devtool: 'source-map',
    resolve: {
        extensions: [".js", ".ts"]
    },
    module: {
        rules: [
            { test: /\.ts?$/, exclude: /node_modules/, loader: 'ts-loader' },
            { enforce: 'pre', test: /\.js$/, loader: 'source-map-loader' }
        ]
    },
    plugins: [
        new CleanWebpackPlugin()
    ]
};

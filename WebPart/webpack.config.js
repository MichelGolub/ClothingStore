var path = require('path')
const { VueLoaderPlugin } = require('vue-loader');
const webpack = require('webpack')

module.exports = {
    mode: 'development',
    entry : './src/main.js',
    output : {
        path : path.resolve(__dirname, './dist'),
        publicPath : '/dist/',
        filename : 'build.js'
    },
    module : {
        rules : [
            {
                test : /\.vue$/,
                loader : 'vue-loader'
            }, {
                test : /\.css$/,
                use : [
                    'vue-style-loader',
                    'css-loader'
                ]
            }
        ]
    },
    devServer: {
        historyApiFallback: true
    },        
    plugins : [
        new VueLoaderPlugin(),
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery'
        })
    ],
}
"use strict";

var env = require('node-env-file'),
    path = require("path");

env(path.join(__dirname, ".env"));

function getEnv(name, defaultValue) {
    if (!process.env.hasOwnProperty(name)) {
        if (defaultValue) {
            return defaultValue;
        }
        throw new Error("Env setting: " + name + " is not configured!");
    }
    return process.env[name].trim();
}

module.exports = {
    port: getEnv('PORT', 8080),
    fromUserAccount: getEnv('FROM_USER_ACCOUNT'),
    fromUserPass: getEnv('FROM_USER_PASSWROD'),
    toUserAccount: getEnv('TO_USER_ACCOUNT'),
    ethHttpUrl: getEnv('ETH_HTTP_URL'),
    etherEachTime: getEnv('ETHER_EACH_TIME', '0.01')
};
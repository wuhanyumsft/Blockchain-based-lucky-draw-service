const express = require('express');
const app = express();
const router = express.Router();
const Web3 = require('web3');
const bodyParser = require('body-parser');
const web3Admin = require('web3admin');

var config = require('./config');
var controller = require("./controller");

var web3 = new Web3(new Web3.providers.HttpProvider(config.ethHttpUrl));

app.use(bodyParser.json({ limit: '5mb' }));
app.use(bodyParser.urlencoded({ limit: '5mb', extended: true }));
app.set('port', config.port);
app.use(router);

router.get('/', controller.get);
router.post('/', controller.create);

setTimeout(function() {
    web3Admin.extend(web3);
    app.listen(app.get('port'), () =>
        console.log(`Express server listening on port ${app.get('port')}`)
    );
}, 1000);
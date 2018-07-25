const express = require('express');
const app = express();
const router = express.Router();
const bodyParser = require('body-parser');
var config = require('./config');
var controller = require("./controller");

app.use(bodyParser.json({ limit: '5mb' }));
app.use(bodyParser.urlencoded({ limit: '5mb', extended: true }));
app.set('port', config.port);
app.use(router);

router.get('/', controller.get);
router.post('/', controller.create);

app.listen(app.get('port'), () =>
    console.log(`Express server listening on port ${app.get('port')}`)
);
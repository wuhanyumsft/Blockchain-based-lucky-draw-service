const express = require('express');
const app = express();
const router = express.Router();
const Web3 = require('web3');
const bodyParser = require('body-parser');
const web3Admin = require('web3admin');
var web3 = new Web3(new Web3.providers.HttpProvider("http://13.77.152.248:30306"));

app.use(bodyParser.json({ limit: '5mb' }));
app.use(bodyParser.urlencoded({ limit: '5mb', extended: true }));
app.set('port', "8080");
app.get('/', (req, res) => {
    res.json('Lucydraw web api!');
});

app.post('/', async(req, res) => {
    var from = "0xff38a2b8dbc9285fcef0eb3cf5551fc593527c52";
    var to = "0x2f9f903012b6e58604c551720206ca95218ffa50";
    var transaction = {
        from: from,
        to: to,
        value: web3.toWei('0.01', 'ether'),
        data: web3.toHex(req.body.timestamp)
    };
    console.log("transaction");
    console.log(transaction);
    web3.personal.unlockAccount(from, "luckydraw");
    web3.eth.sendTransaction(transaction, function(err, transactionHash) {
        if (!err) {
            console.log("transactionHash:");
            console.log(transactionHash);
            web3.miner.start();
            var receipt = null;
            while (receipt == null) {
                receipt = web3.eth.getTransactionReceipt(transactionHash);
                console.log("receipt:");
                console.log(receipt);
            }
            web3.miner.stop();
            var blockInfo = web3.eth.getBlock(receipt.blockNumber);
            console.log("blockInfo:");
            console.log(blockInfo);
            res.json(blockInfo.timestamp);
        } else {
            console.log(err);
        }
    });
});
setTimeout(function() {
    web3Admin.extend(web3);
    app.listen(app.get('port'), () =>
        console.log(`Express server listening on port ${app.get('port')}`)
    );
}, 1000);
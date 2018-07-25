var config = require('./config');

exports.get = function(req, res) {
    res.json('Lucydraw web api!');
};

exports.create = function(req, res) {
    if (!req.body.timestamp) {
        res.json({ "error": "timestamp is empty" });
    } else {
        var transaction = {
            from: config.fromUserAccount,
            to: config.toUserAccount,
            value: web3.toWei(config.etherEachTime, 'ether'),
            data: web3.toHex(req.body.timestamp)
        };
        console.log("transaction");
        console.log(transaction);
        try {
            web3.personal.unlockAccount(config.fromUserAccount, config.fromUserPass);
            web3.eth.sendTransaction(transaction, function(err, transactionHash) {
                if (!err) {
                    web3.miner.start();
                    var receipt = null;
                    while (receipt == null) {
                        receipt = web3.eth.getTransactionReceipt(transactionHash);
                    }
                    web3.miner.stop();
                    var blockInfo = web3.eth.getBlock(receipt.blockNumber);
                    console.log("blockInfo:");
                    console.log(blockInfo);
                    res.json({ "block_timestamp": blockInfo.timestamp });
                } else {
                    res.json({ "error": err });
                }
            });
        } catch (e) {
            res.json({ "error": e });
        }
    }

};
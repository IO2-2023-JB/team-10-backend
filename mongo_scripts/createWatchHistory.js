const databaseName = 'mojeWideloDb';
db = connect(`mongodb://localhost:27017/${databaseName}`);

db.users.find().forEach(user => {
    if (!db.history.countDocuments({ _id: user._id }, { limit: 1 }))
        db.history.insertOne({
            _id: user._id,
            WatchedVideos: []
        });
});

if (db.history.countDocuments() === db.users.countDocuments()) {
    print('History successfully created.')
}



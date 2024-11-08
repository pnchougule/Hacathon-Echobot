<div class="container">
    <div class="row">
        <label class="page-title">Landing Page</label>
    </div>
    <div class="row">
        <div class="col-lg-6">
            <div class="row searchbox">
                <div class="row">
                    <label class="label-text">User Prompt</label>
                </div>
                <div class="row">
                    <div class="input-group">
                        <input type="search" class="form-control rounded" placeholder="User will provide prompt/query" aria-label="Search" aria-describedby="search-addon" />
                        <button type="button" class="btn btn-outline-primary" data-mdb-ripple-init>search</button>
                    </div>
                </div>
            </div>
            <div class="row">
                <label class="label-text">Response</label>
                <div>
                    <div class="message-box"></div>
                </div>
            </div>
            <div class="row">
                <label class="label-text">Chat History</label>
                <div>
                    <div class="message-box"></div>
                </div>
            </div>
        </div>
        <div class="col-lg-6">
            <div class="row">
                <p class="label-text">Available Plugins</p>
            </div>
            <div class="row">
                <div class="col-lg-1 checkbox-name">
                    <input type="radio" name="" />
                </div>
                <div class="col-lg-11">
                    <p>Web Search</p>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-1 checkbox-name">
                    <input type="radio" name="" />
                </div>
                <div class="col-lg-11">
                    <p>Sharepoint Connect</p>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-1 checkbox-name">
                    <input type="radio" name="" />
                </div>
                <div class="col-lg-11">
                    <p>Dynamic Connect</p>
                </div>
            </div>
            <div class="row">
                <label class="label-text">Semantic Index History</label>
                <div>
                    <div class="message-box"></div>
                </div>
            </div>
        </div>
    </div>
</div>



.checkbox-name {
    font-size: 16px;
    line-height: 17px;
}

    .checkbox-name input {
        background: #2A5A98;
    }

.search-btn {
    width: 100%;
}

.page-title {
    text-align: center;
    font-size: xx-large;
    margin-bottom: 22px;
    margin-top: 10px;
}

.searchbox-dimension {
    width: 100%;
}

.label-text {
    margin-bottom: 8px;
    font-size:large;
}

.message-box{
    border: solid black 1px;
    height: 150px;
    margin-bottom: 20px;
}

.searchbox{
    margin-bottom: 20px;
}

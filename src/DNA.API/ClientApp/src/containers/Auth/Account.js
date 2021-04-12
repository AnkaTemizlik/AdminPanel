import React from "react";
import { Grid } from "@material-ui/core";
import SettingsIcon from '@material-ui/icons/Settings';

const Account = () => {
    return (
        <Grid container style={{ minHeight: 500 }} justify="center" alignItems="center" >
            <SettingsIcon style={{ fontSize: 240, opacity: "0.1" }} />
        </Grid>
    );
}

export default Account;
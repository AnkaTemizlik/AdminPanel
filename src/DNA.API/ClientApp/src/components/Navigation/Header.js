import React from 'react'
import Toolbar from "@material-ui/core/Toolbar";
import { Header as TreasuryHeader } from "@mui-treasury/layout";

const Header = (props) => {

    return <TreasuryHeader
        color={props.color || "primary"}
        {...props}
    >
        <Toolbar>
            {props.children}
        </Toolbar>
    </TreasuryHeader>
}

export default Header
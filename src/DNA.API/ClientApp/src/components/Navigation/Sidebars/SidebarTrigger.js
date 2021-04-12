import React from 'react'
import * as Treasury from "@mui-treasury/layout";
import AccountCircleTwoTone from '@material-ui/icons/AccountCircleTwoTone';

const SidebarTrigger = ({ secondary }) => {

    // ["action","disabled","error","inherit","primary","secondary"]
    return secondary
        ? <Treasury.SecondarySidebarTrigger style={{ marginLeft: "8px", color:"white" }} >
            {/* <Treasury.SidebarTriggerIcon /> */}
            <AccountCircleTwoTone />
        </Treasury.SecondarySidebarTrigger>
        : <Treasury.SidebarTrigger style={{ color:"white" }}>
            <Treasury.SidebarTriggerIcon />
        </Treasury.SidebarTrigger>
}

export default SidebarTrigger
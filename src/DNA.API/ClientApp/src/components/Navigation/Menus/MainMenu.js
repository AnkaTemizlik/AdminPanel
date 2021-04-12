
import React from 'react'
import { withWidth } from "@material-ui/core";
import CascadingHoverMenus from '../../UI/Menu/CascadingHoverMenus';

// const useStyles = makeStyles(theme => {
//     return {
//         hoverMenu: {
//             [theme.breakpoints.down('sm')]: {
//                 display: "none"
//             }
//         }
//     }
// });

const MainMenu = (props) => {
    //console.write("[MainMenu]", props.width)
    let minimal = props.width === "xs" || props.width === "sm";

    return minimal
        ? <CascadingHoverMenus menu={{ label: "Menu", menus: props.menu }} />
        : props.menu.menus.map((m, i) => {
            return <CascadingHoverMenus key={i} menu={m} />
        })
}

export default withWidth()(MainMenu)

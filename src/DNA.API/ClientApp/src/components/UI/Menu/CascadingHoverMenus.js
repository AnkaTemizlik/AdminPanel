import * as React from "react";
import { NavLink } from "react-router-dom";
import Menu from "material-ui-popup-state/HoverMenu";
import MenuItem from "@material-ui/core/MenuItem";
import Button from "@material-ui/core/Button";
import ArrowDropDown from '@material-ui/icons/ArrowDropDown'
import PopupState, { bindHover, bindMenu } from "material-ui-popup-state";
import SubMenu from "./SubMenu";

const CascadingHoverMenus = props => (
    <PopupState variant="popover" popupId="demoMenu">
        {popupState => {
            function getSubMenu(m, j, b) {
                return (
                    <SubMenu key={j} popupId="moreChoicesMenu" title={m.label}>
                        {m.menus.map((x, i) => {
                            return getMenu(x, i,
                                {
                                    "label": m.label,
                                    "id": m.id,
                                    "code": m.code
                                }
                            );
                        })}
                    </SubMenu>
                );
            }
            function getMenu(m, i, b) {
                if (props.onSelect)
                    return <MenuItem key={i} onClick={(e) => {
                        props.onSelect(m, i, b);
                        popupState.close(e)
                    }}>{m.label}</MenuItem>
                else
                    return <MenuItem key={i} onClick={popupState.close} component={NavLink} to={m.to}>{m.label}</MenuItem>
            }
            return (
                <div>
                    <Button style={{ textTransform: "none" }} {...bindHover(popupState)}>
                        {props.menu.label}
                        {props.menu.menus
                            ? props.hideRootArrow ? null : <ArrowDropDown />
                            : null}
                    </Button>

                    {props.menu.menus ? (

                        <Menu dense={true}
                            {...bindMenu(popupState)}
                            anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
                            transformOrigin={{ vertical: "top", horizontal: "left" }}
                            getContentAnchorEl={null}
                            disableScrollLock={true}
                        >

                            {props.menu.menus.map((m, i) => {
                                if (m.menus)
                                    return getSubMenu(m, i, props.menu);
                                else return getMenu(m, i, props.menu);
                            })}

                        </Menu>

                    ) : null}
                </div>
            );
        }}
    </PopupState>
);

export default CascadingHoverMenus
import * as React from 'react'
import withStyles from '@material-ui/core/styles/withStyles'
import Menu from 'material-ui-popup-state/HoverMenu'
import MenuItem from '@material-ui/core/MenuItem'
import ListItemText from '@material-ui/core/ListItemText'
import ChevronRight from '@material-ui/icons/ChevronRight'
import PopupState, { bindHover, bindMenu } from 'material-ui-popup-state'

const submenuStyles = theme => ({
    menu: {
        top: theme.spacing(-1),
    },
    title: {
        flexGrow: 1,
    },
    moreArrow: {
        marginRight: theme.spacing(-1),
    },
})

const ParentPopupState = React.createContext(null)

const SubMenu = withStyles(submenuStyles)(
    // Unfortunately, MUI <Menu> injects refs into its children, which causes a
    // warning in some cases unless we use forwardRef here.
    React.forwardRef(({ classes, title, popupId, children, ...props }, ref) => (
        <ParentPopupState.Consumer>
            {parentPopupState => (
                <PopupState
                    variant="popover"
                    popupId={popupId}
                    parentPopupState={parentPopupState}
                >
                    {popupState => (
                        <ParentPopupState.Provider value={popupState}>
                            <MenuItem
                                {...bindHover(popupState)}
                                selected={popupState.isOpen}
                                ref={ref}
                            >
                                <ListItemText className={classes.title}>{title}</ListItemText>
                                <ChevronRight className={classes.moreArrow} />
                            </MenuItem>
                            <Menu
                                dense={true}
                                {...bindMenu(popupState)}
                                className={classes.menu}
                                anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
                                transformOrigin={{ vertical: 'top', horizontal: 'left' }}
                                getContentAnchorEl={null}
                                {...props}
                            >
                                {children}
                            </Menu>
                        </ParentPopupState.Provider>
                    )}
                </PopupState>
            )}
        </ParentPopupState.Consumer>
    ))
)

export { ParentPopupState }

export default SubMenu
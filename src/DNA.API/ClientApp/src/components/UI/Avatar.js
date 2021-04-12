import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Avatar from "@material-ui/core/Avatar"

const useStyles = makeStyles(theme => {
    return {
        avatar: {
            backgroundColor: props => props.color === "primary" ? theme.palette.primary.main : props.color === "secondary" ? theme.palette.secondary.main : "inherit",
            color: props => props.color === "primary" ? theme.palette.primary.contrastText : props.color === "secondary" ? theme.palette.secondary.contrastText : "inherit",
            margin: "auto"
        }
    }
})

export default (props) => {
    const classes = useStyles(props)
    return <Avatar {...props} className={classes.avatar}>{props.children}</Avatar>
}
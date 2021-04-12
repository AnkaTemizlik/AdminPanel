import React from 'react'
import { makeStyles } from '@material-ui/styles';
import CircularProgress from '@material-ui/core/CircularProgress';

const useStyles = makeStyles((theme) => ({
    progress: {
        color: theme.palette.secondary.main,
        position: 'absolute',
        top: theme.spacing(2),
        left: theme.spacing(2),
        zIndex: 1,
    }
}));

const Progress = ({ loading, className, size }) => {
    const classes = useStyles()
    return loading
        ? <CircularProgress color="secondary" size={size} className={className || classes.progress} />
        : null
}

export default Progress





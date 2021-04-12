import React, { useEffect, useState } from 'react'
import { makeStyles } from '@material-ui/styles';
import LinearProgress from '@material-ui/core/LinearProgress';

const useStyles = makeStyles(() => ({
    linearProgress: {
        position: "absolute", margin: "auto", left: 0, right: 0
    }
}));

var timer;

export default ({ loading, className, size }) => {
    const classes = useStyles()
    const [active, setActive] = useState(false)

    useEffect(() => {
        if (loading) {
            timer = setTimeout(() => setActive(loading), 250)
        }
        else {
            setActive(false)
            clearTimeout(timer)
        }
        return () => clearTimeout(timer)
    }, [loading])

    return active
        ? <LinearProgress color="secondary" className={classes.linearProgress} />
        : null
}



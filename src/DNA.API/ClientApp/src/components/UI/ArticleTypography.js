import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { Typography } from '@material-ui/core';

const useStyles = makeStyles(theme => {
    return {
        article: {

        },
        section: {

        },
        header1: {
            margin: theme.spacing(5, 0, 4)
        },
        header2: {
            margin: theme.spacing(4, 0, 2)
        },
        header3: {
            margin: theme.spacing(3, 0, 2)
        },
        header4: {
            margin: theme.spacing(3, 0, 1)
        },
        title: {
            margin: theme.spacing(1, 0, 1),
            fontWeight: 900
        },
        subTitle: {
            margin: theme.spacing(1, 0, 0)
        },
        paragraph: {
            margin: theme.spacing(0, 0, 1)
        }
    }
});

const Article = (props) => {
    const classes = useStyles()
    return <article className={classes.article}>
        {props.children}
    </article>
}
const Section = (props) => {
    const classes = useStyles()
    return <section className={classes.article}>
        {props.children}
    </section>
}

const H1 = (props) => {
    const classes = useStyles()
    return <Typography variant="h3" component="h1" {...props} className={classes.header1}>
        {props.children}
    </Typography>
}
const H2 = (props) => {
    const classes = useStyles()
    return <Typography variant="h4" component="h2" {...props} className={classes.header2}>
        {props.children}
    </Typography>
}
const H3 = (props) => {
    const classes = useStyles()
    return <Typography variant="h5" component="h3" {...props} className={classes.header3}>
        {props.children}
    </Typography>
}
const H4 = (props) => {
    const classes = useStyles()
    return <Typography variant="h6" component="h4" {...props} className={classes.header4}>
        {props.children}
    </Typography>
}

const Title = (props) => {
    const classes = useStyles()
    return <Typography variant="subtitle1" component="h5" {...props} className={classes.title}>
        {props.children}
    </Typography>
}
const SubTitle = (props) => {
    const classes = useStyles()
    return <Typography variant="subtitle2" component="h6" {...props} className={classes.subTitle}>
        {props.children}
    </Typography>
}

const P = (props) => {
    const classes = useStyles()
    return <Typography variant="body1" component="p" {...props} className={classes.paragraph}>
        {props.children}
    </Typography>
}


export {
    H1, H2, H3, H4, P, Article, Section, Title, SubTitle
}
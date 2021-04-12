import React from 'react'
import { Typography, Container, Avatar, Grid, Box } from "@material-ui/core";
import { makeStyles } from "@material-ui/core/styles";
import Progress from './Progress';

const useStyles = makeStyles(theme => {

    return {
        avatar: {
            margin: theme.spacing(2),
            backgroundColor: theme.palette.primary.main,
            width: 72,
            height: 72
        },
        wrapper: {
            position: 'relative',
        },
        progress: {
            color: theme.palette.secondary.main,
            position: 'absolute',
            top: theme.spacing(2),
            left: theme.spacing(2),
            zIndex: 1,
        }
    }
});

const FormWrapper = (props) => {
    //console.write("[FormWrapper]", props)
    const { title, comment, children, loading } = props
    const classes = useStyles();

    return (
        <Container component="main" maxWidth="xs">
            <Grid container direction="column" justify="space-evenly" alignItems="center">
                <Box mt={3} mb={1} className={classes.wrapper}>
                    <Avatar className={classes.avatar}>
                        <props.icon style={{ fontSize: 36 }} />
                    </Avatar>
                    <Progress loading={loading} className={classes.progress} size={72} />
                </Box>
                <Box mb={3}>
                    <Typography component="h1" variant="h4" >
                        {title}
                    </Typography>
                </Box>
                <Box mb={2}>
                    <Typography component="h1" variant="body1" style={{ textAlign: "center" }}>
                        {comment}
                    </Typography>
                </Box>
                <Box width="100%" mb={10}>{children}</Box>

            </Grid>
        </Container>
    );
}

export default FormWrapper
import React from 'react'
import * as Icons from '@material-ui/icons'
import { Box, Avatar } from '@material-ui/core'
import { makeStyles } from '@material-ui/core/styles';
import clsx from 'clsx';
import Card from '@material-ui/core/Card';
import CardHeader from '@material-ui/core/CardHeader';
import CardMedia from '@material-ui/core/CardMedia';
import CardContent from '@material-ui/core/CardContent';
import CardActions from '@material-ui/core/CardActions';

const useStyles = makeStyles((theme) => ({
    root: {
        maxWidth: 345,
    },
    media: {
        height: 0,
        paddingTop: '150px', // 16:9
    },
    // expand: {
    //     transform: 'rotate(0deg)',
    //     marginLeft: 'auto',
    //     transition: theme.transitions.create('transform', {
    //         duration: theme.transitions.duration.shortest,
    //     }),
    // },
    // expandOpen: {
    //     transform: 'rotate(180deg)',
    // },
    // avatar: {
    //     backgroundColor: red[500],
    // },
}));

const Showcase = () => {
    const classes = useStyles();
    let prevName = "";
    let card = []

    return <Box display="flex" flexDirection="row" flexWrap="wrap">
        {Object.keys(Icons).map((n, i) => {
            //names.push(n)

            if (prevName == "")
                prevName = n

            if (n.startsWith(prevName)) {
                // benzer, listeye ekle
                if (n.endsWith("TwoTone")) {
                    card.push(<Box m={1} title={n}>
                        {React.createElement(Icons[n], { name: n, fontSize: "large" })}
                    </Box>)
                }
                return null
            }
            else {

                // değişti
                let groupName = prevName;
                let temp = card;
                card = []
                prevName = n;

                card.push(<Box m={1} title={n}>
                    {React.createElement(Icons[n], { name: n, fontSize: "large" })}
                </Box>)


                return <Card style={{ margin: "2px" }} key={i} className={classes.root}>
                    <CardHeader title={groupName} avatar={
                        <Avatar aria-label="recipe" className={classes.avatar}>
                            {Icons[n]}
                        </Avatar>
                    }
                    />
                    <CardContent>
                        <Box p={2} display="flex" flexDirection="row" flexWrap="wrap">
                            {temp.map((b) => {
                                return b
                            })}
                        </Box>
                    </CardContent>
                </Card>

                // öncekini gönder
            }

        })
        }</Box>
}

export default Showcase
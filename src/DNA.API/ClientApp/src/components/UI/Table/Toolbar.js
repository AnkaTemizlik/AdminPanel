import React, { useState } from 'react';
import PropTypes from 'prop-types';
import clsx from 'clsx';
import { lighten, makeStyles } from '@material-ui/core/styles';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import IconButton from '@material-ui/core/IconButton';
import Tooltip from '@material-ui/core/Tooltip';
import AddIcon from '@material-ui/icons/Add';
import ViewStreamTwoToneIcon from '@material-ui/icons/ViewStreamTwoTone';
import ViewAgendaTwoToneIcon from '@material-ui/icons/ViewAgendaTwoTone';

const useToolbarStyles = makeStyles((theme) => ({
    root: {
        paddingLeft: theme.spacing(2),
        paddingRight: theme.spacing(1),
    },
    highlight: theme.palette.type === 'light'
        ? {
            color: theme.palette.secondary.main,
            backgroundColor: lighten(theme.palette.secondary.light, 0.85),
        }
        : {
            color: theme.palette.text.primary,
            backgroundColor: theme.palette.secondary.dark,
        },
    title: {
        flex: '1 1 100%',
    },
}));

const EnhancedTableToolbar = (props) => {
    const classes = useToolbarStyles();
    const { numSelected, title, allowAdding,
        //allowEdit, allowDelete, 
        allowFilter,
        onAddClick,
        onDenseToggle
    } = props;

    const [dense, setDense] = useState(props.dense || false);

    const handlerAdding = (e) => {
        onAddClick(e);
    }
    const toggleDense = (e) => {
        setDense(!dense)
        onDenseToggle(!dense);
    }
    return (
        <Toolbar
            className={clsx(classes.root, {
                [classes.highlight]: numSelected > 1 //&& (allowEdit || allowDelete)
            })}
        >
            {numSelected > 1 //&& (allowEdit || allowDelete) 
                ? (
                    <><div className={classes.title}>
                        <Typography color="inherit" variant="h6" component="div" id="tableTitle">
                            {title}
                        </Typography>
                        <Typography color="inherit" variant="subtitle1" component="div">
                            {numSelected} selected
                    </Typography>
                    </div>
                    </>)
                : (
                    <Typography className={classes.title} color="primary" variant="h6" component="div" id="tableTitle">
                        {title}
                    </Typography>
                )
            }

            {allowAdding ?
                <Tooltip title="Add">
                    <span>
                        <IconButton aria-label="add" onClick={handlerAdding}>
                            <AddIcon />
                        </IconButton>
                    </span>
                </Tooltip> : null}

            <Tooltip title="Toggle dense">
                <span>
                    <IconButton aria-label="add" onClick={toggleDense}>
                        {dense
                            ? <ViewAgendaTwoToneIcon />
                            : <ViewStreamTwoToneIcon />}
                    </IconButton>
                </span>
            </Tooltip>
        </Toolbar>
    );
};

EnhancedTableToolbar.propTypes = {
    numSelected: PropTypes.number.isRequired,
    title: PropTypes.string.isRequired
};

export default EnhancedTableToolbar
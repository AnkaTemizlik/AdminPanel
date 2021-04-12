import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableRow from '@material-ui/core/TableRow';
import IconButton from '@material-ui/core/IconButton';
import Tooltip from '@material-ui/core/Tooltip';
import FormControl from '@material-ui/core/FormControl';
import CheckIcon from '@material-ui/icons/Check';
import CancelRoundedIcon from '@material-ui/icons/Cancel';
import InfoIcon from '@material-ui/icons/InfoTwoTone';
import Box from '@material-ui/core/Box';
import TextField from '@material-ui/core/TextField';


function NewRowForm(props) {
    const { dense, keyFieldName, onCancel, onComplete, columns } = props
    const [newRow, setNewRow] = useState({})

    const handleCancel = () => {
        onCancel()
    }
    const handleCopmlete = () => {
        onComplete(newRow, () => {
            
        })
    }
    const handleChange = (value, id) => {
        setNewRow({
            ...newRow,
            [id]: value
        })
    }
    return <TableBody>
        <TableRow>
            {/* <TableCell style={{ borderBottom: "0", paddingBottom: 0 }}></TableCell> */}
            <TableCell colSpan="6" style={{ borderBottom: "0", paddingBottom: 0 }}>
                <Box display="flex" alignItems="center">
                    <Tooltip title="Gerekli alanları doldurun ve kaydedin.">
                        <span>
                            <IconButton size={dense ? "small" : null} >
                                <InfoIcon style={{ color: "#03A9F4" }} />
                            </IconButton>
                        </span>
                    </Tooltip>
                    <Box flexGrow="1"></Box>
                    <Tooltip title="İptal">
                        <span>
                            <IconButton onClick={handleCancel} size={dense ? "small" : null}>
                                <CancelRoundedIcon />
                            </IconButton>
                        </span>
                    </Tooltip>
                    <Tooltip title="Kaydet">
                        <span>
                            <IconButton onClick={handleCopmlete} size={dense ? "small" : null}>
                                <CheckIcon />
                            </IconButton>
                        </span>
                    </Tooltip>
                </Box>
            </TableCell>
        </TableRow>
        <TableRow hover key={0}>
            <TableCell />
            {columns.map((col, i) => {
                if (col.id == keyFieldName)
                    return <TableCell key={i} />
                return (
                    <TableCell key={i} >
                        <FormControl >
                            <TextField
                                autoFocus
                                required
                                fullWidth
                                color="secondary"
                                variant="outlined"
                                size="small"
                                placeholder={col.label}
                                type={col.numeric ? "number" : "text"}
                                onChange={(e) => handleChange(e.target.value, col.id)}
                            />
                        </FormControl>
                    </TableCell>
                );
            })}
        </TableRow>
    </TableBody>
}

export default NewRowForm
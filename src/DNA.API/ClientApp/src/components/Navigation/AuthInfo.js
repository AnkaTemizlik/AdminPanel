import React from 'react'
import PropTypes from 'prop-types'
import { Box, Avatar, Typography } from '@material-ui/core'

const AuthInfo = ({ user }) => {
    
    return <Box style={{
        padding: 16,
        transition: '0.8s'
    }} display="flex" justifyContent="center" alignItems="center" flexWrap="wrap" flexDirection="column">

        <Avatar style={{
            width: 60,
            height: 60,
            transition: '0.3s',
            marginBottom: 24,
            //marginTop: 24,
        }} />
        {user
            ? <>
                <Typography variant='h6' noWrap>{`${user.FullName}`}</Typography>
                <Typography color='textSecondary' gutterBottom noWrap>{`${user.Email}`}</Typography>
            </> : null}
    </Box>
}
AuthInfo.propTypes = {
    user: PropTypes.object
};
export default AuthInfo
import React from 'react'
import { Box, Typography, Link, Grid, Divider } from '@material-ui/core'
import { Trans } from '../../../store/i18next'

const ContactMenu = ({ menu }) => {
	let contactMenu = menu.additionalInfo
	return <Grid container spacing={2}>
		<Grid item xs={12}>
			<Box p={2}>
				<Typography color={"inherit"} variant="h5">
					<Trans>{menu.label}</Trans>
				</Typography>
			</Box>
		</Grid>

		<Grid item xs={12} sm={6} md={12}>

			<Box pl={2} >
				{/* <Link component={Link}
                    href={contactMenu.companyWebsiteUrl}
                    target={'_blank'}
                    rel="noopener"
                    style={{ display: "block", marginBottom: "8px" }}>
                    <BrandLogo hideIcon /> 
                </Link> */}

				<Typography variant="body1" gutterBottom>
					<Link href={contactMenu.companyWebsiteUrl}
						target={'_blank'}
						rel="noopener">
						{contactMenu.companyWebsite}</Link>
				</Typography>
				<Divider />
				<Typography variant="body1" gutterBottom >{contactMenu.companyName}</Typography>
			</Box>
		</Grid>
		<Grid item xs={12} sm={6} md={12}>
			<Box pl={2}>

				{/* Adres */}
				{contactMenu.address.map((address, i) => {
					return <Typography key={i} variant="body2">{address}</Typography>
				})}
				<Divider />
				{contactMenu.phoneNumber &&
					<Typography color={"inherit"} variant="body1">
						{`Telefon: `}
						<Typography component={Link} href={`tel:${contactMenu.phoneNumber}`} >{contactMenu.phoneNumber}</Typography>
					</Typography>}
			</Box>


		</Grid>
	</Grid >

}

export default ContactMenu
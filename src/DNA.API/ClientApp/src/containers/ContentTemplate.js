import React from 'react'
import { Container, Typography } from '@material-ui/core'

const ContentTemplate = ({ title, h1, h2, h3, h4, h5, h6 }) => {
    return <Container maxWidth="lg">
        <Typography variant="h1" gutterBottom>{title}</Typography>
        <Typography gutterBottom> Morbi sodales nulla at augue semper efficitur</Typography>
        <Typography variant="h2" gutterBottom>{h2 || "Vestibulum accumsan"}</Typography>
        <Typography variant="body1" gutterBottom>Duis body1 condimentum condimentum ultricies. Donec viverra ipsum nec lacus semper, ac bibendum odio consectetur. Suspendisse fermentum felis a ligula finibus, in mollis lectus tempor.</Typography>
        <Typography variant="h3" gutterBottom>{h3 || "Lorem ipsum"}</Typography>
        <Typography gutterBottom>
            Lorem body2 ipsum dolor sit amet, consectetur adipiscing elit.
        </Typography>
        <ol>
            <li>Nulla sit amet bibendum nisl. </li>
            <li>Sed elit turpis, tempus at accumsan eu, tincidunt nec felis. </li>
            <li>Nam facilisis felis nisl, nec porta elit tincidunt a. </li>
            <li>Aenean congue libero urna, nec imperdiet tortor laoreet vel. </li>
        </ol>

        <Typography gutterBottom>Aenean maximus velit elementum, sollicitudin magna vel, finibus diam. Nunc fringilla, tortor dignissim egestas sagittis, libero quam rutrum tellus, in imperdiet eros velit vitae sem. </Typography>
        <Typography gutterBottom>Interdum et malesuada fames ac ante ipsum primis in faucibus. Donec dui ex, pellentesque vehicula nulla congue, placerat vulputate risus. Pellentesque quis suscipit lectus, ac vestibulum neque. </Typography>
        <Typography variant="body2" gutterBottom>Sample <b>body2</b>. Donec orci felis, condimentum et bibendum vitae, gravida at sapien. Vestibulum cursus ante sed porttitor dapibus. Etiam dapibus mauris in tempus feugiat. Sed vitae lectus mi. Morbi nec accumsan mi. Nunc eget nibh vel quam gravida hendrerit.</Typography>

        <Typography variant="h4" gutterBottom>{h4 || "Dolor sit amet"}</Typography>
        <Typography gutterBottom>
            Vestibulum accumsan augue eu lorem suscipit, ac consectetur urna tempor. Praesent in mauris eros. Suspendisse potenti. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse aliquet non quam vitae ultricies. Pellentesque a dui a magna feugiat congue eu id tortor. Ut auctor turpis ac massa luctus, eget scelerisque purus scelerisque. Nullam sit amet quam dolor. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Etiam scelerisque, felis et porta accumsan, nibh dui gravida purus, eu dignissim turpis arcu non est. Donec eleifend nisl nec lectus interdum malesuada. Donec nibh eros, elementum ac nisi at, condimentum rutrum nisl. Curabitur pulvinar tempor feugiat.
        </Typography>
        <Typography variant="h5" gutterBottom>{h5 || "Pellentesque ante lacus, eu ornare"}</Typography>
        <Typography gutterBottom>
            Pellentesque ante lacus, eu ornare mi pretium a. Aenean elementum blandit iaculis. Curabitur non metus sapien. Nulla sit amet lectus ut ipsum sagittis tristique. In ullamcorper sapien eget nunc dignissim imperdiet eu at enim. Praesent in sollicitudin enim. Nullam rutrum facilisis vestibulum. Aliquam quis porta augue. Fusce vehicula leo ac pretium vulputate. Donec dignissim laoreet ex.
        </Typography>
        <Typography variant="h6" gutterBottom>{h6 || "Duis condimentum condimentum ultricies"}</Typography>
        <Typography gutterBottom>
            Aliquam erat volutpat. Duis condimentum condimentum ultricies. Donec viverra ipsum nec lacus semper, ac bibendum odio consectetur. Suspendisse fermentum felis a ligula finibus, in mollis lectus tempor. Donec eu magna eros. Duis id porta sem. Aenean euismod congue justo ac bibendum. Maecenas vitae nunc laoreet, dignissim tortor vitae, laoreet ligula. Quisque condimentum nec risus ac lacinia. Nullam vel lectus in velit vulputate fermentum. Sed gravida auctor mi, sit amet lacinia quam gravida eget. Curabitur ante tortor, eleifend sed scelerisque pellentesque, fringilla nec purus. Praesent iaculis commodo eros, vitae pellentesque ligula.
        </Typography>

        <Typography variant="h1" gutterBottom >h1.Heading</Typography>
        <Typography variant="h2" gutterBottom>            h2.Heading                </Typography>
        <Typography variant="h3" gutterBottom>            h3.Heading                </Typography>
        <Typography variant="h4" gutterBottom>            h4.Heading                </Typography>
        <Typography variant="h5" gutterBottom>            h5.Heading                </Typography>
        <Typography variant="h6" gutterBottom>            h6. Heading                </Typography>
        <Typography variant="subtitle1" gutterBottom>
            <b>subtitle1</b>. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Quos blanditiis tenetur
                </Typography>
        <Typography variant="subtitle2" gutterBottom>
            <b>subtitle2</b>. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Quos blanditiis tenetur
                </Typography>
        <Typography variant="body1" gutterBottom>
            <b>body1</b>. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Quos blanditiis tenetur
                unde suscipit, quam beatae rerum inventore consectetur, neque doloribus, cupiditate numquam
                dignissimos laborum fugiat deleniti? Eum quasi quidem quibusdam.
                </Typography>
        <Typography variant="body2" gutterBottom>
            <b>body2</b>. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Quos blanditiis tenetur
                unde suscipit, quam beatae rerum inventore consectetur, neque doloribus, cupiditate numquam
                dignissimos laborum fugiat deleniti? Eum quasi quidem quibusdam.
                </Typography>
        <Typography variant="button" display="block" gutterBottom>
            button text
                </Typography>
        <Typography variant="caption" display="block" gutterBottom>
            caption text
                </Typography>
        <Typography variant="overline" display="block" gutterBottom>
            overline text
                </Typography>
    </Container>
}

export default ContentTemplate
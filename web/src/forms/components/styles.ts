import styled from '@emotion/styled';
import { DialogContent } from '@mui/material';
import { Form } from 'formik';

export const StyledForm = styled(Form)({
	display: 'flex',
	flexDirection: 'column',
	justifyContent: 'center',
	width: '100%',

	'& p.Mui-error': {
		position: 'absolute',
		top: '95%',
	},
});

export const StyledDialogContent = styled(DialogContent)(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	justifyContent: 'center',
	width: '100%',
	padding: 0,
	overflow: 'unset',
	gap: theme.spacing(2),
}));

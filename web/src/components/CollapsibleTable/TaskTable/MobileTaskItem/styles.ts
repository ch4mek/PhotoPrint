import { styled } from '@mui/material';

const PADDING_LEFT_SPACE = 48;

export const MobileTaskItemWrapper = styled('span')(({ theme }) => ({
	width: '100%',
	display: 'flex',
	padding: `${theme.spaces.s}px`,
	paddingLeft: `${PADDING_LEFT_SPACE}px`,
	flexDirection: 'column',
}));

export const TableTitleContainer = styled('span')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	paddingTop: theme.spacing(1),
}));

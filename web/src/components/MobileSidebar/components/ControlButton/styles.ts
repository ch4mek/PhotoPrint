import styled from '@emotion/styled';
import { Button } from '@mms/mms-ui-library/components/Button';

import { IconSize } from '@/constants/iconSizes';

export const ControlButtonWrapper = styled(Button)(({ theme }) => ({
	display: 'flex',
	justifyContent: 'center',
	alignContent: 'center',

	backgroundColor: theme.palette['background-white'],
	color: theme.palette['text-black'],
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	width: IconSize.XL,
	height: IconSize.XL,
	padding: 0,
	minWidth: 'auto',

	'&:hover': {
		borderColor: theme.palette['graphics-primary-2'],
	},

	'& svg, & span': {
		width: IconSize.S,
		height: IconSize.S,
	},

	'&::before': {
		content: 'none',
	},
}));

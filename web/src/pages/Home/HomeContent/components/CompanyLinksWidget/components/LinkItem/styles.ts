import styled from '@emotion/styled';
import { lightTheme } from '@mms/mms-ui-library/theme';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { IconSize } from '@/constants/iconSizes';
import { zIndexes } from '@/constants/zIndexes';

const LINK_HEIGHT = 54;
const LINK_WIDTH = 63;
const ICON_BACKGROUND_SIZE = 25;

export const LinkItemWrapper = styled('a')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'center',
	justifyContent: 'space-between',
	gap: theme.spaces.xs,

	borderRadius: BorderRadiuses.m,
	textDecoration: 'none',
	color: theme.palette['text-black'],
	width: LINK_WIDTH,
	height: LINK_HEIGHT,
}));

export const LinkIconWrapper = styled('div')({
	width: IconSize.L,
	height: IconSize.L,
	position: 'relative',

	'&::after': {
		content: '""',
		position: 'absolute',
		top: '50%',
		left: '50%',
		transform: 'translate(-50%, -50%)',
		zIndex: 0,
		width: ICON_BACKGROUND_SIZE,
		height: ICON_BACKGROUND_SIZE,
		backgroundColor: lightTheme.palette['background-white'],
	},

	'> svg': {
		position: 'relative',
		zIndex: zIndexes.headlingPlateEditor,
	},
});
